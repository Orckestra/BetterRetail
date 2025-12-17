##########################################################################
# This is the Cake bootstrapper script for PowerShell.
# This file was downloaded from https://github.com/cake-build/resources
# Feel free to change this file to fit your needs.
##########################################################################

<#

.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.

.DESCRIPTION
This Powershell script will download NuGet if missing, restore NuGet tools (including Cake)
and execute your Cake build script with the parameters you provide.

.PARAMETER Script
The build script to execute.
.PARAMETER Target
The build script target to run.
.PARAMETER Configuration
The build configuration to use.
.PARAMETER CakeVerbosity
Specifies the amount of information to be displayed.
.PARAMETER ShowDescription
Shows description about tasks.
.PARAMETER DryRun
Performs a dry run.
.PARAMETER SkipToolPackageRestore
Skips restoring of packages.
.PARAMETER RemainingArguments
$RemainingArguments is an array of all parameters not bound to this PowerShell script's parameters, as commanded by parameter attribute ValueFromRemainingArguments=$true.

.LINK
https://cakebuild.net

#>

[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [string]$Target,
    [string]$Configuration,
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$CakeVerbosity,
    [switch]$ShowDescription,
    [Alias("WhatIf", "Noop")]
    [switch]$DryRun,
    [switch]$SkipToolPackageRestore,
    [switch]$Docs,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]  # $RemainingArguments is an array of all parameters not bound to this PowerShell script's parameters, as commanded by parameter attribute ValueFromRemainingArguments=$true.
    [string[]]$RemainingArguments
)

$scriptpath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path $scriptpath


[Reflection.Assembly]::LoadWithPartialName("System.Security") | Out-Null
function MD5HashFile([string] $filePath)
{
    if ([string]::IsNullOrEmpty($filePath) -or !(Test-Path $filePath -PathType Leaf))
    {
        return $null
    }

    [System.IO.Stream] $file = $null;
    [System.Security.Cryptography.MD5] $md5 = $null;
    try
    {
        $md5 = [System.Security.Cryptography.MD5]::Create()
        $file = [System.IO.File]::OpenRead($filePath)
        return [System.BitConverter]::ToString($md5.ComputeHash($file))
    }
    finally
    {
        if ($null -ne $file)
        {
            $file.Dispose()
        }
    }
}

function GetProxyEnabledWebClient
{
    $wc = New-Object System.Net.WebClient
    $proxy = [System.Net.WebRequest]::GetSystemWebProxy()
    $proxy.Credentials = [System.Net.CredentialCache]::DefaultCredentials        
    $wc.Proxy = $proxy
    return $wc
}

Write-Host "Preparing to run build script..."

Write-Host "`$`(Build.SourcesDirectory) = `$env:Build_SourcesDirectory = $($env:Build_SourcesDirectory)"
Write-Host "`$PWD = $($PWD)"
if (Test-Path Orckestra.StarterSite) {
    Write-Host "Orckestra.StarterSite exists"
}
else
{
    Write-Host "Orckestra.StarterSite does not exist"
}

