using System;
using System.Collections.Generic;
using System.Text;

namespace Cafeine.Shared.Models
{
    public static class DefaultTagConst
    {
        public static readonly string[] StatusTagValue = new string[] 
        {
            "Watching",
            "completed",
            "on_hold",
            "dropped",
            "planned"
        };

        public static readonly string[] IsOfflineMediaAvailableTagValue = new string[]
        {
            "true",
            "false"
        };

        public static readonly string[] MediaTypeTagValue = new string[]
        {
            "manga",
            "anime",
            "novel"
        };

        public static readonly string[] MediaTagValue = new string[]
        {
            "any",
            "tv",
            "movie",
            "special",
            "ova",
            "ona",
            "music",
            "manga",
            "novel",
            "one_shot"
        };
    }
}
