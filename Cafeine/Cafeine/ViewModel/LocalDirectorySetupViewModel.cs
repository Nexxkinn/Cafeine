using Cafeine.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;

namespace Cafeine.ViewModel {
    public class localDirectorySetup : ViewModelBase {
        private int _itemID;
        public int ItemID {
            get {
                return _itemID;
            }

            set {
                Set(ref _itemID, value);
            }
        }

        private string _itemName = null;
        public string ItemName {
            get {
                return _itemName;
            }

            set {
                Set(ref _itemName, value);
            }
        }
        public string FolderName;
    }
    public class LocalDirectorySetupViewModel : ViewModelBase {
        INavigationService _navigationservice;
        public ObservableCollection<localDirectorySetup> FoldersVM = new ObservableCollection<localDirectorySetup>();
        private int FolderID; //C# sometimes suck at sharing a field between classes.

        public LocalDirectorySetupViewModel(INavigationService nav) {
            _navigationservice = nav;
        }
        
        private string _defaultDirectoryLabel = string.Empty;
        public string DefaultDirectoryLabel {
            get {
                return _defaultDirectoryLabel;
            }
            set {
                Set(ref _defaultDirectoryLabel, value);
            }
        }

        private Visibility _isBrowseButtonVisible = Visibility.Visible;
        public Visibility IsBrowseButtonVisible {
            get {
                return _isBrowseButtonVisible;
            }
            set {
                Set(ref _isBrowseButtonVisible, value);
            }
        }

        private Visibility _isStepTwoOK = Visibility.Collapsed;
        public Visibility IsStepTwoOK {
            get {
                return _isStepTwoOK;
            }
            set {
                Set(ref _isStepTwoOK, value);
            }
        }

        private Visibility _isLinkedItemSuggestionVisible = Visibility.Collapsed;
        public Visibility IsLinkedItemSuggestionVisible {
            get {
                return _isLinkedItemSuggestionVisible;
            }
            set {
                Set(ref _isLinkedItemSuggestionVisible, value);
            }
        }

        private IEnumerable<LocalDirectorySetupItems> _linkedItemSuggestionItemSource;
        public IEnumerable<LocalDirectorySetupItems> LinkedItemSuggestionItemSource {
            get {
                return _linkedItemSuggestionItemSource;
            }
            set {
                Set(ref _linkedItemSuggestionItemSource, value);
            }
        }

        private RelayCommand _browseClicked;
        public RelayCommand BrowseClicked {
            get {
                return _browseClicked
                    ?? (_browseClicked = new RelayCommand(
                    async () => {
                        var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                        folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                        folderPicker.FileTypeFilter.Add("*");

                        StorageFolder defaultfolder = await folderPicker.PickSingleFolderAsync();
                        if (defaultfolder != null) {
                            // Application now has read/write access to all contents in the picked folder
                            // (including other sub-folder contents)
                            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", defaultfolder);
                            DefaultDirectoryLabel = defaultfolder.Name;
                            await FeedFoldersToListView();
                            IsBrowseButtonVisible = Visibility.Collapsed;
                            IsStepTwoOK = Visibility.Visible;
                        }
                        else {
                            DefaultDirectoryLabel = "None";
                        }
                    }));
            }
        }
        private async Task FeedFoldersToListView() {
            StorageFolder Defaultfolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
            IReadOnlyList<StorageFolder> FL = await Defaultfolder.GetFoldersAsync();
            foreach (StorageFolder folder in FL) {
                FoldersVM.Add(new localDirectorySetup{
                    FolderName = folder.Name
                });
            }
        }

        private RelayCommand<LocalDirectorySetupItems> _AutoSuggestBoxQuerySubmited;
        public RelayCommand<LocalDirectorySetupItems> AutoSuggestBoxQuerySubmited {
            get {
                return _AutoSuggestBoxQuerySubmited
                    ?? (_AutoSuggestBoxQuerySubmited = new RelayCommand<LocalDirectorySetupItems>(
                    p => {
                        FoldersVM[FolderID].ItemID = p.ItemID;
                        FoldersVM[FolderID].ItemName = p.Title;
                        IsLinkedItemSuggestionVisible = Visibility.Collapsed;
                    }));
            }
        }

        private RelayCommand<string> _AutoSuggestBoxTextChanged;
        public RelayCommand<string> AutoSuggestBoxTextChanged {
            get {
                return _AutoSuggestBoxTextChanged
                    ?? (_AutoSuggestBoxTextChanged = new RelayCommand<string>(
                    async p => {
                        var itemlist = await FeedCollectionToIObservableList();
                        LinkedItemSuggestionItemSource = itemlist.Where(x => x.Title.ToLower().Contains(p)).Take(7);
                    }));
            }
        }
        private static async Task<List<LocalDirectorySetupItems>> FeedCollectionToIObservableList() {
            List<LocalDirectorySetupItems> Item = new List<LocalDirectorySetupItems>();
            var products = await Design.DataProvider.GrabOfflineCollection();
            foreach (var item in products) {
                Item.Add(new LocalDirectorySetupItems {
                    Category = item.Category.ToString(),
                    ItemID = item.Item_Id,
                    Title = item.Item_Title,
                });
            }
            return Item;
        }

        private RelayCommand _PrimaryButton;
        public RelayCommand PrimaryButton {
            get {
                return _PrimaryButton
                    ?? (_PrimaryButton = new RelayCommand(
                    async () => {
                        //save to JSON as LinkedFolder_service.json
                        string JsonItems = JsonConvert.SerializeObject(FoldersVM, Formatting.Indented);
                        var OfflineFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
                        var SaveFile = await OfflineFolder.CreateFileAsync("LinkedFolder_1.json", CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteTextAsync(SaveFile, JsonItems);
                        IsBrowseButtonVisible = Visibility.Visible;
                        IsLinkedItemSuggestionVisible = Visibility.Collapsed;
                        IsStepTwoOK = Visibility.Collapsed;
                    }));
            }
        }

        private RelayCommand _secondaryButton;
        public RelayCommand SecondaryButton {
            get {
                return _secondaryButton
                    ?? (_secondaryButton = new RelayCommand(
                    () => {
                        IsBrowseButtonVisible = Visibility.Visible;
                        IsStepTwoOK = Visibility.Collapsed;
                        IsLinkedItemSuggestionVisible = Visibility.Collapsed;
                        LinkedItemSuggestionItemSource = null;
                    }));
            }
        }

        private RelayCommand<localDirectorySetup> _linkedFolderItemClicked;
        public RelayCommand<localDirectorySetup> LinkedFolderItemClicked {
            get {
                return _linkedFolderItemClicked
                    ?? (_linkedFolderItemClicked = new RelayCommand<localDirectorySetup>(
                    p => {
                        FolderID = FoldersVM.IndexOf(p);
                        LinkedItemSuggestionItemSource = null;
                        IsLinkedItemSuggestionVisible = Visibility.Visible;
                    }));
            }
        }
    }
}
