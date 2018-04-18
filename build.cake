
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

RunTarget("ReCreatePublishDir");

RunTarget("Build");

RunTarget("CopyKaLEDoscope");
 
RunTarget("BuildSetup");

RunTarget("CreateZipPackage");
