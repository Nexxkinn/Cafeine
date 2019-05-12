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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.ViewModels
{
    public class MainViewModel : ViewModelBase
    { 
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
        
        public ReactiveProperty<int> SortBy { get; }

        public ReactiveProperty<int> TabbedIndex { get; }
        public ReactiveProperty<int> FilterBy { get; }

        public ReactiveProperty<int> TypeBy { get; }

        private int CurrentCategory;

        private ItemLibraryModel RightClickedItem;

        public MainViewModel()
        {
            //setup

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

            TabbedIndex = new ReactiveProperty<int>();
            TabbedIndex.Subscribe(async
                x =>
                {
                    await Task.Factory.StartNew(() => DisplayItem(x),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext());
                });

            Library = new ObservableCollection<ItemLibraryModel>();

            Database.DatabaseUpdated += Database_DatabaseUpdated;
        }

        private void Database_DatabaseUpdated(object sender, EventArgs e)
        {
            ListedItems = Database.SearchBasedonCategory(TabbedIndex.Value);
            var RemovedList = Library.Except(ListedItems, new ItemLibraryListComparer()).ToList();
            foreach (var item in RemovedList) Library.Remove(item);
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            navigationService.ClearHistory();
            if(e.NavigationMode == NavigationMode.Back)
            {
            }
            else
            {
                await DisplayItem(TabbedIndex.Value);
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

        public void ItemClicked(object sender, ItemClickEventArgs e)
        {
            ItemLibraryModel item = e.ClickedItem as ItemLibraryModel;

            // Why would you need to use EventAggregator to pass the data?
            // Because the navigation parameter is so shitty that it only
            // accepts primitve types such as string, int, etc. Otherwise,
            // the app will crash whether you minimize it or something.
            //
            // Reference : http://archive.is/L1v1H
            // Backup    : http://runtime117.rssing.com/chan-13993968/all_p3.html

            if (navigationService.CanGoBack())
            {
                navigationService.GoBack();
                navigationService.RemoveLastPage();
            }
            ItemLibraryService.Push(item);
            navigationService.Navigate(typeof(ItemDetailsPage));
        }

        public void RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            RightClickedItem = ((FrameworkElement)e.OriginalSource).DataContext as ItemLibraryModel;
        }
    }
}
