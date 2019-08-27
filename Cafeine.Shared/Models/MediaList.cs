using Cafeine.Models.Enums;
using Cafeine.Shared.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Networking.NetworkOperators;

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
        public MediaListState State { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public Uri Thumbnail { get; set; }

        public List<MediaStream> Streams { get; set; }

        public List<MediaFile> Files { get; set; }

        public MediaList() { }

        public MediaList( List<MediaStream> streams,int number, string title, Uri thumbnail)
            : this(streams, null,number, title, thumbnail, MediaListState.ONLINE) { }

        public MediaList(List<MediaFile> files,int number, int index = 0) 
            : this(null, files,number, null, null, MediaListState.OFFLINE)
        {
            this.Title = files[index].FileName;
            this.Thumbnail = new Uri(files[index].Path);
        }

        public MediaList( List<MediaStream> streams, List<MediaFile> files,int number, string title, Uri thumbnail)
            : this (streams,files,number,title,thumbnail,MediaListState.BOTH)
        {

        }

        private MediaList(
            List<MediaStream> streams, 
            List<MediaFile> files, 
            int number,
            string title, 
            Uri thumbnail,
            MediaListState state)
        {
            this.Thumbnail = thumbnail;
            this.Title = title;
            this.Files = files;
            this.Streams = streams;
            this.Number = number;
            this.State = state;
        }

        public string GenerateEpisodeNumber()
        {
            // for possible language expansion ?
            return (Number != -1) ? $"Episode {Number}" : "Extras";
        }

        public enum MediaListState
        {
            ONLINE=0,
            OFFLINE=1,
            BOTH=2
        }
    }
}
