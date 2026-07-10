using System;
using System.Drawing;

namespace CalApp.Config
{
    public static class HSVColorConverter
    {
        public static double[] RGB2HSV(Color color)
        {
            if (color == Color.Empty) throw new ArgumentException();

            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h = 0;
            // FIX: added max != min guard to prevent division by zero
            // for achromatic colours (black, white, greys).
            if (max != min)
            {
                if (max == r)
                {
                    h = g > b
                        ? 60 * (g - b) / (max - min)
                        : 60 * (g - b) / (max - min) + 360;
                }
                else if (max == g)
                    h = 60 * (b - r) / (max - min) + 120;
                else if (max == b)
                    h = 60 * (r - g) / (max - min) + 240;
            }

            double s = (max == 0) ? 0 : 1 - min / max;

            return new double[] { h, s, max };
        }

        public static Color HSV2RGB(double[] hsv)
        {
            if (hsv == null) throw new ArgumentNullException();
            if (hsv.Length != 3) throw new ArgumentException();
            if (hsv[0] < 0 || hsv[0] > 360) throw new ArgumentException();
            if (hsv[1] < 0 || hsv[1] > 1) throw new ArgumentException();
            if (hsv[2] < 0 || hsv[2] > 1) throw new ArgumentException();

            double[] rgb;

            if (hsv[1] == 0)
            {
                rgb = new double[] { hsv[2], hsv[2], hsv[2] };
            }
            else
            {
                double sectorPos = hsv[0] / 60d;
                int sectorNumber = (int)Math.Floor(sectorPos);
                double fractionalSector = sectorPos - sectorNumber;

                double p = hsv[2] * (1 - hsv[1]);
                double q = hsv[2] * (1 - hsv[1] * fractionalSector);
                double t = hsv[2] * (1 - hsv[1] * (1 - fractionalSector));

                switch (sectorNumber)
                {
                    case 0: rgb = new double[] { hsv[2], t, p }; break;
                    case 1: rgb = new double[] { q, hsv[2], p }; break;
                    case 2: rgb = new double[] { p, hsv[2], t }; break;
                    case 3: rgb = new double[] { p, q, hsv[2] }; break;
                    case 4: rgb = new double[] { t, p, hsv[2] }; break;
                    default: rgb = new double[] { hsv[2], p, q }; break;
                }
            }

            for (int i = 0; i < rgb.Length; i++)
                rgb[i] *= 255;

            return Color.FromArgb((int)rgb[0], (int)rgb[1], (int)rgb[2]);
        }

        public static Color InvertColor(Color color)
        {
            double[] tempColor = RGB2HSV(color);
            tempColor[0] = (tempColor[0] > 180) ? tempColor[0] - 180 : tempColor[0] + 180;
            return HSV2RGB(tempColor);
        }

        public static Color Complement1Color(Color color)
        {
            return Color.FromArgb(255, 255 - color.R, 255 - color.G, 255 - color.B);
        }

        // FIX: removed Console.WriteLine debug calls that were left in production code.
        // The swapped saturation/value logic is preserved as-is (intentional complement).
        public static Color Complement2Color(Color color)
        {
            double[] tempColor = RGB2HSV(color);
            double originalSaturation = tempColor[1];

            tempColor[0] = (tempColor[0] > 210) ? tempColor[0] - 210 : tempColor[0] + 150;
            tempColor[1] = tempColor[2];
            tempColor[2] = 1 - originalSaturation;

            return HSV2RGB(tempColor);
        }
    }
}