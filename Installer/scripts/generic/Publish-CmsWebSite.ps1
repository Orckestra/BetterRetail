param(
	[Parameter(Mandatory=$true)]
	[string]
	$Environment
	,
	[string]
	$Component = ""
	,
	[Parameter(Mandatory=$true)]
	[ValidateSet("cm","cd")]
	[string]
	$Configuration
	,
	[switch]
	$NoContent
)

#=======================================
# Initialize Script Execution Context
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path
$scriptsPath = (get-item $currentScriptPath).parent.FullName
. $scriptsPath\core\Initialize-Context.ps1
#=======================================

#=======================================
$coreScriptPath = Join-Path $scriptsPath "core"
$rootPath = (get-item $scriptsPath).parent.FullName
$packagesPath = Join-Path $rootPath "packages"
$genericPackagesFolder = Join-Path $packagesPath "generic"
$genericC1CMSPackagesFolder = Join-Path $genericPackagesFolder "C1CMS"
$specificPackagesFolder = Join-Path $packagesPath "specific"
$specificEnvPackagesFolder = Join-Path $specificPackagesFolder $Environment
$7zExe = [string](Resolve-Path ".") + "\tools\7zip\7z.exe"
#---------------------------------------
$cmsWebSiteName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSiteName"
$cmsHostName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-hostname"
$cmsHostPhysicalPath	= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSitePhysicalPath"
$cmsUrl				    = Get-Settings -environment $Environment -key "cms-$($Configuration)-url"
$cmsDeploymentToken 	= Get-Settings -environment $Environment -key "cms-deployment-token"
$cmsC1version 			= Get-Settings -environment $Environment -key "cms-c1-version"
$cmsC1Culture 			= Get-Settings -environment $Environment -key "cms-c1-init-culture"
$cmsC1ConsoleLogin		= Get-Settings -environment $Environment -key "cms-c1-conslole-login"
$cmsC1ConsolePassword	= Get-Settings -environment $Environment -key "cms-c1-console-password"
#=======================================

# Error handling for W3SVC and admin module
Test-W3svc

Start-WorldWideWebPublishingServiceIfNotRunning

function TimeStamp { "$(Get-Date -Format "HH:mm:ss.fff")"}

function Invoke-GetWebRequest([string]$uri) {
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy


	$response = Invoke-WebRequest -UseBasicParsing -URI $uri -Headers @{ "X-Auth" = $cmsDeploymentToken } -TimeoutSec $(60*15)
	return $response
}

