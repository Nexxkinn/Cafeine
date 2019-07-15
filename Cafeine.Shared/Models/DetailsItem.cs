using Cafeine.Models.Enums;

namespace Cafeine.Models
{
    public class DetailsItem
    {
        public ServiceType servicetype { get; }
        public int ServiceID { get; }
        public int MalID { get; }

        public DetailsItem(ServiceItem item)
        {
            servicetype = item.Service;
            ServiceID = item.ServiceID;
            MalID = item.MalID;
            Description = item.Description;
        }

        public string Description { get; set; }
        public string Studio { get; set; }
        public string Author { get; set; }
    }
}
