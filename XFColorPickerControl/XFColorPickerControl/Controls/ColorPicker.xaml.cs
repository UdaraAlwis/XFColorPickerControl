﻿using System;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace XFColorPickerControl.Controls
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
            set 
            {
                if (!PickedColor.Equals(value))
                {
                    SetValue(PickedColorProperty, value);
                    _lastTouchPoint = new SKPoint() { X = int.MaxValue, Y = int.MaxValue };
                    SkCanvasView.InvalidateSurface();
                    PickedColorChanged?.Invoke(this, PickedColor);
                }
            }
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

        public static readonly BindableProperty ColorListProperty
            = BindableProperty.Create(
                nameof(ColorList),
                typeof(string[]),
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
                BindingMode.OneTime, null);

        /// <summary>
        /// Sets the Color List
        /// </summary>
        public string[] ColorList
        {
            get { return (string[])GetValue(ColorListProperty); }
            set { SetValue(ColorListProperty, value); }
        }

        public static readonly BindableProperty ColorListDirectionProperty
            = BindableProperty.Create(
                nameof(ColorListDirection),
                typeof(ColorListDirection),
                typeof(ColorPicker),
                ColorListDirection.Horizontal,
                BindingMode.OneTime);

        /// <summary>
        /// Sets the Color List flow Direction
        /// </summary>
        public ColorListDirection ColorListDirection
        {
            get { return (ColorListDirection)GetValue(ColorListDirectionProperty); }
            set { SetValue(ColorListDirectionProperty, value); }
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

        private SKPoint _lastTouchPoint = new SKPoint() { X = int.MaxValue, Y = int.MaxValue};

        public ColorPicker()
        {
            InitializeComponent();
        }

        /// <summary>
        /// I found this algorythm at https://stackoverflow.com/a/33782458/648919 
        /// It's simple enough but really effective!
        /// </summary>
        private double ColorDistance(SKColor c1, SKColor c2)
        {
            int rmean = (c1.Red + c2.Red) / 2;
            int r = c1.Red - c2.Red;
            int g = c1.Green - c2.Green;
            int b = c1.Blue - c2.Blue;
            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
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

                System.Collections.Generic.List<SKColor> colors = new System.Collections.Generic.List<SKColor>();
                ColorList.ForEach((color) => { colors.Add(Color.FromHex(color).ToSKColor()); });

                // create the gradient shader between Colors
                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    ColorListDirection == ColorListDirection.Horizontal ?
                        new SKPoint(skCanvasWidth, 0) : new SKPoint(0, skCanvasHeight),
                    colors.ToArray(),
                    null,
                    SKShaderTileMode.Clamp))
                {
                    paint.Shader = shader;
                    skCanvas.DrawPaint(paint);
                }
            }

            // Draw darker gradient spectrum
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                // Initiate the darkened primary color list
                var colors = GetGradientOrder();

                // create the gradient shader 
                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    ColorListDirection == ColorListDirection.Horizontal ?
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
            SKColor touchPointColor = SKColors.White;

            // Efficient and fast
            // https://forums.xamarin.com/discussion/92899/read-a-pixel-info-from-a-canvas
            // create the 1x1 bitmap (auto allocates the pixel buffer)
            using (SKBitmap bitmap = new SKBitmap(skImageInfo))
            {
                // get the pixel buffer for the bitmap
                IntPtr dstpixels = bitmap.GetPixels();

                // If color isn't set by touch, we need to find a color location on bitmap
                if (_lastTouchPoint.X.Equals(int.MaxValue) && _lastTouchPoint.Y.Equals(int.MaxValue))
                {
                    // read the surface into the bitmap
                    var res = skSurface.ReadPixels(skImageInfo, dstpixels, skImageInfo.RowBytes, 0, 0);
                    touchPointColor = PickedColor.ToSKColor();
                    var bpp = bitmap.BytesPerPixel;

                    // Get rid of SKBitmap getters
                    var pixels = new byte[bitmap.ByteCount];
                    bitmap.Bytes.CopyTo(pixels, 0);

                    // Let's try to find our color coordinates
                    var exactMatch = false;
                    var minDistance = double.MaxValue;

                    for (int y = 0; y < skCanvasHeight; y++)
                    {
                        for (int x = 0; x < skCanvasWidth; x++)
                        {
                            var c = new SKColor(pixels[(y * skCanvasWidth + x) * bpp + 2],
                                                pixels[(y * skCanvasWidth + x) * bpp + 1],
                                                pixels[(y * skCanvasWidth + x) * bpp + 0]);

                            // Check for exact color mach first
                            if (c.Equals(touchPointColor))
                            {
                                _lastTouchPoint.X = x;
                                _lastTouchPoint.Y = y;
                                exactMatch = true;
                                break;
                            }
                            else
                            {
                                var dist = ColorDistance(touchPointColor, c);
                                if (dist < 5)
                                {
                                    _lastTouchPoint.X = x;
                                    _lastTouchPoint.Y = y;
                                    exactMatch = true;
                                    break;
                                }
                                else if (minDistance > dist)
                                {
                                    minDistance = dist;
                                    _lastTouchPoint.X = x;
                                    _lastTouchPoint.Y = y;

#if false
                                    // Small optimization based on RL tests; btw, it works 
                                    // fast enough even without these lines                                    if (minDistance < 15)
                                    {
                                        exactMatch = true;
                                        break;
                                    }
#endif
                                }
                            }
                        }

                        if (exactMatch) break;
                    }
                }

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
            SetValue(PickedColorProperty, touchPointColor.ToFormsColor());
        }

        private void SkCanvasView_OnTouch(object sender, SKTouchEventArgs e)
        {
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
                PickedColorChanged?.Invoke(this, PickedColor);
            }
        }

        private SKColor[] GetGradientOrder()
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

    public enum ColorListDirection
    {
        Horizontal,
        Vertical
    }
}