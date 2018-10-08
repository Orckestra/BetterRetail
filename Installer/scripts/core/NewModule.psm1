#requires -version 3.0
$currentScriptPath	= Split-Path $MyInvocation.MyCommand.Path
$rootPath			= (gi $currentScriptPath).parent.parent.FullName
$tools				= Join-Path $rootPath "tools"
$ProgressPreference = "SilentlyContinue"

Set-Alias msbuild (gi "$Env:SystemRoot\Microsoft.NET\Framework64\v*\MSBuild.exe" | Sort-Object -Property Fullname -Descending | Select-Object -First 1) -Option AllScope -Scope Global

function New-WindowsService {
param(
	[Parameter(Mandatory=$true)][string]$Name,
	[Parameter(Mandatory=$true)][string]$BinaryPathName,
	[Parameter(Mandatory=$true)][string]$DisplayName,
	[Parameter(Mandatory=$true)][string]$Description,
	[Parameter(Mandatory=$true)][string]$Login,
	[Parameter(Mandatory=$true)][string]$Password
)
	"Creating windows service '$Name' "
	
	# if user is not a domain user, modify it to be local, otherwise service registration will fail if no machine or domain is specified...
	if($Login -notmatch '\\') {
		"No machine or domain name detected, modyfing login to appear local: from '$Login' to .\$Login"
		$Login = ".\$Login"
	}

	$secretpasswd  = ConvertTo-SecureString $Password -AsPlainText -Force
	$creds_object  = New-Object System.Management.Automation.PSCredential ($Login, $secretpasswd)
	$cmdResponse   = New-Service -Name $Name -BinaryPathName $BinaryPathName -DisplayName $DisplayName -Description $Description -StartupType Automatic -Credential $creds_object
	if ($cmdResponse) {	
		"Windows service '$Name' created successfully."
		$cmdResponse
	} else {
		throw "Unable to install the Windows service '$Name'."
	}
}

function Remove-WindowsService {
param(
	[Parameter(Mandatory=$true)][string]$Name
)
	$service = Get-WmiObject -Class Win32_Service -Filter "name='${Name}'"
	if ($service -eq $null)	{
		Write-Log warning "Servive '$Name' is not installed in the system."
	} else {
		$service.delete()
	}
}

function Add-HostFileEntry {
param(
	[Parameter(Mandatory=$true)][string]$Hostname,
	$Ip = "127.0.0.1"
)
	try {	
		$hostsFile = Join-Path $env:windir "System32\drivers\etc\hosts"
		$tmpFile = Join-Path $(Get-RandomTempFolderPath) $([System.IO.Path]::GetRandomFileName())
		Copy-Item $hostsFile $tmpFile -Force
		
		$lines = (Get-Content $tmpFile) | ? { $_ -notmatch $Hostname }	
		if ($lines) {
			$lines += "$Ip		$Hostname"
			Set-Content $tmpFile -value $lines
			Copy-Item $tmpFile $hostsFile		
		}
	} catch {
		throw "Add-HostFileEntry failed!"
	}
}

function Remove-HostFileEntry {
param(
	[Parameter(Mandatory=$true)][string]$Hostname
)		
	try {	
		$hostsFile = Join-Path $env:windir "System32\drivers\etc\hosts"
		$tmpFile = Join-Path $(Get-RandomTempFolderPath) $([System.IO.Path]::GetRandomFileName())
		Copy-Item $hostsFile $tmpFile
		
		$lines = (Get-Content $tmpFile) | ? { $_ -notmatch $Hostname }	
		if ($lines) {
			Set-Content $tmpFile -value $lines -Force		 
			Copy-Item $tmpFile $hostsFile
		}
	} catch {
		throw "Remove-HostFileEntry failed!"
	}
}

function Test-NetworkPath {
param([string]$path)
	[string]$driveName = split-path $path -qualifier -erroraction silentlycontinue
	if (![string]::IsNullOrWhiteSpace($driveName)) {
		$drive = new-object system.io.driveinfo($driveName)	
		return ($drive.drivetype -eq "Network")
	}	
	return $true
}

function Test-LocalMachine {
param([string]$server)
	$dnsDomain      = $env:USERDNSDOMAIN
	$computerName   = $env:COMPUTERNAME
	$fullNameDns    = "${computerName}.${dnsDomain}"
	if ($server -eq "localhost")	{ return $true }
	if ($server -eq $computerName) { return $true }
	if ($server -eq $fullNameDns)  { return $true }
	if ($server -eq "127.0.0.1")   { return $true }
	return $false
}

function Get-FullExceptionMessage {
param(
	[Exception]$exception
)
	if ($exception.InnerException) {
		$innerMessage = Get-FullExceptionMessage $exception.InnerException
		return "$($exception.Message) --> $innerMessage"
	}
	return $exception.Message
}

