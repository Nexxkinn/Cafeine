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
using Windows.UI.Text;
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

            // default settings
            Width = 100;
            Height = 32;
            FontWeight = FontWeights.Medium;
            Loaded += OnLoading;
            Unloaded += OnUnloaded;
        }

        void OnLoading(object sender, object args)
        {
            // DirectX layer
            _device = CanvasDevice.GetSharedDevice();
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            
            // create text format
            using CanvasTextFormat textformat = new CanvasTextFormat
            {
                FontFamily = FontFamily.Source,
                FontSize = (float)FontSize,
                FontWeight = FontWeight,
                WordWrapping = CanvasWordWrapping.NoWrap,
                LineSpacingMode = CanvasLineSpacingMode.Proportional, // no option to make it properly centered.
                LineSpacing = 0.4f,
                LineSpacingBaseline = 0.6f,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };
            using CanvasTextLayout textLayout = new CanvasTextLayout(_device, Text, textformat, 0, 0);

            if (this.Width < textLayout.LayoutBounds.Width) 
            { 
                this.Width = textLayout.LayoutBounds.Width + 20;
            } 

            _size = new Vector2((float)Width, (float)Height);
            
            _shape = _compositor.CreateShapeVisual();
            _shape.Size = _size;

            _graphdevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _device);
            _drawsurface = _graphdevice.CreateDrawingSurface(new Size(Width, Height), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

            DrawText(textLayout);
            DrawShape();

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
            _visible.Duration = TimeSpan.FromMilliseconds(75);
            _visible.InsertKeyFrame(0f, Color.FromArgb(30, 0xff, 0xff, 0xff));
            _visible.InsertKeyFrame(0.5f, Color.FromArgb(0x40, 0x40, 0x40, 0x40));
            _visible.InsertKeyFrame(1f, Color.FromArgb(0xcc, 0x26, 0x2a, 0x2f));

            _hidden = _compositor.CreateColorKeyFrameAnimation();
            _hidden.Duration = TimeSpan.FromMilliseconds(75);
            _hidden.InsertKeyFrame(0f, Color.FromArgb(0xcc, 0x26, 0x2a, 0x2f));
            _hidden.InsertKeyFrame(0.5f, Color.FromArgb(0x40, 0x40, 0x40, 0x40));
            _hidden.InsertKeyFrame(1f, Color.FromArgb(30, 0xff, 0xff, 0xff));
        }

        void DrawText(CanvasTextLayout textLayout)
        {
            #region unused
            // text drawer method 1 - no anti-aliasing feature
            //var layout = new CanvasTextLayout(_device, Text, textformat, (float)Width, (float)Height);
            //var geometry = CanvasGeometry.CreateText(layout);
            //var compPath = new CompositionPath(geometry);
            //var pathGeo = _compositor.CreatePathGeometry(compPath);

            // set to container
            //var w = _compositor.CreateSpriteShape(pathGeo);
            //w.FillBrush = _compositor.CreateColorBrush(Color.FromArgb(0xd7, 0xff, 0xff, 0xff));
            //_shape.Shapes.Add(w);
            #endregion

            // text drawer - with anti-aliasing feature
            using var ds = CanvasComposition.CreateDrawingSession(_drawsurface);

            ds.Antialiasing = CanvasAntialiasing.Antialiased;
            //ds.DrawText(Text, rectext, Color.FromArgb(0xd7, 0xff, 0xff, 0xff), textformat);
            ds.DrawTextLayout(textLayout,(float)Width/2,(float)Height/2, Color.FromArgb(0xd7, 0xff, 0xff, 0xff));
            ds.Flush();
            ds.Dispose();

            // set to container
            var brush = _compositor.CreateSurfaceBrush(_drawsurface);
            var sprite = _compositor.CreateSpriteVisual();
                sprite.Brush = brush;
                sprite.Size = _size;
            _shape.Children.InsertAtTop(sprite);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e) => _shapecolorbrush.StartAnimation("Color", _visible);
        protected override void OnPointerExited(PointerRoutedEventArgs e)  => _shapecolorbrush.StartAnimation("Color", _hidden);
        protected override void OnGotFocus(RoutedEventArgs e)  => _shapecolorbrush.StartAnimation("Color", _visible);
        protected override void OnLostFocus(RoutedEventArgs e) => _shapecolorbrush.StartAnimation("Color", _hidden);
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
            Unloaded -= OnUnloaded;
            Loading -= OnLoading;

            _device?.Dispose();
            _device = null;

            _graphdevice?.Dispose();
            _graphdevice = null;

            _drawsurface?.Dispose();
            _drawsurface = null;

        }

        #region unused
        #if DEBUG
        private Vector2 GetElementPosition()
        {
            // current method
            var Transform = this.TransformToVisual(Window.Current.Content);
            var ElementPosition = Transform.TransformPoint(new Point(0, 0));
            return ElementPosition.ToVector2();
        }

        private Vector2 GetPointerPosition()
        {
            Vector2 UIPosition = GetElementPosition();
            var pointer = Window.Current.CoreWindow.PointerPosition;
            var bounds = Window.Current.Bounds;

            var point = new Vector2((float)(pointer.X - bounds.X), (float)(pointer.Y - bounds.Y));
            var position = point - UIPosition;
            return position;
        }
        #endif
        #endregion
    }
}
