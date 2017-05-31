using Cafeine.Services;
using Cafeine.Models;
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
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var DataReceived = (VirtualDirectory)e.Parameter;

            //userlibrary = LibraryList.querydata(1);
            Task.Run(async () => await GrabUserItemList(DataReceived));
            //Task.Run(async () => userlibrary = await LibraryList.querydata(1));
        }
        async Task GrabUserItemList(VirtualDirectory Datareceived)
        {
            try
            {
                switch(Datareceived.AnimeOrManga)
                {
                    case true:
                        {
                            var Librarylist = await LibraryList.QueryUserAnimeMangaListAsync(true);
                            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                                watch.ItemsSource = Librarylist.Where(x => x.My_status == Datareceived.DirectoryType - 3);
                            });
                            break;
                        }
                    case false:
                        {
                            var Librarylist = await LibraryList.QueryUserAnimeMangaListAsync(false);
                            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                                watch.ItemsSource = Librarylist.Where(x => x.My_status == Datareceived.DirectoryType - 3);
                            });
                            break;
                        }
                }
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
