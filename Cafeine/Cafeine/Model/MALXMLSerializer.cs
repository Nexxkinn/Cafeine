namespace Cafeine.Model
{
    public class MALXMLSerializerRootobject
    {
        public Entry entry { get; set; }
    }

    public class Entry
    {
        public string episode { get; set; }
        public string status { get; set; }
        public int score { get; set; }
        public string storage_type { get; set; }
        public string storage_value { get; set; }
        public string times_rewatched { get; set; }
        public string rewatch_value { get; set; }
        public string date_start { get; set; }
        public string date_finish { get; set; }
        public string priority { get; set; }
        public string enable_discussion { get; set; }
        public string enable_rewatching { get; set; }
        public string comments { get; set; }
        public string tags { get; set; }
    }

}