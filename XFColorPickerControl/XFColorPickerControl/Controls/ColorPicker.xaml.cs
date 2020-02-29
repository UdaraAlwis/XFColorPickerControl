using System;
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
			private set { SetValue(PickedColorProperty, value); }
		}

		public static readonly BindableProperty GradientOrderProperty
			= BindableProperty.Create(
				nameof(GradientOrder),
				typeof(GradientOrder),
				typeof(ColorPicker),
				GradientOrder.ColorsToDark, 
				BindingMode.OneTime, null);

		/// <summary>
		/// Set the Color Spectrum Gradient Order
		/// </summary>
		public GradientOrder GradientOrder
		{
			get { return (GradientOrder)GetValue(GradientOrderProperty); }
			set { SetValue(GradientOrderProperty, value); }
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

				// Outer circle (Ring)
				var outerRingRadius =
					((float)skCanvasWidth / (float)skCanvasHeight) * (float)18;
				skCanvas.DrawCircle(
					_lastTouchPoint.X,
					_lastTouchPoint.Y,
					outerRingRadius, paintTouchPoint);

				// Draw another circle with picked color
				paintTouchPoint.Color = touchPointColor;

				// Outer circle (Ring)
				var innerRingRadius =
					((float)skCanvasWidth / (float)skCanvasHeight) * (float)12;
				skCanvas.DrawCircle(
					_lastTouchPoint.X,
					_lastTouchPoint.Y,
					innerRingRadius, paintTouchPoint);
			}

			// Set selected color
			PickedColor = touchPointColor.ToFormsColor();
			PickedColorChanged?.Invoke(this, PickedColor);
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
			}
		}

		private SKColor[] GetGradientOrder() 
		{
			if (GradientOrder == GradientOrder.Colors)
			{
				return new SKColor[]
				{
						SKColors.Transparent
				};
			}
			else if (GradientOrder == GradientOrder.ColorsToDark)
			{
				return new SKColor[]
				{
						SKColors.Transparent,
						SKColors.Black
				};
			}
			else if (GradientOrder == GradientOrder.DarkToColors)
			{
				return new SKColor[]
				{
						SKColors.Black,
						SKColors.Transparent
				};
			}
			else if (GradientOrder == GradientOrder.ColorsToLight)
			{
				return new SKColor[]
				{
						SKColors.Transparent,
						SKColors.White
				};
			}
			else if (GradientOrder == GradientOrder.LightToColors)
			{
				return new SKColor[]
				{
						SKColors.White,
						SKColors.Transparent
				};
			}
			else if (GradientOrder == GradientOrder.LightToColorsToDark)
			{
				return new SKColor[]
				{
						SKColors.White,
						SKColors.Transparent,
						SKColors.Black
				};
			}
			else if (GradientOrder == GradientOrder.DarkToColorsToLight)
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

	public enum GradientOrder
	{
		Colors,
		ColorsToDark,
		DarkToColors,
		ColorsToLight,
		LightToColors,
		LightToColorsToDark,
		DarkToColorsToLight
	}

	public enum ColorListDirection 
	{
		Horizontal,
		Vertical
	}
}