function Invoke-ConfigTransformation {
param(
	[Parameter(Mandatory=$true)][string]$source, 
	[Parameter(Mandatory=$true)][string[]]$transform,
	[Parameter(Mandatory=$true)][string]$destination,
	$Parameters = @{},
	$ParamFilePath,
	[bool]$CleanDestination = $true,
	[bool]$WarnWhenTokenNotReplaced = $true,
	[bool]$ThrowWhenTokenNotReplaced = $false
)
	# Clone the source into a temp file to perform the multi transformation
	$tempFile = [System.IO.Path]::GetTempFileName()
	Copy-Item $source $tempFile -Force
	
	$paramStr = ""
	if ($parameters -and ($parameters.count -gt 0)) {		
		$parameters.Keys | % { $paramStr += "$($_):$($parameters[$_]);" }
		$paramStr = $paramStr.TrimEnd(';')
	}
	
	"====================================================" | Out-Log
	"Source`t`t: $source"								   | Out-Log
	"Destination`t: $destination"						   | Out-Log
	"Temp file`t: $tempFile"							   | Out-Log
	"Param file`t: $ParamFilePath" 						   | Out-Log
	"Param str`t: $paramStr"							   | Out-Log	
	"====================================================" | Out-Log
	$cttPath = Join-Path $tools "ctt\ctt.exe"
	$transform | % {
		$currentTransform = $_		
		"----------------------------------------------------" | Out-Log	
		"Transform`t: $currentTransform"					   | Out-Log
		"----------------------------------------------------" | Out-Log	
		. "$cttPath" source:$tempFile transform:$currentTransform destination:$tempFile p:"$paramStr" pf:"$ParamFilePath" preservewhitespace v | out-log

		if ($lastexitcode -ne 0) {
			$errMsg = "An error occurred during the config transformation $currentTransform"
			Write-Log ERROR $errMsg
			throw $errMsg
		}
	}
	"====================================================" | Out-Log
	
	# Copy the temp file to the actual destination
	Copy-Item $tempFile $destination -Force
	
	if ($CleanDestination) {
		$fileInfo = Split-FileInformation $destination
		Remove-Item "$($fileInfo.directoryName)\$($fileInfo.fileNameWithoutExtension).*$($fileInfo.extension)" -Force
	}
	
	Test-FileLockPath $destination 5
	$xml = (Get-Content $destination)
	
	Save-XmlAsFormattedDocument $xml $destination
		
	if ($warnWhenTokenNotReplaced -or $throwWhenTokenNotReplaced) {
		Test-TokenReplacement $destination -warnWhenTokenNotReplaced $warnWhenTokenNotReplaced -throwWhenTokenNotReplaced $throwWhenTokenNotReplaced
	}
}

function Save-XmlAsFormattedDocument {
param(
	[Parameter(Mandatory=$True)][xml]$xml,
	[Parameter(Mandatory=$True)][string]$xmlConfigPath	
)
	# Retrieve encoding specified in the xml file
	$encoding = $null
	if ($xml.xml -and $xml.xml.Contains( "encoding")) {
		$regex = [regex]::Matches($xml.xml, "encoding[\s]*=[\s]*[`"`'](.*)[`"`']") #"
		if ($regex.value) {
			$encodingInfo = [System.Text.Encoding]::GetEncodings() | ? { $_.Name -eq $regex.groups[1].Value }
			if ($encodingInfo) {
				$encoding = $encodingInfo.GetEncoding()
			}
		}
	}

	New-Item -ItemType Directory -Force -Path (Split-Path $xmlConfigPath -Parent) | Out-Null

	Test-FileLockPath $xmlConfigPath 5
	$writer = New-Object System.Xml.XmlTextWriter($xmlConfigPath, $encoding);
	$writer.Formatting = [System.Xml.Formatting]::Indented;
	$xml.Save($writer) | Out-Null
	$writer.Dispose()
}

function Test-TokenReplacement {
param(
	[Parameter(Mandatory=$true)][string]$fileToValidate, 
	[bool]$warnWhenTokenNotReplaced = $true,
	[bool]$throwWhenTokenNotReplaced = $false
)
	"TokenReplacement validation for file: $fileToValidate"
	$fileContent = (Get-Content $fileToValidate)
	[regex]::Matches($fileContent, "{([\S-[}]]*)}") | % {
		$notReplaceToken = $_.groups[0].Value
		if (!($notReplaceToken -match "^\{[a-fA-F\d]{8}-([a-fA-F\d]{4}-){3}[a-fA-F\d]{12}\}$")) { #skip GUID
			if (!($notReplaceToken -match "^\{[0-9a-zA-Z\d][,:]?[0-9a-zA-Z\d]?\}$")) { #skip {0} and {0,} and {a,2} and {0:} and {0:a} and {b,5}  etc.
				$message = "Not Replaced Token: $notReplaceToken"
				if ($throwWhenTokenNotReplaced) {
					throw "$($message). File: '$($fileToValidate)'."
				} elseif ($warnWhenTokenNotReplaced) {
					Write-Log WARNING $message
				} else {
					"$message"
				}
			} 
		}
	}
}

function Add-DiagnosticsEventLogSources {
param(
	[Parameter(Mandatory=$true)][string[]]$EventSources,
	[string]$LogName = "Application"
)
	$EventSources | % {
		if(![System.Diagnostics.EventLog]::SourceExists($_))
		{
			New-EventLog -Source $_ -LogName $LogName
		}
	}
}

