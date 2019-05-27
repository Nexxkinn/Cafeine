using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.ViewModels;
using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : BasePage
    {
        public SearchViewModel Vm => DataContext as SearchViewModel;
        private double _listviewheight;
        private double _gridviewheight;
        public SearchPage()
        {
            this.InitializeComponent();
        }

        private void Vm_ItemIsSearched(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Onlineresultgridview_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.RegisterUpdateCallback(LoadImage);
            args.Handled = true;
        }
        private async void LoadImage(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var imageurl = (args.Item as ServiceItem).CoverImageUri;
            var image = templateRoot.Children[0] as Image;

            var file = await ImageCache.GetFromCacheAsync(imageurl.AbsoluteUri);
            image.Source = new BitmapImage { UriSource = new Uri(file.Path) };

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

            Storyboard.SetTarget(ImageOpenedOpacity, image);
            Storyboard.SetTargetProperty(ImageOpenedOpacity, "Opacity");
            ImageOpenedOpacity.Begin();

        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var ListViewheight = (sender as FrameworkElement).ActualHeight;
            if (ListViewheight == _listviewheight) return;

            _listviewheight = ListViewheight;
            CompositionPropertySet scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(pagescroll);
            Compositor compositor = scrollerPropertySet.Compositor;

            string progress = $"Clamp(Floor(Abs(doe.Translation.Y) / {_listviewheight}), 0.0, 1.0)";

            // Shift the header by 50 pixels when scrolling down
            var offsetExpression = compositor.CreateExpressionAnimation($"-doe.Translation.Y + ({progress}*(doe.Translation.Y + {_listviewheight})) ");
            offsetExpression.SetReferenceParameter("doe", scrollerPropertySet);

            // Shift the option button by 244 pixel (?) when scrolling down
            var flTextBoxVisual = ElementCompositionPreview.GetElementVisual(fromLibraryTextBox);
            flTextBoxVisual.StopAnimation("offset.Y");
            flTextBoxVisual.StartAnimation("offset.Y", offsetExpression);
        }

        private void Onlineresultgridview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var GridViewheight = (sender as FrameworkElement).ActualHeight;
            if (GridViewheight == _gridviewheight) return;

            _gridviewheight = GridViewheight;
            CompositionPropertySet scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(pagescroll);
            Compositor compositor = scrollerPropertySet.Compositor;

            string progress = $"Clamp(Floor(Abs(doe.Translation.Y) / ({_listviewheight} + 52)), 0.0, 1.0)";

            // Shift the header by 50 pixels when scrolling down
            var offsetExpression = compositor.CreateExpressionAnimation($"{_listviewheight} + 52  - ({progress}*(doe.Translation.Y + {_listviewheight} + 52)) ");
            offsetExpression.SetReferenceParameter("doe", scrollerPropertySet);

            var orVisual = ElementCompositionPreview.GetElementVisual(onlineresultsTextBox);
            orVisual.StartAnimation("offset.Y", offsetExpression);
        }
    }
}
