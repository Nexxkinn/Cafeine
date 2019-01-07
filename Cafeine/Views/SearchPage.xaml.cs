using Cafeine.Models;
using Cafeine.Services;
using Cafeine.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPageViewModel Vm {
            get {
                return DataContext as SearchPageViewModel;
            }
        }
        public SearchPage()
        {
            this.InitializeComponent();
        }

        private void GridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.RegisterUpdateCallback(LoadImage);
        }

        private async void LoadImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var imageurl = (args.Item as ItemLibraryModel).Service["default"].CoverImageUri;
            var cache = await ImageCache.GetFromCacheAsync(imageurl);
            var image = templateRoot.Children[0] as Image;
            image.Source = new BitmapImage()
            {
                UriSource = new Uri(cache.Path)
            };
        }
    }
}
