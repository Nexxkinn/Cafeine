using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;
using Cafeine.Data;
using Windows.UI.ViewManagement;
using Windows.Foundation;

namespace Cafeine
{
    public sealed partial class Animelist : Page
    {
        Frame f;
        public Animelist(Frame frame)
        {
            InitializeComponent();
            f = frame;
            ContentFrame.Content = f;
            f.Navigated += ContentFrame_Navigated;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (f.CanGoBack)
            {
                e.Handled = true;
                f.GoBack();
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            switch (f.CurrentSourcePageType.ToString())
            {
                case "Cafeine.Pages.CollectionLibrary":
                    Library.IsChecked = true;
                    break;
            }
            if (f.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void TabChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name.ToString())
            {
                case "Schedule":
                    f.Navigate(typeof(Pages.Schedule));
                    break;
                case "Library":
                    f.Navigate(typeof(Pages.CollectionLibrary));
                    break;
            }
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
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

        }
    }
}
