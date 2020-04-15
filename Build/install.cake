#addin "nuget:?package=Cake.CoreCLR&version=0.35.0"
#addin "nuget:?package=Cake.IIS&version=0.4.2"
#addin "nuget:?package=Cake.Hosts&version=1.5.1"
#addin "nuget:?package=Cake.Powershell&version=0.4.8"
#addin "nuget:?package=System.Reflection.TypeExtensions&version=4.6.0"
#addin "nuget:?package=System.ServiceProcess.ServiceController&version=4.7.0"
#addin "nuget:?package=Microsoft.Web.Administration&version=11.1.0"
#addin "nuget:?package=Microsoft.Win32.Registry&version=4.6.0"

#tool "nuget:?package=OrckestraCommerce.Website.SetupServer&version=0.0.4"

#load "helpers/filesystem.cake"
#load "helpers/certificates.cake"
#load "helpers/webrequests.cake"
#load "helpers/cakeconfig.cake"
#load "helpers/process.cake"
#load "helpers/symboliclink.cake"
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

var setupPort = 9876;
var setupServer = GetFiles($"{rootDir}/build/tools/**/*SetupServer.exe").Last();

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////


Dictionary<string, string> Parameters;
string localSiteName = null;
Task("Load-CakeConfig").Does(() =>
{
    var config = CreateCakeConfig()
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
        Information($"{{{kvPair.Key}}}: \"{kvPair.Value}\"");
    }

    var requiredParameters = new [] { "C1Url", "websiteName", "setupDescription", "websiteUrl", "adminName", "adminEmail", "adminPassword", "baseCulture" };
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

Task("Kill-Processes").Does(() =>
{
    var processes = Process.GetProcessesByName(setupServer.GetFilenameWithoutExtension().ToString());
    foreach (var process in processes)
    {
        process.Kill();
    }
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

Task("Modify-Configs").Does(() =>
{
    XmlPoke($"{websiteDir}/App_Data/Composite/Composite.config", "/configuration/Composite.SetupConfiguration/@PackageServerUrl", $"http://localhost:{setupPort}");
    StopSite(localSiteName);
    StartSite(localSiteName);
});

Process _process = null;

Task("Start-SetupServer").Does(() => 
{
    var setupTemplate = Parameters["setupDescription"];
    _process = StartHiddenProcess(setupServer.ToString(), "--port", setupPort, "--template" ,setupTemplate, "--branch", branch);
});

Task("Select-Package").Does(() => 
{
    MakeRequest($"{Parameters["websiteUrl"]}/Composite/top.aspx");

    if (_process.HasExited)
    {
        throw new Exception("Setup server has stopped unexpectedly. Make sure you have 'dotnet core 3.1' installed");
    }        

    var script = $@"
$setupServiceWsdlUri = '{Parameters["websiteUrl"]}/Composite/services/Setup/SetupService.asmx?WSDL'
$baseCulture = '{Parameters["baseCulture"]}'

$proxy = New-WebServiceProxy -Uri $setupServiceWsdlUri 

Write-Output 'mapped wsdl proxy...'

$proxy.Timeout = 5 * 60 * 1000
$setupDescription = $proxy.GetSetupDescription('true') 
$xml = New-Object -TypeName xml
$xml.AppendChild($xml.ImportNode($setupDescription, $true)) | Out-Null

echo 'Setup chosen:'
$xml.setup

$setupXml = (($xml.setup.radio)).OuterXml

$selection = '<setup>' + $setupXml + '</setup>'
echo 'Setup chosen:'
$selection
echo 'Running setup...'

$setupResult = $proxy.SetUp($selection, '{Parameters["adminName"]}','{Parameters["adminEmail"]}','{Parameters["adminPassword"]}',$baseCulture,$baseCulture,'false')
echo 'Setup result:'
$setupResult;

if (!$setupResult)
{{
    throw 'Website setup has failed'
}}
";
    try
    {
        var result = StartPowershellScript(script, new PowershellSettings()
            .SetFormatOutput()
            .SetLogOutput());
    }
    catch
    {
        if (_process != null && !_process.HasExited)
            _process.Kill();

        throw;
    }
});

Task("Stop-SetupServer").Does(() => 
{
    if (_process != null)
    {
        _process.Kill();
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

Task("Install-Secondary-Language").Does(() => 
{
    var autoInstallDir = $"{websiteDir}/App_Data/Composite/AutoInstallPackages";
    var sourcePackage = $"{outputDir}/artifacts/Orckestra.Composer.C1.Content.FR-CA.zip";
    
    CreateDirectory(autoInstallDir);
    CopyFiles(sourcePackage, autoInstallDir);

    StopSite(localSiteName);
    StartSite(localSiteName);
    MakeRequest($"{Parameters["websiteUrl"]}/Composite/top.aspx");
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

    StopSite(localSiteName);
    StartSite(localSiteName);

    MakeRequest($"{Parameters["websiteUrl"]}/Composite/top.aspx");
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
    .IsDependentOn("Kill-Processes")
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
    .IsDependentOn("Modify-Configs")
    .IsDependentOn("Start-SetupServer")
    .IsDependentOn("Select-Package")
    .IsDependentOn("Stop-SetupServer")
    .IsDependentOn("Patch-ExperienceManagement-Config")
    .IsDependentOn("Install-Secondary-Language");

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

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
