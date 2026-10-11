using System;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.Preferences;
using CalApp.Localization;
using System.Drawing;
using CalApp.DateFunctions;
using System.Xml;

namespace CalApp
{
    public partial class PreferencesForm : Form
    {
        private readonly SettingsController settingsController;
        private readonly GroupBox languageGroup = new GroupBox { Name = "languageGroup" };
        private readonly Label languageLabel = new Label { Name = "languageLabel", AutoSize = true };
        private readonly ComboBox comboBoxLanguage = new ComboBox { Name = "comboBoxLanguage", DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Label fiscalPreview = new Label { Name = "fiscalPreview" };
        private readonly GroupBox transferGroup = new GroupBox { Name = "transferGroup" };
        private readonly Button importButton = new Button { Name = "importButton" };
        private readonly Button exportButton = new Button { Name = "exportButton" };
        private readonly Label transferHint = new Label { Name = "transferHint" };
        private readonly Func<DateTime> clock;

        public PreferencesForm() : this(new SettingsController(SettingsStore.CreateDefault(),
            new StartupRegistration(), Application.ExecutablePath)) { }

        public PreferencesForm(SettingsController settingsController, Func<DateTime> clock = null)
        {
            this.settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
            this.clock = clock ?? (() => DateTime.Now);
            InitializeComponent();
            fiscalPreview.SetBounds(9, 111, 405, 38);
            fiscalCalendarGroup.Controls.Add(fiscalPreview);
            fiscalCalendarGroup.Height += 43;
            startupGroup.Top = fiscalCalendarGroup.Bottom + 6;
            languageGroup.SetBounds(9, startupGroup.Bottom + 6, 423, 55);
            languageLabel.Location = new Point(9, 22);
            comboBoxLanguage.SetBounds(158, 19, 190, 21);
            languageGroup.Controls.Add(languageLabel);
            languageGroup.Controls.Add(comboBoxLanguage);
            Controls.Add(languageGroup);
            transferGroup.SetBounds(9, languageGroup.Bottom + 6, 423, 81);
            importButton.SetBounds(9, 19, 100, 23);
            exportButton.SetBounds(121, 19, 100, 23);
            transferHint.SetBounds(9, 47, 405, 30);
            transferGroup.Controls.AddRange(new Control[] { importButton, exportButton, transferHint });
            Controls.Add(transferGroup);
            importButton.Click += ImportButton_Click;
            exportButton.Click += ExportButton_Click;
            int tabIndex = 0;
            foreach (GroupBox group in new[] { iconsGroup, fiscalCalendarGroup, startupGroup, languageGroup, transferGroup }) group.TabIndex = tabIndex++;
            foreach (Button button in new[] { buttonOk, buttonCancel, buttonApply, buttonReset, buttonDefault })
            {
                button.Top = transferGroup.Bottom + 6;
                button.TabIndex = tabIndex++;
            }
            comboBoxLanguage.TabIndex = 0;
            comboBoxLanguage.DataSource = Enum.GetValues(typeof(AppLanguage));
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Format += (sender, e) => e.Value = AppText.LanguageName((AppLanguage)e.ListItem);
            comboBoxFirstFiscalMonth.DataSource = Enum.GetValues(typeof(Month));
            comboBoxFiscalWeekRule.DataSource = Enum.GetValues(typeof(FiscalWeekRule));
            comboBoxFirstFiscalWeekday.DataSource = Enum.GetValues(typeof(DayOfWeek));
            comboBoxFirstFiscalMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFiscalWeekRule.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFirstFiscalWeekday.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFiscalWeekRule.Format += (sender, e) =>
            {
                int days = (int)(FiscalWeekRule)e.ListItem;
                e.Value = days == 7 ? AppText.Get("FullWeek") : days == 1 ? AppText.Get("OneDay") : AppText.Format("MinimumDays", days);
            };
            comboBoxFirstFiscalMonth.FormattingEnabled = true;
            comboBoxFirstFiscalWeekday.FormattingEnabled = true;
            comboBoxFiscalWeekRule.FormattingEnabled = true;
            comboBoxFirstFiscalMonth.Format += (sender, e) => e.Value = AppText.Culture.DateTimeFormat.GetMonthName((int)(Month)e.ListItem);
            comboBoxFirstFiscalWeekday.Format += (sender, e) => e.Value = AppText.Culture.DateTimeFormat.GetDayName((DayOfWeek)e.ListItem);
            comboBoxFirstFiscalMonth.SelectedValueChanged += PreviewSettingsChanged;
            comboBoxFirstFiscalWeekday.SelectedValueChanged += PreviewSettingsChanged;
            comboBoxFiscalWeekRule.SelectedValueChanged += PreviewSettingsChanged;
            AcceptButton = buttonOk;
            CancelButton = buttonCancel;
            buttonCancel.DialogResult = DialogResult.Cancel;
            ApplyLanguage();
        }

        internal void ApplyLanguage()
        {
            SuspendLayout();
            AppText.Translate(this);
            RefreshList(comboBoxLanguage, typeof(AppLanguage));
            RefreshList(comboBoxFirstFiscalMonth, typeof(Month));
            RefreshList(comboBoxFirstFiscalWeekday, typeof(DayOfWeek));
            RefreshList(comboBoxFiscalWeekRule, typeof(FiscalWeekRule));
            buttonFiscalWeekIconForeColor.Left = Math.Max(340, checkBoxFiscalWeekAutoFore.Right + 7);
            buttonCalendarWeekIconForeColor.Left = Math.Max(340, checkBoxCalendarWeekAutoFore.Right + 7);
            comboBoxFiscalWeekRule.Width = 155;
            // Allow longer translations without increasing the original compact English layout.
            int left = 9;
            foreach (Button button in new[] { buttonOk, buttonCancel, buttonApply, buttonReset, buttonDefault })
            {
                button.Width = Math.Max(75, TextRenderer.MeasureText(button.Text, button.Font).Width + 16);
                button.Left = left;
                left += button.Width + 12;
            }
            int width = Math.Max(Math.Max(423, left - 21), buttonFiscalWeekIconForeColor.Right + 8);
            foreach (GroupBox group in new[] { iconsGroup, fiscalCalendarGroup, startupGroup, languageGroup, transferGroup }) group.Width = width;
            fiscalPreview.Width = transferHint.Width = width - 18;
            UpdatePreview();
            ResumeLayout(true);
        }

        private static void RefreshList(ComboBox combo, Type type)
        {
            object selected = combo.SelectedItem;
            combo.DataSource = Enum.GetValues(type);
            if (selected != null) combo.SelectedItem = selected;
        }

        private void PreferencesForm_Load(object sender, EventArgs e) => LoadSettings(settingsController.Settings);

        internal AppSettings ReadPendingSettings()
        {
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
            settings.Language = (AppLanguage)comboBoxLanguage.SelectedItem;
            if (!settings.StandardIcon && !settings.FiscalWeekIcon && !settings.CalendarWeekIcon) settings.StandardIcon = true;
            return settings;
        }

        private bool SaveSettings()
        {
            AppSettings settings = ReadPendingSettings();
            checkBoxStandardIcon.Checked = settings.StandardIcon;
            try
            {
                settingsController.Apply(settings);
                AppText.UseLanguage(settings.Language);
                ApplyLanguage();
                return true;
            }
            catch (Exception ex) when (SettingsStore.IsStorageError(ex)
                || ex is InvalidOperationException || ex is ArgumentException)
            {
                MessageBox.Show(this, AppText.Format("ApplyFailed", ex.Message),
                    AppText.Get("PreferencesForm"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            comboBoxLanguage.SelectedItem = settings.Language;
            UpdatePreview();
        }

        private void PreviewSettingsChanged(object sender, EventArgs e) => UpdatePreview();

        private void UpdatePreview()
        {
            if (comboBoxFirstFiscalMonth.SelectedItem == null || comboBoxFiscalWeekRule.SelectedItem == null || comboBoxFirstFiscalWeekday.SelectedItem == null) return;
            var month = (Month)comboBoxFirstFiscalMonth.SelectedItem;
            var rule = (FiscalWeekRule)comboBoxFiscalWeekRule.SelectedItem;
            var weekday = (DayOfWeek)comboBoxFirstFiscalWeekday.SelectedItem;
            FiscalDate year = FiscalCalendar.GetDate(clock(), weekday, rule, month);
            DateTime start = FiscalCalendar.GetYearStart(year.StartYear, weekday, rule, month);
            fiscalPreview.Text = AppText.Format("FiscalPreview", year.YearLabel,
                start.ToString("d", AppText.Culture), start.AddDays(6).ToString("d", AppText.Culture));
        }

        internal void ImportFile(string path) => LoadSettings(SettingsTransfer.Import(path));
        internal void ExportFile(string path) => SettingsTransfer.Export(path, ReadPendingSettings(), settingsController.SettingsFilePath);

        private void ImportButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Title = AppText.Get("ImportTitle"), Filter = AppText.Get("SettingsFileFilter"), CheckFileExists = true })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try { ImportFile(dialog.FileName); }
                catch (Exception ex) when (TransferError(ex))
                {
                    MessageBox.Show(this, AppText.Format("ImportFailed", ex.Message), AppText.Get("ImportTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Title = AppText.Get("ExportTitle"), Filter = AppText.Get("SettingsFileFilter"),
                DefaultExt = "xml", AddExtension = true, FileName = "CalApp-Settings.xml", OverwritePrompt = true })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    ExportFile(dialog.FileName);
                    MessageBox.Show(this, AppText.Get("Exported"), AppText.Get("ExportTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) when (TransferError(ex))
                {
                    MessageBox.Show(this, AppText.Format("ExportFailed", ex.Message), AppText.Get("ExportTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static bool TransferError(Exception ex) => SettingsStore.IsStorageError(ex) || ex is InvalidOperationException || ex is ArgumentException || ex is XmlException;

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
