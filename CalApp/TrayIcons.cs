using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.DateFunctions;

namespace CalApp
{
    // Owns every tray resource. No calendar window is needed to show or update icons.
    internal sealed class TrayIcons : IDisposable
    {
        private readonly ContextMenuStrip trayMenu = new ContextMenuStrip();
        private readonly NotifyIcon standardTrayIcon = new NotifyIcon();
        private readonly NotifyIcon fiscalWeekTrayIcon = new NotifyIcon();
        private readonly NotifyIcon calendarWeekTrayIcon = new NotifyIcon();
        private readonly Icon calendarIcon = (Icon)Resource.Calendar_Icon.Clone();
        private bool disposed;

        public event EventHandler CalendarToggleRequested;
        public event EventHandler CalendarShowRequested;
        public event EventHandler CalendarHideRequested;
        public event EventHandler PreferencesRequested;
        public event EventHandler ExitRequested;

        public TrayIcons()
        {
            trayMenu.Items.Add(new ToolStripMenuItem("Exit", null, (sender, e) => ExitRequested?.Invoke(this, e)));
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Preferences", null, (sender, e) => PreferencesRequested?.Invoke(this, e)));
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Show", null, (sender, e) => CalendarShowRequested?.Invoke(this, e)));
            trayMenu.Items.Add(new ToolStripMenuItem("Close", null, (sender, e) => CalendarHideRequested?.Invoke(this, e)));
            foreach (NotifyIcon tray in new[] { standardTrayIcon, fiscalWeekTrayIcon, calendarWeekTrayIcon })
            {
                tray.ContextMenuStrip = trayMenu;
                tray.MouseClick += Tray_MouseClick;
            }
        }

        private void Tray_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) CalendarToggleRequested?.Invoke(this, e);
        }

        public void Update(AppSettings settings, WeekInfo weeks)
        {
            if (disposed) throw new ObjectDisposedException(nameof(TrayIcons));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (weeks == null) throw new ArgumentNullException(nameof(weeks));
            Color fiscalText = settings.FiscalWeekIconForeColorAuto
                ? IconColors.GetTextColor(settings.FiscalWeekIconBackColor) : settings.FiscalWeekIconForeColor;
            Color calendarText = settings.CalendarWeekIconForeColorAuto
                ? IconColors.GetTextColor(settings.CalendarWeekIconBackColor) : settings.CalendarWeekIconForeColor;
            if (settings.FiscalWeekIcon)
                ReplaceIcon(fiscalWeekTrayIcon, IconFactory.CreateText(weeks.Fiscal.Week.ToString(CultureInfo.InvariantCulture),
                    settings.FiscalWeekIconBackColor, fiscalText));
            if (settings.CalendarWeekIcon)
                ReplaceIcon(calendarWeekTrayIcon, IconFactory.CreateText(weeks.CalendarWeek.ToString(CultureInfo.InvariantCulture),
                    settings.CalendarWeekIconBackColor, calendarText));
            if (settings.StandardIcon) ReplaceIcon(standardTrayIcon, IconFactory.Recolor(calendarIcon, settings.StandardIconColor));
            fiscalWeekTrayIcon.Text = "Fiscal week " + weeks.Fiscal.Week + " (" + weeks.Fiscal.YearLabel + ")";
            calendarWeekTrayIcon.Text = "Calendar week " + weeks.CalendarWeek;
            standardTrayIcon.Text = "Calendar Week Info";
            standardTrayIcon.Visible = settings.StandardIcon;
            fiscalWeekTrayIcon.Visible = settings.FiscalWeekIcon;
            calendarWeekTrayIcon.Visible = settings.CalendarWeekIcon;
        }

        private static void ReplaceIcon(NotifyIcon tray, Icon replacement)
        {
            Icon previous = tray.Icon;
            try { tray.Icon = replacement; }
            catch { replacement.Dispose(); throw; }
            previous?.Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            foreach (NotifyIcon tray in new[] { standardTrayIcon, fiscalWeekTrayIcon, calendarWeekTrayIcon })
            {
                tray.Visible = false;
                tray.MouseClick -= Tray_MouseClick;
                tray.ContextMenuStrip = null;
                Icon previous = tray.Icon;
                tray.Icon = null;
                previous?.Dispose();
                tray.Dispose();
            }
            trayMenu.Dispose();
            calendarIcon.Dispose();
        }
    }
}
