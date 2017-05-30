using Cafeine.Data;
using Cafeine.Properties;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionLibrary : Page
    {
        public CollectionLibrary()
        {
            this.InitializeComponent();
            this.Loaded += Animelist_Loaded;
        }

        private void Animelist_Loaded(object sender, RoutedEventArgs e)
        {
            //userlibrary = LibraryList.querydata(1);
            Task.Run(async () => await grabuserprofile());
            //Task.Run(async () => userlibrary = await LibraryList.querydata(1));
        }
        async Task grabuserprofile()
        {
            try
            {
                var Librarylist = await LibraryList.QueryUserAnimeMangaListAsync(1);
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    watch.ItemsSource = Librarylist.Where(x => x.My_status == 1);
                });
            }
            catch (Exception e)
            {
                //TODO: show error message?? e.Message
                //to the msdn it goes
            }
        }
        private void NavigateItemtoDetailsPage(object sender, ItemClickEventArgs e)
        {
            //pass data to other page
            var SelectedItem = (ItemProperties)e.ClickedItem;
            Frame.Navigate(typeof(MoreDetails), SelectedItem);
        }
    }
}
