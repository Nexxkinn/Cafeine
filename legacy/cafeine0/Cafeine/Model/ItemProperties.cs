using System.Xml.Serialization;

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

        [XmlIgnore]
        public string[] VirtualDirectory;
    }
    public class VirtualDirectory
    {

        public AnimeOrManga AnimeOrManga;
        public int DirectoryType;
        public string DirectoryTitle;
    }
}