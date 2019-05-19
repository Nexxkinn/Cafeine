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
        
        public static ItemLibraryModel Pull()
        {
            var token = library.Last();
            library.Remove(library.Last());

            if (token.IsOnlineItem) return token.OnlineItem;
            var item = Database.GetItemLibraryModel(DatabaseID: token.DatabaseID);
            return item;
        }

        public static void Push(ItemLibraryModel Item)
        {
            var token = (Item.Id == 0)
                ? new ItemLibraryToken(OnlineItem: Item)
                : new ItemLibraryToken(Item.Id);
            if (library == null) library = new List<ItemLibraryToken>();
            library.Add(token);
        }

        private class ItemLibraryToken
        {
            public bool IsOnlineItem { get; private set; }
            public int DatabaseID { get; private set; }
            public ItemLibraryModel OnlineItem { get; private set; }

            public ItemLibraryToken(ItemLibraryModel OnlineItem)
            {
                this.IsOnlineItem = true;
                this.OnlineItem = IsOnlineItem ? OnlineItem : null;
            }
            public ItemLibraryToken(int DatabaseID)
            {
                this.IsOnlineItem = false;
                this.DatabaseID = DatabaseID;
            }
        }
    }
}
