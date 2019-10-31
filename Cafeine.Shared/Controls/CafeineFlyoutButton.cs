using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Cafeine.Views.Resources.Controls
{
    public class CafeineFlyoutButton : Control
    {
        private Vector2 _size;
        private TypedEventHandler<CoreWindow,PointerEventArgs> PositionHandler;

        // Visual Layer
        private Compositor _compositor;
        private ShapeVisual _shape;
        
        // DirectX? Layer
        private CanvasDevice _device;
        private CompositionGraphicsDevice _graphdevice;
        private CompositionDrawingSurface _drawsurface;

        // reveal layer
        private CompositionSpriteShape _spriteShape;
        private CompositionColorBrush _shapecolorbrush;
        private ColorKeyFrameAnimation _hidden;
        private ColorKeyFrameAnimation _visible;

        // border reveal layer
        private CompositionColorGradientStop lightgradientbrush;
        private ColorKeyFrameAnimation _borderHidden;
        private ColorKeyFrameAnimation _borderVisible;


        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CafeineFlyoutButton), new PropertyMetadata(string.Empty));

        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(CafeineFlyoutButton), new PropertyMetadata(string.Empty));

        public CafeineFlyoutButton()
        {
            DefaultStyleKey = typeof(CafeineFlyoutButton);
            HorizontalAlignment = HorizontalAlignment.Left;
            Width = 100;
            Height = 32;

            Loaded += OnLoading;
            Unloaded += OnUnloaded;
        }

        void OnLoading(object sender, object args)
        {
            // get Visual layer and compositor
            _size = new Vector2((float)Width, (float)Height);
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _shape = _compositor.CreateShapeVisual();
            _shape.Size = _size;

            // DirectX layer
            _device = CanvasDevice.GetSharedDevice();
            _graphdevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _device);
            _drawsurface = _graphdevice.CreateDrawingSurface(new Size(Width, Height), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

            DrawShape();
            DrawText();
            DrawRevealBorder();
            
            ElementCompositionPreview.SetElementChildVisual(this, _shape);
        }
        void DrawShape()
        {
            // handle border shape geometry
            using var geo = _compositor.CreateRoundedRectangleGeometry();
            geo.Size = _size;
            geo.CornerRadius = new Vector2(2);
            _shapecolorbrush = _compositor.CreateColorBrush(Colors.Transparent);
            _spriteShape = _compositor.CreateSpriteShape(geo);
            _spriteShape.FillBrush = _shapecolorbrush;
            _shape.Shapes.Add(_spriteShape);

            // Handle Visibility on composition
            _visible = _compositor.CreateColorKeyFrameAnimation();
            _visible.Duration = TimeSpan.FromMilliseconds(100);
            _visible.InsertKeyFrame(0f, Colors.Transparent);
            _visible.InsertKeyFrame(1f, Color.FromArgb(30, 0xff, 0xff, 0xff));

            _hidden = _compositor.CreateColorKeyFrameAnimation();
            _hidden.Duration = TimeSpan.FromMilliseconds(100);
            _hidden.InsertKeyFrame(0f, Color.FromArgb(30, 0xff, 0xff, 0xff));
            _hidden.InsertKeyFrame(1f, Colors.Transparent);
        }

        void DrawText()
        {

            // create text format
            CanvasTextFormat textformat = new CanvasTextFormat
            {
                FontFamily = FontFamily.Source,
                FontSize = (float)FontSize,
                FontWeight = FontWeight,
                WordWrapping = CanvasWordWrapping.WholeWord,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };

            // text drawer
            using (var ds = CanvasComposition.CreateDrawingSession(_drawsurface))
            {
                ds.Antialiasing = CanvasAntialiasing.Antialiased;
                var rectext = new Rect(0, 0, Width, Height);
                ds.DrawText(Text, rectext, Color.FromArgb(0xd7, 0xff, 0xff, 0xff), textformat);

            }

            // set to container
            var brush = _compositor.CreateSurfaceBrush(_drawsurface);
            brush.Surface = _drawsurface;

            var sprite = _compositor.CreateSpriteVisual();
            sprite.Brush = brush;
            sprite.Size = _size;
            _shape.Children.InsertAtTop(sprite);

        }

        void DrawRevealBorder()
        {
            // handle reveal brush effect
            var _revealBorderBrush = _compositor.CreateRadialGradientBrush();
            CompositionColorGradientStop lightgradientbrush2 = _compositor.CreateColorGradientStop(1, Color.FromArgb(0x00, 0xff, 0xff, 0xff));
                                         lightgradientbrush  = _compositor.CreateColorGradientStop(0, Color.FromArgb(0x7f, 0xff, 0xff, 0xff));
            _revealBorderBrush.ColorStops.Add(lightgradientbrush);
            _revealBorderBrush.ColorStops.Add(lightgradientbrush2);
            _revealBorderBrush.MappingMode = CompositionMappingMode.Absolute;
            _revealBorderBrush.EllipseRadius = new Vector2(100f);
            _revealBorderBrush.Offset = GetPointerPosition(); // reference : https://stackoverflow.com/a/44906442/

            // handle outer shape geometry
            var bordergeo = _compositor.CreateRoundedRectangleGeometry();
            bordergeo.Size = Vector2.Subtract(_size, new Vector2(1));
            bordergeo.Offset = new Vector2(0.5f);
            bordergeo.CornerRadius = new Vector2(2);

            // create sprite shapes
            var _borderBrushShape = _compositor.CreateSpriteShape(bordergeo);
            _borderBrushShape.StrokeBrush = _revealBorderBrush;
            _borderBrushShape.StrokeThickness = 1;
            //_fillBrushShape.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0xff, 0x26, 0x2a, 0x2f));

            // set color animation for _revealBorderBrush
            _borderVisible = _compositor.CreateColorKeyFrameAnimation();
            _borderVisible.Duration = TimeSpan.FromMilliseconds(100);
            _borderVisible.InsertKeyFrame(0f, Color.FromArgb(0x0, 0xff, 0xff, 0xff));
            _borderVisible.InsertKeyFrame(1f, Color.FromArgb(0x7f, 0xff, 0xff, 0xff));

            _borderHidden = _compositor.CreateColorKeyFrameAnimation();
            _borderHidden.Duration = TimeSpan.FromMilliseconds(100);
            _borderHidden.InsertKeyFrame(0f, Color.FromArgb(0x7f, 0xff, 0xff, 0xff));
            _borderHidden.InsertKeyFrame(1f, Color.FromArgb(0x0, 0xff, 0xff, 0xff));

            // final setup
            _shape.Shapes.Add(_borderBrushShape);
            PositionHandler = async (s, e) =>
            {
                _revealBorderBrush.Offset = GetPointerPosition();
            };
            Window.Current.CoreWindow.PointerMoved += PositionHandler;
        }

        private Vector2 GetPointerPosition()
        {
            var Transform = this.TransformToVisual(Window.Current.Content);
            var pointer = Window.Current.CoreWindow.PointerPosition;
            var bounds = Window.Current.Bounds;

            var point = new Point(pointer.X - bounds.X, pointer.Y - bounds.Y);
            var position  = Transform.Inverse.TransformPoint(point);
            return position.ToVector2();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            _shapecolorbrush.StartAnimation("Color", _visible);
            lightgradientbrush.StartAnimation("Color", _borderHidden);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            _shapecolorbrush.StartAnimation("Color", _hidden);
            lightgradientbrush.StartAnimation("Color", _borderVisible);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if(this.ContextFlyout != null)
            {
                this.ContextFlyout.Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom;
                this.ContextFlyout.ShowAt(this);
            }
            base.OnTapped(e);
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerMoved -= PositionHandler;
            Unloaded -= OnUnloaded;
            Loading -= OnLoading;

            _device?.Dispose();
            _device = null;

            _graphdevice?.Dispose();
            _graphdevice = null;

            _drawsurface?.Dispose();
            _drawsurface = null;

        }
    }
}
