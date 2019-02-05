using Cafeine.Models;
using Cafeine.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    public interface IService
    {
        
        Task AddItem(ItemLibraryModel item);

        void GetItem(ItemLibraryModel item);

        Task<ItemDetailsModel> GetItemDetails(UserItem item, MediaTypeEnum media);

        Task<IList<Episode>> GetItemEpisodes(UserItem item, MediaTypeEnum media);

        Task UpdateItem(ItemLibraryModel item);

        Task DeleteItem(ItemLibraryModel item);

        void DeleteRange(IList<ItemLibraryModel> items);

        Task<UserAccountModel> CreateAccount(bool isDefaultUser);

        void DeleteAccount(UserAccountModel account);

        Task VerifyAccount();

        Task VerifyAccount(string token);

        Task VerifyAccount(UserAccountModel account);

        Task<IList<ItemLibraryModel>> CreateCollection(UserAccountModel account);

        void ClearCollection(UserAccountModel account);

        Task<IList<ItemLibraryModel>> OnlineSearch(string keyword, MediaTypeEnum media);

        Task<IList<ItemLibraryModel>> OnlineSearch(string keyword);
    }
}
