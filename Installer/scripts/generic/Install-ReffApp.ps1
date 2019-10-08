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
$genericRefAppPackagesFolder = Join-Path $genericPackagesFolder "C1CMS\RefApp"
$specificPackagesFolder = Join-Path $packagesPath "specific"
$specificEnvPackagesFolder = Join-Path $specificPackagesFolder $Environment
$7zExe = [string](Resolve-Path ".") + "\tools\7zip\7z.exe"
#---------------------------------------
$cmsWebSiteName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSiteName"
$cmsHostName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-hostname"
$cmsHostPhysicalPath	= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSitePhysicalPath"
$cmsUrl				    = Get-Settings -environment $Environment -key "cms-$($Configuration)-url"
$cmsDeploymentToken 	= Get-Settings -environment $Environment -key "cms-deployment-token"
$refappContentCultures 	= Get-Settings -environment $Environment -key "cms-c1-all-cultures"
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
	if ($Configuration -eq 'cm') {
	
	AccessHomePage

	$dstFolder = "$cmsHostPhysicalPath\WebSite\App_Data\Composite\AutoInstallPackages"
	Write-Log "Copying Composer.C1.Core.zip" 
	robocopy $genericRefAppPackagesFolder $dstFolder "Composer.C1.Core.zip" /NJH /NDL /NS /NC /NP

	RestartingIIS 
	AccessHomePage

		
	Write-Log  "Copying Other ReffApp Features...."
	Robocopy $genericRefAppPackagesFolder $dstFolder "Orckestra.Composer.C1CMS.Queries.zip" /MIR /NJH /NDL /NFL /NS /NC /NP /NJS
	Robocopy $genericRefAppPackagesFolder $dstFolder "Orckestra.Composer.SEO.Organization.zip" /MIR /NJH /NDL /NFL /NS /NC /NP /NJS
	Robocopy $genericRefAppPackagesFolder $dstFolder "Orckestra.Composer.Languages.zip" /MIR /NJH /NDL /NFL /NS /NC /NP /NJS
	
	RestartingIIS 
	AccessHomePage 
	
	Write-Log  "Install Page Contents...."
	Install-ContentPackages
	
	Write-Log  "Install Features Default Contents...."
	Robocopy $genericRefAppPackagesFolder $dstFolder "Orckestra.Composer.SEO.Organization.Content.zip" /MIR /NJH /NDL /NFL /NS /NC /NP /NJS
	RestartingIIS 
	AccessHomePage 
	
			
	Write-Log ACTION "Repackaging site as CD deployment package..."
	iisreset /stop
	Remove-Item $cmsHostPhysicalPath\Website\App_Data\Composite\Cache -recurse -force 
	Invoke-MSDeployContentToPackage -ContentPath $cmsHostPhysicalPath\Website -PackageFile "$specificPackagesFolder\Cms-CD-$Environment-Repack.zip"	-ExcludePath "Website\App_Data\Composite\LogFiles"
	iisreset /start
	
	} elseif ($Configuration -eq 'cd') {
		Invoke-MSDeployPackageToContent -PackageFile "$specificPackagesFolder\Cms-CD-$Environment-Repack.zip" -ContentPath $cmsHostPhysicalPath\Website
	
		Write-Log "Disable C1 console ..."
		$C1ConsoleAccessXmlFile = [string[]](Get-Content $cmsHostPhysicalPath\WebSite\App_Data\Composite\Configuration\C1ConsoleAccess.xml)
		$C1ConsoleAccessXmlFile[0] = '<C1ConsoleAccess enabled="false">'
		$C1ConsoleAccessXmlFile | Set-Content $cmsHostPhysicalPath\WebSite\App_Data\Composite\Configuration\C1ConsoleAccess.xml
		$C1ConsoleAccessXmlFile | Set-Content $cmsHostPhysicalPath\WebSite\App_Data\Composite\Configuration\C1ConsoleAccess.xml.disabled
	}
	
	
}

function Install-ContentPackages() {

	$packagesAutoInstallFolder = "$cmsHostPhysicalPath\WebSite\App_Data\Composite\AutoInstallPackages"
	
	$names = $refappContentCultures.Split(",");
	$names | ForEach-Object {
		$name = ($_).ToUpper()
		$source = Join-Path $genericRefAppPackagesFolder "Composer.C1.Content.$name.zip"

		if((Test-Path -Path $source)) {
			Write-Log ACTION "Install content package for $($name) culture ...."
			Copy-Item -Path $source -Destination $packagesAutoInstallFolder -Force
		} else {
			Write-Log ACTION "Content package does not exist - $($source) "
		}

	}
	
	RestartingIIS 
	AccessHomePage
}

function  TransformConfigFiles () {
	#Configuration Tranforms
	Write-Log ACTION "Transforming Web.config..."
	Invoke-ComponentConfigTransformation -ConfigFile "Web.config" -InstallPath $cmsHostPhysicalPath\Website -Environment $Environment -Component $Component

	Write-Log ACTION "Transforming EM.config..."
	Invoke-ComponentConfigTransformation -ConfigFile "ExperienceManagement.config" -InstallPath $cmsHostPhysicalPath\Website\App_Config -Environment $Environment -Component $Component

	#Custom files Tranforms
	Write-Log ACTION "Transforming error.html..."
	Invoke-ComponentConfigTransformation -ConfigFile "error.html" -InstallPath $cmsHostPhysicalPath\Website -Environment $Environment -Component $Component

}

Install-Packages
TransformConfigFiles