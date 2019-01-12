using Cafeine.Models;
using Cafeine.Services;
using Cafeine.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http.Filters;

//using Html2Markdown;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel Vm {
            get {
                return DataContext as MainPageViewModel;
            }
        }

        public MainPage()
        {

            //deletecookies();
            InitializeComponent();
        }

        private void deletecookies()
        {
            HttpBaseProtocolFilter handler = new HttpBaseProtocolFilter();
            var cookies = handler.CookieManager.GetCookies(new Uri("https://anilist.co"));
            foreach (var i in cookies)
            {
                handler.CookieManager.DeleteCookie(i);
            }
        }

        //WHAT IS THIS BLACK MAGIC??
        //HOW DOES ANY OF THIS WORKS?!?!
        //Reference : https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-gridview-and-listview#update-listview-and-gridview-items-progressively
        private void Collection_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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
