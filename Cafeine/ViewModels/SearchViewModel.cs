using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.Views;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private int? MainPageCurrentState;

        public ReactiveProperty<string> Keyword { get; }
                
        public ObservableCollection<ServiceItem> OfflineResults;

        public ObservableCollection<ServiceItem> OnlineResults;

        public CafeineProperty<bool> LoadResults = new CafeineProperty<bool>();

        public CafeineProperty<bool> OnlineResultsProgressRing = new CafeineProperty<bool>();

        public CafeineProperty<bool> OfflineResultsNoMatches = new CafeineProperty<bool>();

        public CafeineProperty<bool> OnlineResultsNoMatches = new CafeineProperty<bool>();

        public SearchViewModel()
        {            
            OfflineResults = new ObservableCollection<ServiceItem>();

            Link.Subscribe<string>(x=> Keyword.Value = x, typeof(Keyword));

            // RX.NET RANT:
            // Not implementing .ObserveOnDispatcher() causes
            //
            // System.Runtime.InteropServices.COMException with RPC_E_WRONG_THREAD tag
            //
            // On Throttle() method but no guideline or documentation mentioned it.
            // Even worse, only one source EVER give you a proper example of it.
            // http://rxwiki.wikidot.com/101samples --> Throttle - Simple.
            // Good job RX.Net and all of their team 👏.
            //SuggestItemSource?.Clear();
            Keyword = new ReactiveProperty<string>();
            Keyword.Throttle(TimeSpan.FromSeconds(0.3)).ObserveOnDispatcher().Subscribe(async x => await GetResults(x));
            Keyword.Subscribe(x =>
            {
                if (x == string.Empty)
                {
                    LoadResults.Value = false;
                }
            });
            OnlineResults = new ObservableCollection<ServiceItem>();
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            Link.Publish(Visibility.Visible, typeof(SearchBoxVisibility));
            MainPageCurrentState = e.Parameter as int?;
            await base.OnNavigatedTo(e);
        }

        public override Task OnNavigatedFrom(NavigationEventArgs e = null)
        {
            Link.Publish(Visibility.Collapsed,typeof(SearchBoxVisibility));
            return base.OnNavigatedFrom(e);
        }

        public void ItemClicked(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as ServiceItem;
            NavigateToItemDetails(clicked);
        }

        private void NavigateToItemDetails(ServiceItem item)
        {
            // Why would you need to use EventAggregator to pass the data?
            // Because the navigation parameter is so shitty that it only
            // accepts primitve types such as string, int, etc. Otherwise,
            // the app will crash whether you minimize it or something.
            //
            // Reference : http://archive.is/L1v1H
            // Backup    : http://runtime117.rssing.com/chan-13993968/all_p3.html
            //if(MainPageCurrentState.Value == 1)
            //{
            //    navigationService.RemoveLastPage();
            //}
            ItemLibraryService.Push(item);
            navigationService.Navigate(typeof(ItemDetailsPage));
        }

        private async Task GetResults(string keyword)
        {
            if (keyword != null && keyword != string.Empty)
            {
                //cleaning first & startup
                OfflineResultsNoMatches.Value = false;
                OnlineResults = new ObservableCollection<ServiceItem>();
                OnlineResultsNoMatches.Value = false;
                OnlineResultsProgressRing.Value = true;
                RaisePropertyChanged(nameof(OnlineResults));

                LoadResults.Value = true;

                IList<ServiceItem> offlineresultslist = Database.SearchOffline(keyword);
                OfflineResults = new ObservableCollection<ServiceItem>(offlineresultslist);
                OfflineResultsNoMatches.Value = (offlineresultslist.Count == 0);
                RaisePropertyChanged(nameof(OfflineResults));

                IList<ServiceItem> onlineresultlist = await Database.SearchOnline(keyword);
                var filteredOnlineResult = onlineresultlist.Except(offlineresultslist,new Itemcomparer()).ToList();
                OnlineResults = new ObservableCollection<ServiceItem>(filteredOnlineResult);
                OnlineResultsNoMatches.Value = (onlineresultlist.Count == 0);
                OnlineResultsProgressRing.Value = false;

                RaisePropertyChanged(nameof(OnlineResults));
            }
        }

    }
    public class Itemcomparer : IEqualityComparer<ServiceItem>
    {
        public bool Equals(ServiceItem x, ServiceItem y)
        {
            return x.ServiceID == y.ServiceID;
        }

        public int GetHashCode(ServiceItem obj)
        {
            return obj.ServiceID.GetHashCode();
        }
    }
}
