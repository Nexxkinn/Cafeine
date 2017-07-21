using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Cafeine.ViewModels;
using Cafeine.Models;
using System.Xml.Linq;
using Windows.Storage.AccessCache;
using Newtonsoft.Json;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views.SettingsPages {
    public sealed partial class LocalDirectorySetup : ContentDialog {
        ObservableCollection<LocalDirectorySetupViewModel> FoldersVM = new ObservableCollection<LocalDirectorySetupViewModel>();
        int FolderID; //C# sometimes suck at sharing a field between classes.

        public LocalDirectorySetup() {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            //save to JSON as LinkedFolder_service.json
            string JsonItems = JsonConvert.SerializeObject(FoldersVM, Formatting.Indented);
            var OfflineFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            var SaveFile = await OfflineFolder.CreateFileAsync("LinkedFolder_1.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(SaveFile, JsonItems);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            //Clear any stored memory in this page
        }

        private async void BrowseDefaultFolder_Click(object sender, RoutedEventArgs e) {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder defaultfolder = await folderPicker.PickSingleFolderAsync();
            if (defaultfolder != null) {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", defaultfolder);
                DefaultDirectory.Text = defaultfolder.Name;
                await FeedFoldersToListView();
                LinkedFolder.Visibility = Visibility.Visible;
                FolderLabel.Visibility = Visibility.Visible;
                BrowseDefaultFolder.Visibility = Visibility.Collapsed;
            }
            else {
                DefaultDirectory.Text = "None";
            }
        }

        private async Task FeedFoldersToListView() {
            StorageFolder Defaultfolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
            IReadOnlyList<StorageFolder> FL = await Defaultfolder.GetFoldersAsync();
            foreach (StorageFolder folder in FL) {
                FoldersVM.Add(new LocalDirectorySetupViewModel(new LocalDirectorySetupLocalFoldersModel {
                    FolderName = folder.Name
                }));
            }
            LinkedFolder.ItemsSource = FoldersVM;
        }

        private static async Task<List<LocalDirectorySetupItems>> FeedCollectionToIObservableList() {
            List<LocalDirectorySetupItems> Item = new List<LocalDirectorySetupItems>();
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            StorageFile OpenJSONFile = await OffFolder.GetFileAsync("RAW_1.json");
            string ReadJSONFile = await FileIO.ReadTextAsync(OpenJSONFile);
            List<ItemProperties> products = JsonConvert.DeserializeObject<List<ItemProperties>>(ReadJSONFile);
            foreach (var item in products) {
                Item.Add(new LocalDirectorySetupItems {
                    Category = item.Category.ToString(),
                    ItemID = item.Item_Id,
                    Title = item.Item_Title,
                });
            }
            return Item;
        }

        private async void ItemAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                var itemlist = await FeedCollectionToIObservableList();
                ItemAutoSuggest.ItemsSource = itemlist.Where(x => x.Title.ToLower().Contains(sender.Text)).Take(10);
            }
        }

        private void ItemAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            var item = (LocalDirectorySetupItems)args.ChosenSuggestion;
            FoldersVM[FolderID].LinkedItemID = item.ItemID;
            FoldersVM[FolderID].LinkedItemName = item.Title;
            ItemAutoSuggest.Visibility = Visibility.Collapsed;
        }

        private void LinkedFolder_ItemClick(object sender, ItemClickEventArgs e) {
            var item = (LocalDirectorySetupViewModel)e.ClickedItem;
            FolderID = FoldersVM.IndexOf(item);
            ItemAutoSuggest.Text = "";
            ItemAutoSuggest.Visibility = Visibility.Visible;
        }
    }
}
