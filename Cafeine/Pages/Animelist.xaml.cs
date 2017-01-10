using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cafeine.Data;
using Cafeine.Datalist;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI;
namespace Cafeine
{
    public sealed partial class Animelist : Page
    {
        public Animelist()
        {
            InitializeComponent();

            //Change Title Bar
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.White;
            titleBar.ButtonForegroundColor = Colors.Black;
            this.Loaded += Animelist_Loaded;
        }
        private void Animelist_Loaded(object sender, RoutedEventArgs e)
        {
            //userlibrary = LibraryList.querydata(1);
            Task.Run(async () => await getonit());
            //Task.Run(async () => userlibrary = await LibraryList.querydata(1));
        }
        async Task getonit()
        {
            try
            {
                var list = await LibraryList.querydata(1);
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    parah.ItemsSource = list;
                });
            }
            catch (Exception e)
            {
                //TODO: show error message?? e.Message
                //to the msdn it goes
            }
        }
        private void Logout_test(object sender, RoutedEventArgs e)
        {
            //remove user credentials
            var getuserpass = new Logincredentials().getcredentialfromlocker(1);
            getuserpass.RetrievePassword();
            var vault = new Windows.Security.Credentials.PasswordVault();
            vault.Remove(new Windows.Security.Credentials.PasswordCredential(getuserpass.Resource, getuserpass.UserName, getuserpass.Password));
            Frame.Navigate(typeof(LoginPage), null);
        }

        private void NavigateItemtoDetailsPage(object sender, ItemClickEventArgs e)
        {
            //pass data to other page
            var SelectedItem = (UserItemCollection)e.ClickedItem;
            Frame.Navigate(typeof(Pages.MoreDetails),SelectedItem);
        }
    }
}
