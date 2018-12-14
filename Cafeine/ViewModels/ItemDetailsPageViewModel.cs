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

        public ReactiveProperty<StorageFile> ImageSource { get; }

        public ObservableCollection<Episode> Episodelist { get; private set; }

        public ReactiveCommand EpisodeListsClicked { get; }

        public ReactiveCommand EpisodeSettingsClicked { get; }

        public AsyncReactiveCommand PageLoaded { get; }

        public ReactiveProperty<double> ScorePlaceHolderRating { get; set; }

        public ReactiveProperty<string> StatusTextBlock { get; }

        public ReactiveProperty<string> ScoreTextBlock { get; }

        public ReactiveProperty<string> DescriptionTextBlock { get; }

        public ReactiveProperty<bool> LoadEpisodeSettings { get; }

        public ReactiveProperty<bool> LoadEpisodeLists { get; }

        public ItemDetailsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            Item = new UserItem();
            ImageSource = new ReactiveProperty<StorageFile>();

            ScorePlaceHolderRating = new ReactiveProperty<double>();
            StatusTextBlock = new ReactiveProperty<string>();
            DescriptionTextBlock = new ReactiveProperty<string>();
            ScoreTextBlock = new ReactiveProperty<string>();

            Episodelist = new ObservableCollection<Episode>();
            LoadEpisodeLists = new ReactiveProperty<bool>(true);
            LoadEpisodeSettings = new ReactiveProperty<bool>(false);
            EpisodeListsClicked = new ReactiveCommand();
            EpisodeListsClicked.Subscribe(_ =>
            {
                LoadEpisodeLists.Value = true;
                LoadEpisodeSettings.Value = false;
            });
            EpisodeSettingsClicked = new ReactiveCommand();
            EpisodeSettingsClicked.Subscribe(_ =>
            {
                LoadEpisodeLists.Value = false;
                LoadEpisodeSettings.Value = true;
            });

            PageLoaded = new AsyncReactiveCommand();
            PageLoaded.Subscribe(async () =>
            {
                await LoadAsync();
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
            _eventAggregator.GetEvent<ItemDetailsID>().Unsubscribe(LoadItem);
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        private void LoadItem(ItemLibraryModel item)
        {
            ItemBase = item;
            Item = ItemBase.Service["default"];
        }

        //TODO : Handle Manga/novel item type.
        public async Task LoadAsync()
        {
            try
            {
                StatusTextBlock.Value = $"{StatusEnum.Anilist_AnimeItemStatus[Item.Status]} • {Item.SeriesStart} • TV";
                ScorePlaceHolderRating.Value = Item.AverageScore ?? -1;
                ScoreTextBlock.Value = (Item.AverageScore.HasValue) ? $"( {Item.AverageScore.ToString()} )" : "( No score )";

                //Parallel task.
                List<Task> task = new List<Task>();
                task.Add(Task.Run(async () =>
                {
                    if (Item.Details == null)
                    {
                        Item.Details = await Database.ViewItemDetails(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    }
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

                    //load episode list from service.
                    List<Episode> EpisodeService = await Database.UpdateItemEpisodes(Item, ItemBase.Id, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    foreach (var episode in EpisodeService)
                    {
                        bool EpisodeAlreadyListed = ItemBase.Episodes.Exists(x => x.Title == episode.Title || x.Image == episode.Image);
                        if (!EpisodeAlreadyListed)
                        {
                            //Only add new items to avoid overwriting old items that contains file path.
                            ItemBase.Episodes.Add(episode);
                            Episodelist?.Add(episode);
                        }
                    }
                    RaisePropertyChanged("Episodelist");
                },
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.FromCurrentSynchronizationContext()).Unwrap());

                task.Add(Task.Run(async () => ImageSource.Value = await ImageCache.GetFromCacheAsync(Item.CoverImageUri)));
                await Task.WhenAll(task);
                Database.EditItem(ItemBase);
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
