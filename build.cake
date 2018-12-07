#tool paket:?package=codecov
#tool paket:?package=gitlink
#tool paket:?package=GitVersion.CommandLine&prerelease
#addin paket:?package=Cake.Paket
#addin paket:?package=Cake.Codecov
#tool paket:?package=JetBrains.dotCover.CommandLineTools
#tool paket:?package=ReportGenerator&version=4.0.4

var target = Argument("target", "Build");
var configuration = Argument("Configuration", "Debug");

GitVersion version;

Task("CI")
    .IsDependentOn("Pack")
    .IsDependentOn("Codecov").Does(() => {});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() => {
        PaketPack("nugets", new PaketPackSettings {
            Version = version.NuGetVersion
        });
    });

Task("GitVersion")
    .Does(() => {
        version = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
        });

        if (BuildSystem.IsLocalBuild == false) 
        {
            GitVersion(new GitVersionSettings {
                OutputType = GitVersionOutput.BuildServer
            });
        }
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
        var buildVersion = string.Format("{0}.build.{1}",
            version.FullSemVer,
            BuildSystem.AppVeyor.Environment.Build.Version
        );
        var settings = new CodecovSettings {
            Files = new[] { "coverage\\cobertura.xml" },
            EnvironmentVariables = new Dictionary<string,string> { { "APPVEYOR_BUILD_VERSION", buildVersion } }
        };
        Codecov(settings);
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
