using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LiteDB;
using System.IO;
using Windows.Storage;
using Cafeine.Models;
using Cafeine.Services;
using Reactive.Bindings;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Prism.Events;

namespace Cafeine.ViewModels
{
    public class LoadItemStatus    : PubSubEvent<int> { }
    public class NavigateItem      : PubSubEvent<ItemLibraryModel> { }
    public class MainPageViewModel : ViewModelBase, INavigationAware
    {
        private static readonly string DB_FILE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "BeaconData.db");
        private INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        public ReactiveCollection<ItemLibraryModel> Library { get; set; }
        public ReactiveCommand<ItemLibraryModel>    ItemClicked { get; }
        public ReactiveCommand                      ForceDisplayItem { get; }

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            //setup
            _navigationService = navigationService;
            _eventAggregator   = eventAggregator;

            Library = new ReactiveCollection<ItemLibraryModel>();

            ForceDisplayItem = new ReactiveCommand()
                .WithSubscribe(async () =>
               {
                    _eventAggregator.GetEvent<LoadItemStatus>().Subscribe(async x => await DisplayItem(x));
                   await DisplayItem(0);
               });

            ItemClicked = new ReactiveCommand<ItemLibraryModel>();
            ItemClicked.Subscribe(item =>
            {
                Navigate(item);
            });
        }
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<ChildFrameNavigating>().Publish(2);
            _eventAggregator.GetEvent<NavigateItem>().Subscribe(Navigate);
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e, viewModelState);
        }
        private async Task DisplayItem(int value)
        {
            await ImageCache.CreateImageCacheFolder();
            var localitems = Database.SearchBasedonCategory(value);
            Library.Clear();
            foreach(var item in localitems) Library?.Add(item);
        }
        private void Navigate(ItemLibraryModel item)
        {
            #region rant
            // Why would you need to use EventAggregator to pass the data?
            // Because the navigation parameter is so shitty that it only
            // accepts primitve types such as string, int, etc. Otherwise,
            // the app will crash whether you minimize it or something.
            //
            // Reference : http://archive.is/L1v1H
            // Backup    : http://runtime117.rssing.com/chan-13993968/all_p3.html

            #endregion
            _navigationService.Navigate("ItemDetails", null);
            _eventAggregator.GetEvent<ItemDetailsID>().Publish(item.Id);
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
