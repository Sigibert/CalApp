using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using WeekTrayInfo.Localization;

namespace WeekTrayInfo
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            AppText.Translate(this);
            var copyright = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>();
            copyrightLabel.Text = copyright?.Copyright ?? "";
            versionLabel.Text = AppText.Format("Version", Application.ProductVersion);
            applicationInfoGroup.Text = Application.ProductName;
            applicationInfoText.Text = AppText.Get("ApplicationInfo");
            licenseText.Text = Resource.Licence;
        }

        private void ContactLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("mailto:christophzieglerapps@outlook.com?subject=WeekTrayInfo")
                { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is InvalidOperationException)
            {
                MessageBox.Show(this, AppText.Get("EmailUnavailable"),
                    AppText.Get("contactLink"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) => Close();
        private void AboutForm_Load(object sender, EventArgs e) => licenseText.DeselectAll();
        private void AboutForm_Shown(object sender, EventArgs e)
        {
            closeButton.Left = (ClientSize.Width - closeButton.Width) / 2;
            applicationInfoText.DeselectAll();
            licenseText.DeselectAll();
        }
    }
}
