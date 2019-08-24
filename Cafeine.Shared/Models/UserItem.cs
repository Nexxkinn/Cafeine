using Cafeine.Models.Enums;
using System;

namespace Cafeine.Models
{
    public class UserItem
    {
        /// <summary>
        /// Service ID. Intended for database.
        /// </summary>
        public int ServiceID { get; set; }
        /// <summary>
        /// User's score.
        /// </summary>
        public double? UserScore { get; set; }
        /// <summary>
        /// User's status.
        /// </summary>
        public int? UserStatus { get; set; }
        /// <summary>
        /// User's total episodes/chapters watched/read.
        /// </summary>
        public int? Watched_Read { get; set; }
        /// <summary>
        /// Intended only if the service use an overengineered method to identify user's item.
        /// </summary>
        public object AdditionalInfo;

        public string GetUserStatus() => UserStatus.HasValue ? StatusEnum.UserStatus_Int2Str[UserStatus.Value] : "";
    }
}
