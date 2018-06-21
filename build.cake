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
	CreateDirectory(targetDir);
	var files = GetFiles($"{sourceDir}/*.*");
	CopyFiles(files, targetDir);
});

Task("CreateZipPackage")
.Does(() =>
{
	Zip($"./{publishDir}/KaLEDoscope", $"./{publishDir}/KaLEDoscope.zip");
	/*
	//удалим папки
	foreach (var subDir in GetSubDirectories(publishDir)) {
		DeleteDirectory(subDir, new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
	}
	
	удалим файлы
	var files = GetFiles(publishDir + "/*");
	foreach(var file in files)
	{
		if(!file.ToString().Contains("KaLEDoscope.zip"))
			DeleteFile(file);
	}
 */
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

RunTarget("CopyKaLEDoscope");
 
RunTarget("BuildSetup");

RunTarget("CreateZipPackage");
