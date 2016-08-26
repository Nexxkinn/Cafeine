using System;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Storage;
using Cafeine.Data;
namespace Cafeine.Data
{
    class FileData
    {

        /// <summary>
        /// grabuserdatatoofline()  :   save recent data from server to local folder
        /// SyncEditedData()        :   Upload and sync edited data
        /// RetreiveOfflineData()   :   retreive offline data
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static async Task GrabUserDatatoOffline(int category)
        {
            string anime_or_manga = (category == 1) ? "anime" : "manga";
            //grab username
            string username = Logincredentials.getusername(1);
            var url = new Uri("http://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=" + anime_or_manga + "&status=all");
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string FetchData = response.Content.ToString();

                //save data
                var OfflineFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
                var SaveFile = await OfflineFolder.CreateFileAsync("RAW_" + anime_or_manga + ".xml",CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(SaveFile, FetchData);
            }
            ///save to offline data :   1.  Compress data
            ///                         2.  save to category+"offlineuserdata.xml"
            ///                         
            
        }
    }

}
