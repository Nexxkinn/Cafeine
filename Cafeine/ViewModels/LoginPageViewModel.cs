using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {

        private NavigationService _navigationService;

        private ViewModelLink _eventAggregator;

        public bool UserPanelVisibility;

        public bool SetupPanelVisibility;

        public UserAccountModel CurrentUserAccount;

        public string welcometext;

        public bool FromWebsiteRegistration = false;

        public LoginPageViewModel()
        {
            _navigationService = new NavigationService();

            _eventAggregator = new ViewModelLink();

            UserPanelVisibility = false;

            SetupPanelVisibility = true;
        }
        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back) {
                FromWebsiteRegistration = true;
            }
            _navigationService.ClearHistory();
            await base.OnNavigatedTo(e);
        }

        public void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            switch (e.ClickedItem as string)
            {
                case "AniList":
                    {
                        _navigationService.Navigate(typeof(BrowserAuthenticationPage));
                        break;
                    }

                default:
                    break;
            }
        }
        public override async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(async () =>
            {
                await ImageCache.CreateImageCacheFolder();
                if (Database.DoesAccountExists())
                {
                    _eventAggregator.Publish(typeof(HomePageAvatarLoad));
                    showUserPanel();
                    await Database.CreateServicesFromUserAccounts();
                    if (FromWebsiteRegistration)
                    {
                        await Database.CreateDBFromServices();
                    }
                    _navigationService.Navigate(typeof(MainPage));
                }
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());
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
    }
}
