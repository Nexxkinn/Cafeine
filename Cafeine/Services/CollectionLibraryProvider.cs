using System;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using Cafeine.Models;
using Cafeine.ViewModels;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
namespace Cafeine.Services
{
    class CollectionLibraryProvider
    {

        ///<summary>
        ///convert user's collection into a list for Gridview or Listview's ItemCollection. 
        ///AnimeorManga : 1 -> anime, 2-> manga
        ///</summary>
        public static async Task<ObservableCollection<CollectionLibraryViewModel>> QueryUserAnimeMangaListAsync(AnimeOrManga AnimeManga)
        {
            ObservableCollection<CollectionLibraryViewModel> Item = new ObservableCollection<CollectionLibraryViewModel>();
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            string OffFile = Path.Combine(OffFolder.Path, "RAW_1_" + AnimeManga + ".xml");
            XDocument ParseData = XDocument.Load(OffFile);
            switch (AnimeManga) //1 - anime  //2 - manga
            {
                case AnimeOrManga.anime:
                    {
                        var anime = ParseData.Descendants("anime");//.Where(x => (int)x.Element("my_status") == Status);
                        foreach (var item in anime)
                        {
                            Item.Add(new CollectionLibraryViewModel(new ItemProperties
                            {
                                Item_Id = (int)item.Element("series_animedb_id"),
                                Item_Title = item.Element("series_title").Value,
                                Item_Totalepisodes = (int)item.Element("series_episodes"),
                                Series_start = new string(item.Element("series_start").Value.Take(4).ToArray()),
                                Imgurl = item.Element("series_image").Value,

                                My_watch = (int)item.Element("my_watched_episodes"),
                                My_score = (int)item.Element("my_score"),
                                My_status = (int)item.Element("my_status")
                            }));
                        }
                        break;
                    }
                case AnimeOrManga.manga:
                    {
                        var manga = ParseData.Descendants("manga");
                        foreach (var item in manga)
                        {
                            Item.Add(new CollectionLibraryViewModel(new ItemProperties
                            {
                                Item_Id = (int)item.Element("series_mangadb_id"),
                                Item_Title = item.Element("series_title").Value,
                                Item_Totalepisodes = (int)item.Element("series_chapters"),
                                Series_start = new string(item.Element("series_start").Value.Take(4).ToArray()),
                                Imgurl = item.Element("series_image").Value,

                                My_watch = (int)item.Element("my_read_chapters"),
                                My_score = (int)item.Element("my_score"),
                                My_status = (int)item.Element("my_status")
                            }));
                        }
                        break;
                    }
            }
            return Item;
        }
        public static async Task UpdateItem(CollectionLibraryViewModel e, AnimeOrManga AnimeManga)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    new XmlSerializer(e.Itemproperty.GetType()).Serialize(writer, e.Itemproperty);
                    var xmlEncodedList = Encoding.UTF8.GetString(stream.ToArray());

                    var User = Logincredentials.getuser(1); //Grab username and password
                    var url = new Uri("https://myanimelist.net/api/" + AnimeManga.ToString() + "list/update/" + e.Itemproperty.Item_Id + ".xml?data=" + xmlEncodedList);

                    //GET
                    byte[] bytes = Encoding.UTF8.GetBytes(User.UserName + ":" + User.Password);
                    string LoginToBase64 = Convert.ToBase64String(bytes);

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Basic " + LoginToBase64);

                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }


        }
    }
}