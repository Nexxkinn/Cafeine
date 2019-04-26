﻿using Cafeine.Models;
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
        public CafeineProperty<string> NavigationTitle { get; }

        public CafeineCommand LogOutButton { get; }

        public CafeineProperty<Visibility> SearchBoxLoad { get; }

        public CafeineProperty<bool> SearchBoxFocus { get; }

        public CafeineProperty<bool> SearchButtonLoad { get; }

        public CafeineCommand SearchButtonClicked { get; }

        public CafeineProperty<string> SuggestText { get; }

        public UserAccountModel AvatarURL { get; set; }
        #endregion

        public HomeViewModel()
        {
            SuggestText = new CafeineProperty<string>();

            NavigationTitle = new CafeineProperty<string>("Details");

            SearchButtonLoad = new CafeineProperty<bool>();
            SearchBoxLoad = new CafeineProperty<Visibility>(Visibility.Collapsed);
            SearchBoxLoad.PropertyChanged += (x,y) => SearchButtonLoad.Value = (SearchBoxLoad.Value == Visibility.Collapsed);
            SearchBoxFocus = new CafeineProperty<bool>();

            SearchButtonClicked = new CafeineCommand(
                () =>
                    {
                        SearchBoxLoad.Value = Visibility.Visible;
                        SearchBoxFocus.Value = true;
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
                        Database.ResetAll();
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
                () =>
                    {
                        SearchBoxLoad.Value = Visibility.Collapsed;
                        SearchBoxFocus.Value = false;
                    }
                    , typeof(RevertSearchBox));

            Link.Subscribe(
                () =>
                    {
                        AvatarURL = Database.GetCurrentUserAccount() ?? null;
                        RaisePropertyChanged(nameof(AvatarURL));
                    }
                    , typeof(HomePageAvatarLoad));
        }

        public Frame ChildFrame { get; set; } = new Frame();

        public void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            Link.Publish(SuggestText.Value, typeof(Keyword));
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
