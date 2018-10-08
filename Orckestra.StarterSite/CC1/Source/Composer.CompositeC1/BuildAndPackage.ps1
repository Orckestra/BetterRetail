param(
	[string]
	$Environment = "DEV",
	[Parameter()]
    [ValidateSet("latest", "preview")]
    $SourceMode = "latest",
    [Parameter()]
    [string]$OvertureSource = "\\s0010cbld01\Overture-Builds\$SourceMode\Deployment"
)

####################################################################################################################
# Variables
$MsBuildExe = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
$MsDeployExe = "C:\Program Files (x86)\IIS\Microsoft Web Deploy v3\msdeploy.exe"
$7zaExe = [string](Resolve-Path ".")+"\Package.C1\7za.exe"

$WindowsVersion = [System.Environment]::OSVersion.Version.Major

$solutionFolder = [string](Resolve-Path ".")
$WebProjectPath = [string](Resolve-Path ".")+"\Composer.CompositeC1.Mvc"
$PackageC1Path = [string](Resolve-Path ".")+"\Package.C1"
$installerFolder = [string](Resolve-Path "..\..\Installer")
$FileSystemPublishedWebSite = [string](Resolve-Path ".")+"\_Published\Composer.CompositeC1.Mvc"
$PackagedWebSite = [string](Resolve-Path "..\..\Installer\Packages\generic\CompositeC1")+"\DeploymentFolder.zip"
$CompositeC1PackagesPath = [string](Resolve-Path "..\..\Installer\Packages\generic\CompositeC1\C1.Packages")

$NuGetSources = @(
    "https://www.nuget.org/api/v2/",
	"C:\LocalNuget\$Environment",
#    "C:\LocalNuget",
    "***REMOVED***",
    "***REMOVED***",
	"***REMOVED***",
	"http://packages.orckestra.com/feeds/Composer"
)

$ContentPackageNumber = 200

#$OvertureSource = "C:\Overture\3.5.0.26613\Deployment"
$GenericOAPath = "Packages\Generic\OA";
$ComposerGenericOAFolder = Join-Path -Path $installerFolder -ChildPath $GenericOAPath;
$OvertureGenericOAFolder = Join-Path -Path $OvertureSource -ChildPath $GenericOAPath;

