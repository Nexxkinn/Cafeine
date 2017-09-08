using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Cafeine;
using Cafeine.Model;
using System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;

namespace Cafeine {
    public partial class Generics {

        public Generics() {
            InitializeComponent();

        }
        private void ImageEntered(object sender, PointerRoutedEventArgs e) {
            Storyboard sb = ((Grid)sender).Resources["ImageOnHover"] as Storyboard;
            sb.Begin();
        }

        private void ImageExited(object sender, PointerRoutedEventArgs e) {
            Storyboard sb = ((Grid)sender).Resources["ImageOffHover"] as Storyboard;
            sb.Begin();
        }

        private void theimage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            ((Image)sender).Source = null;
        }
        private async void theimage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            StorageFolder CacheFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions
                .FutureAccessList
                .GetFolderAsync("ImageCacheFolder");

            var source = ((Image)sender).Source as BitmapImage;
            var filename = source.UriSource.Segments[4];

            try {
                var File = await CacheFolder.GetFileAsync(filename);
                using (IRandomAccessStream fileStream = await File.OpenAsync(FileAccessMode.Read)) {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = Convert.ToInt32(((Image)sender).MaxWidth);
                    await bitmapImage.SetSourceAsync(fileStream);
                    ((Image)sender).Source = bitmapImage;
                }
            }
            catch(FileNotFoundException) {
                return;
            }
        }
    }
}
