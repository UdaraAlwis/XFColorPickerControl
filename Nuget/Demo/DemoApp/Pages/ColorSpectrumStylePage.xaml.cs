using DemoApp.Helpers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Udara.Plugin.XFColorPickerControl;

namespace DemoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorSpectrumStylePage : ContentPage
    {
        public ColorSpectrumStylePage()
        {
            InitializeComponent();

            GradientColorStylePicker.ItemsSource =
                new List<ColorSpectrumStyle>()
                {
                    ColorSpectrumStyle.HueOnlyStyle,
                    ColorSpectrumStyle.HueToShadeStyle,
                    ColorSpectrumStyle.ShadeToHueStyle,
                    ColorSpectrumStyle.HueToTintStyle,
                    ColorSpectrumStyle.TintToHueStyle,
                    ColorSpectrumStyle.TintToHueToShadeStyle,
                    ColorSpectrumStyle.ShadeToHueToTintStyle,
                };
            GradientColorStylePicker.SelectedItem = ColorSpectrumStyle.TintToHueToShadeStyle;
        }

        private void ColorPicker_PickedColorChanged(object sender, Color colorPicked)
        {
            // Use the selected color
            SelectedColorDisplayFrame.BackgroundColor = colorPicked;
            SelectedColorValueLabel.Text = colorPicked.ToHex();

            if (colorPicked.IsColorDark())
                SelectedColorValueLabel.TextColor = Color.White;
            else
                SelectedColorValueLabel.TextColor = Color.SlateGray;
        }
        
        private void GradientColorStylePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GradientColorStylePicker.SelectedItem != null)
                ColorPicker.ColorSpectrumStyle = (ColorSpectrumStyle)GradientColorStylePicker.SelectedItem;
        }

        private void GoBackButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}