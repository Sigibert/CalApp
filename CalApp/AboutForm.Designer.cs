namespace CalApp
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.licenseGroup = new System.Windows.Forms.GroupBox();
            this.licenseText = new System.Windows.Forms.TextBox();
            this.authorLabel = new System.Windows.Forms.Label();
            this.contactLink = new System.Windows.Forms.LinkLabel();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.applicationInfoGroup = new System.Windows.Forms.GroupBox();
            this.applicationInfoText = new System.Windows.Forms.TextBox();
            this.licenseGroup.SuspendLayout();
            this.applicationInfoGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // licenseGroup
            // 
            this.licenseGroup.Controls.Add(this.licenseText);
            this.licenseGroup.Location = new System.Drawing.Point(9, 201);
            this.licenseGroup.Name = "licenseGroup";
            this.licenseGroup.Size = new System.Drawing.Size(300, 106);
            this.licenseGroup.TabIndex = 0;
            this.licenseGroup.TabStop = false;
            this.licenseGroup.Text = "Licence Information";
            // 
            // licenseText
            // 
            this.licenseText.BackColor = System.Drawing.SystemColors.Window;
            this.licenseText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.licenseText.Location = new System.Drawing.Point(7, 20);
            this.licenseText.Multiline = true;
            this.licenseText.Name = "licenseText";
            this.licenseText.ReadOnly = true;
            this.licenseText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.licenseText.Size = new System.Drawing.Size(287, 80);
            this.licenseText.TabIndex = 0;
            this.licenseText.Text = "Licence Infos";
            // 
            // authorLabel
            // 
            this.authorLabel.AutoSize = true;
            this.authorLabel.Location = new System.Drawing.Point(9, 126);
            this.authorLabel.Margin = new System.Windows.Forms.Padding(3);
            this.authorLabel.Name = "authorLabel";
            this.authorLabel.Padding = new System.Windows.Forms.Padding(3);
            this.authorLabel.Size = new System.Drawing.Size(50, 19);
            this.authorLabel.TabIndex = 2;
            this.authorLabel.Text = "Author: ";
            // 
            // contactLink
            // 
            this.contactLink.AutoSize = true;
            this.contactLink.Location = new System.Drawing.Point(9, 176);
            this.contactLink.Margin = new System.Windows.Forms.Padding(3);
            this.contactLink.Name = "contactLink";
            this.contactLink.Padding = new System.Windows.Forms.Padding(3);
            this.contactLink.Size = new System.Drawing.Size(50, 19);
            this.contactLink.TabIndex = 3;
            this.contactLink.TabStop = true;
            this.contactLink.Text = "Contact";
            this.contactLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ContactLink_LinkClicked);
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Location = new System.Drawing.Point(9, 151);
            this.copyrightLabel.Margin = new System.Windows.Forms.Padding(3);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Padding = new System.Windows.Forms.Padding(3);
            this.copyrightLabel.Size = new System.Drawing.Size(57, 19);
            this.copyrightLabel.TabIndex = 4;
            this.copyrightLabel.Text = "Copyright";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.versionLabel.Location = new System.Drawing.Point(9, 101);
            this.versionLabel.Margin = new System.Windows.Forms.Padding(3);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Padding = new System.Windows.Forms.Padding(3);
            this.versionLabel.Size = new System.Drawing.Size(48, 19);
            this.versionLabel.TabIndex = 5;
            this.versionLabel.Text = "Version";
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(9, 313);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Ok";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // spacerLabel
            // 
            this.spacerLabel.AutoSize = true;
            this.spacerLabel.Location = new System.Drawing.Point(9, 34);
            this.spacerLabel.Margin = new System.Windows.Forms.Padding(3);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Padding = new System.Windows.Forms.Padding(3);
            this.spacerLabel.Size = new System.Drawing.Size(6, 19);
            this.spacerLabel.TabIndex = 7;
            // 
            // applicationInfoGroup
            // 
            this.applicationInfoGroup.Controls.Add(this.applicationInfoText);
            this.applicationInfoGroup.Location = new System.Drawing.Point(9, 9);
            this.applicationInfoGroup.Name = "applicationInfoGroup";
            this.applicationInfoGroup.Size = new System.Drawing.Size(300, 86);
            this.applicationInfoGroup.TabIndex = 1;
            this.applicationInfoGroup.TabStop = false;
            this.applicationInfoGroup.Text = "Calendar Week Info";
            // 
            // applicationInfoText
            // 
            this.applicationInfoText.BackColor = System.Drawing.SystemColors.Window;
            this.applicationInfoText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.applicationInfoText.Location = new System.Drawing.Point(7, 20);
            this.applicationInfoText.Multiline = true;
            this.applicationInfoText.Name = "applicationInfoText";
            this.applicationInfoText.ReadOnly = true;
            this.applicationInfoText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.applicationInfoText.Size = new System.Drawing.Size(287, 60);
            this.applicationInfoText.TabIndex = 0;
            this.applicationInfoText.Text = "App Info: Was können wir über diese App erzählen wenn der TExt zu lang wir ddann " +
    "könnte umgebrochen werden aber ob er das wirklich macht";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(334, 434);
            this.ControlBox = false;
            this.Controls.Add(this.applicationInfoGroup);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.contactLink);
            this.Controls.Add(this.authorLabel);
            this.Controls.Add(this.licenseGroup);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.Shown += new System.EventHandler(this.AboutForm_Shown);
            this.licenseGroup.ResumeLayout(false);
            this.licenseGroup.PerformLayout();
            this.applicationInfoGroup.ResumeLayout(false);
            this.applicationInfoGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox licenseGroup;
        private System.Windows.Forms.Label authorLabel;
        private System.Windows.Forms.LinkLabel contactLink;
        private System.Windows.Forms.TextBox licenseText;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label spacerLabel;
        private System.Windows.Forms.GroupBox applicationInfoGroup;
        private System.Windows.Forms.TextBox applicationInfoText;
    }
}
