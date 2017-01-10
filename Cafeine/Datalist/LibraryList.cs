using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;

namespace Cafeine.Datalist
{

    public class UserItemCollection
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int My_score { get; set; }
        public string Imgurl { get; set; }
        public int Totalepisodes { get; set; }
        public Animestatus My_status { get; set; }
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
        public static async Task<ObservableCollection<UserItemCollection>> querydata(int category)
        {
            ObservableCollection<UserItemCollection> userlibrary = new ObservableCollection<UserItemCollection>();
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
                            userlibrary.Add(new UserItemCollection
                            {
                                Id = (int)item.Element("series_animedb_id"),
                                Title = item.Element("series_title").Value,
                                Imgurl = item.Element("series_image").Value,
                                My_score = (int)item.Element("my_score"),
                                My_status = (Animestatus)(int)item.Element("my_status")
                            });
                        }
                        break;
                    }
            }
            return userlibrary;
        }
    }
}