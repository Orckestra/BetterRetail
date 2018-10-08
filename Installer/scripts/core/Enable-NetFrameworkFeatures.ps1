Import-Module ServerManager
$requiredFeatures = @(
	'AS-NET-Framework'
	'AS-Web-Support'
	'AS-TCP-Port-Sharing'
	'AS-WAS-Support'
	'AS-HTTP-Activation'
	'AS-TCP-Activation'
	
	'Web-WebServer'
	'Web-Asp-Net45'
	'Web-Net-Ext45'
	'Web-Health'
	'Web-Basic-Auth'
	'Web-Windows-Auth'
	'Web-Filtering'
	'Web-IP-Security'
	'Web-AppInit'
	
	'Web-Mgmt-Tools'
	'Web-Mgmt-Console'
	'Web-Scripting-Tools'
	'Web-Mgmt-Service'
	
	'WAS'
	
	'NET-Framework-45-Features'
	'NET-Framework-45-Core'
	'NET-WCF-Services45'
	'NET-Framework-45-ASPNET'	
)


try {
	Write-Log ACTION "Enabling .Net Framework Features..."
	
	$featuresToInstall = $requiredFeatures | Get-WindowsFeature
	if($featuresToInstall) {
		# filter on 2012 since the data is available
		if ($featuresToInstall[0].InstallState) {
			$featuresToInstall = $featuresToInstall | Where InstallState -Eq 'Available'
		}
		
        ($featuresToInstall | Select -ExpandProperty DisplayName)
		
		foreach($s in $featuresToInstall) 
        {
	        $result = Add-WindowsFeature $s.Name

            if(-not ($result -and $result.Success)) {
			    throw "$($s.Name)"
		    }
		    else {
			    Write-Log ACTION "Required windows feature $($s.Name) install was successful."
		    }
        }	
	}
	else {
		Write-Log WARNING "Skipped (all features where already installed)..."
	}
}
catch {
	Write-Error "Failed to install required windows features $_" 
}
