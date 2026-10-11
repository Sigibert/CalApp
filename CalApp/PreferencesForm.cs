using System;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.Preferences;

namespace CalApp
{
    public partial class PreferencesForm : Form
    {
        private readonly SettingsController settingsController;

        public PreferencesForm() : this(new SettingsController(SettingsStore.CreateDefault(),
            new StartupRegistration(), Application.ExecutablePath)) { }

        public PreferencesForm(SettingsController settingsController)
        {
            this.settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
            InitializeComponent();
            comboBoxFirstFiscalMonth.DataSource = Enum.GetValues(typeof(Month));
            comboBoxFiscalWeekRule.DataSource = Enum.GetValues(typeof(FiscalWeekRule));
            comboBoxFirstFiscalWeekday.DataSource = Enum.GetValues(typeof(DayOfWeek));
            comboBoxFirstFiscalMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFiscalWeekRule.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFirstFiscalWeekday.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFiscalWeekRule.Format += (sender, e) =>
            {
                int days = (int)(FiscalWeekRule)e.ListItem;
                e.Value = days == 7 ? "Full week (7 days)" : "At least " + days + (days == 1 ? " day" : " days");
            };
            AcceptButton = buttonOk;
            CancelButton = buttonCancel;
            buttonCancel.DialogResult = DialogResult.Cancel;
        }

        private void PreferencesForm_Load(object sender, EventArgs e) => LoadSettings(settingsController.Settings);

        private bool SaveSettings()
        {
            if (!checkBoxStandardIcon.Checked && !checkBoxFiscalWeekIcon.Checked && !checkBoxCalendarWeekIcon.Checked)
                checkBoxStandardIcon.Checked = true;

            var settings = settingsController.Settings;
            settings.FirstFiscalMonth = (Month)comboBoxFirstFiscalMonth.SelectedItem;
            settings.FiscalWeekRule = (FiscalWeekRule)comboBoxFiscalWeekRule.SelectedItem;
            settings.FirstDayOfFiscalWeek = (DayOfWeek)comboBoxFirstFiscalWeekday.SelectedItem;
            settings.StandardIcon = checkBoxStandardIcon.Checked;
            settings.FiscalWeekIcon = checkBoxFiscalWeekIcon.Checked;
            settings.CalendarWeekIcon = checkBoxCalendarWeekIcon.Checked;
            settings.StandardIconColor = colorDialogStandardIcon.Color;
            settings.FiscalWeekIconBackColor = colorDialogFiscalWeekIcon.Color;
            settings.FiscalWeekIconForeColorAuto = checkBoxFiscalWeekAutoFore.Checked;
            settings.FiscalWeekIconForeColor = colorDialogFiscalWeekIconFore.Color;
            settings.CalendarWeekIconBackColor = colorDialogCalendarWeekIcon.Color;
            settings.CalendarWeekIconForeColorAuto = checkBoxCalendarWeekAutoFore.Checked;
            settings.CalendarWeekIconForeColor = colorDialogCalendarWeekIconFore.Color;
            settings.StartWithWindows = checkBoxStartup.Checked;
            try
            {
                settingsController.Apply(settings);
                return true;
            }
            catch (Exception ex) when (SettingsStore.IsStorageError(ex)
                || ex is InvalidOperationException || ex is ArgumentException)
            {
                MessageBox.Show(this, "Preferences could not be applied:\n" + ex.Message,
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void LoadSettings(AppSettings settings)
        {
            comboBoxFirstFiscalMonth.SelectedItem = settings.FirstFiscalMonth;
            comboBoxFiscalWeekRule.SelectedItem = settings.FiscalWeekRule;
            comboBoxFirstFiscalWeekday.SelectedItem = settings.FirstDayOfFiscalWeek;
            checkBoxStandardIcon.Checked = settings.StandardIcon;
            checkBoxFiscalWeekIcon.Checked = settings.FiscalWeekIcon;
            checkBoxCalendarWeekIcon.Checked = settings.CalendarWeekIcon;
            colorDialogStandardIcon.Color = settings.StandardIconColor;
            colorDialogFiscalWeekIcon.Color = settings.FiscalWeekIconBackColor;
            colorDialogCalendarWeekIcon.Color = settings.CalendarWeekIconBackColor;
            checkBoxFiscalWeekAutoFore.Checked = settings.FiscalWeekIconForeColorAuto;
            colorDialogFiscalWeekIconFore.Color = settings.FiscalWeekIconForeColor;
            buttonFiscalWeekIconForeColor.Enabled = !settings.FiscalWeekIconForeColorAuto;
            checkBoxCalendarWeekAutoFore.Checked = settings.CalendarWeekIconForeColorAuto;
            colorDialogCalendarWeekIconFore.Color = settings.CalendarWeekIconForeColor;
            buttonCalendarWeekIconForeColor.Enabled = !settings.CalendarWeekIconForeColorAuto;
            checkBoxStartup.Checked = settings.StartWithWindows;
        }

        private void ButtonReset_Click(object sender, EventArgs e) => LoadSettings(settingsController.Settings);
        private void ButtonDefault_Click(object sender, EventArgs e) => LoadSettings(new AppSettings());
        private void ButtonApply_Click(object sender, EventArgs e) => SaveSettings();
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (SaveSettings()) { DialogResult = DialogResult.OK; Close(); }
        }
        private void ButtonCancel_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
        private void ButtonStandardIconColor_Click(object sender, EventArgs e) => colorDialogStandardIcon.ShowDialog(this);
        private void ButtonFiscalWeekIconColor_Click(object sender, EventArgs e) => colorDialogFiscalWeekIcon.ShowDialog(this);
        private void ButtonCalendarWeekIconColor_Click(object sender, EventArgs e) => colorDialogCalendarWeekIcon.ShowDialog(this);
        private void ButtonFiscalWeekIconForeColor_Click(object sender, EventArgs e) => colorDialogFiscalWeekIconFore.ShowDialog(this);
        private void ButtonCalendarWeekIconForeColor_Click(object sender, EventArgs e) => colorDialogCalendarWeekIconFore.ShowDialog(this);
        private void CheckBoxFiscalWeekAutoFore_CheckedChanged(object sender, EventArgs e)
            => buttonFiscalWeekIconForeColor.Enabled = !checkBoxFiscalWeekAutoFore.Checked;
        private void CheckBoxCalendarWeekAutoFore_CheckedChanged(object sender, EventArgs e)
            => buttonCalendarWeekIconForeColor.Enabled = !checkBoxCalendarWeekAutoFore.Checked;
    }
}
