#addin "nuget:?package=Cake.CoreCLR&version=0.35.0"
#addin "nuget:?package=Cake.IIS&version=0.4.2"
#addin "nuget:?package=Cake.Hosts&version=1.5.1"
#addin "nuget:?package=Cake.Powershell&version=0.4.8"
#addin "nuget:?package=System.Reflection.TypeExtensions&version=4.6.0"
#addin "nuget:?package=System.ServiceProcess.ServiceController&version=4.7.0"
#addin "nuget:?package=Microsoft.Web.Administration&version=11.1.0"
#addin "nuget:?package=Microsoft.Win32.Registry&version=4.6.0"

#tool "nuget:?package=OrckestraCommerce.Website.SetupServer&version=0.0.3"

#load "helpers/filesystem.cake"
#load "helpers/certificates.cake"
#load "helpers/webrequests.cake"
#load "helpers/cakeconfig.cake"

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

if (!isShowingDoc)
{
    Information($"Target: {target}");
    Information($"Environment: {environment}");
}
else
{
    Information("");
    Information("-----------------------------------------------------------------------------------------------");
    Information("-docs                     Displays available commands");
    Information("-t All                    Executes specific target, default is 'ALL'");
    Information("-env INT2                 Use environment from configuration. If not suplied, default is used");
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var rootDir = MakeAbsolute(DirectoryPath.FromString("..")).FullPath;
var outputDir = $"{rootDir}/output";
var C1File = $"{outputDir}/C1.zip";

var deploymentDir = $"{rootDir}/deployment";
var websiteDir = $"{deploymentDir}/Website";

var directoryName = DirectoryPath.FromString(rootDir).GetDirectoryName();

var setupPort = 9876;
var setupServer = GetFiles($"{rootDir}/build/tools/**/*SetupServer.exe").Single();

var baseCulture = "en-CA";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////


Dictionary<string, string> Parameters;
string localSiteName = null;
Task("Load-CakeConfig").Does(() =>
{
    var config = CakeConfig
        .UseContext(Context)
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

    var requiredParameters = new [] { "C1Url", "websiteName", "setupDescription", "websiteUrl", "adminName", "adminEmail", "adminPassword" };
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
    Context.DeleteDirectories(deploymentDir);
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
    DownloadFile(Parameters["C1Url"], C1File);
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
    XmlPoke($"{websiteDir}/web.config", "/configuration/system.web/compilation/@debug", "true");
    XmlPoke($"{websiteDir}/App_Data/Composite/Composite.config", "/configuration/Composite.SetupConfiguration/@PackageServerUrl", $"http://localhost:{setupPort}");
    StopSite(localSiteName);
    StartSite(localSiteName);
});

Process _process = null;

Task("Start-SetupServer").Does(() => 
{
    var setupTemplate = Parameters["setupDescription"];
    _process = new Process();
    _process.StartInfo.FileName = setupServer.ToString();
    _process.StartInfo.Arguments = $@"--port {setupPort} --template ""{setupTemplate}"" ";
    _process.StartInfo.CreateNoWindow = true;
    _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    _process.Start();
});

Task("Select-Package").Does(() => 
{
    if (_process.HasExited)
        throw new Exception("Setup server has stopped unexpectedly");

    Context.MakeRequest($"{Parameters["websiteUrl"]}/Composite/top.aspx");

    var script = $@"
$setupServiceWsdlUri = '{Parameters["websiteUrl"]}/Composite/services/Setup/SetupService.asmx?WSDL'
$baseCulture = '{baseCulture}'

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
    var result = StartPowershellScript(script, new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput());
});

Task("Stop-SetupServer").Does(() => 
{
    if (_process != null)
    {
        _process.Kill();
    }
});

Task("Patch-ExperienceManagement-Config").Does(() =>  
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
    var sourcePackage = $"{outputDir}/artifacts/Composer.C1.Content.FR-CA.zip";
    
    CreateDirectory(autoInstallDir);
    CopyFiles(sourcePackage, autoInstallDir);

    StopSite(localSiteName);
    StartSite(localSiteName);
    Context.MakeRequest($"{Parameters["websiteUrl"]}/Composite/top.aspx");
});

#endregion

Task("Patch-csproj.user").Does(() =>
{
    var userFile = $"{rootDir}/build/configuration/Composer.CompositeC1.Mvc.csproj.user";
    var content = System.IO.File.ReadAllText(userFile);
    content = content.Replace("{CustomServerUrl}", Parameters["websiteUrl"]);

    var dest = $"{rootDir}/src/Composer.CompositeC1/Composer.CompositeC1.Mvc/Composer.CompositeC1.Mvc.csproj.user";
    if (FileExists(dest))
    {
        DeleteFile(dest);
    }        
    
    System.IO.File.WriteAllText(dest, content);
});

Task("Open-Website").Does(() =>
{
    Process.Start($"{Parameters["websiteUrl"]}/Composite/top.aspx");
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

Task("All")
    .Description("Only this task should be used, everything else subtasks")
    .IsDependentOn("Uninstall")
    .IsDependentOn("Install")
    .IsDependentOn("Patch-csproj.user")
    .IsDependentOn("Open-Website");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
