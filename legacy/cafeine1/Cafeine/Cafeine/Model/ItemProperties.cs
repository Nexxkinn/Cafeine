﻿using System.Xml.Serialization;

namespace Cafeine.Model
{
    [XmlRoot("entry")]
    public class ItemModel
    {
        [XmlIgnore]
        public AnimeOrManga Category;

        [XmlIgnore]
        public int Item_Id;
        [XmlIgnore]
        public string Item_Title;
        [XmlIgnore]
        public int Item_Totalepisodes;
        [XmlIgnore]
        public string Item_Start;
        [XmlIgnore]
        public string Item_end;
        [XmlIgnore]
        public int Item_lastupdated;
        [XmlIgnore]
        public string Series_start;
        [XmlIgnore]
        public string Series_end;
        [XmlIgnore]
        public string Imgurl;
        [XmlIgnore]
        public int Series_Status;

        [XmlElement("episode")]
        public int My_watch;
        [XmlElement("status")]
        public int My_status;
        [XmlElement("score")]
        public int My_score;
        [XmlElement("tags")]
        public string Tags;

        [XmlIgnore]
        public string[] VirtualDirectory;
    }
    public enum AnimeOrManga
    {
        anime = 1,
        manga,
        Directory
    }
    /// <summary>
    /// AnimeOrManga : True if Anime, False if manga.
    /// DirectoryCategory : 1 - Anime, 2 - Manga, 3 - Others, 4-8 - Status
    /// DirectoryTitle : Tile
    /// </summary>
    public class VirtualDirectory
    {

        public AnimeOrManga AnimeOrManga;
        public int DirectoryType;
        public string DirectoryTitle;
    }
}