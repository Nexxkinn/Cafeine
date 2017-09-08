using Cafeine.Model;
using Cafeine.Design;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModel {
    public class HomeViewModel : ViewModelBase {
        private INavigationService _navigationservice;
        private ICafeineNavigationService _Fnavigationservice;
        private Frame _frame = new Frame();
        public Frame F {
            get {
                return _frame;
            }
            set {
                Set(() => F, ref _frame, value);
            }
        }

        public HomeViewModel(ICafeineNavigationService caf, INavigationService navigationservice) {
            _navigationservice = navigationservice;
            _Fnavigationservice = caf;
            SystemNavigationManager.GetForCurrentView().BackRequested += HomeViewModel_BackRequested;
            F.Navigated += F_Navigated;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void F_Navigated(object sender, NavigationEventArgs e) {
            if (F.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void HomeViewModel_BackRequested(object sender, BackRequestedEventArgs e) {
            Frame root = (Frame)Window.Current.Content;
            if (F.CanGoBack) {
                e.Handled = true;
                F.GoBack();
            }
        }
        private void SettingPage_BackRequested(object sender, BackRequestedEventArgs e) {
            _navigationservice.GoBack();

            SystemNavigationManager.GetForCurrentView().BackRequested -= SettingPage_BackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += HomeViewModel_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = (F.CanGoBack) ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private RelayCommand _logoutCommand;
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
                            _navigationservice.NavigateTo(ViewModelLocator.SignInPageKey);
                        }

                    }));
            }
        }

        private RelayCommand<string> _AutoSuggestBoxTextChanged;
        public RelayCommand<string> AutoSuggestBoxTextChanged {
            get {
                return _AutoSuggestBoxTextChanged
                    ?? (_AutoSuggestBoxTextChanged = new RelayCommand<string>(
                    async p => {
                        CVS = await SearchProvider.ResultIndex(p);
                    }));
            }
        }

        //TODO : connect it to ExpandItemDialog
        private RelayCommand<GroupedSearchResult> _AutoSuggestBoxQuerySubmited;
        public RelayCommand<GroupedSearchResult> AutoSuggestBoxQuerySubmited {
            get {
                return _AutoSuggestBoxQuerySubmited
                    ?? (_AutoSuggestBoxQuerySubmited = new RelayCommand<GroupedSearchResult>(
                    p => {
                        ExpandItemDialogService.QueryItemExpand(p);
                    }));
            }
        }

        private List<IGrouping<string, GroupedSearchResult>> _cvs = new List<IGrouping<string, GroupedSearchResult>>();
        public List<IGrouping<string, GroupedSearchResult>> CVS {
            get {
                return _cvs;
            }

            set {
                if (_cvs == value) {
                    return;
                }

                _cvs = value;
                RaisePropertyChanged("CVS");
            }
        }
        private RelayCommand _SettingsPage;
        public RelayCommand SettingsPage {
            get {
                return _SettingsPage
                    ?? (_SettingsPage = new RelayCommand(
                    () => {
                        _navigationservice.NavigateTo("SettingsPage");
                        SystemNavigationManager.GetForCurrentView().BackRequested += SettingPage_BackRequested;
                        SystemNavigationManager.GetForCurrentView().BackRequested -= HomeViewModel_BackRequested;
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    }));
            }
        }

        public void TabSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var pivot = (Frame)(((PivotItem)(sender as Pivot).SelectedItem).Content);
            switch (pivot.Name) {
                case "ContentFrame":
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = (F.CanGoBack) ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                break;
                case "RSSFeed":
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = (pivot.CanGoBack) ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                break;
                default:
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                break;
            }
        }
    }
}
