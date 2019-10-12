using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Cafeine.Views.Resources.Controls
{
    public class CafeineFlyoutButton : Control
    {
        private Vector2 _size;

        // Visual Layer
        private Compositor _compositor;
        private ShapeVisual _shape;

        // DirectX? Layer
        private CanvasDevice _device;
        private CompositionGraphicsDevice _graphdevice;
        private CompositionDrawingSurface _drawsurface;

        // reveal layer
        private CompositionRadialGradientBrush _revealBrush;
        private CompositionSpriteShape _radialBrushShape;
        private ExpressionAnimation _revealAnimation;


        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CafeineFlyoutButton), new PropertyMetadata(string.Empty));

        public CafeineFlyoutButton()
        {
            this.DefaultStyleKey = typeof(CafeineFlyoutButton);
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.Loaded += OnLoading;
            this.Unloaded += OnUnloaded;
        }

        void OnLoading(object sender, object args)
        {
            // get Visual layer and compositor
            _size = new Vector2((float)Width, (float)Height);
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _shape = _compositor.CreateShapeVisual();

            // DirectX layer
            _device = CanvasDevice.GetSharedDevice();
            _graphdevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _device);
            _drawsurface = _graphdevice.CreateDrawingSurface(new Size(Width, Height), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

            DrawShape();
            DrawText();

            ElementCompositionPreview.SetElementChildVisual(this, _shape);
        }

        void DrawShape()
        {
            // handle reveal effect
            _revealBrush = _compositor.CreateRadialGradientBrush();

            CompositionColorGradientStop light = _compositor.CreateColorGradientStop(0 , Color.FromArgb(30, 255, 255, 255));
            CompositionColorGradientStop outer = _compositor.CreateColorGradientStop(1 , Colors.Transparent);

            _revealBrush.ColorStops.Add(light);
            _revealBrush.ColorStops.Add(outer);
            _revealBrush.MappingMode = CompositionMappingMode.Absolute;
            _revealBrush.EllipseRadius = new Vector2(100f);

            // handle border shape geometry
            var bordergeo = _compositor.CreateRoundedRectangleGeometry();
            bordergeo.Size = _size;
            bordergeo.CornerRadius = new Vector2(2);

            // handle shape geometry
            var geoshape = _compositor.CreateRoundedRectangleGeometry();
            geoshape.Size = Vector2.Subtract(_size, new Vector2(2));
            geoshape.Offset = new Vector2(1);
            geoshape.CornerRadius = new Vector2(2);

            // create sprite shapes
            _radialBrushShape = _compositor.CreateSpriteShape(geoshape);
            _radialBrushShape.FillBrush = _revealBrush;

            var _borderBrushShape = _compositor.CreateSpriteShape(bordergeo);
            _borderBrushShape.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            
            var _fillBrushShape = _compositor.CreateSpriteShape(geoshape);
            _fillBrushShape.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0xff, 0x26, 0x2a, 0x2f));

            _shape.Shapes.Add(_borderBrushShape);
            _shape.Shapes.Add(_fillBrushShape);
            _shape.Size = _size;

            // handle pointer track
            var CtrlPropSet = ElementCompositionPreview.GetPointerPositionPropertySet(this);
            _revealAnimation = _compositor.CreateExpressionAnimation("Vector2(hover.Position.X,hover.Position.Y)");
            _revealAnimation.SetReferenceParameter("hover", CtrlPropSet);
        }

        void DrawText()
        {

            // create text format
            CanvasTextFormat textformat = new CanvasTextFormat
            {
                FontFamily = this.FontFamily.Source,
                FontSize = (float)this.FontSize,
                FontWeight = this.FontWeight,
                WordWrapping = CanvasWordWrapping.WholeWord,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };

            // text drawer
            using (var ds = CanvasComposition.CreateDrawingSession(_drawsurface))
            {
                ds.Antialiasing = CanvasAntialiasing.Antialiased;
                var rect = new Rect(0, 0, Width, Height);
                ds.DrawText(Text, rect , Color.FromArgb(0xd7,0xff,0xff,0xff),textformat);

            }

            // set to container
            var brush = _compositor.CreateSurfaceBrush(_drawsurface);
            brush.Surface = _drawsurface;

            var     sprite = _compositor.CreateSpriteVisual();
            sprite.Brush = brush;
            sprite.Size = _size;
            _shape.Children.InsertAtTop(sprite);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            _revealBrush.StartAnimation("Offset", _revealAnimation);
            _shape.Shapes.Add(_radialBrushShape);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            _revealBrush.StopAnimation("Offset");
            _shape.Shapes.Remove(_radialBrushShape);
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= OnUnloaded;
            this.Loading -= OnLoading;

            _device?.Dispose();
            _device = null;

            _graphdevice?.Dispose();
            _graphdevice = null;

            _drawsurface?.Dispose();
            _drawsurface = null;

            _revealBrush?.Dispose();
            _revealBrush = null;

            _radialBrushShape?.Dispose();
            _radialBrushShape = null;

            _revealAnimation?.Dispose();
            _revealAnimation = null;

        }

    }
}
