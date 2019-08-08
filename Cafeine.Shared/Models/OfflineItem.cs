using Cafeine.Models.Enums;
using DBreeze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Cafeine.Models
{
    /// <summary>
    /// For offline paths
    /// </summary>
    public sealed class OfflineItem 
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
        /// Accepted IDs for selected service.
        /// </summary>
        public Dictionary<ServiceType,int> ServiceID { get; set; }
        /// <summary>
        /// Saved folder token.
        /// </summary>
        public string FolderToken { get; set; }
        /// <summary>
        /// Saved episodes
        /// </summary>
        public ICollection<ContentList> ContentList { get; set; }
        /// <summary>
        /// Custom regex
        /// </summary>
        public string Regex { get; set; }

        public OfflineItem() { }
        public OfflineItem(int id,int mal_id,Dictionary<ServiceType, int> service_id,string folder_token, ICollection<ContentList> content_list, string regex = null)
        {
            this.Id = id;
            this.MalID = mal_id;
            this.ServiceID = service_id;
            this.ContentList = content_list;
            this.FolderToken = folder_token;
            this.Regex = regex;
        }

        public void AddServiceID(ServiceType service_type,int service_id)
        {
            if (ServiceID.ContainsKey(service_type)) return;

            ServiceID.Add(service_type, service_id);
        }

        public void AddNewContentList(IList<ContentList> newlist)
        {
            foreach(var item in newlist)
            {
                ContentList.Add(item);
            }
            ContentList.OrderBy(x => x.Number);
        }
    }

}
