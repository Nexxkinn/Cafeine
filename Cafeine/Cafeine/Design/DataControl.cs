using System;
using Cafeine.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Storage;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Cafeine.Design {
    class DataProvider {

        ///<summary>
        ///Grab user's data from the server and save it to the local file inside the app.
        /// 
        ///category : 1 -> anime , 2-> manga
        ///</summary>
        public static async Task GrabUserDatatoOffline(int service) {
            string username = Logincredentials.getuser(1).UserName;
            List<ItemModel> Item = new List<ItemModel>();

            ///currently only to serve MyAnimelist
            ///Considering to use NetJSON to get faster serializer and deserializer.
            var url = new Uri("https://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=anime&status=all");
            var url2 = new Uri("https://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=manga&status=all");
            string FetchData,FetchData2;
            using (var client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(url);
                HttpResponseMessage response2 = await client.GetAsync(url2);

                response.EnsureSuccessStatusCode();
                response2.EnsureSuccessStatusCode();

                FetchData = response.Content.ToString();
                FetchData2 = response2.Content.ToString();
            }
            
            //convert XML to JSON for good
            XDocument ParsedItemsAnime = XDocument.Parse(FetchData);
            var anime = ParsedItemsAnime.Descendants("anime");
            foreach (var item in anime) {
                Item.Add(new ItemModel {
                    Category = AnimeOrManga.anime,
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
            XDocument ParsedItemsManga = XDocument.Parse(FetchData2);
            var manga = ParsedItemsManga.Descendants("manga");
            foreach (var item in manga) {
                Item.Add(new ItemModel {
                    Category = AnimeOrManga.manga,
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

            string JsonItems = JsonConvert.SerializeObject(Item, Formatting.Indented);
            //save data
            var OfflineFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            var SaveFile = await OfflineFolder.CreateFileAsync("RAW_" + service + ".json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(SaveFile, JsonItems);

        }
        public static async Task CreateCustomVirtualDirectory(string FolderName) {

        }
        public static async Task AddItemToVirtualDirectory(string CustomFolder) {

        }
        /// <summary>
        /// List all custom folder
        /// </summary>
        /// <returns>
        /// Folder List
        /// </returns>
        //public Collection<CustomFolder> CustomFolderList()
        //{
        //    Collection<CustomFolder> CustomFolder = new Collection<CustomFolder>();
        //    CustomFolder.Add(new CustomFolder {
        //        FolderList = "Completed"
        //    });
        //    return 0;
        //}
    }
}