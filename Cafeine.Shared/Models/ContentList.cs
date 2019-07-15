using Cafeine.Models.Enums;
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

    public class Stream
    {
        // using custom font
        public string Icon;
        public Uri Url;
    }

    public class ContentList
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public Uri Thumbnail { get; set; }

        public List<Stream> StreamingServices { get; set; }

        public List<File> Files { get; set; }

        public string GenerateEpisodeNumber()
        {
            // for possible language expansion ?
            return (Number != -1) ? $"Episode {Number}" : "Extras";
        }
    }
}
