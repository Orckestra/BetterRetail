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
$scriptsPath			= (get-item $currentScriptPath).parent.FullName
$coreScriptPath 		= Join-Path $scriptsPath "core"
$rootPath				= (get-item $scriptsPath).parent.FullName
$configsPath			= Join-Path $rootPath "configs"
$packageCertificatePath	= Join-Path $rootPath "packages\specific\certificates"
$packagesSpecificRelativePath = ".\packages\specific\certificates"
#---------------------------------------

#=======================================
$cmsWebSiteName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSiteName"
$cmsHostName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-hostname"
$cmsHostPhysicalPath	= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSitePhysicalPath"

$cmsAppPoolName			= Get-Settings -environment $Environment -key "cms-apppool-name"
#=======================================

if(!(Test-WebAdministrationModuleAvailability)) {
	Write-Log ACTION "WebAdministration is not available... skipping uninstall..."
	return
}
Import-WebAdministrationModuleSafely

# Error handling for W3SVC and admin module
Test-W3svc

Start-WorldWideWebPublishingServiceIfNotRunning

# Uninstall IIS site
$website = Get-Website | ? { $_.Name -eq $cmsWebSiteName  } | Select Name
$apppool = try { Get-WebAppPoolState -Name $cmsWebSiteName -ErrorAction SilentlyContinue | Select Name } catch { }

if ($website -or $apppool) {
	if ($website) {
		Write-Log ACTION "Stoping the $cmsHostName Website..."
		Stop-WebsiteWithItsAppPool $cmsWebSiteName
		Write-Log ACTION "Removing $cmsHostName  WebSite..."
		Remove-Website -Name $cmsWebSiteName
	} else {
		Write-Log WARNING "Skipping $cmsHostName WebSite removal (not found)..."
	}
	
	if ($apppool) {
		Write-Log ACTION "Stoping the AppPool..."
		Stop-AppPool $cmsAppPoolName	
		Write-Log ACTION "Removing $cmsAppPoolName AppPool..."
		Remove-WebAppPool -Name $cmsAppPoolName
	} else {
		Write-Log WARNING "Skipping $cmsAppPoolName	app pool removal (not found)..."
	}
} else {
	Write-Log WARNING "Skipping $cmsHostName WebSite and $cmsAppPoolName	AppPool removal (not found)..."
}

# Delete destination folder
if (Test-Path $cmsHostPhysicalPath) { 
	Write-Log ACTION "Deleting website physical path..."
	Remove-Item $cmsHostPhysicalPath -recurse -force | out-null
} else {
	Write-Log WARNING "Skipping website physical path removal (not found)..."
}

# Remove HostFile hostname
Write-Log ACTION "Removing hostname from host file..."
Remove-HostFileEntry $cmsHostName
