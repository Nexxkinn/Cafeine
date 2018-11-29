﻿using Cafeine.Models;
using Cafeine.Services;
using LiteDB;
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
using System.Reactive.Linq;
using System.Threading.Tasks;
using Unity;
using Windows.Storage;

namespace Cafeine.ViewModels
{
    public class LoadItemStatus : PubSubEvent<int>
    {
    }

    public class NavigateItem : PubSubEvent<UserItem>
    {
    }

    public class MainPageViewModel : ViewModelBase, INavigationAware
    {
        private static readonly string DB_FILE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "BeaconData.db");

        private INavigationService _navigationService;

        private readonly IEventAggregator _eventAggregator;
        
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

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            //setup
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            MainPageLoaded = new ReactiveCommand();
            MainPageLoaded.Subscribe(async () =>
               {
                   await ImageCache.CreateImageCacheFolder();
                   DisplayItem(0);
               });

            ItemClicked = new ReactiveCommand<ItemLibraryModel>();
            ItemClicked.Subscribe(item =>
            {
                Navigate(item.Service["default"]);
            });

            _eventAggregator.GetEvent<LoadItemStatus>().Subscribe(DisplayItem);
            _eventAggregator.GetEvent<NavigateItem>().Subscribe(Navigate);
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(2);
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e, viewModelState);
        }

        private void DisplayItem(int value)
        {
            var localitems = Database.SearchBasedonCategory(value);
            Library = new ObservableCollection<ItemLibraryModel>(localitems);
        }

        private void Navigate(UserItem item)
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

        //Todo : Remove this
        private void updatedata()
        {
            using (var db = new LiteDatabase(DB_FILE))
            {
                var col = db.GetCollection<ItemLibraryModel>("items");
                //col.Insert(new Item { Id = 1, Name = Name.Value });
                //var getdata = col.FindOne(x => x.Name.StartsWith("Jo"));
                //getdata.Name = Name.Value;
                //col.Update(getdata);
                //NameTextblock.Value = getdata.Name;

            }
        }
    }
}
