using Cafeine.Models;
using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
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

namespace Cafeine.ViewModels
{
    public class SearchPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;

        public IEventAggregator _eventAggregator;

        private bool lr, onrnm, offrnm, onrpr;

        private int? MainPageCurrentState;

        public ReactiveProperty<string> Keyword { get; }

        public ReactiveCommand<ItemLibraryModel> ItemClicked { get; }
        
        public ObservableCollection<ItemLibraryModel> OfflineResults;

        public ObservableCollection<ItemLibraryModel> OnlineResults;

        public bool LoadResults {
            get => lr;
            set {
                if (lr != value)
                {
                    lr = value;
                    RaisePropertyChanged(nameof(LoadResults));
                }
            }
        }

        public bool OnlineResultsProgressRing {
            get => onrpr;
            set {
                if (onrpr != value)
                {
                    onrpr = value;
                    RaisePropertyChanged(nameof(OnlineResultsProgressRing));
                }
            }
        }

        public bool OfflineResultsNoMatches {
            get => offrnm;
            set {
                if (offrnm != value)
                {
                    offrnm = value;
                    RaisePropertyChanged(nameof(OfflineResultsNoMatches));
                }
            }
        }

        public bool OnlineResultsNoMatches {
            get => onrnm;
            set {
                if (onrnm != value)
                {
                    onrnm = value;
                    RaisePropertyChanged(nameof(OnlineResultsNoMatches));
                }
            }
        }
        
        public SearchPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            
            OfflineResults = new ObservableCollection<ItemLibraryModel>();

            _eventAggregator.GetEvent<Keyword>().Subscribe(x=> Keyword.Value = x);

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
            this.Keyword.Throttle(TimeSpan.FromSeconds(0.3)).ObserveOnDispatcher().Subscribe(async x => await GetResults(x));

            OnlineResults = new ObservableCollection<ItemLibraryModel>();
            
            ItemClicked = new ReactiveCommand<ItemLibraryModel>();
            ItemClicked.Subscribe(NavigateToItemDetails);
        }
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(4);
            MainPageCurrentState = e.Parameter as int?;
            base.OnNavigatedTo(e, viewModelState);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            _eventAggregator.GetEvent<Keyword>().Unsubscribe(async x => await GetResults(x));
            _eventAggregator.GetEvent<NavigateSearchPage>().Publish(2);
            base.OnNavigatingFrom(e, viewModelState, suspending);
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
            FrameNavigationService test = _navigationService as FrameNavigationService;
            if(MainPageCurrentState.Value == 1)
            {
                _navigationService.RemoveLastPage();
                _navigationService.GoBack();
            }
            _navigationService.Navigate("ItemDetails", null);
            _eventAggregator.GetEvent<ItemDetailsID>().Publish(item);
        }

        private async Task GetResults(string keyword)
        {
            if (keyword != null && keyword != string.Empty)
            {
                //cleaning first & startup
                OfflineResultsNoMatches = false;
                OnlineResults = new ObservableCollection<ItemLibraryModel>();
                OnlineResultsNoMatches = false;
                OnlineResultsProgressRing = true;
                RaisePropertyChanged(nameof(OnlineResults));

                LoadResults = true;

                IList<ItemLibraryModel> offlineresultslist = Database.SearchItemCollection(keyword);
                OfflineResults = new ObservableCollection<ItemLibraryModel>(offlineresultslist);
                OfflineResultsNoMatches = (offlineresultslist.Count == 0);
                RaisePropertyChanged(nameof(OfflineResults));

                IList<ItemLibraryModel> onlineresultlist = await Database.SearchOnline(keyword);
                var filteredOnlineResult = onlineresultlist.Except(offlineresultslist,new Itemcomparer()).ToList();
                OnlineResults = new ObservableCollection<ItemLibraryModel>(filteredOnlineResult);
                OnlineResultsNoMatches = (onlineresultlist.Count == 0);
                OnlineResultsProgressRing = false;

                RaisePropertyChanged(nameof(OnlineResults));
            }
        }

    }

    public class Keyword : PubSubEvent<string> { }
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
