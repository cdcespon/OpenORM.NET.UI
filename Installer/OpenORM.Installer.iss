[Setup]
AppName=OpenORM.NET
AppVersion=1.5
WizardStyle=modern
DefaultDirName={autopf}\OpenORM.NET
DefaultGroupName=OpenORM.NET
UninstallDisplayIcon={app}\OpenORM.NET.UI.exe
Compression=lzma2
SolidCompression=yes
OutputDir=.
UninstallDisplayName=OpenORM.NET.Uninstall
OutputBaseFilename=OpenORM.NET.Setup

[Files]
Source: "bin\adodb.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\DbTargets.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\FirebirdSql.Data.Firebird.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\FirebirdSql.Data.FirebirdClient.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Interop.MSDASC.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Languages.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Mono.Security.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\MyMeta.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\MyMeta.tlb"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\MySql.Data.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Npgsql.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\OpenORM.NET.UI.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Zeus.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\OpenORM.NET.UI.exe.config"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\OpenORM.NET"; Filename: "{app}\OpenORM.NET.UI.exe"; IconFilename: "{app}\OpenORM.NET.UI.exe"; IconIndex: 0
