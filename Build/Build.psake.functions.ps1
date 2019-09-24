function IsRunningOnVstsHostedAgent
{
    # The property 'IsRunningOnBuildMachine' is used to seperate the tasks that are 
    # executed or skipped on the build machine. It is often helpful to use this property
    # even on dev machines to test the script as it will be done on a build machine.
    #
    # In that case (running on a dev machine with IsRunningOnBuildMachine = $true), 
    # there are some things that can't be done simply because we are not on the build
    # machines that has some capabilities that are not available on a dev machine.
    # For these cases, the current method can be used to know if we are REALLY
    # running on a build machine.
    #
    # The logic to know if we are really on a build machine is to check for an 
    # environment variable that is only defined on build machines.

    return Test-Path env:BUILD_ARTIFACTSTAGINGDIRECTORY
}

function Get-Version-Legacy {
	$version = "AssemblyVersion"
	$Mode = "Build"
	
    if($version -ilike "AssemblyVersion") {$versionSource = "AssemblyVersion"}
	if($version -ilike "AssemblyFileVersion") {$versionSource = "AssemblyFileVersion"}

	$assemblyInfoFile = (Get-ChildItem -Recurse "../Orckestra.StarterSite/CF/Source/Solution Items/SharedAssemblyInfo.cs" -ErrorAction SilentlyContinue)[0]
	$content = [string[]](Get-Content $assemblyInfoFile)
	$versionId = $content | ? { $_ -ilike "*$versionSource*" } | % {
		$_ -replace ('\[assembly: ' + $versionSource + '\("(.*)"\)\]'), '$1'
	}
    
    $changesetIndex = $versionId.LastIndexOf('.')
    $packageVersion = $versionId.Substring(0, $changesetIndex)
    $changeset = $versionId.Substring($changesetIndex + 1, ($versionId.length - $changesetIndex - 1))
    
    if($Mode -ilike "Build") {
        $nugetVersion = $packageVersion + "-Build"
    }
    elseif($Mode -ilike "PreRelease") {
        #This does not follow SemVer because NuGet does not support appending the Build number after the 'Stable'
        $nugetVersion = "$packageVersion.$changeset" + "-Stable"
    }
    else {
        $nugetVersion = "$packageVersion.$changeset"
    }
    
    return $nugetVersion
}

