var target = Argument("target", "Default");

var solution = "./src/Additio.Sitecore.DependencyConfigReader.sln";
var version = "1.0.1";
var buildOutput = "./.build";

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Pack")
    ;

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildOutput);
});

Task("Build")
    .Does(() =>
{
    NuGetRestore(solution);

    MSBuild(solution, settings =>
        settings.SetVerbosity(Verbosity.Minimal)
                .SetConfiguration("Release")
                .WithTarget("Rebuild")
                .UseToolVersion(MSBuildToolVersion.VS2015)
                .SetMaxCpuCount(0)
                .ArgumentCustomization = args => args.Append("/nologo")
    );
});


Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new NuGetPackSettings
    {
        Version = version,
        Symbols = false,
        OutputDirectory = buildOutput
    };

    NuGetPack("./nuget/Additio.Sitecore.DependencyConfigReader.nuspec", settings);
});

RunTarget(target);