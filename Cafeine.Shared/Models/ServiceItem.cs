using Cafeine.Models.Enums;
using Cafeine.Services;
using System;
using System.Threading.Tasks;

namespace Cafeine.Models
{
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

        public async Task PopulateServiceItemDetails(IApiService service)
        {
            await service.PopulateServiceItemDetails(this);
        }
    }
}
