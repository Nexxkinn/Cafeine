using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services.Api;
using DBreeze;
using DBreeze.DataTypes;
using DBreeze.Objects;
using DBreeze.Utils;
using Fastenshtein;
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

        private static UserAccountModel CurrentAccount;

        private static List<ServiceItem> CurrentItems;

        private static IService CurrentService;

        private static Dictionary<int, IService> services;

        public static event EventHandler DatabaseUpdated;

        public static event EventHandler<bool> NetworkisOnline;

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

        public static async Task BuildServices()
        {
            // TODO : Rewrite useraccountfactory
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
                                        if (account.IsDefaultService) CurrentService = service;
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
        #region accounts
        private static List<UserAccountModel> GetAllUserAccounts()
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

        public static void SetCurrentUserAccount(UserAccountModel account) => CurrentAccount = account;

        public static UserAccountModel GetCurrentUserAccount()
        {
            if(CurrentAccount == null)
            {
                CurrentAccount = GetAllUserAccounts().First(x => x.IsDefaultService);
            }
            return CurrentAccount;
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
        #endregion

        /// <summary>
        /// Build/rebuild database.
        /// ASSUME there's at least one user account listed in the local database.
        /// </summary>
        /// <returns></returns>
        public static async Task Build()
        {
            try
            {
                db.Scheme.DeleteTable("TS_Library");
                //Get all available collection from useraccounts to the library.
                var collection = await CurrentService.CreateCollection(CurrentAccount);
                using(var tr = db.GetTransaction())
                {
                    foreach (var item in collection)
                    {
                        tr.TextInsert("TS_Library", item.ServiceID.ToBytes(), containsWords: item.Title, fullMatchWords: "");
                    }
                    tr.Commit();
                }
                CurrentItems = new List<ServiceItem>(collection);
                collection.Clear();
            }
            catch (Exception e)
            {
                // assume offline version
                throw new Exception(e.Message, e);
            }
        }

        public static async Task SyncAllService()
        {
            // TODO: write an instruction here
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

        #region item management
        public static async Task<UserItem> CreateUserItem(ServiceItem Item)
        {
            var useritem = await CurrentService.AddItem(Item);
            CurrentItems.Remove(Item);
            Item.UserItem = useritem;
            CurrentItems.Add(Item);
            return useritem;
        }

        public static async Task PopulateAdditionalItem(ServiceItem Item)
        {
            await Item.PopulateMoreDetails(CurrentService);
        }

        public static async Task UpdateItem(ServiceItem serviceitem)
        {
            await CurrentService.UpdateUserItem(serviceitem.UserItem);
            var oldItem = CurrentItems.FindIndex(x => x.ServiceID == serviceitem.ServiceID);
            CurrentItems.RemoveAt(oldItem);
            CurrentItems.Insert(oldItem, serviceitem);
            DatabaseUpdated.Invoke(serviceitem, null);
        }

        public static OfflineItem CreateOflineItem(ServiceItem item)
        {
            using (var tr = db.GetTransaction())
            {
                OfflineItem offlineitem = new OfflineItem
                {
                    Id = tr.ObjectGetNewIdentity<int>("library"),
                    ServiceID = item.ServiceID,
                    MalID = item.MalID,
                };
                tr.ObjectInsert("library", new DBreezeObject<OfflineItem>
                {
                    NewEntity = true,
                    Entity = offlineitem,
                    Indexes = new List<DBreezeIndex>()
                    {
                        new DBreezeIndex(1,offlineitem.Id){PrimaryIndex = true},
                        new DBreezeIndex(2,offlineitem.ServiceID),
                        new DBreezeIndex(3,offlineitem.MalID)
                    }
                });
                tr.Commit();
                return offlineitem;
            }
        }

        public static async Task<OfflineItem> GetOfflineItem(int? service_id,int? mal_id)
        {
            OfflineItem item = await Task.Run(() =>
            {
                using (var tr = db.GetTransaction())
                {
                    IEnumerable<Row<byte[], byte[]>> result = null;
                    if (service_id !=0)
                    {

                        result = tr.SelectForwardFromTo<byte[], byte[]>("library", 2.ToIndex(service_id, 0), true, 2.ToIndex(service_id, int.MaxValue), true);
                        
                    }
                    else
                    {
                        result = tr.SelectForwardFromTo<byte[], byte[]>("library", 3.ToIndex(mal_id, 0, 0), true, 3.ToIndex(mal_id, int.MaxValue, int.MaxValue),true);
                    }

                    if ( result?.Count() != 0)
                    {
                        var firstresult = result.First();
                        DBreezeObject<OfflineItem> localitem = firstresult.ObjectGet<OfflineItem>();
                        return localitem.Entity;
                    }
                    else return null;
                }
            });
            return item;
        }

        public static Task UpdateOfflineItem(OfflineItem item)
        {
            using (var tr = db.GetTransaction())
            {
                tr.SynchronizeTables("library");
                DBreezeObject<OfflineItem> localitem = tr.Select<byte[], byte[]>("library", 1.ToIndex((int)item.Id)).ObjectGet<OfflineItem>();
                localitem.Entity = item;
                localitem.Indexes = new List<DBreezeIndex>()
                    {
                        new DBreezeIndex(1,item.Id){PrimaryIndex = true},
                        new DBreezeIndex(2,item.ServiceID),
                        new DBreezeIndex(3,item.MalID)
                    };
                tr.ObjectInsert("library", localitem);
                tr.Commit();
            }
            return Task.CompletedTask;
        }

        public static async Task DeleteItem(ServiceItem serviceitem)
        {
            await CurrentService.DeleteItem(serviceitem);
            var oldItem = CurrentItems.FindIndex(x => x.ServiceID == serviceitem.ServiceID);
            CurrentItems.RemoveAt(oldItem);
        }

        public static async Task<List<Episode>> UpdateItemEpisodes(ServiceItem item)
        {
            try
            {
                return await CurrentService.GetItemEpisodes(item) as List<Episode>;
            }
            catch (Exception ex)
            {
                //offline mode, then.
                return new List<Episode>();
            }
        }

        public static ServiceItem GetUserServiceItem(int service_id)
        {
            try
            {
                return CurrentItems.First(x => x.ServiceID == service_id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region search
        public static async Task<IList<ServiceItem>> SearchOnline(string keyword,MediaTypeEnum mediatype = MediaTypeEnum.ANIME)
        {
            if (keyword == string.Empty) return null;
            var results = await CurrentService.OnlineSearch(keyword,mediatype);
            return results;
        }

        public static IList<ServiceItem> SearchOffline(string query)
        {
            using (var tr = db.GetTransaction())
            {
                List<ServiceItem> items = new List<ServiceItem>();
                foreach (var id in tr.TextSearch("TS_Library").BlockAnd(query).GetDocumentIDs())
                {
                    int val = id.To_Int32_BigEndian();
                    var localitem = CurrentItems.First(x => x.ServiceID == val);
                    items.Add(localitem);
                }
                return items;
            }
        }

        public static IList<ServiceItem> SearchBasedonUserStatus(int user_status)
        {
            using (var tr = db.GetTransaction())
            {
                tr.SynchronizeTables("library");
                IList<ServiceItem> result = CurrentItems.Where(x => x.UserItem.UserStatus == user_status).ToList();
                return result;
            }
        }
        #endregion
        public static void ResetDataBase()
        {
            db.Scheme.DeleteTable("user");
            db.Scheme.DeleteTable("library");
            db.Scheme.DeleteTable("TS_Library");
        }
    }
}
