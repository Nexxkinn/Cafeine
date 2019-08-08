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
        public IList<ContentList> Episodes { get; set; }
    }

    public class StreamService : IMediaList
    {
        /// <summary>
        /// Streaming Service Icon
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Streaming service name
        /// </summary>
        public string Source { get; set; }

        public Uri Url;
    }

    public class ContentList 
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public Uri Thumbnail { get; set; }

        public List<StreamService> StreamingServices { get; set; }

        public List<MediaFile> Files { get; set; }

        public string GenerateEpisodeNumber()
        {
            // for possible language expansion ?
            return (Number != -1) ? $"Episode {Number}" : "Extras";
        }
    }
}
