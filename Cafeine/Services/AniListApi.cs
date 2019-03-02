using Cafeine.Models;
using Cafeine.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Cafeine.Services
{
    struct QueryQL
    {
        public string query;

        public dynamic variables;
    }

    public class AniListApi : IService
    {
        private static HttpClient AnilistAuthClient = new HttpClient();

        private static UserAccountModel UserCredentials { get; set; }

        private readonly Uri HostUri = new Uri("https://graphql.anilist.co");

        private async Task<Dictionary<string, dynamic>> AnilistPostAsync(QueryQL query)
        {
            HttpStringContent JSONRequest = new HttpStringContent(JsonConvert.SerializeObject(query),
                Windows.Storage.Streams.UnicodeEncoding.Utf8,
                "application/json");
            var Response = await AnilistAuthClient.PostAsync(HostUri, JSONRequest);
            string JSONResponse = await Response.Content.ReadAsStringAsync();
            var ContentResponse = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JSONResponse);
            return ContentResponse;
        }

        #region account methods
        public async Task<UserAccountModel> CreateAccount(bool isDefaultUser)
        {
            var Request = new QueryQL
            {
                query = @"{
                            Viewer{
                                id
                                name
                                avatar {
                                    large
                                    medium
                                }
                                mediaListOptions{
                                    scoreFormat
                                }
                                options {
                                    titleLanguage
                                    displayAdultContent
                                    airingNotifications
                                    profileColor
                                }
                            }
                        }"
            };
            Dictionary<string, dynamic> ContentResponse = await AnilistPostAsync(Request);
            //Put it to the constructor
            UserCredentials = new UserAccountModel
            {
                HashID = new Random().ToString(),
                Service = ServiceType.ANILIST,
                ServiceId = ContentResponse["data"]["Viewer"]["id"],
                Name = ContentResponse["data"]["Viewer"]["name"],
                Avatar = new Avatar
                {
                    Large = ContentResponse["data"]["Viewer"]["avatar"]["large"],
                    Medium = ContentResponse["data"]["Viewer"]["avatar"]["medium"]
                },
                IsDefaultService = isDefaultUser,
                AdditionalOption = new Tuple<string, string>(
                    ContentResponse["data"]["Viewer"]["mediaListOptions"]["scoreFormat"] as string,
                    AnilistAuthClient.DefaultRequestHeaders.Authorization.Token)
                    
            };
            return UserCredentials;
        }

        public async Task VerifyAccount(string token = default(string))
        {
            //Anilist's Oauth is PITA
            // You need to READ the url's redirect uri to get the token. But to get it, 
            // you just need the basic POST authentication request, right? Nuh uh, my son.
            // Instead of the usual POST request with user&pass inside, they went full Einstein
            // and said "Let's just use web form to login to get the token!"
            // Well, fine you smartass. Just put a WebView control on it and call it the day.
            // But no... Then the MSFT guy come in and say "well, we are using our own library 
            // called Windows.Web.Http for the WebView control instead of extending .NET's HttpClient
            // so the cache and cookies in the WebView control isn't shared with the HttpClient
            // because fuck you and see ya."
            // Source 1 : https://www.hasaltaiar.com.au/sharing-sessions-between-httpclient-and-webviews-on-windows-phones-2/
            // Source 2 : https://github.com/Microsoft/WindowsCommunityToolkit/issues/1289#issuecomment-313020485
            
            // Add headers
            AnilistAuthClient.DefaultRequestHeaders.Clear();
            AnilistAuthClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            AnilistAuthClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // verify by requesting a viewer
            var Request = new QueryQL
            {
                query = @"{ 
                            Viewer{
                                id
                                name
                                }
                          }"
            };
            await AnilistPostAsync(Request);
        }

        public void DeleteAccount(UserAccountModel account)
        {
            throw new NotImplementedException();
        }

        public async Task VerifyAccount(UserAccountModel account)
        {
            await VerifyAccount();
        }
        #endregion

        #region items
        public async Task AddItem(ItemLibraryModel itemModel)
        {
            QueryQL query = new QueryQL()
            {
                query = @"
                        mutation ($mediaid: Int,$score:Float) {
                          SaveMediaListEntry(mediaId:$mediaid,score:$score){
                            id
                            }
                        }",
                variables = new Dictionary<string, object>()
                {
                    ["mediaid"] = itemModel.Item.ItemId,
                    ["score"] = 0
                }
            };
            dynamic result = await AnilistPostAsync(query);

            // Generate / populate userItem
            UserItem Item = itemModel.Service.ContainsKey("default") ? itemModel.Item : throw new Exception("Generated UserItem doesn't exists.");
            Item.AdditionalInfo = new Tuple<int, string>((int)result["data"]["SaveMediaListEntry"]["id"], StatusEnum.Anilist_UserStatus_Int2Str[0]);            
        }

        public void GetItem(ItemLibraryModel item)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteItem(ItemLibraryModel itemlibrary)
        {
            /// why would they need to use medialist's id to delete the entry
            /// instead of using media's id? really, for no reason at all.
            
            var additionalinfo = JsonConvert.DeserializeObject<Tuple<int,string>>(itemlibrary.Item.AdditionalInfo.ToString());
            QueryQL query = new QueryQL()
            {
                query = @"mutation($id:Int){
                              DeleteMediaListEntry(id:$id){
                                deleted
                              }
                            }",
                variables = new Dictionary<string, object>()
                {
                    ["id"] = additionalinfo.Item1
                }
            };

            var result = await AnilistPostAsync(query);

        }

        public void DeleteRange(IList<ItemLibraryModel> items)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItem(ItemLibraryModel itemModel)
        {
            if (!(itemModel.Item.AdditionalInfo is Tuple<int, string> additionalinfo))
                additionalinfo = JsonConvert.DeserializeObject<Tuple<int, string>>(itemModel.Item.AdditionalInfo.ToString());
            var Query = new QueryQL
            {
                query = @"
                        mutation ($mediaid: Int,$userstatus:MediaListStatus,$progress:Int) {
                          SaveMediaListEntry(mediaId:$mediaid,status:$userstatus,progress:$progress){
                            id
                            status
                            }
                        }
            ",
                variables = new Dictionary<string, dynamic>
                {
                    ["mediaid"] = itemModel.Item.ItemId,
                    ["userstatus"] = StatusEnum.Anilist_UserStatus_Int2Str[itemModel.Item.UserStatus],
                    ["progress"] = itemModel.Item.Watched_Read
                }
            };
            await AnilistPostAsync(Query);
        }

        public async Task<ItemDetailsModel> GetItemDetails(UserItem item, MediaTypeEnum media)
        {
            var output = new Dictionary<string, object>();

            QueryQL query = new QueryQL
            {
                query = @"query($id:Int,$type:MediaType){ 
                              Media (id:$id, type:$type) {
                                description(asHtml:false)
                                streamingEpisodes {
                                  title
                                  thumbnail
                                }
                              }
                            }",
                variables = new Dictionary<string, object>()
                {
                    ["id"] = item.ItemId,
                    ["type"] = media.ToString()
                }
            };
            dynamic Content = await AnilistPostAsync(query);

            // because Anilist is a fuckin moron, where the option asHTML:false
            // doesn't remove the <br> tag because "well, everyone is using browser
            // so let's just keep it WHILE AT THE SAME TIME ALSO INCLUDE \n
            // FOR NO PURPOSE. smh wtf is wrong with them??
            dynamic brfilter = Regex.Replace(Content["data"]["Media"]["description"].Value, @"<[^>]+>|&nbsp;", "").Trim();
            var desc = Regex.Replace(brfilter, @"\\n", "\r\n");

            //TODO : add more itemdetails
            var itemdetailsmodel = new ItemDetailsModel()
            {
                Description = desc

            };
            return itemdetailsmodel;
        }

        public async Task<IList<Episode>> GetItemEpisodes(UserItem item, MediaTypeEnum media)
        {
            QueryQL query = new QueryQL
            {
                query = @"query($id:Int,$type:MediaType){ 
                              Media (id:$id, type:$type) {
                                streamingEpisodes {
                                  title
                                  thumbnail
                                }
                              }
                            }",
                variables = new Dictionary<string, object>()
                {
                    ["id"] = item.ItemId,
                    ["type"] = media.ToString()
                }
            };
            dynamic Content = await AnilistPostAsync(query);
            var episodes = new List<Episode>();
            foreach (var episode in Content["data"]["Media"]["streamingEpisodes"])
            {
                episodes.Add(new Episode
                {
                    Title = episode["title"],
                    OnlineThumbnail = episode["thumbnail"]
                });
            };
            return episodes;
        }
        #endregion

        #region collections
        public async Task<IList<ItemLibraryModel>> CreateCollection(UserAccountModel account)
        {

            var Query = new QueryQL
            {
                query = @"query ($id: Int, $user: String, $type: MediaType) {
                            MediaListCollection(userId: $id, userName: $user, type: $type) {
                            lists {
                                status
                                entries {
                                id
                                mediaId
                                progress
                                score
                                media {
                                    coverImage {
                                    large
                                    }
                                    title {
                                    romaji
                                    english
                                    userPreferred
                                    }
                                    startDate {
                                    year
                                    }
                                    idMal
                                    status
                                    episodes
                                    averageScore
                                    seasonInt
                                    format
                                }
                                }
                            }
                            }
                        }
                ",
                variables = new Dictionary<string, object>()
                {
                    ["id"] = account.Id,
                    ["user"] = account.Name,
                    ["type"] = "ANIME"
                }
            };
            dynamic Collection = await AnilistPostAsync(Query);

            var localitem = new List<ItemLibraryModel>();
            string ServiceName = (account.IsDefaultService) ? "default" : "AniList";

            //For Anime
            foreach (var list in Collection["data"]["MediaListCollection"]["lists"])
            {
                string status = list["status"];
                foreach (var item in list["entries"])
                {
                    Debug.WriteLine($"Item ID: {(int)item["mediaId"]}, Title:{item["media"]["title"]["romaji"]}");
                    int itemstatus = StatusEnum.Anilist_ItemStatus[item["media"]["status"].Value];
                    int userstatus = StatusEnum.UserStatus[status];
                    int season = ((int?)item["media"]["seasonInt"]).GetValueOrDefault();
                    var GeneratedItem = new ItemLibraryModel()
                    {
                        MalID = ((int?)item["media"]["idMal"]).GetValueOrDefault(),
                        Service = new Dictionary<string, UserItem>()
                        {
                            [ServiceName] = new UserItem
                            {
                                Title = item["media"]["title"]["romaji"],
                                CoverImageUri = item["media"]["coverImage"]["large"],
                                SeriesStart = ((int?)item["media"]["startDate"]["year"]).GetValueOrDefault(),
                                ItemId = (int)item["mediaId"],
                                UserScore = ((double?)item["score"]).GetValueOrDefault(),
                                AverageScore = ((double?)item["media"]["averageScore"]).GetValueOrDefault(),
                                UserStatus = userstatus,
                                Status = itemstatus,
                                Season = season % 10,
                                Watched_Read = ((int?)item["progress"]).GetValueOrDefault(),
                                TotalEpisodes = ((int?)item["media"]["episodes"]).GetValueOrDefault(),
                                // ID required to identify user's item
                                AdditionalInfo = new Tuple<int, string>((int)item["id"], status)
                            }
                        }
                    };
                    localitem.Add(GeneratedItem);
                };
            }
            return localitem.ToList();
        }

        public void ClearCollection(UserAccountModel account)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region onlinesearch
        public async Task<IList<ItemLibraryModel>> OnlineSearch(string keyword, MediaTypeEnum media)
        {
            QueryQL query = new QueryQL()
            {
                query = @"
                            query ($search: String, $type: MediaType) {
                              anime: Page {
                                media(search: $search, type: $type) {
                                  coverImage {
                                    large
                                    color
                                  }
                                  startDate {
                                    year
                                  }
                                  seasonInt
                                  id
                                  idMal
                                  title {
                                    romaji
                                  }
                                  status
                                  episodes
                                  averageScore
                                  format
                                  description(asHtml: false)
                                }
                              }
                            }",
                variables = new Dictionary<string, dynamic>
                {
                    ["search"] = keyword,
                    ["type"] = media.ToString()
                }
            };
            Dictionary<string, dynamic> result = await AnilistPostAsync(query);
            IList < ItemLibraryModel > items = new List<ItemLibraryModel>();
            foreach(var item in result["data"]["anime"]["media"])
            {
                //using coalese expression (?? argument) can cause an exception.
                var itemstatus = StatusEnum.Anilist_ItemStatus[item["status"].Value];
                int? seasonint = (int?)item["seasonInt"];
                int season = seasonint.HasValue ? seasonint.Value % 10 : 0;
                int? startdate = (int?)item["startDate"]["year"];
                int? EpisodeCheck = (int?)item["episodes"];
                int episodes = EpisodeCheck.HasValue ? EpisodeCheck.Value : default(int);
                items.Add(new ItemLibraryModel()
                {
                    //using coalese expression (?? argument) can cause an exception.
                    MalID = (item["idMal"] != null) ? item["idMal"] : item["id"],
                    Service = new Dictionary<string, UserItem>()
                    {
                        ["default"] = new UserItem()
                        {
                            ItemId = (int)item["id"],
                            Title = item["title"]["romaji"],
                            CoverImageUri = item["coverImage"]["large"],
                            Details = new ItemDetailsModel()
                            {
                                Description = item["description"]
                            },
                            AverageScore = (double?)item["averageScore"],
                            Status = itemstatus,
                            SeriesStart = startdate ?? 0,
                            TotalEpisodes = episodes,
                            Season = season
                        }
                    }
                });
            }
            return items;
        }

        public Task<IList<ItemLibraryModel>> OnlineSearch(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task VerifyAccount()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
