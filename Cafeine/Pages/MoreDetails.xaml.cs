using Cafeine.Datalist;
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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

            Title.Text = DataReceived.Title;
            Rating.Text = DataReceived.My_score.ToString();
            BitmapImage bitmapImage = new BitmapImage(){ UriSource = new Uri(BaseUri, DataReceived.Imgurl) };
            image.Source = bitmapImage;
        }
    }
    
}
