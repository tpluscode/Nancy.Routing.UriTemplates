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
        Codecov("coverage\\cobertura.xml");
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        if(DirectoryExists("coverage")) 
            CleanDirectories("coverage"); 
    })
    .Does(() => {
        var openCoverSettings = new OpenCoverSettings
        {
            OldStyle = true,
            MergeOutput = true,
            MergeByHash = true,
            Register = "user",
            ReturnTargetCodeOffset = 0
        }
        .WithFilter("+[Nancy.Routing.UriTemplates]*");

         DotCoverAnalyse(
            ctx => ctx.DotNetCoreTest(GetFiles($"**\\Nancy.Routing.UriTemplates.sln").Single().FullPath, new DotNetCoreTestSettings
                {
                    Configuration = configuration,
                    NoBuild = true,
                }),
            "./coverage/dotcover.xml",
            new DotCoverAnalyseSettings {
                ReportType = DotCoverReportType.DetailedXML,
            });
    })
    .Does(() => {
        StartProcess(
          @".\packages\tools\ReportGenerator\tools\net47\ReportGenerator.exe",
          @"-reports:.\coverage\dotcover.xml -targetdir:.\coverage -reporttypes:Cobertura;html -assemblyfilters:-UriTemplateString;-Newtonsoft.Json;-FluentAssertions;-xunit*;-Nancy.Routing.UriTemplates.Tests*");
    });

RunTarget(target);
