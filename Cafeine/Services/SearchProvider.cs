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
using System.Xml;

namespace Cafeine.Services
{
    class SearchProvider
    {
        public static async Task<List<IGrouping<string, GroupedSearchResult>>> ResultIndex(string Query)
        {
            //online
            List<GroupedSearchResult> Index = await OnlineResult(Query);
            var group = Index.GroupBy(b => b.GroupName).ToList();
            return group;
        }
        private static async Task<List<GroupedSearchResult>> OnlineResult(string Query)
        {
            var User = Logincredentials.getuser(1); //Grab username and password
            var url = new Uri("https://myanimelist.net/api/anime/search.xml?q=" + Query);
            var url2 = new Uri("https://myanimelist.net/api/manga/search.xml?q=" + Query);                  //my god, I hate this hack

            //GET
            HttpResponseMessage AnimeResponse, MangaResponse;
            byte[] bytes = Encoding.UTF8.GetBytes(User.UserName + ":" + User.Password);
            string LoginToBase64 = Convert.ToBase64String(bytes);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + LoginToBase64);

                AnimeResponse = await client.GetAsync(url);
                AnimeResponse.EnsureSuccessStatusCode();

                MangaResponse = await client.GetAsync(url2);
                MangaResponse.EnsureSuccessStatusCode();
            }

            List<GroupedSearchResult> ItemList = new List<GroupedSearchResult>();
            XDocument AnimeItems = ParseResponse(AnimeResponse.Content.ToString());
            XDocument MangaItems = ParseResponse(MangaResponse.Content.ToString());
            if (AnimeItems == null && MangaItems == null) return ItemList;                                // just return null list.

            AnimeResponse.Dispose();
            MangaResponse.Dispose();
            if(AnimeItems != null)
            {
                var AnimeIndex = AnimeItems.Descendants("entry").Take(3);
                foreach (var item in AnimeIndex)
                {
                    ItemList.Add(new GroupedSearchResult
                    {
                        GroupName = "Anime",
                        Library = new ItemProperties
                        {
                            Item_Id = (int)item.Element("id"),
                            Item_Title = item.Element("title").Value,
                            Imgurl = item.Element("image").Value
                        }
                    });
                }
            }

            if(MangaItems != null)
            {
                var MangaIndex = MangaItems.Descendants("entry").Take(3);
                foreach (var item in MangaIndex)
                {
                    ItemList.Add(new GroupedSearchResult
                    {
                        GroupName = "Manga",
                        Library = new ItemProperties
                        {
                            Item_Id = (int)item.Element("id"),
                            Item_Title = item.Element("title").Value,
                            Imgurl = item.Element("image").Value
                        }
                    });
                }
            }
            return ItemList;

        }

        private static XDocument ParseResponse(string content)
        {
            XDocument ParseResponse;
            try
            {
                ParseResponse = XDocument.Parse(content);
                return ParseResponse;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}