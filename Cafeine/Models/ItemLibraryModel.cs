using Cafeine.Models.Enums;
using Cafeine.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace Cafeine.Models
{
    public sealed class ItemLibraryModel 
    {
        /// <summary>
        /// 0 - MyanimeList
        /// 1 - Anilist
        /// 2 - Kitsu
        /// 3 - ?
        /// </summary>
        //local database id, totally not related to any service's id.
        public int Id { get; set; }

        //Ironically accepted ID for all services.
        public int MalID { get; set; }
        
        public UserItem Item {
            get { return Service?["default"]; }
            set {
                if(Service != null)
                {
                    Service["default"] = value;
                }
            }
        }
        

        //Fetch from Anilist only
        //[MAL ONLY] since MyAnimeList doesn't have any feature to list episodes title
        public List<Episode> Episodes { get; set; }

        //[MAL ONLY] due to MAL being weird, data fetched from Anilist
        public MediaFormatEnum ShowType { get; set; }

        public MediaTypeEnum MediaType { get; set; }

        public Dictionary<string, UserItem> Service { get; set; }
    }

    public class UserItem 
    {
        //Get Id number from the service
        public int ItemId { get; set; }

        //Title from the service
        public string Title { get; set; }

        public string CoverImageUri { get; set; }

        public double? AverageScore { get; set; }

        public int Season { get; set; }

        public int Status { get; set; }

        public int TotalEpisodes { get; set; }

        public int SeriesStart { get; set; }

        //public MediaTypeEnum Category { get; set; }
        public StorageFile CoverImage { get; set; }

        public string Category { get; set; }

        public double UserScore { get; set; }

        public int UserStatus { get; set; }

        public int Total_Watched_Read { get; set; }

        //This instance is filled when user clicked the item.
        public ItemDetailsModel Details { get; set; }

        public string GetUserStatus() => StatusEnum.UserStatus_Int2Str[UserStatus];

        public string GetItemSeasonYear() => $"{Seasons.Seasons_int2string[Season]} {SeriesStart}";

        /// <summary>
        /// Intended only if the service use an overengineered method to identify user's item.
        /// </summary>
        public object AdditionalInfo;
    }

    public class Episode
    {
        /// <summary>
        /// Key are used for getting details about localfile.
        /// </summary>
        public string HashKey { get; set; }

        public StorageFile LocalFile;

        public string Title { get; set; }

        public string OnlineThumbnail;

        public string LocalFileName => LocalFile?.Name;

        public string UrlLink { get; set; }

        public async void Thumbnail_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            var file = await ImageCache.GetFromCacheAsync(OnlineThumbnail);
            image.Source = new BitmapImage { UriSource = new Uri(file.Path) };
        }

        public void ImageOpened(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            image.Opacity = 1;
        }
    }

    public class ItemDetailsModel
    {
        public string Description { get; set; }
    }
}
