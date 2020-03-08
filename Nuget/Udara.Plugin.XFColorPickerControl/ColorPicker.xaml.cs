﻿using System;
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


        public static readonly BindableProperty GradientColorStyleProperty
            = BindableProperty.Create(
                nameof(GradientColorStyle),
                typeof(GradientColorStyle),
                typeof(ColorPicker),
                GradientColorStyle.ColorsToDarkStyle,
                BindingMode.OneTime, null);

        /// <summary>
        /// Set the Color Spectrum Gradient Style
        /// </summary>
        public GradientColorStyle GradientColorStyle
        {
            get { return (GradientColorStyle)GetValue(GradientColorStyleProperty); }
            set { SetValue(GradientColorStyleProperty, value); }
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


        public static readonly BindableProperty PointerCircleDiameterUnitsProperty
            = BindableProperty.Create(
                nameof(PointerCircleDiameterUnits),
                typeof(double),
                typeof(ColorPicker),
                0.6,
                BindingMode.OneTime);

        /// <summary>
        /// Sets the Picker Pointer Size
        /// Value must be between 0-1
        /// Calculated against the View Canvas size
        /// </summary>
        public double PointerCircleDiameterUnits
        {
            get { return (double)GetValue(PointerCircleDiameterUnitsProperty); }
            set { SetValue(PointerCircleDiameterUnitsProperty, value); }
        }


        public static readonly BindableProperty PointerCircleBorderUnitsProperty
            = BindableProperty.Create(
                nameof(PointerCircleBorderUnits),
                typeof(double),
                typeof(ColorPicker),
                0.3,
                BindingMode.OneTime);

        /// <summary>
        /// Sets the Picker Pointer Border Size
        /// Value must be between 0-1
        /// Calculated against pixel size of Picker Pointer
        /// </summary>
        public double PointerCircleBorderUnits
        {
            get { return (double)GetValue(PointerCircleBorderUnitsProperty); }
            set { SetValue(PointerCircleBorderUnitsProperty, value); }
        }


        private SKPoint _lastTouchPoint = new SKPoint();

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
                var colors = GetSecondaryLayerColors();

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

                var pointerCircleDiameterUnits = PointerCircleDiameterUnits; // 0.6 (Default)
                pointerCircleDiameterUnits = (float)pointerCircleDiameterUnits / 10f; //  calculate 1/10th of that value
                var pointerCircleDiameter = (float)(valueToCalcAgainst * pointerCircleDiameterUnits);

                // Outer circle of the Pointer (Ring)
                skCanvas.DrawCircle(
                    _lastTouchPoint.X,
                    _lastTouchPoint.Y,
                    pointerCircleDiameter / 2, paintTouchPoint);

                // Draw another circle with picked color
                paintTouchPoint.Color = touchPointColor;

                var pointerCircleBorderWidthUnits = PointerCircleBorderUnits; // 0.3 (Default)
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

                // update the Canvas as you wish
                SkCanvasView.InvalidateSurface();
            }
        }

        private SKColor[] GetSecondaryLayerColors()
        {
            if (GradientColorStyle == GradientColorStyle.ColorsOnlyStyle)
            {
                return new SKColor[]
                {
                        SKColors.Transparent
                };
            }
            else if (GradientColorStyle == GradientColorStyle.ColorsToDarkStyle)
            {
                return new SKColor[]
                {
                        SKColors.Transparent,
                        SKColors.Black
                };
            }
            else if (GradientColorStyle == GradientColorStyle.DarkToColorsStyle)
            {
                return new SKColor[]
                {
                        SKColors.Black,
                        SKColors.Transparent
                };
            }
            else if (GradientColorStyle == GradientColorStyle.ColorsToLightStyle)
            {
                return new SKColor[]
                {
                        SKColors.Transparent,
                        SKColors.White
                };
            }
            else if (GradientColorStyle == GradientColorStyle.LightToColorsStyle)
            {
                return new SKColor[]
                {
                        SKColors.White,
                        SKColors.Transparent
                };
            }
            else if (GradientColorStyle == GradientColorStyle.LightToColorsToDarkStyle)
            {
                return new SKColor[]
                {
                        SKColors.White,
                        SKColors.Transparent,
                        SKColors.Black
                };
            }
            else if (GradientColorStyle == GradientColorStyle.DarkToColorsToLightStyle)
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
    }

    public enum GradientColorStyle
    {
        ColorsOnlyStyle,
        ColorsToDarkStyle,
        DarkToColorsStyle,
        ColorsToLightStyle,
        LightToColorsStyle,
        LightToColorsToDarkStyle,
        DarkToColorsToLightStyle
    }

    public enum ColorFlowDirection
    {
        Horizontal,
        Vertical
    }
}