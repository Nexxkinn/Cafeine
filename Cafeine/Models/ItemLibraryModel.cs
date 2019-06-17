using Cafeine.Models.Enums;
using Cafeine.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// Accepted ID for all known services. <para />
        /// Use this one for identification or to get item.
        /// </summary>
        public int MalID { get; set; }
        /// <summary>
        /// Accepted IDs for selected service.
        /// </summary>
        public Dictionary<ServiceType,int> ServiceID { get; set; }
        /// <summary>
        /// Saved folder path.
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// Saved episodes
        /// </summary>
        public List<ContentList> ContentList { get; set; }

        public void AddNewContentList(IList<ContentList> newlist)
        {
            foreach(var item in newlist)
            {
                ContentList.Add(item);
            }
            ContentList.OrderBy(x => x.Number);
        }

    }

    public class ContentListComparer : IEqualityComparer<ContentList>
        {
            public bool Equals(ContentList x, ContentList y)
            {
                if (x.Number == -1 || y.Number == -1) return x.Title == y.Title;
                return x.Number == y.Number;
            }

            public int GetHashCode(ContentList obj)
            {
                if (obj.Number == -1) return obj.Title.GetHashCode();
                return obj.Number.GetHashCode();
            }
        }

    /// <summary>
    /// ServiceItem consists entirely for online items <para/>
    /// It always get updated based on useritem.
    /// </summary>
    public class ServiceItem
    {
        public ServiceItem() { }
        public ServiceItem(ServiceItem item)
        {
            this.Service = item.Service;
            this.ServiceID = item.ServiceID;
            this.MalID = item.MalID;
            this.MediaType = item.MediaType;
            this.ShowType = item.ShowType;
            this.Title = item.Title;
            this.CoverImageUri = item.CoverImageUri;
            this.CoverImage = item.CoverImage;
            this.Description = item.Description;
            this.AverageScore = item.AverageScore;
            this.Season = item.Season;
            this.ItemStatus = item.ItemStatus;
            this.Episodes_Chapters = item.Episodes_Chapters;
            this.SeriesStart = item.SeriesStart;
            this.UserItem = item.UserItem;
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

        public async Task PopulateServiceItemDetails(IService service)
        {
            await service.PopulateServiceItemDetails(this);
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

    public class DetailsItem
    {
        public ServiceType servicetype { get; }
        public int ServiceID { get; }
        public int MalID { get; }

        public DetailsItem(ServiceItem item)
        {
            servicetype = item.Service;
            ServiceID = item.ServiceID;
            MalID = item.MalID;
            Description = item.Description;
        }

        public string Description { get; set; }
        public string Studio { get; set; }
        public string Author { get; set; }
    }

    public class SeriesContentList
    {
        /// <summary>
        /// Service ID. Intended for database.
        /// </summary>
        public ServiceType ServiceType { get; set; }
        /// <summary>
        /// listed items with saved configuration
        /// </summary>
        public IList<ContentList> Episodes { get; set; }
    }

    public class ContentList
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public Uri Thumbnail { get; set; }

        public ObservableCollection<Stream> StreamingServices { get; set; }
        
        public string FileName { get; set; }

        public string GenerateEpisodeNumber()
        {
            // for possible language expansion ?
            return (Number != -1) ? $"Episode {Number}" : "Extras";
        }
    }

    public class Stream
    {
        // using custom font
        public string Icon;
        public Uri Url;
    }
}
