$WorkspaceRoot  = Split-Path $PSScriptRoot -Parent
$BuildDirectory = $PSScriptRoot

$NugetPackagesRepository = "$WorkspaceRoot\packages\Nuget"

$packages = Copy-Item "$WorkspaceRoot\build\packages.bootstrap.config" -Destination (Join-Path $env:TEMP 'packages.config') -Force -PassThru
Exec {& "$WorkspaceRoot\lib\nuget\nuget.exe" restore $packages.FullName -PackagesDirectory $NugetPackagesRepository -ConfigFile "$WorkspaceRoot\build\nuget.config" -Verbosity quiet}

# Import bootstrap packages
Get-ChildItem "$NugetPackagesRepository\Orckestra.PsUtil.*" -Include "Orckestra.PsUtil.psd1" -Recurse -Force | Import-Module -Verbose:$false

$nugetVersion = (Get-GitVersion).NuGetVersion
$WindowsVersion = [System.Environment]::OSVersion.Version.Major