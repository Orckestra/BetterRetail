# This script is used to restore the Git LFS files. This should only be used 
# when building on VSTS hosted agents where LFS files are not automatically
# downloaded when cloning repository.
[cmdletbinding()]
param()

$ErrorActionPreference = 'Stop'

# Inspired from here: http://ss64.com/ps/syntax-set-eol.html
function Set-UnixLineEndings([string]$file)
{
    # Replace CR+LF with LF
    $text = [IO.File]::ReadAllText($file) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($file, $text)

    # Replace CR with LF
    $text = [IO.File]::ReadAllText($file) -replace "`r", "`n"
    [IO.File]::WriteAllText($file, $text)
}

filter Convert-ErrorRecordToString
{
    <#
    .SYNOPSIS
    Converts ErrorRecord to string in a stream of objects that can contain a mixture of ErrorRecord and other kinds objects.

    .DESCRIPTION
    This function is often useful when you call an external program that writes on STDERR. 

    When a program writes on STDERR, PowerShell will convert all lines written on STDERR into ErrorRecord objects. 
    It may be useful to get rid of these ErrorRecord. For example, if a VSTS build receives object of type 
    ErrorRecord, it will automatically mark the build as failed. In this case, the current function can be used 
    to get rid of these ErrorRecord.
    #>
    if ($_ -is [System.Management.Automation.ErrorRecord])
    {
        $_.ToString()
    }
    else
    {
        $_
    }
}

$WorkspaceRoot = Split-Path $PSScriptRoot -Parent

if ((Test-Path env:SYSTEM_ACCESSTOKEN) -eq $false)
{
    throw "OAuth token not available. Make sure that you select the option 'Allow Scripts to Access OAuth Token' in build 'Options' pane."
}

# git lfs needs the credentials of the git repository. When running
# under VSTS, these credentials are transfered to the git-lfs.exe
# application using the oauth token provided by VSTS. These
# credentials are stored in a file so that git lfs can find them.

$pwPath = Join-path $WorkspaceRoot 'pw.txt'
$gitPwPath = $pwPath.Replace('\', '/')    # Needs to be in unix format.

$repoUri = New-Object Uri $env:BUILD_REPOSITORY_URI

git config credential.helper "store --file=$gitPwPath"
@"
https://OAuth:$env:SYSTEM_ACCESSTOKEN@$($repoUri.Host)
"@ | Set-Content $pwPath

# Again, needs to be in unix format... sigh...
Set-UnixLineEndings -file $pwPath

$gitLfsZip = Join-Path $env:TEMP 'GitLfs.zip'
Invoke-WebRequest -Uri 'https://github.com/github/git-lfs/releases/download/v1.2.1/git-lfs-windows-amd64-1.2.1.zip' -OutFile $gitLfsZip

$gitLfsFolder = Join-Path $env:TEMP 'GitLfs'
if (Test-Path $gitLfsFolder)
{
    Remove-Item -Recurse $gitLfsFolder -Force
}

New-Item -ItemType Directory $gitLfsFolder > $null

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($gitLfsZip, $gitLfsFolder)

Push-Location "$WorkspaceRoot"
try
{
    $gitLfsExe = (Get-ChildItem -Path "$gitLfsFolder" -Include "git-lfs.exe" -Recurse).FullName

    # Git LFS Pull writes on stderror when it fails. Don't stop when it occurs.
    $ErrorActionPreference = 'Continue'
    & $gitLfsExe pull 2>&1 | Convert-ErrorRecordToString

    if ($LASTEXITCODE -ne 0)
    {
        $pullExitCode = $LASTEXITCODE

        # As suggested here, we log a few things:
        #
        #   https://github.com/github/git-lfs/issues/512

        Write-Host "**** LFS Log ****"
        & $gitLfsExe logs last

        Write-Host "**** LFS Env ****"
        & $gitLfsExe env

        Write-Host "**** LFS fschk ****"
        & $gitLfsExe fsck
        
        throw "Failed to pull LFS files: $pullExitCode"
    }
}
finally
{
    Pop-Location
}
