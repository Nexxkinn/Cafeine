using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views.Wizard;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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

        private DetailsItem _details { get; set; }

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

        public DetailsItem Details {
            get => _details;
            set {
                if (value != _details)
                {
                    _details = value;
                    RaisePropertyChanged(nameof(Details));
                }
            }
        }

        public ServiceItem Service => _service;

        public ObservableCollection<ContentList> Episodelist { get; private set; }
        
        #region mvvm setup properties

        public CafeineCommand PlusOneTotalSeenTextBlock { get; }

        public CafeineCommand EpisodeListsClicked { get; }

        public CafeineCommand EpisodeSettingsClicked { get; }

        public CafeineCommand UserItemDetailsClicked { get; }

        public AsyncReactiveCommand AddButtonClicked { get; }

        public AsyncReactiveCommand DeleteButtonClicked { get; }

        public ReactiveProperty<bool> SetDeleteButtonLoad { get; }

        public ReactiveProperty<bool> IsPaneOpened { get; }

        public ReactiveProperty<bool> LoadItemDetails { get; }

        #region properties
        public double ScorePlaceHolderRating {
            get => _scorePlaceHolderRating;
            set => Set(ref _scorePlaceHolderRating,value);
        }
        public string StatusTextBlock {
            get => _statusTextBlock;
            set => Set(ref _statusTextBlock, value);
        }
        public string ScoreTextBlock {
            get => _scoreTextBlock;
            set => Set(ref _scoreTextBlock, value);
        }
        public string DescriptionTextBlock {
            get => _descriptionTextBlock;
            set => Set(ref _descriptionTextBlock, value);
        }
        public Visibility LoadEpisodesListConfiguration {
            get => _loadEpisodesListConfiguration;
            set => Set(ref _loadEpisodesListConfiguration, value);
        }
        public Visibility LoadEpisodeLists {
            get => _loadEpisodeList;
            set => Set(ref _loadEpisodeList, value);
        }
        public bool LoadEpisodeSettings {
            get => _loadEpisodeSettings;
            set => Set(ref _loadEpisodeSettings, value);
        }
        public bool LoadEpisodeNotFound {
            get => _loadEpisodeNotFound;
            set => Set(ref _loadEpisodeNotFound, value);
        }
        public bool ItemDetailsProgressRing {
            get => _itemDetailsProgressRing;
            set => Set(ref _itemDetailsProgressRing, value);
        }
        public bool SetAddButtonLoad {
            get => _setAddButtonLoad;
            set => Set(ref _setAddButtonLoad, value);
        }
        public bool IsOfflineItemAvailable {
            get => _isOfflineItemAvailable;
            set => Set(ref _isOfflineItemAvailable, value);
        }
        public Brush PaneBackground {
            get => _paneBackground;
            set => Set(ref _paneBackground, value);
        }
        public StorageFile ImageSource {
            get => _imageSource;
            set => Set(ref _imageSource, value);
        }
        #endregion

        #region fields
        private double _scorePlaceHolderRating;
        private string _statusTextBlock;
        private string _scoreTextBlock;
        private string _descriptionTextBlock;
        private Visibility _loadEpisodesListConfiguration;
        private Visibility _loadEpisodeList;
        private bool _loadEpisodeSettings;
        private bool _loadEpisodeNotFound;
        private bool _itemDetailsProgressRing;
        private bool _setAddButtonLoad;
        private bool _isOfflineItemAvailable;
        private Brush _paneBackground;
        private StorageFile _imageSource;
        #endregion

        #endregion

        #region mvvm TwoWay properties
        public ReactiveProperty<int> TotalSeenTextBox { get; }

        public ReactiveProperty<int> UserStatusComboBox { get; }
        #endregion

        public ItemDetailsViewModel()
        {
            _offline = new OfflineItem();
            _service = new ServiceItem();

            navigationService = new NavigationService();
            PaneBackground = new SolidColorBrush(Windows.UI.Colors.Transparent);
            IsPaneOpened = new ReactiveProperty<bool>(false);
            IsPaneOpened.Subscribe((ipo) =>
            {
                PaneBackground = ipo
                    ? Application.Current.Resources["SystemControlBackgroundChromeMediumLowBrush"] as SolidColorBrush
                    : new SolidColorBrush(Windows.UI.Colors.Transparent);
            });
            #region Set initial value

            LoadItemDetails = new ReactiveProperty<bool>(false);
            LoadItemDetails.Subscribe(x => ItemDetailsProgressRing = !x);

            Episodelist = new ObservableCollection<ContentList>();
            LoadEpisodeLists = Visibility.Collapsed;
            LoadEpisodesListConfiguration = Visibility.Visible;

            SetDeleteButtonLoad = new ReactiveProperty<bool>(true);
            SetDeleteButtonLoad.Subscribe(sdb =>
            {
                SetAddButtonLoad = !sdb;
            });

            #endregion

            #region TwoWay Initial Value
            TotalSeenTextBox = new ReactiveProperty<int>();
            UserStatusComboBox = new ReactiveProperty<int>();
            #endregion

            PlusOneTotalSeenTextBlock = new CafeineCommand(()=> TotalSeenTextBox.Value += 1);

            EpisodeListsClicked = new CafeineCommand(() => 
            {
                if (Episodelist.Count == 0)
                {
                    LoadEpisodeNotFound = true;
                    LoadEpisodeLists = Visibility.Collapsed;
                }
                else
                {
                    LoadEpisodeNotFound = false;
                    LoadEpisodeLists = Visibility.Visible;
                }
                LoadEpisodeSettings = false;
                LoadEpisodesListConfiguration = Visibility.Visible;
            });

            EpisodeSettingsClicked = new CafeineCommand(()=> 
            {
                LoadEpisodeLists = Visibility.Collapsed;
                LoadEpisodeNotFound = false;
                LoadEpisodeSettings = true;
                LoadEpisodesListConfiguration = Visibility.Collapsed;
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
            Dispose();
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            await base.OnNavigatedTo(e);
            // Let UI Thread free
            await Task.Yield();
            (_offline, _service) = await ItemLibraryService.Pull();
            _ = LoadItem();
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

                StatusTextBlock = $"{StatusEnum.Anilist_AnimeItemStatus[_service.ItemStatus.Value]} • {_service.SeriesStart} • TV";
                double score = _service.AverageScore ?? -1;
                ScorePlaceHolderRating = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(score);
                ScoreTextBlock = (_service.AverageScore.HasValue) ? $"( {_service.AverageScore.ToString()} )" : "( No score )";

                //Parallel task.
                List<Task> task = new List<Task>()
                {
                    LoadServiceItemDetails(),
                    LoadEpisodeList(),
                    LoadServiceItemCoverImage()
                };
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

        public async Task LoadServiceItemDetails()
        {
            await Database.PopulateServiceItemDetails(Service);
            LoadItemDetails.Value = true;
            DescriptionTextBlock = Service.Description;
        }
        public async Task LoadEpisodeList()
        {
            //load episode list from database.
            if (_offline == null)
            {
                // online mode
                var onlinecontentlist = await Database.GetSeriesContentList(Service);
                Episodelist = new ObservableCollection<ContentList>(onlinecontentlist);
                RaisePropertyChanged("Episodelist");
            }
            else
            {
                // handle offline content then
                IsOfflineItemAvailable = true;
                Episodelist = new ObservableCollection<ContentList>(_offline.ContentList);
                RaisePropertyChanged("Episodelist");
                var onlinecontentlist = await Database.GetSeriesContentList(Service);
                if (onlinecontentlist.Count != _offline.ContentList.Count)
                {
                    var newcontentlist = onlinecontentlist.Except(Episodelist, new ContentListComparer());
                    foreach (var item in newcontentlist)
                    {
                        Episodelist.Add(item);
                    }
                    RaisePropertyChanged("Episodelist");
                    _offline?.AddNewContentList(newcontentlist.ToList());
                    await Database.UpdateOfflineItem(_offline);
                }
            }
            LoadEpisodeLists = (Episodelist.Count != 0) ? Visibility.Visible : Visibility.Collapsed;
            LoadEpisodeNotFound = (Episodelist.Count == 0);
        }
        public async Task LoadServiceItemCoverImage()
        {
            ImageSource = await ImageCache.GetFromCacheAsync(_service.CoverImageUri.AbsoluteUri);
        }
        public async void CreateOfflineItem()
        {
            OfflineItemWizard wizard = new OfflineItemWizard(Service, null, Episodelist);
            await wizard.ShowAsync();

            if (wizard.IsCanceled) return;
            //OfflineItem item = await Database.CreateOflineItem(Service, Episodelist);
        }

        public override void Dispose()
        {
            _service = null;
            _offline = null;
            _details = null;
            Episodelist = null;
            GC.Collect();
        }
    }
}
