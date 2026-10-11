using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.DateFunctions;
using CalApp.Preferences;

namespace CalApp.Tests
{
    // A package-free regression runner: exit code 1 means a failed test.
    internal static class RegressionTests
    {
        private static int passed;
        private static int failed;
        private static long assertions;

        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Contains("--startup-timing"))
            {
                MeasureStartup(args.Contains("--empty-settings"));
                return 0;
            }
            Run("ISO weeks over the complete Gregorian cycle", IsoWeeks);
            Run("Fiscal boundaries for all configurable rules", FiscalBoundaries);
            Run("Fiscal dates throughout leap and ordinary years", FiscalWholeYears);
            Run("Known fiscal regressions and date-only calculation", FiscalExamples);
            Run("Calendar settings reject invalid enum values", InvalidCalendarSettings);
            Run("Automatic text contrast", TextContrast);
            Run("Refresh scheduling at midnight and sub-millisecond boundaries", RefreshIntervals);
            Run("Legacy XML colors and defaults remain compatible", LegacyXml);
            Run("Settings round trip and independent reloads", SettingsRoundTrip);
            Run("Highest legacy version is migrated without changing its file", SettingsMigration);
            Run("Corrupt preferences recover from backup", BackupRecovery);
            Run("Corrupt files and unsafe XML use defaults with a warning", CorruptSettings);
            Run("Invalid saves leave file and current settings unchanged", FailedSave);
            Run("Autostart changes are rolled back after a save failure", StartupRollback);
            Run("Denied autostart changes do not commit preferences", StartupDenied);
            Run("Successful Apply publishes one committed snapshot", SuccessfulApply);
            if (args.Contains("--ui"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Run("Calendar selection remains consistent after Apply", CalendarSelection);
                Run("Preference cancel, defaults, reset and manual colors", PreferenceControls);
                Run("Hidden tray form creates handles and releases resources", TrayLifecycle);
                Run("Window close hides the app; application exit ends its context", ApplicationLifetime);
            }
            Console.WriteLine("Passed: " + passed + "; failed: " + failed + "; assertions: " + assertions);
            return failed == 0 ? 0 : 1;
        }

        private static void Run(string name, Action test)
        {
            try { test(); passed++; Console.WriteLine("PASS " + name); }
            catch (Exception ex) { failed++; Console.WriteLine("FAIL " + name + ": " + ex); }
        }

