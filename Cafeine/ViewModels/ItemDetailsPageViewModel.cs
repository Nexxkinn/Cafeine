using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;

namespace Cafeine.ViewModels
{
    public class ItemDetailsID : PubSubEvent<UserItem> { }
    public class ItemDetailsPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;


        public UserItem Item { get; set; }
        public ReactiveProperty<StorageFile> ImageSource { get; }
        public ReactiveCollection<Episode>   Episodelist { get; }
        
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

            Item        = new UserItem();
            Episodelist = new ReactiveCollection<Episode>();
            ImageSource = new ReactiveProperty<StorageFile>();

            ScorePlaceHolderRating = new ReactiveProperty<double>();
            StatusTextBlock        = new ReactiveProperty<string>();
            DescriptionTextBlock   = new ReactiveProperty<string>();
            ScoreTextBlock         = new ReactiveProperty<string>();

            LoadEpisodeLists    = new ReactiveProperty<bool>(true);
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
            PageLoaded.Subscribe(async ()=> {
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
                task.Add(Task.Run(async () => {
                    Item.Details = await Database.ViewItemDetails(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                    DescriptionTextBlock.Value = Item.Details.Description;
                }));

                task.Add(
                    Task.Factory.StartNew(async () =>
                    {
                        List<Episode> EpisodeDB = await Database.ViewItemEpisodes(Item, ServiceType.ANILIST, MediaTypeEnum.ANIME);
                        foreach (var episode in EpisodeDB)
                        {
                            Episodelist?.Add(episode);
                        }
                    },
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext())
                );
                task.Add(Task.Run(async () => ImageSource.Value = await ImageCache.GetFromCacheAsync(Item.CoverImageUri)));

                await Task.WhenAll(task);
                
                
                //Database.EditItem(item);
            }
            catch (Exception ex)
            {
                //assume an exception exists?
                //ItemScoreReadOnly.Value   = true;
                //ItemScore.Value           = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(-1, 0);

                //ItemStatusTextBlock.Value = $"An error occured. {ex.Message}";
                Item.Details.Description  = $"{ex.StackTrace}\n" +
                    $"Screenshot this image and contact to the developer.";
            }
        }
    }
}
