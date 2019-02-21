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
        #region setup
        public CafeineProperty<string> NavigationTitle { get; }

        public CafeineCommand LogOutButton { get; }

        public CafeineProperty<Visibility> SearchBoxLoad { get; }

        public CafeineProperty<bool> SearchBoxFocus { get; }

        public CafeineProperty<bool> SearchButtonLoad { get; }

        public CafeineCommand SearchButtonClicked { get; }
        
        public ReactiveProperty<int> TabbedIndex { get; }

        public CafeineProperty<string> SuggestText { get; }

        public CafeineProperty<GridLength> InvisibleTab { get; }

        public CafeineProperty<bool> WatchHoldPivot_Visibility { get; }

        public CafeineProperty<bool> DetailsTab_Visibility { get; }

        public UserAccountModel AvatarURL { get; set; }
        #endregion

        public HomePageViewModel()
        {
            InvisibleTab = new CafeineProperty<GridLength>(new GridLength(0, GridUnitType.Star));
            WatchHoldPivot_Visibility = new CafeineProperty<bool>(true);
            DetailsTab_Visibility = new CafeineProperty<bool>(false);

            SuggestText = new CafeineProperty<string>();

            NavigationTitle = new CafeineProperty<string>("Details");

            SearchButtonLoad = new CafeineProperty<bool>();
            SearchBoxLoad = new CafeineProperty<Visibility>(Visibility.Collapsed);
            SearchBoxLoad.PropertyChanged += (x,y) => SearchButtonLoad.Value = (SearchBoxLoad.Value == Visibility.Collapsed);
            SearchBoxFocus = new CafeineProperty<bool>();

            SearchButtonClicked = new CafeineCommand(()=>
            {
                SearchBoxLoad.Value = Visibility.Visible;
                eventAggregator.Publish(1, typeof(NavigateSearchPage));
                SearchBoxFocus.Value = true;
            });

            LogOutButton = new CafeineCommand(async()=> 
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
                    navigationService.Navigate(typeof(LoginPage));
                }
            });

            TabbedIndex = new ReactiveProperty<int>();
            TabbedIndex.Subscribe(
                x =>
                    {
                        eventAggregator.Publish(x, typeof(LoadItemStatus));
                    });
            eventAggregator.Subscribe(
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
            eventAggregator.Publish(SuggestText.Value, typeof(Keyword));
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
