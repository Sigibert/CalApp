# Calendar Week Info

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
- **Auto-refresh** — updates on the hour and at midnight without any interaction.
  A one-minute heartbeat also catches wake from sleep and clock changes.

## Requirements

- Windows
- .NET Framework 4.7.2 or a compatible later .NET Framework runtime
- For development: Visual Studio with **.NET desktop development** and the
  **.NET Framework 4.7.2 targeting pack**

## Building

Open `CalApp.sln` in Visual Studio and build, or from a developer prompt:

```
msbuild CalApp\CalApp.csproj /t:Rebuild /p:Configuration=Release
```

The executable is produced at `CalApp\bin\Release\CalendarWeekInfo.exe`.

To build the solution and run the regression tests from PowerShell:

```powershell
.\build.ps1
.\build.ps1 -UiTests -Package
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

You can also run the test executable after building `CalApp.sln`:

```powershell
.\CalApp.Tests\bin\Release\CalApp.Tests.exe --ui
```

To measure startup phases in an isolated process:

```powershell
.\CalApp.Tests\bin\Release\CalApp.Tests.exe --startup-timing
```

The runner returns a nonzero exit code on failure. Tests cover a complete 400-year
Gregorian cycle for ISO weeks, every fiscal month/weekday/week-rule combination,
settings migration and recovery, save failures, automatic contrast, and UI behavior.

## Usage

- **Left-click** a tray icon: show/hide the info panel.
- **Right-click** a tray icon: context menu (Show, Preferences, Close, Exit).
- **Preferences**: choose which icons are shown, their colours, the fiscal-calendar
  rules, and whether the app starts with Windows.

Settings are stored per-user at the stable, version-independent location:

```text
%APPDATA%\ChristophZieglerHomeLab\Calendar Week Info\Preferences.xml
```

If that file is absent, the app reads the newest usable preferences from the old
version-number subfolders. Applying preferences saves them to the new location;
the original file remains intact. Updates therefore keep the same settings.
Writes replace the file atomically and keep the previous valid save as
`Preferences.xml.bak`. A damaged file triggers a warning and recovery from that
backup, or default settings if no readable backup exists. At least one tray icon
always remains enabled.

Autostart is changed only when you apply preferences. If saving fails, the previous
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

- `TrayApplicationContext`: owns settings, refresh, application lifetime, and window coordination.
- `TrayIcons`: owns the tray icons and their context menu, with the established 16-pixel number rendering.
- `ApplicationLifetimeWindow`: a lightweight hidden dialog owner that receives Windows shutdown messages.
- `CalendarForm`: a calendar view created on demand; displays results and reports user requests.
- `PreferencesForm`, `AboutForm`: settings and application information dialogs.
- `DateFunctions/WeekCalculator`: calculates a typed `WeekInfo` result for the selected date.
- `DateFunctions/IsoCalendar`, `FiscalCalendar`: calendar algorithms, independent of the UI.
- `Config`: settings snapshots, persistence, apply coordination, and icon contrast.
- `Preferences/StartupRegistration`: the current-user Windows startup entry.
- `CalApp.Tests`: regression runner with isolated temporary settings and fake startup entries.

The stable 1.1 baseline is kept on `master` and tagged `v1.1.0`. The compatible 1.2
architecture update is developed on `dev`; existing preferences and week-icon
appearance remain compatible. Startup, background refresh, and preferences never
create the calendar view. Closing the calendar hides it; Exit ends the tray application.

Before distributing a release internally, check the actual desktop behavior at your
company's DPI settings and after sleep, logoff, and a Windows restart. The ZIP is a
portable release, without an installer or automatic updates.

## License

Released under the [MIT License](LICENSE). Copyright (c) 2026 Christoph Ziegler.
