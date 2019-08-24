using Cafeine.Shared.Models;
using System;
using Windows.Storage;

namespace Cafeine.Models
{
    public class MediaFile : IMediaList
    {
        public string[] Fingerprint;
        public string[] Unique_Numbers;
        public string Path;
        public string FileName;

        public string Icon { get; set; } = "P";
        public string Source { get; set; } = "Play Offline";

        public async void Clicked()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Path);
            await Windows.System.Launcher.LaunchFileAsync(file);
        }
    }
}
