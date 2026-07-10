using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace CalApp
{
    class IconFunctions
    {
        // Must be called to release the raw HICON handle after we've
        // cloned it into a managed Icon object.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public static Icon CreateTextIcon(string twoDigits, Color iconBackColor, Color iconForeColor)
        {
            // Every GDI-backed object is in a using block so it's
            // disposed deterministically, not left to the finalizer.
            using (var bitmap = new Bitmap(16, 16))
            using (var g = Graphics.FromImage(bitmap))
            using (var iconFont = new Font("Microsoft Sans Serif", 16, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var iconBrush = new SolidBrush(iconForeColor))
            {
                var drawFormat = new StringFormat
                {
                    FormatFlags = StringFormatFlags.DirectionRightToLeft,
                    LineAlignment = StringAlignment.Near
                };

                g.Clear(iconBackColor);
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.DrawString(twoDigits, iconFont, iconBrush, 20, -1, drawFormat);

                // GetHicon() allocates a raw Win32 HICON handle.
                // Icon.FromHandle wraps it but does NOT take ownership —
                // if we just return that icon, the handle leaks.
                // Clone() creates a fully managed Icon that owns its own copy,
                // then we explicitly release the original raw handle.
                IntPtr hIcon = bitmap.GetHicon();
                try
                {
                    return (Icon)Icon.FromHandle(hIcon).Clone();
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
        }

        public static Icon ChangeToColor(Icon icon, Color color)
        {
            // ToBitmap() allocates a new Bitmap — needs disposing.
            using (Bitmap bitmap = BitmapFunctions.ChangeToColor(icon.ToBitmap(), color))
            {
                IntPtr hIcon = bitmap.GetHicon();
                try
                {
                    return (Icon)Icon.FromHandle(hIcon).Clone();
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
        }
    }
}