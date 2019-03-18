#tool paket:?package=codecov
#tool paket:?package=gitlink
#tool paket:?package=GitVersion.CommandLine&prerelease
#addin paket:?package=Cake.Paket
#addin paket:?package=Cake.Codecov
#tool paket:?package=JetBrains.dotCover.CommandLineTools
#tool paket:?package=ReportGenerator&version=4.0.4

var target = Argument("target", "Build");
var configuration = Argument("Configuration", "Debug");
var version = Argument("NuGetVersion", "");

Task("CI")
    .IsDependentOn("Pack")
    .IsDependentOn("Codecov");

Task("Pack")
    .IsDependentOn("Build")
    .IsDependentOn("_pack");

Task("_pack")
    .WithCriteria(configuration.Equals("Release", StringComparison.OrdinalIgnoreCase))
    .Does(() => {
        PaketPack("nugets", new PaketPackSettings {
            Version = version
        });
    });

Task("GitVersion")
    .WithCriteria(BuildSystem.IsLocalBuild && string.IsNullOrWhiteSpace(version))
    .Does(() => {
        version = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
        }).NuGetVersion;
    });

Task("Build")
    .IsDependentOn("GitVersion")
    .Does(() => {
        DotNetCoreBuild("Nancy.Routing.UriTemplates.sln", new DotNetCoreBuildSettings {
            Configuration = configuration
        });
    })
    .DoesForEach(GetFiles("**/Nancy.Routing.UriTemplates.pdb"), 
        pdb => GitLink3(pdb, new GitLink3Settings {
                RepositoryUrl = "https://github.com/tpluscode/Nancy.Routing.UriTemplates",
            }));

Task("Codecov")
    .IsDependentOn("Test")
    .Does(() => {
        Codecov("coverage\\cobertura.xml");
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        if(DirectoryExists("coverage")) 
            CleanDirectories("coverage"); 
    })
    .Does(CoverTests("Nancy.Routing.UriTemplates.Tests"))
    .Does(CoverTests("Nancy.Routing.UriTemplates.Tests.Functional"))
    .Does(() => {
        DotCoverMerge(GetFiles("coverage\\*.dcvr"), "coverage\\merged.dcvr");
    })
    .Does(() => {        
        DotCoverReport(
          "./coverage/merged.dcvr",
          "./coverage/dotcover.xml",
          new DotCoverReportSettings {
            ReportType = DotCoverReportType.DetailedXML,
          });
    })
    .Does(() => {
        StartProcess(
          @".\packages\tools\ReportGenerator\tools\net47\ReportGenerator.exe",
          @"-reports:.\coverage\dotcover.xml -targetdir:.\coverage -reporttypes:Cobertura;html -assemblyfilters:-UriTemplateString;-Newtonsoft.Json;-FluentAssertions;-xunit*;-Nancy.Routing.UriTemplates.Tests*");
    });

public Action CoverTests(string name)
{
    var settings = new DotNetCoreTestSettings
            {
                Configuration = configuration,
                NoBuild = true,
            };

    return () => {
        DotCoverCover(
          ctx => ctx.DotNetCoreTest(GetFiles($"**\\{name}.csproj").Single().FullPath, settings),
          $"./coverage/{name}.dcvr",
          new DotCoverCoverSettings());
    };
}

RunTarget(target);
