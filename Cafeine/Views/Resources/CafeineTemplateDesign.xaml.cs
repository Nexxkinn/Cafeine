using Cafeine.Models;
using Cafeine.Services;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace Cafeine.Views.Resources
{
    public partial class Template
    {
        public Template()
        {
            InitializeComponent();
        }

        private void ImageEntered(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((Grid)sender).Resources["ImageOnHover"] as Storyboard;
            sb.Begin();
        }

        private void ImageExited(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((Grid)sender).Resources["ImageOffHover"] as Storyboard;
            sb.Begin();
        }

        private void CoverImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            Storyboard sb = ((Image)sender).Resources["ImageOpened"] as Storyboard;
            sb.Begin();
        }

        private void SuggestionsList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            //Load grid background for autosuggestbox itemsuggested
            //Datatemplate -> SuggestItemTemplate -> SuggestedItemGrid.background
            args.RegisterUpdateCallback(LoadImage);
        }

        private async void LoadImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var imageurl = (args.Item as ItemLibraryModel).Service["default"].CoverImageUri;
            var cache = await ImageCache.GetFromCacheAsync(imageurl);
            var imagegrid = templateRoot.Children[0] as Grid;

            var imagebrush = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(cache.Path)),
                Opacity = 0,
                Stretch = Stretch.UniformToFill,
                AlignmentY = AlignmentY.Top,
            };
            imagebrush.ImageOpened += SuggestedItemBackground_ImageOpened;
            imagegrid.Background = imagebrush;

            InitializeBlur(imagegrid);
        }

        private void InitializeBlur(UIElement element)
        {
            var UIelement = element as Grid;
            GaussianBlurEffect blurEffect = new GaussianBlurEffect()
            {
                Name = "Blur",
                BlurAmount = 20.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Speed,
                Source = new CompositionEffectSourceParameter("Backdrop")
            };
            Visual _backgroundVisual = ElementCompositionPreview.GetElementVisual(element);
            Compositor _backgroundCompositor = _backgroundVisual.Compositor;

            var effectBrush = _backgroundCompositor.CreateEffectFactory(blurEffect).CreateBrush();

            var destinationBrush = _backgroundCompositor.CreateBackdropBrush();
            effectBrush.SetSourceParameter("Backdrop", destinationBrush);

            //Generate a UIElement visual.
            var blurSprite = _backgroundCompositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)UIelement.ActualWidth, (float)UIelement.ActualHeight);
            blurSprite.Brush = effectBrush;
            //Then "inject" it to the XAML.
            ElementCompositionPreview.SetElementChildVisual(element, blurSprite);
        }

        private void SuggestedItemBackground_ImageOpened(object sender, RoutedEventArgs e)
        {
            var BackgroundImage = sender as ImageBrush;
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
        
        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            var item = sv.FindName("CControl") as Grid;
            var scrollpropertyset = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(sv);
            var composition = scrollpropertyset.Compositor;
            var offset = composition.CreateExpressionAnimation("-america.Translation.Y");
            offset.SetReferenceParameter("america", scrollpropertyset);

            var itemoption = ElementCompositionPreview.GetElementVisual(item);
            itemoption.StartAnimation("Offset.Y", offset);
        }

       
    }
}