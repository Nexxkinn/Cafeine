using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class ItemDetailsViewModel : ViewModelBase
    {
        private OfflineItem _offline { get; set; }

        private ServiceItem _service { get; set; }

        public UserItem User {
            get => _service.UserItem;
            set {
                if(value != _service.UserItem)
                {
                    _service.UserItem = value;
                    RaisePropertyChanged(nameof(User));
                }
            }
        }

        public ServiceItem Service => _service;

        public ObservableCollection<Episode> Episodelist { get; private set; }
        
        #region mvvm setup properties

        public CafeineCommand PlusOneTotalSeenTextBlock { get; }

        public CafeineCommand EpisodeListsClicked { get; }

        public CafeineCommand EpisodeSettingsClicked { get; }

        public CafeineCommand UserItemDetailsClicked { get; }

        public AsyncReactiveCommand AddButtonClicked { get; }

        public AsyncReactiveCommand DeleteButtonClicked { get; }

        public CafeineProperty<double> ScorePlaceHolderRating { get; set; }

        public CafeineProperty<string> StatusTextBlock { get; }

        public CafeineProperty<string> ScoreTextBlock { get; }

        public CafeineProperty<string> DescriptionTextBlock { get; }

        public ReactiveProperty<bool> LoadItemDetails { get; }

        public CafeineProperty<Visibility> LoadEpisodesListConfiguration { get; }

        public CafeineProperty<Visibility> LoadEpisodeLists { get; }

        public CafeineProperty<bool> LoadEpisodeSettings { get; }

        public CafeineProperty<bool> LoadEpisodeNotFound { get; }

        public CafeineProperty<bool> ItemDetailsProgressRing { get; }

        public CafeineProperty<bool> SetAddButtonLoad { get; }

        public ReactiveProperty<bool> SetDeleteButtonLoad { get; }

        public ReactiveProperty<bool> IsPaneOpened { get; }

        public CafeineProperty<Brush> PaneBackground { get; }

        public CafeineProperty<StorageFile> ImageSource { get; }
        #endregion

        #region mvvm TwoWay properties
        public CafeineProperty<int> TotalSeenTextBox { get; }

        public CafeineProperty<int> UserStatusComboBox { get; }
        #endregion

        public ItemDetailsViewModel()
        {
            _offline = new OfflineItem();
            _service = new ServiceItem();

            navigationService = new NavigationService();
            PaneBackground = new CafeineProperty<Brush>(new SolidColorBrush(Windows.UI.Colors.Transparent));
            IsPaneOpened = new ReactiveProperty<bool>(false);
            IsPaneOpened.Subscribe((ipo) =>
            {
                PaneBackground.Value = ipo
                    ? Application.Current.Resources["SystemControlBackgroundChromeMediumLowBrush"] as SolidColorBrush
                    : new SolidColorBrush(Windows.UI.Colors.Transparent);
            });
            #region Set initial value

            ImageSource = new CafeineProperty<StorageFile>();
            ItemDetailsProgressRing = new CafeineProperty<bool>();
            LoadItemDetails = new ReactiveProperty<bool>(false);
            LoadItemDetails.Subscribe(x => ItemDetailsProgressRing.Value = !x);

            ScorePlaceHolderRating = new CafeineProperty<double>();
            StatusTextBlock = new CafeineProperty<string>();
            DescriptionTextBlock = new CafeineProperty<string>();
            ScoreTextBlock = new CafeineProperty<string>();

            Episodelist = new ObservableCollection<Episode>();
            LoadEpisodeLists = new CafeineProperty<Visibility>(Visibility.Collapsed);
            LoadEpisodeSettings = new CafeineProperty<bool>(false);
            LoadEpisodeNotFound = new CafeineProperty<bool>(false);
            LoadEpisodesListConfiguration = new CafeineProperty<Visibility>(Visibility.Visible);

            SetAddButtonLoad = new CafeineProperty<bool>(false);
            SetDeleteButtonLoad = new ReactiveProperty<bool>(true);
            SetDeleteButtonLoad.Subscribe(sdb =>
            {
                SetAddButtonLoad.Value = !sdb;
            });
            #endregion

            #region TwoWay Initial Value
            TotalSeenTextBox = new CafeineProperty<int>();
            UserStatusComboBox = new CafeineProperty<int>();
            #endregion

            PlusOneTotalSeenTextBlock = new CafeineCommand(()=> TotalSeenTextBox.Value += 1);

            EpisodeListsClicked = new CafeineCommand(() => 
            {
                if (_offline.Episodes.Count == 0)
                {
                    LoadEpisodeNotFound.Value = true;
                    LoadEpisodeLists.Value = Visibility.Collapsed;
                }
                else
                {
                    LoadEpisodeNotFound.Value = false;
                    LoadEpisodeLists.Value = Visibility.Visible;
                }
                LoadEpisodeSettings.Value = false;
                LoadEpisodesListConfiguration.Value = Visibility.Visible;
            });

            EpisodeSettingsClicked = new CafeineCommand(()=> 
            {
                LoadEpisodeLists.Value = Visibility.Collapsed;
                LoadEpisodeNotFound.Value = false;
                LoadEpisodeSettings.Value = true;
                LoadEpisodesListConfiguration.Value = Visibility.Collapsed;
            });

            UserItemDetailsClicked = new CafeineCommand(() => IsPaneOpened.Value = !IsPaneOpened.Value);

            AddButtonClicked = new AsyncReactiveCommand();
            AddButtonClicked.Subscribe(async _ =>
            {
                User = await Database.CreateUserItem(_service);
                SetDeleteButtonLoad.Value = true;
            });

            DeleteButtonClicked = new AsyncReactiveCommand();
            DeleteButtonClicked.Subscribe( async _ =>
            {
                MessageDialog popup = new MessageDialog($"You are going to delete {_service.Title} from your list.\n" +
                    $"Removing this item will also unlink your local directory in this item.\n" +
                    $"Do you really want to Remove it?", $"Remove this item?");
                popup.Commands.Add(new UICommand("Remove") { Id = 0 });
                popup.Commands.Add(new UICommand("Cancel") { Id = 1 });
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var result = await popup.ShowAsync();
                if ((int)result.Id == 0)
                {
                    await Database.DeleteItem(_service);
                    navigationService.GoBack();
                }
            });
        }

        public override async Task OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back) ItemLibraryService.Push(_service); 

            if (User != null &&
                (User?.UserStatus != UserStatusComboBox.Value || User?.Watched_Read != TotalSeenTextBox.Value))
            {
                User.Watched_Read = TotalSeenTextBox.Value;
                User.UserStatus = UserStatusComboBox.Value;
                await Database.UpdateItem(_service);
            }
            await base.OnNavigatedFrom(e);
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            await base.OnNavigatedTo(e);
            // Let UI Thread free
            await Task.Yield();
            (_offline, _service) = await ItemLibraryService.Pull();
            await LoadItem();
        }
        public async Task LoadItem()
        {
            try
            {
                RaisePropertyChanged(nameof(User));
                RaisePropertyChanged(nameof(Service));
                TotalSeenTextBox.Value = User?.Watched_Read ?? 0;
                UserStatusComboBox.Value = User?.UserStatus ?? 0;
                SetDeleteButtonLoad.Value = (User != null);

                StatusTextBlock.Value = $"{StatusEnum.Anilist_AnimeItemStatus[_service.ItemStatus.Value]} • {_service.SeriesStart} • TV";
                double score = _service.AverageScore ?? -1;
                ScorePlaceHolderRating.Value = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(score);
                ScoreTextBlock.Value = (_service.AverageScore.HasValue) ? $"( {_service.AverageScore.ToString()} )" : "( No score )";

                //Parallel task.
                List<Task> task = new List<Task>();
                task.Add(Task.Run(async () =>
                {
                    //if (Item.Details == null)
                    //{
                    //    Item.Details = await Database.ViewItemDetails(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    //}
                    //LoadItemDetails.Value = true;
                    //DescriptionTextBlock.Value = Item.Details.Description;

                }));

                // Task.Factory.StartNew is the only logical option
                // As it needs to set ThreadScheduler synchronous
                // Reference  : https://blogs.msdn.microsoft.com/pfxteam/2011/10/24/task-run-vs-task-factory-startnew/ 
                task.Add(Task.Factory.StartNew(async () =>
                {
                    //load episode list from database.
                    if (_offline == null) _offline =  Database.CreateOflineItem(_service);

                    if (_offline.Episodes != null)
                    {
                        Episodelist = new ObservableCollection<Episode>(_offline.Episodes);
                        RaisePropertyChanged(nameof(Episodelist));
                    }
                    else _offline.Episodes = new List<Episode>();
                    LoadEpisodeLists.Value = (_offline.Episodes.Count != 0) ? Visibility.Visible : Visibility.Collapsed;
                    await Task.Yield();
                    //load episode list from service.
                    List<Episode> EpisodeService = await Database.UpdateItemEpisodes(_service);
                    foreach (var episode in EpisodeService)
                    {
                        bool EpisodeAlreadyListed = _offline.Episodes.Exists(x => x.Title == episode.Title || x.OnlineThumbnail == episode.OnlineThumbnail);
                        if (!EpisodeAlreadyListed)
                        {
                            //Only add new items to avoid overwriting old items that contains file path.
                            _offline.Episodes.Add(episode);
                            Episodelist?.Add(episode);
                        }
                    }
                    if (_offline.Episodes.Count == 0)
                    {
                        LoadEpisodeNotFound.Value = true;
                        LoadEpisodeLists.Value = Visibility.Collapsed;
                    }
                    else
                    {
                        LoadEpisodeNotFound.Value = false;
                        LoadEpisodeLists.Value = Visibility.Visible;
                    }
                    RaisePropertyChanged("Episodelist");
                },
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.FromCurrentSynchronizationContext()).Unwrap());

                task.Add(Task.Run(async () => ImageSource.Value = await ImageCache.GetFromCacheAsync(_service.CoverImageUri.AbsoluteUri)));
                await Task.WhenAll(task);

                // Check if item source is from library or search query
                
                await Database.UpdateOfflineItem(_offline);
            }
            catch (Exception ex)
            {
                //assume an exception exists?
                //ItemScoreReadOnly.Value   = true;
                //ItemScore.Value           = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(-1, 0);

                //ItemStatusTextBlock.Value = $"An error occured. {ex.Message}";

                //Item.Details.Description = $"{ex.StackTrace}\n" +
                //    $"Screenshot this image and contact to the developer.";
            }
        }

    }
}
