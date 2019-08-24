using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Shared.Models;
using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Cafeine.Views.Resources
{
    /// Please do NOT use MVVM pattern in this usercontrol.
    public sealed partial class ItemDetailsList : UserControl
    {
        // handled in SetMediaList()
        private IMediaList MainMediaList;
        private List<IMediaList> SubMediaList;

        public ItemDetailsList()
        {
            this.InitializeComponent();
        }

        public MediaList contentlist {
            get { return (MediaList)GetValue(contentlistProperty); }
            set { if (value != null) SetValue(contentlistProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for contentlist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty contentlistProperty =
            DependencyProperty.Register("contentlist", typeof(MediaList), typeof(ItemDetailsList), null);

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EpisodeNumber.Text = contentlist.GenerateEpisodeNumber();
            EpisodeTitle.Text = contentlist.Title;
            SetMediaList();
        }
        private void SetMediaList()
        {
            SubMediaList = new List<IMediaList>();

            if(contentlist.Files != null && contentlist.Files.Count != 0)
            {
                MainMediaList = contentlist.Files[0];
                if (contentlist.Files.Count > 1)
                {
                    SubMediaList.AddRange(contentlist.Files.GetRange(1, contentlist.Files.Count - 1));
                }
                SubMediaList.AddRange(contentlist.StreamingServices);
            }
            else
            {
                MainMediaList = contentlist.StreamingServices[0];
                if(contentlist.StreamingServices.Count > 1)
                {
                    SubMediaList.AddRange(contentlist.StreamingServices.GetRange(1, contentlist.StreamingServices.Count - 1));
                }
            }

            this.FindName(nameof(MediaList));
            StreamServiceGrid.ItemsSource = SubMediaList;
            MainMediaListTitle.Text = MainMediaList.Source;
        }
        
        // This is only get called by ItemDetailsPage.Episodesitem_ContainerContentChanging
        public async void LoadImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var file = await ImageCache.GetFromCacheAsync(contentlist.Thumbnail.AbsoluteUri);
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                var img = new BitmapImage
                {
                    DecodePixelWidth = 180,
                    CreateOptions = BitmapCreateOptions.IgnoreImageCache
                };
                await img.SetSourceAsync(fileStream);
                Thumbnail.Source = img;
            }

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(700)),
                EasingFunction = new ExponentialEase
                {
                    Exponent = 7,
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard ImageOpenedOpacity = new Storyboard();
            ImageOpenedOpacity.Children.Add(animation);

            Storyboard.SetTarget(ImageOpenedOpacity, Thumbnail);
            Storyboard.SetTargetProperty(ImageOpenedOpacity, "Opacity");
            ImageOpenedOpacity.Begin();
        }

        private void GetPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", false);
        }

        private void GetPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void ItemClicked()
        {

        }

        private void GetTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse) return;

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void ItemTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
