using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Cafeine.Data;

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
        public Shell(Frame frame)
        {
            InitializeComponent();
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(dragarea);
            //coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            shellsplit.Content = frame;

        }
        public void Showshell(object sender, RoutedEventArgs e)
        {
            shellsplit.IsPaneOpen = !shellsplit.IsPaneOpen;
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
    }
}
