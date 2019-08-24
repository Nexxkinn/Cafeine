using Cafeine.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Cafeine.Models
{
    /// <summary>
    /// The offline parts, obtained mostly from database.
    /// </summary>
    public sealed class LocalItem 
    {
        /// <summary>
        /// Database ID. Not for use.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Accepted ID for all known services. <para />
        /// Use this one for identification or to get item.
        /// </summary>
        public int MalID { get; set; }
        /// <summary>
        /// ID for service.
        /// </summary>
        public Dictionary<ServiceType,int> ServiceID { get; set; }
        /// <summary>
        /// folder path.
        /// </summary>
        public string FolderToken { get; set; }
        /// <summary>
        /// Saved episodes
        /// </summary>
        public ICollection<MediaList> MediaCollection { get; set; }
        /// <summary>
        /// Custom regex
        /// </summary>
        public string Regex { get; set; }

        public LocalItem() { }
        public LocalItem(
            int id,
            int mal_id,
            Dictionary<ServiceType, int> service_id,
            string folder_token,
            ICollection<MediaList> content_list,
            string regex = null
            )
        {
            this.Id = id;
            this.MalID = mal_id;
            this.ServiceID = service_id;
            this.MediaCollection = content_list;
            this.FolderToken = folder_token;
            this.Regex = regex;
        }

        public void AddServiceID(ServiceType service_type,int service_id)
        {
            if (ServiceID.ContainsKey(service_type)) return;

            ServiceID.Add(service_type, service_id);
        }

        public void AddNewContentList(IList<MediaList> newlist)
        {
            foreach(var item in newlist)
            {
                MediaCollection.Add(item);
            }
            MediaCollection.OrderBy(x => x.Number);
        }
    }

}
