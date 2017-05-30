using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Cafeine.Data;
using Windows.UI.Popups;
using System;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;

namespace Cafeine.Pages
{
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
            //coreTitleBar.ExtendViewIntoTitleBar = true;
            //Window.Current.SetTitleBar(dragarea);
            f = frame;
            shellsplit.Content = f;
            f.Navigated += F_Navigated;
        }

        private void F_Navigated(object sender, NavigationEventArgs e)
        {
            //pretty ugly. Worth to recode it.
            if(f.CanGoBack)
            {
                BackBut.Visibility = Visibility.Visible;
                HamBut.Visibility = Visibility.Collapsed;
                AddCollection.SetValue(RelativePanel.BelowProperty, BackBut);
                AddCollection.SetValue(RelativePanel.AlignRightWithProperty, BackBut);
            }
            else
            {

                BackBut.Visibility = Visibility.Collapsed;
                HamBut.Visibility = Visibility.Visible;
                AddCollection.SetValue(RelativePanel.BelowProperty, HamBut);
                AddCollection.SetValue(RelativePanel.AlignRightWithProperty, HamBut);
            }
        }

        private void Backbutton(object sender, RoutedEventArgs e)
        {
            f.GoBack();
        }
        private void AddCollection_Click(object sender, RoutedEventArgs e)
        {
            f.Navigate(typeof(Searchpage));
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
    }
}
