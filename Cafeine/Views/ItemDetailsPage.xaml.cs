using Cafeine.Models;
using Cafeine.Services;
using Cafeine.ViewModels;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemDetailsPage : Page
    {
        public ItemDetailsPageViewModel Vm {
            get {
                return DataContext as ItemDetailsPageViewModel;
            }
        }

        public ItemDetailsPage()
        {
            this.InitializeComponent();
        }

        private async void Episodesitem_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var imageurl = (args.Item as Episode).OnlineThumbnail;
            var cache = await ImageCache.GetFromCacheAsync(imageurl);
            var image = templateRoot.Children[0] as Image;
            image.Source = new BitmapImage()
            {
                UriSource = new Uri(cache.Path)
            };
            image.Opacity = 1;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }
        }
        //View mode
        private void OnGridViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Here I'm calculating the number of columns I want based on
            // the width of the page
            var columns = Math.Ceiling(ActualWidth / 625);
            ((ItemsWrapGrid)episodesitem.ItemsPanelRoot).ItemWidth = e.NewSize.Width / columns;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //This whole thing is pure black magic. Literally undebugable.

            CompositionPropertySet scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(ItemDetailScroller);
            Compositor compositor = scrollerPropertySet.Compositor;

            string progress = "Clamp(Abs(america.Translation.Y) / 244.0, 0.0, 1.0)";

            // Shift the header by 50 pixels when scrolling down
            var offsetExpression = compositor.CreateExpressionAnimation($"-america.Translation.Y - {progress} * 244");
            offsetExpression.SetReferenceParameter("america", scrollerPropertySet);

            // Shift the option button by 244 pixel (?) when scrolling down
            var itemoptionsvisual = ElementCompositionPreview.GetElementVisual(ItemOptions);
            itemoptionsvisual.StartAnimation("offset.Y", offsetExpression);
        }

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            GaussianBlurEffect blurEffect = new GaussianBlurEffect()
            {
                Name = "Blur",
                BlurAmount = 20.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Speed,
                Source = new CompositionEffectSourceParameter("Backdrop")
            };


            Visual _backgroundVisual = ElementCompositionPreview.GetElementVisual(BackgroundGrid);
            Compositor _backgroundCompositor = _backgroundVisual.Compositor;

            var effectBrush = _backgroundCompositor.CreateEffectFactory(blurEffect).CreateBrush();

            var destinationBrush = _backgroundCompositor.CreateBackdropBrush();
            effectBrush.SetSourceParameter("Backdrop", destinationBrush);

            //Generate a UIElement visual.
            var blurSprite = _backgroundCompositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)BackgroundGrid.ActualWidth, (float)BackgroundGrid.ActualHeight);
            blurSprite.Brush = effectBrush;
            //Then "inject" it to the XAML.
            ElementCompositionPreview.SetElementChildVisual(BackgroundGrid, blurSprite);

            //create animation for opacity of the image from 0 -> 1
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            animation.EnableDependentAnimation = true;

            //create storyboard.
            Storyboard loadImageOpacity = new Storyboard();
            loadImageOpacity.Children.Add(animation);

            //what kind of bullshittery to make these method as global method??
            Storyboard.SetTarget(animation, BackgroundImage);
            Storyboard.SetTargetProperty(animation, "Opacity");
            loadImageOpacity.Begin();
        }

        private void OnBackgroundGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Visual blur = ElementCompositionPreview.GetElementChildVisual(BackgroundGrid);
            if (blur != null)
            {
                blur.Size = new Vector2((float)BackgroundGrid.ActualWidth, (float)BackgroundGrid.ActualHeight);
                ElementCompositionPreview.SetElementChildVisual(BackgroundGrid, blur);
            }
        }
    }
}
