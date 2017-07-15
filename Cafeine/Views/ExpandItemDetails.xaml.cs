using Cafeine.Models;
using Cafeine.Services;
using Cafeine.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine
{
    public sealed partial class ExpandItemDetails : ContentDialog
    {
        public CollectionLibraryViewModel Item;
        public AnimeOrManga category;
        //public ItemProperties SearchedItem;
        public ExpandItemDetails()
        {
            this.InitializeComponent();
            this.Opened += ExpandItemDetails_Opened;
        }
        
        private void ExpandItemDetails_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            BackgroundBlur.Source = new BitmapImage(new Uri(Item.Image, UriKind.Absolute));
            //pass data to other page
            if (category == AnimeOrManga.anime) EpisodesLabel.Text = "Episodes Watched";
            else EpisodesLabel.Text = "Chapters read";
            //image.Source = new BitmapImage(new Uri(Item.Image, UriKind.Absolute))
            Title.Text = Item.Itemproperty.Item_Title;
            User_Rating.Text = Item.My_score;
            User_Episodes.Text = Item.My_watch;
            popupp.VerticalAlignment = VerticalAlignment.Center;
        }

        private void ExpandItemDetails_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            //clear data;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Item.My_score = User_Rating.Text;
            Item.My_watch = User_Episodes.Text;
            await CollectionLibraryProvider.UpdateItem(Item, category);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Item = null;
        }
    }
}