####################################################################################################################
# Helper functions
function Copy-UiPackageFromNuget {
	$srcFolder = (gci "$solutionFolder\packages\Composer.*\").FullName
	Robocopy $srcFolder\UI.Package $WebProjectPath\UI.Package /E
}

function Restore-Nuget {
    Write-Host "Restoring NuGet packages using the following sources: $NuGetSources" -ForegroundColor DarkGray
    $src = [string]::Join(";", $NuGetSources);

	 get-childItem . -Filter *.sln | % {
		Write-Host "Restoring packages for $($_.FullName)"
		./.nuget/NuGet.exe restore -NoCache -source $src $($_.FullName)
	 }

     $exitcode = $LASTEXITCODE

     if($exitcode -ne 0) {
         throw "Error while restoring NuGet packages"
     }
}

function Build-Solution {
	& $MsBuildExe $solutionFolder\Composer.CompositeC1.sln /p:Configuration=Release

	if($LASTEXITCODE -ne 0) {
		throw "MSBuild failed."
	}
}

function Copy-DeploymentFolder {
	Robocopy $WebProjectPath\Deployment $FileSystemPublishedWebSite\Deployment *.* /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)

	# MsDeploy to packaged website folder
	$tmp = $("`"{0}`" -verb:sync -source:contentpath=`"{1}`" -dest:package=`"{2}`"" -f $MsDeployExe, $FileSystemPublishedWebSite, $PackagedWebSite )
	
	#if($WindowsVersion -eq 10)
	cmd.exe /C $tmp
	#if($WindowsVersion -ne 10)
	cmd.exe /C "`"$tmp`""
}

function Copy-Packages {
	Robocopy $PackageC1Path $CompositeC1PackagesPath *.zip /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
}

function Build-CorePackage() {
	$corePackagePath = "$PackageC1Path\Composer.C1.Core"

	# Copy missing items into the package
	Robocopy $WebProjectPath $corePackagePath "robots*.txt" /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath $corePackagePath "Web.config" /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
    Robocopy $WebProjectPath $corePackagePath "Global.asax" /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath $corePackagePath "error.html" /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\App_Data $corePackagePath\App_Data /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\App_Config $corePackagePath\App_Config /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\Composite $corePackagePath\Composite /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\UI.Package $corePackagePath\UI.Package /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\Views $corePackagePath\Views /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\bin $corePackagePath\Bin *.dll /XF Composite*.dll /NJH /NDL /NS /NC /NP
	Remove-Item $corePackagePath\Bin\Orckestra.Logging.dll
	Remove-Item $corePackagePath\Bin\Microsoft.Extensions.DependencyInjection*.dll
	Remove-Item $corePackagePath\Bin\System.Reactive*.dll
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.EnterpriseLibrary.Common.dll
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.EnterpriseLibrary.Validation.dll
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.EnterpriseLibrary.Logging.dll
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.ObjectBuilder.dll
	Remove-Item $corePackagePath\Bin\Microsoft.Web.Infrastructure.dll
	

	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $WebProjectPath\Renderers $corePackagePath\Renderers *.cs /E /NJH /NDL /NS /NC /NP
	Complete-RobocopyExecution($LASTEXITCODE)

    $packageZipName = "$PackageC1Path\100 - Composer.C1.Core.zip"
    if(Test-Path $packageZipName) {
        Write-Host "Removing '$packageZipName'" -ForegroundColor DarkGray
        Remove-Item $packageZipName -Force
	}

	& $7zaExe a -tzip "$packageZipName" -r $corePackagePath\*
	Copy-Item -Path "$packageZipName" -Destination "$PSScriptRoot\CmpPackages\RefApp\artifacts\OrckestraCmsPackages\Composer.C1.Core.zip" -Force
	
	$XPCFiles = Join-Path $PSscriptRoot '..\..\Installer\packages\generic\CompositeC1\XPC'
	
	Get-ChildItem $XPCFiles -filter '*.zip' |
	Copy-Item -Destination "$PSScriptRoot\CmpPackages\ExperienceComposer\artifacts\OrckestraCmsPackages" -Force
}

function Build-ContentPackage($CultureName) {
	$packageZipName = "$PackageC1Path\$script:ContentPackageNumber - Composer.C1.Content.$CultureName.zip"

    if(Test-Path $packageZipName) {
        Write-Host "Removing '$packageZipName'" -ForegroundColor DarkGray
        Remove-Item $packageZipName -Force
    }
	& $7zaExe a -tzip $packageZipName -r "$PackageC1Path\Composer.C1.Content.$CultureName\*"
	Copy-Item -Path "$packageZipName" -Destination "$PSScriptRoot\CmpPackages\RefApp\artifacts\OrckestraCmsPackages\Composer.C1.Content.$CultureName.zip" -Force
	$script:ContentPackageNumber++
}

function Copy-ChangesetIfExists {
    $source = "$solutionFolder\..\..\Changeset.txt"
	$destination = "$FileSystemPublishedWebSite/changeset.txt"
	if((Test-Path -Path $source)) {
        Copy-Item -Path $source -Destination $destination -Force
    }
}

function Complete-RobocopyExecution {
	param(
		[int]$RetVal
	)
	switch ($RetVal) {
		0  { $codeMsg = "The source and destination directory trees were already synchronized."}
        1  { $codeMsg = "One or more files were copied successfully."}
        2  { $codeMsg = "The source and destination directory trees were already synchronized, except for some detected extra files or directories."}
		3  { $codeMsg = "One or more files were copied successfully, but some Extra files or directories were detected."}
        4  { $codeMsg = "Succeeded. But Some Mismatched files or directories were detected."}
        8  { $codeMsg = "Error. Some files or directories could not be copied (copy errors occurred and the retry limit was exceeded). Check these errors further."}
		16 { $codeMsg = "Serious Error. Robocopy did not copy any files. Either a usage error or an error due to insufficient access privileges on the source or destination directories."}
        default { $codeMsg = "Unknown Robocopy return code: '$RetVal'"}
    }

	if ($RetVal -ge 8) {
		throw $errMsg
	} if ($RetVal -ge 2) {
		"WARNING $codeMsg"
	} else {
		"$codeMsg"
	}

	$global:LASTEXITCODE = 0;
}

function Package-OA {

	Robocopy $OvertureGenericOAFolder $ComposerGenericOAFolder /W:1 /Z /E /NP
}

####################################################################################################################
# Execution flow
Write-Host "Starting Build and Package for Starter Site C1 in $Environment" -ForegroundColor DarkGray

Package-OA
Restore-Nuget
Copy-UiPackageFromNuget
Build-Solution

	Build-CorePackage
	Build-ContentPackage "EN_CA"
	Build-ContentPackage "FR_CA"

	Copy-DeploymentFolder
	Copy-ChangesetIfExists
	Copy-Packages
