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
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    { 
        private NavigationService _navigationService;

        private ViewModelLink VMLink;

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
        
        public MainPageViewModel()
        {
            //setup
            _navigationService = new NavigationService();
            VMLink = new ViewModelLink();

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

            ItemClicked = new ReactiveCommand<ItemLibraryModel>();
            ItemClicked.Subscribe(NavigateToItemDetails);

            VMLink.Subscribe<int>(async (x) =>
                {
                    await Task.Factory.StartNew(() => DisplayItem(x),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext());
                }
                , typeof(LoadItemStatus));
            VMLink.Subscribe(x =>
                {
                    switch (x)
                    {
                        case 1:
                            // can't go back -> assume it's the main page
                            // can go back -> assume it's details page
                            int state = _navigationService.CanGoBack() ? 1 : 0;
                            _navigationService.Navigate(typeof(SearchPage), state);
                            return;
                        case 2:
                            _navigationService.GoBack();
                            return;
                    }
                }
                , typeof(NavigateSearchPage));
        }
        public override async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(async () => await DisplayItem(0),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
            await base.OnLoaded(sender, e);
        }
        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            await Task.Yield();
            _navigationService.ClearHistory();
            if(e.NavigationMode == NavigationMode.Back)
            {
                Library = new ObservableCollection<ItemLibraryModel>(Library);
            }
            await base.OnNavigatedTo(e);
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
            _navigationService.Navigate(typeof(ItemDetailsPage));
            VMLink.Publish(item, typeof(ItemDetailsID));
        }
    }
}
