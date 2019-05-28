using Cafeine.Models.Enums;
using Cafeine.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Cafeine.Models
{
    /// <summary>
    /// For offline paths
    /// </summary>
    public sealed class OfflineItem 
    {
        /// <summary>
        /// Database ID. Not for use.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Accepted id for its service. <para /> 
        /// Use this one for identification or to get item.
        /// </summary>
        public int ServiceID { get; set; }
        /// <summary>
        /// Accepted id for all known services. <para />
        /// Use this one for identification or to get item.
        /// </summary>
        public int MalID { get; set; }
        /// <summary>
        /// Saved folder path.
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// Saved episodes
        /// </summary>
        public List<Episode> Episodes { get; set; }
    }

    /// <summary>
    /// ServiceItem consists entirely for online items <para/>
    /// It always get updated based on useritem.
    /// </summary>
    public class ServiceItem
    {
        public override bool Equals(object obj)
        {
            if(obj is UserItem)
            {
                return this.ServiceID == (obj as UserItem).ServiceID;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public ServiceType Service { get; set; }

        /// <summary>
        /// Service ID. Intended for database.
        /// </summary>
        public int ServiceID { get; set; }

        public int MalID { get; set; }

        public MediaTypeEnum MediaType { get; set; }

        //[MAL ONLY] due to MAL being weird, data fetched from Anilist
        public MediaFormatEnum ShowType { get; set; }

        public string Title { get; set; }

        public Uri CoverImageUri { get; set; }
        /// <summary>
        /// Cover Image converted to bytes.
        /// </summary>
        public byte[] CoverImage { get; set; }
        /// <summary>
        /// Item's description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Item's score
        /// </summary>
        public Nullable<double> AverageScore { get; set; }
        /// <summary>
        /// Item's season
        /// </summary>
        public Nullable<SeasonsEnum> Season { get; set; }
        /// <summary>
        /// Item's status
        /// </summary>
        public Nullable<int> ItemStatus { get; set; }
        /// <summary>
        /// Item's total episodes.
        /// </summary>
        public Nullable<int> Episodes_Chapters { get; set; }
        /// <summary>
        /// Item's year aired
        /// </summary>
        public Nullable<int> SeriesStart { get; set; }

        public UserItem UserItem { get; set; }

        public string GetItemSeasonYear() => Season.HasValue ? $"{Seasons.Seasons_int2string[(int)Season.Value]} {SeriesStart}" : "";

        public async Task PopulateMoreDetails(IService service)
        {
            await service.GetItemDetails(this);
        }
    }
    public class UserItem
    {
        /// <summary>
        /// Service ID. Intended for database.
        /// </summary>
        public int ServiceID { get; set; }
        /// <summary>
        /// User's score.
        /// </summary>
        public Nullable<double> UserScore { get; set; }
        /// <summary>
        /// User's status.
        /// </summary>
        public Nullable<int> UserStatus { get; set; }
        /// <summary>
        /// User's total episodes/chapters watched/read.
        /// </summary>
        public Nullable<int> Watched_Read { get; set; }
        /// <summary>
        /// Intended only if the service use an overengineered method to identify user's item.
        /// </summary>
        public object AdditionalInfo;

        public string GetUserStatus() => UserStatus.HasValue ? StatusEnum.UserStatus_Int2Str[UserStatus.Value] : "";
    }

    public class EpisodeItem
    {
        /// <summary>
        /// Service ID. Intended for database.
        /// </summary>
        public ServiceType ServiceType { get; set; }
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
}
