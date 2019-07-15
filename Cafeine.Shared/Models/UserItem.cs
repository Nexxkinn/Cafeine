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
        public Nullable<double> UserScore { get; set; }
        /// <summary>
        /// User's status.
        /// </summary>
        public Nullable<int> UserStatus { get; set; }
        /// <summary>
        /// User's total episodes/chapters watched/read.
        /// </summary>
        public Nullable<int> Watched_Read { get; set; }
        /// <summary>
        /// Intended only if the service use an overengineered method to identify user's item.
        /// </summary>
        public object AdditionalInfo;

        public string GetUserStatus() => UserStatus.HasValue ? StatusEnum.UserStatus_Int2Str[UserStatus.Value] : "";
    }
}
