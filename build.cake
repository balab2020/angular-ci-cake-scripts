#addin "Cake.Npm"
#tool "nuget:?package=GitVersion.CommandLine"
#addin nuget:?package=Cake.Git

var target = Argument("target", "Default");
var prod = Argument("prod", "true");
var isProduction = prod == "true";

Task("NpmCI")
    .Does(() => {
        var settings = new NpmCiSettings();
        settings.LogLevel = NpmLogLevel.Error;
        NpmCi(settings);
    });

Task("Clean")
    .IsDependentOn("NpmCI")
    .Does(() => {
        Information(logAction=>logAction("Running build for {0} environment at time", isProduction ? "production" : "development"));
        NpmRunScript("clean");
    });

Task("Lint")
    .IsDependentOn("Clean")
    .Does(() => {
        NpmRunScript("lint");
    });

Task("Build")
    .IsDependentOn("Lint")
    .Does(() => {
        var settings = new NpmRunScriptSettings {
            ScriptName = "build"
        };
        if(isProduction){
            settings.WithArguments("--prod=true --sourceMap=false");
        } else {
            settings.WithArguments("--prod=false --sourceMap=true");
        }
        NpmRunScript(settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        var settings = new NpmRunScriptSettings {
            ScriptName = "test"
        };
        if(isProduction){
            settings.WithArguments("--ci --coverage=true");
        }
        NpmRunScript(settings);
    });

Task("E2E-Test")
    .IsDependentOn("Test")
    .Does(() => {
        var settings = new NpmRunScriptSettings {
            ScriptName = "e2e"
        };
        NpmRunScript(settings);
    });

//https://cakebuild.net/api/Cake.Common.Tools.GitVersion/GitVersionAliases/4B9950AF
Task("UpdateVersionInfo")
    .Does(() => {
            var result = GitVersion(new GitVersionSettings {});
            var semver = result.NuGetVersionV2;
            Information(logAction=>logAction("SemVer : " + semver));
            var settings = new NpmRunScriptSettings {
                ScriptName = "version"
            };
            settings.WithArguments(semver + " --force --allow-same-version");
            NpmRunScript(settings);
        });
Task("NpmPack")
    .IsDependentOn("UpdateVersionInfo")
    .Does(() => {
        var packSettings = new NpmRunScriptSettings {
            ScriptName = "pack"
        };
        packSettings.WithArguments("--allow-same-version");
        NpmRunScript(packSettings);
    });

Task("GitRemotePush") 
    .IsDependentOn("NpmPack")
    .Does(() => {
        var result = GitVersion(new GitVersionSettings {});
        var semver = result.NuGetVersionV2;
        GitAddAll(".");
        GitCommit(".", "ci-bot", "balamuruganb2020@gmail.com", $"commit from build with version {semver} by ci");
        GitPush(".");
    });

Task("Default")
    .IsDependentOn("GitRemotePush");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);