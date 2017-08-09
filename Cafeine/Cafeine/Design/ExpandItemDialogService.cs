using Cafeine.Model;
using Cafeine.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.Design {
    class ExpandItemDialogService : ViewModelBase {
        public static async Task ItemCollectionExpand(CollectionLibrary ez, AnimeOrManga aa) {
            ItemModel item;
            ExpandItemDetails ExpandItemDialog = new ExpandItemDetails();
            ExpandItemDialog.Item = ez.Itemproperty;
            ExpandItemDialog.category = aa;
            await ExpandItemDialog.ShowAsync();
            item = ExpandItemDialog.Item;
            if (item != null) {
                ez.My_score = item.My_score.ToString();
                ez.My_watch = item.My_watch.ToString();
            }//TODO : CANCEL BUTTON
        }

        ///Steps :
        ///1.   Check if page in collectionlibrary
        ///     If so, check if it has the item
        ///2.   Check if it's available in collection
        ///     If so, expand from its vm
        /// 
        public static async void QueryItemExpand(GroupedSearchResult o) {
            CollectionLibrary input = new CollectionLibrary(o.Library);

            //fetch if it has local library
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            StorageFile OpenJSONFile = await OffFolder.GetFileAsync("RAW_1.json");
            string ReadJSONFile = await FileIO.ReadTextAsync(OpenJSONFile);
            try {
                input = new CollectionLibrary(
                    JsonConvert.DeserializeObject<List<ItemModel>>(ReadJSONFile)
                    .Where(x => x.Item_Id == o.Library.Item_Id)
                    .First()
                    );
            }
            catch (InvalidOperationException) {
            }

            //check if CollectionLibrary frame has the item too
            Messenger.Default.Send(
                new NotificationMessageAction<CollectionLibrary>(o.Library, "", reply => {
                    input = reply;
                }));

            await ItemCollectionExpand(input, o.Library.Category);
        }

    }
}
