using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services.Api;
using DBreeze;
using DBreeze.DataTypes;
using DBreeze.Objects;
using DBreeze.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.Services
{
    public static class Database
    {
        private static readonly string DB_FILE = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "db");

        private static DBreezeEngine db = new DBreezeEngine(DB_FILE);

        private static Dictionary<int, IService> services;

        public static event EventHandler DatabaseUpdated;

        static Database()
        {
            CustomSerializator.ByteArraySerializator = (object o) => { return JsonConvert.SerializeObject(o).To_UTF8Bytes(); };
            CustomSerializator.ByteArrayDeSerializator = (byte[] bt, Type x) => { return JsonConvert.DeserializeObject(bt.UTF8_GetString(), x); };
        }

        public static bool DoesAccountExists()
        {
            using (var tr = db.GetTransaction())
            {
                var e = tr.SelectForward<byte[], byte[]>("user").ToList();
                return e.Count != 0;
            }
        }

        public static UserAccountModel GetCurrentUserAccount()
        {
            using (var tr = db.GetTransaction())
            {
                var item = tr.Select<byte[], byte[]>("user", 1.ToIndex(true)).ObjectGet<UserAccountModel>();
                return item?.Entity;
            }
        }

        public static async Task CreateServicesFromUserAccounts()
        {
            List<UserAccountModel> userAccounts = GetAllUserAccounts();
            List<Task> tasks = new List<Task>();
            services = new Dictionary<int, IService>();
            foreach (var account in userAccounts)
            {
                tasks.Add(Task.Run(async () =>
                {
                    switch (account.Service)
                    {
                        case ServiceType.ANILIST:
                            {
                                IService service = new AniList();
                                var additionalinfo = JsonConvert.DeserializeObject<Tuple<string, string>>(account.AdditionalOption.ToString());
                                await service.VerifyAccount(additionalinfo.Item2);
                                    lock (services)
                                    {
                                        services.Add(account.Id, service);
                                    }
                                    break;
                            }
                        case ServiceType.MYANIMELIST:
                            {
                                break;
                            }
                        case ServiceType.KITSU:
                            {
                                break;
                            }
                        default: break;
                    }
                }));
            }
            await Task.WhenAll(tasks);

        }

        public static List<UserAccountModel> GetAllUserAccounts()
        {
            using (var tr = db.GetTransaction())
            {
                var accounts = new List<UserAccountModel>();
                using (var t = db.GetTransaction())
                {
                    foreach (var item in t.SelectForwardFromTo<byte[], byte[]>("user",
                        2.ToIndex(0), true,
                        2.ToIndex(int.MaxValue), false))
                    {
                        var w = item.ObjectGet<UserAccountModel>();
                        accounts.Add(w.Entity);
                    }
                }
                return accounts;
            }
        }

        public static void AddAccount(UserAccountModel account)
        {
            using (var tr = db.GetTransaction())
            {
                account.Id = tr.ObjectGetNewIdentity<int>("user");
                tr.ObjectInsert("user", new DBreezeObject<UserAccountModel>
                {
                    NewEntity = true,
                    Entity = account,
                    Indexes = new List<DBreezeIndex>()
                    {
                        new DBreezeIndex(1,account.IsDefaultService){PrimaryIndex = true},
                        new DBreezeIndex(2,account.Id)
                    }
                });
                tr.Commit();
            }
        }

        public static void DeleteAccount(UserAccountModel account)
        {
            using (var tr = db.GetTransaction())
            {
                tr.ObjectRemove("user", 1.ToIndex((int)account.Id));
                tr.Commit();
            }
        }

        /// <summary>
        /// Build/rebuild database.
        /// ASSUME there's at least one user account listed in the local database.
        /// </summary>
        /// <returns></returns>
        public static async Task CreateDBFromServices()
        {
            List<UserAccountModel> useraccounts = GetAllUserAccounts();
            List<List<ItemLibraryModel>> Listedlibrary = new List<List<ItemLibraryModel>>();
            try
            {
                List<Task> tasks = new List<Task>();
                //Get all available collection from useraccounts to the library.
                foreach (var user in useraccounts)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        IService service = services[user.Id];
                        var collection = await service.CreateCollection(user);
                        lock (Listedlibrary)
                        {
                            Listedlibrary.Add(new List<ItemLibraryModel>(collection));
                        }
                        collection.Clear();
                    }));
                }
                await Task.WhenAll(tasks);

                //drop old collection
                db.Scheme.DeleteTable("library");

                using (var tr = db.GetTransaction())
                {
                    //reorganize library into the localitem.
                    foreach (var library in Listedlibrary)
                    {
                        foreach (var item in library)
                        {
                            //Find if an item exists first in the local database
                            var localitem = tr.Select<byte[], byte[]>("library", 2.ToIndex(item.MalID, item.Id)).ObjectGet<ItemLibraryModel>();

                            if (localitem == null)
                            {
                                item.Id = tr.ObjectGetNewIdentity<int>("library");
                                var usertatus = item.Service.First().Value.UserStatus;
                                var title = item.Service.First().Value.Title;
                                var res = tr.ObjectInsert("library", new DBreezeObject<ItemLibraryModel>()
                                {
                                    NewEntity = true,
                                    Entity = item,
                                    Indexes = new List<DBreezeIndex>()
                                    {
                                        new DBreezeIndex(1,item.Id){PrimaryIndex = true},
                                        new DBreezeIndex(2,item.MalID),
                                        new DBreezeIndex(3,usertatus),
                                    }
                                });
                                tr.TextInsert("TS_Library", item.Id.ToBytes(), containsWords: title, fullMatchWords: "");
                            }
                            else
                            {
                                //Suppose other service already filled the MalID
                                var service_item = item.Service.First();
                                localitem.Entity.Service.Add(service_item.Key, service_item.Value);
                                tr.ObjectInsert("library", localitem);
                            }
                        }
                        // Blame this one line of code that causes this mess lol
                        tr.Commit();
                    };
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                Listedlibrary.Clear();
                useraccounts.Clear();
            }
        }

        //public static void SyncDatabase(List<ItemLibraryModel> library)
        //{
        //    foreach (var item in library)
        //    {
        //        // Find if an item exists first in the local database
        //        // 
        //        var localitem = LocalItemCollections.FindOne(Query.EQ("MalID", item.MalID));
        //        if (localitem == null)
        //        {
        //            LocalItemCollections.Insert(item);
        //        }
        //        else
        //        {
        //            //Suppose other service already filled the MalID
        //            var x = item.Service.First();
        //            localitem.Service.Add(x.Key, x.Value);
        //            LocalItemCollections.Update(localitem);
        //        }
        //    }
        //}

        public static async Task AddItem(ItemLibraryModel Item)
        {
            // Item must be at least from one currently used service.
            // Assume the current service is "default".
            UserAccountModel user = GetCurrentUserAccount();
            IService service = services[user.Id];
            await service.AddItem(Item);

            using (var tr = db.GetTransaction())
            {
                Item.Id = tr.ObjectGetNewIdentity<int>("library");
                DBreezeObject<ItemLibraryModel> localitem = new DBreezeObject<ItemLibraryModel>()
                {
                    Entity = Item,
                    Indexes = new List<DBreezeIndex>()
                    {
                        new DBreezeIndex(1,Item.Id){PrimaryIndex = true},
                        new DBreezeIndex(2,Item.MalID),
                        new DBreezeIndex(3,Item.Item.UserStatus),
                    }
                };
                tr.ObjectInsert("library", localitem, false);
                tr.TextInsert("TS_Library", Item.Id.ToBytes(), containsWords: Item.Item.Title, fullMatchWords: "");
                tr.Commit();
            }
        }

        /// <summary>
        /// One parameter MUST exist.
        /// </summary>
        /// <param name="DatabaseID"></param>
        /// <param name="MAL_ID"></param>
        public static ItemLibraryModel GetItemLibraryModel(int? DatabaseID = null, int? MAL_ID = null)
        {
            ItemLibraryModel item;
            using(var tr = db.GetTransaction())
            {
                DBreezeObject<ItemLibraryModel> localitem;
                if (DatabaseID.HasValue)
                {
                    byte[] key = 1.ToIndex(DatabaseID.Value);
                    localitem = tr.Select<byte[], byte[]>("library", key).ObjectGet<ItemLibraryModel>();
                    item = localitem?.Entity;
                }
                else
                {
                    var itemaavilable = tr.SelectForwardFromTo<byte[], byte[]>("library", 2.ToIndex(MAL_ID.Value, int.MinValue), true, 2.ToIndex(MAL_ID.Value, int.MaxValue),true);
                    var offlineitem = itemaavilable.First();
                    localitem = offlineitem.ObjectGet<ItemLibraryModel>();
                    item = localitem?.Entity;
                }
                

            }
            return item;
        }

        public static async Task DeleteItem(ItemLibraryModel Item)
        {
            UserAccountModel user = GetCurrentUserAccount();
            IService service = services[user.Id];
            await service.DeleteItem(Item);

            using (var tr = db.GetTransaction())
            {
                tr.ObjectRemove("library", 1.ToIndex(Item.Id));
                tr.TextRemoveAll("TS_Library", Item.Id.ToBytes());
                tr.Commit();
            }
        }

        public static async Task UpdateItem(ItemLibraryModel PooledItem,bool userItemChanged = false)
        {
            //assuming the item is from "default" UserItem 
            if (userItemChanged)
            {
                UserAccountModel user = GetCurrentUserAccount();
                UserItem item = PooledItem.Item;
                IService service = services[user.Id];

                // Item checking
                // -> Set Status to Complete if either total watched is more than total episodes
                //    or userstatus is completed
                if ( item.Watched_Read >= item.EpisodesChapters || item.UserStatus == 1)
                {
                    item.UserStatus = 1;
                    if( item.EpisodesChapters != 0) item.Watched_Read = item.EpisodesChapters; 
                }
                await service.UpdateItem(PooledItem);
            }

            using (var tr = db.GetTransaction())
            {
                tr.SynchronizeTables("library");
                DBreezeObject<ItemLibraryModel> localitem = tr.Select<byte[], byte[]>("library", 1.ToIndex((int)PooledItem.Id)).ObjectGet<ItemLibraryModel>();
                localitem.Entity = PooledItem;
                localitem.Indexes = new List<DBreezeIndex>()
                    {
                        new DBreezeIndex(1,localitem.Entity.Id){PrimaryIndex = true},
                        new DBreezeIndex(2,localitem.Entity.MalID),
                        new DBreezeIndex(3,localitem.Entity.Service["default"].UserStatus),
                    };
                tr.ObjectInsert("library", localitem, false);
                tr.Commit();
            }

            if(userItemChanged) DatabaseUpdated.Invoke(null, null);

        }

        public static async Task<ItemDetailsModel> ViewItemDetails(UserItem item, ServiceType serviceType, MediaTypeEnum media)
        {
            var output = new ItemDetailsModel();
            UserAccountModel userAccount = GetCurrentUserAccount();
            IService service = services[userAccount.Id];
            output = await service.GetItemDetails(item, media);
            return output;
        }

        public static async Task<List<Episode>> UpdateItemEpisodes(UserItem item, int ItemLibraryID, ServiceType serviceType, MediaTypeEnum media)
        {
            var output = new List<Episode>();
            try
            {
                UserAccountModel user = GetCurrentUserAccount();
                IService service = services[user.Id];
                output = await service.GetItemEpisodes(item, media) as List<Episode>;
                return output;
            }
            catch (Exception ex)
            {
                //offline mode, then.
                return new List<Episode>();
            }
        }

        public static async Task<IList<ItemLibraryModel>> SearchOnline(string keyword,MediaTypeEnum mediatype = MediaTypeEnum.ANIME)
        {
            if (keyword == string.Empty) return null;
            UserAccountModel account = GetCurrentUserAccount();
            IService service = services[account.Id];
            IList<ItemLibraryModel> results = await service.OnlineSearch(keyword,mediatype);
            return results;
        }

        public static IList<ItemLibraryModel> SearchItemCollection(string query)
        {
            using (var tr = db.GetTransaction())
            {
                List<ItemLibraryModel> items = new List<ItemLibraryModel>();
                foreach (var id in tr.TextSearch("TS_Library").BlockAnd(query).GetDocumentIDs())
                {
                    var localitem = tr.Select<byte[], byte[]>("library", 1.ToIndex(id)).ObjectGet<ItemLibraryModel>();
                    items.Add(localitem.Entity);
                }
                return items;
            }
        }

        public static IList<ItemLibraryModel> SearchBasedonCategory(int category)
        {
            using (var tr = db.GetTransaction())
            {
                tr.SynchronizeTables("library");
                IList<ItemLibraryModel> items = new List<ItemLibraryModel>();
                foreach (var localitem in tr.SelectForwardStartsWith<byte[], byte[]>("library", 3.ToIndex(category)))
                {
                    var item = localitem.ObjectGet<ItemLibraryModel>();
                    items.Add(item.Entity);
                }
                return items;
            }
        }

        public static void ResetAll()
        {
            db.Scheme.DeleteTable("user");
            db.Scheme.DeleteTable("library");
            db.Scheme.DeleteTable("TS_Library");
        }
    }
}
