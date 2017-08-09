using Cafeine.Model;
using Cafeine.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static async void da(GroupedSearchResult o, AnimeOrManga a) {
            CollectionLibrary fff = new CollectionLibrary(o.Library);
            Messenger.Default.Send(
                new NotificationMessageAction<CollectionLibrary>(o.Library, "", reply => {
                    fff = reply;
                }));
            await ItemCollectionExpand(fff, a);
        }

    }
}
