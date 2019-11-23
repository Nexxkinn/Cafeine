using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Cafeine.Shared.Controls
{
    public class CafeineFlyoutButton : Control
    {
        private Vector2 _size;

        // Visual Layer
        private Compositor _compositor;
        private ShapeVisual _shape;
        
        // Direct2D? Layer
        private CanvasDevice _device;
        private CompositionGraphicsDevice _graphdevice;
        private CompositionDrawingSurface _drawsurface;

        // reveal layer
        private CompositionSpriteShape _spriteShape;
        private CompositionColorBrush _shapecolorbrush;
        private ColorKeyFrameAnimation _hidden;
        private ColorKeyFrameAnimation _visible;

        // border reveal layer
        private CompositionColorBrush _borderBrushColor;
        private CompositionSpriteShape _borderBrushShape;
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

            var dispatcher = ElementCompositionPreview.GetElementVisual(this).Dispatcher;
            DrawShape();
            DrawText();

            ElementCompositionPreview.SetElementChildVisual(this, _shape);
        }

        void DrawShape()
        {
            // handle border shape geometry
            using var geo = _compositor.CreateRoundedRectangleGeometry();
            geo.Size = _size;
            geo.CornerRadius = new Vector2(2);
            _shapecolorbrush = _compositor.CreateColorBrush(Color.FromArgb(30, 0xff, 0xff, 0xff));
            _spriteShape = _compositor.CreateSpriteShape(geo);
            _spriteShape.FillBrush = _shapecolorbrush;
            _shape.Shapes.Add(_spriteShape);

            // Handle Visibility on composition
            _visible = _compositor.CreateColorKeyFrameAnimation();
            _visible.Duration = TimeSpan.FromMilliseconds(100);
            _visible.InsertKeyFrame(0f, Color.FromArgb(30, 0xff, 0xff, 0xff));
            _visible.InsertKeyFrame(0.5f, Color.FromArgb(0x40, 0x40, 0x40, 0x40));
            _visible.InsertKeyFrame(1f, Color.FromArgb(0xcc, 0x26, 0x2a, 0x2f));

            _hidden = _compositor.CreateColorKeyFrameAnimation();
            _hidden.Duration = TimeSpan.FromMilliseconds(100);
            _hidden.InsertKeyFrame(0f, Color.FromArgb(0xcc, 0x26, 0x2a, 0x2f));
            _hidden.InsertKeyFrame(0.5f, Color.FromArgb(0x40, 0x40, 0x40, 0x40));
            _hidden.InsertKeyFrame(1f, Color.FromArgb(30, 0xff, 0xff, 0xff));
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
                LineSpacingMode = CanvasLineSpacingMode.Proportional, // no option to make it properly centered.
                LineSpacing = 0.4f,
                LineSpacingBaseline = 0.6f,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };

            // text drawer method 1 - no anti-aliasing feature
            //var layout = new CanvasTextLayout(_device, Text, textformat, (float)Width, (float)Height);
            //var geometry = CanvasGeometry.CreateText(layout);
            //var compPath = new CompositionPath(geometry);
            //var pathGeo = _compositor.CreatePathGeometry(compPath);

            // set to container
            //var w = _compositor.CreateSpriteShape(pathGeo);
            //w.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0xd7, 0xff, 0xff, 0xff));
            //_shape.Shapes.Add(w);

            // text drawer 2 - with anti-aliasing feature
            using var ds = CanvasComposition.CreateDrawingSession(_drawsurface);
            var rectext  = new Rect(0, 0, Width, Height);
            ds.Antialiasing = CanvasAntialiasing.Antialiased;
            ds.DrawText(Text, rectext, Color.FromArgb(0xd7, 0xff, 0xff, 0xff), textformat);
            ds.Flush();
            ds.Dispose();

            // set to container
            var brush = _compositor.CreateSurfaceBrush(_drawsurface);
            var sprite = _compositor.CreateSpriteVisual();
                sprite.Brush = brush;
                sprite.Size = _size;
            _shape.Children.InsertAtTop(sprite);

        }

        private Vector2 GetElementPosition()
        {
            // current method
            var Transform = this.TransformToVisual(Window.Current.Content);
            var ElementPosition = Transform.TransformPoint(new Point(0,0));
            return ElementPosition.ToVector2();
        }

        private Vector2 GetPointerPosition() {
            Vector2 UIPosition = GetElementPosition();
            var pointer = Window.Current.CoreWindow.PointerPosition;
            var bounds = Window.Current.Bounds;

            var point = new Vector2((float)(pointer.X - bounds.X), (float)(pointer.Y - bounds.Y));
            var position = point - UIPosition;
            return position;
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            _shapecolorbrush.StartAnimation("Color", _visible);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            _shapecolorbrush.StartAnimation("Color", _hidden);
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

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            _shapecolorbrush.StartAnimation("Color", _visible);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            _shapecolorbrush.StartAnimation("Color", _hidden);
            base.OnLostFocus(e);
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
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
