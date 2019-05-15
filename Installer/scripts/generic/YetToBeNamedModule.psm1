
function CreateDirectoryStructureIfNotExists() {
<#
	.SYNOPSIS
		Create the entire folder structure from a given abolute path to a folder or file

	.DESCRIPTION
		Create the entire folder structure from a given abolute path to a folder or file, 
		will create the missing part(s), if file is specific in the path, it's folder will be used

	.PARAMETER  fullPathToFolderOrFile
		The absolute folder path to be created if it does not already exit.
	
	.EXAMPLE
		CreateDirectoryStructureIfNotExists "c:\tmp\data\backups\"
		
		Will create all missing folder in structure
		
	.EXAMPLE
		CreateDirectoryStructureIfNotExists "c:\tmp\data\backups\db.bak"
		
		Will create all missing folder in structure, filename is ignored.
#>
	[CmdletBinding()]
	param(
		[ValidateScript({[System.IO.Path]::IsPathRooted($_)})]
		[string]
		$fullPathToFolderOrFile
	)
	$fullPathToFolded = (Split-Path $fullPathToFolderOrFile -Parent)
	if(!(Test-Path -Path $fullPathToFolded )){
		New-Item -ItemType directory -Path $fullPathToFolded
	}	
}

function DownloadC1Package($packagename, $c1version, $outputRoot) {

	$output = $outputRoot + "/" + $packagename + ".zip"

	if (Test-Path $output) {
		Remove-Item $output -recurse -force
	} 
	Write-Log ACTION "Downloading package $packagename ..." 
	$url = "https://package.composite.net/Download.ashx?package=$packagename&c1version=$c1version"
	
	try { Invoke-WebRequest -Uri $url -OutFile $output } catch { 
		Write-Host ERROR "Error downloading $packagename"
	}
	

}

function ExecuteMsSqlQuery() {
<#
	.SYNOPSIS
		Executes an TSQL Query using SQLCMD
		
	.DESCRIPTION
		Executes an TSQL Query using SQLCMD, it must be installed or an explicit error will inform you that it was not found at the expected path.
		The following variables must be present in the final Parameter.xml :
			"DB-SqlCmdExePath"
			"DB-SqlCmdExeUser"
			"DB-SqlCmdExePassword"
			"DB-SqlCmdExeUseIntegratedSecurity"

	.PARAMETER  server
		The SQL Server name or IP to execute the query on.
	
	.PARAMETER  query
		The TSQL Query to execute	
	
	.PARAMETER  Environment
		The environment where the deployement is executed
		
	.EXAMPLE
		ExecuteMsSqlQuery "127.0.0.1" "SELECT 'hello'" DEV
		
	.EXAMPLE
		$query = 
			"IF OBJECT_ID('dbo.Scores', 'U') IS NOT NULL" +
			"DROP TABLE dbo.Scores"
		ExecuteMsSqlQuery -server "s1020dv10" -query $query -Environment STG
#>
	[CmdletBinding()]
	param(
		[Parameter(Mandatory=$true)]
		[string]
		$server
		,
		[Parameter(Mandatory=$true)]
		[string]
		$query	
		,
		[Parameter(Mandatory=$true)]
		[string]
		$Environment	
	)
	
	$sqlCmdExe = Get-Settings -environment $Environment -key "DB-SqlCmdExePath"
	
	if (!(Test-Path($sqlCmdExe))) { 
		Write-Error "SQL standalone client (sqlcmd.exe) is not detected, please install the latest version (sqlncli.msi and SqlCmdLnUtils.msi) or correct 'DB-SqlCmdExePath' parameter value."
	}

	$sqlCmdExeUser = Get-Settings -environment $Environment -key "DB-SqlCmdExeUser"
	$sqlCmdExePassword = Get-Settings -environment $Environment -key "DB-SqlCmdExePassword"
	$sqlCmdExeUseIntegratedSecurity = [System.Convert]::ToBoolean((Get-Settings -environment $Environment -key "DB-SqlCmdExeUseIntegratedSecurity"))

	if($sqlCmdExeUseIntegratedSecurity) {
		$sqlCmdExeUser = $null
		$sqlCmdExePassword = $null
	}else{
		if($sqlCmdExeUser -xor $sqlCmdExePassword) {
			Write-Error "If not using Integrated Security ('DB-SqlCmdExeUseIntegratedSecurity' = 'true'), you must specify both the SQL Username ('DB-SqlCmdExeUser') and SQL Password ('DB-SqlCmdExePassword') to use to execute the query"
		}		
	}
	
	if ($sqlCmdExeUseIntegratedSecurity) {		
		$args = @("-S","$server","-Q","$query","-b","-E")
	} else {			
		$args = @("-S","$server","-Q","$query","-b","-U","$sqlCmdExeUser","-P","$sqlCmdExePassword")
	}

	$result = & "$sqlCmdExe" $args		
	if($result -is [array]) {
		$result | % {
			if($LastExitCode){
				Write-Error "$_"
			}else{
				if(![string]::IsNullOrEmpty("$_")){
					Write-Log INFO "$_"
				}
			}
		}
	} else {
		if($LastExitCode){
			Write-Error $result
		}elseif(-not [string]::IsNullOrEmpty($result)){
			Write-Log ACTION -Message $result
		}else{
			Write-Log ACTION -Message "Completed succesfuly"
		}
	}
}

