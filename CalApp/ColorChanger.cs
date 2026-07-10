using System.Collections.Generic;
using System.Drawing;

namespace CalApp
{
    public class ColorPoint
    {
        public Point point;
        public Color color;
    }

    public class BitmapFunctions
    {
        public static Bitmap ChangeToColor(Bitmap bitmap, Color color)
        {
            var colorPoints = new List<ColorPoint>();

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    colorPoints.Add(new ColorPoint
                    {
                        point = new Point(j, i),
                        color = bitmap.GetPixel(j, i)
                    });
                }
            }

            foreach (ColorPoint n in colorPoints.FindAll(IsBlackPixel))
            {
                n.color = color;
                bitmap.SetPixel(n.point.X, n.point.Y, n.color);
            }

            return bitmap;
        }

        private static bool IsBlackPixel(ColorPoint n)
        {
            // FIX: was an exact match on "ff000000" (pure opaque black), which
            // left anti-aliased near-black edge pixels their original colour,
            // giving the recoloured icon a dark fringe. Treat any sufficiently
            // dark, mostly-opaque pixel as "black" so edges recolour too.
            Color c = n.color;
            return c.A > 128 && c.R < 64 && c.G < 64 && c.B < 64;
        }
    }
}