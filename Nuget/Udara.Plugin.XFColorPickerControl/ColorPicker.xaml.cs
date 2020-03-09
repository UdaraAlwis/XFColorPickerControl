using System;
using System.Collections;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Udara.Plugin.XFColorPickerControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPicker : ContentView
    {
        /// <summary>
        /// Occurs when the Picked Color changes
        /// </summary>
        public event EventHandler<Color> PickedColorChanged;

        /// <summary>
        /// Occurs when the Picked Color changes
        /// </summary>
        public event EventHandler<(double, double)> PickerRingPositionChanged;

        public static readonly BindableProperty PickedColorProperty
            = BindableProperty.Create(
                nameof(PickedColor),
                typeof(Color),
                typeof(ColorPicker));

        /// <summary>
        /// Get the current Picked Color
        /// </summary>
        public Color PickedColor
        {
            get { return (Color)GetValue(PickedColorProperty); }
            private set { SetValue(PickedColorProperty, value); }
        }


        public static readonly BindableProperty ColorSpectrumStyleProperty
            = BindableProperty.Create(
                nameof(ColorSpectrumStyle),
                typeof(ColorSpectrumStyle),
                typeof(ColorPicker),
                ColorSpectrumStyle.HueToShadeStyle,
                BindingMode.Default, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                        ((ColorPicker)bindable).SkCanvasView.InvalidateSurface();
                    else
                        ((ColorPicker)bindable).ColorSpectrumStyle = default;
                });

        /// <summary>
        /// Set the Color Spectrum Gradient Style
        /// </summary>
        public ColorSpectrumStyle ColorSpectrumStyle
        {
            get { return (ColorSpectrumStyle)GetValue(ColorSpectrumStyleProperty); }
            set { SetValue(ColorSpectrumStyleProperty, value); }
        }


        public static readonly BindableProperty BaseColorListProperty
            = BindableProperty.Create(
                nameof(BaseColorList),
                typeof(IEnumerable),
                typeof(ColorPicker),
                new string[]
                {
                    new Color(255, 0, 0).ToHex(), // Red
					new Color(255, 255, 0).ToHex(), // Yellow
					new Color(0, 255, 0).ToHex(), // Green (Lime)
					new Color(0, 255, 255).ToHex(), // Aqua
					new Color(0, 0, 255).ToHex(), // Blue
					new Color(255, 0, 255).ToHex(), // Fuchsia
					new Color(255, 0, 0).ToHex(), // Red
				},
                BindingMode.Default, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                        ((ColorPicker)bindable).SkCanvasView.InvalidateSurface();
                    else
                        ((ColorPicker)bindable).BaseColorList = default;
                });

        /// <summary>
        /// Sets the Base Color List
        /// </summary>
        public IEnumerable BaseColorList
        {
            get { return (IEnumerable)GetValue(BaseColorListProperty); }
            set { SetValue(BaseColorListProperty, value); }
        }


        public static readonly BindableProperty ColorFlowDirectionProperty
            = BindableProperty.Create(
                nameof(ColorFlowDirection),
                typeof(ColorFlowDirection),
                typeof(ColorPicker),
                ColorFlowDirection.Horizontal,
                BindingMode.Default, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                        ((ColorPicker)bindable).SkCanvasView.InvalidateSurface();
                    else
                        ((ColorPicker)bindable).ColorFlowDirection = default;
                });

        /// <summary>
        /// Sets the Color List flow Direction
        /// Horizontal or Verical
        /// </summary>
        public ColorFlowDirection ColorFlowDirection
        {
            get { return (ColorFlowDirection)GetValue(ColorFlowDirectionProperty); }
            set { SetValue(ColorFlowDirectionProperty, value); }
        }


        public static readonly BindableProperty PointerRingDiameterUnitsProperty
            = BindableProperty.Create(
                nameof(PointerRingDiameterUnits),
                typeof(double),
                typeof(ColorPicker),
                0.6,
                BindingMode.Default, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                        ((ColorPicker)bindable).SkCanvasView.InvalidateSurface();
                    else
                        ((ColorPicker)bindable).PointerRingDiameterUnits = default;
                });

        /// <summary>
        /// Sets the Picker Pointer Ring Diameter
        /// Value must be between 0-1
        /// Calculated against the View Canvas size
        /// </summary>
        public double PointerRingDiameterUnits
        {
            get { return (double)GetValue(PointerRingDiameterUnitsProperty); }
            set { SetValue(PointerRingDiameterUnitsProperty, value); }
        }


        public static readonly BindableProperty PointerRingBorderUnitsProperty
            = BindableProperty.Create(
                nameof(PointerRingBorderUnits),
                typeof(double),
                typeof(ColorPicker),
                0.3,
                BindingMode.Default, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                        ((ColorPicker)bindable).SkCanvasView.InvalidateSurface();
                    else
                        ((ColorPicker)bindable).PointerRingBorderUnits = default;
                });

        /// <summary>
        /// Sets the Picker Pointer Ring Border Size
        /// Value must be between 0-1
        /// Calculated against pixel size of Picker Pointer
        /// </summary>
        public double PointerRingBorderUnits
        {
            get { return (double)GetValue(PointerRingBorderUnitsProperty); }
            set { SetValue(PointerRingBorderUnitsProperty, value); }
        }


        public static readonly BindableProperty PointerPositionXUnitsProperty
            = BindableProperty.Create(
                nameof(PointerPositionXUnits),
                typeof(double),
                typeof(ColorPicker),
                0.5,
                BindingMode.OneTime, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                    {
                        ((ColorPicker)bindable).SetPickerPosition((double)newValue, ((ColorPicker)bindable).PointerPositionYUnits);
                    }
                    else
                        ((ColorPicker)bindable).ColorFlowDirection = default;
                });

        /// <summary>
        /// Sets the Picker Pointer X position
        /// Value must be between 0-1
        /// Calculated against the View Canvas Width value
        /// </summary>
        public double PointerPositionXUnits
        {
            get { return (double)GetValue(PointerPositionXUnitsProperty); }
            set { SetValue(PointerPositionXUnitsProperty, value); }
        }


        public static readonly BindableProperty PointerPositionYUnitsProperty
            = BindableProperty.Create(
                nameof(PointerPositionYUnits),
                typeof(double),
                typeof(ColorPicker),
                0.5,
                BindingMode.OneTime, null,
                propertyChanged: (bindable, value, newValue) =>
                {
                    if (newValue != null)
                    {
                        ((ColorPicker) bindable).SetPickerPosition(((ColorPicker)bindable).PointerPositionXUnits, (double)newValue);
                    }
                    else
                        ((ColorPicker)bindable).ColorFlowDirection = default;
                });

        /// <summary>
        /// Sets the Picker Pointer Y position
        /// Value must be between 0-1
        /// Calculated against the View Canvas Width value
        /// </summary>
        public double PointerPositionYUnits
        {
            get { return (double)GetValue(PointerPositionYUnitsProperty); }
            set { SetValue(PointerPositionYUnitsProperty, value); }
        }


        private SKPoint _lastTouchPoint = new SKPoint();
        private bool _checkPointerInitPositionDone = false;

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void SkCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var skImageInfo = e.Info;
            var skSurface = e.Surface;
            var skCanvas = skSurface.Canvas;

            var skCanvasWidth = skImageInfo.Width;
            var skCanvasHeight = skImageInfo.Height;

            skCanvas.Clear(SKColors.White);

            // Draw gradient rainbow Color spectrum
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                // Initiate the base Color list
                ColorTypeConverter converter = new ColorTypeConverter();
                System.Collections.Generic.List<SKColor> colors = new System.Collections.Generic.List<SKColor>();
                foreach (var color in BaseColorList)
                    colors.Add(((Color)converter.ConvertFromInvariantString(color.ToString())).ToSKColor());

                // create the gradient shader between base Colors
                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    ColorFlowDirection == ColorFlowDirection.Horizontal ?
                        new SKPoint(skCanvasWidth, 0) : new SKPoint(0, skCanvasHeight),
                    colors.ToArray(),
                    null,
                    SKShaderTileMode.Clamp))
                {
                    paint.Shader = shader;
                    skCanvas.DrawPaint(paint);
                }
            }

            // Draw secondary gradient color spectrum
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                // Initiate gradient color spectrum style layer
                var colors = GetSecondaryLayerColors(ColorSpectrumStyle);

                // create the gradient shader between secondary colors
                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    ColorFlowDirection == ColorFlowDirection.Horizontal ?
                        new SKPoint(0, skCanvasHeight) : new SKPoint(skCanvasWidth, 0),
                    colors,
                    null,
                    SKShaderTileMode.Clamp))
                {
                    paint.Shader = shader;
                    skCanvas.DrawPaint(paint);
                }
            }

            if (!_checkPointerInitPositionDone)
            {
                var x = ((float)skCanvasWidth * (float)PointerPositionXUnits);
                var y = ((float)skCanvasHeight * (float)PointerPositionYUnits);

                _lastTouchPoint = new SKPoint(x, y);

                _checkPointerInitPositionDone = true;
            }

            // Picking the Pixel Color values on the Touch Point

            // Represent the color of the current Touch point
            SKColor touchPointColor;

            // Efficient and fast
            // https://forums.xamarin.com/discussion/92899/read-a-pixel-info-from-a-canvas
            // create the 1x1 bitmap (auto allocates the pixel buffer)
            using (SKBitmap bitmap = new SKBitmap(skImageInfo))
            {
                // get the pixel buffer for the bitmap
                IntPtr dstpixels = bitmap.GetPixels();

                // read the surface into the bitmap
                skSurface.ReadPixels(skImageInfo,
                    dstpixels,
                    skImageInfo.RowBytes,
                    (int)_lastTouchPoint.X, (int)_lastTouchPoint.Y);

                // access the color
                touchPointColor = bitmap.GetPixel(0, 0);
            }

            // Painting the Touch point
            using (SKPaint paintTouchPoint = new SKPaint())
            {
                paintTouchPoint.Style = SKPaintStyle.Fill;
                paintTouchPoint.Color = SKColors.White;
                paintTouchPoint.IsAntialias = true;

                var valueToCalcAgainst = (skCanvasWidth > skCanvasHeight) ? skCanvasWidth : skCanvasHeight;

                var pointerCircleDiameterUnits = PointerRingDiameterUnits; // 0.6 (Default)
                pointerCircleDiameterUnits = (float)pointerCircleDiameterUnits / 10f; //  calculate 1/10th of that value
                var pointerCircleDiameter = (float)(valueToCalcAgainst * pointerCircleDiameterUnits);

                // Outer circle of the Pointer (Ring)
                skCanvas.DrawCircle(
                    _lastTouchPoint.X,
                    _lastTouchPoint.Y,
                    pointerCircleDiameter / 2, paintTouchPoint);

                // Draw another circle with picked color
                paintTouchPoint.Color = touchPointColor;

                var pointerCircleBorderWidthUnits = PointerRingBorderUnits; // 0.3 (Default)
                var pointerCircleBorderWidth = (float)pointerCircleDiameter *
                                                        (float)pointerCircleBorderWidthUnits; // Calculate against Pointer Circle

                // Inner circle of the Pointer (Ring)
                skCanvas.DrawCircle(
                    _lastTouchPoint.X,
                    _lastTouchPoint.Y,
                    ((pointerCircleDiameter - pointerCircleBorderWidth) / 2), paintTouchPoint);
            }

            // Set selected color
            PickedColor = touchPointColor.ToFormsColor();
            PickedColorChanged?.Invoke(this, PickedColor);
        }

        private void SkCanvasView_OnTouch(object sender, SKTouchEventArgs e)
        {
            // to fix the UWP touch bevaior
            if (Device.RuntimePlatform == Device.UWP)
            {
                // avoid mouse over touch events
                if (!e.InContact)
                    return;
            }

            _lastTouchPoint = e.Location;

            var canvasSize = SkCanvasView.CanvasSize;

            // Check for each touch point XY position to be inside Canvas
            // Ignore any Touch event ocurred outside the Canvas region 
            if ((e.Location.X > 0 && e.Location.X < canvasSize.Width) &&
                (e.Location.Y > 0 && e.Location.Y < canvasSize.Height))
            {
                e.Handled = true;

                PointerPositionXUnits = e.Location.X / canvasSize.Width;
                PointerPositionYUnits = e.Location.Y / canvasSize.Height;

                // update the Canvas as you wish
                SkCanvasView.InvalidateSurface();
            }
        }

        private SKColor[] GetSecondaryLayerColors(ColorSpectrumStyle colorSpectrumStyle)
        {
            if (colorSpectrumStyle == ColorSpectrumStyle.HueOnlyStyle)
            {
                return new SKColor[]
                {
                        SKColors.Transparent
                };
            }
            else if (colorSpectrumStyle == ColorSpectrumStyle.HueToShadeStyle)
            {
                return new SKColor[]
                {
                        SKColors.Transparent,
                        SKColors.Black
                };
            }
            else if (colorSpectrumStyle == ColorSpectrumStyle.ShadeToHueStyle)
            {
                return new SKColor[]
                {
                        SKColors.Black,
                        SKColors.Transparent
                };
            }
            else if (colorSpectrumStyle == ColorSpectrumStyle.HueToTintStyle)
            {
                return new SKColor[]
                {
                        SKColors.Transparent,
                        SKColors.White
                };
            }
            else if (colorSpectrumStyle == ColorSpectrumStyle.TintToHueStyle)
            {
                return new SKColor[]
                {
                        SKColors.White,
                        SKColors.Transparent
                };
            }
            else if (colorSpectrumStyle == ColorSpectrumStyle.TintToHueToShadeStyle)
            {
                return new SKColor[]
                {
                        SKColors.White,
                        SKColors.Transparent,
                        SKColors.Black
                };
            }
            else if (colorSpectrumStyle == ColorSpectrumStyle.ShadeToHueToTintStyle)
            {
                return new SKColor[]
                {
                        SKColors.Black,
                        SKColors.Transparent,
                        SKColors.White
                };
            }
            else
            {
                return new SKColor[]
                {
                    SKColors.Transparent,
                    SKColors.Black
                };
            }
        }

        private void SetPickerPosition(double xPositionUnits, double yPositionUnits)
        {
            PointerPositionXUnits = xPositionUnits;
            PointerPositionYUnits = yPositionUnits;

            var xPosition = SkCanvasView.CanvasSize.Width * xPositionUnits; // Calculate actual X Position
            var yPosition = SkCanvasView.CanvasSize.Height * yPositionUnits; // Calculate actual Y Position

            _lastTouchPoint = new SKPoint(Convert.ToSingle(xPosition), Convert.ToSingle(yPosition));
            SkCanvasView.InvalidateSurface();
        }
    }

    public enum ColorSpectrumStyle
    {
        HueOnlyStyle,
        HueToShadeStyle,
        ShadeToHueStyle,
        HueToTintStyle,
        TintToHueStyle,
        TintToHueToShadeStyle,
        ShadeToHueToTintStyle
    }

    public enum ColorFlowDirection
    {
        Horizontal,
        Vertical
    }
}