using System;
using System.Drawing;

namespace CalApp.Config
{
    public static class IconColors
    {
        public static Color GetTextColor(Color background)
        {
            double luminance = 0.2126 * Linear(background.R)
                + 0.7152 * Linear(background.G) + 0.0722 * Linear(background.B);
            double blackContrast = (luminance + 0.05) / 0.05;
            double whiteContrast = 1.05 / (luminance + 0.05);
            return blackContrast >= whiteContrast ? Color.Black : Color.White;
        }

        private static double Linear(byte channel)
        {
            double value = channel / 255.0;
            return value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }
    }
}
