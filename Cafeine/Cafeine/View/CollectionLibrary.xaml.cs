using Cafeine.Design;
using Cafeine.Model;
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
using Cafeine.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class CollectionLibraryFrame : Page {
        public CollectionLibraryViewModel Vm => (CollectionLibraryViewModel)DataContext;
        ObservableCollection<CollectionLibrary> ItemList = new ObservableCollection<CollectionLibrary>();
        VirtualDirectory DirectoryDetail;
        public CollectionLibraryFrame() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            watch.ItemsSource = null;
            GC.Collect();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Vm.Directory = (VirtualDirectory)e.Parameter;
            Task.Run(async () => await Vm.GrabUserItemList());
        }
        //private async void ExpandItem(object sender, ItemClickEventArgs e) {

        //    var itemselected = (CollectionLibrary)e.ClickedItem; //converter
        //    ItemModel x = await lo(itemselected);
        //    if (x != null) {
        //        itemselected.My_score = x.My_score.ToString();
        //        itemselected.My_watch = x.My_watch.ToString();
        //    }

        //}
        //private async Task<ItemModel> lo(ViewModels.CollectionLibrary e) {
        //    ExpandItemDetails ExpandItemDialog = new ExpandItemDetails();
        //    ItemModel item;
        //    ExpandItemDialog.Item = e.Itemproperty;
        //    ExpandItemDialog.category = DirectoryDetail.AnimeOrManga;
        //    await ExpandItemDialog.ShowAsync();
        //    item = ExpandItemDialog.Item;
        //    return item;
        //}
    }
}