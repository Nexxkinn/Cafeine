using GalaSoft.MvvmLight;
using System;
using Cafeine.Model;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Command;

namespace Cafeine.ViewModel {
    public class ExpandItemDialogViewModel : ViewModelBase {
        private ItemModel _item = new ItemModel();
        public ItemModel Item {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public ImageSource source => new BitmapImage(new Uri(Item.Imgurl, UriKind.Absolute));
        public int selectedindex {
            get { return Item.My_score -1; }
            set { Set(ref Item.My_score, value+1); }
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

        public ExpandItemDialogViewModel() {
        }

        private RelayCommand _primaryButton;
        public RelayCommand PrimaryButton {
            get {
                return _primaryButton
                    ?? (_primaryButton = new RelayCommand(
                    async () => {
                        //Item.My_score = selectedindex + 1;
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
