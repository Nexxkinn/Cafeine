using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Models.Enums
{
    class ScoreFormatEnum
    {
        /// <summary>
        /// Anilist Only. Default for UI : Point_5
        /// [ 0 - POINT_100 ]
        /// [ 1 - POINT_10_DECIMAL ]
        /// [ 2 - POINT_10 ]
        /// [ 3 - POINT_5 ]
        /// [ 4 - POINT_3 ]
        /// </summary>
        public static Dictionary<string, int> Anilist_ScoreFormat = new Dictionary<string, int>
        {
            ["POINT_100"] = 0,
            ["POINT_10_DECIMAL"] = 1,
            ["POINT_10"] = 2,
            ["POINT_5"] = 3,
            ["POINT_3"] = 4
        };
        public static double Anilist_ConvertToGlobalUnit(double value, int AnilistScoreFormat)
        {
            switch (AnilistScoreFormat)
            {
                case 0: return value / 20;
                case 1: return value / 2;
                case 2: return value / 2;
                case 3: return value;
                case 4: return value;
            }
            throw new InvalidOperationException();
        }
    }
}
