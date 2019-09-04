using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views.Wizard;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class ItemDetailsViewModel : ViewModelBase
    {
        private LocalItem _offline;
        private DetailsItem _details;

        public LocalItem Offline {
            get => _offline;
            set => Set(ref _offline, value);
        }
        public ServiceItem Service { get; private set; }
        public UserItem User {
            get => Service.UserItem;
            set {
                if(value != Service.UserItem)
                {
                    Service.UserItem = value;
                    RaisePropertyChanged(nameof(User));
                }
            }
        }
        public DetailsItem Details {
            get => _details;
            set => Set(ref _details, value);
        }
        public ObservableCollection<MediaList> Episodelist 
        {
            get => _episodelist;
            set => Set(ref _episodelist, value);
        }
        
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
        private ObservableCollection<MediaList> _episodelist;
        #endregion

        #endregion

        #region mvvm TwoWay properties
        public ReactiveProperty<int> TotalSeenTextBox { get; }

        public ReactiveProperty<int> UserStatusComboBox { get; }
        #endregion

        public ItemDetailsViewModel()
        {

            (Offline, Service) = ItemLibraryService.Pull();

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

            Episodelist = new ObservableCollection<MediaList>();
            LoadEpisodeLists = Visibility.Collapsed;
            LoadEpisodesListConfiguration = Visibility.Visible;

            SetDeleteButtonLoad = new ReactiveProperty<bool>(User != null);
            SetDeleteButtonLoad.Subscribe(sdb =>
            {
                SetAddButtonLoad = !sdb;
            });

            #endregion

            #region TwoWay Initial Value
            TotalSeenTextBox = new ReactiveProperty<int>( User?.Watched_Read ?? 0 );
            UserStatusComboBox = new ReactiveProperty<int>( User?.UserStatus ?? 0 );
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
                User = await Database.CreateUserItem(Service);
                SetDeleteButtonLoad.Value = true;
            });

            DeleteButtonClicked = new AsyncReactiveCommand();
            DeleteButtonClicked.Subscribe( async _ =>
            {
                MessageDialog popup = new MessageDialog($"You are going to delete {Service.Title} from your list.\n" +
                    $"Removing this item will also unlink your local directory in this item.\n" +
                    $"Do you really want to Remove it?", $"Remove this item?");
                popup.Commands.Add(new UICommand("Remove") { Id = 0 });
                popup.Commands.Add(new UICommand("Cancel") { Id = 1 });
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var result = await popup.ShowAsync();
                if ((int)result.Id == 0)
                {
                    await Database.DeleteItem(Service);
                    navigationService.GoBack();
                }
            });

            LoadItem();
        }

        public override async Task OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back) ItemLibraryService.Push(Service);

            if (User != null &&
                (User?.UserStatus != UserStatusComboBox.Value 
                || User?.Watched_Read != TotalSeenTextBox.Value ))
            {
                User.Watched_Read = TotalSeenTextBox.Value;
                User.UserStatus = UserStatusComboBox.Value;
                await Database.UpdateItem(Service);
            }

            await base.OnNavigatedFrom(e);

            Dispose();
        }

        public void LoadItem()
        {
            StatusTextBlock = $"{StatusEnum.Anilist_AnimeItemStatus[Service.ItemStatus.Value]} • {Service.SeriesStart} • TV";
            ScorePlaceHolderRating = ScoreFormatEnum.Anilist_ConvertToGlobalUnit( Service.AverageScore ?? -1 );
            ScoreTextBlock = (Service.AverageScore.HasValue) ? $"( {Service.AverageScore.ToString()} )" : "( No score )";

            //Parallel task.
            var tasks = new Task[]
            {
                LoadServiceItemDetails(),
                LoadEpisodeList(),
                LoadServiceItemCoverImage(),
            };
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
            try
            {
                if (Offline == null)
                {
                    // online mode
                    var onlinecontentlist = await Database.GetSeriesContentList(Service);
                    Episodelist = new ObservableCollection<MediaList>(onlinecontentlist);
                }
                else
                {
                    // handle offline content then
                    IsOfflineItemAvailable = true;
                    Episodelist = new ObservableCollection<MediaList>(_offline.MediaCollection);
                    // TODO: Handle a condition when the streaming service pull the list.
                    //       The app should update the streaming link.
                    var onlinecontentlist = await Database.GetSeriesContentList(Service);
                    if (onlinecontentlist.Count != _offline.MediaCollection.Count)
                    {
                        var newcontentlist = onlinecontentlist.Except(Episodelist, new MediaListComparer());
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
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }
        public async Task LoadServiceItemCoverImage()
        {
            ImageSource = await ImageCache.GetFromCacheAsync(Service.CoverImageUri.AbsoluteUri);
        }
        public async void CreateOfflineItem()
        {
            OfflineItemWizard wizard = new OfflineItemWizard(Service, null, Episodelist);
            await wizard.ShowAsync();

            if (wizard.IsCanceled) return;

            Offline = wizard.Result;
            Episodelist = new ObservableCollection<MediaList>(Offline.MediaCollection);

            IsOfflineItemAvailable = true;
        }

        public override void Dispose()
        {
            Service = null;
            Offline = null;
            Details = null;
            Episodelist = null;
            IsPaneOpened.Dispose();
            AddButtonClicked.Dispose();
            DeleteButtonClicked.Dispose();

            GC.Collect();
        }
    }
}
