using Cafeine.Design;
using Cafeine.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cafeine.ViewModel {
    public class CollectionLibrary : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private ItemModel itemproperty;
        public ItemModel Itemproperty {
            get => itemproperty;

        }
        public string Image {
            get => itemproperty.Imgurl;
        }
        public string My_score {
            get { return Itemproperty.My_score.ToString(); }
            set {
                Itemproperty.My_score = Convert.ToInt32(value);
                this.OnPropertyChanged();
            }
        }
        public string My_watch {
            get { return Itemproperty.My_watch.ToString(); }
            set {
                Itemproperty.My_watch = Convert.ToInt32(value);
                this.OnPropertyChanged();
            }
        }
        public CollectionLibrary(ItemModel itemProperties) {
            itemproperty = itemProperties;
        }

        public CollectionLibrary() {
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class CollectionLibraryViewModel : ViewModelBase {
        private ICafeineNavigationService _navigationservice;

        private ObservableCollection<CollectionLibrary> ItemList = new ObservableCollection<CollectionLibrary>();
        //public ObservableCollection<CollectionLibrary> ItemList {
        //    get { return _itemList; }
        //    set { Set(ref _itemList, value); }
        //}

        private VirtualDirectory _directory = new VirtualDirectory();
        public VirtualDirectory Directory {
            get {
                return _directory;
            }
            set {
                Set(ref _directory, value);
            }
        }

        private IEnumerable<CollectionLibrary> _itemItemSource;
        public IEnumerable<CollectionLibrary> ItemItemSource {
            get {
                return _itemItemSource;
            }
            set {
                Set(ref _itemItemSource, value);
            }
        }

        public CollectionLibraryViewModel(ICafeineNavigationService navigation) {
            _navigationservice = navigation;
        }
        public async void GrabUserItemList() {

            try {
                ItemList = await CollectionLibraryProvider.QueryUserAnimeMangaListAsync(Directory.AnimeOrManga);
                ItemItemSource = ItemList.Where(x => x.Itemproperty.My_status == Directory.DirectoryType - 3);
            }
            catch (Exception e) {
                //TODO: show error message?? e.Message
            }
        }

        private RelayCommand<CollectionLibrary> _expandItem;
        public RelayCommand<CollectionLibrary> ExpandItem {
            get {
                return _expandItem
                    ?? (_expandItem = new RelayCommand<CollectionLibrary>(
                    async p => {
                        ItemModel x = await lo(p);
                        if (x != null) {
                            p.My_score = x.My_score.ToString();
                            p.My_watch = x.My_watch.ToString();
                        }
                    }));
            }
        }
        private async Task<ItemModel> lo(CollectionLibrary e) {
            ExpandItemDetails ExpandItemDialog = new ExpandItemDetails();
            ItemModel item;
            ExpandItemDialog.Item = e.Itemproperty;
            ExpandItemDialog.category = Directory.AnimeOrManga;
            await ExpandItemDialog.ShowAsync();
            item = ExpandItemDialog.Item;
            return item;
        }


    }
}