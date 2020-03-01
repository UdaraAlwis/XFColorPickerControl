using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DemoApp.Helpers
{
    public static class ColorHelpers
    {
        public static bool IsColorDark(this Color color)
        {
            var hsp = Math.Sqrt(
                0.299 * (color.R * color.R) +
                0.587 * (color.G * color.G) +
                0.114 * (color.B * color.B)
                );

            return (hsp < 0.75);
        }
    }
}
