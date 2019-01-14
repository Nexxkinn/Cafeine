using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Cafeine.ViewModels
{
    public class ItemDetailsID : PubSubEvent<ItemLibraryModel>
    {
    }

    public class ItemDetailsPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;

        private readonly IEventAggregator _eventAggregator;

        public UserItem Item { get; set; }

        private ItemLibraryModel ItemBase { get; set; }

        private List<ItemLibraryModel> OpenedItems;

        public ObservableCollection<Episode> Episodelist { get; private set; }
        
        #region mvvm setup properties
        public ReactiveCommand EpisodeListsClicked { get; }

        public ReactiveCommand EpisodeSettingsClicked { get; }

        public AsyncReactiveCommand PageLoaded { get; }

        public CafeineProperty<double> ScorePlaceHolderRating { get; set; }

        public CafeineProperty<string> StatusTextBlock { get; }

        public CafeineProperty<string> ScoreTextBlock { get; }

        public CafeineProperty<string> DescriptionTextBlock { get; }

        public ReactiveProperty<bool> LoadItemDetails { get; }

        public CafeineProperty<bool> LoadEpisodesListConfiguration { get; }

        public CafeineProperty<Visibility> LoadEpisodeLists { get; }

        public CafeineProperty<bool> LoadEpisodeSettings { get; }

        public CafeineProperty<bool> LoadEpisodeNotFound { get; }

        public CafeineProperty<bool> ItemDetailsProgressRing { get; }
        #endregion

        public ItemDetailsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            Item = new UserItem();
            OpenedItems = new List<ItemLibraryModel>();

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
            LoadEpisodesListConfiguration = new CafeineProperty<bool>(true);

            EpisodeListsClicked = new ReactiveCommand();
            EpisodeListsClicked.Subscribe(_ =>
            {
                if (ItemBase.Episodes.Count == 0)
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
                LoadEpisodesListConfiguration.Value = true;
            });

            EpisodeSettingsClicked = new ReactiveCommand();
            EpisodeSettingsClicked.Subscribe(_ =>
            {
                LoadEpisodeLists.Value = Visibility.Collapsed;
                LoadEpisodeNotFound.Value = false;
                LoadEpisodeSettings.Value = true;
                LoadEpisodesListConfiguration.Value = false;
            });

            _eventAggregator.GetEvent<ItemDetailsID>().Subscribe(LoadItem);

        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(3);
            base.OnNavigatedTo(e, viewModelState);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            if(e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back)
                _eventAggregator.GetEvent<ItemDetailsID>().Unsubscribe(LoadItem);
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        private void LoadItem(ItemLibraryModel item)
        {
            ItemBase = item;
            Item = ItemBase.Service["default"];
            RaisePropertyChanged(nameof(Item));
            Task.Factory.StartNew(async () => await LoadAsync(),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        //TODO : Handle Manga/novel item type.
        public async Task LoadAsync()
        {
            try
            {
                StatusTextBlock.Value = $"{StatusEnum.Anilist_AnimeItemStatus[Item.Status]} • {Item.SeriesStart} • TV";
                double score = Item.AverageScore ?? -1;
                ScorePlaceHolderRating.Value = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(score);
                ScoreTextBlock.Value = (Item.AverageScore.HasValue) ? $"( {Item.AverageScore.ToString()} )" : "( No score )";

                //Parallel task.
                List<Task> task = new List<Task>();
                task.Add(Task.Run(async () =>
                {
                    if (Item.Details == null)
                    {
                        Item.Details = await Database.ViewItemDetails(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    }
                    LoadItemDetails.Value = true;
                    DescriptionTextBlock.Value = Item.Details.Description;

                }));

                // Task.Factory.StartNew is the only logical option
                // As it needs to set ThreadScheduler synchronous
                // Reference  : https://blogs.msdn.microsoft.com/pfxteam/2011/10/24/task-run-vs-task-factory-startnew/ 
                task.Add(Task.Factory.StartNew(async () =>
                {
                    //load episode list from database.
                    if (ItemBase.Episodes != null)
                    {
                        Episodelist = new ObservableCollection<Episode>(ItemBase.Episodes);
                        RaisePropertyChanged(nameof(Episodelist));
                    }
                    else ItemBase.Episodes = new List<Episode>();
                    await Task.Delay(100);
                    LoadEpisodeLists.Value = (ItemBase.Episodes.Count != 0) ? Visibility.Visible : Visibility.Collapsed;
                    
                    //load episode list from service.
                    List<Episode> EpisodeService = await Database.UpdateItemEpisodes(Item, ItemBase.Id, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    foreach (var episode in EpisodeService)
                    {
                        bool EpisodeAlreadyListed = ItemBase.Episodes.Exists(x => x.Title == episode.Title || x.OnlineThumbnail == episode.OnlineThumbnail);
                        if (!EpisodeAlreadyListed)
                        {
                            //Only add new items to avoid overwriting old items that contains file path.
                            ItemBase.Episodes.Add(episode);
                            Episodelist?.Add(episode);
                        }
                    }
                    if(ItemBase.Episodes.Count == 0)
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
                await Task.WhenAll(task);

                // Check if item source is from library or search query
                if(ItemBase.Id != default(int)) Database.EditItem(ItemBase);
            }
            catch (Exception ex)
            {
                //assume an exception exists?
                //ItemScoreReadOnly.Value   = true;
                //ItemScore.Value           = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(-1, 0);

                //ItemStatusTextBlock.Value = $"An error occured. {ex.Message}";
                Item.Details.Description = $"{ex.StackTrace}\n" +
                    $"Screenshot this image and contact to the developer.";
            }
        }
    }
}
