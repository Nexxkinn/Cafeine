﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cafeine.Data;
using Cafeine.Datalist;
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
            InitializeComponent();
            var data = new LibraryList().querydata(1);
            currentwatch_anime.DataContext = data;
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
