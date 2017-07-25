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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cafeine.Services {
    class CollectionLibraryProvider {

        ///<summary>
        ///convert user's collection into a list for Gridview or Listview's ItemCollection. 
        ///AnimeorManga : 1 -> anime, 2-> manga
        ///</summary>
        public static async Task<ObservableCollection<CollectionLibraryViewModel>> QueryUserAnimeMangaListAsync(AnimeOrManga AnimeManga) {
            ObservableCollection<CollectionLibraryViewModel> Item = new ObservableCollection<CollectionLibraryViewModel>();
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            StorageFile OpenJSONFile = await OffFolder.GetFileAsync("RAW_1.json");
            string ReadJSONFile = await FileIO.ReadTextAsync(OpenJSONFile);
            List<ItemProperties> products = JsonConvert.DeserializeObject<List<ItemProperties>>(ReadJSONFile);
            switch (AnimeManga) //1 - anime  //2 - manga
            {
                case AnimeOrManga.anime: {
                    var anime = products.Where(x => x.Category == AnimeOrManga.anime);
                    foreach (var item in anime) {
                        Item.Add(new CollectionLibraryViewModel(item));
                    }
                    break;
                }
                case AnimeOrManga.manga: {
                    var manga = products.Where(x => x.Category == AnimeOrManga.manga);
                    foreach (var item in manga) {
                        Item.Add(new CollectionLibraryViewModel(item));
                    }
                    break;
                }
            }
            return Item;
        }
        public static async Task UpdateItem(CollectionLibraryViewModel e, AnimeOrManga AnimeManga) {
            using (var stream = new MemoryStream()) {
                using (var writer = XmlWriter.Create(stream)) {
                    new XmlSerializer(e.Itemproperty.GetType()).Serialize(writer, e.Itemproperty);
                    var xmlEncodedList = Encoding.UTF8.GetString(stream.ToArray());

                    var User = Logincredentials.getuser(1); //Grab username and password
                    var url = new Uri("https://myanimelist.net/api/" + AnimeManga.ToString() + "list/update/" + e.Itemproperty.Item_Id + ".xml?data=" + xmlEncodedList);

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