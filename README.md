# WeekTrayInfo

**Kalender- und Geschäftswochen auf einen Blick.**

*Calendar and fiscal weeks at a glance.*

A lightweight Windows system-tray app that shows the current **calendar week** (ISO 8601)
and **fiscal week** at a glance, and opens a small calendar popup on click.

It sits quietly in the notification area as one or more tiny icons that display the
week number, and refreshes itself automatically at the top of every hour and at midnight.

## Features

- **Tray icons** for the ISO-8601 calendar week and a configurable fiscal week — each a
  small number badge you can colour to taste.
- **Popup panel** (left-click a tray icon) with the calendar week, fiscal week, fiscal year,
  and a month calendar for looking up any date. The calendar window is created only
  on first use; tray icons and preferences work without opening it.
- **Configurable fiscal calendar** — first fiscal month, first day of the week, and the
  fiscal-week rule (how many days of a week must fall in the first month to count as week 1).
- **Per-icon colours** — pick a background per icon; the text colour is either
  auto-contrasted for you or picked manually.
- **Start with Windows** — optional autostart via the current-user `Run` key.
- **English and German** — automatic selection from the Windows display language,
  with a saved manual override in Preferences.
- **Fiscal-year preview** — see the year start and first-week dates before applying a rule.
- **Import/export** — transfer calendar rules, colors, language and startup preferences between computers.
- **Update check** — compare published GitHub releases and open the matching setup or ZIP download.
- **Auto-refresh** — updates on the hour and at midnight without any interaction.
  A one-minute heartbeat also catches wake from sleep and clock changes.

## Requirements

- Windows
- The installer supports Windows 10 and later.
- .NET Framework 4.7.2 or a compatible later .NET Framework runtime
- For development: Visual Studio with **.NET desktop development** and the
  **.NET Framework 4.7.2 targeting pack**

## Building

Open `WeekTrayInfo.sln` in Visual Studio and build, or from a developer prompt:

```
msbuild WeekTrayInfo\WeekTrayInfo.csproj /t:Rebuild /p:Configuration=Release
```

The executable is produced at `WeekTrayInfo\bin\Release\WeekTrayInfo.exe`.

To build the solution and run the regression tests from PowerShell:

```powershell
.\build.ps1
.\build.ps1 -UiTests -Package
.\build.ps1 -UiTests -Package -Installer
```

If PowerShell's local execution policy blocks the script, you can run this trusted
repository script with a policy override limited to the new PowerShell process:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\build.ps1 -UiTests -Package
```

The script locates Visual Studio's MSBuild automatically. Tests need no external
NuGet packages. `-UiTests` additionally exercises WinForms controls and tray cleanup
on a Windows desktop; it briefly creates tray icons but does not edit your personal
preferences or Windows startup entries. `-Package` creates a ZIP under `artifacts`
with the application, configuration file, documentation, and license.

`-Installer` also produces `artifacts\WeekTrayInfo-Setup-<version>.exe`. It requires
Inno Setup 6 or newer; use `-InnoSetupCompiler <path-to-ISCC.exe>` if necessary.
The installer version comes from the same `Version.props` as the app and ZIP.
Native tray-registration tests require access to the actual Windows desktop/Explorer;
run `-UiTests` in your normal user session, not an isolated sandbox desktop.

After building the release, `installer\Test-Installer.ps1` runs a real install,
update and uninstall under an isolated artifacts folder with a separate test AppId.
It skips app launch, shortcuts and startup synchronization, checks program-file
replacement and verifies that personal preferences and autostart are unchanged.
Pass `-InnoSetupCompiler <path-to-ISCC.exe>` when needed. Test logs remain in artifacts.

You can also run the test executable after building `WeekTrayInfo.sln`:

```powershell
.\WeekTrayInfo.Tests\bin\Release\WeekTrayInfo.Tests.exe --ui
```

To measure startup phases in an isolated process:

```powershell
.\WeekTrayInfo.Tests\bin\Release\WeekTrayInfo.Tests.exe --startup-timing
```

The runner returns a nonzero exit code on failure. Tests cover a complete 400-year
Gregorian cycle for ISO weeks, every fiscal month/weekday/week-rule combination,
settings migration and recovery, save failures, automatic contrast, and UI behavior.

## Usage

### Installation and updates

Run `WeekTrayInfo-Setup-<version>.exe`. Setup installs for the current user without
administrator rights at `%LOCALAPPDATA%\Programs\WeekTrayInfo\WeekTrayInfo.exe`.
Source, solution, executable, installer, settings and internal identities all use
WeekTrayInfo. Source and releases are at `https://github.com/Sigibert/WeekTrayInfo`.
Public downloads use `WeekTrayInfo-Setup-<version>.exe` and `WeekTrayInfo-<version>.zip`.
Later versions replace the program in this same folder. Setup closes the running
installed app gracefully and starts the new version. When moving from a portable
copy, exit it in its tray menu first. Setup creates a Start menu entry
and an entry under Windows' installed apps.

