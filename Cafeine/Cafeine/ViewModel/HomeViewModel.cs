using Cafeine.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModel {
    public class HomeViewModel : ViewModelBase {
        private INavigationService _navigationservice;
        private Frame _frame = new Frame();
        private bool? _LibraryTabChecked = true;
        private bool? _ScheduleTabEnabled = false;
        private RelayCommand _logoutCommand;

        public HomeViewModel(INavigationService navigationservice) {
            _navigationservice = navigationservice;
            F.Navigate(typeof(DirectoryExplorer));
            F.Navigated += F_Navigated;
            SystemNavigationManager.GetForCurrentView().BackRequested += HomeViewModel_BackRequested;
        }
        private void HomeViewModel_BackRequested(object sender, BackRequestedEventArgs e) {
            if (F.CanGoBack) {
                e.Handled = true;
                F.GoBack();
            }
        }
        private void F_Navigated(object sender, NavigationEventArgs e) {
            if (F.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        public Frame F {
            get {
                return _frame;
            }
            set {
                switch (F.CurrentSourcePageType.ToString()) {
                    case "Cafeine.DirectoryExplorer": LibraryTabChecked = true; break;
                    case "Cafeine.CollectionLibrary": LibraryTabChecked = true; break;
                }
                Set(() => F, ref _frame, value);
            }
        }
        public bool? LibraryTabChecked {
            get {
                return _LibraryTabChecked;
            }
            set {
                if (_LibraryTabChecked == value) {
                    return;
                }
                _LibraryTabChecked = value;
                RaisePropertyChanged("LibraryTabChecked");
            }
        }
        public bool? ScheduleTabEnabled {
            get {
                return _ScheduleTabEnabled;
            }
            set {
                if (_ScheduleTabEnabled == value) {
                    return;
                }
                _ScheduleTabEnabled = value;
                RaisePropertyChanged("ScheduleTabEnabled");
            }
        }
        public RelayCommand LogOutCommand {
            get {
                return _logoutCommand
                    ?? (_logoutCommand = new RelayCommand(
                    async () => {
                        MessageDialog popup = new MessageDialog("Do you want to log out from this account?", "Logout");
                        popup.Commands.Add(new UICommand("Logout") { Id = 0 });
                        popup.Commands.Add(new UICommand("Cancel") { Id = 1 });
                        popup.DefaultCommandIndex = 0;
                        popup.CancelCommandIndex = 1;
                        var result = await popup.ShowAsync();

                        if ((int)result.Id == 0) {
                            //remove user credentials
                            var getuserpass = Logincredentials.getuser(1);
                            var vault = new Windows.Security.Credentials.PasswordVault();
                            vault.Remove(new Windows.Security.Credentials.PasswordCredential(getuserpass.Resource, getuserpass.UserName, getuserpass.Password));
                            F.Navigate(typeof(LoginPage));
                            Window.Current.Content = F;
                            Window.Current.Activate();
                        }

                    }));
            }
        }
    }
}
