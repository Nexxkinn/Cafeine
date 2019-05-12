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
        private static List<ItemLibraryModel> library { get; set; }
        
        public static ItemLibraryModel Pull()
        {
            var item = library.Last();
            library.Remove(library.Last());
            return item;
        }
        public static void Push(ItemLibraryModel item)
        {
            if (library == null) library = new List<ItemLibraryModel>();
            library.Add(item);
        }
    }
}
