using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using Cafeine.Data;
using System;
using Windows.Web.Http;


//still deciphering. 
namespace Cafeine.Datalist
{
    public class animelibrary : ILibrarydata
    {
        int id { get; set; }
        string Title { get; set; }
        int my_score { get; set; }
        string imgurl { get; set; }
        int totalepisodes { get; set; }
    }

    public interface ILibrarydata
    {
        int id { get; set; }
        string Title { get; set; }
        int my_score { get; set; }
        string imgurl { get; set; }
        int totalepisodes { get; set; }
    }

    public enum Animestatus
    {
        Watching = 1,
        Completed = 2,
        OnHold = 3,
        Dropped = 4,
        PlanToWatch = 6,
        AllOrAiring = 7
    }
    class LibraryList
    {
        private async Task<string> grabuserdata(int category)
        {
            string anime_or_manga = (category == 1) ? "anime" : "manga";
            //grab username
            string username = new Logincredentials().getusername(1);
            var url = new Uri("http://myanimelist.net/malappinfo.php?u=" + username + "&type=" + anime_or_manga + "&status=all");
            var client = new HttpClient();
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                string getrespond = response.Content.ReadAsStringAsync().ToString();
                response.Dispose();
                return getrespond;
            }
                
        }
        public async Task<List<ILibrarydata>> querydata(int category,int orderby)
        {
            var data = new List<ILibrarydata>();
            var raw = await grabuserdata(category)
            XDocument parseddata = XDocument.Parse(raw);
            switch (category) //1 - anime  //2 - manga
            {
                case 1:
                    {
                        var anime = parseddata.Root.Elements("anime").ToList();
                        foreach (var item in anime)
                        {
                            data.Add{
                                new 
                            }
                        }

                            var list = from query in parseddata.Root.Elements("anime").ToList()
                                   orderby (int)query.Element("my_score") descending
                                   select new
                                   {
                                       imgurl = (string)query.Element("series_image"),
                                       Title = (string)query.Element("series_title"),
                                       my_score = (int)query.Element("my_score")
                                   };
                        
                        break;
                    }
            }


            //LINQ
        }
    }
}
