using Cafeine.Models;
using Cafeine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Security.Credentials;

namespace Cafeine.Services
{
    public static class MyAnimeListApi
    {
        //QueryData
        //UpdateData
        //DeleteData
        public static string Username { get; private set; }

        public static string Password { get; private set; }

        public static string HashID { get; set; } = string.Empty;

        private static HttpClient MALAuthClient = new HttpClient();

        public static void PopulateAuthentication()
        {
            if (HashID != string.Empty) throw new Exception("Hash hasn't been included");
            //grab password from vault
            var vault = new PasswordVault();
            var thelist = vault.FindAllByResource($"Cafeine-{HashID}");
            PasswordCredential credential = thelist[0];
            if (credential != null)
            {
                credential.RetrievePassword();
                Username = credential.UserName;
                Password = credential.Password;
                //TODO : Add exception when passowrd is failed.
            };
        }

        //This method is only used once for setup, and intended as a password verification
        //for other methods.
        public static async Task Authenticate()
        {
            var url = new Uri("https://myanimelist.net/api/account/verify_credentials.xml");
            byte[] bytes = Encoding.UTF8.GetBytes(Username + ":" + Password);
            string LoginToBase64 = Convert.ToBase64String(bytes);
            MALAuthClient.DefaultRequestHeaders.Add("Authorization", "Basic " + LoginToBase64);
            HttpResponseMessage response = await MALAuthClient.GetAsync(url);
            //TODO : Add exception for wrong username / password.
            response.EnsureSuccessStatusCode();
            response.Dispose();
        }

        public static void AddCredentials(bool isdefaultservice)
        {
            //store password
            UserAccountModel userAccount = new UserAccountModel()
            {
                HashID = new Random().ToString(),
                Name = Username,
                Service = ServiceType.MYANIMELIST,
                IsDefaultService = isdefaultservice
            };
            Database.AddAccount(userAccount);
            var vault = new PasswordVault();
            var cred = new PasswordCredential("Cafeine-MAL", Username, Password);
            vault.Add(cred);
        }

        public static async Task<List<ItemLibraryModel>> GetUserData(bool IsDefaultService)
        {

            string FetchData, FetchData2;
            HttpResponseMessage response = new HttpResponseMessage();
            HttpResponseMessage response2 = new HttpResponseMessage();
            List<ItemLibraryModel> Item = new List<ItemLibraryModel>();
            XDocument ParsedItems = new XDocument();

            try
            {
                //get user data
                var url = new Uri("https://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(Username) + "&type=anime&status=all");
                var url2 = new Uri("https://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(Username) + "&type=manga&status=all");
                response = await MALAuthClient.GetAsync(url);
                response2 = await MALAuthClient.GetAsync(url2);

                response.EnsureSuccessStatusCode();
                response2.EnsureSuccessStatusCode();

                FetchData = response.Content.ToString();
                FetchData2 = response2.Content.ToString();
                //TODO : Universalize item type

                string ServiceName = (IsDefaultService) ? "Default" : "Myanimelist";
                ParsedItems = XDocument.Parse(FetchData);
                var Anime = ParsedItems.Descendants("anime");
                foreach (var item in Anime)
                {
                    Item.Add(new ItemLibraryModel
                    {

                        MalID = (int)item.Element("series_animedb_id"),
                        Service = new Dictionary<string, UserItem>
                        {
                            [ServiceName] = new UserItem
                            {
                                Title = item.Element("series_title").Value,
                                TotalEpisodes = (int)item.Element("series_episodes"),
                                SeriesStart = Convert.ToInt32(item.Element("series_start").Value.Take(4).ToString()),
                                Category = "Anime",
                                ItemId = (int)item.Element("series_animedb_id"),
                                Watched_Read = (int)item.Element("my_watched_episodes"),
                                UserScore = (int)item.Element("my_score"),
                                //HACK : lol wtf with this abomination, I need a beer.
                                UserStatus = StatusEnum.UserStatus[$"MAL_{item.Element("my_status").Value}"],
                                //Tags = item.Element("my_tags").Value
                            }
                        }
                    });
                }
                ParsedItems = null;

                XDocument ParsedItemsManga = XDocument.Parse(FetchData2);
                var Manga = ParsedItemsManga.Descendants("manga");
                foreach (var item in Manga)
                {
                    Item.Add(new ItemLibraryModel
                    {
                        MalID = (int)item.Element("series_mangadb_id"),
                        //Imgurl = item.Element("series_image").Value,
                        Service = new Dictionary<string, UserItem>
                        {
                            [ServiceName] = new UserItem
                            {
                                Category = "Manga",
                                Title = item.Element("series_title").Value,
                                TotalEpisodes = (int)item.Element("series_chapters"),
                                SeriesStart = Convert.ToInt32(item.Element("series_start").Value.Take(4).ToString()),
                                Watched_Read = (int)item.Element("my_read_chapters"),
                                UserScore = (int)item.Element("my_score"),
                                //HACK : You know what? I need a Vodka, instead.
                                UserStatus = StatusEnum.UserStatus[$"MAL_{item.Element("my_status").Value}"],
                                //Tags = item.Element("my_tags").Value
                            }
                        }
                    });
                }

                return Item;
            }
            finally
            {
                response.Dispose();
                response2.Dispose();
                Item.Clear();
                ParsedItems = null;
                GC.Collect();
            }
        }

        public static async Task<ItemDetailsModel> GetItemInfo(ItemLibraryModel item)
        {
            throw new NotImplementedException();
        }
    }
}
