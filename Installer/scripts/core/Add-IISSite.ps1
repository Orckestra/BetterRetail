param(
	[parameter(Mandatory=$true, ValueFromPipeline=$true)]
	$siteDefinition
)
	
$inetsrvPath = "{0}\system32\inetsrv\" -f ${env:windir}
[System.Reflection.Assembly]::LoadFrom( $inetsrvPath + "Microsoft.Web.Administration.dll" )
[System.Reflection.Assembly]::LoadFrom( $inetsrvPath + "Microsoft.Web.Management.dll" )
$computer = $env:computername
$serverManager = [Microsoft.Web.Administration.ServerManager]::OpenRemote($computer.ToLower())

Filter Children {
	if ($_.children) 
	{ 
		$_.children | Children
		$_.children
	}
}

Filter ConfigureIISAnonymousAuthentication {
	if ($_.anonymousAuthentication)
	{
		$config = $serverManager.GetApplicationHostConfiguration()
		$anonymousAuthenticationSection = $config.GetSection("system.webServer/security/authentication/anonymousAuthentication", $_.webSiteName)
		if ($_.anonymousAuthentication.enabled) {
			$anonymousAuthenticationSection["enabled"] = $true
			$anonymousAuthenticationSection["username"] = $_.anonymousAuthentication.username
			$anonymousAuthenticationSection["password"] = $_.anonymousAuthentication.password
		} else {
			$anonymousAuthenticationSection["enabled"] = $false
		}
	}
}

Filter SetChildrenPhysicalPath {
	if ($_.children) 
	{ 
		$parentPath = $_.physicalPath
		$_.children | % { 
			if ($_.physicalPath -match '{PARENT}\\') {
				$_.physicalPath = join-path $parentPath ($_.physicalPath -replace '{PARENT}\\', '')
			}
		}
		$_.children | SetChildrenPhysicalPath
		$_.children
	}
}

############ AppPool
$appPool = $serverManager.ApplicationPools[$siteDefinition.appPoolName]

if($appPool) {
	Write-Log WARNING ("Removing existing '{0}' AppPool..." -f $appPool.Name)
	$serverManager.ApplicationPools.Remove($appPool)
}

#Identity types for process model
 
$parsedProcessModelIdentityType = [Microsoft.Web.Administration.ProcessModelIdentityType]::ApplicationPoolIdentity;

if([Microsoft.Web.Administration.ProcessModelIdentityType]::TryParse($siteDefinition.appPoolIdentityType,[ref] $parsedProcessModelIdentityType) -eq $false){
	$exceptionMessage="Unable to parse application pool ProcessModelIdentityType. Acceptable values are: LocalSystem,LocalService, NetworkService,SpecificUser or ApplicationPoolIdentity";
	throw $exceptionMessage;
}


$siteDefinition.appPoolIdentityType
Write-Log ACTION ("Creating '{0}' AppPool..." -f $siteDefinition.appPoolName)
$appPool = $serverManager.ApplicationPools.Add($siteDefinition.appPoolName)
$appPool.processModel.identityType = $siteDefinition.appPoolIdentityType
$appPool.managedRuntimeVersion = "v4.0" 

if($siteDefinition.appPoolIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::SpecificUser){

	Write-Log ACTION ("Granting LogonAsABatchJob to appool user {0}" -f $siteDefinition.appPoolUserName)
	. $coreScriptPath\Add-AccountToLogonLocally.ps1 -AccountUsername $siteDefinition.appPoolUserName -Right 'LogonAsABatchJob'
	
	$appPool.processModel.userName = $siteDefinition.appPoolUserName
	$appPool.processModel.password = $siteDefinition.appPoolPassword	
}



# load user profile if setting is not defined, otherwise take setting value+
if ($siteDefinition.loadUserProfile -eq $null)
{ $appPool.processModel.loadUserProfile = $true }
else
{ $appPool.processModel.loadUserProfile = $siteDefinition.loadUserProfile }

# Improve debugging (no timeout)
# pingingEnabled if setting is defined, otherwise leave default IIS setting
if ($siteDefinition.pingingEnabled -ne $null) { $appPool.processModel.pingingEnabled = $siteDefinition.pingingEnabled }

