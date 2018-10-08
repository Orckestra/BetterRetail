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
$cmsC1customPackages 	= Get-Settings -environment $Environment -key "cms-c1-custom-packages"
#=======================================

Write-Log "Installation of Reference Application Starter Site..."
$environment = $Environment.ToLower()
$tryCounts = 0

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

function RestartingIIS() {
	Write-Log "Restarting IIS..."
	iisreset
}

function AccessHomePage() {
	try {
		Write-Log "Accessing to home page in order to install the packages..."
		$response =  Invoke-GetWebRequest -uri "$cmsUrl" 
		Write-Log ACTION $response.StatusDescription
	}
	catch {
		Write-Log ACTION "Error accesing home page"
		if($tryCounts -eq 0) {
			Write-Log ACTION "Try Again..." 
			RestartingIIS
			AccessHomePage
			$tryCounts++
		}

	}
	
}

function Install-Packages()
{
	$packagesAutoInstallFolder = "$cmsHostPhysicalPath\WebSite\App_Data\Composite\AutoInstallPackages"
	
	$names = $cmsC1customPackages.Split(",");
	
	$names | ForEach-Object {
		$name = $_
		Write-Log "Downoloading $($name) package..."
		DownloadC1Package -packagename $name -outputRoot $genericC1CMSPackagesFolder -c1version $cmsC1version
		Robocopy $genericC1CMSPackagesFolder $packagesAutoInstallFolder "$name.zip"
	}
	
	RestartingIIS
	AccessHomePage
}


Install-Packages
