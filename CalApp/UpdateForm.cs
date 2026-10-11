using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalApp.Localization;
using CalApp.Updates;

namespace CalApp
{
    internal sealed class UpdateForm : Form
    {
        private readonly UpdateChecker checker;
        private readonly string currentVersion;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private readonly Label statusLabel = new Label { AutoSize = true, MaximumSize = new Size(430, 0), Margin = new Padding(0, 12, 0, 12) };
        private readonly Button checkButton = new Button { Name = "checkUpdatesMenu", AutoSize = true };
        private readonly Button downloadButton = new Button { Name = "openReleases", AutoSize = true };
        private Uri download = UpdateChecker.ReleasesPage;
        private bool checking;
        private bool disposed;

        internal UpdateForm(UpdateChecker checker = null, string currentVersion = null)
        {
            this.checker = checker ?? new UpdateChecker();
            this.currentVersion = currentVersion ?? Application.ProductVersion;
            Name = "UpdateForm";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = MinimizeBox = ShowInTaskbar = false;
            AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink; Padding = new Padding(12);
            var close = new Button { Name = "closeButton", AutoSize = true, DialogResult = DialogResult.Cancel };
            var buttons = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Dock = DockStyle.Fill, Margin = Padding.Empty };
            buttons.Controls.AddRange(new Control[] { checkButton, downloadButton, close });
            var layout = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty };
            layout.Controls.Add(new Label { AutoSize = true, Text = AppText.Format("CurrentVersion", this.currentVersion) });
            layout.Controls.Add(statusLabel);
            layout.Controls.Add(buttons);
            Controls.Add(layout);
            AppText.Translate(this);
            AcceptButton = CancelButton = close;
            close.Click += (sender, e) => Close();
            checkButton.Click += async (sender, e) => await CheckAsync();
            Shown += async (sender, e) => await CheckAsync();
            downloadButton.Click += OpenDownload;
        }

        internal async Task CheckAsync()
        {
            if (checking || disposed) return;
            checking = true; checkButton.Enabled = false;
            statusLabel.Text = AppText.Get("UpdateChecking");
            download = UpdateChecker.ReleasesPage;
            downloadButton.Text = AppText.Get("openReleases");
            try
            {
                if (!ReleaseVersion.TryParse(currentVersion, out ReleaseVersion current))
                    throw new UpdateLookupException("UpdateUnavailable");
                UpdateResult result = await checker.CheckAsync(current, cancellation.Token);
                if (disposed || cancellation.IsCancellationRequested) return;
                bool available = result.Version != null && result.Version.CompareTo(current) > 0;
                statusLabel.Text = available ? AppText.Format("UpdateFound", result.Version) : AppText.Get("UpdateCurrent");
                if (available) { download = result.Download; downloadButton.Text = AppText.Get("downloadUpdate"); }
            }
            catch (UpdateLookupException ex) { if (!disposed) statusLabel.Text = AppText.Get(ex.MessageKey); }
            catch (OperationCanceledException) { if (!disposed && !cancellation.IsCancellationRequested) statusLabel.Text = AppText.Get("UpdateTimeout"); }
            catch (Exception ex) when (ex is HttpRequestException || ex is SerializationException || ex is System.IO.IOException)
            { if (!disposed) statusLabel.Text = AppText.Get("UpdateUnavailable"); }
            finally { checking = false; if (!disposed) checkButton.Enabled = true; }
        }

        private void OpenDownload(object sender, EventArgs e)
        {
            try { Process.Start(new ProcessStartInfo(download.AbsoluteUri) { UseShellExecute = true }); }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is InvalidOperationException)
            { MessageBox.Show(this, AppText.Get("BrowserFailed"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                cancellation.Cancel();
                checker.Dispose();
                cancellation.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
