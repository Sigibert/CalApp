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
            this.calendarWeekLabel = new System.Windows.Forms.Label();
            this.fiscalWeekLabel = new System.Windows.Forms.Label();
            this.fiscalYearLabel = new System.Windows.Forms.Label();
            this.dateCalendar = new System.Windows.Forms.MonthCalendar();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.hideCalendarMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.dateSelectionGroup = new System.Windows.Forms.GroupBox();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.dateSelectionGroup.SuspendLayout();
            this.SuspendLayout();
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
            this.fileMenu,
            this.helpMenu});
            this.mainMenu.Location = new System.Drawing.Point(3, 0);
            this.mainMenu.Margin = new System.Windows.Forms.Padding(3);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(3);
            this.mainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mainMenu.Size = new System.Drawing.Size(273, 26);
            this.mainMenu.TabIndex = 6;
            this.mainMenu.Text = "mainMenu";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideCalendarMenu,
            this.preferencesMenu,
            this.fileSeparator,
            this.exitMenu});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(41, 20);
            this.fileMenu.Text = "File";
            // 
            // hideCalendarMenu
            // 
            this.hideCalendarMenu.Name = "hideCalendarMenu";
            this.hideCalendarMenu.Size = new System.Drawing.Size(180, 22);
            this.hideCalendarMenu.Text = "Close";
            this.hideCalendarMenu.Click += new System.EventHandler(this.HideCalendarMenu_Click);
            // 
            // preferencesMenu
            // 
            this.preferencesMenu.Name = "preferencesMenu";
            this.preferencesMenu.Size = new System.Drawing.Size(180, 22);
            this.preferencesMenu.Text = "Preferences";
            this.preferencesMenu.Click += new System.EventHandler(this.PreferencesMenu_Click);
            // 
            // fileSeparator
            // 
            this.fileSeparator.Name = "fileSeparator";
            this.fileSeparator.Size = new System.Drawing.Size(177, 6);
            // 
            // exitMenu
            // 
            this.exitMenu.Name = "exitMenu";
            this.exitMenu.Size = new System.Drawing.Size(180, 22);
            this.exitMenu.Text = "Exit";
            this.exitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
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
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenu});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(27, 20);
            this.helpMenu.Text = "?";
            // 
            // aboutMenu
            // 
            this.aboutMenu.Name = "aboutMenu";
            this.aboutMenu.Size = new System.Drawing.Size(180, 22);
            this.aboutMenu.Text = "Info";
            this.aboutMenu.Click += new System.EventHandler(this.AboutMenu_Click);
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
            this.Text = "WeekTrayInfo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalendarForm_FormClosing);
            this.Move += new System.EventHandler(this.CalendarForm_Move);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.dateSelectionGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label calendarWeekLabel;
        private System.Windows.Forms.Label fiscalWeekLabel;
        private System.Windows.Forms.Label fiscalYearLabel;
        private System.Windows.Forms.MonthCalendar dateCalendar;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenu;
        private System.Windows.Forms.ToolStripMenuItem hideCalendarMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenu;
        private System.Windows.Forms.ToolStripSeparator fileSeparator;
        private System.Windows.Forms.GroupBox dateSelectionGroup;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenu;
    }
}

