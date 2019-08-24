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
        private IEnumerable<ServiceItem> ListedItems;

        private ObservableCollection<ServiceItem> _Library;

        public ObservableCollection<ServiceItem> Library {
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

        private OfflineItem RightClickedItem;

        public MainViewModel()
        {
            //setup

            SortBy = new ReactiveProperty<int>(0);
            SortBy.PropertyChanged += (_, e) =>
            {
                Library = new ObservableCollection<ServiceItem>(SortAndFilter(ListedItems));
            };

            FilterBy = new ReactiveProperty<int>(0);
            FilterBy.PropertyChanged += (_, e) =>
            {
                Library = new ObservableCollection<ServiceItem>(SortAndFilter(ListedItems));
            };

            TypeBy = new ReactiveProperty<int>(0);
            FilterBy.PropertyChanged += (_, e) =>
            {
                Library = new ObservableCollection<ServiceItem>(SortAndFilter(ListedItems));
            };

            TabbedIndex = new ReactiveProperty<int>();
            TabbedIndex.Subscribe(x => _ = DisplayItem(x) );

            Library = new ObservableCollection<ServiceItem>();

            Database.DatabaseUpdated += Database_DatabaseUpdated;
        }

        private void Database_DatabaseUpdated(object sender, EventArgs e)
        {
            ListedItems = Database.SearchBasedonUserStatus(TabbedIndex.Value);
            var RemovedList = Library.Except(ListedItems, new ServiceItemComparer()).ToList();
            foreach (var item in RemovedList) Library.Remove(item);
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                Library = new ObservableCollection<ServiceItem>(Library);
            }
            else
            {
                _ = DisplayItem(TabbedIndex.Value);
            }
            navigationService.ClearHistory();
            await base.OnNavigatedTo(e);
        }

        private async Task DisplayItem(int value)
        {
            await Task.Yield();
            ListedItems = Database.SearchBasedonUserStatus(value);
            Library = new ObservableCollection<ServiceItem>(SortAndFilter(ListedItems));
        }

        private IEnumerable<ServiceItem> SortAndFilter(IEnumerable<ServiceItem> items)
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
                return xx.ItemStatus == FilterBy.Value;
            });
            switch (SortBy.Value)
            {
                case 0:
                    return filtered.OrderBy(y =>
                    {
                        string title = y.Title;
                        if (title.Length < 5) return title;
                        else return title.Substring(0, 5);
                    });
                case 1:
                    return filtered.OrderByDescending(y =>
                   {
                       string title = y.Title;
                       if (title.Length < 5) return title;
                       else return title.Substring(0, 5);
                   });
                default:
                    return filtered;
            };
        }

        public void ItemClicked(object sender, ItemClickEventArgs e)
        {
            ServiceItem item = e.ClickedItem as ServiceItem;
            ItemLibraryService.Push(item);
            navigationService.Navigate(typeof(ItemDetailsPage));
        }

        public void RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            RightClickedItem = ((FrameworkElement)e.OriginalSource).DataContext as OfflineItem;
        }
    }
}
