using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
namespace Cafeine.Datalist
{

    public class UserItemCollection
    {
        public int Item_Id;
        public string Item_Title;
        public int Item_Totalepisodes;
        public string Item_Start;
        public string Item_end;
        public int Item_rewatch;
        public int Item_lastupdated;
        public string Series_start;
        public string Series_end;
        public int My_score;
        public string Imgurl;

        public Animestatus Series_Status;
        public Animestatus My_status;
    }
    /// <summary>
    /// <anime>
    ///series_animedb_id>1</series_animedb_id>
    //<series_title>Cowboy Bebop</series_title>
    //<series_synonyms>; Cowboy Bebop</series_synonyms>
    //<series_type>1</series_type>
    //<series_episodes>26</series_episodes>
    //<series_status>2</series_status>
    //<series_start>1998-04-03</series_start>
    //<series_end>1999-04-24</series_end>
    //<series_image>https://myanimelist.cdn-dena.com/images/anime/4/19644.jpg</series_image>
    //<my_id>0</my_id>
    //<my_watched_episodes>26</my_watched_episodes>
    //<my_start_date>0000-00-00</my_start_date>
    //<my_finish_date>2015-03-09</my_finish_date>
    //<my_score>10</my_score>
    //<my_status>2</my_status>
    //<my_rewatching>0</my_rewatching>
    //<my_rewatching_ep>0</my_rewatching_ep>
    //<my_last_updated>1425909894</my_last_updated>
    //<my_tags/>
    //</anime>
    /// </summary>
    public enum Animestatus
    {
        Watching = 1,
        Completed = 2,
        OnHold = 3,
        Dropped = 4,
        PlannToWatch = 6,
        AllOrAiring = 7
    }
    class LibraryList
    {
        public static async Task<ObservableCollection<UserItemCollection>> QueryUserAnimeMangaListAsync(int AnimeorManga, int Status)
        {
            ObservableCollection<UserItemCollection> userlibrary = new ObservableCollection<UserItemCollection>();
            string anime_or_manga = (AnimeorManga == 1) ? "anime" : "manga";
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            string OffFile = Path.Combine(OffFolder.Path, "RAW_" + anime_or_manga + ".xml");
            XDocument ParseData = XDocument.Load(OffFile);

            switch (AnimeorManga) //1 - anime  //2 - manga
            {
                case 1:
                    {
                        var anime = ParseData.Descendants("anime").Where(x => (int)x.Element("my_status") == Status);
                        foreach (var item in anime)
                        {

                            userlibrary.Add(new UserItemCollection
                            {
                                Item_Id = (int)item.Element("series_animedb_id"),
                                Item_Title = item.Element("series_title").Value,
                                Item_Totalepisodes = (int)item.Element("series_episodes"),

                                Imgurl = item.Element("series_image").Value,
                                My_score = (int)item.Element("my_score"),
                                My_status = (Animestatus)(int)item.Element("my_status")
                                });
                        }
                        break;
                    }

                default:
                    break;
            }
            return userlibrary;
        }
    }
}