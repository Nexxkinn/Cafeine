using Cafeine.Models.Enums;
using Cafeine.Shared.Models;
using System;
using System.Collections.Generic;

namespace Cafeine.Models
{
    public class MediaStream : IMediaList
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

        public async void Clicked()
        {
            await Windows.System.Launcher.LaunchUriAsync(Url);
        }
    }
}
