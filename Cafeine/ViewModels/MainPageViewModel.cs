using Cafeine.Models;
using Cafeine.Models.Enums;
using Cafeine.Services;
using Prism.Events;
using Prism.Unity.Windows;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Windows.Storage;

namespace Cafeine.ViewModels
{
    public class LoadItemStatus : PubSubEvent<int>
    {
    }

    public class NavigateItem : PubSubEvent<ItemLibraryModel>
    {
    }

    public class NavigateSearchPage : PubSubEvent<int> { }

    public class MainPageViewModel : ViewModelBase, INavigationAware
    { 
        private INavigationService _navigationService;

        private readonly IEventAggregator _eventAggregator;

        private IEnumerable<ItemLibraryModel> ListedItems;

        private ObservableCollection<ItemLibraryModel> _Library;

        public ObservableCollection<ItemLibraryModel> Library {
            get {
                return _Library;
            }
            set {
                if (_Library != value) // or the appropriate comparison for your specific case
                {
                    _Library = value;
                    RaisePropertyChanged("Library");
                }
            }
        }

        public ReactiveCommand<ItemLibraryModel> ItemClicked { get; }

        public ReactiveCommand MainPageLoaded { get; }
        
        public ReactiveProperty<int> SortBy { get;}

        public ReactiveProperty<int> FilterBy { get;}

        public ReactiveProperty<int> TypeBy { get; set; }
        
        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            //setup
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            SortBy = new ReactiveProperty<int>(0);
            SortBy.PropertyChanged += (_, e) =>
            {
                Library = new ObservableCollection<ItemLibraryModel>(SortAndFilter(ListedItems));
            };

            FilterBy = new ReactiveProperty<int>(0);
            FilterBy.PropertyChanged += (_, e) =>
            {
                Library = new ObservableCollection<ItemLibraryModel>(SortAndFilter(ListedItems));
            };

            TypeBy = new ReactiveProperty<int>(0);
            FilterBy.PropertyChanged += (_, e) =>
            {
                Library = new ObservableCollection<ItemLibraryModel>(SortAndFilter(ListedItems));
            };

            MainPageLoaded = new ReactiveCommand();
            MainPageLoaded.Subscribe(async () =>
               {
                   await DisplayItem(0);
               });

            ItemClicked = new ReactiveCommand<ItemLibraryModel>();
            ItemClicked.Subscribe(item =>
            {
                NavigateToItemDetails(item);
            });

            _eventAggregator.GetEvent<LoadItemStatus>().Subscribe(async(x) => await DisplayItem(x));
            _eventAggregator.GetEvent<NavigateItem>().Subscribe(NavigateToItemDetails);
            _eventAggregator.GetEvent<NavigateSearchPage>().Subscribe(x =>
            {
                switch (x)
                {
                    case 1:
                        _navigationService.Navigate("Search", null);
                        return;
                    case 2:
                        _navigationService.GoBack();
                        return;
                }
            });
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(2);
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e, viewModelState);
        }

        private async Task DisplayItem(int value)
        {
            await Task.Factory.StartNew(() =>
            {
                ListedItems = Database.SearchBasedonCategory(value);
                Library = new ObservableCollection<ItemLibraryModel>(SortAndFilter(ListedItems));
            },
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false);
        }

        private IEnumerable<ItemLibraryModel> SortAndFilter(IEnumerable<ItemLibraryModel> items)
        {
            // "filtering" type first.
            var filtered = items.Where(x =>
            {
                //just skip if TypeBy is "All"
                if (TypeBy.Value == 3) return true;
                return (int)x.MediaType == TypeBy.Value;
            }).Where(xx =>
            {
                //Just skip if FilterBy is "All"
                if (FilterBy.Value == 0) return true;
                return (int)xx.Service["default"].Status == FilterBy.Value;
            });
            switch (SortBy.Value)
            {
                case 0:
                    return filtered.OrderBy(y =>
                    {
                        string title = y.Service["default"].Title;
                        if (title.Length < 5) return title;
                        else return title.Substring(0, 5);
                    }).ToList();
                case 1:
                    return filtered.OrderByDescending(y =>
                   {
                       string title = y.Service["default"].Title;
                       if (title.Length < 5) return title;
                       else return title.Substring(0, 5);
                   }).ToList();
                default:
                    return filtered;
            };
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

            if (_navigationService.CanGoBack())
            {
                _navigationService.GoBack();
                _navigationService.RemoveLastPage();
            }
            _navigationService.Navigate("ItemDetails", null);
            _eventAggregator.GetEvent<ItemDetailsID>().Publish(item);
        }
    }
}