        private static void MeasureStartup(bool emptySettings) => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml");
            if (!emptySettings) File.WriteAllText(path, "<ConfigDataObject><FirstFiscalMonth>October</FirstFiscalMonth></ConfigDataObject>");
            var timer = Stopwatch.StartNew();
            var store = new SettingsStore(path);
            Console.WriteLine("Settings store creation: " + timer.ElapsedMilliseconds + " ms");
            timer.Restart();
            var controller = new SettingsController(store, new FakeStartup(), "test.exe");
            Console.WriteLine("Settings load: " + timer.ElapsedMilliseconds + " ms");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            timer.Restart();
            using (var form = new CalendarForm(controller))
            {
                Console.WriteLine("Calendar form and tray icons: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                form.EnsureHandleCreated();
                Console.WriteLine("Window handle creation: " + timer.ElapsedMilliseconds + " ms");
            }
        });

        private static void Equal<T>(T expected, T actual, string detail = "")
        {
            assertions++;
            if (!Equals(expected, actual)) throw new Exception(detail + " expected=" + expected + " actual=" + actual);
        }

        private static void Throws<T>(Action action) where T : Exception
        {
            assertions++;
            try { action(); } catch (T) { return; }
            throw new Exception("Expected " + typeof(T).Name);
        }

        private static void IsoWeeks()
        {
            for (DateTime date = new DateTime(2000, 1, 1); date < new DateTime(2400, 1, 1); date = date.AddDays(1))
            {
                DateTime thursday = date.AddDays(3 - ((int)date.DayOfWeek + 6) % 7);
                Equal((thursday.DayOfYear - 1) / 7 + 1, IsoCalendar.GetWeek(date), date.ToString("yyyy-MM-dd"));
            }
            Equal(52, IsoCalendar.GetWeek(DateTime.MaxValue));
        }

        // Independent reference: count new-year days in the week BEFORE the first
        // matching weekday, rather than using the production offset expression.
        private static DateTime ReferenceStart(int year, int month, DayOfWeek weekday, int requiredDays)
        {
            DateTime first = new DateTime(year, month, 1);
            DateTime nextWeek = first;
            while (nextWeek.DayOfWeek != weekday) nextWeek = nextWeek.AddDays(1);
            return (nextWeek - first).Days >= requiredDays ? nextWeek.AddDays(-7) : nextWeek;
        }

        private static void CheckFiscal(DateTime date, int month, DayOfWeek weekday, int rule)
        {
            int year = date.Year - 1;
            while (date >= ReferenceStart(year + 1, month, weekday, rule)) year++;
            DateTime start = ReferenceStart(year, month, weekday, rule);
            FiscalDate result = FiscalCalendar.GetDate(date, weekday, (FiscalWeekRule)rule, (Month)month);
            Equal(year, result.StartYear, "Fiscal year " + date);
            Equal((date - start).Days / 7 + 1, result.Week, "Fiscal week " + date);
        }

        private static void FiscalBoundaries()
        {
            for (int year = 2020; year < 2030; year++)
            for (int month = 1; month <= 12; month++)
            for (int weekday = 0; weekday < 7; weekday++)
            for (int rule = 1; rule <= 7; rule++)
            for (int offset = -7; offset <= 7; offset++)
                CheckFiscal(new DateTime(year, month, 1).AddDays(offset), month, (DayOfWeek)weekday, rule);
        }

        private static void FiscalWholeYears()
        {
            for (DateTime date = new DateTime(2023, 1, 1); date < new DateTime(2025, 1, 1); date = date.AddDays(1))
            for (int month = 1; month <= 12; month++)
            for (int weekday = 0; weekday < 7; weekday++)
            for (int rule = 1; rule <= 7; rule++) CheckFiscal(date, month, (DayOfWeek)weekday, rule);
        }

        private static void FiscalExamples()
        {
            Equal(1, FiscalCalendar.GetDate(new DateTime(2026, 10, 3), DayOfWeek.Saturday, FiscalWeekRule.FirstFullWeek, Month.October).Week);
            Equal(2, FiscalCalendar.GetDate(new DateTime(2026, 10, 3), DayOfWeek.Saturday, FiscalWeekRule.FirstDay, Month.October).Week);
            var january = FiscalCalendar.GetDate(new DateTime(2024, 12, 30), DayOfWeek.Monday, FiscalWeekRule.FirstFourDayWeek, Month.January);
            Equal(1, january.Week); Equal(2025, january.StartYear); Equal("25/26", january.YearLabel);
            var century = FiscalCalendar.GetDate(new DateTime(2099, 10, 15), DayOfWeek.Monday, FiscalWeekRule.FirstDay, Month.October);
            Equal("99/00", century.YearLabel);
            Equal(january.Week, FiscalCalendar.GetDate(new DateTime(2024, 12, 30, 23, 59, 59), DayOfWeek.Monday, FiscalWeekRule.FirstFourDayWeek, Month.January).Week);
        }

        private static void InvalidCalendarSettings()
        {
            Throws<ArgumentOutOfRangeException>(() => FiscalCalendar.GetDate(DateTime.Today, (DayOfWeek)9, FiscalWeekRule.FirstDay, Month.January));
            Throws<ArgumentOutOfRangeException>(() => FiscalCalendar.GetDate(DateTime.Today, DayOfWeek.Monday, (FiscalWeekRule)0, Month.January));
            Throws<ArgumentOutOfRangeException>(() => FiscalCalendar.GetDate(DateTime.Today, DayOfWeek.Monday, FiscalWeekRule.FirstDay, (Month)13));
            var settings = new AppSettings { StandardIcon = false, FiscalWeekIcon = false, CalendarWeekIcon = false };
            Throws<ArgumentException>(settings.Validate);
        }

        private static void TextContrast()
        {
            Equal(Color.White, IconColors.GetTextColor(Color.Black));
            Equal(Color.Black, IconColors.GetTextColor(Color.White));
            Equal(Color.Black, IconColors.GetTextColor(Color.FromArgb(128, 128, 128)));
            for (int r = 0; r <= 255; r += 17)
            for (int g = 0; g <= 255; g += 17)
            for (int b = 0; b <= 255; b += 17)
            {
                Color color = Color.FromArgb(r, g, b);
                Color foreground = IconColors.GetTextColor(color);
                double luminance = 0.2126 * Linear(r) + 0.7152 * Linear(g) + 0.0722 * Linear(b);
                double ratio = foreground == Color.Black ? (luminance + 0.05) / 0.05 : 1.05 / (luminance + 0.05);
                Equal(true, ratio >= 4.5, "Contrast " + color);
            }
        }
        private static double Linear(int channel) { double v = channel / 255.0; return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4); }

        private static void RefreshIntervals()
        {
            Equal(60000, RefreshSchedule.GetInterval(new DateTime(2026, 1, 1, 23, 0, 0)));
            Equal(1000, RefreshSchedule.GetInterval(new DateTime(2026, 1, 1, 23, 59, 59)));
            Equal(1, RefreshSchedule.GetInterval(new DateTime(2026, 1, 2).AddTicks(-1)));
            Equal(new DateTime(2026, 1, 1, 23, 0, 0), RefreshSchedule.GetHour(new DateTime(2026, 1, 1, 23, 59, 59)));
        }

        private static void WithDirectory(Action<string> test)
        {
            string directory = Path.Combine(Path.GetTempPath(), "CalApp-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try { test(directory); }
            finally { Directory.Delete(directory, true); }
        }

        private static void LegacyXml() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml");
            File.WriteAllText(path, "<ConfigDataObject><FirstFiscalMonth>April</FirstFiscalMonth><StandardIconColor>#123456</StandardIconColor><FiscalWeekIconForeColorAuto>false</FiscalWeekIconForeColorAuto><FiscalWeekIconForeColor>Red</FiscalWeekIconForeColor></ConfigDataObject>");
            var store = new SettingsStore(path);
            AppSettings settings = store.Load();
            Equal(Month.April, settings.FirstFiscalMonth);
            Equal(Color.FromArgb(0x12, 0x34, 0x56).ToArgb(), settings.StandardIconColor.ToArgb());
            Equal(Color.Red.ToArgb(), settings.FiscalWeekIconForeColor.ToArgb());
            Equal(false, settings.FiscalWeekIconForeColorAuto);
            Equal(true, settings.CalendarWeekIconForeColorAuto);
            Equal(null, store.LoadWarning);
        });

        private static void SettingsRoundTrip() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "nested", "Preferences.xml");
            var first = new SettingsStore(path); var second = new SettingsStore(path);
            first.Load(); second.Load();
            AppSettings settings = new AppSettings { FirstFiscalMonth = Month.April, CalendarWeekIconForeColor = Color.Magenta, StartWithWindows = false };
            second.Save(settings);
            second.Load(); Equal(Month.April, first.Load().FirstFiscalMonth);
            Equal(Color.Magenta.ToArgb(), first.Current.CalendarWeekIconForeColor.ToArgb());
            Equal(false, first.Current.StartWithWindows);
            Equal(true, DateTime.TryParse(first.Current.DateOfChange, out DateTime timestamp));
            AppSettings copy = first.Current; copy.FirstFiscalMonth = Month.December;
            Equal(Month.April, first.Current.FirstFiscalMonth);
            Equal(true, File.ReadAllText(path).Contains("<ConfigDataObject"));
        });

        private static void SettingsMigration() => WithDirectory(directory =>
        {
            string older = Path.Combine(directory, "1.0.0.0", "Preferences.xml");
            string newer = Path.Combine(directory, "2.0.0.0", "Preferences.xml");
            new SettingsStore(older).Save(new AppSettings { FirstFiscalMonth = Month.April });
            new SettingsStore(newer).Save(new AppSettings { FirstFiscalMonth = Month.July });
            string original = File.ReadAllText(newer);
            string path = Path.Combine(directory, "Preferences.xml");
            var migrated = new SettingsStore(path, directory);
            Equal(Month.July, migrated.Load().FirstFiscalMonth);
            migrated.Save(migrated.Current);
            Equal(original, File.ReadAllText(newer));
            Equal(Month.July, new SettingsStore(path).Load().FirstFiscalMonth);
        });

        private static void BackupRecovery() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml"); var store = new SettingsStore(path);
            store.Save(new AppSettings { FirstFiscalMonth = Month.April });
            store.Save(new AppSettings { FirstFiscalMonth = Month.July });
            File.WriteAllText(path, "<broken>");
            Equal(Month.April, store.Load().FirstFiscalMonth);
            Equal(true, store.LoadWarning != null);
            string backup = File.ReadAllText(path + ".bak");
            store.Save(store.Current);
            Equal(backup, File.ReadAllText(path + ".bak"));
            Equal(Month.April, new SettingsStore(path).Load().FirstFiscalMonth);
        });

        private static void CorruptSettings() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml"); var store = new SettingsStore(path);
            foreach (string xml in new[] { "<broken>", "<!DOCTYPE ConfigDataObject [<!ENTITY x SYSTEM 'file:///invalid'>]><ConfigDataObject><DateOfChange>&x;</DateOfChange></ConfigDataObject>" })
            {
                File.WriteAllText(path, xml);
                Equal(Month.October, store.Load().FirstFiscalMonth);
                Equal(true, store.LoadWarning != null);
            }
            File.WriteAllText(path, "<ConfigDataObject><StandardIcon>false</StandardIcon><FiscalWeekIcon>false</FiscalWeekIcon><CalendarWeekIcon>false</CalendarWeekIcon></ConfigDataObject>");
            Equal(true, store.Load().StandardIcon);
        });

        private static void FailedSave() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml"); var store = new SettingsStore(path);
            store.Save(new AppSettings()); string original = File.ReadAllText(path);
            var invalid = new AppSettings { FirstFiscalMonth = (Month)0 };
            Throws<ArgumentException>(() => store.Save(invalid));
            Equal(original, File.ReadAllText(path)); Equal(Month.October, store.Current.FirstFiscalMonth);
            using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                Throws<IOException>(() => store.Save(new AppSettings { FirstFiscalMonth = Month.April }));
            Equal(original, File.ReadAllText(path)); Equal(Month.October, store.Current.FirstFiscalMonth);
            Equal(0, Directory.GetFiles(directory, "*.tmp").Length);
        });

        private sealed class FakeStartup : IStartupRegistration
        {
            private string command;
            public bool Denied { get; set; }
            public string Command { get => command; set { if (Denied) throw new UnauthorizedAccessException("Denied by policy."); command = value; } }
        }

        private static void StartupRollback() => WithDirectory(directory =>
        {
            string blocker = Path.Combine(directory, "file"); File.WriteAllText(blocker, "block");
            var store = new SettingsStore(Path.Combine(blocker, "Preferences.xml"));
            var startup = new FakeStartup { Command = "old-command" };
            var controller = new SettingsController(store, startup, @"C:\Program Files\Calendar\CalendarWeekInfo.exe");
            int notifications = 0; controller.Saved += (sender, e) => notifications++;
            Throws<IOException>(() => controller.Apply(new AppSettings { FirstFiscalMonth = Month.April }));
            Equal("old-command", startup.Command); Equal(Month.October, controller.Settings.FirstFiscalMonth); Equal(0, notifications);
        });

        private static void StartupDenied() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml"); var startup = new FakeStartup { Denied = true };
            var controller = new SettingsController(new SettingsStore(path), startup, "app.exe");
            Throws<UnauthorizedAccessException>(() => controller.Apply(new AppSettings()));
            Equal(false, File.Exists(path)); Equal(Month.October, controller.Settings.FirstFiscalMonth);
        });

        private static void SuccessfulApply() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml"); var startup = new FakeStartup();
            var controller = new SettingsController(new SettingsStore(path), startup, @"C:\Program Files\Calendar\CalendarWeekInfo.exe");
            int notifications = 0; controller.Saved += (sender, e) => { notifications++; Equal(Month.April, controller.Settings.FirstFiscalMonth); };
            var pending = new AppSettings { FirstFiscalMonth = Month.April }; controller.Apply(pending);
            Equal("\"C:\\Program Files\\Calendar\\CalendarWeekInfo.exe\"", startup.Command);
            Equal(1, notifications); pending.FirstFiscalMonth = Month.July; Equal(Month.April, controller.Settings.FirstFiscalMonth);
        });

        private static T Field<T>(object instance, string name) => (T)instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        private static void Invoke(object instance, string name) => instance.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, new object[] { instance, EventArgs.Empty });

        private static void CalendarSelection() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            using (var form = new CalendarForm(controller))
            {
                form.EnsureHandleCreated();
                MonthCalendar calendar = Field<MonthCalendar>(form, "dateCalendar");
                DateTime selected = new DateTime(2024, 12, 30); calendar.SetDate(selected);
                controller.Apply(new AppSettings { FirstFiscalMonth = Month.January, FirstDayOfFiscalWeek = DayOfWeek.Monday, StartWithWindows = false });
                Equal(selected, calendar.SelectionStart);
                Equal("Calendar Week: 1", Field<Label>(form, "calendarWeekLabel").Text);
                Equal("Fiscal Week: 1", Field<Label>(form, "fiscalWeekLabel").Text);
                Equal("Fiscal Year: 25/26", Field<Label>(form, "fiscalYearLabel").Text);
            }
        });

        private static void PreferenceControls() => WithDirectory(directory =>
        {
            var store = new SettingsStore(Path.Combine(directory, "Preferences.xml"));
            store.Save(new AppSettings { FirstFiscalMonth = Month.July, FiscalWeekIconForeColor = Color.Magenta, StartWithWindows = false });
            var controller = new SettingsController(store, new FakeStartup(), "test.exe");
            using (var form = new PreferencesForm(controller))
            {
                Invoke(form, "PreferencesForm_Load");
                Field<ComboBox>(form, "comboBoxFirstFiscalMonth").SelectedItem = Month.April;
                Invoke(form, "ButtonReset_Click"); Equal(Month.July, Field<ComboBox>(form, "comboBoxFirstFiscalMonth").SelectedItem);
                Invoke(form, "ButtonDefault_Click"); Equal(Month.October, Field<ComboBox>(form, "comboBoxFirstFiscalMonth").SelectedItem);
                Equal(false, Field<Button>(form, "buttonFiscalWeekIconForeColor").Enabled);
                Invoke(form, "ButtonReset_Click");
                Field<CheckBox>(form, "checkBoxFiscalWeekAutoFore").Checked = true;
                Invoke(form, "ButtonApply_Click");
                Equal(Color.Magenta.ToArgb(), controller.Settings.FiscalWeekIconForeColor.ToArgb());
                Field<ComboBox>(form, "comboBoxFirstFiscalMonth").SelectedItem = Month.April;
                Invoke(form, "ButtonCancel_Click"); Equal(Month.July, controller.Settings.FirstFiscalMonth);
            }
        });

        private static void TrayLifecycle() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            var form = new CalendarForm(controller);
            form.EnsureHandleCreated(); Equal(true, form.IsHandleCreated); Equal(false, form.Visible);
            Timer timer = Field<Timer>(form, "refreshTimer"); Equal(true, timer.Enabled);
            NotifyIcon icon = Field<NotifyIcon>(form, "fiscalWeekTrayIcon");
            Equal(true, icon.Icon != null); form.Dispose(); Equal(false, timer.Enabled);
            Equal(false, icon.Visible); Equal(null, icon.Icon); form.Dispose();
            controller.Apply(new AppSettings { StartWithWindows = false });
        });

        private static void ApplicationLifetime() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            CalendarForm form;
            Timer refreshTimer;
            using (var context = new TrayApplicationContext(controller))
            {
                form = Field<CalendarForm>(context, "calendarForm");
                refreshTimer = Field<Timer>(form, "refreshTimer");
                int exits = 0; context.ThreadExit += (sender, e) => exits++;
                Exception callbackError = null;
                using (var quitTimer = new Timer { Interval = 30 })
                {
                    quitTimer.Tick += (sender, e) =>
                    {
                        quitTimer.Stop();
                        try
                        {
                            form.Close();
                            Equal(false, form.IsDisposed); Equal(false, form.Visible); Equal(0, exits);
                            Application.Exit();
                            Equal(1, exits);
                        }
                        catch (Exception ex) { callbackError = ex; Application.ExitThread(); }
                    };
                    quitTimer.Start();
                    Application.Run(context);
                }
                if (callbackError != null) throw callbackError;
            }
            Equal(true, form.IsDisposed);
            Equal(false, refreshTimer.Enabled);
        });
    }
}
