using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafeine.Shared.DummyData
{
    public class DummyContentList : MediaList
    {
        public DummyContentList() 
        {
            List<MediaFile> file = new List<MediaFile>()
            {
                new MediaFile { FileName = "wololo.mkv", Fingerprint = new string[1] , Unique_Numbers=new string[1] }
            };
            List<MediaStream> stream = new List<MediaStream>()
            {
                new MediaStream { Icon = "", Url = new Uri("https://www.crunchyroll.com/kaguya-sama-love-is-war/episode-1-i-will-make-you-invite-me-to-a-movie-kaguya-wants-to-be-stopped-kaguya-wants-it-781498")}
            };
            Title = "Generic Long Title that can contains almost two lines of title";
            Number = 100;
            Thumbnail = new Uri("https://img1.ak.crunchyroll.com/i/spire4-tmb/83a6a11ce7ffea4af4792326a035c4c71547911382_full.jpg");
            Files = file;
            Streams = stream;
        }
    }
}
