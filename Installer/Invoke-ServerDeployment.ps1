#requires -version 3.0
param(
	[Parameter(Mandatory=$True)][string]$Environment,
	[Parameter(Mandatory=$True)][string]$Server = "localhost",
	[Parameter(Mandatory=$True)][string]$Pipeline,
	[Parameter(Mandatory=$False)][PSCredential]$Credentials,
	[Parameter(Mandatory=$False)]$locally = $false,	
	[Parameter(Mandatory=$False)][bool]$waitOnExit = $true,
	[Parameter(Mandatory=$False)][bool]$skipLogging = $false,
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
$myScriptPath = Split-Path $MyInvocation.MyCommand.Path
. $myScriptPath\scripts\core\Reset-Context.ps1
. $myScriptPath\scripts\core\Initialize-Context.ps1
#=======================================

$windowsVersion = (Get-WmiObject -class Win32_OperatingSystem).Caption
# if (($windowsVersion -match "Server 2012") -or ($windowsVersion -match "Server 2008 R2")) {
	# "Current OS: $windowsVersion" | Out-Log $LogVerbose
# } else {
	# Write-Log ERROR "Your current Windows version '$windowsVersion' is not supported!"
	# Write-Log none "Currently Supported versions: 'Server 2012' and 'Server 2008 R2'"
	# " "
	# Throw "Unsupported Windows version"
# }

Write-Output ""
Write-Log none "In environment '$Environment', Starting '$Pipeline' for server '$Server':"
Invoke-ServerPipeline -environment $Environment -server $Server -pipeline $Pipeline -Credentials $Credentials -Locally $locally | Out-Log $LogVerbose

if ($waitOnExit) {
	Write-Host "`r`nPress any key to continue ..." -ForegroundColor yellow
	$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

