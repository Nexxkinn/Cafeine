using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Cafeine.Models;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;
using System.Collections;

namespace Cafeine.Services
{
    class SearchProvider
    {
        ///project:
        ///1 - QuickSuggestion   : give 5 first result from each group ( anime, manga, and people in order )
        ///2 - Deepsearch        : give all relevant result from each group

        //public static async Task<ObservableCollection<animelibrary>> quicksearch(string searchedtext)
        // {

        // }
        public static async Task<IEnumerable<IGrouping<string, GroupedSearchResult>>> ResultIndex(string Query)
        {
            //offline
            List<GroupedSearchResult> Index = new List<GroupedSearchResult>();
            Index.Add(new GroupedSearchResult { GroupName = "Anime", Library = await OnlineResult(Query, AnimeOrManga.anime) });
            Index.Add(new GroupedSearchResult { GroupName = "Manga", Library = await OnlineResult(Query, AnimeOrManga.manga) });
            var group = from i in Index
                        group i by i.GroupName;
            return group;
        }
        private static async Task<List<ItemProperties>> OnlineResult(string Query, AnimeOrManga animeormanga)
        {
            var User = Logincredentials.getuser(1); //Grab username and password
            var url = new Uri("https://myanimelist.net/api/" + animeormanga.ToString() + "search.xml?q=" + Query);

            //GET
            var clientHeader = new HttpBaseProtocolFilter() { ServerCredential = User, AllowUI = false };
            HttpResponseMessage response;
            using (var client = new HttpClient(clientHeader))
            {
                response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
            }
            return ParseItem(response.Content.ToString());

        }
        private static List<ItemProperties> ParseItem(string Data)
        {
            List<ItemProperties> ItemList = new List<ItemProperties>();
            XDocument ParseData = XDocument.Load(Data);
            //parse xml -> ItemProperties
            var Item = ParseData.Descendants("entry").Take(3);
            foreach (var item in Item)
            {
                ItemList.Add(new ItemProperties
                {
                    Item_Id = (int)item.Element("id"),
                    Item_Title = item.Element("title").ToString(),
                    Imgurl = item.Element("image").ToString()
                });
            }
            return ItemList;
        }
    }
}