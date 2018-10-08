param(
	[string]
	$NugetFeed = "file:///C:/LocalNuget/DEV/",
	[string]
	$version = "AssemblyVersion",
    [ValidateSet("Package", "Publish", "PackageAndPublish")]
    [string]
    $Action = "PackageAndPublish",
    [ValidateSet("Build", "PreRelease", "Release")]
    [string]
    $Mode = "Build",
    [Switch]
    $Symbols
)

#=====================================================================
$UseSymbols = $Symbols -and $Mode.ToLower() -ne "build"
$ctt = ".\NugetFiles\buildTools\ctt\ctt.exe"
#=====================================================================

Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force

function Clean-Packages() {
    Write-Host "`r`n============================================================="
    Write-Host "Cleaning packages" -ForegroundColor Yellow
    Write-Host "=============================================================`r`n"
    
    $items = Get-ChildItem ".\*.nupkg"
    
    Write-Host "Deleting packages: $items"
    
    $items | Remove-Item -force
}

function Get-Version() {
    if($version -ilike "AssemblyVersion") {$versionSource = "AssemblyVersion"}
	if($version -ilike "AssemblyFileVersion") {$versionSource = "AssemblyFileVersion"}

	$assemblyInfoFile = (Get-ChildItem -Recurse "**/SharedAssemblyInfo.cs" -ErrorAction SilentlyContinue)[0]
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

function Transform-Nuspec($NuspecFile, $NuDestFile, $NuVersion) {
	$content = [string[]](get-content $NuspecFile)
	$content | % {
		$_ -replace '<version>(.*)<\/version>', ('<version>'+$NuVersion+'</version>')
	} | Set-Content $NuDestFile
    
    $transformFile = $NuspecFile.Replace("template.nuspec", "template.symbols.nuspec")
    $hasTransforms = (Test-Path $transformFile)
    if($UseSymbols -and $hasTransforms) {
        Write-Host "Appending symbols files to the nuspec."
        
        $args = @(
            "s:`"$NuDestFile`"",
            "t:`"$transformFile`"",
            "d:`"$NuDestFile`""
        )
        
        Write-Host " > $ctt $args" -ForegroundColor DarkGray
        & $ctt $args 2>&1 | Write-Host -ForegroundColor DarkGray
        
        if($LASTEXITCODE -ne 0) {
            Write-Host "CTT EXIT CODE: $LASTEXITCODE" -ForegroundColor DarkGray
            
            throw "Error while appending the symbols. Please review the ctt error."
        }
        
        Write-Host "Symbols were appended with success" -ForegroundColor DarkGreen
    }
}

function Invoke-NugetPackage($NugetExeLocation, $NuspecFile, $NuVersion) {
    Write-Host "Will create package with assembly version $NuVersion" -ForegroundColor DarkGray
    
    $nuspecFinalFile = $NuspecFile.Replace(".template.nuspec", ".nuspec")
    Transform-Nuspec $NuspecFile $nuspecFinalFile $NuVersion
    
	$args = @("pack", "$nuspecFinalFile")
    
	& "$NugetExeLocation\NuGet.exe" @args
}

function Invoke-NugetPush($NugetExeLocation, $NugetFeedUrl, $NugetPackageFile) {

	$tmp = $("`"{0}\Nuget.exe`" push {1} -Source {2}" -f $NugetExeLocation, $NugetPackageFile, $NugetFeedUrl )
	$tmp
	$WindowsVersion = [System.Environment]::OSVersion.Version.Major
	if($WindowsVersion -eq 10)
	{cmd.exe /C $tmp}
	if($WindowsVersion -ne 10)
	{cmd.exe /C "`"$tmp`""}

return
	$args = @("push", "`"$NugetPackageFile`"", "-Source `"$NugetFeedUrl`"")
	& "$NugetExeLocation\NuGet.exe" @args
}

function Package-Nuget($nugetVersion) {
    Write-Host "`r`n============================================================="
    Write-Host "Packaging NuGet" -ForegroundColor Yellow
    Write-Host "=============================================================`r`n"
    
    "" > "..\nugetpackage.log"
    $nuspecs = [string[]](get-childitem ".\NugetFiles\*.template.nuspec" | % {$_.name})
    foreach($nuspec in $nuspecs) {
        Write-Host "Packaging $nuspec" -ForegroundColor Green
        Invoke-NugetPackage -NugetExeLocation ".\.nuget" -NuspecFile .\NugetFiles\$nuspec -NuVersion $nugetVersion 2>&1 >> "..\nugetpackage.log"
    }
    Get-Content "..\nugetpackage.log"
}

function Publish-Nuget($nugetVersion) {
    Write-Host "`r`n============================================================="
    Write-Host "Publishing NuGet to '$NugetFeed'" -ForegroundColor Yellow
    Write-Host "=============================================================`r`n"
    
    "" > "..\nugetpush.log"
    $packages = [string[]](Get-ChildItem ".\*.$nugetVersion.nupkg" | % {$_.name})
    foreach($package in $packages) {
        Write-Host "Pushing  [$package] to [$NugetFeed]..." -ForegroundColor DarkGreen
        
        Invoke-NugetPush -NugetExeLocation ".\.nuget" -NugetFeedUrl $NugetFeed -NugetPackageFile .\$package 2>&1 >> "..\nugetpush.log"
    }
    Get-Content "..\nugetpush.log"
}

#-------------------------------------------------
## Main
#-------------------------------------------------
if($Symbols -and $Mode.ToLower() -eq "build") {
    Write-Host "The 'Build' mode is not compatible with the Symbols flag. The Symbols flag will be ignored." -ForegroundColor Gray
}

$nugetVersion = Get-Version

if($Action.toLower() -eq "package" -or $Action.toLower() -eq "packageandpublish") {
    Clean-Packages
    Package-Nuget $nugetVersion
}

if($Action.toLower() -eq "publish" -or $Action.toLower() -eq "packageandpublish") {
    Publish-Nuget $nugetVersion
}