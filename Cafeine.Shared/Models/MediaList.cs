using Cafeine.Models.Enums;
using Cafeine.Shared.Models;
using System;
using System.Collections.Generic;

namespace Cafeine.Models
{

    public class SeriesContentList
    {
        /// <summary>
        /// Service ID. Intended for database.
        /// </summary>
        public ServiceType ServiceType { get; set; }
        /// <summary>
        /// listed items with saved configuration
        /// </summary>
        public IList<MediaList> Episodes { get; set; }
    }

    public class MediaList 
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public Uri Thumbnail { get; set; }

        public List<MediaStream> StreamingServices { get; set; }

        public List<MediaFile> Files { get; set; }

        public string GenerateEpisodeNumber()
        {
            // for possible language expansion ?
            return (Number != -1) ? $"Episode {Number}" : "Extras";
        }
    }
}
