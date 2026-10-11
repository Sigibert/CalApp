using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace CalApp
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            authorLabel.Text = "Author: Christoph Ziegler";
            var copyright = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>();
            copyrightLabel.Text = copyright?.Copyright ?? "";
            versionLabel.Text = "Version: " + Application.ProductVersion;
            contactLink.Text = "Contact";
            applicationInfoGroup.Text = Application.ProductName;
            applicationInfoText.Text = Resource.Information;
            licenseGroup.Text = "License Information";
            licenseText.Text = Resource.Licence;
        }

        private void ContactLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("mailto:christophzieglerapps@outlook.com?subject=Calendar%20Week%20Info")
                { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is InvalidOperationException)
            {
                MessageBox.Show(this, "The email application could not be opened.\nContact: christophzieglerapps@outlook.com",
                    "Contact", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