function Transform-Nuspec{
    param(
        [Parameter(ValueFromPipeline,Mandatory)]
        [string[]]$NuspecFile,
        [Parameter(Mandatory)]
        [string]$NuVersion,
        [bool]$UseSymbols = $false
    )
    
    process{
        $NuspecFile | % {
            
            $NuDestFile = $_.Replace(".template.nuspec", ".nuspec")
            
            $content = [string[]](get-content $_)
            $content | % {
                $_ -replace '<version>(.*)<\/version>', ('<version>'+$NuVersion+'</version>')
            } | % { 
                $_ -replace '<dependency id="Composer" version="(.*)" />', ('<dependency id="Composer" version="'+ $NuVersion +'" />') 
            } | Set-Content $NuDestFile
            
            $transformFile = $NuspecFile.Replace("template.nuspec", "template.symbols.nuspec")

            if($UseSymbols -and (Test-Path $transformFile)) {
                Write-Verbose "Appending symbols files to the nuspec."
                
                $args = @(
                    "s:`"$NuDestFile`"",
                    "t:`"$transformFile`"",
                    "d:`"$NuDestFile`""
                )
                
                Write-Verbose " > $($Build.ctt) $args"
                & $($Build.ctt) $args 2>&1 | Write-Verbose
                
                if($LASTEXITCODE -ne 0) {
                    Write-Error "CTT EXIT CODE: $LASTEXITCODE"
                    
                    throw "Error while appending the symbols. Please review the ctt error."
                }
                
                Write-Verbose "Symbols were appended with success"
                
                Write-Output $NuDestFile
            }
        }
    }
}

function Package-Nuget {
    param (
        [Parameter(ValueFromPipeline)]
        [string[]]$NuspecFile,
        $OutputDirectory
    )
    process {
        $Logger = (Join-Path $Build.CentralLogsFolder "nugetpackage.log")

        $NuspecFile | % {
            "" >> $Logger
        
            Write-Verbose "Will create package $($Build.NugetExe) $_"
            
            $args = @(  "pack", $_, 
                        "-OutputDirectory", $OutputDirectory)
            & $Build.NugetExe @args 2>&1 >> $Logger
        }
    }	
}

function Invoke-NugetPush($NugetFeedUrl, $NugetPackageFile) {

	$tmp = $("`"{0}`" push {1} -Source {2}" -f $Build.NugetExe, $NugetPackageFile, $NugetFeedUrl )
	$tmp
	$WindowsVersion = [System.Environment]::OSVersion.Version.Major
	if($WindowsVersion -eq 10)
	{cmd.exe /C $tmp}
	if($WindowsVersion -ne 10)
	{cmd.exe /C "`"$tmp`""}
}

function Publish-Nuget($files,$NugetFeed) {
    Write-Verbose "Publish-Nuget to '$NugetFeed'"
    $Logger = (Join-Path $Build.CentralLogsFolder "nugetpush.log")

    "" >> $Logger
    foreach($file in $files)
    {
        Write-Verbose "Pushing  [$file_] to [$NugetFeed]..."
        Invoke-NugetPush -NugetFeedUrl $NugetFeed -NugetPackageFile $file_ 2>&1 >> $Logger
    }
}

function Invoke-MSDeploy{
    param(
        [Parameter(Mandatory)]
        [string]$Source,
        [Parameter(Mandatory)]
        [string]$Destination
    )
    process {
        # MsDeploy to packaged website folder
        $tmp = $("`"{0}`" -verb:sync -source:contentpath=`"{1}`" -dest:package=`"{2}`"" -f $Build.MsDeployExe, $source, $destination )
        
        $WindowsVersion = [System.Environment]::OSVersion.Version.Major
        if($WindowsVersion -eq 10)
        {cmd.exe /C $tmp}
        if($WindowsVersion -ne 10)
        {cmd.exe /C "`"$tmp`""}
    }
}

function UploadArtifact {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$ContainerFolder,
        [Parameter(Mandatory)]
        [string]$ArtifactName,
        [Parameter(Mandatory)]
        [string]$Path
    )

    if ($IsRunningOnBuildMachine)
    {
        # Artifacts are uploaded through VSTS logging commands: 
        #
        #    https://github.com/Microsoft/vso-agent-tasks/blob/master/docs/authoring/commands.md
        #
        Write-Host "##vso[artifact.upload containerfolder=$ContainerFolder;artifactname=$ArtifactName;]$Path"
    }
}

function Invoke-NUnit(){
[cmdletbinding()]
param(
	[string[]]
	$FilePathTestDlls
	,
	[string]
	$FilePathPatternTestDlls = $null
	,
	[string]
	$FilePathXmlReport = "TestResults.xml"
	,
	[string]
	$Workspace = $null
	,
	[string]
	$FullFilePathNUnitExe = "C:\Program Files (x86)\NUnit 2.6.4\bin\nunit-console.exe"
)
	try{
        Write-Verbose "Invoke-NUnit"
		Push-Location
		if($null -ne $Workspace){
			cd $Workspace
		}

		if($null -ne $FilePathPatternTestDlls){
			$globbedFiles = Get-ChildItem "$FilePathPatternTestDlls" -recurse -ErrorAction SilentlyContinue -ErrorVariable WontUseIt | ? {-not ((Split-Path $_.FullName) -ilike "*\obj\*")} | %{$_.FullName}
			$FilePathTestDlls = $globbedFiles
		}

        Write-Host $FilePathTestDlls

		foreach($dll in $FilePathTestDlls){
			if(-not (Test-Path -path "$dll" -ErrorAction SilentlyContinue)){
				Write-Warning "Entry [$dll] of `$FilePathTestDlls resolved no dll(s)"
			}else{
				Write-Verbose "Entry [$dll] of `$FilePathTestDlls resolved to :"
				Get-Item $dll | % { Write-Verbose "$_"}
			}
		}

		if($null -ne $FilePathTestDlls) {
            $args = [string[]]( $FilePathTestDlls + $FilePathXmlReport)
            Write-Verbose "Executing: $($FullFilePathNUnitExe) $($args)" 
			& "$FullFilePathNUnitExe" @args
		}else{
			Write-Warning "No dlls found, skipped unit tests"
		}
	}finally{
		Pop-Location
	}
}