using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.DateFunctions;
using CalApp.Localization;

namespace CalApp
{
    // Owns every tray resource. No calendar window is needed to show or update icons.
    internal sealed class TrayIcons : IDisposable
    {
        private readonly ContextMenuStrip trayMenu = new ContextMenuStrip();
        private readonly PersistentTrayIcon standardTrayIcon;
        private readonly PersistentTrayIcon fiscalWeekTrayIcon;
        private readonly PersistentTrayIcon calendarWeekTrayIcon;
        private readonly Icon calendarIcon = (Icon)Resource.Calendar_Icon.Clone();
        private bool disposed;

        public event EventHandler CalendarToggleRequested;
        public event EventHandler CalendarShowRequested;
        public event EventHandler CalendarHideRequested;
        public event EventHandler PreferencesRequested;
        public event EventHandler ExitRequested;
        public event EventHandler UpdatesRequested;

        public TrayIcons(bool persistentIdentity = true)
        {
            standardTrayIcon = new PersistentTrayIcon(Identity(TrayIconIdentity.Standard, persistentIdentity));
            fiscalWeekTrayIcon = new PersistentTrayIcon(Identity(TrayIconIdentity.FiscalWeek, persistentIdentity));
            calendarWeekTrayIcon = new PersistentTrayIcon(Identity(TrayIconIdentity.CalendarWeek, persistentIdentity));
            trayMenu.Items.Add(new ToolStripMenuItem("", null, (sender, e) => ExitRequested?.Invoke(this, e)) { Name = "exitMenu" });
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("", null, (sender, e) => PreferencesRequested?.Invoke(this, e)) { Name = "preferencesMenu" });
            trayMenu.Items.Add(new ToolStripMenuItem("", null, (sender, e) => UpdatesRequested?.Invoke(this, e)) { Name = "checkUpdatesMenu" });
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("", null, (sender, e) => CalendarShowRequested?.Invoke(this, e)) { Name = "trayShow" });
            trayMenu.Items.Add(new ToolStripMenuItem("", null, (sender, e) => CalendarHideRequested?.Invoke(this, e)) { Name = "hideCalendarMenu" });
            AppText.Translate(trayMenu);
            foreach (PersistentTrayIcon tray in new[] { standardTrayIcon, fiscalWeekTrayIcon, calendarWeekTrayIcon })
            {
                tray.ContextMenuStrip = trayMenu;
                tray.MouseClick += Tray_MouseClick;
            }
        }

        private static Guid Identity(Guid identity, bool persistent) => persistent
            ? TrayIconIdentity.ForExecutable(identity, Application.ExecutablePath) : Guid.NewGuid();

        internal void RetryRegistration()
        {
            standardTrayIcon.RetryRegistration();
            fiscalWeekTrayIcon.RetryRegistration();
            calendarWeekTrayIcon.RetryRegistration();
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
            AppText.Translate(trayMenu);
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
            fiscalWeekTrayIcon.Text = AppText.Format("FiscalWeekTip", weeks.Fiscal.Week, weeks.Fiscal.YearLabel);
            calendarWeekTrayIcon.Text = AppText.Format("CalendarWeekTip", weeks.CalendarWeek);
            standardTrayIcon.Text = "WeekTrayInfo";
            standardTrayIcon.Visible = settings.StandardIcon;
            fiscalWeekTrayIcon.Visible = settings.FiscalWeekIcon;
            calendarWeekTrayIcon.Visible = settings.CalendarWeekIcon;
        }

        private static void ReplaceIcon(PersistentTrayIcon tray, Icon replacement)
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
            foreach (PersistentTrayIcon tray in new[] { standardTrayIcon, fiscalWeekTrayIcon, calendarWeekTrayIcon })
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
