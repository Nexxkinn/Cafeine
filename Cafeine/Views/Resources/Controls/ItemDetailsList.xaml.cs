using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Shared.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
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
            this.Loaded += (s, e) => SetBinding();
        }

        public MediaList contentlist {
            get { return (MediaList)GetValue(contentlistProperty); }
            set { SetValue(contentlistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for contentlist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty contentlistProperty =
            DependencyProperty.Register("contentlist", typeof(MediaList), typeof(ItemDetailsList), null);

        private void SetBinding()
        {
            EpisodeNumber.Text = contentlist.GenerateEpisodeNumber();
            EpisodeTitle.Text = contentlist.Title;

            SubMediaList = new List<IMediaList>();

            // if offline exists, set as the main media
            if(contentlist.Files != null && contentlist.Files.Count != 0)
            {
                MainMediaList = contentlist.Files[0];
                if (contentlist.Files.Count > 1)
                {
                    SubMediaList.AddRange(contentlist.Files.GetRange(1, contentlist.Files.Count - 1));
                }
                // add the rest.
                if (contentlist.Streams != null)
                {
                    SubMediaList.AddRange(contentlist.Streams);
                }
            }
            // set the first listed stream service as the main media.
            else
            {
                MainMediaList = contentlist.Streams[0];
                if(contentlist.Streams.Count > 1)
                {
                    SubMediaList.AddRange(contentlist.Streams.GetRange(1, contentlist.Streams.Count - 1));
                }
            }
            StreamServiceGrid.ItemsSource = SubMediaList;
            MainMediaListTitle.Text = MainMediaList.Source;
            MainMediaListIcon.Glyph = MainMediaList.Icon;
        }
        
        // This is only get called by ItemDetailsPage.Episodesitem_ContainerContentChanging
        public async void LoadImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var img = new BitmapImage
            {
                DecodePixelWidth = 180,
                DecodePixelType = DecodePixelType.Logical,
                CreateOptions = BitmapCreateOptions.IgnoreImageCache
            };
            Thumbnail.Source = img;

            if ( contentlist.State != Models.MediaList.MediaListState.OFFLINE )
            {
                // thumbnail is from online source.
                var file = await ImageCache.GetFromCacheAsync(contentlist.Thumbnail.AbsoluteUri);
                img.UriSource = new Uri(file.Path);
            }
            else
            {
                // thumbnail is from offline source.
                var file = await StorageFile.GetFileFromPathAsync(contentlist.Thumbnail.LocalPath);
                var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.VideosView);
                if (thumbnail != null) await img.SetSourceAsync(thumbnail);
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

        // for mouse-based pointer event
        private void GetPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (SubMediaList.Count == 0) VisualStateManager.GoToState(this, nameof(PointerOver), false);
            else VisualStateManager.GoToState(this, nameof(MediaDescPointerOver), false);

            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        // for mouse-based pointer event
        private void GetPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (SubMediaList.Count == 0) VisualStateManager.GoToState(this, nameof(Normal), false);
            else VisualStateManager.GoToState(this, nameof(MediaDescNormal), false);

            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        // for touch-based click
        private void GetTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse) return;

            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        // for touch-based link clicked
        private void ItemTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        // For Composition
        private void ContentPresenterLoaded(object sender, RoutedEventArgs e)
        {
            var root = (UIElement)sender;
            var rootVisual = ElementCompositionPreview.GetElementVisual(root);
            var compositor = rootVisual.Compositor;

            var pointerEnteredAnimation = compositor.CreateVector3KeyFrameAnimation();
            pointerEnteredAnimation.InsertKeyFrame(1.0f, new Vector3(1.1f));

            var pointerExitedAnimation = compositor.CreateVector3KeyFrameAnimation();
            pointerExitedAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f));

            root.PointerEntered += (s, a) =>
            {
                rootVisual.CenterPoint = new Vector3(rootVisual.Size / 2, 0);
                rootVisual.StartAnimation("Scale", pointerEnteredAnimation);
            };

            root.PointerExited += (s, a) => rootVisual.StartAnimation("Scale", pointerExitedAnimation);
        }
    }
}
