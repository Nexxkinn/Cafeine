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
using System.Threading.Tasks;

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
    }
}