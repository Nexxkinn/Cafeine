using Cafeine.Models;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
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
        private INavigationService _navigationService;

        private IEventAggregator _eventAggregator;

        #region setup
        public ReactiveCommand GoBackButton { get; }

        public ReactiveProperty<string> NavigationTitle { get; }

        public ReactiveCommand LogOutButton { get; }

        public ReactiveProperty<Visibility> SearchBoxLoad { get; }

        public ReactiveProperty<bool> SearchBoxFocus { get; }

        public ReactiveProperty<bool> SearchButtonLoad { get; }

        public ReactiveCommand SearchBoxTextChanged { get; }

        public ReactiveCommand SearchButtonClicked { get; }

        public ReactiveCommand SearchBoxLostFocused { get; }
        
        public ReactiveProperty<int> TabbedIndex { get; }

        public ReactiveProperty<string> SuggestText { get; }

        public ReactiveProperty<GridLength> InvisibleTab { get; }

        public ReactiveProperty<bool> WatchHoldPivot_Visibility { get; }

        public ReactiveProperty<bool> DetailsTab_Visibility { get; }

        public UserAccountModel AvatarURL { get; set; }
        #endregion

        public HomePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            InvisibleTab = new ReactiveProperty<GridLength>(new GridLength(0, GridUnitType.Star));
            WatchHoldPivot_Visibility = new ReactiveProperty<bool>(true);
            DetailsTab_Visibility = new ReactiveProperty<bool>(false);

            SuggestText = new ReactiveProperty<string>();

            NavigationTitle = new ReactiveProperty<string>("Details");

            SearchButtonLoad = new ReactiveProperty<bool>();
            SearchBoxLoad = new ReactiveProperty<Visibility>(Visibility.Collapsed);
            SearchBoxLoad.Subscribe(x => SearchButtonLoad.Value = (SearchBoxLoad.Value == Visibility.Collapsed));
            SearchBoxFocus = new ReactiveProperty<bool>();
            SearchBoxTextChanged = new ReactiveCommand();
            SearchBoxTextChanged.Subscribe(_ =>
            {
                _eventAggregator.GetEvent<Keyword>().Publish(SuggestText.Value);
            });

            SearchButtonClicked = new ReactiveCommand();
            SearchButtonClicked.Subscribe(_ =>
            {
                SearchBoxLoad.Value = Visibility.Visible;
                _eventAggregator.GetEvent<NavigateSearchPage>().Publish(1);
                SearchBoxFocus.Value = true;
            });

            GoBackButton = new ReactiveCommand();
            GoBackButton.Subscribe(_ => {
                _navigationService.GoBack();
                if (WatchHoldPivot_Visibility.Value)
                {
                    IScheduler scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
                    scheduler.Schedule(() => _eventAggregator.GetEvent<LoadItemStatus>().Publish(TabbedIndex.Value));
                }
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
                    _navigationService.Navigate("Login", null);
                    _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(1);
                }
            });

            TabbedIndex = new ReactiveProperty<int>();
            TabbedIndex.Subscribe(x =>
            {
                // The method below requires a lot of time to process, so
                // running it under task factory under UI thread would be
                // a better choice, for now.
                Task.Factory.StartNew(()=> _eventAggregator.GetEvent<LoadItemStatus>().Publish(x),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext()
                    );
            });

            _eventAggregator.GetEvent<ChildFrameNavigating>().Subscribe(x =>
            {

                switch (x)
                {
                    case 1:
                        //login phase
                        InvisibleTab.Value = new GridLength(0, GridUnitType.Star);
                        break;
                    case 2:
                        //main phase
                        InvisibleTab.Value = new GridLength();
                        WatchHoldPivot_Visibility.Value = true;
                        DetailsTab_Visibility.Value = false;
                        SearchBoxLoad.Value = Visibility.Collapsed;
                        break;
                    case 3:
                        //details phase
                        InvisibleTab.Value = new GridLength();
                        WatchHoldPivot_Visibility.Value = false;
                        DetailsTab_Visibility.Value = true;
                        SearchBoxLoad.Value = Visibility.Collapsed;
                        NavigationTitle.Value = "Details";
                        break;
                    //TODO: add functionality when settings made.
                    case 4:
                        //search phase
                        InvisibleTab.Value = new GridLength();
                        WatchHoldPivot_Visibility.Value = false;
                        DetailsTab_Visibility.Value = true;
                        SearchBoxLoad.Value = Visibility.Visible;
                        NavigationTitle.Value = "Search";
                        break;

                }
            });
            _eventAggregator.GetEvent<HomePageAvatarLoad>().Subscribe(() =>
            {
                AvatarURL = Database.GetCurrentUserAccount() ?? null;
                RaisePropertyChanged(nameof(AvatarURL));
            });
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
    public class ChildFrameNavigating : PubSubEvent<int>{ }
    public class HomePageAvatarLoad : PubSubEvent { }
}
