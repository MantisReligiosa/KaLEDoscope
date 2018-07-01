using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using WixSharp;
using WixSharp.Forms;

namespace Setup
{
    public static class Script
    {
        private static string[] _dependentLibraries => new[]
        {
            "Setups.Common.dll",
        //    "BCrypt.Net-Next.dll"
        };

        [STAThread]
        static public void Main(string[] args)
        {
            GetAssemblyInfo(@".\..\_publish\KaLEDoscope\KaLEDoscope.exe",
                out Guid guid, out Version version);

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
                    GUID = guid,
                    Version = version
                };
            project.ControlPanelInfo.Manufacturer = "SmartTechnologies-M";

            var codeBase = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryName = Path.GetDirectoryName(codeBase);
            foreach (var dependentLibrary in _dependentLibraries)
            {
                var pathToLibrary = Path.Combine(directoryName, dependentLibrary);
                project.DefaultRefAssemblies.Add(pathToLibrary);
            }

            project.UIInitialized += UIInitialized;

            project.ManagedUI = new ManagedUI();
            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                        //.Add<ActivationDialog>()
                                        .Add(Dialogs.InstallDir)
                                        .Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                       .Add(Dialogs.Progress)
                                       .Add(Dialogs.Exit);

            Compiler.BuildMsi(project);
        }

        private static void UIInitialized(SetupEventArgs e)
        {
        }

        private static void GetAssemblyInfo(string assemblyPath,
            out Guid guid, out Version version)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);

            var guidValue = ((GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)
                .First()).Value;

            guid = new Guid(guidValue);
            version = new Version(assembly.GetName().Version.ToString(3));
        }
    }
}