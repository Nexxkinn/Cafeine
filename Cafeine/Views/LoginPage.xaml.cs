using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : BasePage
    {
        public LoginViewModel Vm => DataContext as LoginViewModel;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void Ellipse_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse item = sender as Ellipse;
            await ImageCache.CreateImageCacheFolder();
            var cache = await ImageCache.GetFromCacheAsync(Vm.CurrentUserAccount.Avatar.Large);

            item.Fill = new ImageBrush()
            {
                ImageSource = new BitmapImage()
                {
                    DecodePixelHeight = 108,
                    DecodePixelWidth = 108,
                    UriSource = new Uri(cache.Path)
                }
            };
        }
    }
}
