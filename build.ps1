param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$UiTests,
    [switch]$Package,
    [switch]$Installer,
    [string]$InnoSetupCompiler
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
    $version = [Diagnostics.FileVersionInfo]::GetVersionInfo($applicationPath).ProductVersion
    $artifactDirectory = Join-Path $projectRoot 'artifacts'
    New-Item -ItemType Directory -Path $artifactDirectory -Force | Out-Null
    $archivePath = Join-Path $artifactDirectory "CalendarWeekInfo-$version.zip"
    $packageFiles = @($applicationPath, "$applicationPath.config",
        (Join-Path $projectRoot 'README.md'), (Join-Path $projectRoot 'CHANGELOG.md'),
        (Join-Path $projectRoot 'VERSIONING.md'), (Join-Path $projectRoot 'LICENSE'),
        (Join-Path $projectRoot 'CalApp\bin\Release\de'))
    Compress-Archive -LiteralPath $packageFiles -DestinationPath $archivePath -Force
    Write-Host "Package: $archivePath"
}

if ($Installer) {
    if ($Configuration -ne 'Release') { throw 'Installers must use the Release configuration.' }
    if (-not $InnoSetupCompiler) {
        $compilerCommand = Get-Command ISCC.exe -ErrorAction SilentlyContinue
        $compilerCandidates = @(
            (Join-Path $projectRoot 'artifacts\tools\InnoSetup\ISCC.exe'),
            (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
            (Join-Path $env:ProgramFiles 'Inno Setup 7\ISCC.exe')
        )
        if ($compilerCommand) { $InnoSetupCompiler = $compilerCommand.Source }
        else { $InnoSetupCompiler = $compilerCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1 }
    }
    if (-not $InnoSetupCompiler -or -not (Test-Path -LiteralPath $InnoSetupCompiler)) {
        throw 'Install Inno Setup 6 or newer, or pass -InnoSetupCompiler with the path to ISCC.exe.'
    }
    $applicationPath = Join-Path $projectRoot 'CalApp\bin\Release\CalendarWeekInfo.exe'
    $versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($applicationPath)
    & $InnoSetupCompiler '/Qp' "/DAppVersion=$($versionInfo.ProductVersion)" "/DNumericVersion=$($versionInfo.FileVersion)" (Join-Path $projectRoot 'installer\CalApp.iss')
    if ($LASTEXITCODE -ne 0) { throw "Installer build failed with exit code $LASTEXITCODE." }
    Write-Host "Installer: $(Join-Path $projectRoot "artifacts\CalApp-Setup-$($versionInfo.ProductVersion).exe")"
}
