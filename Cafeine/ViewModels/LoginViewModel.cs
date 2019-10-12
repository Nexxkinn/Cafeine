using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Api;
using Cafeine.Services.Mvvm;
using Cafeine.Shared.Services;
using Cafeine.Views;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public bool UserPanelVisibility;

        public bool SetupPanelVisibility;

        public UserAccountModel CurrentUserAccount;

        public string welcometext;

        public bool FromWebsiteRegistration = false;

        private IAuthService AuthService;

        public LoginViewModel() { }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back) {
                FromWebsiteRegistration = true;
            }
            navigationService.ClearHistory();
            await base.OnNavigatedTo(e);
            await Load();
        }

        public async Task Load()
        {
            await Task.Yield();
            await ImageCache.CreateImageCacheFolder();
            if (Database.DoesAccountExists())
            {
                showUserPanel();
                await Database.BuildServices();
                await Database.SyncCurrentService();
                NavigateToMainPage();
            }
            else
            {
                SetupPanelVisibility = true;
                RaisePropertyChanged(nameof(SetupPanelVisibility));
            }
        }

        public async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            switch (e.ClickedItem as string)
            {
                case "AniList":
                    {
                        AuthService = new AniList();
                        await LoadBrowser(new Uri("https://anilist.co/api/v2/oauth/authorize?client_id=873&response_type=token"));
                        break;
                    }

                default:
                    break;
            }
        }
        

        public void showUserPanel()
        {
            CurrentUserAccount = Database.GetCurrentUserAccount();
            welcometext = $"Welcome back, {CurrentUserAccount.Name}";
            UserPanelVisibility = true;
            SetupPanelVisibility = false;
            RaisePropertyChanged(nameof(welcometext));
            RaisePropertyChanged(nameof(CurrentUserAccount));
            RaisePropertyChanged(nameof(UserPanelVisibility));
            RaisePropertyChanged(nameof(SetupPanelVisibility));
        }

        public void NavigateToMainPage()
        {
#if DEBUG
            Database.DEBUGMODE();
#endif
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(HomePage), null, new DrillInNavigationTransitionInfo());
            Link.Publish(typeof(HomePageAvatarLoad));
            navigationService.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Authentication from browser.
        /// </summary>
        /// <param name="uriAuth"></param>
        public async Task LoadBrowser(Uri uriAuth)
        {
            // enable shadow drop
            // CODE HERE

            // add event
            App.AuthenticationResult += AuthenticationBrowserStatus;
            // open link on another window
            await Windows.System.Launcher.LaunchUriAsync(uriAuth);
        }

        public async void AuthenticationBrowserStatus(object sender,Uri result)
        {
            App.AuthenticationResult -= AuthenticationBrowserStatus;

            if(AuthService is AniList)
            {
                if (result.Host != "anilist") return; // consider authentication failed.

                Regex r = new Regex(@"(?<=token=).*?(?=&)");
                Match m = r.Match(result.Fragment);

                await AuthService.VerifyAccount(m.Value);

                var AnilistAccount = await AuthService.CreateAccount(true);
                Database.AddAccount(AnilistAccount);
            }

            // TODO: Refactor this
            await Load();
        }

        public void AuthenticationCanceled(object sernder,object res)
        {
            App.AuthenticationResult -= AuthenticationBrowserStatus;
            // disable shadow drop
        }
    }
}