############ Website
$webSite = $serverManager.Sites[$siteDefinition.webSiteName]
if($webSite) {
	Write-Log WARNING ("Removing existing '{0}' website ..." -f $webSite.Name)
	$serverManager.Sites.Remove($webSite)
	$serverManager.CommitChanges()
}
New-Item $siteDefinition.physicalPath -ItemType Directory -Force | out-null
$siteDefinition.physicalPath = Get-Item $siteDefinition.physicalPath
Write-Log ACTION ("Creating '{0}' website..." -f $siteDefinition.webSiteName)
$webSite = $serverManager.Sites.Add($siteDefinition.webSiteName, $siteDefinition.physicalPath, 80)
$webSite.Bindings.Clear()
$siteDefinition.bindings | % {
	$protocol = $_.protocol
	Write-Log ACTION ("Creating '$protocol' binding: {0}" -f $_.bindingInformation)
	$binding = $webSite.Bindings.CreateElement("binding");
	$binding["protocol"] = $protocol;
	$binding["bindingInformation"] = $_.bindingInformation
	if($protocol -eq 'https') {	
		if (Test-Path $siteDefinition.certificatePath) {
			$cert =  Get-Item $siteDefinition.certificatePath
			$binding["certificateHash"] = $cert.Thumbprint.ToString();
			$binding["certificateStoreName"] = (Split-Path  $cert.PSParentPath -leaf).ToString()
			#$binding["sslFlags"] = 1
		} else {
			throw "Unable to find certificate '$certificateSubject' for HTTPS binding."
		}
	}

	$webSite.Bindings.Add($binding)
}
if($siteDefinition.enabledProtocols) { 
	$webSite.ApplicationDefaults.EnabledProtocols = $siteDefinition.enabledProtocols 
}
$siteDefinition | ConfigureIISAnonymousAuthentication

# process children
if ($siteDefinition.children)
{
	$siteDefinition | SetChildrenPhysicalPath
	$siteDefinition | Children | Sort-Object | % {
		New-Item $_.physicalPath -ItemType Directory -Force
		$_.physicalPath = Get-Item $_.physicalPath
		# ensure here is a slash at the beginning
		$_.name = $_.name -replace '^(/)?(.*)', '/$2'
		if($_.childtype -ieq "application"){
			Write-Log ACTION ("Creating application '{0}'..." -f $_.name)
			$application = $webSite.Applications.Add($_.name, $_.physicalPath)
			$application.ApplicationPoolName = $appPool.name
			if ($_.enabledProtocols) { $application.EnabledProtocols = $_.enabledProtocols }
			$_ | ConfigureIISAnonymousAuthentication
		}
		else {	#expected virtual directory
			Write-Log ACTION ("Creating virtual directory '{0}'..." -f $_.name)
						
			#  Beginning of line or string
			#  [1]: A numbered capture group. [[/]?.*], zero or one repetitions
			#      [/]?.*
			#          Any character in this class: [/], zero or one repetitions
			#          Any character, any number of repetitions
			#  Match expression but don't capture it. [(?:/).*]
			#      (?:/).*
			#          Match expression but don't capture it. [/]
			#              /
			#          Any character, any number of repetitions
			#  End of line or string
			#
			### grab the virtual directory parent part from the name
			$parent = $_.name -replace '^([/]?.*)?(?:(?:/).*)$', '$1'
			if (!($parent)) { $parent = '/' }
			
			#  ^.*
			#      Beginning of line or string
			#      Any character, any number of repetitions
			#  [1]: A numbered capture group. [/.*]
			#      /.*
			#          /
			#          Any character, any number of repetitions
			#  End of line or string				
			### grab the virtual directory name part from the name property
			$name = $_.name -replace '^.*(/.*)$', '$1'

			$parent = $webSite.Applications[$parent]
			$parent.VirtualDirectories.Add($name, $_.physicalPath) | Out-Null
		}
	}		
}

$webSite.ServerAutoStart = $true;
$webSite.ApplicationDefaults.ApplicationPoolName = $appPool.Name

$serverManager.CommitChanges()
$serverManager.Dispose()