function Remove-DiagnosticsEventLogSources {
param(
	[Parameter(Mandatory=$true)][string[]]$EventSources
)
	$EventSources | % {
		if([System.Diagnostics.EventLog]::SourceExists($_))
		{
			Remove-EventLog -Source $_
		}
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
		Write-Log ERROR $codeMsg
		throw $errMsg
	} if ($RetVal -ge 2) {
		Write-Log WARNING $codeMsg
	} else {
		"$codeMsg"
	}
}

function Invoke-UnzipFile {
param (
	[string] $ZipFile = $(throw "zipFile must be specified."),
	[string] $DestinationDir = $(throw "DestinationDir must be specified.")
)		
	$zipCommand = Join-Path $tools "7zip\7z.exe"
	if (!(Test-Path $zipCommand)) {
		throw "7z.exe was not found at $ZipCommand."
	}
	set-alias zip $zipCommand
			 
	if (!(Test-Path($ZipFile))) {
		throw "Zip filename does not exist: $ZipFile"
		return
	}
	 
	zip x -y "-o$DestinationDir" $ZipFile
	Confirm-7zipExecution $lastexitcode	$ZipFile
}

function Invoke-ZipFile {
param (
	[string] $SourceDir = $(throw "SourceDir must be specified."),
	[string] $DestinationZipFile = $(throw "DestinationZipFile must be specified."),
	[string] $OptionZipFile
)		
	$zipCommand = Join-Path $tools "7zip\7z.exe"
	if (!(Test-Path $zipCommand)) {
		throw "7z.exe was not found at $ZipCommand."
	}
	set-alias zip $zipCommand
			 
	if (!(Test-Path($SourceDir))) {
		throw "SourceDir does not exist: $SourceDir"
		return
	}
	 
	Write-Output "Zipping '$SourceDir' to '$DestinationZipFile'."
	zip a -y -tzip $DestinationZipFile $SourceDir $OptionZipFile
	Confirm-7zipExecution $lastexitcode	$DestinationZipFile
}
	 
function Confirm-7zipExecution {
param (
	$retVal,
	[string]$ZipFile
)
	if ($retVal -eq 0) {
		#0 No error 
	} elseif ($retVal -eq 1) {
		Write-Log WARNING "7-zip finish with a Warning (Non fatal error(s)). For example, one or more files were locked by some other application, so they were not compressed."
	} else {
		if ($retVal -eq 255) {
			$errMsg = "User stopped the process"
		} elseif ($retVal -eq 9) {
			$errMsg = "Create file error"
		} elseif ($retVal -eq 8) {
			$errMsg = "Not enough memory for operation"
		} elseif ($retVal -eq 7) {
			$errMsg = "Command line option error"
		} elseif ($retVal -eq 6) {
			$errMsg = "Open file error"
		} elseif ($retVal -eq 5) {
			$errMsg = "Write to disk error"
		} elseif ($retVal -eq 4) {
			$errMsg = "Attempt to modify an archive previously locked"
		} elseif ($retVal -eq 3) {
			$errMsg = "A CRC error occurred when unpacking"
		} elseif ($retVal -eq 2) {
			$errMsg = "A fatal error occurred"
		} else {
			$errMsg = "Unkown error"
		}	
		Write-Log ERROR "7-zip finish with a fatal error(s) for file : $ZipFile"
		throw " : $($errMsg)."
	}
}

<#
.EXAMPLE
    Grant-FolderPermission -Username 'domain\user' -Path c:\temp -Rights FullControl
		Grant-FolderPermission -Username 'domain\user' -Path c:\temp -Rights Read,Write
		
		See [System.Security.AccessControl.FileSystemRights] for all possible for $Rights
#>
function Grant-FolderPermission {
param(
	[Parameter(Mandatory = $true)]$Username,
	[Parameter(Mandatory = $true)][ValidateScript({Test-Path $_ -PathType 'Container'})]$Path,
	[Parameter(Mandatory = $true)][ValidateScript({[System.Security.AccessControl.FileSystemRights].GetEnumValues() -contains $_ })]$Rights,
	[bool]$EnsureUserExits = $false,
	[string]$UserPassword
)
	if($Username -eq 'LocalSystem') {
		Write-Log action "Interpreting user 'LocalSystem' to 'System'."
		$Username = 'System'
	}
	#When installing for the first time the IIS Apppool account will not be created and available to grant user rights until it has runned.
	#To work around this to grant permissions we set the permissions to the local users group to which all IIS APPPool virtual accounts belong
	if($Username.toLower().Contains("iis apppool")){
			Write-Log INFO "Cannot create account '$Username' and grant permissions. Permissions will be granted to local Users group"
            $Username='Users'
    }

	try {
		if (!(Test-User $Username) -and $EnsureUserExits) {
			if($Username.toLower().Contains("iis apppool")){
				Write-Log WARNING "Could not create account '$Username' and grant permissions";
			}
			else{
				New-LocalUser $Username $UserPassword
			}
		}
	} catch {
		#TODO: fix the issue related with the fact that we don't look in the AD looking on the computer
	}
	
	Write-Log action "Granting access to user '$Username' on folder '$Path'..."
	$rule = New-Object Security.AccessControl.FileSystemAccessRule $Username, $Rights, 'ContainerInherit,ObjectInherit', 'None', 'Allow'
	$acl = Get-Acl $Path
	$acl.AddAccessRule($rule)
	$acl.SetOwner([System.Security.Principal.NTAccount] "Administrators")
	Set-Acl $Path $acl
}

