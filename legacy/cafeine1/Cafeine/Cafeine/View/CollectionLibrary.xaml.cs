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
        //ObservableCollection<CollectionLibrary> ItemList = new ObservableCollection<CollectionLibrary>();
        public CollectionLibraryFrame() {
            this.InitializeComponent();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            watch.ItemsSource = null;
            Vm.ItemItemSource = null;
            Vm.ErrorVisibility = Visibility.Collapsed;
            GC.Collect();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Vm.Directory = (VirtualDirectory)e.Parameter;
            Vm.GrabUserItemList();
        }
    }
}