function Resolve-AbsolutePath{
<# 
.SYNOPSIS 
	Returns the absolute path of a folder of file receieved as parameter (absolute or relative) wether it exists or not on disk.
 
.DESCRIPTION 
	Returns the absolute path version of a folder of file receieved as parameter wether it exists or not on disk.
	
	Unlike bult-in Powershell functions, this one does not try to validate against on-disk files or folder.
	It can be used to generate absolute path for non-existing files (to create them or other purposes...)
	
	The input path can contain ".", "..", "..\..", etc. meta-folder and will be resolved correctly.
 
.PARAMETER Path 
    The path tat should be resolved as an absolute path. it can contain ".", "..", "..\..", etc. meta-folder and will be resolved correctly.
   
.EXAMPLE 
	Resolve-AbsolutePath -Path "C:\windows\system32\drivers\..\.."

	Will return "C:\Windows"
 
.EXAMPLE 
	Resolve-AbsolutePath -Path "C:\windows\system32\"

	Will return "C:\Windows\system32"
 
.EXAMPLE 
	Resolve-AbsolutePath -Path "drivers"

	Will return "C:\windows\system32\drivers", assuming we were in th folder "C:\Windows\system32" when the call was made

.EXAMPLE 
	Resolve-AbsolutePath -Path "./drivers"
   
	Will return "C:\windows\system32\drivers", assuming we were in th folder "C:\Windows\system32" when the call was made
 #> 
[CmdletBinding()]
 param(
	[string]
	$Path
)`
	return $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Path)
}

function Invoke-MSDeployContentToPackage(){
[cmdletbinding()]
param(
	[string]
	$MsDeployExe = "C:\Program Files (x86)\IIS\Microsoft Web Deploy v3\msdeploy.exe"
	,
	[parameter(mandatory=$true)]
	[string]
	$ContentPath
	,
	[parameter(mandatory=$true)]
	[string]
	$PackageFile	
	,
	[switch]
	$DoNotDeleteExtraFiles
	,
	[string]
	$ExcludePath = $null
)
	$fullContentPath = Resolve-AbsolutePath "$ContentPath"
	$fullPackageFile = Resolve-AbsolutePath "$PackageFile"
	
	Write-Log $fullPackageFile
	Write-Log $fullContentPath
	
	$MsDeployArguments = @("-verb:sync", "-source:contentpath=$fullContentPath", "-dest:package=$fullPackageFile")
	
	if($DoNotDeleteExtraFiles){
		$MsDeployArguments += "-enableRule:DoNotDeleteRule"
	}
	if($ExcludePath -ne $null){
		$absolutePathToExclude = [System.Text.RegularExpressions.Regex]::Escape("$fullContentPath\$ExcludePath")
		$MsDeployArguments += "-skip:objectName=contentPath,absolutePath=$absolutePathToExclude"
	}

	& $MSDeployExe $MsDeployArguments
}

function Invoke-MSDeployPackageToContent(){
[cmdletbinding()]
param(
	[string]
	$MsDeployExe = "C:\Program Files (x86)\IIS\Microsoft Web Deploy v3\msdeploy.exe"
	,
	[parameter(mandatory=$true)]
	[string]
	$PackageFile	
	,
	[parameter(mandatory=$true)]
	[string]
	$ContentPath
	,
	[switch]
	$DoNotDeleteExtraFiles	
)

	$fullPackageFile = Resolve-AbsolutePath "$PackageFile"
	$fullContentPath = Resolve-AbsolutePath "$ContentPath"
	
	Write-Log $fullPackageFile
	Write-Log $fullContentPath
	
	$MsDeployArguments = @("-verb:sync", "-source:package=$fullPackageFile", "-dest:contentpath=$fullContentPath")
	
	if($DoNotDeleteExtraFiles){
		$MsDeployArguments += "-enableRule:DoNotDeleteRule"
	}
	& $MSDeployExe $MsDeployArguments
}