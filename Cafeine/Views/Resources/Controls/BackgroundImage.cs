using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Cafeine.Views.Resources.Controls
{
    public class BackgroundImage : Control
    {
        private Compositor              _compositor;
        private CompositionSurfaceBrush _compositionSurfaceBrush;
        private CompositionBrush        _compositionBrush;
        private Uri                     _source;
        private SpriteVisual            _sprite;

        public BackgroundImage()
        {
            this.DefaultStyleKey = typeof(BackgroundImage);
            this.Loading += BackgroundImage_Loading;
            this.Unloaded += BackgroundImage_Unloaded;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        }

        private void BackgroundImage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundImage_Loading(Windows.UI.Xaml.FrameworkElement sender, object args)
        {
            throw new NotImplementedException();
        }

        private async void UpdateBrush()
        {
            if (_source == null)
            {
                DisposeBrush();
                return;
            }

        }

        private void DisposeBrush()
        {
            throw new NotImplementedException();
        }

        public Uri Source {
            get => _source;
            set {
                if (_source != value)
                {
                    _source = value;
                    UpdateBrush();
                }
            }
        }
    }
}
