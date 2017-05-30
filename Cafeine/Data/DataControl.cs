using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Storage;
namespace Cafeine.Data
{
    class DataControl
    {

        ///<summary>
        ///Grab user's data from the server and save it to the local file inside the app.
        /// 
        ///category : 1 -> anime , 2-> manga
        ///</summary>
        public static async Task GrabUserDatatoOffline(int service)
        {
            string username = Logincredentials.getusername(1);


            //currently only to serve MyAnimelist
            var url = new Uri("http://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=anime&status=all");
            var url2 = new Uri("http://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=manga&status=all");
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
                var SaveFile = await OfflineFolder.CreateFileAsync("RAW_" + service + "_anime.xml", CreationCollisionOption.ReplaceExisting);
                var SaveFile2 = await OfflineFolder.CreateFileAsync("RAW_" + service + "_manga.xml", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(SaveFile, FetchData);
                await FileIO.WriteTextAsync(SaveFile2, FetchData2);

            }
            ///save to offline data :   1.  Compress data
            ///                         2.  save to category+"offlineuserdata.xml"
            ///                         

        }
        public static async Task CreateCustomFolder(string FolderName)
        {

        }
        public static async Task AddItemToFolder(string CustomFolder)
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
