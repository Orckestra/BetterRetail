<# 
.SYNOPSIS
Main script to execute build and test related tasks. This script is used on both development machines and build machines.

.DESCRIPTION 
This script is only a thin wrapper around our psake script (Build.psake.ps1). Since the current 
script has parameters with 'ValidateSet', it is easier to use than the hashtable that has to
be provided to Invoke-psake. It also avoids the need to import the psake module yourself.

Note that detailed logs of the operation being executed are available in '..\Logs'.

To print more details about progress, you can use the -Verbose common parameter.

To get up and running quickly on psake, refer to the following: https://github.com/psake/psake/wiki

.EXAMPLE

.\Build.ps1 -Docs

List available tasks defined in Build.psake.ps1:

.EXAMPLE

.\Build.ps1 All

This is the equivalent of Install-DevMachine.ps1.

.EXAMPLE

.\Build.ps1 -TaskList Clean,Compile -Configuration Release -BuildType Rolling

Cleans and compiles everything in the 'Release' configuration as it is done in the 'Rolling' build.
#>

[cmdletbinding(DefaultParameterSetName="execute")]
param(
    # There are three types of build (dev, rolling and release). Depending on the BuildType provided,
    # some tasks may be skipped. For example, when running a 'rolling' build, the integration tests 
    # will be skipped since they are not executed in an rolling build. 
    [Parameter(Position=1, ParameterSetName='execute')]
    [ValidateSet('Dev', 'Rolling', 'Release')]
    [string]$BuildType,

    # The configuration to use when compiling/deploying/testing. Defaults to 'Debug'.
    [Parameter(Position=2,ParameterSetName='execute')]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration,

    # Can be used to change the verbosity used when compiling the solutions. Defaults to 'normal'.
    [Parameter(Position=3,ParameterSetName='execute')]
    [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$MsbuildVerbosity,

    # Since the script is used both on dev machine and on build machine, this parameter is used to specify
    # where it is called from. You should not have to use this parameter on your machine.
    [Parameter(Position=4, ParameterSetName='execute')]
    [switch]$IsRunningOnBuildMachine,

    # Deployment flavor to deploy.
    [Parameter(Position=5, ParameterSetName='execute')]
    [ValidateSet('Nature','ComposerSitecore','ComposerC1')]
    [string]$DeploymentFlavor = 'ComposerSitecore',

    # Can be used to pass non standard properties to the psake script. This is usually only needed 
    # when builds are started by the VSTS agents that have to provide some credentials.
    [Parameter(Position=6, ParameterSetName='execute')]
    $ExtraProperties,

    # Use this switch to list the available tasks that can be provided to 'TaskList'.
    [Parameter(ParameterSetName='doc')]
    [switch]$Docs
    )
dynamicparam {
    $options =  . (Join-Path $PSScriptRoot Get-BuildTaskList.ps1)
	
    Get-ChildItem (Join-Path $PSScriptRoot "..\packages\") -Include "New-DynamicParam.psm1" -Recurse -Force | Import-Module -Force

	$HelpMessage = 'Which tasks do you want to run? Start the script with -Docs to list the available tasks.'
	New-DynamicParameterGroup {
        New-DynamicParam -Name TaskList -Type $([String[]]) |
			Add-ParameterSet -Mandatory -Name 'execute' -HelpMessage $HelpMessage |
			Add-ParameterSet -Name 'doc' -HelpMessage $HelpMessage | 
			Add-ValidateSet -ValidateSet $options |
            Add-Alias -Alias 'T' 
	}
}
process {
    $TaskList = $PSBoundParameters.TaskList

if ($Configuration.Length -eq 0)
{
    $Configuration = 'Release'
}

if ($BuildType.Length -eq 0)
{
    $BuildType = 'Dev'
}

if ($MsbuildVerbosity.Length -eq 0)
{
    $MsbuildVerbosity = 'normal'
}

# Fail on first error.
$ErrorActionPreference = 'Stop'

if ($IsRunningOnBuildMachine)
{
    Write-Host 'TFS environment variables:'
    dir env:* | Format-Table
}

$psakeScript = Join-Path $PSScriptRoot 'Build.psake.ps1'

function Get-TaskSubTree([int]$Indent, $Task, $TasksHashTable)
{
    $prefix = ' ' * $Indent
    write-host "$prefix$($Task.Name)"
	$Task.Name
    foreach ($subTask in $Task.'Depends On')
    {
        Get-TaskSubTree -Indent ($Indent + 2) $TasksHashTable[$subTask] $TasksHashTable
    }
}

if ($Docs)
{
    $tasks = Invoke-psake $psakeScript -structuredDocs -nologo
    $tasksHashTable = @{}
    $tasks | ForEach {$tasksHashTable[$_.Name] = $_}
	#http://patorjk.com/software/taag/#p=display&f=Slant
	Write-Host '
********************************************************************************************
********************************************************************************************
                                                    \||/
                                                    |  @___oo
                                          /\  /\   / (__,,,,|
                                         ) /^\) ^\/ _)
                                         )   /^\/   _)
                                         )   _ /  / _)
                                     /\  )/\/ ||  | )_)
                                    <  >      |(,,) )__)
                                     ||      /    \)___)\
                                     | \____(      )___) )___
                                      \______(_______;;; __;;;                                                                   
 
********************************************************************************************
********************************************************************************************
'


	if(!$TaskList){
		Write-Host '
The following displays the available tasks that you can provide to -TaskList. 
The hierarchy of the tasks is also displayed.
		'
		$printedTasks = @()
		
		
		Get-TaskSubTree -Indent 2 -Task $tasksHashTable['all'] -TasksHashTable $tasksHashTable | % {
			$printedTasks += $_
		}

		$extraTasks = $tasks | Where-Object {$_.Name -notin $printedTasks }
		if($extraTasks) {
			Write-Host '***********************************************************'
			Write-Host 'Extra tasks:'
			$extraTasks | % { Write-Host "  $($_.Name)" }
			Write-Host '***********************************************************'
		}
		Write-Host ''
		return
	}

	Write-Host 'The following displays the hierarchy of the tasks passed with ''-TaskList''.'
	
	$TaskList | %{

		$startupTask = $tasksHashTable[$_]
		
		Get-TaskSubTree -Indent 2 -Task $startupTask -TasksHashTable $tasksHashTable | Out-Null
		Write-Host ''
	}
}
else
{
    #
    # If the user called the current script with -Verbose or -Debug, we want this option to propagate to 
    # our psake script. By default, these options don't propagate to other modules. This is why 
    # we have to do this propagation ourselves.
    #
    # See http://stackoverflow.com/questions/16406682/write-verbose-ignored-in-powershell-module for more details.
    #
    $verboseEnabled = (Write-Verbose 'x' 4>&1).Length -gt 0

    $psakeProperties = @{
        Configuration=$Configuration; 
        BuildType=$BuildType; 
        IsRunningOnBuildMachine=$IsRunningOnBuildMachine; 
        MsbuildVerbosity=$MsbuildVerbosity;
        DeploymentFlavor=$DeploymentFlavor
    }

    foreach ($key in $ExtraProperties.Keys)
    {
        $value = $ExtraProperties[$key]
        $psakeProperties.Add($key, $value)
    }

    if ($IsRunningOnBuildMachine)
    {
        Write-Output $psakeProperties
    }

    Invoke-psake $psakeScript -properties $psakeProperties -taskLis $TaskList -nologo -Verbose:$verboseEnabled

    if ($psake.build_success -ne $true)
    {
        throw 'psake failed.'
    }

    <#if ($psake.number_of_test_run_errors -ne 0)
    {
        throw "psake ran until the end but $($psake.number_of_test_run_errors) test runs failed."
    }#>

    # We have to clear the last exit code that may be indicate failure.
    # If we don't clear it, VSTS build will complain with errors like the 
    # following:
    #
    #   2016-04-02T13:40:44.8205974Z ##[error]Process completed with exit code 1
    #
    $LASTEXITCODE = 0
}
}