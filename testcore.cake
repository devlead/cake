string sdkUrl = "https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/Latest/dotnet-dev-win-x64.latest.zip";
FilePath sdkZip = "./tools/dotnet-dev-win-x64.latest.zip";
DirectoryPath sdkPath = "./tools/dotnet-dev-win-x64";
FilePath toolPath = "./tools/dotnet-dev-win-x64/dotnet.exe";
FilePath coreTestProj = "./coreTest/project.json";

Task("Install-RunTime")
    .Does(() =>
{
    if (!DirectoryExists(sdkPath))
    {
        Information("SDK Path missing {0}", sdkPath);
        if (!FileExists(sdkZip))
        {
            Information("SDK Zip missing {0}, downloading from {1}", sdkZip, sdkUrl);
            DownloadFile(sdkUrl, sdkZip);
        }
        Information("Unzipping {0} to {1}", sdkZip, sdkPath);
        Unzip(sdkZip, sdkPath);
    }
});


Task("Restore")
    .IsDependentOn("Install-RunTime")
    .Does(() =>
{
    DotNetCoreRestore(
        coreTestProj.FullPath,
        new DotNetCoreRestoreSettings
        {
            Sources = new [] { "https://api.nuget.org/v3/index.json" },
            ToolPath = toolPath
        }
    );
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(
        coreTestProj.FullPath,
        new DotNetCoreBuildSettings {
            ToolPath = toolPath,
            Framework = "netcoreapp1.0"
        }
    );
});

Task("Run")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreRun(
            coreTestProj.FullPath,
            string.Empty,
            new DotNetCoreRunSettings {
                ToolPath = toolPath
            }
    );
});

RunTarget("Run");