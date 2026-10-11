# Changelog

## 1.2.0

- Release the new tray architecture, per-user installer, persistent tray identities, English/German language packs, update check, fiscal preview and settings transfer as a stable version.
- Rename the app to WeekTrayInfo: "Kalender- und Geschäftswochen auf einen Blick".
- Retain install path, executable, settings, autostart and tray identities for update compatibility.
- Use WeekTrayInfo names for setup and portable downloads; accept both new and legacy asset names during update checks.
- Replace the calendar icon with original MIT artwork and include its editable drawing script.
- Remove the redundant Today button; retain the native calendar's Today line and Ctrl+T.
- Keep the GitHub repository URL unchanged for earlier update checks.

## 1.2.0-rc.4

- Add a manual update check to the tray menu with semantic version comparison and direct setup/ZIP links.
- Stable builds consider stable releases; prerelease builds also consider prereleases.
- Keep the app responsive during lookup; handle timeouts, rate limits and inaccessible private releases.
- Add Today and Ctrl+T to return to the current date; Escape hides the calendar.
- Preview the current fiscal year's start and first week while editing calendar rules.
- Import and export XML settings, including colors, calendar rules, startup preference and language.
- Stage imported values for review; Apply commits them and Cancel leaves active settings unchanged.
- Reject malformed, unsafe and oversized imports and prevent exports from overwriting active preferences.
- Include English/German text for every new app control and message.

## 1.2.0-rc.3

- Add English and German language resources, included in both ZIP and setup.
- Default to the Windows display language (German for de locales, English otherwise).
- Add System, Deutsch and English to Preferences; persist the choice without changing existing settings.
- Apply language changes immediately to menus, labels, tooltips and app messages.
- Localize fiscal month, weekday and week-rule choices while preserving the selected values.
- Allow longer translated captions without clipped buttons or overlapping color controls.
- Keep the native Windows calendar and standard dialogs in the Windows regional/display language.
- Test both language packs, system fallback, migration, persistence, live switching and cancellation.

## 1.2.0-rc.2

- Add a per-user setup that keeps the same executable path between updates.
- Identify the three tray icons with persistent GUIDs; isolate portable and development copies.
- Restore icons after Explorer restarts and retry registration if Explorer is temporarily unavailable.
- Close the tray application gracefully before updates and start the installed version afterward.
- Preserve preferences during install, update and uninstall; migrate the autostart path without resetting its setting.
- Rename ambiguous preferences captions to Color and Auto contrast.
- Test native tray registration, input, cleanup, installer shutdown and startup synchronization.

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
