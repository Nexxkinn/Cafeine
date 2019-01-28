using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {

        public ReactiveCommand LoginPageLoaded { get; }

        public ReactiveCommand<string> LoginClicked { get; }

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

            LoginPageLoaded = new ReactiveCommand();
            LoginPageLoaded.Subscribe(async () => await LoadTask());

            LoginClicked = new ReactiveCommand<string>();
            LoginClicked.Subscribe(item =>
            {
                switch (item)
                {
                    case "Anilist":
                        {
                            _navigationService.Navigate(typeof(BrowserAuthenticationPage));

                            break;
                        }
                }
            });
        }
        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back) {
                FromWebsiteRegistration = true;
            }
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e);
        }

        public async Task LoadTask()
        {

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
