using System;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using Cafeine.Models;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    class LibraryList
    {

        ///<summary>
        ///convert user's collection into a list for Gridview or Listview's ItemCollection. 
        ///AnimeorManga : 1 -> anime, 2-> manga
        ///</summary>
        public static async Task<ObservableCollection<ItemProperties>> QueryUserAnimeMangaListAsync(bool AnimeorManga)
        {
            ObservableCollection<ItemProperties> Item = new ObservableCollection<ItemProperties>();
            string anime_or_manga = (AnimeorManga) ? "anime" : "manga";
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            string OffFile = Path.Combine(OffFolder.Path, "RAW_" + anime_or_manga + ".xml");
            XDocument ParseData = XDocument.Load(OffFile);
            switch (AnimeorManga) //1 - anime  //2 - manga
            {
                case true:
                    {
                        var anime = ParseData.Descendants("anime");//.Where(x => (int)x.Element("my_status") == Status);
                        foreach (var item in anime)
                        {
                            Item.Add(new ItemProperties
                            {
                                Item_Id = (int)item.Element("series_animedb_id"),
                                Item_Title = item.Element("series_title").Value,
                                Item_Totalepisodes = (int)item.Element("series_episodes"),
                                Series_start = new string(item.Element("series_start").Value.Take(4).ToArray()),
                                Imgurl = item.Element("series_image").Value,

                                My_watch = (int)item.Element("my_watched_episodes"),
                                My_score = (int)item.Element("my_score"),
                                My_status = (int)item.Element("my_status")
                            });
                        }
                        break;
                    }
                case false:
                    {
                        var manga = ParseData.Descendants("manga");
                        foreach(var item in manga)
                        {
                            Item.Add(new ItemProperties
                            {
                                Item_Id = (int)item.Element("series_mangadb_id"),
                                Item_Title = item.Element("series_title").Value,
                                Item_Totalepisodes = (int)item.Element("series_chapters"),
                                Series_start = new string(item.Element("series_start").Value.Take(4).ToArray()),
                                Imgurl = item.Element("series_image").Value,

                                My_watch = (int)item.Element("my_read_chapters"),
                                My_score = (int)item.Element("my_score"),
                                My_status = (int)item.Element("my_status")
                            });
                        }
                        break;
                    }
                default:
                    break;
            }
            return Item;
        }
    }
}