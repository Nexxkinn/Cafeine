using Newtonsoft.Json;

namespace Cafeine.Shared.Models
{
    /// <summary>
    /// Tag-based system that enabled custom tag for better searching <br/>
    /// Considered to replace the old list-based system on v2 release
    /// </summary>
    /// //TODO: Replace database with this new system.
    public class Tag
    {
        public string Key   { get; }
        public string Value { get; }
        public TagType Type { get; }
        public bool KeyOnly { get; }

        [JsonConstructor]
        public Tag(string Key,string Value,TagType Type = TagType.SYSTEM)
        {
            this.Key = Key;
            this.Value = Value;
            this.Type = Type;
            this.KeyOnly = string.IsNullOrWhiteSpace(Value);
        }
    }

    public enum TagType
    {
        SYSTEM,
        CUSTOM
    }

    public static class DefaultTagConst
    {
        public static readonly string[] KeyTag = new string[]
        {
            "status",
            "localmedia",
            "type",
            "media"
        };

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
