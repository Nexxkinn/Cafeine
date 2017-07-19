using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;
using Cafeine.Services;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace Cafeine
{
    public sealed partial class HomePage : Page
    {
        Frame f;
        public HomePage(Frame frame)
        {
            InitializeComponent();
            f = frame;
            ContentFrame.Content = f;
            f.Navigated += ContentFrame_Navigated;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                titleBar.BackgroundColor = Application.Current.Resources["SystemChromeMediumColor"] as Color?;
                titleBar.InactiveBackgroundColor = Application.Current.Resources["SystemChromeMediumColor"] as Color?;
                titleBar.ButtonBackgroundColor = Application.Current.Resources["SystemChromeMediumColor"] as Color?;
            }
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
            //ugly hack
            Schedule.Visibility = Visibility.Visible;
            Library.Visibility = Visibility.Visible;
            SettingsTab.Visibility = Visibility.Collapsed;
            switch (f.CurrentSourcePageType.ToString())
            {
                case "Cafeine.DirectoryExplorer": Library.IsChecked = true; break;
                case "Cafeine.CollectionLibrary": Library.IsChecked = true; break;
            }
            if (f.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        //very ugly hack
        private void TabChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name.ToString())
            {
                case "Schedule":
                    f.Navigate(typeof(Pages.Schedule));
                    break;
                case "Library":
                    f.Navigate(typeof(DirectoryExplorer));
                    //f.BackStack.Clear();
                    //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    break;
            }
        }
        private void SettingPage(object sender, RoutedEventArgs e)
        {
            f.Navigate(typeof(SettingsPage));
            Schedule.Visibility = Visibility.Collapsed;
            Library.Visibility = Visibility.Collapsed;
            SettingsTab.Visibility = Visibility.Visible;
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
                var getuserpass = Logincredentials.getuser(1);
                var vault = new Windows.Security.Credentials.PasswordVault();
                vault.Remove(new Windows.Security.Credentials.PasswordCredential(getuserpass.Resource, getuserpass.UserName, getuserpass.Password));
                //navigate back to the loginpage
                f.Navigate(typeof(LoginPage));
                Window.Current.Content = f;
                Window.Current.Activate();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

        }

        private async void Search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text != "")
            {
                cvs.Source = await SearchProvider.ResultIndex(sender.Text);
                Search.ItemsSource = cvs.View;
            }
        }

        private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            f.Navigate(typeof(Searchpage));
        }
    }
}