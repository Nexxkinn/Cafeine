using Cafeine.Services;
using Cafeine.Models;
using Cafeine.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class CollectionLibrary : Page {
        ObservableCollection<CollectionLibraryViewModel> ItemList = new ObservableCollection<CollectionLibraryViewModel>();
        VirtualDirectory DirectoryDetail;
        public CollectionLibrary() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            DirectoryDetail = (VirtualDirectory)e.Parameter;
            Task.Run(async () => await GrabUserItemList());
        }
        async Task GrabUserItemList() {

            try {
                ItemList = await CollectionLibraryProvider.QueryUserAnimeMangaListAsync(DirectoryDetail.AnimeOrManga);
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                    watch.ItemsSource = ItemList.Where(x => x.Itemproperty.My_status == DirectoryDetail.DirectoryType - 3);
                });
            }
            catch (Exception e) {
                //TODO: show error message?? e.Message
            }
        }
        private async void ExpandItemAsync(object sender, ItemClickEventArgs e) {

            var itemselected = (CollectionLibraryViewModel)e.ClickedItem;
            ItemProperties x = await lo(itemselected);
            if (x != null) {
                itemselected.My_score = x.My_score.ToString();
                itemselected.My_watch = x.My_watch.ToString();
            }

        }
        private async Task<ItemProperties> lo(CollectionLibraryViewModel e) {
            ExpandItemDetails ExpandItemDialog = new ExpandItemDetails();
            ItemProperties item;
            ExpandItemDialog.Item = e.Itemproperty;
            ExpandItemDialog.category = DirectoryDetail.AnimeOrManga;
            await ExpandItemDialog.ShowAsync();
            item = ExpandItemDialog.Item;
            return item;
        }
    }
}