# Changelog

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
