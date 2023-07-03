using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using XFColorPickerControl.Controls;

namespace XFColorPickerControl
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public class ColorItem
        {
            public string Name { get; set; }
            public Color Value { get; set; }
        }
        public MainPage()
        {
            InitializeComponent();

            var allColors = typeof(Color)
                .GetRuntimeFields()
                .Where(f => f.FieldType == typeof(Color))
                .Select(prop => new ColorItem 
                { 
                    Name = prop.Name,
                    Value = (Color)prop.GetValue(null)
                })
                .ToList();

            SystemColorPicker.ItemsSource = allColors;
            SystemColorPicker.SelectedIndex = 0;
        }

		private void ColorPicker_PickedColorChanged(object sender, Color colorPicked)
		{
			// Use the selected color
			SelectedColorDisplayFrame.BackgroundColor = colorPicked;
			SelectedColorValueLabel.Text = colorPicked.ToHex();
			ColorPickerHolderFrame.BackgroundColor = colorPicked;

			if (colorPicked.Luminosity < 0.5)
				SelectedColorValueLabel.TextColor = Color.White;
			else
				SelectedColorValueLabel.TextColor = Color.SlateGray;
		}

        private void TestColorPicker_PickedColorChanged(object sender, Color e)
        {
            ColorPicker.PickedColor = e;
        }

        private void SystemColorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var color = (SystemColorPicker.SelectedItem as ColorItem).Value;

            ColorPicker.PickedColor = color;

            // Use the selected color
            SystemSelectedColorDisplayFrame.BackgroundColor = color;
            SystemSelectedColorValueLabel.Text = color.ToHex();

            if (color.Luminosity < 0.5)
                SystemSelectedColorValueLabel.TextColor = Color.White;
            else
                SystemSelectedColorValueLabel.TextColor = Color.SlateGray;
        }
    }
}
