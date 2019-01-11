using Cafeine.Models;
using Cafeine.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Cafeine.ViewModels
{
    public class LoginPageViewModel : ViewModelBase, INavigationAware
    {
        public DelegateCommand LoginPageLoaded { get; }

        public ReactiveCommand<string> LoginClicked { get; }

        private INavigationService _navigationService;

        private IEventAggregator _eventAggregator;

        public bool UserPanelVisibility;

        public bool SetupPanelVisibility;

        public UserAccountModel CurrentUserAccount;

        public string welcometext;

        public bool FromWebsiteRegistration = false;

        public LoginPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;

            _eventAggregator = eventAggregator;

            UserPanelVisibility = false;

            SetupPanelVisibility = true;

            LoginPageLoaded = new DelegateCommand(async () => await LoadTask());

            LoginClicked = new ReactiveCommand<string>();
            LoginClicked.Subscribe(item =>
            {
                switch (item)
                {
                    case "Anilist":
                        {
                            _navigationService.Navigate("BrowserAuthentication", null);
                            break;
                        }
                }
            });
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back) {
                FromWebsiteRegistration = true;
                Task.Factory.StartNew(async () => await LoadTask(),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e, viewModelState);
        }

        public async Task LoadTask()
        {
            showUserPanel();
            _eventAggregator.GetEvent<HomePageAvatarLoad>().Publish();
            await Database.CreateServicesFromUserAccounts();

            if (!Database.IsAccountEmpty() && FromWebsiteRegistration)
            {
                await Database.CreateDBFromServices();
            }
            _navigationService.Navigate("Main", null);
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
