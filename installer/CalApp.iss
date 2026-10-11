; Build through build.ps1 -Installer. Keep AppId and install directory stable.
#ifndef AppVersion
  #error AppVersion must be supplied by the build script
#endif
#ifndef NumericVersion
  #error NumericVersion must be supplied by the build script
#endif
#ifdef TestBuild
  #define InstallerId "{{EAF114D6-B88F-4913-884C-503264F29A83}"
  #define InstallerName "CalApp Installer Test"
  #define OutputName "CalApp-Setup-Test"
#else
  #define InstallerId "{{30E02D1A-52C6-4953-AAC9-6D5C1AF764CC}"
  #define InstallerName "CalApp"
  #define OutputName "CalApp-Setup-" + AppVersion
#endif

[Setup]
AppId={#InstallerId}
AppName={#InstallerName}
AppVersion={#AppVersion}
AppPublisher=Christoph Ziegler
AppPublisherURL=https://github.com/Sigibert/CalApp
#ifdef TestBuild
DefaultDirName={localappdata}\Programs\CalAppInstallerTest
#else
DefaultDirName={localappdata}\Programs\CalApp
#endif
PrivilegesRequired=lowest
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableWelcomePage=yes
DisableReadyPage=yes
UsePreviousAppDir=yes
OutputDir=..\artifacts
OutputBaseFilename={#OutputName}
SetupIconFile=..\CalApp\Resources\Calendar_Icon.ico
UninstallDisplayIcon={app}\CalendarWeekInfo.exe
VersionInfoVersion={#NumericVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
MinVersion=10.0
CloseApplications=yes
CloseApplicationsFilter=*.exe
RestartApplications=no
SetupMutex=CalApp_Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "..\CalApp\bin\Release\CalendarWeekInfo.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CalApp\bin\Release\CalendarWeekInfo.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CalApp\bin\Release\de\*.resources.dll"; DestDir: "{app}\de"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CHANGELOG.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\VERSIONING.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion

#ifndef TestBuild
[Icons]
Name: "{userprograms}\CalApp"; Filename: "{app}\CalendarWeekInfo.exe"; WorkingDir: "{app}"

[Run]
Filename: "{app}\CalendarWeekInfo.exe"; Flags: nowait
#endif

[CustomMessages]
english.CloseCalApp=Please exit the running Calendar Week Info app using its tray menu, then retry.
german.CloseCalApp=Bitte Calendar Week Info im Tray-Menü über Exit beenden und erneut versuchen.
english.StartupFailed=The app was installed, but Windows startup could not be updated safely. Please check Start with Windows in Preferences.
german.StartupFailed=Die App wurde installiert, aber der Autostart konnte nicht sicher aktualisiert werden. Bitte Start with Windows in den Einstellungen prüfen.
english.FrameworkRequired=CalApp requires .NET Framework 4.7.2 or newer. Please install it before running this setup.
german.FrameworkRequired=CalApp benötigt .NET Framework 4.7.2 oder neuer. Bitte vor der Installation einrichten.

[Code]
const
  RunKey = 'Software\Microsoft\Windows\CurrentVersion\Run';
  RunValue = 'Calendar Week Info';

function IsTestBuild: Boolean;
begin
#ifdef TestBuild
  Result := True;
#else
  Result := False;
#endif
end;

function InitializeSetup: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and
    (Release >= 461808);
  if not Result then
    SuppressibleMsgBox(CustomMessage('FrameworkRequired'), mbError, MB_OK, IDOK);
end;

function StopCalApp: Boolean;
var
  OwnerWindow: HWND;
  Attempt: Integer;
begin
  Result := True;
  if IsTestBuild then Exit;
  OwnerWindow := FindWindowByWindowName('CalApp.UpdateOwner');
  if OwnerWindow <> 0 then
  begin
    { Close the application's lifetime owner; never kill the process. }
    PostMessage(OwnerWindow, $0010, 0, 0);
    for Attempt := 1 to 100 do
    begin
      if not CheckForMutexes('CalApp_SingleInstance') then Exit;
      Sleep(50);
    end;
  end;
  Result := not CheckForMutexes('CalApp_SingleInstance');
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := '';
  if not StopCalApp then Result := CustomMessage('CloseCalApp');
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ExitCode: Integer;
begin
  if (CurStep = ssPostInstall) and not IsTestBuild then
  begin
    { Reads existing preferences; changes only the current user's autostart path. }
    if not Exec(ExpandConstant('{app}\CalendarWeekInfo.exe'), '--sync-startup',
      ExpandConstant('{app}'), SW_HIDE, ewWaitUntilTerminated, ExitCode) or (ExitCode <> 0) then
    begin
      Log('Startup synchronization failed. Existing preferences were preserved.');
      SuppressibleMsgBox(CustomMessage('StartupFailed'), mbInformation, MB_OK, IDOK);
    end;
  end;
end;

function InitializeUninstall: Boolean;
begin
  Result := StopCalApp;
  if not Result then
    SuppressibleMsgBox(CustomMessage('CloseCalApp'), mbError, MB_OK, IDOK);
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  Command: String;
  InstalledExe: String;
begin
  if (CurUninstallStep = usUninstall) and not IsTestBuild then
  begin
    InstalledExe := ExpandConstant('{app}\CalendarWeekInfo.exe');
    if RegQueryStringValue(HKCU, RunKey, RunValue, Command) and
      ((CompareText(Command, '"' + InstalledExe + '"') = 0) or
       (CompareText(Command, InstalledExe) = 0)) then
      RegDeleteValue(HKCU, RunKey, RunValue);
    { Preferences and Explorer's icon settings are intentionally retained. }
  end;
end;
