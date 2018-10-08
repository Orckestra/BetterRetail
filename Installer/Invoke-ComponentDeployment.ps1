#requires -version 3.0
param(
	[Parameter(Mandatory=$True)][string]$Environment,
	[Parameter(Mandatory=$False)][string]$Server,
	[Parameter(Mandatory=$True)][string]$Component,
	[Parameter(Mandatory=$True)][string]$Pipeline,
	[Parameter(Mandatory=$False)][PSCredential]$Credentials,
	[Parameter(Mandatory=$False)][bool]$Locally = $false,
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

Write-Output ""
Write-Log none "In environment '$Environment', Starting '$Pipeline' for component '$Component':"
Invoke-ComponentPipeline -environment $Environment -server $Server -component $Component -pipeline $Pipeline -Credentials $Credentials -Locally $locally | Out-Log $LogVerbose

if ($waitOnExit) {
	Write-Host "`r`nPress any key to continue ..." -ForegroundColor yellow
	$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

