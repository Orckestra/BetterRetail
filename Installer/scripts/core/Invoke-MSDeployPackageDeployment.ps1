param(	
	[Parameter(Mandatory=$true)]$IisAppName,
	[Parameter(Mandatory=$true)]$PackageFilePath,
	[Parameter(Mandatory=$false)]$ParameterFile,
	[Parameter(Mandatory=$false)]$IisSitePhysicalPath,
	[Parameter(Mandatory=$false)][bool]$DoNotDeleteExtraFiles = $true
)

#=======================================
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path
$scriptsPath = (get-item $currentScriptPath).parent.FullName
$rootPath = (get-item $scriptsPath).parent.FullName
$tools = Join-Path $rootPath "tools"
#=======================================

$msdeployRegKey = "HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy"
if (Test-Path $msdeployRegKey) {
	$msdeployRegValues = (get-childitem $msdeployRegKey)
	if ($msdeployRegValues.Count -gt 0)	{
		$msdeployPath =  Join-Path ($msdeployRegValues | Select -last 1).GetValue("InstallPath") "msdeploy.exe"
	} else {
		Write-Log INFO "The RegKey 'HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy' exists but is empty..."
	}
} else {
	Write-Log INFO "Unable to find the RegKey 'HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy'..."
}

if (!$msdeployPath) {
	if (Test-Path "${env:ProgramFiles}\IIS\") {
		$installedMSDeploy = (Get-ChildItem "${env:ProgramFiles}\IIS\Microsoft Web Deploy*") | ?{ $_.PSIsContainer }
	}
	if (!$installedMSDeploy) {
		if (Test-Path "${env:ProgramFiles(x86)}\IIS\") {
			$installedMSDeploy = (Get-ChildItem "${env:ProgramFiles(x86)}\IIS\Microsoft Web Deploy*") | ?{ $_.PSIsContainer }
		}
	}
	if ($installedMSDeploy.Count -gt 0)	{
		$msdeployPath =  Join-Path ($installedMSDeploy | Select -last 1).FullName "msdeploy.exe"
	}
}

if (!$msdeployPath) {
	$scriptPath = Split-Path $MyInvocation.MyCommand.Path
	$msdeployPath = Join-Path $tools "msdeploy\msdeploy.exe"
}

if (!$msdeployPath) {
	throw "Unable to find the 'MSDeploy' installation path. Ensure that 'Web Deploy' is installed on your machine. http://www.iis.net/downloads/microsoft/web-deploy."
}

Write-Log action "Using MSDeploy: '$msdeployPath'..."
if (($ParameterFile -eq $null) -or ($ParameterFile -eq ""))
{
    # Issue here with msdeploy when we use local msdeploy v3 instead of install it, trace option are null and print an error but the deployment is done this way all is hidden.
    $arguments = "-verb:sync -source:package=""" + $PackageFilePath +  """ -dest:contentPath=""" + $IisAppName + """ -allowUntrusted"
    if($DoNotDeleteExtraFiles) {
		$arguments += " -enableRule:DoNotDeleteRule"
	}
	Write-Log ACTION "Executing msdeploy command : $($msdeployPath) $($arguments)"
    
	$ps = new-object System.Diagnostics.Process
	$ps.StartInfo.Filename = $msdeployPath
	$ps.StartInfo.Arguments = $arguments
	$ps.StartInfo.UseShellExecute = $false
	$null = $ps.start()
    $ps.WaitForExit()
}
else
{
    # Issue here with msdeploy when we use local msdeploy v3 instead of install it, trace option are null and print an error but the deployment is done this way all is hidden.
    $arguments = "-verb:sync -source:package=""" + $PackageFilePath +  """ -dest:contentPath=""" + $IisAppName + """ -setParamFile:""" + $ParameterFile + """ -allowUntrusted -enableRule:DoNotDeleteRule"
    Write-Log ACTION "Executing msdeploy command : $($msdeployPath) $($arguments)"
	
	$ps = new-object System.Diagnostics.Process
	$ps.StartInfo.Filename = $msdeployPath
	$ps.StartInfo.Arguments = $arguments
	$ps.StartInfo.UseShellExecute = $false
	$null = $ps.start()
    $ps.WaitForExit()
}
if ($LASTEXITCODE -ne 0) { throw "An error occured while deploying the website..." }