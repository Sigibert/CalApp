# WeekTrayInfo versioning

WeekTrayInfo uses `MAJOR.MINOR.PATCH`, following Semantic Versioning with compatibility
defined by its user behavior and saved preferences.

| Change | Example |
| --- | --- |
| Fix a bug without changing the supported behavior | `1.2.0` to `1.2.1` |
| Add compatible functionality or make a substantial internal improvement | `1.1.0` to `1.2.0` |
| Introduce an incompatible change that users must explicitly adapt to | `1.2.0` to `2.0.0` |

Reset the positions to the right whenever increasing a position: after `1.2.7`
the next minor release is `1.3.0`. Numbers are integers: `1.10.0` follows `1.9.0`.
A large refactor alone does not require a major version.

`1.2.0-rc.1` is the first release candidate for `1.2.0`. If fixes are needed before
the final release, publish `1.2.0-rc.2`, then `1.2.0-rc.3`. Once accepted, publish
`1.2.0`. Subsequent compatible bug fixes become `1.2.1`.

## Preparing a release

1. Set `WeekTrayInfoVersion` in `Version.props`, e.g. `1.2.0-rc.1` or `1.2.0`.
2. Update `CHANGELOG.md` and run `build.ps1 -UiTests -Package`.
3. Commit the tested source and create its matching Git tag, e.g. `v1.2.0-rc.1`.
4. Publish a GitHub release with that exact tag and ZIP. Mark release candidates
   as prereleases; keep the latest stable release recommended for normal use.
5. After desktop acceptance, promote the tested development work to `master`,
   set the final version, build/test again, and tag the stable release.

Never move a published tag or replace a released binary with different code.
Give corrections a new version instead. Keep `master` for stable releases and
`dev` for the next version; ordinary commits do not need new release numbers.

## .NET versions

`Version.props` is the source of the release version. The build derives the numeric
.NET assembly/file version automatically: `1.2.0-rc.1` becomes `1.2.0.0` internally.
The complete release version appears in the About dialog and ZIP filename.
The fourth zero is a technical .NET field and is not a separate WeekTrayInfo release.

The existing `v1.1.0` baseline retains its historical `1.1.0.0` metadata. Starting
with 1.2 release candidates, the displayed product version uses three positions.

Reference: https://semver.org/
