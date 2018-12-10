using Cafeine.Models;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
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
    public sealed class HomePageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        private IEventAggregator _eventAggregator;

        public ReactiveCommand GoBackButton { get; }

        public ReactiveCommand LogOutButton { get; }

        public ReactiveProperty<int> TabbedIndex { get; }

        public ReactiveProperty<string> SuggestText { get; }

        public ReactiveCommand<ItemLibraryModel> SuggestionChosen { get; }

        public ReactiveCollection<ItemLibraryModel> SuggestItemSource { get; }

        public ReactiveProperty<GridLength> InvisibleTab { get; }

        public ReactiveProperty<bool> WatchHoldPivot_Visibility { get; }

        public ReactiveProperty<bool> DetailsTab_Visibility { get; }

        public UserAccountModel AvatarURL { get; set; }

        public HomePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            InvisibleTab = new ReactiveProperty<GridLength>(new GridLength(0, GridUnitType.Star));
            WatchHoldPivot_Visibility = new ReactiveProperty<bool>(true);
            DetailsTab_Visibility = new ReactiveProperty<bool>(false);

            SuggestText = new ReactiveProperty<string>();
            // RX.NET RANT:
            // Not implementing .ObserveOnDispatcher() causes
            //
            // System.Runtime.InteropServices.COMException with RPC_E_WRONG_THREAD tag
            //
            // On Throttle() method but no guideline or documentation mentioned it.
            // Even worse, only one source EVER give you a proper example of it.
            // http://rxwiki.wikidot.com/101samples --> Throttle - Simple.
            // Good job RX.Net and all of their team 👏.
            SuggestText.Throttle(TimeSpan.FromSeconds(.5)).ObserveOnDispatcher()
                .Subscribe(x =>
               {
                   SuggestItemSource?.Clear();
                   if (x != null && x != "")
                   {
                       var finditem = Database.SearchItemCollection(x);
                       foreach (var item in finditem)
                       {
                            //item.Service["default"].CoverImage = await ImageCache.GetFromCacheAsync(item.Service["default"].CoverImageUri);
                            SuggestItemSource?.Add(item);
                       }
                   }
               });
            SuggestionChosen = new ReactiveCommand<ItemLibraryModel>()
                .WithSubscribe(x => _eventAggregator.GetEvent<NavigateItem>().Publish(x));
            SuggestItemSource = new ReactiveCollection<ItemLibraryModel>();

            GoBackButton = new ReactiveCommand();
            GoBackButton.Subscribe(_ => {
                _navigationService.GoBack();
                _eventAggregator.GetEvent<LoadItemStatus>().Publish(TabbedIndex.Value);
            }
                );

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
                    _navigationService.Navigate("Login", null);
                    _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(1);
                }
            });

            TabbedIndex = new ReactiveProperty<int>();
            TabbedIndex.Subscribe(x =>
            {
                _eventAggregator.GetEvent<LoadItemStatus>().Publish(x);
            });

            _eventAggregator.GetEvent<ChildFrameNavigating>().Subscribe(x =>
            {

                switch (x)
                {
                    case 1:
                        InvisibleTab.Value = new GridLength(0, GridUnitType.Star);
                        break;
                    case 2:
                        InvisibleTab.Value = new GridLength();
                        WatchHoldPivot_Visibility.Value = true;
                        DetailsTab_Visibility.Value = false;
                        break;
                    case 3:
                        InvisibleTab.Value = new GridLength();
                        WatchHoldPivot_Visibility.Value = false;
                        DetailsTab_Visibility.Value = true;
                        break;
                    //TODO: add functionality when settings made.
                    case 4:
                        break;

                }
            });

            AvatarURL = Database.GetCurrentUserAccount() ?? null;
        }

        public Frame ChildFrame { get; set; } = new Frame();

        public void SetFrame(Frame frame)
        {
            frame.Navigating += Frame_PreventGoFordWard;
            ChildFrame.Content = frame;
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

    /// <summary>
    /// 1 : Collapse tab frame
    /// 2 : Show Watching, Hold, etc. pivot
    /// 3 : Show Details pivot
    /// 4 : Show Settings text.
    /// </summary>
    /// <param name="state"></param>
    public class ChildFrameNavigating : PubSubEvent<int>
    {
    }
}
