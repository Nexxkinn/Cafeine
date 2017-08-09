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
        private bool? _LibraryTabChecked = true;
        private bool? _ScheduleTabEnabled = false;
        private RelayCommand _logoutCommand;
        private RelayCommand<string> _AutoSuggestBoxTextChanged;
        private RelayCommand<GroupedSearchResult> _AutoSuggestBoxQuerySubmited;
        private List<IGrouping<string, GroupedSearchResult>> _cvs = new List<IGrouping<string, GroupedSearchResult>>();
        private RelayCommand _FNavigated;

        public HomeViewModel(ICafeineNavigationService caf) {
            //_navigationservice = navigationservice;
            _Fnavigationservice = caf;
            SystemNavigationManager.GetForCurrentView().BackRequested += HomeViewModel_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }
        private void HomeViewModel_BackRequested(object sender, BackRequestedEventArgs e) {
            if (F.CanGoBack) {
                e.Handled = true;
                F.GoBack();
            }
        }
        public void test(object sender, RoutedEventArgs e) {
            _Fnavigationservice.NavigateTo("VirDir");
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
                            _navigationservice.NavigateTo(ViewModelLocator.SignInPageKey);
                        }

                    }));
            }
        }
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
        public RelayCommand<GroupedSearchResult> AutoSuggestBoxQuerySubmited {
            get {
                return _AutoSuggestBoxQuerySubmited
                    ?? (_AutoSuggestBoxQuerySubmited = new RelayCommand<GroupedSearchResult>(
                    p => {
                        ExpandItemDialogService.da(p, p.Library.Category);
                    }));
            }
        }

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
        public RelayCommand FNavigated {
            get {
                return _FNavigated
                    ?? (_FNavigated = new RelayCommand(
                    () => {
                        if (F.CanGoBack) {
                            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                        }
                        else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }));
            }
        }
        private RelayCommand _SettingsPage;

        /// <summary>
        /// Gets the SettingsPage.
        /// </summary>
        public RelayCommand SettingsPage {
            get {
                return _SettingsPage
                    ?? (_SettingsPage = new RelayCommand(
                    () => {
                        F.Navigate(typeof(SettingsPage));
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    }));
            }
        }
    }
}