function Grant-CertificatePermission {
param(
	$CertificatePath = $(throw 'certificatePath is required.')
)
	if ($CertificatePath) {
		$CertificatePath | % {
			if (Test-Path $_) {
				$certificate = Get-Item $_
				$keyPath = $env:ProgramData + "\Microsoft\Crypto\RSA\MachineKeys\" + $certificate.PrivateKey.CspKeyContainerInfo.UniqueKeyContainerName
				$rule = New-Object Security.AccessControl.FileSystemAccessRule "IIS_IUSRS", "FullControl", "Allow"
				$acl = Get-Acl $keyPath
				$acl.AddAccessRule($rule)
				Set-Acl $keyPath $acl
			} else {
				throw "Unable to find the specified certificate path ('$_')."
			}			
		}
	}
}

function Test-User {
param(
	[Parameter(Mandatory=$true)][string] $userName
)
	#TODO: check in the AD before looking on the computer
	$objOu = [ADSI]"WinNT://${env:Computername}"
	$localUsers = $objOu.Children | where {$_.SchemaClassName -eq 'user'}  | % {$_.name[0].ToString()}
	return ($localUsers -Contains $userName)
}

function New-LocalUser {
param(
	[Parameter(Mandatory=$true)][string] $userName,
	[Parameter(Mandatory=$true)][string] $passWord
)
	$objOu = [ADSI]"WinNT://${env:Computername}"
	$localUsers = $objOu.Children | where {$_.SchemaClassName -eq 'user'}  | % {$_.name[0].ToString()}
	if($localUsers -NotContains $userName){
		Write-Log action "Creating local '$Username'..."
		$objUser = $objOU.Create("User", $userName)
		$objUser.setpassword($password)
		$objUser.SetInfo()
		$objUser.description = "Overture Deployment Generated User"
		$objUser.SetInfo()
		return $true
	}
	return $false
}

function Start-WebsiteWithItsAppPool($websiteName) {
	Import-WebAdministrationModuleSafely

	$webSite = Get-Website | Where { $_.Name -eq "$websiteName" }
	if ($webSite) {		
		Start-AppPool $webSite.ApplicationPool
		if ($webSite.State -ne $null -and $webSite.State -ne 'Started') {
			"Starting website '$websiteName'..."
			$webSite | Start-Website
		}		
	} else {
		throw "Website '$websiteName' not found."
	}
}

function Stop-WebsiteWithItsAppPool($websiteName) {
	Import-WebAdministrationModuleSafely

	$webSite = Get-Website | Where { $_.Name -eq "$websiteName" }
	if ($webSite) {
		if ($webSite.State -ne $null -and $webSite.State -ne 'Stopped') {
			"Stoping website '$websiteName'..."
			$webSite | Stop-Website
		}
		Stop-AppPool $webSite.ApplicationPool
	} else {
		throw "Website '$websiteName' not found."
	}
}

function Start-AppPool($appPoolName) {
	if ((Get-WebAppPoolState $appPoolName).Value -ne 'Started') { 
		"Starting AppPool '$appPoolName'..."
		Start-WebAppPool $appPoolName
		Wait-WebAppPoolStateChange $appPoolName 'Started'			
	}
}

function Stop-AppPool($appPoolName) {
	if ((Get-WebAppPoolState $appPoolName).Value -ne 'Stopped') {
		"Stoping AppPool '$appPoolName'..."
		Stop-WebAppPool $appPoolName
		Wait-WebAppPoolStateChange $appPoolName 'Stopped'	
	}
}

function Wait-WebAppPoolStateChange($appPoolName, $state) {
	Do {		
		$appPoolState = (Get-WebAppPoolState $appPoolName).Value;
		if ($appPoolState -ne $state) {
			"Waiting after '$state' of appPool '$appPoolName'..."
			Sleep -Milliseconds 250 
		}
	} While($appPoolState -ne $state)
}

function Start-WorldWideWebPublishingServiceIfNotRunning {
	$svc = Get-Service W3SVC
	if ($svc) {
		if ($svc.Status -ne "Running") {
			Write-Log ACTION "Starting 'World Wide Web Publishing (W3SVC)' Service... "
			Start-Service $svc.name
		}
	} else {
		throw "Unable to find the World Wide Web Publishing Service (W3SVC)."
	}
}

