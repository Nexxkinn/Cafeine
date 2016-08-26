using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cafeine.Data;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && e.Parameter.ToString() == "usr_ava")
            {
                textBlock.Text = "Login...";
                var autologin = new Logincredentials().getcredentialfromlocker(1);
                autologin.RetrievePassword();
                Logincredentials login = new Logincredentials();
                bool canweusethepassword = await login.logincredential(autologin.UserName, autologin.Password, 1);
                switch (canweusethepassword) {
                    case true: await FileData.GrabUserDatatoOffline(1);
                        await FileData.GrabUserDatatoOffline(2);
                        Frame nextpage = Window.Current.Content as Frame;
                        Window.Current.Content = new Pages.Shell(nextpage);
                        nextpage.Navigate(typeof(Animelist));
                        Window.Current.Activate();
                        break;
                    case false: textBlock.Text = "Wrong username and/or password. Try again"; break;
                }
                
            }
        }
        public LoginPage()
        {
            InitializeComponent();
        }
      /*  private async void result(object sender, RoutedEventArgs e)
        {
            try
            {
                var getlogincredentials = new GrabUserList().getcredentialfromlocker();
                getlogincredentials.RetrievePassword();
                var url = new Uri("http://myanimelist.net/api/account/verify_credentials.xml");
                //string storeusercredential_myanimelist = "mall";
                GrabUserList nn = new GrabUserList();

                var clientHeader = new HttpBaseProtocolFilter();
                clientHeader.ServerCredential = new PasswordCredential(getlogincredentials.Resource,getlogincredentials.UserName, getlogincredentials.Password);
                clientHeader.AllowUI = false;
                using (var client = new HttpClient(clientHeader))
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string getrespond = await response.Content.ReadAsStringAsync();
                    //store password
                    //var vault = new PasswordVault();
                    //var cred = clientHeader.ServerCredential;
                    //vault.Add(cred);
                    test.Content = getrespond;
                }
                #region oldcode_but_works
                // old one, but works
                // problem : can't disable basic authentication pop-up when failed to login.
                /*using (HttpClient client = new HttpClient()) {
                    //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("basic",Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();
                        string getresponse = await response.Content.ReadAsStringAsync();
                        test.Content = getresponse;
                    }
                  }
                
                #endregion
            }
            catch (Exception ex) {
                test.Content = "Maa-kun confused. She can't use your username / password to access it :(";
                }
        }*/

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Logincredentials login = new Logincredentials();
            bool verify = await login.logincredential(usrnm.Text, psswd.Password, 1);
            switch (verify)
            {
                case true: Frame.Navigate(typeof(Animelist), null); break;
                case false: MAL_login.Content = "nay yo";break;
            }
        }

        private void MAL_click(object sender, RoutedEventArgs e)
        {
            FindName("userlogin");
            loginpage.SelectedItem = userlogin;
        }
    }
}
