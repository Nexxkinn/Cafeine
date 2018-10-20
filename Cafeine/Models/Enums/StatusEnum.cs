using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Models.Enums
{
    public enum UserItemStatus
    {
        UserStatus,
        ItemStatus,
    }

    public sealed class StatusEnum
    {
        // using Anilist as Base standard, because Web is all about string, ya know? /s
        // Do not get fooled when the response said it's an interger, because it's not!
        // it's a string with a little checker that said "it's an interger, believe me".
        // Just ask Javascript. He said so.
        public static readonly Dictionary<string, int> UserStatus = new Dictionary<string, int>()
        {
            ["CURRENT"] = 0,
            ["COMPLETED"] = 1,
            ["ONHOLD"] = 2,
            ["DROPPED"] = 3,
            ["PLANNING"] = 4,
            ["PAUSED"] = 2,
            ["REPEATING"] = 0,
            ["MAL_1"] = 0,
            ["MAL_2"] = 1,
            ["MAL_3"] = 2,
            ["MAL_4"] = 3,
            ["MAL_6"] = 4,
        };

        //probably unneeded.
        public static readonly Dictionary<string,int> Anilist_ItemStatus = new Dictionary<string, int>()
        { 
            ["FINISHED"] = 1,
            ["RELEASING"] = 2,
            ["NOT_YET_RELEASED"] = 3,
            ["CANCELLED"] = 4,
        };

        public static readonly Dictionary<int, string> Anilist_AnimeItemStatus = new Dictionary<int, string>()
        {
            [1] = "Finished Airing",
            [2] = "Currently Airing",
            [3] = "Not Yet Released",
            [4] = "Oh man, it got canceled :(((((((("
        };
    }
}