function DownloadC1CMS() {
	$url = "https://github.com/Orckestra/C1-CMS-Foundation/releases/download/v$cmsC1version/C1.CMS.$cmsC1version.zip"
	$destinationPath = $genericPackagesFolder + "\C1CMS"
	md $destinationPath  -ErrorAction SilentlyContinue
	$output = $destinationPath + "\C1.CMS.$cmsC1version.zip"
	Write-Host "Download from " $url  + " and save to " + $output
	$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12'
	[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols
	(Invoke-WebRequest -Uri $url -OutFile $output -TimeoutSec $(60*15)).StatusDescription
}

try {
	Write-Log ACTION "Stopping the '$cmsHostName' Website..."
	Stop-WebsiteWithItsAppPool $cmsWebSiteName

	if ($Environment -ne "dev") {
		Write-Log ACTION "Backing-up '$cmsHostName' Website folder..."
		Invoke-FolderBackup -Component $Component -SourcePath $cmsHostPhysicalPath
	} else {
		Write-Log WARNING "Backing-up '$cmsHostName' Website folder skipped ('DEV' environment)..."
	}
	
	$c1Package = $genericPackagesFolder + "\C1CMS\C1.CMS.$cmsC1version.zip"
	
	if (Test-Path $c1Package) {
		Write-Log ACTION "Skip Downloading 'C1 CMS' build version $cmsC1version - it exists" 
	} else {
		Write-Log ACTION "Downloading 'C1 CMS' build version $cmsC1version ..." 
		DownloadC1CMS
	}

	Write-Log ACTION "Extract 'C1 CMS' build version $cmsC1version ..."
	
	
	$tempUnzipPath = $genericPackagesFolder + "\temp"
	& $7zExe x $c1Package "-o$tempUnzipPath" -y
	robocopy $tempUnzipPath\Website $cmsHostPhysicalPath\WebSite /E /NJH /NDL /NS /NC /NP
	Remove-Item $tempUnzipPath -recurse -force


	Write-Log ACTION "Starting the Website..."
	Start-WebsiteWithItsAppPool $cmsWebSiteName
	
	if ($Configuration -eq 'cm') {
		
		#Initialize Bare Bone Starter Site
		Write-Log ACTION "Initialize Bare Bone Starter Site..."
		$proxy = New-WebServiceProxy -Uri http://$cmsHostName/Composite/services/Setup/SetupService.asmx?WSDL
		$proxy.Timeout = 300 * 1000
		$setupDescription = $proxy.GetSetupDescription("true") 
		$xml = New-Object -TypeName xml
		$xml.AppendChild($xml.ImportNode($setupDescription, $true)) | Out-Null
 
		$selection = "<setup>" + $xml.setup.radio[2].OuterXml + "</setup>"
		Write-Host "Setup chosen:" $selection
		
		Write-Host "Running C1 CMS setup... login: $cmsC1ConsoleLogin  Password: $cmsC1ConsolePassword " 
 		$setupResult = $proxy.SetUp($selection,$cmsC1ConsoleLogin,"admin@test.com",$cmsC1ConsolePassword,$cmsC1Culture,$cmsC1Culture,"false")
 		Write-Host "Setup result:"   $setupResult
		
		DownloadC1Package -packagename "Composite.AspNet.MvcFunctions" -outputRoot $genericC1CMSPackagesFolder -c1version $cmsC1version
		
		Write-Log ACTION "Create AutoInstallPackages folder and install required Composite.AspNet.MvcFunctions.zip"
		$packagesAutoInstallFolder = "$cmsHostPhysicalPath\WebSite\App_Data\Composite\AutoInstallPackages"
		md $packagesAutoInstallFolder  -ErrorAction SilentlyContinue
		copy ($genericPackagesFolder + '\C1CMS\Composite.AspNet.MvcFunctions.zip') $packagesAutoInstallFolder
		iisreset
		Write-Log "Accessing to home page in order to install the packages..."
		try {
			$response =  Invoke-GetWebRequest -uri "$cmsUrl" 
			Write-Log ACTION $response.StatusDescription 
		} catch {}
		
		
		Write-Log ACTION "Repackaging site as CD deployment package..."
		
		Invoke-MSDeployContentToPackage -ContentPath $cmsHostPhysicalPath\Website -PackageFile "$specificPackagesFolder\Cms-CD-$Environment-Repack.zip"	-ExcludePath "Website\App_Data\Composite\LogFiles"
		
	
	} elseif ($Configuration -eq 'cd') {
		Invoke-MSDeployPackageToContent -PackageFile "$specificPackagesFolder\Cms-CD-$Environment-Repack.zip" -ContentPath $cmsHostPhysicalPath\Website
	}

	Write-Log ACTION "Deploying specific '$cmsHostName' files..."

	$cmsHostPackagePath = "$specificPackagesFolder\$Component\"
	if (Test-Path $cmsHostPackagePath) {
		robocopy $cmsHostPackagePath $cmsHostPhysicalPath\WebSite /E /NJH /NDL /NS /NC /NP "web.*.config" "Composer.*.config"
		Complete-RobocopyExecution $lastexitcode
	} else {
		Write-Log WARNING "Deploying specific '$cmsHostName' config files skipped ('$cmsHostPackagePath' folder not found)..."
	}

 
	Write-Log ACTION "Done."
} catch {
	Write-Log WARNING "Starting the Website..."
	Start-WebsiteWithItsAppPool $cmsWebSiteName
	throw $_
}