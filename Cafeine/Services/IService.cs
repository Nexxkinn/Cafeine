using Cafeine.Models;
using Cafeine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Cafeine.Services
{
    public interface IService
    {
        void AddItem(ItemLibraryModel item);

        void GetItem(ItemLibraryModel item);

        Task<ItemDetailsModel> GetItemDetails(UserItem item, MediaTypeEnum media);

        Task<IList<Episode>> GetItemEpisodes(UserItem item, MediaTypeEnum media);

        void UpdateItem(ItemLibraryModel item);

        void DeleteItem(ItemLibraryModel item);

        void DeleteRange(IList<ItemLibraryModel> items);

        Task<UserAccountModel> CreateAccount(bool isDefaultUser);

        void DeleteAccount(UserAccountModel account);

        Task VerifyAccount();

        Task VerifyAccount(UserAccountModel account);

        Task<IList<ItemLibraryModel>> CreateCollection(UserAccountModel account);

        void ClearCollection(UserAccountModel account);

        Task<IList<ItemLibraryModel>> OnlineSearch(string keyword, MediaTypeEnum media);

        Task<IList<ItemLibraryModel>> OnlineSearch(string keyword);
    }
}
