# Credits to Ingo Karstein, Karstein Consulting, Partner
# http://blog.karstein-consulting.com/
param(
	[Parameter(Mandatory=$true)]
	[string]
	$AccountUsername
,
	[Parameter(Mandatory=$true)]
	[ValidateSet('LogonAsAService','LogonAsABatchJob')]
	[string]
	$Right
,
	[string]$Component = "generic"
)

#=======================================
# Initialize Script Execution Context
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path
$scriptsPath = (get-item $currentScriptPath).parent.FullName
. $scriptsPath\core\Initialize-Context.ps1
#=======================================

$sidstr = $null
$internalRightName = if ($Right -match 'LogonAsAService') { 'SeServiceLogonRight' } else { 'SeBatchLogonRight' }
try 
{
	$ntprincipal = new-object System.Security.Principal.NTAccount "$AccountUsername"
	$sid = $ntprincipal.Translate([System.Security.Principal.SecurityIdentifier])
	$sidstr = $sid.Value.ToString()
} 
catch 
{
	$sidstr = $null
}

Write-Log action "Setting $internalRightName for account: $AccountUsername"

if( [string]::IsNullOrEmpty($sidstr) ) 
{
	Write-Log error "Account for '$AccountUsername' was not found!" 
	throw "Account not found"
}

Write-Log action "Account SID: $($sidstr)" 

$tmp = [System.IO.Path]::GetTempFileName()

Write-Log action "Exporting current Local Security Policy" 
secedit.exe /export /cfg "$($tmp)" 


$contentOfSecurityPolicy = Get-Content -Path $tmp

if ( ($contentOfSecurityPolicy -eq "") -or ($contentOfSecurityPolicy -eq $null))
{
	Write-Log error "secedit was not able to dump its security policy, aborting"
	throw "secedit.exe can't dump security policies."
}

$currentSetting = ""

foreach($s in $contentOfSecurityPolicy) 
{
	if( $s -like "$($internalRightName)*") 
	{
		$x = $s.split("=",[System.StringSplitOptions]::RemoveEmptyEntries)
		$currentSetting = $x[1].Trim()
	}
}

if ( $currentSetting -notlike "*$($sidstr)*" )
{
	Write-Log action "Adding User : '$AccountUsername' for $internalRightName"	
	$tmpFileName = (Get-Item $tmp).Name

	$currentScriptName = [io.path]::GetFileNameWithoutExtension($MyInvocation.MyCommand.Name)
	$backupSessionPath = Join-Path (Get-BackupFolderPath) "LocalSecurityPolicy"

	Write-Log action "Backing up current Local Security Policy to: $backupSessionPath\$tmpFileName" 
	mkdir $backupSessionPath -Force
	Copy-Item $tmp $backupSessionPath
	$instructions = "To restore, use the following command:`r`n secedit.exe /configure /db `"secedit.sdb`" /cfg $tmpFileName /areas USER_RIGHTS"
	Add-Content -Path $(Join-Path $backupSessionPath "Readme.txt") $instructions -ErrorAction SilentlyContinue
}
else
{	
	Write-Log warning "Adding User '$AccountUsername' skipped, already existing..."
}

if( $currentSetting -notlike "*$($sidstr)*" ) 
{
	"Modify Setting : Allow $Right"
	if( [string]::IsNullOrEmpty($currentSetting) ) 
	{
		$currentSetting = "*$($sidstr)"
	} 
	else 
	{
		$currentSetting = "*$($sidstr),$($currentSetting)"
	}
	
	Write-Host "$currentSetting"
	
	$outfile = @"
[Unicode]
Unicode=yes
[Version]
signature="`$CHICAGO`$"
Revision=1
[Privilege Rights]
$internalRightName = $($currentSetting)
"@

	$tmp2 = [System.IO.Path]::GetTempFileName()
	Write-Output "Import new settings to Local Security Policy" 
	$outfile | Set-Content -Path $tmp2 -Encoding Unicode -Force
	Push-Location (Split-Path $tmp2)
	try 
	{
		secedit.exe /configure /db "secedit.sdb" /cfg "$($tmp2)" /areas USER_RIGHTS 
		#write-host "secedit.exe /configure /db ""secedit.sdb"" /cfg ""$($tmp2)"" /areas USER_RIGHTS "
	} 
	finally 
	{	
		Pop-Location
	}
} 
