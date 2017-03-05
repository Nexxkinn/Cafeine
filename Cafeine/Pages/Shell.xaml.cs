using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Cafeine.Data;
using Windows.UI.Popups;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        private CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        //private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        //{
        //    dragarea.Height = sender.Height;
        //    HamBut.Margin = new Thickness(0, dragarea.Height, 0, 0);
        //}
        Frame f;
        public Shell(Frame frame)
        {
            InitializeComponent();
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(dragarea);
            f = frame;
            shellsplit.Content = f;
        }

        private async void Logout_test(object sender, RoutedEventArgs e)
        {
            MessageDialog popup = new MessageDialog("Do you want to log out from this account?", "Logout");
            popup.Commands.Add(new UICommand("Logout") { Id = 0 });
            popup.Commands.Add(new UICommand("Cancel") { Id = 1 });
            popup.DefaultCommandIndex = 0;
            popup.CancelCommandIndex = 1;
            var result = await popup.ShowAsync();

            if ((int)result.Id == 0)
            {
                //remove user credentials
                var getuserpass = new Logincredentials().getcredentialfromlocker(1);
                getuserpass.RetrievePassword();
                var vault = new Windows.Security.Credentials.PasswordVault();
                vault.Remove(new Windows.Security.Credentials.PasswordCredential(getuserpass.Resource, getuserpass.UserName, getuserpass.Password));
                //navigate back to the loginpage
                f.Navigate(typeof(LoginPage));
                Window.Current.Content = f;
                Window.Current.Activate();
            }
            


        }

        private void AddCollection_Click(object sender, RoutedEventArgs e)
        {
            f.Navigate(typeof(Searchpage));
        }
    }
}
