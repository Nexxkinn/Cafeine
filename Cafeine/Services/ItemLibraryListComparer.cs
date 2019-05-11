using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    public class ItemLibraryListComparer : IEqualityComparer<ItemLibraryModel>
    {
        public bool Equals(ItemLibraryModel x, ItemLibraryModel y)
        {
            if (x.MalID != 0 && x.MalID != 0) return x.MalID == y.MalID;
            else return x.Id == y.Id;
        }

        public int GetHashCode(ItemLibraryModel obj)
        {
            return obj.MalID.GetHashCode();
        }
    }
}
