#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"
#addin "nuget:?package=Cake.MsDeploy&version=0.8.0"
#addin "nuget:?package=Cake.CoreCLR&version=0.35.0"
#addin "nuget:?package=Cake.Npm&version=0.17.0"

#load "helpers/filesystem.cake"

using System.Xml.Linq;
using System.Text.RegularExpressions;
using Cake.Npm;

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

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean").Does(() =>
{
    DeleteDirectories($"{srcDir}/**/bin/");
    DeleteDirectories($"{srcDir}/**/obj/");

    DeleteDirectories($"{srcDir}/**/node_modules/");

    DeleteDirectories($"{srcDir}/**/_Package/");
    DeleteDirectories($"{srcDir}/**/Release/");

    DeleteDirectories(outputDir);
});


Task("Restore-NuGet-Packages").Does(() =>
{
    NuGetRestore(solutionFile);
});


Task("Compile").Does(() =>
{
      MSBuild(solutionFile, settings =>
        settings.SetConfiguration(configuration));
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

//TODO: check - delete NPM modules which we don't use, update outdated packages
Task("Load-NPM-Modules").Does(() =>
{
	NpmInstall(settings => settings.FromPath($"{buildDir}/"));
});

Task("Run-Composer-UI-UnitTests").Does(() =>
{
    var settings = new NpmRunScriptSettings 
    {
        WorkingDirectory = $"{buildDir}/",
        ScriptName = "unitTests",
    };
    NpmRunScript(settings);
});

Task("Compile-Typescripts").Does(() =>
{
    var settings = new NpmRunScriptSettings 
    {
        WorkingDirectory = $"{buildDir}/",
        ScriptName = "scripts",
    };
    NpmRunScript(settings);
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
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Compile");

Task("Tests")
    .IsDependentOn("Run-NUnit-Tests")
	.IsDependentOn("Load-NPM-Modules")
    .IsDependentOn("Run-Composer-UI-UnitTests");

Task("Frontend")
    .IsDependentOn("Load-NPM-Modules")
    .IsDependentOn("Compile-Typescripts");

Task("Artifacts")
    .IsDependentOn("Copy-To-Artifacts");

Task("Package")
    .IsDependentOn("Create-NuGet-Package");

Task("All")
    .Description("Executed on build server")
    .IsDependentOn("Build")
    .IsDependentOn("Tests")
    .IsDependentOn("Frontend")
    .IsDependentOn("Artifacts")
    .IsDependentOn("Package");

Task("Dev")
    .Description("Should be used on dev machine only. Triggered before local install")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Compile")
    .IsDependentOn("Artifacts");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