Setup preserves preferences and synchronizes the Windows startup path according to
the saved Start with Windows setting. With no preferences, the app's existing default
(enabled) applies. If preferences cannot be read safely, startup is left unchanged
and setup reports that you should check it in Preferences.

Each tray icon has a permanent GUID at the installed path. Portable and development
copies use separate identities so they cannot claim the installed app's GUIDs.
Windows retains control of visibility and ordering; one final rearrangement may be
needed when switching from the ZIP release. Later updates retain the same executable
path and icon identities. Explorer restarts re-register the icons.

Uninstall removes the program, shortcut and its own autostart entry, but retains your
preferences for a future reinstall. ZIP packages remain available for portable use;
keep the same extraction path if you want stable tray identities.

### Using the app

In Preferences, choose **System (automatic)**, **Deutsch** or **English** under
Language. System mode uses German for a German Windows display language and English
otherwise. Apply updates app labels, menus, tooltips and messages immediately, without
changing the selected calendar date. Existing preference files default to System.
Cancel discards an unapplied language choice; Default selects System again.

The native Windows calendar gets its month/day strings from Windows' regional
settings, independently of the app language. Standard Windows dialogs, such as the
color picker, also use Windows' own language.

English resources are built into the EXE; the German pack is
`de\WeekTrayInfo.resources.dll`. Keep that folder alongside the EXE when using
the ZIP package; setup installs and updates it automatically.

- **Left-click** a tray icon: show/hide the info panel.
- **Calendar menu (⋯)** or **Alt+M**: Preferences, About WeekTrayInfo, or Exit app. Dialogs open centered over the calendar. The window's X hides the calendar; Exit app also removes its tray icons.
- Click the calendar's built-in **Today** line or press **Ctrl+T** to select the current date again. **Escape** hides the calendar.
- **Right-click** a tray icon: context menu (Show, Preferences, Close, Exit).
- **Preferences**: choose which icons are shown, their colours, the fiscal-calendar
  rules, and whether the app starts with Windows.

The fiscal-calendar preview shows the start and first week of the fiscal year that
contains today's date under the currently selected rules. It updates while editing,
including when week 1 starts in the previous calendar year. It does not save changes.

Import loads an XML settings file into Preferences for review. Apply or OK saves the
imported values, including the Start with Windows and Language choices; Cancel discards
unapplied changes. Export writes the values currently displayed in Preferences to a
separate XML file without applying them. Exports cannot overwrite active preferences
or their backup. Imports reject invalid calendar/language values, unsafe XML, oversized
documents and configurations with no visible tray icon.

### Checking for updates

Choose Check for updates in the tray menu. The check runs only on request, uses GitHub's
release API without credentials and keeps the UI responsive. Stable versions consider
stable releases; RC versions also consider prereleases. The newest matching release is
selected by semantic version, with preference for the setup asset and then the ZIP.
Open download launches your browser; run the downloaded setup to install the update.
No background installation is performed.

If GitHub is unavailable, rate-limited or the release data is private, the dialog explains
the result and offers the releases page in your browser. Private repositories require
GitHub sign-in in the browser; automatic release comparison requires public release data.

