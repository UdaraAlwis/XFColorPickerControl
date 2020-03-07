using DemoApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseColorListPage : ContentPage
    {
        public BaseColorListPage()
        {
            InitializeComponent();
        }

        private void ColorPicker_PickedColorChanged(object sender, Color colorPicked)
        {
            // Use the selected color
            SelectedColorDisplayFrame.BackgroundColor = colorPicked;
            SelectedColorValueLabel.Text = colorPicked.ToHex();

            if (colorPicked.IsColorDark())
                SelectedColorValueLabel.TextColor = Xamarin.Forms.Color.White;
            else
                SelectedColorValueLabel.TextColor = Xamarin.Forms.Color.SlateGray;
        }

        private void SetColorSetDefault_Clicked(object sender, EventArgs e)
        {
            ColorPicker.BaseColorList = new List<string>()
            {                    
                new Color(255, 0, 0).ToHex(), // Red
				new Color(255, 255, 0).ToHex(), // Yellow
				new Color(0, 255, 0).ToHex(), // Green (Lime)
				new Color(0, 255, 255).ToHex(), // Aqua
				new Color(0, 0, 255).ToHex(), // Blue
				new Color(255, 0, 255).ToHex(), // Fuchsia
				new Color(255, 0, 0).ToHex(), // Red
            };
        }

        private void SetColorSet1Button_Clicked(object sender, EventArgs e)
        {
            ColorPicker.BaseColorList = new List<string>()
            {
                "#ffff00",
                "#ff8000",
                "#ff0000",
                "#ff00ff",
                "#ff0080",
            };
        }

        private void SetColorSet2Button_Clicked(object sender, EventArgs e)
        {
            ColorPicker.BaseColorList = new List<string>()
            {
                new Color(255, 255, 0).ToHex(), // Yellow
				new Color(0, 255, 255).ToHex(), // Aqua
				new Color(255, 0, 255).ToHex(), // Fuchsia
				new Color(255, 255, 0).ToHex(), // Yellow
            };
        }
        
        private void SetColorSet3Button_Clicked(object sender, EventArgs e)
        {
            ColorPicker.BaseColorList = new List<string>()
            {
                "#00bfff",
                "#0040ff",
                "#8000ff",
                "#ff00ff",
                "#ff0000",
            };
        }

        private void GoBackButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}