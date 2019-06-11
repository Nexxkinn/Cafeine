using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views;
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

        public LoginViewModel()
        {
            UserPanelVisibility = false;

            SetupPanelVisibility = true;
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back) {
                FromWebsiteRegistration = true;
            }
            navigationService.ClearHistory();
            await base.OnNavigatedTo(e);
        }

        public void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            switch (e.ClickedItem as string)
            {
                case "AniList":
                    {
                        navigationService.Navigate(typeof(BrowserAuthenticationPage));
                        break;
                    }

                default:
                    break;
            }
        }
        public override async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Yield();
            await ImageCache.CreateImageCacheFolder();
            if (Database.DoesAccountExists())
            {
                showUserPanel();
                await Database.BuildServices();
                await Database.Build();
                NavigateToMainPage();
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
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(HomePage), null, new DrillInNavigationTransitionInfo());
            Link.Publish(typeof(HomePageAvatarLoad));
            navigationService.Navigate(typeof(MainPage));
        }
    }
}
