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

        private void GoToBaseColorListDemoPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new BaseColorListPage());
        }

        private void GoToColorFlowDirectionDemoPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ColorFlowDirectionPage());
        }

        private void GoToColorSpectrumStyleDemoPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ColorSpectrumStylePage());
        }

        private void GoToPointerRingStyle_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PointerRingStylePage());
        }
    }
}
