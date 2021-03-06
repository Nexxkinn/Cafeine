﻿using Cafeine.Models;
using System.Collections.Generic;

namespace Cafeine.Services
{
    public class ItemLibraryService
    {
        private static List<ItemLibraryToken> library { get; set; }
        
        public static (LocalItem offline,ServiceItem service) Pull()
        {
            int last = library.Count == 0 
                ? 0 
                : library.Count - 1;
            var token = library[last];
            library.Remove(token);
            ServiceItem service = token.ServiceItem;
            if(service.UserItem == null)
            {
                // check if local database contains the same serviceitem
                var LocalServiceItem = Database.GetUserServiceItem(service.ServiceID);
                if (LocalServiceItem != null) service = LocalServiceItem;
            }
            LocalItem offline = Database.GetOfflineItem(service);
            return (offline,service);
        }

        public static void Push(ServiceItem Item)
        {
            var token = new ItemLibraryToken(item: Item);
            if (library == null) library = new List<ItemLibraryToken>(capacity:10);
            library.Add(token);
        }

        private class ItemLibraryToken
        {
            public ServiceItem ServiceItem { get; private set; }
            public ItemLibraryToken(ServiceItem item)
            {
                this.ServiceItem = new ServiceItem(item);
            }
        }
    }
}
