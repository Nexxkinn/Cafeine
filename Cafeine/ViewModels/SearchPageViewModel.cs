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

namespace Cafeine.ViewModels
{
    public class Keyword : PubSubEvent<string> { }
    public class SearchPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;

        public IEventAggregator _eventAggregator;

        public ReactiveProperty<string> Keyword { get; }

        public bool LoadResults = false;

        public bool OnlineResultsProgressRing;

        public bool OfflineResultsNoMatches;

        public bool OnlineResultsNoMatches;

        public ObservableCollection<ItemLibraryModel> OfflineResults;

        public ObservableCollection<ItemLibraryModel> OnlineResults;

        public SearchPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            OfflineResults = new ObservableCollection<ItemLibraryModel>();

            _eventAggregator.GetEvent<Keyword>().Subscribe(x=> Keyword.Value = x);

            Keyword = new ReactiveProperty<string>();
            this.Keyword.Throttle(TimeSpan.FromSeconds(0.3)).ObserveOnDispatcher().Subscribe(async x => await GetResults(x));

            OnlineResults = new ObservableCollection<ItemLibraryModel>();
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            _eventAggregator.GetEvent<Keyword>().Unsubscribe(async x => await GetResults(x));
            base.OnNavigatingFrom(e, viewModelState, suspending);
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
                RaisePropertyChanged(nameof(OfflineResultsNoMatches));
                RaisePropertyChanged(nameof(OnlineResults));
                RaisePropertyChanged(nameof(OnlineResultsNoMatches));
                RaisePropertyChanged(nameof(OnlineResultsProgressRing));

                LoadResults = true;
                RaisePropertyChanged(nameof(LoadResults));

                IList<ItemLibraryModel> offlineresultslist = Database.SearchItemCollection(keyword);
                OfflineResults = new ObservableCollection<ItemLibraryModel>(offlineresultslist);
                OfflineResultsNoMatches = (offlineresultslist.Count == 0);

                RaisePropertyChanged(nameof(OfflineResults));
                RaisePropertyChanged(nameof(OfflineResultsNoMatches));

                IList<ItemLibraryModel> onlineresultlist = await Database.SearchOnline(keyword);
                var filteredOnlineResult = onlineresultlist.Except(offlineresultslist,new Itemcomparer()).ToList();
                OnlineResults = new ObservableCollection<ItemLibraryModel>(filteredOnlineResult);
                OnlineResultsNoMatches = (onlineresultlist.Count == 0);
                OnlineResultsProgressRing = false;

                RaisePropertyChanged(nameof(OnlineResults));
                RaisePropertyChanged(nameof(OnlineResultsNoMatches));
                RaisePropertyChanged(nameof(OnlineResultsProgressRing));
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
