using Cafeine.Models;
using Cafeine.Models.Enums;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.Services
{
    public static class Database
    {
        private static readonly string DB_FILE = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Database.db");

        private static LiteCollection<UserAccountModel> LocalUserAccount { get; set; }

        private static LiteCollection<ItemLibraryModel> LocalItemCollections { get; set; }

        private static LiteDatabase db = new LiteDatabase(DB_FILE);

        static Database()
        {
            LocalUserAccount = db.GetCollection<UserAccountModel>("user");
            LocalItemCollections = db.GetCollection<ItemLibraryModel>("library");
        }

        public static bool IsAccountEmpty() => (LocalUserAccount.Count() == 0) ? true : false;

        public static void AddAccount(UserAccountModel account) => LocalUserAccount.Insert(account);

        public static void DeleteAccount(UserAccountModel account) => LocalUserAccount.Delete(x => x.ServiceId == account.ServiceId);

        /// <summary>
        /// Build/rebuild database.
        /// ASSUME there's at least one user account listed in the local database.
        /// </summary>
        /// <returns></returns>
        public static async Task CreateDBFromServices()
        {
            List<UserAccountModel> useraccounts = LocalUserAccount.FindAll().ToList();
            List<ItemLibraryModel> library = new List<ItemLibraryModel>();

            try
            {
                IService User;
                //Get all available collection from useraccounts to the library.
                foreach (var user in useraccounts)
                {
                    switch (user.Service)
                    {
                        case ServiceType.MYANIMELIST:
                            {
                                //TODO: make it contractable.
                                MyAnimeListApi.HashID = user.HashID;
                                MyAnimeListApi.PopulateAuthentication();
                                await MyAnimeListApi.Authenticate();
                                var usercollection = await MyAnimeListApi.GetUserData(user.IsDefaultService);
                                library.AddRange(usercollection);
                                usercollection.Clear();
                                break;
                            }
                        case ServiceType.ANILIST:
                            {

                                User = new AniListApi();
                                var collection = User.CreateCollection(user);
                                library.AddRange(await collection);
                                collection.Result.Clear();
                                break;

                            }
                        case ServiceType.KITSU:
                            {
                                //TODO : Build database for Kitsu
                                //TODO : add IsDefaultService option
                                break;
                            }
                    }
                }

                //drop old collection
                db.DropCollection("library");

                //reorganize library into the localitem.
                Parallel.ForEach(library, (item) =>
                {
                    //Find if an item exists first in the local database
                    var localitem = LocalItemCollections.FindOne(Query.EQ("MalID", item.MalID));
                    if (localitem == null)
                    {
                        LocalItemCollections.Insert(item);
                    }
                    else
                    {
                        //Suppose other service already filled the MalID
                        var x = item.Service.First();
                        localitem.Service.Add(x.Key, x.Value);
                        LocalItemCollections.Update(localitem);
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                library.Clear();
                useraccounts.Clear();
            }
        }

        public static void SyncDatabase(List<ItemLibraryModel> library)
        {
            foreach (var item in library)
            {
                // Find if an item exists first in the local database
                // 
                var localitem = LocalItemCollections.FindOne(Query.EQ("MalID", item.MalID));
                if (localitem == null)
                {
                    LocalItemCollections.Insert(item);
                }
                else
                {
                    //Suppose other service already filled the MalID
                    var x = item.Service.First();
                    localitem.Service.Add(x.Key, x.Value);
                    LocalItemCollections.Update(localitem);
                }
            }
        }

        public static void AddItem(ItemLibraryModel Item)
        {
        }

        public static void DeleteItem(ItemLibraryModel Item)
        {
        }

        public static void EditItem(UserItem Item)
        {
            ItemLibraryModel itemlibrary = LocalItemCollections.FindOne(x => x.Service["default"].ItemId == Item.ItemId);
            itemlibrary.Service["default"] = Item;
            LocalItemCollections.Update(itemlibrary);
        }

        public static async Task<ItemDetailsModel> ViewItemDetails(UserItem item, ServiceType serviceType, MediaTypeEnum media)
        {
            var output = new ItemDetailsModel();
            switch (serviceType)
            {
                case ServiceType.ANILIST:
                    {
                        IService service = new AniListApi();
                        output = await service.GetItemDetails(item, media);
                        break;
                    }
            }
            return output;
        }

        public static async Task<List<Episode>> UpdateItemEpisodes(UserItem item, ServiceType serviceType, MediaTypeEnum media)
        {
            var output = new List<Episode>();
            try
            {
                switch (serviceType)
                {
                    case ServiceType.ANILIST:
                        {

                            IService service = new AniListApi();
                            output = await service.GetItemEpisodes(item, media) as List<Episode>;
                            break;
                        }
                }
                ItemLibraryModel itemlibrary = LocalItemCollections.FindOne(x => x.Service["default"].ItemId == item.ItemId);
                itemlibrary.Episodes = output;
                LocalItemCollections.Update(itemlibrary);
                return output;
            }
            catch (Exception)
            {
                //offline mode, then.
                var offlineitem = LocalItemCollections.FindOne(x => x.Service["default"].ItemId == item.ItemId);
                output = offlineitem.Episodes ?? new List<Episode>();
                return output;
            }
        }

        public static List<Episode> ViewItemEpisodes(UserItem item)
        {
            ItemLibraryModel itemlibrary = LocalItemCollections.FindOne(x => x.Service["default"].ItemId == item.ItemId);
            return itemlibrary.Episodes;
        }

        public static IEnumerable<ItemLibraryModel> SearchItemCollection(string query)
        {
            return LocalItemCollections.Find(item => item.Service["default"].Title.ToLower().Contains(query));
        }

        public static IEnumerable<ItemLibraryModel> SearchBasedonCategory(int category)
        {
            return LocalItemCollections.Find(x => x.Service["default"].UserStatus == category);
        }

        public static void ResetAll()
        {
            db.DropCollection("user");
            db.DropCollection("library");
        }
    }
}
