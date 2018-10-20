using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.ViewModels
{
    public class ItemDetailsID : PubSubEvent<int> { }
    public class ItemDetailsPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        public ReactiveCommand ItemDetailPageLoad { get; }

        public ItemLibraryModel   item { get; set; }
        public ReactiveProperty<StorageFile>ImageSource { get; }
        public ReactiveCollection<Episode>  Episodelist { get; }
        public ReactiveProperty<string>     Description { get; }
        public ReactiveProperty<string>     ItemStatusTextBlock;
        public ReactiveProperty<double>     ItemScore;
        public ReactiveProperty<string>     ItemScoreDetails;
        public ReactiveProperty<bool>       ItemScoreReadOnly;

        public ReactiveCommand EpisodeListsClicked { get; }
        public ReactiveCommand EpisodeSettingsClicked { get; }

        public ReactiveProperty<bool> LoadEpisodeSettings { get; }
        public ReactiveProperty<bool> LoadEpisodeLists { get; }

        public ItemDetailsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            item        = new ItemLibraryModel();
            Episodelist = new ReactiveCollection<Episode>();
            Description = new ReactiveProperty<string>();

            ImageSource = new ReactiveProperty<StorageFile>();
            ItemScore           = new ReactiveProperty<double>();
            ItemScoreDetails    = new ReactiveProperty<string>();
            ItemStatusTextBlock = new ReactiveProperty<string>();
            ItemScoreReadOnly   = new ReactiveProperty<bool>();

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

            ItemDetailPageLoad = new ReactiveCommand();
            ItemDetailPageLoad.Subscribe(async _ => await DoTheMath());
        }
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(3);
            _eventAggregator.GetEvent<ItemDetailsID>().Subscribe(x =>item = Database.ViewItem(x));
            base.OnNavigatedTo(e, viewModelState);
        }
        //TODO : Handle Manga/novel item type.
        public async Task DoTheMath()
        {
            try
            {
                ImageSource.Value = await ImageCache.GetFromCacheAsync(item.Service["default"].CoverImageUri);
                var selecteditem = item.Service["default"];

                ItemStatusTextBlock.Value = $"{StatusEnum.Anilist_AnimeItemStatus[selecteditem.Status]} • {selecteditem.SeriesStart} • TV";
                ItemScoreReadOnly.Value   = !selecteditem.AverageScore.HasValue;

                double score           = selecteditem.AverageScore ?? -1;
                ItemScoreDetails.Value = (selecteditem.AverageScore.HasValue) ? $"( {selecteditem.AverageScore.ToString()} )" : "( No score )";
                ItemScore.Value        = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(score, 0);

                if (selecteditem.Details == null)
                {
                    var itemdetail = await Database.ViewItemDetails<ItemDetailsModel>(selecteditem.ItemId, MediaTypeEnum.ANIME, ServiceType.ANILIST);
                    Description.Value = itemdetail.Description;
                    selecteditem.Details = itemdetail;
                }
                else Description.Value = selecteditem.Details.Description;

                //load from online service 
                if (item.Episodes == null)
                {
                    item.Episodes = await Database.ViewItemDetails<List<Episode>>(selecteditem.ItemId, MediaTypeEnum.ANIME, ServiceType.ANILIST);
                }

                //load episode list from database.
                foreach (var episode in item.Episodes) Episodelist?.Add(episode);

                //load episode updates;
                var updatedEpisodes = await Database.ViewItemDetails<List<Episode>>(selecteditem.ItemId, MediaTypeEnum.ANIME, ServiceType.ANILIST);
                
                foreach (var episode in item.Episodes)
                {
                    var offlineEpisodes = item.Episodes.Find(x => x.Title == episode.Title || x.Image == episode.Image);
                    if( offlineEpisodes == null)
                    {
                        //add & load item if no episodes found in the database.
                        item.Episodes.Add(episode);
                        Episodelist?.Add(episode);
                    }
                }

                Database.EditItem(item);
            }
            catch (Exception ex)
            {
                //assume an exception exists?
                ItemScoreReadOnly.Value   = true;
                ItemScore.Value           = ScoreFormatEnum.Anilist_ConvertToGlobalUnit(-1, 0);

                ItemStatusTextBlock.Value = $"An error occured. {ex.Message}";
                Description.Value         = $"{ex.StackTrace}\n" +
                    $"Screenshot this image and contact to the developer.";
            }
        }
    }
}
