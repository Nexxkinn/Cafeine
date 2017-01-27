using Cafeine.Datalist;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
namespace Cafeine
{
    public sealed partial class MoreDetails : Page
    {
        public MoreDetails()
        {
            this.InitializeComponent();

        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Receive passed data from previous page
            var DataReceived = (UserItemCollection)e.Parameter;
            Title.Text = DataReceived.Item_Title;
            User_Rating.Text = DataReceived.My_score.ToString();
            BitmapImage bitmapImage = new BitmapImage(){ UriSource = new Uri(BaseUri, DataReceived.Imgurl) };
            image.Source = bitmapImage;

            /// Proof of Concept - Parse Data Straight from MyAnimelist page
            /// Requirement : RetreiveItemDetail(Item Id, AnimeOrManga)
            /// 
            ///Task.Run(async () => await ExpandDetail(DataReceived.Id));

        }
        private void backbutton(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Animelist));
            this.Frame.BackStack.Clear();
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
