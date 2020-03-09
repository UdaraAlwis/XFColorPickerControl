using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoApp.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PointerRingStylePage : ContentPage
    {
        public PointerRingStylePage()
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

        private void GoBackButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}