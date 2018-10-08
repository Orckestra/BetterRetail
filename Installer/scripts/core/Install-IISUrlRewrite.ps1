param(
	[Parameter(Mandatory=$true)][string]$Environment,
	[Parameter(Mandatory=$true)][string]$Component
)

#=======================================
# Initialize Script Execution Context
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path
$scriptsPath = (get-item $currentScriptPath).parent.FullName
. $scriptsPath\core\Initialize-Context.ps1
#=======================================

function Is64Bit {        
	[IntPtr]::Size -eq 8  
}  

function Install-IISRewriteModule {  
	$wc = New-Object System.Net.WebClient  	  
	$dest = Join-Path $(Get-RandomTempFolderPath) "IISRewrite.msi"
	if (Is64Bit){  
		$url = "http://go.microsoft.com/?linkid=9722532"  
	} else{  
		$url = "http://go.microsoft.com/?linkid=9722533"       
	}		
	
	Write-Log INFO "Downloading: $url"
	$wc.DownloadFile($url, $dest)  

	Write-Log INFO "Executing: $dest"
	msiexec.exe /i $dest /passive
}  

Write-Log ACTION "Installing IIS Url Rewrite Module 2..."
if (!(Test-Path "$env:programfiles\Reference Assemblies\Microsoft\IIS\Microsoft.Web.Iis.Rewrite.dll")){  
	Install-IISRewriteModule  
} else {
	Write-Log WARNING "IIS Rewrite Module - Already Installed..."
}