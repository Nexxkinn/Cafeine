using Cafeine.Design;
using Cafeine.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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
        public string TotalEpisodesChapters {
            get { return (itemproperty.Item_Totalepisodes != 0) ? itemproperty.Item_Totalepisodes.ToString() : "-"; }
        }
        public CollectionLibrary(ItemModel itemProperties) {
            itemproperty = itemProperties;
        }
        public Visibility MangaItemVisibility {
            get { return (itemproperty.Category == AnimeOrManga.manga) ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility AnimeItemVisibility {
            get { return (itemproperty.Category == AnimeOrManga.anime) ? Visibility.Visible : Visibility.Collapsed; }
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

        private VirtualDirectory _directory = new VirtualDirectory();
        public VirtualDirectory Directory {
            get {
                return _directory;
            }
            set {
                Set(ref _directory, value);
            }
        }
        private Visibility _errorVisibility = Visibility.Collapsed;
        public Visibility ErrorVisibility {
            get {
                return _errorVisibility;
            }
            set {
                Set(ref _errorVisibility, value);
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
            Messenger.Default.Register<NotificationMessageAction<CollectionLibrary>>(this, HandleNotificationMessage);
        }
        private void HandleNotificationMessage(NotificationMessageAction<CollectionLibrary> message) {
            //check if it has a same id from sender
            var x = (ItemModel)message.Sender;
            CollectionLibrary checkitem;
            try {
                checkitem = ItemItemSource.Where(i => i.Itemproperty.Item_Id == x.Item_Id).First();
                message.Execute(checkitem);
            }
            catch (Exception) {
                checkitem = null;
            }
        }

        public async void GrabUserItemList() {

            try {
                ItemList = await CollectionLibraryProvider.QueryUserAnimeMangaListAsync(Directory.AnimeOrManga);
                ItemItemSource = ItemList.Where(x => x.Itemproperty.My_status == Directory.DirectoryType - 3);
                int count = ItemItemSource.Count();
                if (count == 0) ErrorVisibility = Visibility.Visible;
                ItemList = null;
            }
            catch (Exception e) {
                
            }
        }
        private RelayCommand<CollectionLibrary> _expandItem;
        public RelayCommand<CollectionLibrary> ExpandItem {
            get {
                return _expandItem
                    ?? (_expandItem = new RelayCommand<CollectionLibrary>(
                    async p => {
                        await ExpandItemDialogService.ItemCollectionExpand(p);
                    }));
            }
        }
    }
}