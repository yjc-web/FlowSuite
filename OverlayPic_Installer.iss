; Inno Setup Script for OverlayPic
; Generated for v1.0.4.0

#define MyAppName "OverlayPic"
#define MyAppVersion "1.0.4.0"
#define MyAppPublisher "YJC"
#define MyAppURL "https://blog.naver.com/nds-macro"
#define MyAppExeName "OverlayPic.exe"

[Setup]
; App Details
AppId={{C3D4E5F6-A7B8-9012-CDEF-123456789012}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} v{#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; Destination & Privileges
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

; Output
OutputDir=dist
OutputBaseFilename=OverlayPic_v{#MyAppVersion}_Setup
SetupIconFile=Resources\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

; Compression & UI
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "bin\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "사용안내_및_단축키.txt"; DestDir: "{app}"; Flags: ignoreversion; DestName: "사용안내_및_단축키.txt"
Source: "userManual.html"; DestDir: "{app}"; Flags: ignoreversion; DestName: "userManual.html"
Source: "releasenotes.html"; DestDir: "{app}"; Flags: ignoreversion; DestName: "releasenotes.html"

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
