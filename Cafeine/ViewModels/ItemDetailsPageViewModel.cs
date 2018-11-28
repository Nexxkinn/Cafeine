using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.ViewModels
{
    public class ItemDetailsID : PubSubEvent<UserItem>
    {
    }

    public class ItemDetailsPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;

        private readonly IEventAggregator _eventAggregator;

        public UserItem Item { get; set; }

        public ReactiveProperty<StorageFile> ImageSource { get; }

        public ReactiveCollection<Episode> Episodelist { get; }

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
            Episodelist = new ReactiveCollection<Episode>();
            ImageSource = new ReactiveProperty<StorageFile>();

            ScorePlaceHolderRating = new ReactiveProperty<double>();
            StatusTextBlock = new ReactiveProperty<string>();
            DescriptionTextBlock = new ReactiveProperty<string>();
            ScoreTextBlock = new ReactiveProperty<string>();

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

        private void LoadItem(UserItem item)
        {
            Item = item;
        }

        //TODO : Handle Manga/novel item type.
        public async Task LoadAsync()
        {
            try
            {
                StatusTextBlock.Value = $"{StatusEnum.Anilist_AnimeItemStatus[Item.Status]} • {Item.SeriesStart} • TV";
                ScorePlaceHolderRating.Value = Item.AverageScore ?? -1;
                ScoreTextBlock.Value = (Item.AverageScore.HasValue) ? $"( {Item.AverageScore.ToString()} )" : "( No score )";

                List<Task> task = new List<Task>();
                task.Add(Task.Run(async () =>
                {
                    if (Item.Details == null)
                    {
                        Item.Details = await Database.ViewItemDetails(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    }
                    DescriptionTextBlock.Value = Item.Details.Description;

                }));

                task.Add(Task.Factory.StartNew(async () =>
                {
                    //load episode list from database.
                    List<Episode> EpisodeDB = Database.ViewItemEpisodes(Item) ?? Enumerable.Empty<Episode>().ToList();
                    foreach (var episode in EpisodeDB)
                    {
                        Episodelist?.Add(episode);
                    };

                    //load episode list from service.
                    List<Episode> EpisodeService = await Database.UpdateItemEpisodes(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    foreach (var episode in EpisodeService)
                    {
                        bool EpisodeAlreadyListed = EpisodeDB.Exists(x => x.Title == episode.Title || x.Image == episode.Image);
                        if (!EpisodeAlreadyListed)
                        {
                            Episodelist?.Add(episode);
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()));

                task.Add(Task.Run(async () => ImageSource.Value = await ImageCache.GetFromCacheAsync(Item.CoverImageUri)));

                await Task.WhenAll(task);
                Database.EditItem(Item);
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
