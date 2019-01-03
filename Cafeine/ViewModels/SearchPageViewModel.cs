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

        public ObservableCollection<ItemLibraryModel> OfflineResults;

        public ObservableCollection<ItemLibraryModel> OnlineResults;

        public SearchPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            OfflineResults = new ObservableCollection<ItemLibraryModel>();

            _eventAggregator.GetEvent<Keyword>().Subscribe(x=> Keyword.Value = x);

            Keyword = new ReactiveProperty<string>();
            this.Keyword.Throttle(TimeSpan.FromSeconds(0.3)).ObserveOnDispatcher().Subscribe(async x => await LoadResults(x));

            OnlineResults = new ObservableCollection<ItemLibraryModel>();
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            _eventAggregator.GetEvent<Keyword>().Unsubscribe(async x => await LoadResults(x));
            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        private async Task LoadResults(string keyword)
        {
            if (keyword != null && keyword != string.Empty)
            {
                var finditem = Database.SearchItemCollection(keyword);
                OfflineResults = new ObservableCollection<ItemLibraryModel>(finditem);
                RaisePropertyChanged("OfflineResults");

                IList<ItemLibraryModel> onlineresult = await Database.SearchOnline(keyword);
                var filteredOnlineResult = onlineresult.Except(finditem,new Itemcomparer()).ToList();
                OnlineResults = new ObservableCollection<ItemLibraryModel>(filteredOnlineResult);
                RaisePropertyChanged("OnlineResults");
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