Settings are stored per-user at the stable, version-independent location:

```text
%APPDATA%\WeekTrayInfo\Preferences.xml
```

If that file is absent, the app uses its defaults. Applying preferences saves to
this location. Future WeekTrayInfo updates keep the same settings.
Writes replace the file atomically and keep the previous valid save as
`Preferences.xml.bak`. A damaged file triggers a warning and recovery from that
backup, or default settings if no readable backup exists. At least one tray icon
always remains enabled.

Autostart is changed when you apply preferences, and its program path is synchronized
by setup during installation and updates. If saving preferences fails, the previous
startup entry is restored and the dialog stays open. Automatic text color chooses
black or white for contrast; a manually selected color is retained when automatic
color is temporarily enabled.

## How the week numbers are calculated

- **Calendar week** follows the **ISO 8601** definition (weeks start Monday; week 1 is the
  week containing the first Thursday of the year).
- **Fiscal week / year** are derived from your configured first fiscal month, first day of
  the week, and fiscal-week rule.
  Week 1 begins with the first week containing at least the configured number of
  days in the new fiscal year. Its start may fall in the preceding calendar month
  or year. The fiscal-year label uses the start year and following year (`25/26`),
  including when January is the first fiscal month.

## Code layout

Release numbering and the preparation process are documented in [VERSIONING.md](VERSIONING.md).

- `TrayApplicationContext`: owns settings, refresh, application lifetime, and window coordination.
- `TrayIcons`: owns the tray icons and their context menu, with the established 16-pixel number rendering.
- `PersistentTrayIcon`, `TrayIconIdentity`: native GUID registration, input and Explorer recovery.
- `Localization`: language choice and resource lookup; `Strings.resx` contains English,
  `Strings.de.resx` contains German. Add translations to both files using the same keys.
- `ApplicationLifetimeWindow`: a lightweight hidden dialog owner that receives Windows shutdown messages.
- `CalendarForm`: a calendar view created on demand; displays results and reports user requests.
- `PreferencesForm`, `AboutForm`: settings and application information dialogs.
- `UpdateForm`, `Updates`: asynchronous release lookup, version comparison and download selection.
- `DateFunctions/WeekCalculator`: calculates a typed `WeekInfo` result for the selected date.
- `DateFunctions/IsoCalendar`, `FiscalCalendar`: calendar algorithms, independent of the UI.
- `Config`: settings snapshots, persistence, apply coordination, and icon contrast.
- `Config/SettingsTransfer`: validated XML import and atomic export, separate from Apply.
- `Preferences/StartupRegistration`: the current-user Windows startup entry.
- `WeekTrayInfo.Tests`: regression runner with isolated temporary settings and fake startup entries.

The stable release is 2.0.1 on `master`, tagged `v2.0.1`. Development continues on
`dev`. Version 2.0 starts a clean WeekTrayInfo installation with new application,
settings and tray identities. Earlier CalApp / Calendar Week Info releases remain
available as historical tags. Exit and uninstall an earlier installed version before
rolling out 2.0; its old preferences are left untouched and are not imported automatically.
Configure the new version once and export its settings to transfer them to other machines.
Tray icons may need to be arranged once for this new identity; subsequent updates retain it.
The number icons keep their familiar appearance. Startup, background refresh, and preferences never
create the calendar view. Closing the calendar hides it; Exit ends the tray application.

Before distributing a release internally, check the actual desktop behavior at your
company's DPI settings and after sleep, logoff, and a Windows restart. Updates are
installed by running the new setup; a manual update check is included in the tray menu.
EXE and setup are currently unsigned; no publicly trusted signing certificate is configured.

## License

Released under the [MIT License](LICENSE). Copyright (c) 2026 Christoph Ziegler.

The calendar icon is original artwork under the same MIT license. Its editable
source is `tools/New-CalendarIcon.ps1`; run it with PowerShell to regenerate the
ICO at 16, 20, 24, 32, 48, 64, 128 and 256 pixels. It uses geometric shapes only,
without external artwork or fonts. The numeric week icons are drawn by the app.
