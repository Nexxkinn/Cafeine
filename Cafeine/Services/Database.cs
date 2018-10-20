using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cafeine.Models;
using Cafeine.Models.Enums;
using LiteDB;
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
        public static async Task<List<ItemLibraryModel>> GetDatabaseFromServices()
        {
            //UNDONE: Universalize component?
            List<UserAccountModel> useraccounts = LocalUserAccount.FindAll().ToList();
            List<ItemLibraryModel> library = new List<ItemLibraryModel>();

            try
            {
                //Grab all available collection from useraccounts to the library.
                foreach (var list in useraccounts)
                {
                    switch (list.Service)
                    {
                        case ServiceType.MYANIMELIST:
                            {
                                MyAnimeListApi.HashID = list.HashID;
                                MyAnimeListApi.PopulateAuthentication();
                                await MyAnimeListApi.Authenticate();
                                var usercollection = await MyAnimeListApi.GetUserData(list.IsDefaultService);
                                library.AddRange(usercollection);
                                usercollection.Clear();
                                break;
                            }
                        case ServiceType.ANILIST:
                            {
                                var usercollection = AniListApi.BuildUserCollection(list.IsDefaultService);
                                library.AddRange(await usercollection);
                                usercollection.Dispose();
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

                return library;
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }
        public static void BuildDatabase(List<ItemLibraryModel> library) {
            //Delete old database.
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
        public static void SyncDatabase(List<ItemLibraryModel> library)
        {
            foreach (var item in library)
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
            }
        }

        public static void AddItem(ItemLibraryModel Item)
        {
        }
        public static void DeleteItem(ItemLibraryModel Item)
        {
        }
        public static void EditItem(ItemLibraryModel Item)
        {
            LocalItemCollections.Update(Item);
        }
        public static ItemLibraryModel ViewItem(int id)
        {
            return LocalItemCollections.FindById(id);
        }
        public static async Task<T> ViewItemDetails<T>(int id, MediaTypeEnum media, ServiceType service)
        {
            
            Type type = typeof(T);
            if (type == typeof(ItemDetailsModel))
            {
                var output = new ItemDetailsModel();
                switch (service)
                {
                    case ServiceType.ANILIST:
                        {
                            output = await AniListApi.GetItemDetailsFromService(id, media);
                            break;
                        }
                }
                return (T)Convert.ChangeType(output, typeof(T));
            }
            else if (type == typeof(List<Episode>))
            {
                var output = new List<Episode>();
                try
                {
                    switch (service)
                    {
                        case ServiceType.ANILIST:
                            {
                                output = await AniListApi.GetItemEpisodesFromService(id, media);
                                break;
                            }
                    }
                    return (T)Convert.ChangeType(output, typeof(T));
                }
                catch (Exception ex)
                {
                    //offline mode, then.
                    var offlineitem = LocalItemCollections.FindOne(x => x.Service["default"].ItemId == id);
                    output = offlineitem.Episodes ?? new List<Episode>();
                    return (T)Convert.ChangeType(output, typeof(T));
                }
            }
            else throw new Exception("Unknown type");
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
