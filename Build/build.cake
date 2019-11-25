#tool nuget:https://www.nuget.org/api/v2?package=NUnit.ConsoleRunner&version=3.10.0
#addin nuget:https://www.nuget.org/api/v2?package=Cake.MsDeploy&version=0.8.0
#addin nuget:https://www.nuget.org/api/v2?package=Cake.CoreCLR&version=0.35.0

#load "scripts/extensions.cake"

using System.Xml.Linq;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "All");
var configuration = Argument("configuration", "Release");
var packageVersion = Argument("package-version", "");

Information($"Target: {target}");
Information($"Configuration: {configuration}");
Information($"Package Version: {packageVersion}");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var rootDir = "..";
var srcDir = $"{rootDir}/src";
var installerGenericPackagesDir = $"{rootDir}/Installer/packages/generic";

var solutionFile = GetFiles($"{rootDir}/*.sln").Single();

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    Context.DeleteDirectories($"{srcDir}/**/bin/");
    Context.DeleteDirectories($"{srcDir}/**/obj/");

    Context.DeleteDirectories($"{srcDir}/**/node_modules/");
    Context.DeleteDirectories($"{srcDir}/**/Composer/UI.Package/");
    Context.DeleteDirectories($"{srcDir}/**/Composer.CompositeC1.Mvc/UI.Package/");

    Context.DeleteDirectories($"{srcDir}/**/_Package/");
    Context.DeleteDirectories($"{srcDir}/**/Release/");

    Context.DeleteDirectories(installerGenericPackagesDir);
    Context.DeleteDirectories($"{rootDir}/Build/output");
});


Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore(solutionFile);
});


Task("Compile")
    .Does(() =>
{
      MSBuild(solutionFile, settings =>
        settings.SetConfiguration(configuration));
});


Task("Run-NUnit-Tests")
    .Does(() =>
{
    var testAssemblies = GetFiles($"{srcDir}/**/bin/{configuration}/*.Tests.dll");
    testAssemblies.Add(GetFiles($"{srcDir}/**/bin/{configuration}/*.IntegrationTests.dll"));

    NUnit3(testAssemblies, new NUnit3Settings 
    { 
        NoResults = true, 
        SkipNonTestAssemblies = true,
    });
});


Task("Copy-To-Installer")
    .Does(() =>
{
    var installerRefAppDir = $"{installerGenericPackagesDir}/C1CMS/RefApp";

    CreateDirectory(installerRefAppDir);
    CopyFiles($"{srcDir}/**/bin/**/*.zip", installerRefAppDir);
});


Task("Copy-DeploymentFolder")
    .Does(() =>
{
    var deploymentBaseFolder = "../src/Composer.CompositeC1/_Published/Composer.CompositeC1.Mvc";
    var deploymentFolder = $"{deploymentBaseFolder}/Deployment";

    CreateDirectory(deploymentFolder);
    CopyFiles($"{srcDir}/Composer.CompositeC1/Composer.CompositeC1.Mvc/Deployment/*.*", deploymentFolder);

    var settings = new MsDeploySettings
    {
        Verb = Operation.Sync,
        Source = new ContentPathProvider
        {
            Direction = Direction.source,
            Path = MakeAbsolute(File(deploymentBaseFolder)).ToString()
        },
        Destination = new PackageProvider
        {
            Direction = Direction.dest,
            Path = MakeAbsolute(File($"{installerGenericPackagesDir}/C1CMS/DeploymentFolder.zip")).ToString()
        },
    };

    MsDeploy(settings);
});


Task("Create-NuGet-Package")
    .Does(() => 
{
    var dependencies = XDocument
        .Load($"{srcDir}/Composer/Composer/packages.config")
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
        OutputDirectory = $"{rootDir}/Build/output"
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
    .IsDependentOn("Run-NUnit-Tests");

Task("Artifacts")
    .IsDependentOn("Copy-To-Installer")
    .IsDependentOn("Copy-DeploymentFolder");

Task("Package")
    .IsDependentOn("Create-NuGet-Package");

Task("All")
    .IsDependentOn("Build")
    //.IsDependentOn("Tests")
    .IsDependentOn("Artifacts")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
