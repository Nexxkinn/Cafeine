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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views.SettingsPages {
    public sealed partial class LocalDirectorySetup : ContentDialog {
        public LocalDirectorySetup() {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
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
                Animelist.Visibility = Visibility.Visible;
            }
            else {
                DefaultDirectory.Text = "None";
            }
            //for each item's directory tab
            //IReadOnlyList<StorageFile> w = await folder.GetFilesAsync();
            //foreach (StorageFile file in w)
            //{
            //    DefaultDirectory.Text = DefaultDirectory.Text + "\n" + file.Name;
            //}
        }
        private async Task FeedFoldersToListView() {
            StorageFolder Defaultfolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
            ObservableCollection<LocalDirectorySetupViewModel> FoldersVM = new ObservableCollection<LocalDirectorySetupViewModel>();

            IReadOnlyList<StorageFolder> FL = await Defaultfolder.GetFoldersAsync();
            foreach (StorageFolder folder in FL) {
                FoldersVM.Add(new LocalDirectorySetupViewModel(new LocalDirectorySetupLocalFoldersModel {
                    FolderName = folder.Name
                }));
            }
            Animelist.ItemsSource = FoldersVM;
        }
        private async Task<ObservableCollection<LocalDirectorySetupItems>> FeedCollectionToIObservableList() {
            ObservableCollection<LocalDirectorySetupItems> Item = new ObservableCollection<LocalDirectorySetupItems>();
            var OffFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
            string OffFile = Path.Combine(OffFolder.Path, "RAW_1_anime.xml");
            XDocument ParseData = XDocument.Load(OffFile);
            var anime = ParseData.Descendants("anime");
            ParseData.Remove();
            foreach (var item in anime) {
                Item.Add(new LocalDirectorySetupItems {
                    Category = "Anime",
                    ItemID = (int)item.Element("series_animedb_id"),
                    Title = item.Element("series_title").Value,
                });
            }
            anime.Remove();
            return Item;
        }
    }
}
