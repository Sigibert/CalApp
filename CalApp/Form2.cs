using System;
using System.Drawing;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.Preferences;

namespace CalApp
{
    public partial class Form2 : Form
    {
        private ConfigMng configMng = new ConfigMng();
        public event EventHandler ApplyEvent;

        public Form2()
        {
            InitializeComponent();
            this.comboBoxFirstFiscalMonth.DataSource = Enum.GetValues(typeof(Month));
            this.comboBoxFiscalWeekRule.DataSource = Enum.GetValues(typeof(FiscalWeekRule));
            this.comboBoxFirstDayOfFW.DataSource = Enum.GetValues(typeof(DayOfWeek));
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            configMng.LoadConfig();
            LoadSettings();
        }

        private void SaveSettings()
        {
            // FIX: was using | (bitwise OR) on booleans — should be || (logical OR)
            if (!(this.checkBoxStandardIcon.Checked || this.checkBoxFiscalWeekIcon.Checked || this.checkBoxCalendarWeekIcon.Checked))
                this.checkBoxStandardIcon.Checked = true;

            configMng.ConfigDataObject.FirstFiscalMonth = (Month)Enum.Parse(typeof(Month), comboBoxFirstFiscalMonth.SelectedItem.ToString());
            configMng.ConfigDataObject.FiscalWeekRule = (FiscalWeekRule)Enum.Parse(typeof(FiscalWeekRule), comboBoxFiscalWeekRule.SelectedItem.ToString());
            configMng.ConfigDataObject.FirstDayOfFiscalWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), comboBoxFirstDayOfFW.SelectedItem.ToString());
            configMng.ConfigDataObject.StandardIcon = this.checkBoxStandardIcon.Checked;
            configMng.ConfigDataObject.FiscalWeekIcon = this.checkBoxFiscalWeekIcon.Checked;
            configMng.ConfigDataObject.CalendarWeekIcon = this.checkBoxCalendarWeekIcon.Checked;
            configMng.ConfigDataObject.StandardIconColor = this.colorDialogStandardIcon.Color;
            configMng.ConfigDataObject.FiscalWeekIconBackColor = this.colorDialogFiscalWeekIcon.Color;
            // Text colour: auto = contrast complement of the background;
            // manual = whatever the user picked in the "Text" colour dialog.
            configMng.ConfigDataObject.FiscalWeekIconForeColorAuto = this.checkBoxFiscalWeekAutoFore.Checked;
            configMng.ConfigDataObject.FiscalWeekIconForeColor = this.checkBoxFiscalWeekAutoFore.Checked
                ? HSVColorConverter.Complement1Color(this.colorDialogFiscalWeekIcon.Color)
                : this.colorDialogFiscalWeekIconFore.Color;
            configMng.ConfigDataObject.CalendarWeekIconBackColor = this.colorDialogCalendarWeekIcon.Color;
            configMng.ConfigDataObject.CalendarWeekIconForeColorAuto = this.checkBoxCalendarWeekAutoFore.Checked;
            configMng.ConfigDataObject.CalendarWeekIconForeColor = this.checkBoxCalendarWeekAutoFore.Checked
                ? HSVColorConverter.Complement1Color(this.colorDialogCalendarWeekIcon.Color)
                : this.colorDialogCalendarWeekIconFore.Color;
            configMng.ConfigDataObject.StartWithWindows = this.checkBoxStartup.Checked;
            configMng.SaveConfig();
            Startup.SetStartup(configMng.ConfigDataObject.StartWithWindows);
        }

        private void LoadSettings()
        {
            this.comboBoxFirstFiscalMonth.SelectedItem = configMng.ConfigDataObject.FirstFiscalMonth;
            this.comboBoxFiscalWeekRule.SelectedItem = configMng.ConfigDataObject.FiscalWeekRule;
            this.comboBoxFirstDayOfFW.SelectedItem = configMng.ConfigDataObject.FirstDayOfFiscalWeek;
            this.checkBoxStandardIcon.Checked = configMng.ConfigDataObject.StandardIcon;
            this.checkBoxFiscalWeekIcon.Checked = configMng.ConfigDataObject.FiscalWeekIcon;
            this.checkBoxCalendarWeekIcon.Checked = configMng.ConfigDataObject.CalendarWeekIcon;
            this.colorDialogStandardIcon.Color = configMng.ConfigDataObject.StandardIconColor;
            this.colorDialogFiscalWeekIcon.Color = configMng.ConfigDataObject.FiscalWeekIconBackColor;
            this.colorDialogCalendarWeekIcon.Color = configMng.ConfigDataObject.CalendarWeekIconBackColor;
            this.checkBoxFiscalWeekAutoFore.Checked = configMng.ConfigDataObject.FiscalWeekIconForeColorAuto;
            this.colorDialogFiscalWeekIconFore.Color = configMng.ConfigDataObject.FiscalWeekIconForeColor;
            this.buttonFiscalWeekIconForeColor.Enabled = !configMng.ConfigDataObject.FiscalWeekIconForeColorAuto;
            this.checkBoxCalendarWeekAutoFore.Checked = configMng.ConfigDataObject.CalendarWeekIconForeColorAuto;
            this.colorDialogCalendarWeekIconFore.Color = configMng.ConfigDataObject.CalendarWeekIconForeColor;
            this.buttonCalendarWeekIconForeColor.Enabled = !configMng.ConfigDataObject.CalendarWeekIconForeColorAuto;
            this.checkBoxStartup.Checked = configMng.ConfigDataObject.StartWithWindows;
        }

        private void ButtonReset_Click(object sender, EventArgs e) => LoadSettings();

        private void ButtonDefault_Click(object sender, EventArgs e)
        {
            this.comboBoxFirstFiscalMonth.SelectedItem = configMng.DefaultConfig.FirstFiscalMonth;
            this.comboBoxFiscalWeekRule.SelectedItem = configMng.DefaultConfig.FiscalWeekRule;
            this.comboBoxFirstDayOfFW.SelectedItem = configMng.DefaultConfig.FirstDayOfFiscalWeek;
            this.checkBoxStandardIcon.Checked = configMng.DefaultConfig.StandardIcon;
            this.checkBoxFiscalWeekIcon.Checked = configMng.DefaultConfig.FiscalWeekIcon;
            this.checkBoxCalendarWeekIcon.Checked = configMng.DefaultConfig.CalendarWeekIcon;
            this.colorDialogStandardIcon.Color = configMng.DefaultConfig.StandardIconColor;
            this.colorDialogFiscalWeekIcon.Color = configMng.DefaultConfig.FiscalWeekIconBackColor;
            this.colorDialogCalendarWeekIcon.Color = configMng.DefaultConfig.CalendarWeekIconBackColor;
            this.checkBoxFiscalWeekAutoFore.Checked = configMng.DefaultConfig.FiscalWeekIconForeColorAuto;
            this.colorDialogFiscalWeekIconFore.Color = configMng.DefaultConfig.FiscalWeekIconForeColor;
            this.checkBoxCalendarWeekAutoFore.Checked = configMng.DefaultConfig.CalendarWeekIconForeColorAuto;
            this.colorDialogCalendarWeekIconFore.Color = configMng.DefaultConfig.CalendarWeekIconForeColor;
            this.checkBoxStartup.Checked = configMng.DefaultConfig.StartWithWindows;
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            SaveSettings();
            ApplyEvent?.Invoke(this, e);
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            SaveSettings();
            ApplyEvent?.Invoke(this, e);
            // FIX: was this.Dispose() which bypassed the framework close sequence.
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e) => this.Close();

        private void ButtonStandardIconColor_Click(object sender, EventArgs e)
        {
            this.colorDialogStandardIcon.ShowDialog();
        }

        private void ButtonFiscalWeekIconColor_Click(object sender, EventArgs e)
        {
            this.colorDialogFiscalWeekIcon.ShowDialog();
        }

        private void ButtonCalendarWeekIconColor_Click(object sender, EventArgs e)
        {
            this.colorDialogCalendarWeekIcon.ShowDialog();
        }

        private void ButtonFiscalWeekIconForeColor_Click(object sender, EventArgs e)
        {
            this.colorDialogFiscalWeekIconFore.ShowDialog();
        }

        private void ButtonCalendarWeekIconForeColor_Click(object sender, EventArgs e)
        {
            this.colorDialogCalendarWeekIconFore.ShowDialog();
        }

        // The manual "Text" colour picker only makes sense when auto is off,
        // so enable/disable it to match the checkbox.
        private void CheckBoxFiscalWeekAutoFore_CheckedChanged(object sender, EventArgs e)
        {
            this.buttonFiscalWeekIconForeColor.Enabled = !this.checkBoxFiscalWeekAutoFore.Checked;
        }

        private void CheckBoxCalendarWeekAutoFore_CheckedChanged(object sender, EventArgs e)
        {
            this.buttonCalendarWeekIconForeColor.Enabled = !this.checkBoxCalendarWeekAutoFore.Checked;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // FIX: removed manual this.Dispose() call via Close_Form2().
            // Framework handles dispose automatically after FormClosing completes.
        }
    }
}