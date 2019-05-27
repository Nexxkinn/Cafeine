using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    public class ItemLibraryService
    {
        private static List<ItemLibraryToken> library { get; set; }
        
        public static async Task<(OfflineItem offline,ServiceItem service)> Pull()
        {
            var token = library.Last();
            library.Remove(library.Last());
            ServiceItem service = token.ServiceItem;
            if(service.UserItem == null)
            {
                // check if local database contains the same serviceitem
                var LocalServiceItem = Database.GetUserServiceItem(service.ServiceID);
                if (LocalServiceItem != null) service = LocalServiceItem;
            }
            OfflineItem offline = await Database.GetOfflineItem(service_id: service.ServiceID,mal_id:service.MalID);
            return (offline,service);
        }

        public static void Push(ServiceItem Item)
        {
            var token = new ItemLibraryToken(item: Item);
            if (library == null) library = new List<ItemLibraryToken>();
            library.Add(token);
        }

        private class ItemLibraryToken
        {
            public ServiceItem ServiceItem { get; private set; }

            public ItemLibraryToken(ServiceItem item)
            {
                this.ServiceItem = item;
            }
        }
    }
}
