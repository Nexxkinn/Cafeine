using Cafeine.Models.Enums;
using LiteDB;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
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
        [BsonId]
        public int Id { get; set; }

        //Ironically accepted ID for all services.
        public int MalID { get; set; }

        //Fetch from Anilist only
        //[MAL ONLY] since MyAnimeList doesn't have any feature to list episodes title
        public List<Episode> Episodes { get; set; }

        //Fetch from main service

        //[MAL ONLY] due to MAL being weird, data fetched from Anilist
        public MediaFormatEnum ShowType { get; set; }

        public MediaTypeEnum MediaType { get; set; }

        public Dictionary<string, UserItem> Service { get; set; }
    }

    public class UserItem
    {
        //Id based on main service
        public int ItemId { get; set; }

        //Title from main service
        public string Title { get; set; }

        public string CoverImageUri { get; set; }

        public double? AverageScore { get; set; }

        public int Status { get; set; }

        public int TotalEpisodes { get; set; }

        public int SeriesStart { get; set; }

        //public MediaTypeEnum Category { get; set; }
        [BsonIgnore]
        public StorageFile CoverImage { get; set; }

        public string Category { get; set; }

        public double UserScore { get; set; }

        public int UserStatus { get; set; }

        public int Total_Watched_Read { get; set; }

        //This instance is filled when user clicked the item.
        public ItemDetailsModel Details { get; set; }
    }

    public class Episode
    {
        public string Title { get; set; }

        public string Image { get; set; }

        public StorageFile file { get; set; }
    }

    public class ItemDetailsModel
    {
        public string Description { get; set; }
    }
}
