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

        public LoginPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;

            _eventAggregator = eventAggregator;

            UserPanelVisibility = false;

            SetupPanelVisibility = true;

            LoginPageLoaded = new DelegateCommand(async () =>
                {
                    if (!Database.IsAccountEmpty())
                    {
                        CurrentUserAccount = Database.GetCurrentUserAccount();
                        showUserPanel();
                        _eventAggregator.GetEvent<HomePageAvatarLoad>().Publish();
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        _navigationService.Navigate("Main", null);
                    }

                });

            LoginClicked = new ReactiveCommand<string>();
            LoginClicked.Subscribe(item =>
            {
                switch (item)
                {
                    case "Anilist":
                        {
                            _eventAggregator.GetEvent<IsKeyAvailable>().Subscribe(async x => await AnilistLogin(x));
                            _navigationService.Navigate("BrowserAuthentication", null);
                            break;
                        }
                }
            });
        }

        public async Task AnilistLogin(bool x)
        {
            IService service = new AniListApi();
            await service.VerifyAccount();
            CurrentUserAccount = await service.CreateAccount(true);
            Database.AddAccount(CurrentUserAccount);
            showUserPanel();
            await Database.CreateDBFromServices();
            _eventAggregator.GetEvent<HomePageAvatarLoad>().Publish();
            _navigationService.Navigate("Main", null);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e, viewModelState);
        }

        public void showUserPanel()
        {
            welcometext = $"Welcome back, {CurrentUserAccount.Name}";
            UserPanelVisibility = true;
            SetupPanelVisibility = false;
            RaisePropertyChanged(nameof(welcometext));
            RaisePropertyChanged(nameof(CurrentUserAccount));
            RaisePropertyChanged(nameof(UserPanelVisibility));
            RaisePropertyChanged(nameof(SetupPanelVisibility));
        }
    }

    public class IsKeyAvailable : PubSubEvent<bool>
    {
    }
}
