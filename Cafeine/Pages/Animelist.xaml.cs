using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Cafeine.Data;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Animelist : Page
    {
        public Animelist()
        {
            this.InitializeComponent();
            //manual offline xml data
            //GrabUserList grab = new GrabUserList();
            //currentwatch_anime.DataContext = grab.querydata(1, 1);
            //GrabUserList nana = new GrabUserList();
            //textBox.Text = nana.getcredentialfromlocker();
        }

        private void logout_test(object sender, RoutedEventArgs e)
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
