#tool "nuget:?package=xunit.runner.console"

var configuration = Argument("configuration", "Release");
var publishDir = "_publish";

var packagesDir = "./packages";

Task("Build")
.Does(() =>
{
    NuGetRestore("KaLEDoscope.sln");
    DotNetBuild("KaLEDoscope.sln", x => x
        .SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .WithTarget("build")
        .WithProperty("TreatWarningsAsErrors", "false")
    );
	
});

Task("RunUnitTests")
.Does(() =>
{
	XUnit2("./Testing/bin/Release/Testing.dll");
	XUnit2("./Devices/PixelBoardDeviceTesting/bin/Release/PixelBoardDeviceTesting.dll");
	XUnit2("./Devices/SevenSegmentTesting/bin/Release/SevenSegmentTesting.dll");
});

Task("ReCreatePublishDir")
.Does(() =>
{
	if (DirectoryExists(publishDir))
	{
		DeleteDirectory(publishDir, new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
	}
	CreateDirectory(publishDir);
});

Task("CopyKaLEDoscope")
.Does(() =>
{
	var sourceDir = $"./KaLEDoscope/bin/Release";
	var targetDir = $"{publishDir}/KaLEDoscope";
	CopyBase(sourceDir, targetDir);
});

Task("CopyKeygen")
.Does(() =>
{
	var sourceDir = $"./Keygen/bin/Release";
	var targetDir = $"{publishDir}/Keygen";
	CopyBase(sourceDir, targetDir);
});


private void CopyBase(string sourceDir, string targetDir)
{
	var ignoredExts = new string[] { ".pdb", ".key",".struct" };
	if (!DirectoryExists(targetDir))
	{
		CreateDirectory(targetDir);
	}
	var files = GetFiles($"{sourceDir}/*.*")
		.Where(f => !ignoredExts.Contains(f.GetExtension().ToLower()));
	CopyFiles(files, targetDir);
}

Task("ZipKaLEDoscope")
.Does(() =>
{
	Zip($"./{publishDir}/KaLEDoscope", $"./{publishDir}/KaLEDoscope.zip");
});

Task("ZipKeygen")
.Does(() =>
{
	Zip($"./{publishDir}/Keygen", $"./{publishDir}/KaLEDoscope Keygen.zip");
});

Task("BuildSetup")
.Does(() =>
{
	MSBuild("./Setup/Setup.csproj", new MSBuildSettings());
	var files = GetFiles("./Setup/*.msi");
	MoveFiles(files, publishDir);
});

Task("UpdateVersion")
.Does(() =>
{
	var path = "./KaLEDoscope/Properties/AssemblyVersion.cs";
	var assemblyInfo = ParseAssemblyInfo(path);
	var parsedVersion = assemblyInfo.AssemblyVersion.Split('.');
	var major = Convert.ToInt32(parsedVersion[0]);
	var minor = Convert.ToInt32(parsedVersion[1]);
	var build = Convert.ToInt32(parsedVersion[2]);
	Information($"Major {major}");
	Information($"Minor {minor}");
	Information($"Build {build}");
	build++;
	var newVersion = $"{major}.{minor}.{build}";
	Information($"Update version to {newVersion}");
	CreateAssemblyInfo(path, new AssemblyInfoSettings 
	{
    	Version = newVersion,
    	FileVersion = newVersion,
    	InformationalVersion = newVersion,
    	Copyright = string.Format("Copyright (c) {0}", DateTime.Now.Year)
	});
});

RunTarget("ReCreatePublishDir");

RunTarget("UpdateVersion");

RunTarget("Build");

RunTarget("RunUnitTests");

RunTarget("CopyKaLEDoscope");

RunTarget("CopyKeygen");

RunTarget("BuildSetup");

RunTarget("ZipKaLEDoscope");

RunTarget("ZipKeygen");