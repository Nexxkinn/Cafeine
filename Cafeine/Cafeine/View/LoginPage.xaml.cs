using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.UI;

namespace Cafeine
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage() {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null) {
                titleBar.BackgroundColor = Color.FromArgb(255, 47, 52, 59);
                titleBar.InactiveBackgroundColor = Color.FromArgb(255, 47, 52, 59);
                titleBar.ButtonBackgroundColor = Color.FromArgb(255, 47, 52, 59);
            }
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
          
        //private void Navigate() {
        //    Frame rootframe = new Frame();
        //    Frame newframe = new Frame();
        //    rootframe.Content = new HomePage(newframe);
        //    newframe.Navigate(typeof(DirectoryExplorer));
        //    Window.Current.Content = rootframe;
        //    Window.Current.Activate();
        //}
        
    }
}