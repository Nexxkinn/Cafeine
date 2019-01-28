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
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private NavigationService _navigationService;

        public ViewModelLink _eventAggregator;

        private int? MainPageCurrentState;

        public ReactiveProperty<string> Keyword { get; }

        public ReactiveCommand<ItemLibraryModel> ItemClicked { get; }
        
        public ObservableCollection<ItemLibraryModel> OfflineResults;

        public ObservableCollection<ItemLibraryModel> OnlineResults;

        public CafeineProperty<bool> LoadResults = new CafeineProperty<bool>();

        public CafeineProperty<bool> OnlineResultsProgressRing = new CafeineProperty<bool>();

        public CafeineProperty<bool> OfflineResultsNoMatches = new CafeineProperty<bool>();

        public CafeineProperty<bool> OnlineResultsNoMatches = new CafeineProperty<bool>();
        
        public SearchPageViewModel()
        {
            _navigationService = new NavigationService();
            _eventAggregator = new ViewModelLink();
            
            OfflineResults = new ObservableCollection<ItemLibraryModel>();

            _eventAggregator.Subscribe<string>(x=> Keyword.Value = x, typeof(Keyword));

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
            OnlineResults = new ObservableCollection<ItemLibraryModel>();
            
            ItemClicked = new ReactiveCommand<ItemLibraryModel>();
            ItemClicked.Subscribe(NavigateToItemDetails);
        }
        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPageCurrentState = e.Parameter as int?;
            base.OnNavigatedTo(e);
        }

        private void NavigateToItemDetails(ItemLibraryModel item)
        {
            // Why would you need to use EventAggregator to pass the data?
            // Because the navigation parameter is so shitty that it only
            // accepts primitve types such as string, int, etc. Otherwise,
            // the app will crash whether you minimize it or something.
            //
            // Reference : http://archive.is/L1v1H
            // Backup    : http://runtime117.rssing.com/chan-13993968/all_p3.html
            if(MainPageCurrentState.Value == 1)
            {
                _navigationService.RemoveLastPage();
            }
            _navigationService.Navigate(typeof(ItemDetailsPage));
            _eventAggregator.Publish(item, typeof(ItemDetailsID));
        }

        private async Task GetResults(string keyword)
        {
            if (keyword != null && keyword != string.Empty)
            {
                //cleaning first & startup
                OfflineResultsNoMatches.Value = false;
                OnlineResults = new ObservableCollection<ItemLibraryModel>();
                OnlineResultsNoMatches.Value = false;
                OnlineResultsProgressRing.Value = true;
                RaisePropertyChanged(nameof(OnlineResults));

                LoadResults.Value = true;

                IList<ItemLibraryModel> offlineresultslist = Database.SearchItemCollection(keyword);
                OfflineResults = new ObservableCollection<ItemLibraryModel>(offlineresultslist);
                OfflineResultsNoMatches.Value = (offlineresultslist.Count == 0);
                RaisePropertyChanged(nameof(OfflineResults));

                IList<ItemLibraryModel> onlineresultlist = await Database.SearchOnline(keyword);
                var filteredOnlineResult = onlineresultlist.Except(offlineresultslist,new Itemcomparer()).ToList();
                OnlineResults = new ObservableCollection<ItemLibraryModel>(filteredOnlineResult);
                OnlineResultsNoMatches.Value = (onlineresultlist.Count == 0);
                OnlineResultsProgressRing.Value = false;

                RaisePropertyChanged(nameof(OnlineResults));
            }
        }

    }
    public class Itemcomparer : IEqualityComparer<ItemLibraryModel>
    {
        public bool Equals(ItemLibraryModel x, ItemLibraryModel y)
        {
            return x.MalID == y.MalID;
        }

        public int GetHashCode(ItemLibraryModel obj)
        {
            return obj.MalID.GetHashCode();
        }
    }
}
