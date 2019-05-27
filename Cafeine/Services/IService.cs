using Cafeine.Models;
using Cafeine.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    public interface IService
    {
        
        Task<UserItem> AddItem(ServiceItem item);

        Task GetItem(OfflineItem item);

        Task GetItemDetails(ServiceItem item, MediaTypeEnum media);

        Task<IList<Episode>> GetItemEpisodes(ServiceItem item);

        Task UpdateItem(ServiceItem item);

        Task DeleteItem(ServiceItem item);

        void DeleteRange(IList<OfflineItem> items);

        Task<UserAccountModel> CreateAccount(bool isDefaultUser);

        void DeleteAccount(UserAccountModel account);

        Task VerifyAccount();

        Task VerifyAccount(string token);

        Task VerifyAccount(UserAccountModel account);

        Task<IList<ServiceItem>> CreateCollection(UserAccountModel account);

        Task<IList<(ServiceItem service, UserItem user)>> CreateServiceCollection(UserAccountModel account);

        Task<IList<UserItem>> CreateUserCollection(UserAccountModel account);

        void ClearCollection(UserAccountModel account);

        Task<IList<ServiceItem>> OnlineSearch(string keyword, MediaTypeEnum media);

        Task<IList<OfflineItem>> OnlineSearch(string keyword);
    }
}
