using System;
using System.Collections.Generic;
using System.Text;

namespace Cafeine.Models
{
    public class ContentListComparer : IEqualityComparer<ContentList>
    {
        public bool Equals(ContentList x, ContentList y)
        {
            if (x.Number == -1 || y.Number == -1) return x.Title == y.Title;
            return x.Number == y.Number;
        }

        public int GetHashCode(ContentList obj)
        {
            if (obj.Number == -1) return obj.Title.GetHashCode();
            return obj.Number.GetHashCode();
        }
    }
}