function Watch-ServiceStatus{
[CmdletBinding()]
param (
    [string]$ServiceName,
    [string]$Status,
    [int]$StartServiceTimeLimit
)
	$count=0
	while ((Get-Service $ServiceName).status -ne $Status)
	{
		if($count>$StartServiceTimeLimit)
		{
			throw [System.Exception] "Service named `"$($ServiceName)`" is not at status `"$($Status)`" after $($StartServiceTimeLimit) seconds." 
		}
		Write-Log ACTION "Wait until $ServiceName has this status: `"$($Status)`""
		sleep 1
		$count++		
	}

}

#<#
#.SYNOPSIS
#	Retrieves information about a file into a dictionary
#.DESCRIPTION
#	The Split-FileInformation will output a dictionary with the following members:
#	- fullName = Full path the of file as received.
#	- fileName = Only the filename part
#	- fileNameWithoutExtension = The filename without the extension
#	- hasExtension = If the filename contains an extension
#	- extension = The filename extension if present (including the . (dot))
#	- directoryName = The full path except the filename
#	- isPathRooted = If path is rooted
#	- pathRoot = The path root
#.PARAMETER Path
#    The filename to split information. 
#.EXAMPLE
#	Split-FileInformation -Path 'c:\temp\1 2\Deployment_Package1.zip'
#
#	 Will output:
#		Name                           Value                                                                                                                                                    
#		----                           -----                                                                                                                                                    
#		fullName                       c:\temp\1 2\Deployment_Package1.zip                                                                                                                     
#		fileNameWithoutExtension       Deployment_Package1                                                                                                                                     
#		hasExtension                   True                                                                                                                                                     
#		fileName                       Deployment_Package1.zip                                                                                                                                 
#		isPathRooted                   True                                                                                                                                                     
#		directoryName                  c:\temp\1 2                                                                                                                                              
#		extension                      .zip                                                                                                                                                     
#		pathRoot                       c:\  
##>
function Split-FileInformation{
[CmdletBinding()]
param(
	[Parameter(Mandatory=$true)][string]$Path
)
	@{
		fullName = $Path
		fileName = [System.IO.Path]::GetFileName($Path)
		fileNameWithoutExtension = [System.IO.Path]::GetFileNameWithoutExtension($Path)
		hasExtension = [System.IO.Path]::HasExtension($Path)
		extension = [System.IO.Path]::GetExtension($Path)
		directoryName = [System.IO.Path]::GetDirectoryName($Path)
		isPathRooted = [System.IO.Path]::IsPathRooted($Path)
		pathRoot = [System.IO.Path]::GetPathRoot($Path)
	}
}

function Test-FileLock {
param (
	[parameter(Mandatory=$true)][string]$Filepath
)
	$oFile = New-Object System.IO.FileInfo $Filepath
  	try
  	{
		$oStream = $oFile.Open([System.IO.FileMode]::Open, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
		if ($oStream)
		{
			$oStream.Close()
	  	}
	  	return $false	
	}
	catch
	{
		# file is locked by a process.
		return $true
	}
}

function Test-FileLockPath {
param (
	[parameter(Mandatory=$true)][string]$Path,
	[parameter(Mandatory=$true)][int]$Tries
)
	#test for file lock
	"Checking if files are still locked before deleting:"
	for ($i=1; $i -le $Tries; $i++)
	{
		$installedFiles = Get-ChildItem -Path $Path -Recurse -File
		$foundLockedFile = $false
		foreach ($file in $installedFiles) {
			if (Test-FileLock -Filepath $($file.FullName)) {
				Write-Log warning "File: '$($file.FullName)' is locked, waiting..."
				$foundLockedFile = $true
			}
		}
		if (!$foundLockedFile) { 
			break 
		} else { 
			Start-Sleep -Seconds (2 * $i)
		}
	}
}

function Get-WebInstallerAllInstalled {
	$webPI = Join-Path $env:ProgramFiles  "Microsoft\Web Platform Installer\WebpiCmd.exe"
	if (-not(Test-Path $webPI)) {
		Write-Log warning "Unable to find WebPICMD.exe at location: $webPI"
		$webPI = Join-Path $env:ProgramW6432  "Microsoft\Web Platform Installer\WebpiCmd.exe"
		Write-Log warning "Checking for alternate path in location: $webPI"
	}
	if (-not(Test-Path $webPI)) {
		throw "Unable to find WebPICMD.exe at location: " + $webPI
	}	
	$cmdOutput = & $webPI /List /ListOption:Installed
	return $cmdOutput | where { $_ -ne "" -and $_ -notmatch "^ "}
}

function Get-WebInstallerSoftwareMatch {
param (
	[parameter(Mandatory=$true)][string]$Name,
	$ListOutput = $null
)
	$itsThere = Get-Command "WebPICMD.exe" -ErrorAction SilentlyContinue
	if ($itsThere)
	{
		if ( -not $ListOutput) 
		{
			$foundItems = Get-WebInstallerAllInstalled | Select-String $Name
		}
		else
		{
			$foundItems = $ListOutput | Select-String $Name
		}
		if (-not $foundItems) { return $null }
		$filteredItems = $foundItems -replace '\s+', ' ' | foreach { $_.Split()[1..50] -join ' ' }
		return $filteredItems
	}
	else
	{
		Write-Log ERROR "Please Install 'Microsoft Web Platform Installer'"
		Write-Log ERROR "And make sure that its command line interface 'WebPICMD.exe' is available"
		Write-Log ERROR "Aborting!"
		throw "`n`nMissing pre-requisite: WebPICMD.exe"
	}
}

function Get-InstalledSoftwareMatch {
param (
	[parameter(Mandatory=$true)][string]$Name
)
	$keys     = Get-ChildItem HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\
    $items    = $keys | foreach-object {Get-ItemProperty $_.PsPath}
    $allItems = $items | select DisplayName, DisplayVersion, InstallDate, InstallLocation 
    $allItems | where {$_.DisplayName -match "$Name"}
}
 
function Test-InstalledSoftware {
param ( 
	[parameter(Mandatory=$true)][string]$Lookfor 
)
	Write-Log action "Checking for Pre-requisite software: '$Lookfor'"
	$installed = Get-InstalledSoftwareMatch -Name $Lookfor
	if (!$installed)
	{
		Write-Log ERROR "Failed, '$Lookfor' is not installed"
		Write-host ""
		throw "Missing pre-requisite: $Lookfor"
	}
}

function Test-W3svc {
	$w3svc = Get-Service W3SVC -ErrorAction SilentlyContinue
	if (!$w3svc)
	{
		Write-Log ERROR "Failed, Can't continue, IIS (W3SVC) is not installed on this computer."
		Write-host ""
		throw "Missing pre-requisite: IIS (W3SVC)"
	}
}

Function Install-Certificate {
param(
	[Parameter(Mandatory=$true)][string]$CertificateFilePath,
	[Parameter(Mandatory=$false)][string]$CertificatePassword,
	[Parameter(Mandatory=$false)][string]$CertificateThumbprint,
	[Parameter(Mandatory=$false)][string]$CertificateStoreLocation = "cert:\LocalMachine\TrustedPublisher\"
)
	$LastExitCode = $null

	if ($CertificateThumbprint) {
		$LocalStoreCertificatePath = Join-Path $CertificateStoreLocation $CertificateThumbprint
		if (Test-Path $LocalStoreCertificatePath) {
			Write-Log WARNING "Installing certificate skipped (already installed in $LocalStoreCertificatePath)..."
			"$CertificateFilePath"
			return
		}
	}

	$originalCertificatePath = $CertificateFilePath
	if (!(Test-AbsolutePath $CertificateFilePath)) {
		$CertificateFilePath = Join-Path $rootPath $CertificateFilePath
	}

	if (!(Test-Path $CertificateFilePath)) {
		$fileName = Split-Path $CertificateFilePath -Leaf
		$CertificateFilePath = Join-Path $rootPath "packages\specific\certificates\$($fileName)"		
		if (!(Test-Path $CertificateFilePath)) {
			$msg = "Unable to find certificate: '$originalCertificatePath'."
			if (!(Test-AbsolutePath $originalCertificatePath)) {
				$msg += " Relative path must start from the root of the deployment package folder (ex: 'packages\specific\certificates\MyCertificate.cer')."
			}
			throw  $msg
		}
	}
	
	Write-Log ACTION "Installing Certificate: '$CertificateFilePath'."
	if ([System.IO.Path]::GetExtension($CertificateFilePath) -eq ".cer") {
		Import-Certificate -FilePath $CertificateFilePath -CertStoreLocation $CertificateStoreLocation
	} else {
		$mypwd = ConvertTo-SecureString -AsPlainText -Force -String $CertificatePassword
		if (Get-Command "Import-PfxCertificate" -errorAction SilentlyContinue) {
			Import-PfxCertificate -FilePath $CertificateFilePath -CertStoreLocation $CertificateStoreLocation -Exportable -Password $mypwd
		} else {
			. certutil -f -p $CertificatePassword -importpfx $CertificateFilePath
		}
	}
		
	if ($LastExitCode) {
		throw "An error occured during cartificate installation."
	}
	$LastExitCode = $null
}

Function New-CompatibleNetFirewallRule {
param(
	[Parameter(Mandatory=$true)][string]$DisplayName,
	[Parameter(Mandatory=$true)][string]$Direction,
	[Parameter(Mandatory=$true)][string]$LocalPort,
	[Parameter(Mandatory=$true)][string]$Protocol
)
	# Detecting OS: netsh commands still works on 2012 for firewall but will be deprecated soon
	# to go forward and avoid deprecated error messages, this is what we do.
	$windowsVersion = (Get-WmiObject -class Win32_OperatingSystem).Caption
	if (-not ($windowsVersion -match "2008")) 
	{
		Import-Module NetSecurity -Force -ErrorAction SilentlyContinue
		New-NetFirewallRule -DisplayName $DisplayName -Direction $Direction -LocalPort $LocalPort -Protocol $($Protocol.ToUpper())
	}
	else
	{
		$netshDirection = "unknown"
		if ($Direction -eq "inbound") { $netshDirection = "in" }
		if ($Direction -eq "outbound") { $netshDirection = "out" }
		netsh advfirewall firewall add rule name="$DisplayName" dir=$netshDirection action=allow protocol=$($Protocol.ToUpper()) localport=$LocalPort profile=any
	}
}

Function Remove-CompatibleNetFirewallRule {
param(
	[Parameter(Mandatory=$true)][string]$DisplayName
)
	# Detecting OS: netsh commands still works on 2012 for firewall but will be deprecated soon
	# to go forward and avoid deprecated error messages, this is what we do.
	$windowsVersion = (Get-WmiObject -class Win32_OperatingSystem).Caption
	if (-not ($windowsVersion -match "2008")) 
	{		
		Import-Module NetSecurity -Force -ErrorAction SilentlyContinue
		Remove-NetFirewallRule -DisplayName "$DisplayName" -ErrorAction SilentlyContinue
	}
	else
	{
		netsh advfirewall firewall delete rule name="$DisplayName"
	}
}

Function Import-WebAdministrationModuleSafely {	
	try {
	    Get-WebSite | Out-Null
	} catch [System.IO.FileNotFoundException] {
		If (Get-module WebAdministration) {
			Remove-Module WebAdministration -Force -ErrorAction SilentlyContinue
		}		
		Import-Module WebAdministration -Force -ErrorAction SilentlyContinue
	    Get-WebSite | Out-Null
		"Safely imported Web-Administration module..."
	}
}

Function Stop-ServiceSynchronously {
param(
	[Parameter(Mandatory=$true)][String]$Name
)
	$service = Get-Service -Name $name -ErrorAction SilentlyContinue | Select Status

	if($service -and $service.Status -ne 'Stopped') { 
		Stop-Service -Name $name 
		while($service.Status -ne 'Stopped') {
			Write-Log ACTION "Waiting for service $name to stop..."
			Sleep -Milliseconds 500
			$service = Get-Service -Name $name -ErrorAction SilentlyContinue | Select Status
		}
		Write-Log ACTION "Service $name stopped."
	}
}

Function Test-WebAdministrationModuleAvailability {
	$env:PSModulePath -split ';' | % {
		if (Test-Path $_) {
			if (Get-ChildItem -Path $_ -Filter WebAdministration -Directory) {
				return $true
			}
		}
	}
	$false
}

function Invoke-RestMethodWithAuthenticationAndRetry{
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)][string]$Uri,
	[Parameter(Mandatory=$true)][string]$User,
	[Parameter(Mandatory=$true)][string]$Pass,
	[Parameter(Mandatory=$true)][int]$RetryCount,
	[Parameter(Mandatory=$true)][int]$RetryIntervalSeconds
)

	for ($i=1; $i -le $RetryCount; $i++){
		try{
			Write-Log ACTION "Retry-$i"			
			$response =  Invoke-RestMethodWithAuthentication -Uri $Uri -User $User -Pass $Pass
			return $response;
		}
		catch
		{
			if($i-eq $RetryCount)
			{
				throw $_
			}	
			sleep $RetryIntervalSeconds
		}
	}
}

function Invoke-RestMethodWithAuthentication{
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)][string]$Uri,
	[Parameter(Mandatory=$true)][string]$User,
	[Parameter(Mandatory=$true)][string]$Pass,
	[string]$Method = "Default",
	[string]$ContentType,
	[string]$Body
)
	$EncodedAuthorization = [System.Text.Encoding]::UTF8.GetBytes($user + ":" + $pass)
	$EncodedPassword = [System.Convert]::ToBase64String($EncodedAuthorization)
	$headers = @{"Authorization"="Basic $($EncodedPassword)"}
	
	Write-Log ACTION  "Invoking RestMethod User $user $Uri"			
	
	$response = "";	
	if ($ContentType -and $Body) { 
		$response = Invoke-RestMethod -Uri $uri -Method $Method -ContentType $ContentType -Body $Body
	}
	else{
		$response = Invoke-RestMethod -Uri $Uri -Method $Method -Headers $headers
	}
	return $response;
}

function Invoke-RestMethodWithRetry{
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)][string]$Uri,
	[Parameter(Mandatory=$true)][int]$RetryCount,
	[Parameter(Mandatory=$true)][int]$RetryIntervalSeconds
)
	for ($i=1; $i -le $RetryCount; $i++){
		try{
			Write-Log ACTION "Retry-$i Invoking RestMethod $Uri"
			$response = Invoke-RestMethod -Uri $Uri
			return $response;
		}
		catch
		{
			if($i-eq $RetryCount)
			{
				throw $_
			}	
			sleep $RetryIntervalSeconds
		}
	}
}

function Invoke-RestMethodWithHandling {
param (
    [Parameter(Mandatory=$true)][string]$Uri,
	[string]$Method = "Default",
	[string]$ContentType,
	[string]$Body
)
	try {
		$param = @($Uri, $Method)
		#TODO: Fix this (find a way to remove this strange conditional)
		if ($ContentType -and $Body) { 
			Invoke-RestMethod -Uri $uri -Method $Method -ContentType $ContentType -Body $Body
		} elseif ($ContentType) { 
			Invoke-RestMethod -Uri $uri -Method $Method -ContentType $ContentType
		} elseif ($Body) { 
			Invoke-RestMethod -Uri $uri -Method $Method -Body $Body
		} else {
			Invoke-RestMethod -Uri $uri -Method $Method
		}		
	} catch	{
		Write-Log ERROR "REST call failed: $Uri"
		Write-Log ERROR $_.ErrorDetails	
		throw $_
	}
}

Function Invoke-MSDeployUnpackage {
param(
	[Parameter(Mandatory=$true)][String]$MSDeployUnpackage,
	[Parameter(Mandatory=$true)][String]$DestinationPath,
	[String]$configuration,
	[switch]$DoNotDeleteExtraFiles
)
	"Unpackaging: $MSDeployPackageZipPath"
	"Into: $DestinationPath"
	
	if ((-not $DoNotDeleteExtraFiles) -and (Test-Path $DestinationPath)) { Remove-Item $DestinationPath -Recurse -Force }
	Invoke-UnzipFile $MSDeployPackageZipPath $DestinationPath
	Get-ChildItem $DestinationPath | where {!$_.PsIsContainer} | % { Remove-Item $_.FullName }	
	
	do {
		$subFolders = Get-ChildItem $DestinationPath | where {$_.PsIsContainer}
		$subFolders | % {			
			$subFolder = $_
			$subFolderItems = Get-ChildItem $subFolder.FullName
			"Flatting: $($subFolder.FullName)"
			
			# Select the configuration if more than one in the package
			if (($subFolder.Name -eq "obj") -and ($subFolderItems.count -gt 1)){				
				if (!$configuration) {
					Write-Log WARNING "The MSDeploy package contains more than one configuration ($subFolderItems) and no configuration has been specified."					
					if ($subFolderItems.name -contains "Release") { $configuration = "Release" }
					elseif ($subFolderItems.name -contains "Debug") { $configuration = "Debug" }
					else { throw "Unable to select the configuration. You must set the '-configuration' parameter." }
					Write-Log WARNING "The configuration '$configuration' has been selected..."
				}
				if ($subFolderItems.name -contains $configuration) {
					$toDelete = $subFolderItems | where { $_.Name -ne $configuration }
					$toDelete | % { Remove-Item $_.FullName -force -Recurse	}					
					$subFolderItems = $subFolderItems | where { $_.Name -eq $configuration }		
				} else { throw "Unable to found the configuration '$configuration' in the msdeploy package." }
			}

			# Move SubFolder ChildItems to temp folder				
			$tmpSubDir = $(Get-RandomTempFolderPath)
			if (Test-Path $tmpSubDir) { Remove-Item $tmpSubDir -Recurse -Force }			
			New-Item -ItemType Directory -Force -Path $tmpSubDir			
			$subFolderItems | % { 
				Move-Item $_.FullName $tmpSubDir -force 
			}
			Remove-Item $subFolder.FullName -force -Recurse
			
			# Bring back ChildItems to Destination
			Get-ChildItem $tmpSubDir | % { 
				Move-Item $_.FullName $DestinationPath -force 
			}
			
			# Over if subFolder is PackageTmp
			if ($subFolder.Name -eq "PackageTmp") { break }		
		}
	} while ($subFolders -ne $null)
	
	"MSDeploy Extraction completed!"
}

function Get-RandomTempFolderPath {
	$temp  = $env:TEMP.Split(';')[0]
	$randomPath = Join-Path $temp $([guid]::NewGuid().ToString().Substring(0, 8))
	New-Item -ItemType directory -Path $randomPath -Force -ErrorAction SilentlyContinue | out-null
	return $randomPath
}

function Test-AbsolutePath {
Param (
    [Parameter(Mandatory=$True)][String]$Path
)
	return [System.IO.Path]::IsPathRooted($Path)
}

function Test-RemoteMachine {
param(
	[string]$server
)
	$dnsDomain      = $env:USERDNSDOMAIN
	$computerName   = $env:COMPUTERNAME
	$fullNameDns    = "${computerName}.${dnsDomain}"
	if ($server -eq "localhost")	{ return $false }
	if ($server -eq $computerName) { return $false }
	if ($server -eq $fullNameDns)  { return $false }
	if ($server -eq "127.0.0.1")   { return $false }
	return $true
}

function Test-NetworkPath {
param(
	[string]$path
)
	$driveName = split-path $path -qualifier -erroraction silentlycontinue
	if (![string]::IsNullOrWhiteSpace($driveName)) {
		$drive = new-object system.io.driveinfo($driveName)	
		return ($drive.drivetype -eq "Network")
	}	
	return $true
}

function Get-IISSitePhysicalPath {
param(
	[Parameter(Mandatory=$true)]$IisAppName
)
	Import-WebAdministrationModuleSafely | out-null
	$website = Get-Website | ?{ $_.name -eq $IisAppName }
	return $website.PhysicalPath
}

function IsWindowsServer2012OrLater {
    [Environment]::OSVersion.Version.Major -ge 6 -and [Environment]::OSVersion.Version.Minor -ge 2
}

function Get-PathFromAbsoluteOrRelativePath {
Param (
    [string]$AbsoluteOrRelativePath,
	[string]$PackageRootRelativePath
)
	if (Test-Path $AbsoluteOrRelativePath) { 
		return (Get-Item $AbsoluteOrRelativePath).Fullname 
	}
	
	$filePath = Join-Path $rootPath $AbsoluteOrRelativePath
	if (Test-Path $filePath) { return $filePath;}
	
	$fileName = Split-Path $AbsoluteOrRelativePath -Leaf
	$filePath = Join-Path $rootPath $PackageRootRelativePath
	$filePath = Join-Path $filePath $fileName
	if (Test-Path $filePath) 
	{ 
		if ($AbsoluteOrRelativePath.Split("\").Count -gt 1) { Write-Log WARNING "Path '$AbsoluteOrRelativePath' was auto corrected to '$filePath', please review your parameters file." }
		return $filePath;
	}

	$msg = "Unable to find file: '$AbsoluteOrRelativePath'."
	$msg += "Path '$AbsoluteOrRelativePath' must be valid and one of these 3 options : be relative to '$PackageRootRelativePath', be relative to the root of the deployment package folder (ex: 'packages\specific\certificates\MyCertificate.cer'), be absolute."
	throw  $msg
}