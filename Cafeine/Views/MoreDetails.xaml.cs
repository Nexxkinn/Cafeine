using Cafeine.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Cafeine.Services;
using Cafeine.ViewModels;
namespace Cafeine
{
    public sealed partial class MoreDetails : Page
    {
        ItemProperties item;
        public MoreDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Receive passed data from previous page
            BitmapImage bitmapImage = new BitmapImage(){ UriSource = new Uri(BaseUri, item.Imgurl) };
            Title.Text = item.Item_Title;

            User_Rating.Text = item.My_score.ToString();
            image.Source = bitmapImage;
            
            /// Proof of Concept - Parse Data Straight from MyAnimelist page
            /// Requirement : RetreiveItemDetail(Item Id, AnimeOrManga)
            /// 
            ///Task.Run(async () => await ExpandDetail(DataReceived.Id));

        }
        #region ExpandDetail
        //async Task ExpandDetail(int id)
        //{
        //    try
        //    {
        //        ItemDetails Item = await ExpandedItemDetail.RetreiveItemDetail(id, 1);
        //        await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
        //        {
        //            Description.Text = Item.ItemSynopsis;
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}
        #endregion
            
    }
    
}
