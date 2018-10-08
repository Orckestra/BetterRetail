#requires -version 3.0
param(
	[Parameter(Mandatory=$True)][string]$Environment,
	[Parameter(Mandatory=$True)][string]$Pipeline,
	[Parameter(Mandatory=$False)][PSCredential]$Credentials,
	[Parameter(Mandatory=$False)][bool]$WaitOnExit = $true,
	[Parameter(Mandatory=$False)][bool]$SkipLogging = $false,
	[Parameter(Mandatory=$False)][string]$DeploymentToken = "$($environment)_$(Get-Date -Format 'yyyy-MM-dd_HH\Hmm.ss')"
)

Set-Variable -Name "currentDeploymentToken" -Value $deploymentToken -Scope Global

#=======================================
$errorActionPreference = "Stop"
if ($PSBoundParameters['Verbose']) { $LogVerbose = "verbose"}
$env:SkipLogging = $skipLogging
#=======================================

#=======================================
# Initialize Script Execution Context
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path
. $currentScriptPath\scripts\core\Reset-Context.ps1
. $currentScriptPath\scripts\core\Initialize-Context.ps1
#=======================================

# $windowsVersion = (Get-WmiObject -class Win32_OperatingSystem).Caption
# if (($windowsVersion -match "Server 2012") -or ($windowsVersion -match "Server 2008 R2")){	
	# "Current OS: $windowsVersion" | Out-Log $LogVerbose
# } else {
	# Write-Log ERROR "Your current Windows version '$windowsVersion' is not supported!"
	# Write-Log none "Currently Supported versions: 'Server 2012' and 'Server 2008 R2'"
	# " "
	# Throw "Unsupported Windows version"
# }

Write-Output ""
Write-Log none "Starting the '$Pipeline' pipeline of all machines of the environment '$Environment':"
Invoke-EnvironmentPipeline -environment $Environment -pipeline $Pipeline -Credentials $Credentials | Out-Log $LogVerbose

#if ($waitOnExit) {
	#Write-Host "`r`nPress any key to continue ..." -ForegroundColor yellow
	#$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
#}

