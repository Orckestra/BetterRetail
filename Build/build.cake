#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"
#tool "nuget:?package=Microsoft.TypeScript.Compiler&version=3.1.5"

#addin "nuget:?package=Cake.MsDeploy&version=0.8.0"
#addin "nuget:?package=Cake.CoreCLR&version=0.35.0"
#addin "nuget:?package=Cake.Npm&version=0.17.0"
#addin "nuget:?package=Cake.Karma&version=0.2.0"
#addin "nuget:?package=Cake.Powershell&version=0.4.8"

#load "helpers/filesystem.cake"
#load "helpers/typescripts.cake"

using System;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Cake.Npm;
using Cake.Karma;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var isShowingDoc = Context.Configuration.GetValue("showtree") != null;

var target = Argument("target", "All");
var configuration = Argument("configuration", "Release");
var packageVersion = Argument("package-version", "");

if (target.ToLower() == "dev")
    configuration = "Debug";

if (!isShowingDoc)
{
    Information($"Target: {target}");
    Information($"Configuration: {configuration}");
    Information($"Package Version: {packageVersion}");
}
else
{
    Information("");
    Information("-----------------------------------------------------------------------------------------------");
    Information("-docs                     Displays available commands");
    Information("-t All                    Executes specific target, default is 'ALL'");
    Information("-configuration Release    Build configuration");
    Information("-package-version=0.0.1   Nuget package version. If not specified, taken from 'SharedAssemblyInfo.cs'");
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var rootDir = "..";
var srcDir = $"{rootDir}/src";
var testsDir = $"{rootDir}/tests";
var outputDir = $"{rootDir}/output";
var buildDir = $"{rootDir}/build";
var solutionFile = GetFiles($"{rootDir}/*.sln").Single();
var tslintFiles = $"{srcDir}/Orckestra.Composer.Website/UI.Package/Typescript/**/*.ts";
var tslintConfig = $"{buildDir}/tslint.json";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Tslint-Tests").Does(() =>
{
    StartPowershellScript("Invoke-Command", args =>
        {
            args.Append($"-ScriptBlock {{tslint {tslintFiles} --config {tslintConfig}}}");
        });
});

Task("Tslint-Fix").Does(() =>
{
    StartPowershellScript("Invoke-Command", args =>
        {
            args.Append($"-ScriptBlock {{tslint {tslintFiles} --config {tslintConfig} --fix}}");
        });
});

Task("Clean-Solution").Does(() =>
{
    DeleteDirectories($"{srcDir}/**/bin/");
    DeleteDirectories($"{srcDir}/**/obj/");
    DeleteDirectories($"{srcDir}/**/node_modules/");
    DeleteDirectories($"{srcDir}/**/_Package/");
    DeleteDirectories($"{srcDir}/**/Release/");

    DeleteDirectories($"{buildDir}/.temp");

    DeleteDirectories(outputDir);
});

Task("Compile-Solution").Does(() =>
{
      MSBuild(solutionFile, settings =>
        settings.SetConfiguration(configuration));
});


Task("Restore-NuGet-Packages").Does(() =>
{
    NuGetRestore(solutionFile);
});

Task("Restore-NPM-Packages").Does(() =>
{
	NpmInstall(settings => settings.FromPath($"{buildDir}/"));
});


Task("Run-NUnit-Tests").Does(() =>
{
    var testAssemblies = GetFiles($"{testsDir}/**/bin/{configuration}/*.Tests.dll");
    testAssemblies.Add(GetFiles($"{testsDir}/**/bin/{configuration}/*.IntegrationTests.dll"));

    NUnit3(testAssemblies, new NUnit3Settings 
    { 
        NoResults = true, 
        SkipNonTestAssemblies = true,
    });
});

Task("Clean-Typescripts-Unit-Tests").Does(() => 
{
    var filesToDelete = GetFiles($"{srcDir}/Orckestra.Composer.Website/UI.Package/**/orckestra.composer.tests.*");
    DeleteFiles(filesToDelete);
    DeleteDirectories($"{buildDir}/.temp");
});

Task("Prepare-Typescripts-Unit-Tests").Does(() =>
{
    var destPath = $"{buildDir}/.temp";
    DeleteDirectories(destPath);
    CopyDirectory($"{srcDir}/Orckestra.Composer.Website/UI.Package/Tests", $"{destPath}/Tests");
    CopyDirectory($"{srcDir}/Orckestra.Composer.Website/UI.Package/Typescript", $"{destPath}/Typescript");
    CopyDirectory($"{srcDir}/Orckestra.Composer.Website/UI.Package/Typings", $"{destPath}/Typings");
});


Task("Compile-Typescripts-Unit-Tests").Does(() =>
{
    CompileTypeScripts(rootDir, $"--project {buildDir}/tsconfigs/unittests.json", 30);
});

