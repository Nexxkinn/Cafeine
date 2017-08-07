using System;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using Cafeine.Model;
using Cafeine.ViewModel;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cafeine.Design {
    class CollectionLibraryProvider {

        ///<summary>
        ///convert user's collection into a list for Gridview or Listview's ItemCollection. 
        ///AnimeorManga : 1 -> anime, 2-> manga
        ///</summary>
        public static async Task<ObservableCollection<CollectionLibrary>> QueryUserAnimeMangaListAsync(AnimeOrManga AnimeManga) {
            ObservableCollection<CollectionLibrary> Item = new ObservableCollection<CollectionLibrary>();
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            StorageFile OpenJSONFile = await OffFolder.GetFileAsync("RAW_1.json");
            string ReadJSONFile = await FileIO.ReadTextAsync(OpenJSONFile);
            List<ItemModel> products = JsonConvert.DeserializeObject<List<ItemModel>>(ReadJSONFile);
            switch (AnimeManga) //1 - anime  //2 - manga
            {
                case AnimeOrManga.anime: {
                    var anime = products.Where(x => x.Category == AnimeOrManga.anime);
                    foreach (var item in anime) {
                        Item.Add(new CollectionLibrary(item));
                    }
                    break;
                }
                case AnimeOrManga.manga: {
                    var manga = products.Where(x => x.Category == AnimeOrManga.manga);
                    foreach (var item in manga) {
                        Item.Add(new CollectionLibrary(item));
                    }
                    break;
                }
            }
            return await Task.FromResult(Item);
        }
        public static async Task UpdateItem(ItemModel e, AnimeOrManga AnimeManga) {
            using (var stream = new MemoryStream()) {
                using (var writer = XmlWriter.Create(stream)) {
                    new XmlSerializer(e.GetType()).Serialize(writer, e);
                    var xmlEncodedList = Encoding.UTF8.GetString(stream.ToArray());

                    var User = Logincredentials.getuser(1); //Grab username and password
                    var url = new Uri("https://myanimelist.net/api/" + AnimeManga.ToString() + "list/update/" + e.Item_Id + ".xml?data=" + xmlEncodedList);

                    //GET
                    byte[] bytes = Encoding.UTF8.GetBytes(User.UserName + ":" + User.Password);
                    string LoginToBase64 = Convert.ToBase64String(bytes);

                    using (var client = new HttpClient()) {
                        client.DefaultRequestHeaders.Add("Authorization", "Basic " + LoginToBase64);

                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }


        }
    }
}