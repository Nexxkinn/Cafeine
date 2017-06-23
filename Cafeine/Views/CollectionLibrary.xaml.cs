using Cafeine.Services;
using Cafeine.Models;
using Cafeine.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionLibrary : Page
    {
        ObservableCollection<CollectionLibraryViewModel> ItemList = new ObservableCollection<CollectionLibraryViewModel>();
        CollectionLibraryViewModel SelectedItem = new CollectionLibraryViewModel();
        public CollectionLibrary()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VirtualDirectory DataReceived = (VirtualDirectory)e.Parameter;
            Task.Run(async () => await GrabUserItemList(DataReceived));
        }
        async Task GrabUserItemList(VirtualDirectory c)
        {

            try
            {
                ItemList = await CollectionLibraryProvider.QueryUserAnimeMangaListAsync(c.AnimeOrManga);
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    watch.ItemsSource = ItemList.Where(x => x.Itemproperty.My_status == c.DirectoryType - 3);
                });
            }
            catch (Exception e)
            {
                //TODO: show error message?? e.Message
            }
        }
        private void NavigateItemtoDetailsPage(object sender, ItemClickEventArgs e)
        {
            //pass data to other page
            SelectedItem = (CollectionLibraryViewModel)e.ClickedItem;
            BitmapImage bitmapImage = new BitmapImage() { UriSource = new Uri(BaseUri, SelectedItem.Itemproperty.Imgurl) };
            image.Source = bitmapImage;
            Title.Text = SelectedItem.Itemproperty.Item_Title;

            User_Rating.Text = SelectedItem.My_score.ToString();
            popupp.Height = Window.Current.Bounds.Height - 48;
            popupp.Width = Window.Current.Bounds.Width;
            popupp.VerticalAlignment = VerticalAlignment.Center;
            ppup.IsOpen = true;

            SelectedItem.Itemproperty.My_score = 100;
            btn_save.Click += Btn_save_Click;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            ItemList[ItemList.IndexOf(SelectedItem)].My_score = Convert.ToInt32(User_Rating.Text);
            ppup.IsOpen = false;
        }
    }
}
