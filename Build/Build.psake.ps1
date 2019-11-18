#---------------------------------------------------------------------------------------
#
# This script is used to run the same build process on dev machines and on official 
# build machines. By using this script on your dev machine, you are therefore using 
# the same steps as on the build machine.
#
# Note that the script Build.ps1 makes it easier to call Build.psake.ps1. You should
# be using Build.ps1 instead.
#
#
# EXAMPLES:
# ---------
#
# To run this script, you first need to import the psake module. Once this is done,
# simply call the Invoke-psake cmdlet. Ex:
#
#    Import-Module ..\Dependencies\Lib\psake\psake.psm1
#    Invoke-psake Build.psake.ps1 Compile -properties @{Configuration = 'Release'}
#
# If you want to list the available tasks, you can simply run one of the following:
#
#    Invoke-psake Build.psake.ps1 -docs
#    Invoke-psake Build.psake.ps1 -detailedDocs
#
#
# GUIDELINES:
# -----------
#
# In order to avoid surprises, the script follows these guidelines:
#
#   - all the tasks executed on a dev machine will be executed the exact same way
#     on the build machine.
#   - the build machine may do more tasks than a dev machine (ex: run SonarQube)
#   - compiling from the build script should do the same thing as compiling from 
#     within VisualStudio.
#  
#---------------------------------------------------------------------------------------


# Fail on first error.
$ErrorActionPreference = 'Stop'


#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                 psake properties
#
# The following properties can be overwritten from the command line by using the 
# -properties option of Invoke-psake.
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

properties {
    $Configuration      = 'Release'   # Can be: Debug, Release
    $BuildType          = 'Dev'     # This option is a high level switch that controls which tasks
                                    # are / are not executed. 
                                    #
                                    # Can be 'Dev','Rolling','Release'
	$VisualStudioVersion	 = '2019'
    $IsRunningOnBuildMachine = $false
    $MsbuildVerbosity        = 'normal' # Can be: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
    $NugetVerbosity          = 'quiet'   # Can be: normal, quiet, detailed
    $DatabaseServerName      = '(local)'
    $DeploymentFlavor        = 'ComposerSiteCore'
    $SonarLogin              = $null     # Will be set only when running on build machine.
    $ProGetPassword          = $null     # Will be set only when running on build machine.

    $Build = @{}
}



#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                High level targets
#
# The following targets describe the overall flow of the script. See the sub targets
# for more details.
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
# Download nuget packages that are required very soon in the build process 
# (i.e. before RestorePackages is called)

. .\Load-BootstrapPackages.ps1

. .\Build.psake.functions.ps1
. .\Build.CF.psake.ps1
. .\Build.CC1.psake.ps1


Task default      -depends All # This task needs to be provided because -Docs does not return the 'default' task. I therefore introduce a dummy task.


Task All          -depends AddBuildTags,
						   CF,
                           CC1,
                           PublishGlobalArtifacts
						   
Task Dev      -depends AddBuildTags,
						   CFDev,
                           CC1Dev,
                           PublishGlobalArtifacts				  
						   
		   

#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                 psake initialization
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

# This task will run before each task is exeucted. We can use it to validate parameters.
# See the following page for more information on 'TaskSetup':
#
#    https://github.com/psake/psake/wiki/What-is-the-structure-of-a-psake-build-script%3F
#

function PopulateVariables {

    if($Build.Populated){
        return
    }

    if (($Configuration -ne 'Release') -and ($Configuration -ne 'Debug'))
    {
        throw "Configuration $Configuration is invalid."
    }

    $availableBuildTypes = 'dev','rolling','Release'
    if ($availableBuildTypes -notcontains $BuildType)
    {
        throw "Parameter 'BuildType' ($BuildType) is invalid. The allowed values are: $availableBuildTypes."
    }

    . Global_InitializeVariables
    . CF_InitializeVariables
    . CC1_InitializeVariables
    
    $Build.Populated = $true

    Write-Host "All Build Variables"
    $Build | Format-HashTable
}

function Format-HashTable {
    [CmdLetBinding()]
    param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [System.Collections.Hashtable]$HashSet, 
    [string]$BaseKey = ""
    )

    foreach($key in $HashSet.Keys){
        if($HashSet[$key] -is [System.Collections.Hashtable]){
            $HashSet[$key] | Format-HashTable -BaseKey "$key."
        }
        else{
            @{"$BaseKey$key" = $HashSet[$key]}
        }
    }
}

