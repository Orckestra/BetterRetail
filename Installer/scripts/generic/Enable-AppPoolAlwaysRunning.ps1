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
$specificPackagesFolder = Join-Path $packagesPath "specific"
$specificEnvPackagesFolder = Join-Path $specificPackagesFolder $Environment
$7zExe = [string](Resolve-Path ".") + "\tools\7zip\7z.exe"
#---------------------------------------
$cmsWebSiteName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSiteName"
$cmsHostName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-hostname"
$cmsHostPhysicalPath	= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSitePhysicalPath"
$cmsUrl				    = Get-Settings -environment $Environment -key "cms-$($Configuration)-url"
#=======================================

Write-Log "Enable Application Pool always running mode ..."
$environment = $Environment.ToLower()

# Because of Hangfire ApplicationPool need to be AlwaysRunning 
# Source: http://docs.hangfire.io/en/latest/deployment-to-production/making-aspnet-app-always-running.html
# Source: https://github.com/HangfireIO/Hangfire.AspNet/blob/master/src/Hangfire.AspNet/AlwaysRunning.ps1
function Enable-AlwaysRunning([string]$webSiteName)
{	
	$ApplicationPreloadType = "Orckestra.Composer.CompositeC1.Mvc.HangfireHostApplicationPreload, Orckestra.Composer.CompositeC1.Mvc"
	$ApplicationPreloadProvider = "ApplicationPreload"
	$WebSiteFullName = "IIS:\Sites\$webSiteName"
	$ApplicationPool = Get-Item $WebSiteFullName | Select-Object applicationPool
	$ApplicationPoolFullName = "IIS:\AppPools\$($ApplicationPool.applicationPool)"

	Write-Log "Enable always running mode on $ApplicationPoolFullName"
	
	# Stop Application Pool
	Stop-AppPool $ApplicationPool.applicationPool
	
	# Disable IdleTime-out property for application pool	
	Set-ItemProperty $ApplicationPoolFullName -Name processModel.idleTimeout -Value ([TimeSpan]::FromMinutes(0))

	# Indicates to the World Wide Web Publishing Service (W3SVC) that 
	# the application pool should be automatically started when it is 
	# created or when IIS is started.
	# Required to start a worker process.
	Set-ItemProperty $ApplicationPoolFullName -Name autoStart -Value True

	# Specifies that the Windows Process Activation Service (WAS) will 
	# always start the application pool. This behavior allows an application 
	# to load the operating environment before any serving any HTTP requests, 
	# which reduces the start-up processing for initial HTTP requests for 
	# the application.
	# Requried for serviceAutoStartProviders
	Set-ItemProperty $ApplicationPoolFullName -Name startMode -Value 1 # 1 = AlwaysRunning, 0 = OnDemand

	# Specifies a collection of managed assemblies that will be loaded when the AlwaysRunning is specifed for an applocation pool's startMode.
	Set-WebConfiguration -Filter '/system.applicationHost/serviceAutoStartProviders' -Value (@{name=$ApplicationPreloadProvider;type=$ApplicationPreloadType})
	Set-ItemProperty $WebSiteFullName -Name applicationDefaults.serviceAutoStartEnabled -Value True
	Set-ItemProperty $WebSiteFullName -Name applicationDefaults.serviceAutoStartProvider -Value $ApplicationPreloadProvider
	
	# Start Application Pool	
	Start-AppPool $ApplicationPool.applicationPool
}

function Start-AppPool($appPoolName) {
	if ((Get-WebAppPoolState $appPoolName).Value -ne 'Started') { 
		Write-Log "Starting AppPool '$appPoolName'..."
		Start-WebAppPool $appPoolName
		Wait-WebAppPoolStateChange $appPoolName 'Started'			
	}
}

function Stop-AppPool($appPoolName) {
	if ((Get-WebAppPoolState $appPoolName).Value -ne 'Stopped') {
		Write-Log "Stoping AppPool '$appPoolName'..."
		Stop-WebAppPool $appPoolName
		Wait-WebAppPoolStateChange $appPoolName 'Stopped'	
	}
}

function Wait-WebAppPoolStateChange($appPoolName, $state) {
	Do {		
		$appPoolState = (Get-WebAppPoolState $appPoolName).Value;
		if ($appPoolState -ne $state) {
			Write-Log "Waiting after '$state' of appPool '$appPoolName'..."
			Sleep -Milliseconds 250 
		}
	} While($appPoolState -ne $state)
}

Enable-AlwaysRunning($cmsWebSiteName)