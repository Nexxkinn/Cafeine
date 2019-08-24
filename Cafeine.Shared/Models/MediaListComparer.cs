using System;
using System.Collections.Generic;
using System.Text;

namespace Cafeine.Models
{
    public class MediaListComparer : IEqualityComparer<MediaList>
    {
        public bool Equals(MediaList x, MediaList y)
        {
            if (x.Number == -1 || y.Number == -1) return x.Title == y.Title;
            return x.Number == y.Number;
        }

        public int GetHashCode(MediaList obj)
        {
            if (obj.Number == -1) return obj.Title.GetHashCode();
            return obj.Number.GetHashCode();
        }
    }
}