TaskSetup { PopulateVariables }

# This function changes what psake prints when a task starts.
FormatTaskName {
   param($taskName)
   "[$(Get-Date -f 'yyyy-MM-dd HH:mm:ss')] $taskName"
}


# This function tells psake which version of the build tools it should use.
#
# Since VS uses the 32 bits version of msbuild and our previous TFS build 
# also used the 32 bits version of msbuild, we also use the 32 bits version
# of msbuild.
#
Framework "4.7.1x86"

#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                 Global variables
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

function Global_InitializeVariables {
    
    $Build.WorkspaceRoot  = Split-Path $psake.build_script_dir -Parent
    $Build.BuildDirectory = $psake.build_script_dir
    $Build.NugetExe       = Join-Path $Build.WorkspaceRoot 'Lib\nuget\nuget.exe'
    $Build.ctt = Join-Path $Build.WorkspaceRoot 'lib\ctt\ctt.exe'

    # Since we know that we want 'msbuild 15' but psake only provides a function to set the 'framework',
    # we validate that 'framework 4.6.1' is equivalent to 'msbuild 15'.
    $Build.MsDeployExe = "C:\Program Files (x86)\IIS\Microsoft Web Deploy v3\msdeploy.exe"
	$Build.ZIPExe = Join-Path $Build.WorkspaceRoot 'lib\7-Zip\7z.exe'
    $Build.NUnitExe = Get-ChildItem "$NugetPackagesRepository\NUnit.ConsoleRunner.*" -Include "nunit3-console.exe" -Recurse -Force | Select-Object -First 1
    
    Write-Verbose "NUnit found here: $($Build.NUnitExe)"

    $Build.msbuildVersion = [System.Version](Exec {MsBuild /version /nologo})
    #Assert (($Build.msbuildVersion.Major -eq 15) -and ($Build.msbuildVersion.Minor -eq 0)) -failureMessage "Unexpected msbuild version: $msbuildVersion"

    $Build.ArtifactsStagingDirectory = "$($Build.WorkspaceRoot)\Artifacts"

    New-FolderIfNotExists $Build.ArtifactsStagingDirectory
    $Build.TroubleshootingArtifactsStagingDirectory = Join-Path $Build.ArtifactsStagingDirectory 'Troubleshooting'
    New-FolderIfNotExists $TroubleshootingArtifactsStagingDirectory
    $Build.CentralLogsFolder = Join-Path $Build.TroubleshootingArtifactsStagingDirectory 'Logs'
    New-FolderIfNotExists $Build.CentralLogsFolder
    $Build.CentralTestsFolder = Join-Path $Build.TroubleshootingArtifactsStagingDirectory 'Tests'
    New-FolderIfNotExists $Build.CentralTestsFolder

    $Build.LocalNugetRepository = "C:\LocalNuget\DEV\"
    New-FolderIfNotExists $Build.LocalNugetRepository

    $gitVersion = Get-GitVersion

    Write-Output 'Build version numbers:'
    Write-Output $gitVersion

    $Build.assemblyVersion     = "$($gitVersion.Major).$($gitVersion.Minor).0.0"
    $Build.assemblyFileVersion = "$($gitVersion.Major).$($gitVersion.Minor).$($gitVersion.Patch).$($gitVersion.CommitsSinceVersionSource)"


}

#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                    Machine setup
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

Task AddBuildTags -precondition {$IsRunningOnBuildMachine} {

    # We tag each build with the branch name. This makes it easier to find builds related to a 
    # branch from the build results page (that contains builds for all the branches).
    #
    # We add the tag using logging commands as explained here:
    #
    #     https://github.com/Microsoft/vso-agent-tasks/blob/master/docs/authoring/commands.md
    #

    # only keep the leaf of the branch name
    $tag = (Get-GitBranchName) -split '/' | select -Last 1

    Write-Host "##vso[build.addbuildtag]$tag"
}


Task PublishGlobalArtifacts {

    # Save version in a file for troubleshooting.
    $Build.assemblyFileVersion > (Join-Path $Build.TroubleshootingArtifactsStagingDirectory 'version.txt')

    if (($BuildType -eq 'rolling') -or ($BuildType -eq 'release')) 
    {
		UploadArtifact -ArtifactName 'NuGet' -ContainerFolder 'NuGet'                                 -Path "$WorkspaceRoot\LocalNugetRepository"
    }
}

