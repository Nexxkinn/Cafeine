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
        private bool? _FeedTabChecked = false;

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
                AutoTabChecked();
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
                Set(() => F, ref _frame, value);
            }
        }
        public void AutoTabChecked() {
            try {
                switch (F.CurrentSourcePageType.ToString()) {
                    case "Cafeine.DirectoryExplorer":
                    LibraryTabChecked = true;
                    ScheduleTabChecked = false;
                    FeedTabChecked = false;
                    break;

                    case "Cafeine.CollectionLibrary":
                    LibraryTabChecked = true;
                    ScheduleTabChecked = false;
                    FeedTabChecked = false;
                    break;

                    case "Cafeine.TorrentManager":
                    LibraryTabChecked = false;
                    ScheduleTabChecked = false;
                    FeedTabChecked = true;
                    break;
                }
            }
            catch (Exception) {

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
        public bool? ScheduleTabChecked {
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
        public bool? FeedTabChecked {
            get {
                return _FeedTabChecked;
            }
            set {
                if (_FeedTabChecked == value) {
                    return;
                }
                _FeedTabChecked = value;
                RaisePropertyChanged("FeedTabChecked");
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
                        ExpandItemDialogService.QueryItemExpand(p);
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
        public RelayCommand SettingsPage {
            get {
                return _SettingsPage
                    ?? (_SettingsPage = new RelayCommand(
                    () => {
                        _Fnavigationservice.NavigateTo("SettingsPage");
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    }));
            }
        }

        private RelayCommand<object> _tabChecked;
        public RelayCommand<object> TabChecked {
            get {
                return _tabChecked
                    ?? (_tabChecked = new RelayCommand<object>(
                    p => {
                        var x = (RoutedEventArgs)p;
                        var xx = (RadioButton)x.OriginalSource;
                        switch (xx.Name.ToString()) {
                            case "TManager":
                            _Fnavigationservice.NavigateTo("TorentManager");
                            break;
                            case "Library":
                            _Fnavigationservice.NavigateTo("VirDir");
                            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                            break;
                        }
                    }));
            }
        }
    }
}
