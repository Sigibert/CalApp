param([string]$InnoSetupCompiler)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
if (-not $InnoSetupCompiler) { $InnoSetupCompiler = Join-Path $projectRoot 'artifacts\tools\InnoSetup\ISCC.exe' }
if (-not (Test-Path -LiteralPath $InnoSetupCompiler)) { throw 'Pass -InnoSetupCompiler with the path to ISCC.exe.' }
$testUninstallKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\{EAF114D6-B88F-4913-884C-503264F29A83}_is1'
if (Test-Path -LiteralPath $testUninstallKey) { throw 'An earlier installer test is still installed. Uninstall it before retrying.' }
$applicationPath = Join-Path $projectRoot 'CalApp\bin\Release\CalendarWeekInfo.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($applicationPath)
& $InnoSetupCompiler '/Qp' '/DTestBuild=1' "/DAppVersion=$($version.ProductVersion)" "/DNumericVersion=$($version.FileVersion)" (Join-Path $PSScriptRoot 'CalApp.iss')
if ($LASTEXITCODE -ne 0) { throw 'Test installer build failed.' }

$artifactRoot = [IO.Path]::GetFullPath((Join-Path $projectRoot 'artifacts'))
$testDirectory = Join-Path $artifactRoot ('installer-test-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testDirectory | Out-Null
$installDirectory = [IO.Path]::GetFullPath((Join-Path $testDirectory 'app'))
if (-not $installDirectory.StartsWith($artifactRoot + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Test install path escaped artifacts.' }
$setupPath = Join-Path $artifactRoot 'CalApp-Setup-Test.exe'
$uninstallPath = Join-Path $installDirectory 'unins000.exe'
$preferencesPath = Join-Path $env:APPDATA 'ChristophZieglerHomeLab\Calendar Week Info\Preferences.xml'
$runKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run'
function Get-PreferencesHash {
    if (Test-Path -LiteralPath $preferencesPath) { return (Get-FileHash -LiteralPath $preferencesPath -Algorithm SHA256).Hash }
    return $null
}
function Get-StartupCommand {
    return (Get-ItemProperty -LiteralPath $runKey -Name 'Calendar Week Info' -ErrorAction SilentlyContinue).'Calendar Week Info'
}
function Invoke-Setup([string]$LogName) {
    $arguments = @('/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/SP-', '/NOICONS',
        ('/DIR="' + $installDirectory + '"'), ('/LOG="' + (Join-Path $testDirectory $LogName) + '"'))
    $process = Start-Process -FilePath $setupPath -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru
    if ($process.ExitCode -ne 0) { throw "Setup failed: $($process.ExitCode). See $testDirectory" }
}

$preferencesBefore = Get-PreferencesHash
$startupBefore = Get-StartupCommand
try {
    Invoke-Setup 'install.log'
    $installedExe = Join-Path $installDirectory 'CalendarWeekInfo.exe'
    if ((Get-FileHash -LiteralPath $installedExe).Hash -ne (Get-FileHash -LiteralPath $applicationPath).Hash) { throw 'Installed EXE differs from build.' }
    if (-not (Test-Path -LiteralPath $testUninstallKey)) { throw 'Missing per-user uninstall registration.' }
    $installedGerman = Join-Path $installDirectory 'de\CalendarWeekInfo.resources.dll'
    $expectedGerman = Join-Path $projectRoot 'CalApp\bin\Release\de\CalendarWeekInfo.resources.dll'
    if ((Get-FileHash -LiteralPath $installedGerman).Hash -ne (Get-FileHash -LiteralPath $expectedGerman).Hash) { throw 'German language pack was not installed correctly.' }
    Write-Host 'PASS per-user installation'

    $installedConfig = "$installedExe.config"
    $expectedConfig = (Get-FileHash -LiteralPath "$applicationPath.config").Hash
    [IO.File]::WriteAllText($installedConfig, 'old-program-file')
    [IO.File]::WriteAllText((Join-Path $installDirectory 'keep-me.txt'), 'user-owned-file')
    Invoke-Setup 'update.log'
    if ((Get-FileHash -LiteralPath $installedConfig).Hash -ne $expectedConfig) { throw 'Update did not replace the old program configuration.' }
    if ([IO.File]::ReadAllText((Join-Path $installDirectory 'keep-me.txt')) -ne 'user-owned-file') { throw 'Update changed an unrelated file.' }
    Write-Host 'PASS update in the same directory'
} finally {
    if (Test-Path -LiteralPath $uninstallPath) {
        $process = Start-Process -FilePath $uninstallPath -ArgumentList @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART',
            ('/LOG="' + (Join-Path $testDirectory 'uninstall.log') + '"')) -WindowStyle Hidden -Wait -PassThru
        if ($process.ExitCode -ne 0) { throw "Test uninstall failed: $($process.ExitCode). See $testDirectory" }
    }
}
if (Test-Path -LiteralPath $testUninstallKey) { throw 'Uninstall registration was not removed.' }
if (Test-Path -LiteralPath (Join-Path $installDirectory 'CalendarWeekInfo.exe')) { throw 'Uninstall left the application behind.' }
if (-not (Test-Path -LiteralPath (Join-Path $installDirectory 'keep-me.txt'))) { throw 'Uninstall removed an unrelated user file.' }
if ((Get-PreferencesHash) -ne $preferencesBefore) { throw 'Personal preferences changed during the installer test.' }
if ((Get-StartupCommand) -ne $startupBefore) { throw 'Personal autostart changed during the installer test.' }
Write-Host 'PASS uninstall and preservation of personal preferences/autostart'
Write-Host "Installer logs: $testDirectory"
