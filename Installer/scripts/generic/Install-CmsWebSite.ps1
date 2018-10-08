param(
	[Parameter(Mandatory=$true)]
	[string]
	$Environment
	,
	[string]
	$Component = ""
	,
	[Parameter(Mandatory=$true)]
	[ValidateSet("cm","cd")]
	[string]
	$Configuration
)

#=======================================
# Initialize Script Execution Context
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path
$scriptsPath = (get-item $currentScriptPath).parent.FullName
. $scriptsPath\core\Initialize-Context.ps1
#=======================================
$scriptsPath			= (get-item $currentScriptPath).parent.FullName
$coreScriptPath 		= Join-Path $scriptsPath "core"
$rootPath				= (get-item $scriptsPath).parent.FullName
$configsPath			= Join-Path $rootPath "configs"
$packageCertificatePath	= Join-Path $rootPath "packages\specific\certificates"
$packagesSpecificRelativePath = ".\packages\specific\certificates"
#---------------------------------------
$cmsWebSiteName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSiteName"
$cmsHostName			= Get-Settings -environment $Environment -key "cms-$($Configuration)-hostname"
$cmsHostPhysicalPath	= Get-Settings -environment $Environment -key "cms-$($Configuration)-IISSitePhysicalPath"

$cmsAppPoolName			= Get-Settings -environment $Environment -key "cms-apppool-name"
$cmsAppPoolIdentityType	= Get-AppPoolProcessModelIdentityType (Get-Settings -environment $Environment -key "cms-apppool-identity-type") 
$cmsAppPoolUserName		= Get-AppPoolUserName $cmsAppPoolIdentityType $cmsWebSiteName (Get-Settings -environment $Environment -key "cms-apppool-username")
$cmsAppPoolPassword		= Get-Settings -environment $Environment -key "cms-apppool-password"	

$sslCertificatePath		= Get-Settings -environment $Environment -key "SSL_CertificatePath"
$sslCertificatePath		= Get-PathFromAbsoluteOrRelativePath -AbsoluteOrRelativePath $sslCertificatePath -PackageRootRelativePath $packagesSpecificRelativePath
$sslCertificatePfxPassword	= Get-Settings -environment $Environment -key "SSL_CertificatePfxPassword"
$sslCertificateThumbprint	= Get-Settings -environment $Environment -key "SSL_CertificateThumbprint"
#=======================================

# Error handling for W3SVC and admin module
Test-W3svc

# Set HostFile hostname
Write-Log ACTION "Adding '$cmsHostName' to host file..."
Add-HostFileEntry $cmsHostName

# Ensure destination folder
if ((Test-Path $cmsHostPhysicalPath) -eq $false) { 
	Write-Log ACTION "Creating folder: '$cmsHostPhysicalPath'"
	New-Item $cmsHostPhysicalPath -ItemType Directory | out-null
}
if($cmsAppPoolIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::SpecificUser){
	Grant-FolderPermission -Username $cmsAppPoolUserName -Path $cmsHostPhysicalPath -Right FullControl -EnsureUserExits $true -UserPassword $cmsAppPoolPassword
}

# Install the certificates
Install-Certificate -CertificateFilePath $sslCertificatePath -CertificatePassword $sslCertificatePfxPassword -CertificateThumbprint $sslCertificateThumbprint
Install-Certificate -CertificateFilePath $sslCertificatePath -CertificatePassword $sslCertificatePfxPassword -CertificateThumbprint $sslCertificateThumbprint -CertificateStoreLocation "cert:\LocalMachine\My\"

# Since Install-Certificate does nothing if Cert is already installed, we look for it where-ever it is installed and use it
$foundCert = [string[]](get-childitem -Path cert:\LocalMachine\ -Recurse | ? {$_.Thumbprint -eq $sslCertificateThumbprint} | % {$_.PSPath})
$foundCert = $foundCert | % {$_.Replace("Microsoft.PowerShell.Security\Certificate::","cert:\")} 

Grant-CertificatePermission -certificatePath $foundCert


# Install IIS sites
  $bindings = @(
			@{ protocol = "http"; bindingInformation="*:80:$cmsHostName" },
			@{ protocol = "https"; bindingInformation="*:443:$cmsHostName" }
	);

Write-Log ACTION "Creating IIS WebSite..."
. $coreScriptPath\Add-IISSite.ps1 -siteDefinition @{
	webSiteName = "$cmsWebSiteName";
	appPoolName = "$cmsAppPoolName";
	appPoolUserName = "$cmsAppPoolUserName";
	appPoolPassword = "$cmsAppPoolPassword";
	appPoolIdentityType = "$cmsAppPoolIdentityType";
	loadUserProfile = $true;
	pingingEnabled = $true;
	anonymousAuthentication = @{ 		
		enabled = $true;
		username = "";
	};
	bindings = @($bindings);
	physicalPath = "$cmsHostPhysicalPath\WebSite";
	certificatePath = $foundCert[0];
}

Write-Log ACTION "Granting access to site folder '$cmsHostPhysicalPath'"
Grant-FolderPermission -Username $cmsAppPoolUserName -Path $cmsHostPhysicalPath -Right Read,Write,Modify -EnsureUserExits $true -UserPassword $cmsAppPoolPassword