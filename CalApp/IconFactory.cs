using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace CalApp
{
    internal static class IconFactory
    {
        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        public static Icon CreateText(string text, Color background, Color foreground)
        {
            const int size = 16;
            using (var bitmap = new Bitmap(size, size))
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Microsoft Sans Serif", 16, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(foreground))
            using (var format = new StringFormat
            {
                FormatFlags = StringFormatFlags.DirectionRightToLeft,
                LineAlignment = StringAlignment.Near
            })
            {
                graphics.Clear(background);
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.DrawString(text, font, brush, 20, -1, format);
                return FromBitmap(bitmap);
            }
        }

        public static Icon Recolor(Icon source, Color color)
        {
            using (Bitmap bitmap = source.ToBitmap())
            {
                for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.A > 0 && pixel.R < 64 && pixel.G < 64 && pixel.B < 64)
                        bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, color.R, color.G, color.B));
                }
                return FromBitmap(bitmap);
            }
        }

        private static Icon FromBitmap(Bitmap bitmap)
        {
            IntPtr handle = bitmap.GetHicon();
            try { return (Icon)Icon.FromHandle(handle).Clone(); }
            finally { DestroyIcon(handle); }
        }
    }
}
