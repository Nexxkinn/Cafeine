using Cafeine.Models.Enums;
using Cafeine.Services;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Cafeine.Models
{
    public sealed class ItemLibraryModel 
    {
        /// <summary>
        /// Database ID. Not for use.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Accepted id for all known services. 
        /// Use this one for identification or to get item.
        /// </summary>
        public int MalID { get; set; }
        
        /// <summary>
        /// Returns currently default item.
        /// </summary>
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
        /// <summary>
        /// Get Id number from the service
        /// </summary>
        public int ServiceId { get; set; }
        
        public string Title { get; set; }

        public string CoverImageUri { get; set; }

        public double? AverageScore { get; set; }

        public int Season { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// Item's total episodes.
        /// </summary>
        public int EpisodesChapters { get; set; }

        public int SeriesStart { get; set; }

        //public MediaTypeEnum Category { get; set; }
        public StorageFile CoverImage { get; set; }

        public string Category { get; set; }

        public double UserScore { get; set; }

        public int UserStatus { get; set; }

        /// <summary>
        /// User's total episodes/chapters watched/read.
        /// </summary>
        public int Watched_Read { get; set; }

        //This instance is filled when user clicked the item.
        public ItemDetailsModel Details { get; set; }

        public string GetUserStatus() => StatusEnum.UserStatus_Int2Str[UserStatus];

        public string GetItemSeasonYear() => $"{Seasons.Seasons_int2string[Season]} {SeriesStart}";

        /// <summary>
        /// Intended only if the service use an overengineered method to identify user's item.
        /// </summary>
        public object AdditionalInfo;
    }

    public class EpisodeItem
    {
        /// <summary>
        /// Database ID. Not for use.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// MAL ID to connect with library item.
        /// </summary>
        public int MAL_ID { get; set; }
        /// <summary>
        /// Saved folder path.
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// listed items with saved configuration
        /// </summary>
        public IList<Episode> Episodes { get; set; }
    }

    public class Episode
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public string OnlineThumbnail { get; set; }

        public string StreamingUrl { get; set; }
        
        public string FileName { get; set; }

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
