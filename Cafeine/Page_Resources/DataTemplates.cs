using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Cafeine
{
    public partial class DataTemplates
    {
        public DataTemplates()
        {
            InitializeComponent();
        }
        private void Enter(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((Image)sender).Resources["ImageOnHover"] as Storyboard;
            sb.Begin();
        }
    }
}