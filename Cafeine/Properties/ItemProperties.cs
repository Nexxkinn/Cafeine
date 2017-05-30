namespace Cafeine.Properties
{
    public class ItemProperties
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
        public int My_watch;
        public string Imgurl;
        public int Series_Status;
        public int My_status;

        public string[] VirtualDirectory;
    }

    /// <summary>
    /// AnimeOrManga : True if Anime, False if manga.
    /// DirectoryCategory : 1 - Anime, 2 - Manga, 3 - Others, 4-8 - Status
    /// DirectoryTitle : Tile
    /// </summary>
    public class VirtualDirectory
    {

        public bool? AnimeOrManga;
        public int DirectoryType;
        public string DirectoryTitle;
    }
}
