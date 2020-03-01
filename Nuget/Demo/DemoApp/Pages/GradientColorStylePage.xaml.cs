using DemoApp.Helpers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Udara.Plugin.XFColorPickerControl;

namespace DemoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradientColorStylePage : ContentPage
    {
        public GradientColorStylePage()
        {
            InitializeComponent();

            GradientColorStylePicker.ItemsSource =
                new List<GradientColorStyle>()
                {
                    GradientColorStyle.ColorsOnlyStyle,
                    GradientColorStyle.ColorsToDarkStyle,
                    GradientColorStyle.DarkToColorsStyle,
                    GradientColorStyle.ColorsToLightStyle,
                    GradientColorStyle.LightToColorsStyle,
                    GradientColorStyle.LightToColorsToDarkStyle,
                    GradientColorStyle.DarkToColorsToLightStyle,
                };
            GradientColorStylePicker.SelectedItem = GradientColorStyle.LightToColorsToDarkStyle;
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
                ColorPicker.GradientColorStyle = (GradientColorStyle)GradientColorStylePicker.SelectedItem;
        }

        private void GoBackButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}