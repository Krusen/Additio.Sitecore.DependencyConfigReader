#tool "xunit.runner.console"

var target = Argument("target", "Default");

var solution = "./src/Additio.Sitecore.DependencyConfigReader.sln";
var version = "1.0.3";
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

Task("UnitTest")
    .IsDependentOn("Build")
    .Does(() =>
{
    var projectFiles = GetFiles("./src/**/*.csproj").Where(x => x.FullPath.EndsWith(".Tests.csproj"));

    foreach (var project in projectFiles)
    {
        Information("Building test project '" + project.Segments.Last() + "'");
        MSBuild(project, settings =>
            settings.SetVerbosity(Verbosity.Normal)
                    .SetConfiguration("Release")
                    .WithTarget("Build")
                    .UseToolVersion(MSBuildToolVersion.VS2015)
                    .ArgumentCustomization = args => args.Append("/nologo")
            );
    }

    XUnit2("./src/**/bin/Release/*.Tests.dll");
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