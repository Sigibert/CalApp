param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$UiTests,
    [switch]$Package
)

$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$msbuildCommand = Get-Command msbuild.exe -ErrorAction SilentlyContinue
if ($msbuildCommand) {
    $msbuildPath = $msbuildCommand.Source
} else {
    $vswherePath = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (-not (Test-Path -LiteralPath $vswherePath)) { throw 'Visual Studio MSBuild was not found. Install the .NET desktop development workload.' }
    $msbuildPath = & $vswherePath -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
    if (-not $msbuildPath) { throw 'Visual Studio MSBuild was not found.' }
}

& $msbuildPath (Join-Path $projectRoot 'CalApp.sln') /t:Rebuild "/p:Configuration=$Configuration" /nologo /verbosity:minimal
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE." }
$testExecutable = Join-Path $projectRoot "CalApp.Tests\bin\$Configuration\CalApp.Tests.exe"
if ($UiTests) { & $testExecutable --ui } else { & $testExecutable }
if ($LASTEXITCODE -ne 0) { throw "Regression tests failed with exit code $LASTEXITCODE." }

if ($Package) {
    if ($Configuration -ne 'Release') { throw 'Packages must use the Release configuration.' }
    $applicationPath = Join-Path $projectRoot 'CalApp\bin\Release\CalendarWeekInfo.exe'
    $version = [Diagnostics.FileVersionInfo]::GetVersionInfo($applicationPath).FileVersion
    $artifactDirectory = Join-Path $projectRoot 'artifacts'
    New-Item -ItemType Directory -Path $artifactDirectory -Force | Out-Null
    $archivePath = Join-Path $artifactDirectory "CalendarWeekInfo-$version.zip"
    $packageFiles = @($applicationPath, "$applicationPath.config",
        (Join-Path $projectRoot 'README.md'), (Join-Path $projectRoot 'CHANGELOG.md'), (Join-Path $projectRoot 'LICENSE'))
    Compress-Archive -LiteralPath $packageFiles -DestinationPath $archivePath -Force
    Write-Host "Package: $archivePath"
}
