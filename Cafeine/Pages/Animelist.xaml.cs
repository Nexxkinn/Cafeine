using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cafeine.Datalist;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;

namespace Cafeine
{
    public sealed partial class Animelist : Page
    {
        public Animelist()
        {
            InitializeComponent();
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
                var Librarylist = await LibraryList.QueryUserAnimeMangaListAsync(1,1);
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    watch.ItemsSource = Librarylist.Where(x => x.My_status == 1);
                    OnHold.ItemsSource = Librarylist.Where(x => x.My_status == 2);
                });
            }
            catch (Exception e)
            {
                //TODO: show error message?? e.Message
                //to the msdn it goes
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
            private void NavigateItemtoDetailsPage(object sender, ItemClickEventArgs e)
        {
            //pass data to other page
            var SelectedItem = (UserItemCollection)e.ClickedItem;
            Frame.Navigate(typeof(MoreDetails),SelectedItem);
        }
    }
}
