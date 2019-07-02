using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views;
using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;

namespace Cafeine.ViewModels
{
    public sealed class HomeViewModel : ViewModelBase
    {
        #region setup
        public ReactiveProperty<Visibility> SearchBoxLoad { get; }
        public CafeineCommand LogOutButton { get; }
        public UserAccountModel AvatarURL { get; set; }
        public CafeineCommand SearchButtonClicked { get; }
        #endregion

        #region properties;
        public string NavigationTitle 
        {
            get => _navigationtile;
            set => Set(ref _navigationtile, value);
        }
        public Visibility BackButtonVisibility 
        {
            get => _backbuttonvisibility;
            set => Set(ref _backbuttonvisibility, value);
        }
        public bool SearchBoxFocus 
        {
            get => _searchboxfocus;
            set => Set(ref _searchboxfocus, value);
        }
        public Visibility SearchButtonLoad 
        {
            get => _searchbuttonload;
            set => Set(ref _searchbuttonload, value);
        }
        public string SuggestText 
        {
            get => _suggesttext;
            set => Set(ref _suggesttext, value);
        }
        #endregion

        #region fields;
        private string _navigationtile;
        private Visibility _backbuttonvisibility;
        private bool _searchboxfocus;
        private Visibility _searchbuttonload;
        private string _suggesttext;
        #endregion

        public HomeViewModel()
        {

            NavigationTitle = "Details";
            BackButtonVisibility = Visibility.Collapsed;
            SearchButtonLoad = Visibility.Visible;

            SearchBoxLoad = new ReactiveProperty<Visibility>(Visibility.Collapsed);
            SearchBoxLoad.Subscribe( _ => SearchButtonLoad = (SearchBoxLoad.Value == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed);
            SearchBoxFocus = false;

            SearchButtonClicked = new CafeineCommand(
                () =>
                    {
                        SearchBoxLoad.Value = Visibility.Visible;
                        SearchBoxFocus = true;
                        navigationService.Navigate(typeof(SearchPage));
                    });

            LogOutButton = new CafeineCommand(
                async () => 
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
                        Database.ResetDataBase();
                        //delete cookies
                        HttpBaseProtocolFilter handler = new HttpBaseProtocolFilter();
                        var cookies = handler.CookieManager.GetCookies(new Uri("https://anilist.co"));
                        foreach (var i in cookies) handler.CookieManager.DeleteCookie(i);

                        //navigate to login
                        Frame rootframe = Window.Current.Content as Frame;
                        rootframe.Navigate(typeof(LoginPage));
                        rootframe.BackStack.Clear();
                    }
                });


            Link.Subscribe(
                (Visibility e) =>
                {
                    SearchBoxLoad.Value = e;
                    SearchBoxFocus = false;
                }
                    , typeof(SearchBoxVisibility));

            Link.Subscribe(
                () =>
                    {
                        AvatarURL = Database.GetCurrentUserAccount() ?? null;
                        RaisePropertyChanged(nameof(AvatarURL));
                    }
                    , typeof(HomePageAvatarLoad));

            NavigationService.EnableBackButton += (s, e) => BackButtonVisibility = e;
        }

        public Frame ChildFrame { get; set; } = new Frame();

        public void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            Link.Publish(SuggestText, typeof(Keyword));
        }
    }
}
