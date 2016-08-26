using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using System.IO;
using System.Collections.ObjectModel;

namespace Cafeine.Datalist
{
    
    public class animelibrary
    {
        public int id { get; set; }
        public string Title { get; set; }
        public int my_score { get; set; }
        public string imgurl { get; set; }
        public int totalepisodes { get; set; }
        public Animestatus my_status { get; set; }
    }

    public enum Animestatus
    {
        Watching = 1,
        Completed = 2,
        OnHold = 3,
        Dropped = 4,
        PlannToWatch = 6,
        AllOrAiring = 7
    }
    class LibraryList
    {
        public static async Task<ObservableCollection<animelibrary>> querydata(int category)
        {
            ObservableCollection<animelibrary> userlibrary = new ObservableCollection<animelibrary>();
            string anime_or_manga = (category == 1) ? "anime" : "manga";
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            string OffFile = Path.Combine(OffFolder.Path, "RAW_" + anime_or_manga + ".xml");
            XDocument ParseData = XDocument.Load(OffFile);

            switch (category) //1 - anime  //2 - manga
            {
                case 1:
                    {
                        var anime = ParseData.Descendants("anime");
                        foreach (var item in anime)
                        {
                            userlibrary.Add(new animelibrary
                            {
                                my_status = (Animestatus)(int)item.Element("my_status"),
                                imgurl = item.Element("series_image").Value,
                                Title = item.Element("series_title").Value,
                                my_score = (int)item.Element("my_score")
                            });
                        }
                        break;
                    }
            }
            return userlibrary;
        }
    }
}