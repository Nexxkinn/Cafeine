using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.ViewModels;
using Cafeine.Views.Resources;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
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
    public sealed partial class ItemDetailsPage : BasePage
    {
        public ItemDetailsPage()
        {
            this.InitializeComponent();
        }

        // TODO : use composition to enable gridview virtualization

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

            string progress = "Clamp(Abs(america.Translation.Y) / 232.0, 0.0, 1.0)";

            // Shift the header by 50 pixels when scrolling down
            var offsetExpression = compositor.CreateExpressionAnimation($"-america.Translation.Y - {progress} * 232");
            offsetExpression.SetReferenceParameter("america", scrollerPropertySet);

            // Shift the option button by 244 pixel (?) when scrolling down
            var itemoptionsvisual = ElementCompositionPreview.GetElementVisual(ItemOptions);
            itemoptionsvisual.StartAnimation("offset.Y", offsetExpression);

            var episodelistconfvisual = ElementCompositionPreview.GetElementVisual(EpisodesListConfiguration);
            episodelistconfvisual.StartAnimation("offset.Y", offsetExpression);

            // add a patch background.
            var backgroundadder = ElementCompositionPreview.GetElementVisual(backgroundadded);
            string _backExpression = "Clamp(Abs(america.Translation.Y) / 276.0, 0.0, 1.0)";
            var _backoffset = compositor.CreateExpressionAnimation($"({progress} * 232) - ({_backExpression} * 276)");
            _backoffset.SetReferenceParameter("america", scrollerPropertySet);

            backgroundadder.StopAnimation("offset.Y");
            backgroundadder.StartAnimation("offset.Y", _backoffset);
        }

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            var gfxEffect = new BlendEffect
            {
                Mode = BlendEffectMode.Multiply,
                Background = new ColorSourceEffect
                {
                    Name = "Darken",
                    Color = Color.FromArgb(102, 0, 0, 0),
                },
                Foreground = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    BlurAmount = 20.0f,
                    BorderMode = EffectBorderMode.Hard,
                    Optimization = EffectOptimization.Speed,
                    Source = new CompositionEffectSourceParameter("ImageSource")
                }
            };


            Visual _backgroundVisual = ElementCompositionPreview.GetElementVisual(BackgroundGrid);
            Compositor _backgroundCompositor = _backgroundVisual.Compositor;

            var effectBrush = _backgroundCompositor.CreateEffectFactory(gfxEffect).CreateBrush();

            var destinationBrush = _backgroundCompositor.CreateBackdropBrush();
            effectBrush.SetSourceParameter("ImageSource", destinationBrush);

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

            //start the storyboard
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

        private void Episodesitem_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            ItemDetailsList item = args.ItemContainer.ContentTemplateRoot as ItemDetailsList;
            args.RegisterUpdateCallback(item.LoadImage);
            args.Handled = true;
        }
    }
}
