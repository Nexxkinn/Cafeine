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
        public int id { get; set; }
        public string Title { get; set; }
        public int my_score { get; set; }
        public string imgurl { get; set; }
        public int totalepisodes { get; set; }
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
        public async Task<string> grabuserdata(int category)
        {
            string anime_or_manga = (category == 1) ? "anime" : "manga";
            //grab username
            string username = new Logincredentials().getusername(1);
            var url = new Uri("http://myanimelist.net/malappinfo.php?u=" + Uri.EscapeDataString(username) + "&type=" + anime_or_manga + "&status=all");
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string getrespond = await response.Content.ReadAsStringAsync();
                return getrespond;
            }
        }
        public async Task<List<ILibrarydata>> querydata(int category)
        {
            var data = new List<ILibrarydata>();
            string raw = await grabuserdata(category);
            //string load = Path.Combine(Package.Current.InstalledLocation.Path, "Dumplist/useranimelist.xml");
            XDocument parseddata = XDocument.Parse(raw);
            switch (category) //1 - anime  //2 - manga
            {
                case 1:
                    {
                        var anime = parseddata.Descendants("anime");
                        foreach (var item in anime)
                        {
                            data.Add(new animelibrary
                            {
                                imgurl = item.Element("series_image").Value,
                                Title = item.Element("series_title").Value,
                                my_score = (int)item.Element("my_score")
                            });
                        }
                        break;
                    }
            }
            return data;
        }
    }
}
