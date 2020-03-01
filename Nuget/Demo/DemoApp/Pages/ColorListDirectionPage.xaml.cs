
using DemoApp.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorListDirectionPage : ContentPage
    {
        public ColorListDirectionPage()
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

        private void SetHorizontalButton_Clicked(object sender, System.EventArgs e)
        {
            ColorPicker.ColorListDirection =
                Udara.Plugin.XFColorPickerControl.ColorListDirection.Horizontal;
        }

        private void SetVerticalButton_Clicked(object sender, System.EventArgs e)
        {
            ColorPicker.ColorListDirection =
                Udara.Plugin.XFColorPickerControl.ColorListDirection.Vertical;
        }

        private void GoBackButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}