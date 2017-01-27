using System;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Windows.Web.Http;

namespace Cafeine.Datalist
{
    //public class ItemDetails
    //{
    //    public int ItemRatings;
    //    public int ItemFavourites;
    //    public string ItemSynopsis;
    //    public string SelectedReview;
    //    public int status;
    //}
    class ExpandItemDetail
    {
        ///Proof of Concept - Expanded Item Details
        ///1. Synopsis                                  -> accomplished
        ///2. Item Details ( Rating, favourites, etc. ) -> too lazy
        ///3. reviews                                   -> too lazy
        ///4. People involved                           -> too lazy
        ///5. Forum                                     -> too lazy
        ///
        #region Code - Working perfectly.
        //public static async Task<ItemDetails> RetreiveItemDetail(int Id, int AnimeOrManga)
        //{
        //    ///GET myanimelist/anime_or_manga/Id/
        //    string Category = (AnimeOrManga == 1) ? "anime" : "manga";
        //    var url = new Uri("https://myanimelist.net/" + Category + "/" + Id);
        //    HttpClient client = new HttpClient();
        //    HtmlDocument res = new HtmlDocument();
        //    res.LoadHtml(await client.GetStringAsync(url));
        //    ///Parse with HTMLAgilitypack
        //    ItemDetails item = new ItemDetails()
        //    {
        //        ItemSynopsis = (string)res.DocumentNode.Descendants().Single(node => node.Name == "span" && node.Attributes["itemprop"] != null && node.Attributes["itemprop"].Value == "description").InnerText
        //    };
        //    ///Return ItemDetails
        //    return item;
        //}
#endregion
    }
}
