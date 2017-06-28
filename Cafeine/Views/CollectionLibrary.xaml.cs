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
        VirtualDirectory DirectoryDetail;
        public CollectionLibrary()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DirectoryDetail = (VirtualDirectory)e.Parameter;
            if (DirectoryDetail.AnimeOrManga == AnimeOrManga.anime) EpisodesLabel.Text = "Episodes Watched";
            else EpisodesLabel.Text = "Chapters read";
            Task.Run(async () => await GrabUserItemList());
        }
        async Task GrabUserItemList()
        {

            try
            {
                ItemList = await CollectionLibraryProvider.QueryUserAnimeMangaListAsync(DirectoryDetail.AnimeOrManga);
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    watch.ItemsSource = ItemList.Where(x => x.Itemproperty.My_status == DirectoryDetail.DirectoryType - 3);
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
            image.Source = new BitmapImage(new Uri(SelectedItem.Image, UriKind.Absolute));
            Title.Text = SelectedItem.Itemproperty.Item_Title;
            User_Rating.Text = SelectedItem.My_score;
            User_Episodes.Text = SelectedItem.My_watch;
            popupp.Height = Window.Current.Bounds.Height - 48;
            popupp.Width = Window.Current.Bounds.Width;
            popupp.VerticalAlignment = VerticalAlignment.Center;
            ppup.IsOpen = true;
            btn_save.Click += Btn_save_Click;
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            var ItemIndex = ItemList[ItemList.IndexOf(SelectedItem)];
            ItemIndex.My_score = User_Rating.Text;
            ItemIndex.My_watch = User_Episodes.Text;
            await CollectionLibraryProvider.UpdateItem(ItemIndex, DirectoryDetail.AnimeOrManga);
            ppup.IsOpen = false;
        }
    }
}