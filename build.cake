#addin "Cake.Npm"
#tool "nuget:?package=GitVersion.CommandLine"

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
            settings.WithArguments("--codeCoverage=true --progress=true --watch=false --browsers ChromeHeadless");
        }
        NpmRunScript(settings);
    });

//https://cakebuild.net/api/Cake.Common.Tools.GitVersion/GitVersionAliases/4B9950AF
Task("UpdateVersionInfo")
    .IsDependentOn("Test")
    .Does(() =>
        {
            var result = GitVersion(new GitVersionSettings {});
            var semver = result.NuGetVersionV2;
            Information(logAction=>logAction("SemVer : " + semver));
            var settings = new NpmRunScriptSettings {
                ScriptName = "version"
            };
            settings.WithArguments(semver + " --force");
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
Task("Default")
    .IsDependentOn("NpmPack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);