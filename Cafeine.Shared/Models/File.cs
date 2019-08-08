using Cafeine.Shared.Models;

namespace Cafeine.Models
{
    public class MediaFile : IMediaList
    {
        public string[] Fingerprint;
        public string[] Unique_Numbers;
        public string FileName;

        public string Icon { get; set; } = "P";
        public string Source { get; set; } = "Play Offline";
    }
}
