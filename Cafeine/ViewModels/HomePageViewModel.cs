using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;

namespace Cafeine.ViewModels
{
    public sealed class HomePageViewModel : ViewModelBase
    {
        private NavigationService _navigationService;
        private ViewModelLink _eventAggregator;
        #region setup
        public ReactiveCommand GoBackButton { get; }

        public ReactiveProperty<string> NavigationTitle { get; }

        public ReactiveCommand LogOutButton { get; }

        public ReactiveProperty<Visibility> SearchBoxLoad { get; }

        public ReactiveProperty<bool> SearchBoxFocus { get; }

        public ReactiveProperty<bool> SearchButtonLoad { get; }

        public ReactiveCommand SearchButtonClicked { get; }

        public ReactiveCommand SearchBoxLostFocused { get; }
        
        public ReactiveProperty<int> TabbedIndex { get; }

        public ReactiveProperty<string> SuggestText { get; }

        public ReactiveProperty<GridLength> InvisibleTab { get; }

        public ReactiveProperty<bool> WatchHoldPivot_Visibility { get; }

        public ReactiveProperty<bool> DetailsTab_Visibility { get; }

        public UserAccountModel AvatarURL { get; set; }
        #endregion

        public HomePageViewModel()
        {
            _navigationService = new NavigationService();
            _eventAggregator = new ViewModelLink();

            InvisibleTab = new ReactiveProperty<GridLength>(new GridLength(0, GridUnitType.Star));
            WatchHoldPivot_Visibility = new ReactiveProperty<bool>(true);
            DetailsTab_Visibility = new ReactiveProperty<bool>(false);

            SuggestText = new ReactiveProperty<string>();

            NavigationTitle = new ReactiveProperty<string>("Details");

            SearchButtonLoad = new ReactiveProperty<bool>();
            SearchBoxLoad = new ReactiveProperty<Visibility>(Visibility.Collapsed);
            SearchBoxLoad.Subscribe(x => SearchButtonLoad.Value = (SearchBoxLoad.Value == Visibility.Collapsed));
            SearchBoxFocus = new ReactiveProperty<bool>();

            SearchButtonClicked = new ReactiveCommand();
            SearchButtonClicked.Subscribe(_ =>
            {
                SearchBoxLoad.Value = Visibility.Visible;
                _eventAggregator.Publish(1, typeof(NavigateSearchPage));
                SearchBoxFocus.Value = true;
            });

            GoBackButton = new ReactiveCommand();
            GoBackButton.Subscribe(_ => {
                _navigationService.GoBack();
            });

            LogOutButton = new ReactiveCommand();
            LogOutButton.Subscribe(async _ =>
            {
                MessageDialog popup = new MessageDialog("Do you want to log out?", "Logout");
                popup.Commands.Add(new UICommand("Logout") { Id = 0 });
                popup.Commands.Add(new UICommand("Cancel") { Id = 1 });
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var result = await popup.ShowAsync();

                if ((int)result.Id == 0)
                {
                    //remove user credentials

                    //drop database
                    Database.ResetAll();
                    //delete cookies
                    HttpBaseProtocolFilter handler = new HttpBaseProtocolFilter();
                    var cookies = handler.CookieManager.GetCookies(new Uri("https://anilist.co"));
                    foreach (var i in cookies) handler.CookieManager.DeleteCookie(i);

                    //navigate to login
                    _navigationService.Navigate(typeof(LoginPage));
                }
            });

            TabbedIndex = new ReactiveProperty<int>();
            TabbedIndex.Subscribe(
                x =>
                    {
                        _eventAggregator.Publish(x, typeof(LoadItemStatus));
                    });
            _eventAggregator.Subscribe(
                () =>
                    {
                        AvatarURL = Database.GetCurrentUserAccount() ?? null;
                        RaisePropertyChanged(nameof(AvatarURL));
                    }
                    , typeof(HomePageAvatarLoad));
        }

        public Frame ChildFrame { get; set; } = new Frame();

        public void SetWindowState(string page)
        {
            switch (page)
            {
                case nameof(LoginPage):
                    InvisibleTab.Value = new GridLength(0, GridUnitType.Star);
                    break;
                case nameof(MainPage):
                    InvisibleTab.Value = new GridLength();
                    WatchHoldPivot_Visibility.Value = true;
                    DetailsTab_Visibility.Value = false;
                    SearchBoxLoad.Value = Visibility.Collapsed;
                    break;
                case nameof(ItemDetailsPage):
                    InvisibleTab.Value = new GridLength();
                    WatchHoldPivot_Visibility.Value = false;
                    DetailsTab_Visibility.Value = true;
                    SearchBoxLoad.Value = Visibility.Collapsed;
                    NavigationTitle.Value = "Details";
                    break;
                case nameof(SearchPage):
                    InvisibleTab.Value = new GridLength();
                    WatchHoldPivot_Visibility.Value = false;
                    DetailsTab_Visibility.Value = true;
                    SearchBoxLoad.Value = Visibility.Visible;
                    NavigationTitle.Value = "Search";
                    break;
                default:
                    break;

            }
        }

        public void SetFrame(Frame frame)
        {
            frame.Navigating += Frame_PreventGoFordWard;
            ChildFrame.Content = frame;
        }

        public void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            _eventAggregator.Publish(SuggestText.Value, typeof(Keyword));
        }
        //Prevent any kind of input to forward the frame.
        private void Frame_PreventGoFordWard(object sender, NavigatingCancelEventArgs e)
        {
            bool b = e.NavigationMode == NavigationMode.Forward;
            if (b)
            {
                e.Cancel = true;
            }
        }
    }
}
