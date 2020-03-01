using DemoApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DemoApp.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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

        private void GoToColorListDirectionDemoPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ColorListDirectionPage());
        }

        private void GoToGradientColorStyleDemoPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new GradientColorStylePage());
        }

        private void GoToPointerCircleDemoPage_Clicked(object sender, EventArgs e)
        {

        }

        private void GoToColorListDemoPage_Clicked(object sender, EventArgs e)
        {

        }
    }
}
