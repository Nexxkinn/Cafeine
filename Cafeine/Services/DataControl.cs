using System;
using Cafeine.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Storage;
namespace Cafeine.Services
{
    class DataProvider
    {

        ///<summary>
        ///Grab user's data from the server and save it to the local file inside the app.
        /// 
        ///category : 1 -> anime , 2-> manga
        ///</summary>
        public static async Task GrabUserDatatoOffline(int service)
        {
            string username = Logincredentials.getuser(1).UserName;


            ///currently only to serve MyAnimelist
            ///Considering to use NetJSON to get faster serializer and deserializer.
            var url = new Uri("https://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=anime&status=all");
            var url2 = new Uri("https://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=manga&status=all");
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                HttpResponseMessage response2 = await client.GetAsync(url2);

                response.EnsureSuccessStatusCode();
                response2.EnsureSuccessStatusCode();

                string FetchData = response.Content.ToString();
                string FetchData2 = response2.Content.ToString();

                //save data
                var OfflineFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
                var SaveFile = await OfflineFolder.CreateFileAsync("RAW_" + service + "_"+AnimeOrManga.Anime+".xml", CreationCollisionOption.ReplaceExisting);
                var SaveFile2 = await OfflineFolder.CreateFileAsync("RAW_" + service + "_"+AnimeOrManga.Manga+".xml", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(SaveFile, FetchData);
                await FileIO.WriteTextAsync(SaveFile2, FetchData2);

            }

        }
        public static async Task CreateCustomVirtualDirectory(string FolderName)
        {

        }
        public static async Task AddItemToVirtualDirectory(string CustomFolder)
        {

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
