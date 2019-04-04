param(
	[bool]
	$SkipNodeModuleWipe = $true,
    [Parameter()]
    [ValidateSet("latest", "preview")]
    $SourceMode = "latest"
)

####################################################################################################################
# Variables
$MsBuildExe = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
$MsDeployExe = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"
$AppCmdRegKey = "HKLM:\SOFTWARE\Microsoft\InetStp"
$MsDeployInstallPathRegKey = "HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy"

$solutionFolder = [string](Resolve-Path ".")
$SolutionPath = $solutionFolder+"\Composer.sln"
$FileSystemPublishedWebSite = $solutionFolder+"\_Published\Composer.Mvc.Sample"

$WindowsVersion = [System.Environment]::OSVersion.Version.Major

$GenericSitePath = "$solutionFolder\..\Installer\Packages\generic\Site"
if((Test-Path $GenericSitePath) -eq $false) { New-Item $GenericSitePath -ItemType "Directory" }
$PackagedWebSite = [string](Resolve-Path $GenericSitePath)+"\Composer.Mvc.Sample.zip"

$ComposerInstallFolder = [string](Resolve-Path "$solutionFolder\..\Installer")
$GenericOAPath = "Packages\Generic\OA";
$ComposerGenericOAFolder = Join-Path -Path $ComposerInstallFolder -ChildPath $GenericOAPath;

$nugetConfigFilePath = 

####################################################################################################################
# Helper functions
function Build-Frontend{

	if(-not $SkipNodeModuleWipe){
		write-output "Cleaning node_modules folders if existing..."
		mkdir _empty
		robocopy _empty/ node_modules/ /MIR /NS /NC /NFL /NDL /NP /NJH > null.txt
		rmdir _empty
	}

	write-output "Install local node deps..."
	if(-not $SkipNodeModuleWipe -and (Test-Path -Path 'D:\INT-NODE_MODULES.7z')) {
		write-output "Using node_modules cache (D:\INT-NODE_MODULES.7z)"
		& "C:\Program Files\7-Zip\7z.exe" x D:\INT-NODE_MODULES.7z 2>&1 > npm_cache_unpack.output
		#2>&1 > npm_cache_creation.output
	}
	write-output "npm prune"...
	npm prune
	write-output "npm install..."
	npm install --msvs_version=2015 2>&1 > npm.output

	write-output "Rebuilding node_modules cache (D:\INT-NODE_MODULES.7z)"
	if(-not $SkipNodeModuleWipe -and (-not (Test-Path -Path 'D:\INT-NODE_MODULES.7z'))) {
		& "C:\Program Files\7-Zip\7z.exe" a D:\INT-NODE_MODULES.7z ./node_modules 2>&1 > npm_cache_creation.output
	}

	#Run-UnitTests

	write-output "Executing Gulp Package"
	npm run package -- --release
}

function Run-UnitTests {
	write-output "Running Unit tests"
	gulp unitTests *> ..\tsUnitTests.karmalog
	robocopy .temp\Tests\test-results\ ..\KarmaTests\ karma.junit.xml /MIR

	$logs = Get-Content ..\tsUnitTests.karmalog
	$logs
	if (($logs -clike "*error TS*").Count -gt 0) { throw "'error TSxxxxxx' keyword found" }
	if (($logs -clike "TypeError:*").Count -gt 0) { throw "'TypeError' keyword found" }
}

function Restore-Nuget {
    Write-Host "Restoring NuGet packages" -ForegroundColor DarkGray


	 ./.nuget/NuGet.exe restore -NoCache -ConfigFile $nugetConfigFilePath

     $exitcode = $LASTEXITCODE

     if($exitcode -ne 0) {
         throw "Error while restoring NuGet packages"
     }
}

function Build-Backend {
	$args = @(
		$SolutionPath,
		"/p:DeployOnBuild=true;PublishProfile=Package;Configuration=Release"
	)

	Write-Output Solution:$SolutionPath
	& $MsBuildExe $args

	if($LASTEXITCODE -ne 0) {
		throw "MSBuild failed."
	}

	robocopy $solutionFolder\UI.Package "$FileSystemPublishedWebSite\UI.Package" /MIR /NS /NC /NFL /NDL /NP /NJH
	robocopy $solutionFolder\UI.Package "$solutionFolder\Composer.Mvc.Sample\UI.Package" /MIR /NS /NC /NFL /NDL /NP /NJH
}

function Get-MsDeployLocation
{
    param(
    [Parameter(Mandatory=$true)]
    [string]$regKeyPath
    )

    $msDeployNotFoundError = "Cannot find MsDeploy.exe location. Verify MsDeploy.exe is installed on $env:ComputeName and try operation again."
    
    if( -not (Test-Path -Path $regKeyPath))
    {
        throw $msDeployNotFoundError 
    }
        
    $path = (Get-ChildItem -Path $regKeyPath | Select -Last 1).GetValue("InstallPath")

    if( -not (Test-Path -Path $path))
    {
        throw $msDeployNotFoundError 
    }

    return (Join-Path $path msDeploy.exe)
}

function MSDeploy-ContentToPackage {
	"publish: $FileSystemPublishedWebSite"
	"package: $PackagedWebSite"
	
	$msDeployExePath = Get-MsDeployLocation -regKeyPath $MsDeployInstallPathRegKey
	$tmp = @("-verb:sync", "-source:contentpath='$FileSystemPublishedWebSite'" , "-dest:package='$PackagedWebSite'")
	
	& $msDeployExePath $tmp 

}

function Copy-ChangesetIfExists {
    $source = "$solutionFolder\..\Changeset.txt"
	$destination = "$FileSystemPublishedWebSite/changeset.txt"
	if((Test-Path -Path $source)) {
        Copy-Item -Path $source -Destination $destination -Force
    }
}


####################################################################################################################
# Execution flow
#Build-Frontend
#Restore-Nuget
#Build-Backend
#Copy-ChangesetIfExists
MSDeploy-ContentToPackage
