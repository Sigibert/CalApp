namespace WeekTrayInfo
{
    partial class CalendarForm
    {
        private System.ComponentModel.IContainer components;
        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            calendarWeekLabel = new System.Windows.Forms.Label();
            fiscalWeekLabel = new System.Windows.Forms.Label();
            fiscalYearLabel = new System.Windows.Forms.Label();
            dateCalendar = new System.Windows.Forms.MonthCalendar();
            dateSelectionGroup = new System.Windows.Forms.GroupBox();
            calendarLayout = new System.Windows.Forms.TableLayoutPanel();
            moreButton = new System.Windows.Forms.Button();
            calendarMenu = new System.Windows.Forms.ContextMenuStrip(components);
            calendarPreferencesMenu = new System.Windows.Forms.ToolStripMenuItem();
            calendarAboutMenu = new System.Windows.Forms.ToolStripMenuItem();
            calendarExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            menuToolTip = new System.Windows.Forms.ToolTip(components);
            SuspendLayout();
            calendarLayout.SuspendLayout();
            dateSelectionGroup.SuspendLayout();

            calendarLayout.Name = "calendarLayout";
            calendarLayout.AutoSize = true;
            calendarLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            calendarLayout.ColumnCount = 2;
            calendarLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            calendarLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            calendarLayout.RowCount = 4;
            for (int row = 0; row < 4; row++)
                calendarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            calendarLayout.Location = new System.Drawing.Point(6, 6);
            calendarLayout.Margin = new System.Windows.Forms.Padding(0);
            var labels = new[] { calendarWeekLabel, fiscalWeekLabel, fiscalYearLabel };
            var names = new[] { "calendarWeekLabel", "fiscalWeekLabel", "fiscalYearLabel" };
            for (int row = 0; row < labels.Length; row++)
            {
                labels[row].Name = names[row];
                labels[row].AutoSize = true;
                labels[row].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                labels[row].Padding = new System.Windows.Forms.Padding(3);
                labels[row].Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
                calendarLayout.Controls.Add(labels[row], 0, row);
                if (row > 0) calendarLayout.SetColumnSpan(labels[row], 2);
            }
            moreButton.Name = "moreButton";
            moreButton.Text = "\u22ef";
            moreButton.Font = new System.Drawing.Font("Segoe UI", 16F);
            moreButton.Size = new System.Drawing.Size(30, 28);
            moreButton.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            moreButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            moreButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            moreButton.FlatAppearance.BorderSize = 0;
            moreButton.BackColor = System.Drawing.SystemColors.ControlLight;
            moreButton.TabIndex = 0;
            moreButton.Click += MoreButton_Click;
            calendarLayout.Controls.Add(moreButton, 1, 0);
            calendarMenu.Name = "calendarMenu";
            calendarMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            calendarPreferencesMenu.Name = "calendarPreferencesMenu";
            calendarPreferencesMenu.Click += PreferencesMenu_Click;
            calendarAboutMenu.Name = "calendarAboutMenu";
            calendarAboutMenu.Click += AboutMenu_Click;
            calendarExitMenu.Name = "calendarExitMenu";
            calendarExitMenu.Click += ExitMenu_Click;
            calendarMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                calendarPreferencesMenu, calendarAboutMenu, new System.Windows.Forms.ToolStripSeparator(), calendarExitMenu });
            dateCalendar.Name = "dateCalendar";
            dateCalendar.Location = new System.Drawing.Point(6, 19);
            dateCalendar.FirstDayOfWeek = System.Windows.Forms.Day.Monday;
            dateCalendar.MinDate = new System.DateTime(1900, 1, 1);
            dateCalendar.ShowWeekNumbers = true;
            dateCalendar.TabIndex = 1;
            dateCalendar.DateChanged += DateCalendar_DateChanged;
            dateCalendar.SizeChanged += DateCalendar_SizeChanged;
            dateSelectionGroup.Name = "dateSelectionGroup";
            dateSelectionGroup.Size = new System.Drawing.Size(dateCalendar.Right + 6, dateCalendar.Bottom + 6);
            dateSelectionGroup.Margin = new System.Windows.Forms.Padding(0);
            dateSelectionGroup.Padding = new System.Windows.Forms.Padding(3, 3, 6, 6);
            dateSelectionGroup.Controls.Add(dateCalendar);
            calendarLayout.Controls.Add(dateSelectionGroup, 0, 3);
            calendarLayout.SetColumnSpan(dateSelectionGroup, 2);

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.SystemColors.Window;
            ForeColor = System.Drawing.SystemColors.WindowText;
            Controls.Add(calendarLayout);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Resource.CalendarIcon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CalendarForm";
            Padding = new System.Windows.Forms.Padding(6);
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "WeekTrayInfo";
            FormClosing += CalendarForm_FormClosing;
            Move += CalendarForm_Move;
            dateSelectionGroup.ResumeLayout(false);
            calendarLayout.ResumeLayout(false);
            calendarLayout.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label calendarWeekLabel;
        private System.Windows.Forms.Label fiscalWeekLabel;
        private System.Windows.Forms.Label fiscalYearLabel;
        private System.Windows.Forms.MonthCalendar dateCalendar;
        private System.Windows.Forms.GroupBox dateSelectionGroup;
        private System.Windows.Forms.TableLayoutPanel calendarLayout;
        private System.Windows.Forms.Button moreButton;
        private System.Windows.Forms.ContextMenuStrip calendarMenu;
        private System.Windows.Forms.ToolStripMenuItem calendarPreferencesMenu;
        private System.Windows.Forms.ToolStripMenuItem calendarAboutMenu;
        private System.Windows.Forms.ToolStripMenuItem calendarExitMenu;
        private System.Windows.Forms.ToolTip menuToolTip;
    }
}
