# Calendar Week Info

A lightweight Windows system-tray app that shows the current **calendar week** (ISO 8601)
and **fiscal week** at a glance, and opens a small calendar popup on click.

It sits quietly in the notification area as one or more tiny icons that display the
week number, and refreshes itself automatically at the top of every hour and at midnight.

## Features

- **Tray icons** for the ISO-8601 calendar week and a configurable fiscal week — each a
  small number badge you can colour to taste.
- **Popup panel** (left-click a tray icon) with the calendar week, fiscal week, fiscal year,
  and a month calendar for looking up any date.
- **Configurable fiscal calendar** — first fiscal month, first day of the week, and the
  fiscal-week rule (how many days of a week must fall in the first month to count as week 1).
- **Per-icon colours** — pick a background per icon; the text colour is either
  auto-contrasted for you or picked manually.
- **Start with Windows** — optional autostart via the current-user `Run` key.
- **Auto-refresh** — updates on the hour and at midnight without any interaction.

## Requirements

- Windows
- .NET Framework 4.7.2

## Building

Open `CalApp.sln` in Visual Studio and build, or from a developer prompt:

```
msbuild CalApp\CalApp.csproj /t:Rebuild /p:Configuration=Release
```

The executable is produced at `CalApp\bin\Release\CalendarWeekInfo.exe`.

## Usage

- **Left-click** a tray icon: show/hide the info panel.
- **Right-click** a tray icon: context menu (Show, Preferences, Close, Exit).
- **Preferences**: choose which icons are shown, their colours, the fiscal-calendar
  rules, and whether the app starts with Windows.

Settings are stored per-user as XML under the app's user data folder
(`Preferences.xml`), so your configuration survives updates.

## How the week numbers are calculated

- **Calendar week** follows the **ISO 8601** definition (weeks start Monday; week 1 is the
  week containing the first Thursday of the year).
- **Fiscal week / year** are derived from your configured first fiscal month, first day of
  the week, and fiscal-week rule.

## License

Released under the [MIT License](LICENSE). Copyright (c) 2026 Christoph Ziegler.
