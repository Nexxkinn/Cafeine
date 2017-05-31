
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Cafeine
{
    public partial class ItemTemplates
    {
        public ItemTemplates()
        {
            InitializeComponent();
        }
        private void ImageEntered(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((Grid)sender).Resources["ImageOnHover"] as Storyboard;
            sb.Begin();
        }

        private void ImageExited(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((Grid)sender).Resources["ImageOffHover"] as Storyboard;
            sb.Begin();
        }
    }
}