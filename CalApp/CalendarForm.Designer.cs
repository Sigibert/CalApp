namespace CalApp
{
    partial class CalendarForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed) ReleaseTrayResources();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalendarForm));
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.standardTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.calendarWeekLabel = new System.Windows.Forms.Label();
            this.fiscalWeekLabel = new System.Windows.Forms.Label();
            this.fiscalYearLabel = new System.Windows.Forms.Label();
            this.dateCalendar = new System.Windows.Forms.MonthCalendar();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fiscalWeekTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.calendarWeekTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.dateSelectionGroup = new System.Windows.Forms.GroupBox();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.dateSelectionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayMenu
            // 
            this.trayMenu.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.toolStripSeparator2,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator1,
            this.showToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(136, 104);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(132, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.PreferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(132, 6);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.ShowToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // standardTrayIcon
            // 
            this.standardTrayIcon.ContextMenuStrip = this.trayMenu;
            this.standardTrayIcon.Text = "Settings";
            this.standardTrayIcon.Visible = true;
            this.standardTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // calendarWeekLabel
            // 
            this.calendarWeekLabel.AutoSize = true;
            this.calendarWeekLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.calendarWeekLabel.Location = new System.Drawing.Point(6, 32);
            this.calendarWeekLabel.Margin = new System.Windows.Forms.Padding(3);
            this.calendarWeekLabel.Name = "calendarWeekLabel";
            this.calendarWeekLabel.Padding = new System.Windows.Forms.Padding(3);
            this.calendarWeekLabel.Size = new System.Drawing.Size(57, 26);
            this.calendarWeekLabel.TabIndex = 1;
            this.calendarWeekLabel.Text = "calendarWeekLabel";
            // 
            // fiscalWeekLabel
            // 
            this.fiscalWeekLabel.AutoSize = true;
            this.fiscalWeekLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.fiscalWeekLabel.Location = new System.Drawing.Point(6, 64);
            this.fiscalWeekLabel.Margin = new System.Windows.Forms.Padding(3);
            this.fiscalWeekLabel.Name = "fiscalWeekLabel";
            this.fiscalWeekLabel.Padding = new System.Windows.Forms.Padding(3);
            this.fiscalWeekLabel.Size = new System.Drawing.Size(57, 26);
            this.fiscalWeekLabel.TabIndex = 2;
            this.fiscalWeekLabel.Text = "fiscalWeekLabel";
            // 
            // fiscalYearLabel
            // 
            this.fiscalYearLabel.AutoSize = true;
            this.fiscalYearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.fiscalYearLabel.Location = new System.Drawing.Point(6, 96);
            this.fiscalYearLabel.Margin = new System.Windows.Forms.Padding(3);
            this.fiscalYearLabel.Name = "fiscalYearLabel";
            this.fiscalYearLabel.Padding = new System.Windows.Forms.Padding(3);
            this.fiscalYearLabel.Size = new System.Drawing.Size(57, 26);
            this.fiscalYearLabel.TabIndex = 3;
            this.fiscalYearLabel.Text = "fiscalYearLabel";
            // 
            // dateCalendar
            // 
            this.dateCalendar.FirstDayOfWeek = System.Windows.Forms.Day.Monday;
            this.dateCalendar.Location = new System.Drawing.Point(6, 19);
            this.dateCalendar.Margin = new System.Windows.Forms.Padding(3);
            this.dateCalendar.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dateCalendar.Name = "dateCalendar";
            this.dateCalendar.ShowWeekNumbers = true;
            this.dateCalendar.TabIndex = 5;
            this.dateCalendar.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.DateCalendar_DateChanged);
            // 
            // mainMenu
            // 
            this.mainMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mainMenu.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(3, 0);
            this.mainMenu.Margin = new System.Windows.Forms.Padding(3);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(3);
            this.mainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mainMenu.Size = new System.Drawing.Size(273, 26);
            this.mainMenu.TabIndex = 6;
            this.mainMenu.Text = "mainMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem1,
            this.preferencesToolStripMenuItem1,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem1.Text = "Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.CloseToolStripMenuItem1_Click);
            // 
            // preferencesToolStripMenuItem1
            // 
            this.preferencesToolStripMenuItem1.Name = "preferencesToolStripMenuItem1";
            this.preferencesToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.preferencesToolStripMenuItem1.Text = "Preferences";
            this.preferencesToolStripMenuItem1.Click += new System.EventHandler(this.PreferencesToolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.ExitToolStripMenuItem1_Click);
            // 
            // fiscalWeekTrayIcon
            // 
            this.fiscalWeekTrayIcon.ContextMenuStrip = this.trayMenu;
            this.fiscalWeekTrayIcon.Text = "Fiscal Week";
            this.fiscalWeekTrayIcon.Visible = true;
            this.fiscalWeekTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // calendarWeekTrayIcon
            // 
            this.calendarWeekTrayIcon.ContextMenuStrip = this.trayMenu;
            this.calendarWeekTrayIcon.Text = "Calendar Week";
            this.calendarWeekTrayIcon.Visible = true;
            this.calendarWeekTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // dateSelectionGroup
            // 
            this.dateSelectionGroup.Controls.Add(this.dateCalendar);
            this.dateSelectionGroup.Location = new System.Drawing.Point(6, 128);
            this.dateSelectionGroup.Name = "dateSelectionGroup";
            this.dateSelectionGroup.Size = new System.Drawing.Size(212, 186);
            this.dateSelectionGroup.TabIndex = 7;
            this.dateSelectionGroup.TabStop = false;
            this.dateSelectionGroup.Text = "Date Selection";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem1});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(27, 20);
            this.infoToolStripMenuItem.Text = "?";
            // 
            // infoToolStripMenuItem1
            // 
            this.infoToolStripMenuItem1.Name = "infoToolStripMenuItem1";
            this.infoToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.infoToolStripMenuItem1.Text = "Info";
            this.infoToolStripMenuItem1.Click += new System.EventHandler(this.InfoToolStripMenuItem_Click);
            // 
            // CalendarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(279, 377);
            this.Controls.Add(this.dateSelectionGroup);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.fiscalYearLabel);
            this.Controls.Add(this.fiscalWeekLabel);
            this.Controls.Add(this.calendarWeekLabel);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.MainMenuStrip = this.mainMenu;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalendarForm";
            this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calendar Week Info";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalendarForm_FormClosing);
            this.Move += new System.EventHandler(this.CalendarForm_Move);
            this.trayMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.dateSelectionGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon standardTrayIcon;
        private System.Windows.Forms.Label calendarWeekLabel;
        private System.Windows.Forms.Label fiscalWeekLabel;
        private System.Windows.Forms.Label fiscalYearLabel;
        private System.Windows.Forms.MonthCalendar dateCalendar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.NotifyIcon fiscalWeekTrayIcon;
        private System.Windows.Forms.NotifyIcon calendarWeekTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.GroupBox dateSelectionGroup;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem1;
    }
}

