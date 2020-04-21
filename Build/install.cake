#addin "nuget:?package=Cake.CoreCLR&version=0.35.0"
#addin "nuget:?package=Cake.IIS&version=0.4.2"
#addin "nuget:?package=Cake.Hosts&version=1.5.1"
#addin "nuget:?package=Cake.Powershell&version=0.4.8"
#addin "nuget:?package=System.Reflection.TypeExtensions&version=4.6.0"
#addin "nuget:?package=System.ServiceProcess.ServiceController&version=4.7.0"
#addin "nuget:?package=Microsoft.Web.Administration&version=11.1.0"
#addin "nuget:?package=Microsoft.Win32.Registry&version=4.6.0"


#load "helpers/filesystem.cake"
#load "helpers/certificates.cake"
#load "helpers/webrequests.cake"
#load "helpers/cakeconfig.cake"
#load "helpers/process.cake"
#load "helpers/symboliclink.cake"
#load "helpers/packageinstaller.cake"
#load "helpers/formating.cake"

using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.PowerShell.Commands;
using Microsoft.Web.Administration;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var isShowingDoc = Context.Configuration.GetValue("showtree") != null;

var target = Argument("target", "All");
var environment = Argument<string>("env", null);
var branch = Argument<string>("branch", "develop");

if (!isShowingDoc)
{
    Information($"Target: {target}");
    Information($"Environment: {environment}");
    Information($"Branch: {branch}");
}
else
{
    Information("");
    Information("-----------------------------------------------------------------------------------------------");
    Information("-docs                     Displays available commands");
    Information("-t All                    Executes specific target, default is 'ALL'");
    Information("-env=INT2                 Use environment from configuration. If not suplied, default is used");
    Information("-branch=develop           Branch of Experience Management, default is 'develop'");
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var rootDir = MakeAbsolute(DirectoryPath.FromString("..")).FullPath;
var outputDir = $"{rootDir}/output";
var cacheDir = $"{rootDir}/Build/.cache";
var C1File = $"{cacheDir}/C1.zip";

var deploymentDir = $"{rootDir}/deployment";
var websiteDir = $"{deploymentDir}/Website";

var directoryName = DirectoryPath.FromString(rootDir).GetDirectoryName();

Func<string,string, string> secure = (key, value) => new[] { "ocsAuthToken" }.Contains(key) ? "<hidden>" : value;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////


Dictionary<string, string> Parameters;
string localSiteName = null;
Task("Load-CakeConfig").Does(() =>
{
    var config = CreateCakeConfig(environment)
        .UseFile($"{rootDir}/build/configuration/parameters.json")
        .UseFile($"{rootDir}/../ref.app.parameters.json")
        .UseFile($"{rootDir}/build/configuration/parameters.local.json");
    
    if (!string.IsNullOrEmpty(environment))
    {
        config = config
            .UseFile($"{rootDir}/build/configuration/parameters.{environment}.json")
            .UseFile($"{rootDir}/../ref.app.parameters.{environment}.json")
            .UseFile($"{rootDir}/build/configuration/parameters.{environment}.local.json");
    }

    Parameters = config
        .UseRootPath(rootDir)
        .Build();

    Information("");
    Information("PARAMETERS:");
    foreach (var kvPair in Parameters)
    {
        Information($"{{{kvPair.Key}}}: \"{secure(kvPair.Key, kvPair.Value)}\"");
    }

    var requiredParameters = new [] { "C1Url", "websiteName", "setupDescription", "websiteUrl", "baseCulture" };
    var hasError = false;
    foreach (var requiredParameter in requiredParameters)
    {
        if (!Parameters.TryGetValue(requiredParameter, out var value))
        {
            Error($"Parameter '{requiredParameter}' is missing");
            hasError = true;
        }
        else if (string.IsNullOrWhiteSpace(value))
        {
            Error($"Parameter '{requiredParameter}' is empty");
            hasError = true;
        }
    }
    if (hasError)
    {
        Error(@$"Parameters can be overridden in:
    ../ref.app.parameters.json
    build/configuration/parameters.local.json
    ../ref.app.parameters.[env].json
    build/configuration/parameters.[env].local.json
    ");
        throw new Exception("Required parameter is missing");
    }

    localSiteName = Parameters["websiteName"];
});


#region Uninstall

Task("Remove-Website").Does(() =>
{
    using (ServerManager manager = BaseManager.Connect(""))
    {
        var site = manager.Sites[localSiteName];
        if (site != null)
        {
            manager.Sites.Remove(site);
            manager.CommitChanges();
        }
    }
});

Task("Remove-ApplicationPool").Does(() =>
{
    DeletePool(localSiteName);
});

Task("Remove-From-Hosts-File").Does(() =>
{
    RemoveHostsRecord("127.0.0.1", localSiteName);
});

Task("Clean-Deployment-Folder").Does(() =>
{
    DeleteDirectories(deploymentDir);
});

#endregion


#region Install-C1
Task("Download-C1").Does(() =>
{
    string url = Parameters["C1Url"];
    string downloadingMessage = $"Downloading C1 CMS by the URL {url}.";
    bool getInfoRes = GetServerFileInfo(url, out DateTime modifiedOnServer, out long sizeOnServer);
    if (getInfoRes == false)
    {
        Information(downloadingMessage);
        CreateDirectory(FilePath.FromString(C1File).GetDirectory());
        DownloadFile(url, C1File); 
        return;
    }
    
    if (!System.IO.File.Exists(C1File))
    {
        Information($"Cannot find C1 CMS on path {C1File}.");
        Information(downloadingMessage);
        CreateDirectory(FilePath.FromString(C1File).GetDirectory());
        DownloadFile(url, C1File);
        Information($"File modified on the server on {FormatDate(modifiedOnServer)}");
        System.IO.File.SetLastWriteTime(C1File, modifiedOnServer);
        return;
    }

    FileInfo fi = new System.IO.FileInfo(C1File);
    DateTime modifiedLocal = fi.LastWriteTime;
    long sizeLocal = fi.Length;

    if (modifiedLocal != modifiedOnServer || sizeLocal != sizeOnServer)
    {
        Information($"Server file modification timestamp: {FormatDate(modifiedOnServer)}");
        Information($"Local file modification timestamp: {FormatDate(modifiedLocal)}");
        Information($"Server file size: {sizeOnServer}");
        Information($"Local file size: {sizeLocal}");
        Information(downloadingMessage);
        DownloadFile(url, C1File);
        System.IO.File.SetLastWriteTime(C1File, modifiedOnServer);
        return;
    }
    else
    {
       Information($"Last C1 CMS file with {sizeLocal} size and {FormatDate(modifiedLocal)} modification timestamp was found by the local path {C1File}"); 
    }
});

Task("Unpack-C1").Does(() =>
{
    Unzip(C1File, deploymentDir);
});

Task("Create-ApplicationPool").Does(() =>
{
    CreatePool(new ApplicationPoolSettings()
    {
        Name = localSiteName,
        IdentityType = IdentityType.ApplicationPoolIdentity
    });
});

Task("Create-Website").Does(() =>
{
    CreateWebsite(new WebsiteSettings()
    {
        Name = localSiteName,
        PhysicalDirectory = websiteDir,
        ApplicationPool = new ApplicationPoolSettings()
        {
            Name = localSiteName
        },
        Binding = IISBindings.Http
                    .SetHostName(localSiteName)
                    .SetIpAddress("*"),
    });
});

Task("Create-Https-Binding").Does(() =>
{
    var certificate = GetCertificate("*.develop.orckestra.cloud");

    AddBinding(localSiteName, 
        IISBindings.Https
            .SetHostName(localSiteName)
            .SetIpAddress("*")
            .SetCertificateHash(certificate.GetCertHash())
            .SetCertificateStoreName("My")
    );
});

Task("Add-To-Hosts-File").Does(() => 
{
    AddHostsRecord("127.0.0.1", localSiteName);
});

#endregion

#region Install-RefApp

Task("Install-Packages").Does(() => 
{
    using(var installer = GetPackageInstaller(websiteDir)) {
        installer.InstallPackagesFromConfig(Parameters["baseCulture"], Parameters["setupDescription"], branch);
    }
});

Task("Patch-ExperienceManagement-Config")
    .IsDependentOn("Load-CakeConfig")
    .Does(() =>  
{
    var webConfig = $"{websiteDir}/App_Config/ExperienceManagement.config";
    var doc = XDocument.Load(webConfig);
    var elements = doc.Descendants("add").ToList();

    foreach (var element in elements)
    {
        var valueAttribute = element.Attribute("value");
        var value = valueAttribute.Value;
        var match = Regex.Match(value, @"{(?<expression>.+)}");
        if (match.Success)
        { 
            var expression = match.Groups["expression"].Value;
            if (Parameters.TryGetValue(expression, out var substituteValue))
            {
                var newValue = value.Replace($"{{{expression}}}", substituteValue);
                valueAttribute.Value = newValue;
            }
            else 
            {
                Warning($"Can't substitute {value}");
            }
        }
   }
   doc.Save(webConfig);
});

Task("Install-Secondary-Packages").Does(() => 
{
    if(!string.IsNullOrWhiteSpace(Parameters["setupDescriptionSecondary"])) {
        using(var installer = GetPackageInstaller(websiteDir)) {
            installer.InstallPackagesFromConfig(Parameters["baseCulture"], Parameters["setupDescriptionSecondary"], branch);
        }
    }
});

#endregion

#region Configure-Local-Debug

Task("Patch-csproj.user").Does(() =>
{
    var userFile = $"{rootDir}/build/configuration/Orckestra.Composer.Website.csproj.user";
    var content = System.IO.File.ReadAllText(userFile);
    content = content.Replace("{CustomServerUrl}", Parameters["websiteUrl"]);

    var dest = $"{rootDir}/src/Orckestra.Composer.Website/Orckestra.Composer.Website.csproj.user";
    if (FileExists(dest))
    {
        DeleteFile(dest);
    }
    
    System.IO.File.WriteAllText(dest, content);
});

Task("Configure-Symbolic-Links").Does(() =>
{
	StopPool(localSiteName);
    ReplaceDirWithSymbolicLink($"{websiteDir}/UI.Package/Sass", $"{rootDir}/src/Orckestra.Composer.Website/UI.Package/Sass");
	ReplaceDirWithSymbolicLink($"{websiteDir}/UI.Package/Templates", $"{rootDir}/src/Orckestra.Composer.Website/UI.Package/Templates");
	ReplaceDirWithSymbolicLink($"{websiteDir}/UI.Package/LocalizedStrings", $"{rootDir}/src/Orckestra.Composer.Website/UI.Package/LocalizedStrings");
	ReplaceDirWithSymbolicLink($"{websiteDir}/UI.Package/Typescript", $"{rootDir}/src/Orckestra.Composer.Website/UI.Package/Typescript");
	StartPool(localSiteName);
});

Task("Modify-Configs-For-Debug").Does(() =>
{
    XmlPoke($"{websiteDir}/web.config", "/configuration/system.web/compilation/@debug", "true");
    XmlPoke($"{websiteDir}/web.config", "/configuration/system.web/caching/outputCacheSettings/outputCacheProfiles/add/@enabled", "false");
});


#endregion

Task("Open-Website").Does(() =>
{
    Process.Start($"{Parameters["websiteUrl"]}/Composite/top.aspx");
});


Task("Link-Razor").Does(() =>
{
    Information("Link-Razor task");
   
	var srcRazorDir = $"{rootDir}/src/Orckestra.Composer.Website/App_Data/Razor";
    var targetRazorPath = new DirectoryPath($"{deploymentDir}/Website/App_Data/Razor");
    var srcRazorPath = new DirectoryPath(srcRazorDir);
    var files = GetFiles($"{srcRazorDir}/**/*.cshtml");
    foreach(var file in files)
    {
        var razorFile = srcRazorPath.GetRelativePath(file);
        var targetFile = targetRazorPath.CombineWithFilePath(razorFile);
        
        Information("RazorFile: {0}", razorFile);
        ReplaceFileWithHardLink(targetFile.FullPath, file.FullPath);
   }

});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Uninstall")
    .IsDependentOn("Load-CakeConfig")
    .IsDependentOn("Remove-Website")
    .IsDependentOn("Remove-ApplicationPool")
    .IsDependentOn("Remove-From-Hosts-File")
    .IsDependentOn("Clean-Deployment-Folder");

Task("Install-C1")
    .IsDependentOn("Load-CakeConfig")
    .IsDependentOn("Download-C1")
    .IsDependentOn("Unpack-C1")
    .IsDependentOn("Create-ApplicationPool")
    .IsDependentOn("Create-Website")
    .IsDependentOn("Create-Https-Binding")
    .IsDependentOn("Add-To-Hosts-File");

Task("Install-RefApp")
    .IsDependentOn("Load-CakeConfig")
    .IsDependentOn("Install-Packages")
    .IsDependentOn("Patch-ExperienceManagement-Config")
    .IsDependentOn("Install-Secondary-Packages");

Task("Install")
    .IsDependentOn("Install-C1")
    .IsDependentOn("Install-RefApp");

Task("Configure-Local-Debug")
    .IsDependentOn("Load-CakeConfig")
    .IsDependentOn("Patch-csproj.user")
    .IsDependentOn("Configure-Symbolic-Links")
	.IsDependentOn("Link-Razor")
    .IsDependentOn("Modify-Configs-For-Debug");


Task("All")
    .Description("Only this task should be used, everything else subtasks")
    .IsDependentOn("Uninstall")
    .IsDependentOn("Install")
    .IsDependentOn("Configure-Local-Debug")
    .IsDependentOn("Open-Website");


Task("Link")
    .Description("Should be used on dev machine only. Used for link dev files")
    .IsDependentOn("Link-Razor");


Task("Build-Website")
    .IsDependentOn("Load-CakeConfig")
    .IsDependentOn("Clean-Deployment-Folder")
    .IsDependentOn("Download-C1")
    .IsDependentOn("Unpack-C1")
    .IsDependentOn("Install-RefApp");


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