Push-Location $scriptDir
Write-Host "`$PWD = $($PWD)"
Try
{
    $TOOLS_DIR = Join-Path $PSScriptRoot "tools"
    $ADDINS_DIR = Join-Path $TOOLS_DIR "Addins"
    $MODULES_DIR = Join-Path $TOOLS_DIR "Modules"
    $NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"
    $NUGET_URL = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    $PACKAGES_CONFIG = Join-Path $TOOLS_DIR "packages.config"
    $PACKAGES_CONFIG_MD5 = Join-Path $TOOLS_DIR "packages.config.md5sum"
    $ADDINS_PACKAGES_CONFIG = Join-Path $ADDINS_DIR "packages.config"
    $MODULES_PACKAGES_CONFIG = Join-Path $MODULES_DIR "packages.config"

    # Make sure tools folder exists
    if ((Test-Path $PSScriptRoot) -and !(Test-Path $TOOLS_DIR)) {
        Write-Verbose -Message "Creating tools directory..."
        New-Item -Path $TOOLS_DIR -Type directory | out-null
    }

    # Try find NuGet.exe in path if not exists
    if (!(Test-Path $NUGET_EXE)) {
        Write-Verbose -Message "Trying to find nuget.exe in PATH..."
        $existingPaths = $env:Path -Split ';' | Where-Object { (![string]::IsNullOrEmpty($_)) -and (Test-Path $_ -PathType Container) }
        $NUGET_EXE_IN_PATH = Get-ChildItem -Path $existingPaths -Filter "nuget.exe" | Select-Object -First 1
        if ($null -ne $NUGET_EXE_IN_PATH -and (Test-Path $NUGET_EXE_IN_PATH.FullName)) {
            Write-Verbose -Message "Found in PATH at $($NUGET_EXE_IN_PATH.FullName)."
            $NUGET_EXE = $NUGET_EXE_IN_PATH.FullName
        }
    }

    # Try download NuGet.exe if not exists
    if (!(Test-Path $NUGET_EXE)) {
        Write-Verbose -Message "Downloading NuGet.exe..."
        try {
            $wc = GetProxyEnabledWebClient
            $wc.DownloadFile($NUGET_URL, $NUGET_EXE)
        } catch {
            Throw "Could not download NuGet.exe."
        }
    }

    # Save nuget.exe path to environment to be available to child processed
    $env:NUGET_EXE = $NUGET_EXE

    # Restore tools from NuGet?
    if(-Not $SkipToolPackageRestore.IsPresent) {
        Push-Location
        Set-Location $TOOLS_DIR

        # Check for changes in packages.config and remove installed tools if true.
        [string] $md5Hash = MD5HashFile($PACKAGES_CONFIG)
        if((!(Test-Path $PACKAGES_CONFIG_MD5)) -Or
        ($md5Hash -ne (Get-Content $PACKAGES_CONFIG_MD5 ))) {
            Write-Verbose -Message "Missing or changed package.config hash..."
            Remove-Item * -Recurse -Exclude packages.config,nuget.exe
        }

        Write-Verbose -Message "Restoring tools from NuGet..."
        $NuGetOutput = Invoke-Expression "&`"$NUGET_EXE`" install -ExcludeVersion -OutputDirectory `"$TOOLS_DIR`""

        if ($LASTEXITCODE -ne 0) {
            Throw "An error occurred while restoring NuGet tools."
        }
        else
        {
            $md5Hash | Out-File $PACKAGES_CONFIG_MD5 -Encoding "ASCII"
        }
        Write-Verbose -Message ($NuGetOutput | out-string)

        Pop-Location
    }

    # Restore addins from NuGet
    if (Test-Path $ADDINS_PACKAGES_CONFIG) {
        Push-Location
        Set-Location $ADDINS_DIR

        Write-Verbose -Message "Restoring addins from NuGet..."
        $NuGetOutput = Invoke-Expression "&`"$NUGET_EXE`" install -ExcludeVersion -OutputDirectory `"$ADDINS_DIR`""

        if ($LASTEXITCODE -ne 0) {
            Throw "An error occurred while restoring NuGet addins."
        }

        Write-Verbose -Message ($NuGetOutput | out-string)

        Pop-Location
    }

    # Restore modules from NuGet
    if (Test-Path $MODULES_PACKAGES_CONFIG) {
        Push-Location
        Set-Location $MODULES_DIR

        Write-Verbose -Message "Restoring modules from NuGet..."
        $NuGetOutput = Invoke-Expression "&`"$NUGET_EXE`" install -ExcludeVersion -OutputDirectory `"$MODULES_DIR`""

        if ($LASTEXITCODE -ne 0) {
            Throw "An error occurred while restoring NuGet modules."
        }

        Write-Verbose -Message ($NuGetOutput | out-string)

        Pop-Location
    }
    
    # Build Cake arguments
    # See https://cakebuild.net/docs/running-builds/runners/dotnet-tool
    $cakeArguments = @("$Script");
    if ($Target) { $cakeArguments += "--target=$Target" }
    if ($Configuration) { $cakeArguments += "--configuration=$Configuration" }
    if ($CakeVerbosity) { $cakeArguments += "--verbosity=$CakeVerbosity" }
    if ($ShowDescription) { $cakeArguments += "--showdescription" }
    if ($DryRun) { $cakeArguments += "--dryrun" }
    if ($Docs) {
        Write-Host "-----------------------------------------------------------------------------------------------"
        Write-Host "-docs                     Displays available commands"
        Write-Host "-t All                    Executes specific target, default is 'ALL'"
        Write-Host "-Configuration Release    Build configuration"
        Write-Host "--package-version=0.0.1   Nuget package version. If not specified, taken from 'SharedAssemblyInfo.cs'"
        Write-Host "`nTarget values:"
        $cakeArguments += "--tree"
    }

    $cakeArguments += $RemainingArguments

    # Start Cake
    Write-Host "Running build script..."
    dotnet cake $cakeArguments
}
Finally
{
    Pop-Location
}


exit $LASTEXITCODE
