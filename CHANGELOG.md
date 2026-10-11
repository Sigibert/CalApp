# Changelog

## 1.2.0-rc.1

- Make the tray application own icons, menus, refresh, and application lifetime.
- Create the calendar window only when first requested; preferences do not require it.
- Keep the calendar view independent of settings storage, tray icons, and timers.
- Calculate one typed week result for any date and share it between tray and calendar.
- Preserve the established 16-by-16 tray-number rendering and existing preferences.
- Test lazy window creation, background updates, and shutdown without a calendar window.

## 1.1.0

- Correct fiscal week rules and fiscal-year transitions, including January starts.
- Use readable black or white text for automatic tray-icon contrast.
- Keep preferences at a stable path across version changes and read legacy files.
- Save preferences atomically, retain a backup, and report recovery or load failures.
- Apply settings only after successful persistence; roll back autostart on save failure.
- Preserve calendar selection and manual colors when preferences are applied.
- Refresh tray icons on the UI thread and catch resume and clock changes.
- Release dialogs, timers, and icon handles explicitly.
- Rename forms and replace legacy helpers with focused calendar and settings classes.
- Add package-free regression tests and a build/test/package script.
