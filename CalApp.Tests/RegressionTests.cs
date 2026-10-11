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
using CalApp.Localization;
using System.Globalization;
using System.Resources;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CalApp.Updates;
using Timer = System.Windows.Forms.Timer;

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
            Run("Installation preserves preferences and synchronizes startup", InstallationStartup);
            Run("Tray identities survive version changes and isolate portable copies", TrayIdentities);
            Run("System language selection, manual overrides and complete language packs", LanguagePacks);
            Run("Language defaults and persistence are compatible with existing settings", LanguageSettings);
            Run("Semantic release versions compare stable, prerelease and numeric identifiers", ReleaseVersions);
            Run("Update lookup selects the right release and download", UpdateSelection);
            Run("Update lookup handles private repositories, rate limits and cancellation", UpdateFailures);
            Run("Settings transfer rejects unsafe files and protects active preferences", TransferFiles);
            if (args.Contains("--ui"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Run("Calendar selection remains consistent after Apply", CalendarSelection);
                Run("Preference cancel, defaults, reset and manual colors", PreferenceControls);
                Run("Hidden tray form creates handles and releases resources", TrayLifecycle);
                Run("Window close hides the app; application exit ends its context", ApplicationLifetime);
                Run("Tray refresh and preferences do not create a calendar window", LazyCalendar);
                Run("Application exit works before the calendar is opened", ExitWithoutCalendar);
                Run("Week number icons retain their original 16-pixel size", IconSize);
                Run("Native tray registration, Explorer recovery and input callbacks", NativeTray);
                Run("Installer closes the lifetime owner without creating a calendar", InstallerShutdown);
                Run("Apply switches language immediately and preserves calendar selection", LanguageSwitch);
                Run("Today button and keyboard shortcuts use the application clock", CalendarNavigation);
                Run("Fiscal preview follows pending rules and fiscal-year boundaries", FiscalPreview);
                Run("Imported settings remain pending; export does not apply changes", TransferControls);
                Run("Update dialog handles results, private access and closing during lookup", UpdateDialog);
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
            using (var context = new TrayApplicationContext(controller))
            {
                Console.WriteLine("Tray startup without a calendar: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                CalendarForm form = context.GetCalendar();
                Console.WriteLine("First calendar creation: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                var unused = form.Handle;
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
            using (var context = new TrayApplicationContext(controller))
            {
                CalendarForm form = context.GetCalendar();
                MonthCalendar calendar = Field<MonthCalendar>(form, "dateCalendar");
                DateTime selected = new DateTime(2024, 12, 30); calendar.SetDate(selected);
                controller.Apply(new AppSettings { FirstFiscalMonth = Month.January, FirstDayOfFiscalWeek = DayOfWeek.Monday, StartWithWindows = false });
                Equal(selected, calendar.SelectionStart);
                Equal(AppText.Format("CalendarWeek", 1), Field<Label>(form, "calendarWeekLabel").Text);
                Equal(AppText.Format("FiscalWeek", 1), Field<Label>(form, "fiscalWeekLabel").Text);
                Equal(AppText.Format("FiscalYear", "25/26"), Field<Label>(form, "fiscalYearLabel").Text);
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
            var context = new TrayApplicationContext(controller);
            Equal(null, Field<CalendarForm>(context, "calendarForm"));
            Form owner = Field<ApplicationLifetimeWindow>(context, "lifetimeWindow");
            Equal(true, owner.IsHandleCreated); Equal(false, owner.Visible);
            Timer timer = Field<Timer>(context, "refreshTimer"); Equal(true, timer.Enabled);
            TrayIcons icons = Field<TrayIcons>(context, "trayIcons");
            PersistentTrayIcon icon = Field<PersistentTrayIcon>(icons, "fiscalWeekTrayIcon");
            Equal(true, icon.Icon != null); context.Dispose(); Equal(false, timer.Enabled);
            Equal(false, icon.Visible); Equal(null, icon.Icon); Equal(true, owner.IsDisposed); context.Dispose();
            controller.Apply(new AppSettings { StartWithWindows = false });
        });

        private static void ApplicationLifetime() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            CalendarForm form;
            Timer refreshTimer;
            using (var context = new TrayApplicationContext(controller))
            {
                form = context.GetCalendar();
                var unused = form.Handle;
                refreshTimer = Field<Timer>(context, "refreshTimer");
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

        private static void LazyCalendar() => WithDirectory(directory =>
        {
            DateTime now = new DateTime(2025, 12, 28, 23, 59, 59);
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            using (var context = new TrayApplicationContext(controller, () => now))
            {
                Equal(null, Field<CalendarForm>(context, "calendarForm"));
                TrayIcons icons = Field<TrayIcons>(context, "trayIcons");
                PersistentTrayIcon icon = Field<PersistentTrayIcon>(icons, "calendarWeekTrayIcon");
                Equal(AppText.Format("CalendarWeekTip", 52), icon.Text);
                now = now.AddSeconds(2);
                Invoke(context, "RefreshTimer_Tick");
                Equal(AppText.Format("CalendarWeekTip", 1), icon.Text);
                controller.Apply(new AppSettings { StartWithWindows = false, StandardIcon = false, CalendarWeekIcon = false });
                Equal(false, Field<PersistentTrayIcon>(icons, "standardTrayIcon").Visible);
                Equal(false, icon.Visible);
                Equal(true, Field<PersistentTrayIcon>(icons, "fiscalWeekTrayIcon").Visible);
                using (var dialog = new PreferencesForm(controller)) Invoke(dialog, "PreferencesForm_Load");
                Equal(null, Field<CalendarForm>(context, "calendarForm"));
                CalendarForm calendar = context.GetCalendar();
                Equal(calendar, context.GetCalendar()); Equal(false, calendar.Visible);
            }
        });

        private static void ExitWithoutCalendar() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            using (var context = new TrayApplicationContext(controller))
            using (var quitTimer = new Timer { Interval = 30 })
            {
                Equal(null, Field<CalendarForm>(context, "calendarForm"));
                int exits = 0; context.ThreadExit += (sender, e) => exits++;
                quitTimer.Tick += (sender, e) => { quitTimer.Stop(); Application.Exit(); };
                quitTimer.Start(); Application.Run(context);
                Equal(1, exits); Equal(null, Field<CalendarForm>(context, "calendarForm"));
            }
        });

        private static void InstallationStartup() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml");
            var store = new SettingsStore(path);
            store.Save(new AppSettings { FirstFiscalMonth = Month.July, StartWithWindows = true });
            string original = File.ReadAllText(path);
            var startup = new FakeStartup { Command = "old-portable-path" };
            var controller = new SettingsController(store, startup, TrayIconIdentity.InstalledExecutable);
            controller.SynchronizeStartup();
            Equal("\"" + TrayIconIdentity.InstalledExecutable + "\"", startup.Command);
            Equal(original, File.ReadAllText(path));
            store.Save(new AppSettings { StartWithWindows = false });
            controller = new SettingsController(store, startup, "new.exe");
            original = File.ReadAllText(path);
            controller.SynchronizeStartup();
            Equal(null, startup.Command); Equal(original, File.ReadAllText(path));
            File.Delete(path + ".bak"); File.WriteAllText(path, "<broken>");
            startup.Command = "unchanged";
            controller = new SettingsController(store, startup, "new.exe");
            Throws<InvalidOperationException>(controller.SynchronizeStartup);
            Equal("unchanged", startup.Command);
            startup.Denied = true;
            File.WriteAllText(path, original);
            controller = new SettingsController(store, startup, "new.exe");
            Throws<UnauthorizedAccessException>(controller.SynchronizeStartup);
            Equal(original, File.ReadAllText(path));
        });

        private static void TrayIdentities()
        {
            Guid[] ids = { TrayIconIdentity.Standard, TrayIconIdentity.FiscalWeek, TrayIconIdentity.CalendarWeek };
            Equal(3, ids.Distinct().Count());
            foreach (Guid id in ids)
            {
                Equal(id, TrayIconIdentity.ForExecutable(id, TrayIconIdentity.InstalledExecutable));
                Equal(id, TrayIconIdentity.ForExecutable(id, TrayIconIdentity.InstalledExecutable.ToUpperInvariant()));
                Guid portable = TrayIconIdentity.ForExecutable(id, @"C:\Portable\CalendarWeekInfo.exe");
                Equal(false, portable == id);
                Equal(portable, TrayIconIdentity.ForExecutable(id, @"c:\portable\CalendarWeekInfo.exe"));
                Equal(false, portable == TrayIconIdentity.ForExecutable(id, @"C:\Other\CalendarWeekInfo.exe"));
            }
        }

        private static void NativeTray()
        {
            using (Icon image = IconFactory.CreateText("41", Color.Yellow, Color.Black))
            using (var icon = new PersistentTrayIcon(Guid.NewGuid()))
            {
                icon.Icon = image; icon.Text = "Calendar week 41"; icon.Visible = true;
                Equal(true, icon.Registered);
                icon.Text = "Calendar week 42";
                Equal(true, icon.Registered);
                // Remove the native registration to simulate Explorer losing it. Do not restart
                // the user's Explorer or change the app's desired visibility/identity.
                object nativeData = typeof(PersistentTrayIcon).GetMethod("Data", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(icon, new object[] { 0x20U });
                Equal(true, (bool)typeof(PersistentTrayIcon).GetMethod("ShellNotifyIcon", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[] { 2U, nativeData }));
                Guid identity = icon.Identity;
                icon.RestoreAfterExplorerRestart();
                Equal(true, icon.Registered); Equal(identity, icon.Identity);
                icon.Visible = false; Equal(false, icon.Registered);
                icon.Visible = true; Equal(true, icon.Registered);
                int clicks = 0; icon.MouseClick += (sender, e) => { Equal(MouseButtons.Left, e.Button); clicks++; };
                Message message = Message.Create(icon.Handle, 0x8001, IntPtr.Zero, new IntPtr(0x400));
                object[] arguments = { message };
                typeof(PersistentTrayIcon).GetMethod("WndProc", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(icon, arguments);
                Equal(1, clicks);
                message = Message.Create(icon.Handle, 0x8001, IntPtr.Zero, new IntPtr(0x401));
                typeof(PersistentTrayIcon).GetMethod("WndProc", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(icon, new object[] { message });
                Equal(2, clicks);
                icon.Dispose(); Equal(IntPtr.Zero, icon.Handle); Equal(false, icon.Registered);
                Throws<ObjectDisposedException>(() => icon.Visible = true);
            }
        }

        private static void InstallerShutdown() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            Form owner;
            using (var context = new TrayApplicationContext(controller))
            using (var closeTimer = new Timer { Interval = 30 })
            {
                owner = Field<ApplicationLifetimeWindow>(context, "lifetimeWindow");
                Equal("CalApp.UpdateOwner", owner.Text);
                int exits = 0; context.ThreadExit += (sender, e) => exits++;
                closeTimer.Tick += (sender, e) => { closeTimer.Stop(); owner.Close(); };
                closeTimer.Start(); Application.Run(context);
                Equal(1, exits);
                Equal(null, Field<CalendarForm>(context, "calendarForm"));
            }
            Equal(true, owner.IsDisposed);
        });

        private static void LanguagePacks()
        {
            Equal("de-DE", AppText.Resolve(AppLanguage.System, CultureInfo.GetCultureInfo("de-CH")).Name);
            Equal("de-DE", AppText.Resolve(AppLanguage.System, CultureInfo.GetCultureInfo("de-AT")).Name);
            Equal("en-GB", AppText.Resolve(AppLanguage.System, CultureInfo.GetCultureInfo("en-US")).Name);
            Equal("en-GB", AppText.Resolve(AppLanguage.System, CultureInfo.GetCultureInfo("fr-FR")).Name);
            Equal("en-GB", AppText.Resolve(AppLanguage.English, CultureInfo.GetCultureInfo("de-DE")).Name);
            Equal("de-DE", AppText.Resolve(AppLanguage.German, CultureInfo.GetCultureInfo("en-US")).Name);
            Throws<ArgumentOutOfRangeException>(() => AppText.Resolve((AppLanguage)999, CultureInfo.InvariantCulture));
            var resources = new ResourceManager("CalApp.Localization.Strings", typeof(AppLanguage).Assembly);
            ResourceSet english = resources.GetResourceSet(CultureInfo.InvariantCulture, true, false);
            ResourceSet german = resources.GetResourceSet(CultureInfo.GetCultureInfo("de"), true, false);
            Equal(false, german == null);
            int count = 0;
            foreach (DictionaryEntry entry in english)
            {
                string translated = german.GetString((string)entry.Key);
                Equal(false, string.IsNullOrWhiteSpace(translated), "German key: " + entry.Key);
                // Formatting must be valid in both packs, including multiline messages.
                string.Format(CultureInfo.InvariantCulture, (string)entry.Value, "0", "1", "2");
                string.Format(CultureInfo.InvariantCulture, translated, "0", "1", "2");
                count++;
            }
            Equal(count, german.Cast<DictionaryEntry>().Count());
        }

        private static void LanguageSettings() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml");
            File.WriteAllText(path, "<ConfigDataObject><FirstFiscalMonth>July</FirstFiscalMonth></ConfigDataObject>");
            var store = new SettingsStore(path);
            Equal(AppLanguage.System, store.Load().Language);
            foreach (AppLanguage language in Enum.GetValues(typeof(AppLanguage)))
            {
                AppSettings settings = store.Current; settings.Language = language; store.Save(settings);
                Equal(language, new SettingsStore(path).Load().Language);
                Equal(Month.July, store.Current.FirstFiscalMonth);
            }
            Throws<ArgumentException>(() => new AppSettings { Language = (AppLanguage)999 }.Validate());
        });

        private static void LanguageSwitch() => WithDirectory(directory =>
        {
            string path = Path.Combine(directory, "Preferences.xml");
            var store = new SettingsStore(path);
            store.Save(new AppSettings { Language = AppLanguage.English, StartWithWindows = false });
            var controller = new SettingsController(store, new FakeStartup(), "test.exe");
            using (var context = new TrayApplicationContext(controller))
            using (var preferences = new PreferencesForm(controller))
            {
                CalendarForm calendar = context.GetCalendar();
                DateTime selected = new DateTime(2024, 12, 30); calendar.SelectDate(selected);
                Invoke(preferences, "PreferencesForm_Load");
                ComboBox languages = Field<ComboBox>(preferences, "comboBoxLanguage");
                foreach (AppLanguage language in new[] { AppLanguage.German, AppLanguage.English })
                {
                    languages.SelectedItem = language;
                    Invoke(preferences, "ButtonApply_Click");
                    bool german = language == AppLanguage.German;
                    Equal(german ? "Einstellungen" : "Preferences", preferences.Text);
                    Equal(german ? "Datei" : "File", Field<ToolStripMenuItem>(calendar, "fileMenu").Text);
                    Equal(german ? "Kalenderwoche: 1" : "Calendar week: 1", Field<Label>(calendar, "calendarWeekLabel").Text);
                    Equal(selected, calendar.SelectedDate);
                    Equal(language, new SettingsStore(path).Load().Language);
                    ComboBox months = Field<ComboBox>(preferences, "comboBoxFirstFiscalMonth");
                    ComboBox days = Field<ComboBox>(preferences, "comboBoxFirstFiscalWeekday");
                    Equal(german ? "Oktober" : "October", months.GetItemText(Month.October));
                    Equal(german ? "Samstag" : "Saturday", days.GetItemText(DayOfWeek.Saturday));
                    CheckBox contrast = Field<CheckBox>(preferences, "checkBoxFiscalWeekAutoFore");
                    Button text = Field<Button>(preferences, "buttonFiscalWeekIconForeColor");
                    Equal(true, contrast.Right < text.Left);
                    Button cancel = Field<Button>(preferences, "buttonCancel");
                    Equal(true, cancel.Width >= TextRenderer.MeasureText(cancel.Text, cancel.Font).Width + 12);
                    TrayIcons icons = Field<TrayIcons>(context, "trayIcons");
                    var menu = Field<ContextMenuStrip>(icons, "trayMenu");
                    Equal(german ? "Beenden" : "Exit", menu.Items[0].Text);
                    using (var about = new AboutForm()) Equal(german ? "Kontakt" : "Contact", Field<LinkLabel>(about, "contactLink").Text);
                }
                languages.SelectedItem = AppLanguage.German;
                Invoke(preferences, "ButtonReset_Click"); Equal(AppLanguage.English, languages.SelectedItem);
                Invoke(preferences, "ButtonDefault_Click"); Equal(AppLanguage.System, languages.SelectedItem);
                languages.SelectedItem = AppLanguage.German;
                Invoke(preferences, "ButtonCancel_Click"); Equal(AppLanguage.English, controller.Settings.Language);
                Equal("en", AppText.Culture.TwoLetterISOLanguageName);
                var pending = controller.Settings; pending.Language = AppLanguage.German;
                using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    Throws<IOException>(() => controller.Apply(pending));
                Equal(AppLanguage.English, controller.Settings.Language);
                Equal("en", AppText.Culture.TwoLetterISOLanguageName);
            }
        });

        private static ReleaseVersion VersionOf(string text)
        {
            Equal(true, ReleaseVersion.TryParse(text, out ReleaseVersion version), text);
            return version;
        }

        private static void ReleaseVersions()
        {
            string[] sequence = { "1.0.0-alpha", "1.0.0-alpha.1", "1.0.0-alpha.beta", "1.0.0-beta", "1.0.0-beta.2", "1.0.0-beta.11", "1.0.0-rc.1", "1.0.0" };
            for (int i = 1; i < sequence.Length; i++) Equal(true, VersionOf(sequence[i]).CompareTo(VersionOf(sequence[i - 1])) > 0);
            Equal(true, VersionOf("1.10.0").CompareTo(VersionOf("1.9.9")) > 0);
            Equal(true, VersionOf("1.2.0-rc.10").CompareTo(VersionOf("1.2.0-rc.2")) > 0);
            Equal(0, VersionOf("v1.2.0+build.7").CompareTo(VersionOf("1.2.0+other")));
            Equal(true, VersionOf("1.2.0-999999999999999999999").CompareTo(VersionOf("1.2.0-99999999999999999999")) > 0);
            foreach (string invalid in new[] { null, "1.2", "01.2.0", "1.2.0-01", "1.2.0-", "1.2.0+", "garbage" })
                Equal(false, ReleaseVersion.TryParse(invalid, out ReleaseVersion ignored));
        }

        private sealed class ReleaseServer : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond;
            internal ReleaseServer(string json, HttpStatusCode status = HttpStatusCode.OK)
                : this((request, cancellation) => Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(json) })) { }
            internal ReleaseServer(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) { this.respond = respond; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
            {
                Equal("api.github.com", request.RequestUri.Host);
                Equal(false, request.Headers.Contains("Authorization"));
                Equal(true, request.Headers.UserAgent.Any());
                return respond(request, cancellation);
            }
        }

        private static UpdateResult Lookup(string current, string json, HttpStatusCode status = HttpStatusCode.OK)
        {
            using (var checker = new UpdateChecker(new HttpClient(new ReleaseServer(json, status))))
                return checker.CheckAsync(VersionOf(current), CancellationToken.None).GetAwaiter().GetResult();
        }

        private static void UpdateSelection()
        {
            const string json = "[{\"tag_name\":\"v2.0.0-rc.1\",\"prerelease\":true},{\"tag_name\":\"v3.0.0\",\"draft\":true},{\"tag_name\":\"invalid\"},{\"tag_name\":\"v1.2.0\",\"html_url\":\"https://github.com/Sigibert/CalApp/releases/tag/v1.2.0\",\"assets\":[{\"name\":\"CalendarWeekInfo-1.2.0.zip\",\"browser_download_url\":\"https://github.com/Sigibert/CalApp/releases/download/v1.2.0/CalendarWeekInfo-1.2.0.zip\"},{\"name\":\"CalApp-Setup-1.2.0.exe\",\"browser_download_url\":\"https://github.com/Sigibert/CalApp/releases/download/v1.2.0/CalApp-Setup-1.2.0.exe\"}]},{\"tag_name\":\"v1.1.0\"}]";
            UpdateResult stable = Lookup("1.1.0", json);
            Equal("1.2.0", stable.Version.ToString());
            Equal(true, stable.Download.AbsolutePath.EndsWith("CalApp-Setup-1.2.0.exe"));
            Equal("2.0.0-rc.1", Lookup("1.2.0-rc.4", json).Version.ToString());
            Equal(null, Lookup("1.2.0", "[]").Version);
            Equal(UpdateChecker.ReleasesPage, Lookup("1.1.0", "[{\"tag_name\":\"1.2.0\",\"html_url\":\"https://example.com/evil\"}]").Download);
            foreach (string invalid in new[] { "http://github.com/Sigibert/CalApp/releases", "https://github.com.evil.test/Sigibert/CalApp/releases", "https://user@github.com/Sigibert/CalApp/releases", "https://github.com/other/repo/releases", "https://github.com:444/Sigibert/CalApp/releases", "file:///C:/evil.exe" })
                Equal(null, UpdateChecker.SafeReleaseUri(invalid));
        }

        private static void UpdateFailures()
        {
            foreach (var failure in new[] { new { Status = HttpStatusCode.NotFound, Key = "UpdatePrivate" }, new { Status = HttpStatusCode.Forbidden, Key = "UpdateLimited" }, new { Status = HttpStatusCode.InternalServerError, Key = "UpdateUnavailable" } })
            {
                try { Lookup("1.2.0", "{}", failure.Status); throw new Exception("Expected update failure"); }
                catch (UpdateLookupException ex) { Equal(failure.Key, ex.MessageKey); }
            }
            using (var cancellation = new CancellationTokenSource())
            using (var checker = new UpdateChecker(new HttpClient(new ReleaseServer(async (request, token) =>
            {
                await Task.Delay(Timeout.Infinite, token).ConfigureAwait(false);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }))))
            {
                Task<UpdateResult> lookup = checker.CheckAsync(VersionOf("1.2.0"), cancellation.Token);
                cancellation.Cancel();
                Throws<OperationCanceledException>(() => lookup.GetAwaiter().GetResult());
            }
            Throws<System.Runtime.Serialization.SerializationException>(() => Lookup("1.2.0", "not JSON"));
        }

        private static void TransferFiles() => WithDirectory(directory =>
        {
            string active = Path.Combine(directory, "Preferences.xml");
            var store = new SettingsStore(active);
            store.Save(new AppSettings { FirstFiscalMonth = Month.April, StartWithWindows = false });
            string original = File.ReadAllText(active);
            string exported = Path.Combine(directory, "CalApp-Settings.xml");
            var pending = new AppSettings { FirstFiscalMonth = Month.July, Language = AppLanguage.German, FiscalWeekIconForeColor = Color.Magenta, StartWithWindows = true };
            SettingsTransfer.Export(exported, pending, active);
            AppSettings imported = SettingsTransfer.Import(exported);
            Equal(Month.July, imported.FirstFiscalMonth); Equal(AppLanguage.German, imported.Language);
            Equal(Color.Magenta.ToArgb(), imported.FiscalWeekIconForeColor.ToArgb()); Equal(true, imported.StartWithWindows);
            Equal(original, File.ReadAllText(active));
            Throws<ArgumentException>(() => SettingsTransfer.Export(active, pending, active));
            Throws<ArgumentException>(() => SettingsTransfer.Export(active + ".bak", pending, active));
            foreach (string unsafeXml in new[] { "<broken>", "<!DOCTYPE ConfigDataObject [<!ENTITY x SYSTEM 'file:///invalid'>]><ConfigDataObject/>", "<ConfigDataObject><Language>Invalid</Language></ConfigDataObject>", "<ConfigDataObject><StandardIcon>false</StandardIcon><FiscalWeekIcon>false</FiscalWeekIcon><CalendarWeekIcon>false</CalendarWeekIcon></ConfigDataObject>" })
            {
                File.WriteAllText(exported, unsafeXml);
                bool rejected = false;
                try { SettingsTransfer.Import(exported); }
                catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException || ex is System.Xml.XmlException) { rejected = true; }
                Equal(true, rejected); Equal(original, File.ReadAllText(active));
            }
            File.WriteAllText(exported, "<ConfigDataObject><DateOfChange>" + new string('x', 1024 * 1024) + "</DateOfChange></ConfigDataObject>");
            Throws<InvalidOperationException>(() => SettingsTransfer.Import(exported));
        });

        private static void CalendarNavigation() => WithDirectory(directory =>
        {
            DateTime now = new DateTime(2026, 10, 11, 10, 20, 0);
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            using (var context = new TrayApplicationContext(controller, () => now))
            {
                CalendarForm calendar = context.GetCalendar();
                calendar.SelectDate(new DateTime(2024, 1, 1));
                Invoke(calendar, "TodayButton_Click"); Equal(now.Date, calendar.SelectedDate);
                now = now.AddDays(1);
                var processKey = typeof(CalendarForm).GetMethod("ProcessCmdKey", BindingFlags.Instance | BindingFlags.NonPublic);
                Message message = Message.Create(IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
                Equal(true, (bool)processKey.Invoke(calendar, new object[] { message, Keys.Control | Keys.T }));
                Equal(now.Date, calendar.SelectedDate);
                Equal(AppText.Format("CalendarWeek", IsoCalendar.GetWeek(now)), Field<Label>(calendar, "calendarWeekLabel").Text);
                Equal(true, (bool)processKey.Invoke(calendar, new object[] { message, Keys.Escape }));
                Equal(false, calendar.IsDisposed);
            }
        });

        private static void FiscalPreview() => WithDirectory(directory =>
        {
            var controller = new SettingsController(new SettingsStore(Path.Combine(directory, "Preferences.xml")), new FakeStartup(), "test.exe");
            DateTime now = new DateTime(2026, 10, 11);
            using (var preferences = new PreferencesForm(controller, () => now))
            {
                Invoke(preferences, "PreferencesForm_Load");
                var rule = Field<ComboBox>(preferences, "comboBoxFiscalWeekRule");
                rule.SelectedItem = FiscalWeekRule.FirstFullWeek;
                Equal(AppText.Format("FiscalPreview", "26/27", new DateTime(2026, 10, 3).ToString("d", AppText.Culture), new DateTime(2026, 10, 9).ToString("d", AppText.Culture)), Field<Label>(preferences, "fiscalPreview").Text);
                rule.SelectedItem = FiscalWeekRule.FirstDay;
                Equal(true, Field<Label>(preferences, "fiscalPreview").Text.Contains(new DateTime(2026, 9, 26).ToString("d", AppText.Culture)));
                Equal(FiscalWeekRule.FirstFourDayWeek, controller.Settings.FiscalWeekRule);
                now = new DateTime(2024, 12, 30);
                Field<ComboBox>(preferences, "comboBoxFirstFiscalMonth").SelectedItem = Month.January;
                Field<ComboBox>(preferences, "comboBoxFirstFiscalWeekday").SelectedItem = DayOfWeek.Monday;
                rule.SelectedItem = FiscalWeekRule.FirstFourDayWeek;
                Equal(true, Field<Label>(preferences, "fiscalPreview").Text.Contains("25/26"));
                Equal(true, Field<Label>(preferences, "fiscalPreview").Text.Contains(now.ToString("d", AppText.Culture)));
            }
        });

        private static void TransferControls() => WithDirectory(directory =>
        {
            string active = Path.Combine(directory, "Preferences.xml"), imported = Path.Combine(directory, "import.xml"), exported = Path.Combine(directory, "export.xml");
            var store = new SettingsStore(active); store.Save(new AppSettings { FirstFiscalMonth = Month.April, Language = AppLanguage.English, StartWithWindows = false });
            new SettingsStore(imported).Save(new AppSettings { FirstFiscalMonth = Month.July, Language = AppLanguage.German, FiscalWeekIconForeColor = Color.Magenta, StartWithWindows = true });
            string original = File.ReadAllText(active);
            var startup = new FakeStartup();
            var controller = new SettingsController(store, startup, "test.exe");
            using (var preferences = new PreferencesForm(controller))
            {
                Invoke(preferences, "PreferencesForm_Load");
                preferences.ImportFile(imported);
                Equal(Month.July, preferences.ReadPendingSettings().FirstFiscalMonth);
                Equal(Month.April, controller.Settings.FirstFiscalMonth); Equal(null, startup.Command);
                preferences.ExportFile(exported);
                Equal(Month.July, SettingsTransfer.Import(exported).FirstFiscalMonth); Equal(original, File.ReadAllText(active));
                Invoke(preferences, "ButtonCancel_Click"); Equal(original, File.ReadAllText(active));
            }
            using (var preferences = new PreferencesForm(controller))
            {
                Invoke(preferences, "PreferencesForm_Load"); preferences.ImportFile(imported);
                Invoke(preferences, "ButtonApply_Click");
                Equal(Month.July, controller.Settings.FirstFiscalMonth); Equal(AppLanguage.German, controller.Settings.Language);
                Equal("\"test.exe\"", startup.Command);
                Equal(Color.Magenta.ToArgb(), controller.Settings.FiscalWeekIconForeColor.ToArgb());
            }
            AppText.UseLanguage(AppLanguage.English);
        });

        private static void PumpTask(Task task)
        {
            var timer = Stopwatch.StartNew();
            while (!task.IsCompleted && timer.ElapsedMilliseconds < 5000) { Application.DoEvents(); Thread.Sleep(1); }
            if (!task.IsCompleted) throw new Exception("UI task did not finish.");
            task.GetAwaiter().GetResult();
        }

        private static void UpdateDialog()
        {
            using (var form = new UpdateForm(new UpdateChecker(new HttpClient(new ReleaseServer("[{\"tag_name\":\"v9.0.0\"}]"))), "1.2.0"))
            {
                PumpTask(form.CheckAsync());
                Equal(true, Field<Label>(form, "statusLabel").Text.Contains("9.0.0"));
                Equal(true, Field<Button>(form, "checkButton").Enabled);
            }
            using (var form = new UpdateForm(new UpdateChecker(new HttpClient(new ReleaseServer("{}", HttpStatusCode.NotFound))), "1.2.0"))
            {
                PumpTask(form.CheckAsync());
                Equal(AppText.Get("UpdatePrivate"), Field<Label>(form, "statusLabel").Text);
                Equal(UpdateChecker.ReleasesPage, Field<Uri>(form, "download"));
                Equal(true, Field<Button>(form, "checkButton").Enabled);
            }
            var waiting = new UpdateForm(new UpdateChecker(new HttpClient(new ReleaseServer(async (request, cancellation) =>
            {
                await Task.Delay(Timeout.Infinite, cancellation).ConfigureAwait(false);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }))), "1.2.0");
            Task check = waiting.CheckAsync(); waiting.Dispose(); PumpTask(check); waiting.Dispose();
            Equal(true, waiting.IsDisposed);
        }

        private static void IconSize()
        {
            using (Icon icon = IconFactory.CreateText("41", Color.Yellow, Color.Black)) Equal(new Size(16, 16), icon.Size);
        }
    }
}
