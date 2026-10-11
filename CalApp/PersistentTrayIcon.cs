using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CalApp
{
    // NotifyIcon in .NET Framework cannot specify NIF_GUID. Keep the small native
    // boundary here so Explorer can recognize each icon after an application update.
    internal sealed class PersistentTrayIcon : NativeWindow, IDisposable
    {
        private const int CallbackMessage = 0x8001;
        private const uint GuidFlag = 0x20;
        private static readonly int TaskbarCreated = RegisterWindowMessage("TaskbarCreated");
        private bool registered;
        private bool visible;
        private bool disposed;
        private bool version4;
        private Icon icon;
        private string text = "";

        internal Guid Identity { get; }
        public ContextMenuStrip ContextMenuStrip { get; set; }
        public event MouseEventHandler MouseClick;

        internal PersistentTrayIcon(Guid identity)
        {
            Identity = identity;
            CreateHandle(new CreateParams { Caption = "CalApp.Tray", Style = 0 });
        }

        public Icon Icon
        {
            get => icon;
            set { CheckDisposed(); icon = value; Refresh(); }
        }

        public string Text
        {
            get => text;
            set
            {
                CheckDisposed();
                if (value != null && value.Length > 127) throw new ArgumentException("Tray text is too long.", nameof(value));
                text = value ?? "";
                Refresh();
            }
        }

        public bool Visible
        {
            get => visible;
            set { CheckDisposed(); visible = value; Refresh(); }
        }

        internal bool Registered => registered;

        internal void RetryRegistration()
        {
            CheckDisposed();
            if (visible && !registered) Refresh();
        }

        private NotifyIconData Data(uint flags = GuidFlag) => new NotifyIconData
        {
            Size = (uint)Marshal.SizeOf(typeof(NotifyIconData)),
            Window = Handle, Id = 1, Flags = flags, Callback = CallbackMessage,
            Icon = icon?.Handle ?? IntPtr.Zero, Tip = text, Identity = Identity,
            Info = "", InfoTitle = ""
        };

        private void Refresh()
        {
            if (!visible || icon == null)
            {
                if (registered)
                {
                    var data = Data();
                    ShellNotifyIcon(2, ref data);
                    registered = false;
                }
                return;
            }
            var update = Data(GuidFlag | 1 | 2 | 4 | 0x80);
            if (registered)
            {
                if (ShellNotifyIcon(1, ref update)) return;
                registered = false;
            }
            // Explorer may be temporarily unavailable during restart. Retry on the next
            // refresh instead of crashing the app or silently dropping the icon forever.
            registered = ShellNotifyIcon(0, ref update);
            if (registered)
            {
                update.Version = 4;
                version4 = ShellNotifyIcon(4, ref update);
            }
        }

        internal void RestoreAfterExplorerRestart()
        {
            CheckDisposed();
            registered = false;
            Refresh();
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == TaskbarCreated && !disposed) RestoreAfterExplorerRestart();
            else if (message.Msg == CallbackMessage && !disposed)
            {
                int notification = (int)(message.LParam.ToInt64() & 0xffff);
                if ((version4 && (notification == 0x400 || notification == 0x401)) ||
                    (!version4 && notification == 0x202))
                    MouseClick?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                else if ((version4 && notification == 0x7b) || (!version4 && notification == 0x205))
                {
                    Point position = Cursor.Position;
                    if (version4)
                    {
                        long coordinates = message.WParam.ToInt64();
                        position = new Point((short)(coordinates & 0xffff), (short)((coordinates >> 16) & 0xffff));
                        if (position == new Point(-1, -1)) position = Cursor.Position;
                    }
                    // A foreground owner is required for menus to dismiss on an outside click.
                    SetForegroundWindow(Handle);
                    ContextMenuStrip?.Show(position);
                }
            }
            base.WndProc(ref message);
        }

        private void CheckDisposed()
        {
            if (disposed) throw new ObjectDisposedException(nameof(PersistentTrayIcon));
        }

        public void Dispose()
        {
            if (disposed) return;
            visible = false;
            Refresh();
            disposed = true;
            icon = null; // The owning TrayIcons object disposes the managed icons.
            ContextMenuStrip = null;
            MouseClick = null;
            DestroyHandle();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NotifyIconData
        {
            internal uint Size;
            internal IntPtr Window;
            internal uint Id, Flags, Callback;
            internal IntPtr Icon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] internal string Tip;
            internal uint State, StateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] internal string Info;
            internal uint Version;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] internal string InfoTitle;
            internal uint InfoFlags;
            internal Guid Identity;
            internal IntPtr BalloonIcon;
        }

        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShellNotifyIcon(uint operation, ref NotifyIconData data);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int RegisterWindowMessage(string message);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr window);
    }
}
