using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Models.Enums
{
    public static class Seasons
    {
        public enum SeasonsEnum
        {
            WINTER=1,
            SPRING,
            SUMMER,
            FALL
        }
        public static readonly string[] Seasons_int2string = new string[5]
        {
            "Unknown",
            "Winter",
            "Spring",
            "Summer",
            "Fall"
        };
    }
}
