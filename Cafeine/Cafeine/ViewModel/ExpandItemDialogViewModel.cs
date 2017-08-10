using GalaSoft.MvvmLight;
using System;
using Cafeine.Model;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System.Collections;
using Windows.Storage;
using System.Collections.Generic;
using Windows.Storage.AccessCache;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Cafeine.ViewModel {
    public class ExpandItemDialogViewModel : ViewModelBase {
        private ItemModel _item = new ItemModel();
        public ItemModel Item {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public ImageSource source => new BitmapImage(new Uri(Item.Imgurl, UriKind.Absolute));
        public int selectedindex {
            get { return Item.My_score - 1; }
            set { Set(ref Item.My_score, value + 1); }
        }
        public int ItemStatus {
            get {
                if (Item.My_status == 6) return 4;
                else return Item.My_status - 1;
            }
            set {
                int ItemStatusShitAPI = value + 1;
                if (value == 5) ItemStatusShitAPI = 6;
                Set(ref Item.My_status, ItemStatusShitAPI);
            }
        }
        public string EpisodeChapterLabel {
            get {
                if (Item.Category == AnimeOrManga.anime) return "Episodes Watched";
                else return "Chapters read";
            }
        }
        public string EpisodesChapters {
            get { return Item.My_watch.ToString(); }
            set { Set(ref Item.My_watch, Convert.ToInt32(value)); }
        }

        private List<LocalDirectoryFile> _files = new List<LocalDirectoryFile>();
        public List<LocalDirectoryFile> Files {
            get {
                _files = Task.Run(async () => await LoadLocalFiles()).Result;
                return _files; }
        }
        public async Task<List<LocalDirectoryFile>> LoadLocalFiles() {
            try {
                //load list
                List<LocalDirectoryFile> file = new List<LocalDirectoryFile>();
                var OfflineFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline_data", CreationCollisionOption.OpenIfExists);
                StorageFile OpenJSONFile = await OfflineFolder.GetFileAsync("LinkedFolder_1.json");
                string ReadJSONFile = await FileIO.ReadTextAsync(OpenJSONFile);
                List<localDirectorySetup> products = JsonConvert.DeserializeObject<List<localDirectorySetup>>(ReadJSONFile);
                localDirectorySetup directory = products.Where(x => x.ItemID == Item.Item_Id).First();
                if (directory != null) {
                    //load files in folder
                    StorageFolder Defaultfolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
                    StorageFolder FL = await Defaultfolder.GetFolderAsync(directory.FolderName);
                    IReadOnlyList<StorageFile> File = await FL.GetFilesAsync();

                    //foreach
                    foreach (var item in File) {
                        file.Add(
                            new LocalDirectoryFile {
                                Title = item.Name,
                                Directory = item.Path
                            }
                            );
                    }
                }
                return await Task.FromResult(file);

            }
            catch (Exception e) {
                List<LocalDirectoryFile> file = new List<LocalDirectoryFile>();
                return await Task.FromResult(file);
            }
        }

        public ExpandItemDialogViewModel() {
        }

        private RelayCommand _primaryButton;
        public RelayCommand PrimaryButton {
            get {
                return _primaryButton
                    ?? (_primaryButton = new RelayCommand(
                    async () => {
                        await Design.CollectionLibraryProvider.UpdateItem(Item, Item.Category);
                    }));
            }
        }

        private RelayCommand _secondaryButton;
        public RelayCommand SecondaryButton {
            get {
                return _secondaryButton
                    ?? (_secondaryButton = new RelayCommand(
                    () => {
                        Item = null;
                    }));
            }
        }
    }
}
