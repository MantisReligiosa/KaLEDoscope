using System;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;

public class Script
{
    static public void Main(string[] args)
    {
        var project =
            new ManagedProject("KaLEDoscope",
                new Dir(@"%ProgramFiles%\SmartTechnologiesM\KaLEDoscope",
                    new DirFiles(@".\..\_publish\KaLEDoscope\*.*")),
                new Dir(@"%ProgramMenu%\Smart Technologies-M",
                        new ExeFileShortcut("Uninstall KaLEDoscope", "[System64Folder]msiexec.exe", "/x [ProductCode]"),
                        new ExeFileShortcut("KaLEDoscope", "[INSTALLDIR]KaLEDoscope.exe", arguments: "")),
                new Dir(@"%Desktop%",
                        new ExeFileShortcut("KaLEDoscope", "[INSTALLDIR]KaLEDoscope.exe", arguments: "")))
            {
                UI = WUI.WixUI_InstallDir
            };

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                    .Add(Dialogs.InstallDir)
                                    .Add(Dialogs.Progress)
                                    .Add(Dialogs.Exit);

        project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                   .Add(Dialogs.Progress)
                                   .Add(Dialogs.Exit);

        Compiler.BuildMsi(project);
    }
}



