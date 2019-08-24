using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    public interface IApiService : IAuthService
    {
        Task<UserItem> AddItem(ServiceItem item);

        Task GetItem(LocalItem item);

        Task PopulateServiceItemDetails(ServiceItem item);

        Task<DetailsItem> GetDetailsItem(ServiceItem item);

        Task<IList<MediaList>> GetItemEpisodes(ServiceItem item);

        Task UpdateUserItem(UserItem item);

        Task DeleteItem(ServiceItem item);

        void DeleteRange(IList<LocalItem> items);

        Task<UserAccountModel> CreateAccount(bool isDefaultUser);

        Task<IList<ServiceItem>> CreateCollection(UserAccountModel account);

        Task<IList<(ServiceItem service, UserItem user)>> CreateServiceCollection(UserAccountModel account);

        Task<IList<UserItem>> CreateUserCollection(UserAccountModel account);

        void ClearCollection(UserAccountModel account);

        Task<IList<ServiceItem>> OnlineSearch(string keyword, MediaTypeEnum media);

        Task<IList<LocalItem>> OnlineSearch(string keyword);
    }
}