Task("Compile-Typescripts-TestBase").Does(() =>
{
    CompileTypeScripts(rootDir, $"--project {rootDir}/Build/tsconfigs/orckestra_tests.json", 30);
});

Task("Compile-Typescripts-Default").Does(() =>
{
    CompileTypeScripts(rootDir, $"--project {rootDir}/Build/tsconfigs/orckestra.json", 30);
});

Task("Run-Karma-Tests-Default")
.Does(() => 
{
    var settings = new KarmaStartSettings
    {
       ConfigFile = "karma.conf.js",
       
       RunMode = KarmaRunMode.Local
    };
    KarmaStart(settings);
});


Task("Run-Karma-Tests-Debug")
.Does(() => 
{
    var settings = new KarmaStartSettings
    {
       ConfigFile = "karma.conf.js",
       RunMode = KarmaRunMode.Local,
       LogLevel = KarmaLogLevel.Debug,
       SingleRun = false,
       NoSingleRun = true,
       Browsers = new List<string>(){"Chrome"}
    };
    KarmaStart(settings);
});

Task("Copy-To-Artifacts").Does(() =>
{
    var artifactsDir = $"{outputDir}/artifacts";

    CreateDirectory(artifactsDir);
    CopyFiles($"{srcDir}/**/bin/**/*.zip", artifactsDir);
});

Task("Create-NuGet-Package").Does(() => 
{
    var dependencies = XDocument
        .Load($"{srcDir}/Orckestra.Composer/packages.config")
        .Descendants("package")
        .Where(x => x.Attribute("developmentDependency") == null
            || !bool.Parse(x.Attribute("developmentDependency").Value))
        .Select(x => new NuSpecDependency
        {
            Id = x.Attribute("id").Value,
            Version = x.Attribute("version").Value,
            TargetFramework = "net471"
        })
        .ToArray();

    var version = packageVersion;
    if (string.IsNullOrEmpty(version))
    {
        var versionMatch = Regex.Match(
            System.IO.File.ReadAllText($"{srcDir}/Solution Items/SharedAssemblyInfo.cs"), 
            @"AssemblyVersion\s*\(\s*""([0-9\.\*]*?)""\s*\)");

        if (versionMatch.Success)
            version = versionMatch.Groups[1].Value;
    }
    if (string.IsNullOrEmpty(version))
    {
        version = "0.0.0";
    }

    var nuGetPackSettings = new NuGetPackSettings 
    {
        Version = version,
        Dependencies = dependencies,
        BasePath = rootDir,
        OutputDirectory = outputDir,
    };

     NuGetPack($"{rootDir}/Build/nuspec/Orckestra.ReferenceApplication.nuspec", nuGetPackSettings);
});

//////////////////////////////////////////////////////////////////////
// GROUPS
//////////////////////////////////////////////////////////////////////

Task("Prepare-Karma-Tests")
.IsDependentOn("Clean-Typescripts-Unit-Tests")
.IsDependentOn("Compile-Typescripts-TestBase")
.IsDependentOn("Prepare-Typescripts-Unit-Tests")
.IsDependentOn("Compile-Typescripts-Unit-Tests");

Task("Build")
    .IsDependentOn("Clean-Solution")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Compile-Solution")
    .IsDependentOn("Restore-NPM-Packages")
    .IsDependentOn("Compile-Typescripts-Default");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("NUnit-Tests")
.Description("Unit tests to control projects source code")
.IsDependentOn("Clean-Solution")
.IsDependentOn("Compile-Solution")
.IsDependentOn("Run-NUnit-Tests");

Task("Karma-Tests")
.Description("Unit tests to control typescripts")
.IsDependentOn("Prepare-Karma-Tests")
.IsDependentOn("Run-Karma-Tests-Default");

Task("Karma-Debug")
.Description("Task to run Karma unit tests in debug mode")
.IsDependentOn("Prepare-Karma-Tests")
.IsDependentOn("Run-Karma-Tests-Debug");

Task("Compile-Typescripts")
.Description("Task to compile default orckestra.composer.js javascripts")
.IsDependentOn("Compile-Typescripts-Default");

Task("Tests")
    .Description("Task to run all available tests")
    .IsDependentOn("NUnit-Tests")
    .IsDependentOn("Karma-Tests");

Task("Artifacts")
    .Description("Task to copy artifacts")
    .IsDependentOn("Copy-To-Artifacts");

Task("Package")
    .Description("Task to create a package")
    .IsDependentOn("Create-NuGet-Package");

Task("All")
    .Description("Executed on build server, default task")
    .IsDependentOn("Build")
    .IsDependentOn("Tests")
    .IsDependentOn("Artifacts")
    .IsDependentOn("Package");

Task("Dev")
    .Description("Should be used on dev machine only. Triggered before local install")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Compile-Solution")
    .IsDependentOn("Restore-NPM-Packages")
    .IsDependentOn("Compile-Typescripts-Default")
    .IsDependentOn("Artifacts");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);