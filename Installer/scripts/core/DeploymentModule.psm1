#requires -version 3.0
$rootPath = (gi (Split-Path $MyInvocation.MyCommand.Path)).parent.parent.FullName
$configurationHashMap = @{}

function Get-AppPoolProcessModelIdentityType{
	param([Parameter(Mandatory=$True)][string]$identityType)
	
	$_empty = LoadAssemblyIfNeeded

	$parsedProcessModelIdentityType = [Microsoft.Web.Administration.ProcessModelIdentityType]::ApplicationPoolIdentity;

	if([Microsoft.Web.Administration.ProcessModelIdentityType]::TryParse($identityType,[ref] $parsedProcessModelIdentityType) -eq $false){
		$exceptionMessage="Unable to parse application pool $identityType ProcessModelIdentityType. Acceptable values are: LocalSystem,LocalService, NetworkService,SpecificUser or ApplicationPoolIdentity";
		throw $exceptionMessage;
	}

	Write-Log INFO "AppPool identity type used will be : $parsedProcessModelIdentityType";
	return $parsedProcessModelIdentityType;
}

function Get-AppPoolUserName{
	param(
	[Parameter(Mandatory=$True)][Microsoft.Web.Administration.ProcessModelIdentityType]$appPoolProcessModelIdentityType,
	[Parameter(Mandatory=$True)][string]$siteName,
	[Parameter(Mandatory=$True)][string]$configuredAppPoolUserName)

	$_empty = LoadAssemblyIfNeeded

	if($appPoolProcessModelIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::SpecificUser){
		Write-Log INFO "AppPool user name used will be : $configuredAppPoolUserName";
		return $configuredAppPoolUserName;		
	}
	if($appPoolProcessModelIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::LocalSystem -or
		$appPoolProcessModelIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::LocalService -or
		$appPoolProcessModelIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::NetworkService){
		Write-Log INFO "AppPool user name used will be : $configuredAppPoolUserName";
		return $configuredAppPoolUserName;		
	}

	if($appPoolProcessModelIdentityType -eq [Microsoft.Web.Administration.ProcessModelIdentityType]::ApplicationPoolIdentity){
		Write-Log INFO "AppPool user name used will be : IIS APPPOOL\$siteName";
		return "IIS APPPOOL\$siteName";
	}
	throw "Unsupported application pool identity type for Overture installation: $appPoolProcessModelIdentityType"
}

function Get-DeploymentFolderPath {
param(
	[string]$DeploymentToken = $currentDeploymentToken
)
	return Join-Path $rootPath "_deployments\$deploymentToken"
}

function Get-BackupFolderPath {
param(
	[string]$DeploymentToken = $currentDeploymentToken
)
	return Join-Path (Get-DeploymentFolderPath $deploymentToken) "backups"
}

function Get-ParametersFilePath {
param(
	[string]$DeploymentToken = $currentDeploymentToken
)
	return Join-Path (Get-DeploymentFolderPath $deploymentToken) "parameters.xml"
}

function Get-LogFilePath {
param(
	[string]$DeploymentToken = $currentDeploymentToken
)
	return Join-Path (Get-DeploymentFolderPath $deploymentToken) "Installation.log"
}

function Invoke-EnvironmentPipeline {
param(
	[string]$environment,
	[string]$pipeline,
	[PSCredential]$Credentials = $null
)
	$topologyElement = Get-TopologyElement -environment $environment
	$topologyElement.server | foreach {		
		Invoke-ServerPipeline -environment $environment -server $_.alias -pipeline $pipeline -Credentials $Credentials
		
		Write-Log ACTION "DONE! Installation completed" 
	}
}

function Invoke-ServerPipeline {
param(
	[string]$environment,
	[string]$server,
	[string]$pipeline,
	[bool]$locally = $false,
	[PSCredential]$Credentials = $null
)
	Write-Host ""
	Write-Log INFO "==========================================================="
	Write-Log INFO "Invoking '$pipeline' on server '$server' ($environment)"
	Write-Log INFO "==========================================================="
	Write-Host ""
	
	$serverElement = Get-InfrastructureServerElement -environment $environment -server $server	
	if (!$locally) {
		$serverType = $($serverElement.LocalName)
		switch ($serverType) {
			onPremiseMachine {
				if (Test-RemoteMachine $serverElement.machineName) {
					Invoke-ServerPipelineRemotely -environment $environment -server $server -pipeline $pipeline -Credentials $Credentials
				} else {				
					Invoke-ServerPipelineLocally -environment $environment -server $server -pipeline $pipeline
				}
			}
			azureWorkerRole {
				Invoke-ServerPipelineOnAzureWorkerRole -environment $environment -server $server -pipeline $pipeline -serverElement $serverElement 
			}
			azureVirtualMachine {
				Invoke-ServerPipelineOnAzureVM -environment $environment -server $server -pipeline $pipeline
			}
			onPremiseDatabaseServer {
				$databaseServerName = $serverElement.dbServerName
				if ([string]::IsNullOrWhiteSpace($databaseServerName)) {
					throw "Unable to find the attribute 'dbServerName' from element <onPremiseDatabaseServer>."
				}
				$parameters = "-dbServerName $databaseServerName"
				Invoke-ServerPipelineLocally -environment $environment -server $server -pipeline $pipeline -parameters $parameters
			}
			azureDatabaseServer {
				$databaseServerName = $serverElement.dbServerName
				if ([string]::IsNullOrWhiteSpace($databaseServerName)) {
					throw "Unable to find the attribute 'dbServerName' from element <azureDatabaseServer>."
				}
				if (!$databaseServerName.EndsWith(".database.windows.net")) {
					$databaseServerName += ".database.windows.net"
				}
				$parameters = "-dbServerName $databaseServerName"
				Invoke-ServerPipelineLocally -environment $environment -server $server -pipeline $pipeline -parameters $parameters -retryCount 3
			}
			default {
				throw "Unrecognised server type: '$serverType'."
			}
		}
	} else {
		Invoke-ServerPipelineLocally -environment $environment -server $server -pipeline $pipeline
	}
}
			
function Invoke-ComponentPipeline {
param(
	[string]$environment,
	[string]$server,
	[string]$component,
	[string]$pipeline,
	[bool]$locally = $false,
	[PSCredential]$Credentials = $null
)		
	if (![string]::IsNullOrWhiteSpace($server) -and (!$locally)) {
		$serverElement = Get-InfrastructureServerElement -environment $environment -server $server	
		$serverType = $($serverElement.LocalName)
		switch ($serverType) {
			onPremiseMachine {
				if (Test-RemoteMachine $serverElement.machineName) {
					Invoke-ComponentPipelineRemotely -environment $environment -server $server -component $component -pipeline $pipeline -Credentials $Credentials
				} else {
					Invoke-ComponentPipelineLocally -environment $environment -server $server -component $component -pipeline $pipeline
				}
			}
			azureWorkerRole {
				Write-Log ERROR "You cannot launch a component deployment to a Azure Worker Role..."
				Write-Log ERROR "Use Invoke-ServerDeployment.ps1 or the '-locally' parameter if you want to install it locally."
				throw "Impossible to invoke component pipeline on a remote machine."
			}
			azureVirtualMachine {
				Write-Log ERROR "You cannot launch a component deployment to a Azure VirtualMachine..."
				Write-Log ERROR "Use Invoke-ServerDeployment.ps1 or the '-locally' parameter if you want to install it locally."
				throw "Impossible to invoke component pipeline on a remote machine."
			}
			onPremiseDatabaseServer {
				Invoke-ComponentPipelineLocally -environment $environment -server $server -pipeline $pipeline
			}
			azureDatabaseServer {
				Invoke-ComponentPipelineLocally -environment $environment -server $server -pipeline $pipeline
			}
			default {
				throw "Unrecognised server type: '$serverType'."
			}
		}
	} else {
		if ([string]::IsNullOrWhiteSpace($server) -and (!$locally)) {
			Write-Log WARNING "No server has been specified. The deployment will be launch locally."
		}
		Invoke-ComponentPipelineLocally -environment $environment -server $server -component $component -pipeline $pipeline
	}	
}

function Invoke-ServerPipelineLocally {
param(
	[string]$environment,
	[string]$server,
	[string]$pipeline,
	[string]$parameters,
	[int]$retryCount = 0
) 	
	$components = Get-ServerComponentsElements -environment $environment -server $server
	$components | foreach {		
		Invoke-ComponentPipelineLocally -environment $environment -component $_ -pipeline $pipeline -parameters $parameters -retryCount $retryCount
	}
}

function Invoke-ServerPipelineRemotely {
param(
	[string]$environment,
	[string]$server,
	[string]$pipeline,
	[PSCredential]$Credentials
)
	$serverElement = Get-InfrastructureServerElement -environment $environment -server $server	
	$machineName = $serverElement.machineName
	
	Write-Log WARNING "Launching the deployment remotely on '$machineName'"
	
	if (!(Test-NetworkPath $rootPath)) {
		Write-Log ERROR "To execute a remote deployment you must launch the script from a shared folder..."
		throw "Unable to execute the deployement remotely."
	}
		
	if ($Credentials -eq $null) {
		if ($currentSessionCredentials -eq $null) {		
			$cmTarget = "OvertureDeploy/$Environment"
			Write-Log WARNING "Looking up for '$cmTarget' in Windows Credential Store ($cmTarget)."
			if(Test-CredentialManagerItem $cmTarget) {
				$currentSessionCredentials = Get-CredentialManagerItem $cmTarget
			} else {				
				Write-Log warning "Please provide your credential to enable remote installation on $machineName"
				$currentUsername = $env:username
				$currentDomainUser = $env:USERDOMAIN
				[PSCredential]$currentSessionCredentials = Get-Credential -UserName "$currentDomainUser\$currentUsername" -Message "Remote Deployment"
			}
		}
		[PSCredential]$Credentials = $currentSessionCredentials
	}
	
	try
	{
		# Enabling SSP crediantials locally
		$enableWsManResult = Enable-WSManCredSSP -Role Client -DelegateComputer "*" -Force
		
		# Remotely enabling SSP credentials
		$enableRemoteSspCredResult = Invoke-Command -ComputerName $machineName -ScriptBlock { Enable-WSManCredSSP -Role Server -Force ; start-service winrm } -Credential $Credentials
		
		# Finally the command
		Invoke-Command -ComputerName $machineName -ScriptBlock `
	    { 	
			cd $args[4]
			write-host "REMOTE COMMAND  : .\Invoke-ServerPipelineLocally.ps1 -Environment $($args[0]) -Server $($args[1]) -Pipeline $($args[2]) -deploymentToken $($args[3]) -waitOnExit $false" -ForegroundColor Cyan
			write-host "EXECUTION FOLDER: $($args[4])" -ForegroundColor Cyan
			. .\Invoke-ServerDeployment.ps1 -Environment $args[0] -Server $args[1] -Pipeline $args[2] -deploymentToken $args[3] -locally $true -waitOnExit $false
		} `
	   -Authentication CredSSP -Credential $credentials -argumentlist @($Environment,$server,$pipeline,$currentDeploymentToken,$rootPath)
	} 
	finally
	{
		# Closing permisssions
		try{
			Disable-WSManCredSSP -Role Client
		}catch{
			Write-Log WARNING "Could not execute 'Disable-WSManCredSSP -Role Client', this could be because GPOs actively enforce it or other settings in registry, please validate it is the expected behavior"
		}
	}
}

function Invoke-ServerPipelineOnAzureWorkerRole {
param(
	[string]$environment,
	[string]$server,
	[string]$pipeline,
	$serverElement
)
	if (!$serverElement) {
		$serverElement = Get-InfrastructureServerElement -environment $environment -server $server	
	}

	$cloudServiceName = $serverElement.servicename
	if ([string]::IsNullOrWhiteSpace($cloudServiceName)) {
		throw "Unable to find the attribute 'servicename' from element <azureWorkerRole>."
	}
	
	$cloudLocation = $serverElement.location
	$cloudSlot = "Production"	
	$cloudInstancesCount = $serverElement.instanceCount
	$cloudVMSize = $serverElement.vmsize	
	$components = Get-ServerComponentsElements -environment $environment -server $server	
			
	& "$rootPath\Scripts\core\Invoke-AzureWorkerRoleDeployment.ps1" -Environment $environment `
																	-Server $server `
																	-Pipeline $pipeline `
																	-CloudServiceName $cloudServiceName `
																	-CloudLocation $cloudLocation `
																	-CloudInstancesCount $cloudInstancesCount `
																	-CloudVMSize $cloudVMSize `
																	-CloudSlot $cloudSlot `
																	-Components $components
}

function Invoke-ServerPipelineOnAzureVM {
param(
	[string]$environment,
	[string]$server,
	[string]$pipeline
)
	$cloudServiceName = $serverElement.servicename
	if ([string]::IsNullOrWhiteSpace($cloudServiceName)) {
		throw "Unable to find the attribute 'servicename' from element <azureVirtualMachine>."
	}
	
	$virtualMachineName = $serverElement.vmname
	if ([string]::IsNullOrWhiteSpace($cloudServiceName)) {
		throw "Unable to find the attribute 'vmname' from element <azureVirtualMachine>."
	}

	$cloudLocation = $serverElement.location
	$cloudInstancesCount = 1 #$serverElement.instanceCount
	$cloudVMSize = $serverElement.vmsize	
	$components = Get-ServerComponentsElements -environment $environment -server $server
		
	& "$rootPath\Scripts\core\Invoke-AzureVMDeployment.ps1" -Environment $environment `
															-Server $server `
															-Pipeline $pipeline `
															-CloudServiceName $cloudServiceName `
															-VirtualMachineName $virtualMachineName `
															-CloudLocation $cloudLocation `
															-CloudInstancesCount $cloudInstancesCount `
															-CloudVMSize $cloudVMSize `
															-Components $components
}

function Invoke-ChildNode(){
param(
	$child,
	[string]$environment,
	[string]$component,
	[string]$pipeline,
	[string]$parameters,
	[int]$retryCount = 0	
)
	#ignore commented pipeline
	if($child.NodeType -eq 'Comment') { 
		Write-Log WARNING "Skipping commented element: '$($child.Value)' from pipeline: '$pipeline' of the '$component' component."
		Continue
	}
	
	if ($child.LocalName -eq "pipelineRef") {
		Invoke-ComponentPipelineLocally -environment $environment -component $component -pipeline $child.name -parameters $parameters -retryCount $retryCount
	} elseif ($child.LocalName -eq "script") {
		$params = ($parameters + " " + $child.parameters).ToString().Trim()
		
		if ($retryCount -ne 0)
		{
			Invoke-ScriptWithRetry -path $child.path -environment $environment -component $component -parameters $params -retryCount $retryCount
		}
		else 
		{
			Invoke-Script -path $child.path -environment $environment -component $component -parameters $params
		}
	} else {
		throw "Not supported pipeline child element: '$child.LocalName'."
	}
}

function Get-NotNull($param1, $param2){
	if($null-ne $param1) {
		return $param1
	}elseif($null -ne $param2) {
		return $param2
	}
}

function Invoke-ComponentPipelineLocally {
param(
	[string]$environment,
	[string]$component,
	[string]$pipeline,
	[string]$parameters,
	[int]$retryCount = 0
)
	$pipelineElement = Get-ComponentPipelineElement -environment $environment -component $component -pipeline $pipeline
		
	if($inInteractiveMode){
		$childNodes = $pipelineElement.childnodes
		if ($childNodes -isnot [array]){
			[array]$childNodes = @($childNodes)
		}		
		$choice=""
		do{			
			$componentName = $pipelineElement.ParentNode.ParentNode.name
			$pipelineComponentName = "[$($pipelineElement.localname)] $($pipelineElement.name)"
			
			Write-Host "`n" -ForegroundColor DarkCyan
			Write-Host "INTERACTIVE MODE"  -ForegroundColor DarkCyan
			Write-Host "`n"  -ForegroundColor DarkCyan
			Write-Host "[Component] $componentName / $pipelineComponentName" -ForegroundColor DarkCyan
			Write-Host "`n"  -ForegroundColor DarkCyan
			for ($i=0; $i -lt $childNodes.length; $i++) {
				$nodeType = $($childNodes[$i].LocalName)
				$nodeDesc = Get-NotNull $($childNodes[$i].path) $($childNodes[$i].name)
				Write-Host "$i - [$nodeType] $nodeDesc"  -ForegroundColor DarkCyan
			}
			
			Write-Host "`n"  -ForegroundColor DarkCyan
			$choice = read-host "Enter script number to run or 'S' to skip"
			
			if ($choice  -match "[0-9]"){		
				$child = $childNodes[[int]$choice]
				Invoke-ChildNode -child $child -environment $environment -component $component -pipeline $pipeline -parameters $params -retryCount $retryCount
			}
		}while($choice.ToLower() -ne 's')
	}else{
		foreach ($child in $pipelineElement.childnodes)	{
			Invoke-ChildNode -child $child -environment $environment -component $component -pipeline $pipeline -parameters $params -retryCount $retryCount
		}
	}
}

function Invoke-ComponentPipelineRemotely {
param(
	[string]$environment,
	[string]$server,
	[string]$component,
	[string]$pipeline
)
	$serverElement = Get-InfrastructureServerElement -environment $environment -server $server	
	$machineName = $serverElement.machineName
	
	Write-Log WARNING "Launching the deployment remotely on '$machineName'"
	
	if (!(Test-NetworkPath $rootPath)) {
		Write-Log ERROR "To execute a remote deployment you must launch the script from a shared folder..."
		throw "Unable to execute the deployement remotely."
	}

	if ($Credentials -eq $null) {
		if ($currentSessionCredentials -eq $null) {		
			$cmTarget = "OvertureDeploy/$Environment"
			Write-Log WARNING "Looking up for '$cmTarget' in Windows Credential Store ($cmTarget)."
			if(Test-CredentialManagerItem $cmTarget) {
				$currentSessionCredentials = Get-CredentialManagerItem $cmTarget
			} else {				
				Write-Log warning "Please provide your credential to enable remote installation on $machineName"
				$currentUsername = $env:username
				$currentDomainUser = $env:USERDOMAIN
				[PSCredential]$currentSessionCredentials = Get-Credential -UserName "$currentDomainUser\$currentUsername" -Message "Remote Deployment"
			}
		}
		[PSCredential]$Credentials = $currentSessionCredentials
	}
		
	try
	{
		# Enabling SSP crediantials locally
		$enableWsManResult = Enable-WSManCredSSP -Role Client -DelegateComputer "*" -Force
		
		# Remotely enabling SSP credentials
		$enableRemoteSspCredResult = Invoke-Command -ComputerName $machineName -ScriptBlock { Enable-WSManCredSSP -Role Server -Force ; start-service winrm } -Credential $Credentials

		# Finally the command
		Invoke-Command -ComputerName $machineName -ScriptBlock `
	    {		
			cd $args[4]		
			write-host "REMOTE COMMAND  : .\Invoke-ServerPipelineLocally.ps1 -Environment $($args[0]) -Server $($args[1]) -component $($args[2]) -Pipeline $($args[3])" -ForegroundColor Cyan
			write-host "EXECUTION FOLDER: $($args[4])" -ForegroundColor Cyan
			. .\Invoke-ComponentDeployment.ps1 -Environment $args[0] -Server $args[1] -component $args[2] -Pipeline $args[3] -locally $true -waitOnExit $false 
		} `
	   -Authentication CredSSP -Credential $credentials -argumentlist @($Environment,$server,$component,$pipeline,$rootPath)
	} 
	finally
	{
		# Closing permisssions
		try{
			Disable-WSManCredSSP -Role Client
		}catch{
			Write-Log WARNING "Could not execute 'Disable-WSManCredSSP -Role Client', this could be because GPOs actively enforce it or other settings in registry, please validate it is the expected behavior"
		}		
	}
}

function Invoke-Script {
param(
	[string]$Path,
	[string]$Environment,
	[string]$Component,
	[string]$Parameters
)
	$errorActionPreference = "Stop"		
	$script = Join-Path $rootPath $Path
	$args = "-environment $Environment "
	if ($Component) {
		$args += "-component $Component "
	}
	$args += $Parameters
    $error.Clear()
	
    $commandLeaf = Split-Path $script -Leaf
    $scriptShort = [System.IO.Path]::GetFileNameWithoutExtension($commandLeaf)

    try 
    {
		Write-Host ""
		Write-Log none "-------------------------------"
        Write-Log none "Starting: $scriptShort ($Component)"
		Write-Log none "File : $Path"
		Write-Log none "Args : $args"
		Write-Log none "-------------------------------"
        Invoke-Expression "$script $args" 
		Write-Log none "-------------------------------"
        Write-Log none "Finished: $scriptShort"
		Write-Log none "-------------------------------"
    }
    catch 
    { 
        Write-Log ERROR "Error in script  : $($_.InvocationInfo.ScriptName)"
		Write-Log ERROR "At Line Number   : $($_.InvocationInfo.ScriptLineNumber)"
		Write-Log ERROR "Exception message: $(Get-FullExceptionMessage $_.Exception)"
		Write-Log ERROR "Exception info   : $($_.InvocationInfo)"
		if ($_.ErrorDetails.Message) {
		Write-Log ERROR "Error details    : $($_.ErrorDetails.Message)"
		}
        $error | Out-Log
        $error[0].ScriptStackTrace | Out-Log

		if ($env:SkipLogging -eq $false) {
			Write-host "Please review complete log for more details:`n $(Get-LogFilePath)`n"
		}

        Write-Log ERROR "Finished: $scriptShort"
		throw $error[0]
    }         
}

function Invoke-ScriptWithRetry{
param(
	[string]$Path,
	[string]$Environment,
	[string]$Component,
	[string]$Parameters,
	[int]$retryCount = 0
)
    $scriptShort = [System.IO.Path]::GetFileNameWithoutExtension($commandLeaf)

	for ($i = 1; $i -le $retryCount +1; $i++)
	{
		try 
		{
			Invoke-Script -path $Path -Environment  $Environment -Component $Component -Parameters $Parameters -retryCount $retryCount
			break;
		}
		catch
		{
			Write-Log ERROR "-------------------------------"
			if ($i -eq $retryCount +1) 
			{
				Write-Log ERROR "Script $scriptShort still failing after $i attempts"
				Write-Log ERROR "-------------------------------"
				throw $error[0]
			}
			else
			{
				Write-Log ERROR "Script $scriptShort failed for $i time, retrying ..."
				Write-Log ERROR "-------------------------------"
			}
		}
	}
}

function Get-TopologyElement {
param(
	[string]$environment
)
	$deploymentConfig = Get-DeploymentConfig -environment $environment
	if (!$deploymentConfig.topologies) {
		throw "Unable to find the '<topologies>' element."
	}

	$topologyName = $($deploymentConfig.infrastructure).topology
	$topologyElement = $deploymentConfig.topologies.topology |  where { $_.name -eq "$topologyName" }
	if (!$topologyElement) {
		throw "Unable to find the '$topologyName' topology from '$topologyXmlPath' file."
	}
	return $topologyElement
}

function Get-InfrastructureServerElement {
param(
	[string]$environment,
	[string]$server
)
    $deploymentConfig = Get-DeploymentConfig -environment $environment
	if (!$deploymentConfig.infrastructure) {
		throw "Unable to find the '<infrastructure>' element."
	}
	
	$serverElement = $deploymentConfig.infrastructure.ChildNodes | ? {$_.alias -eq $server }	
	if (!$serverElement) {
		$serverElement = $deploymentConfig.infrastructure.ChildNodes | ? {$_.machineName -eq $server }	
		if (!$serverElement) { 
			$serverElement = $deploymentConfig.infrastructure.ChildNodes | ? {$_.serviceName -eq $server }	
			if (!$serverElement) { 
				$serverElement = $deploymentConfig.infrastructure.ChildNodes | ? {$_.vmName -eq $server }	
				if (!$serverElement) { 
					$serverElement = $deploymentConfig.infrastructure.ChildNodes | ? {$_.dbServerName -eq $server }	
					if (!$serverElement) { 
						throw "Unable to find the server '$($server)' (from alias/machineName/serviceName/vmName/dbServerName)." 
					}
				}
			}
		}
	}

	return $serverElement
}

function Get-ServerComponentsElements {
param(
	[string]$environment,
	[string]$server
)
	$serverElement = Get-InfrastructureServerElement -environment $environment -server $server
	$serverServerAlias = $serverElement.alias
	
	$topologyElement = Get-TopologyElement -environment	$environment
	$topologyServerAlias = $topologyElement.server | where { $_.alias -eq $serverServerAlias }	
	if (!$topologyServerAlias) {
		throw "Unable to find the server alias '$serverAlias'."
	}
	
	return $topologyServerAlias.component | select -ExpandProperty name
}

function Get-ComponentPipelineElement {
param(
	[string]$Environment,
    [string]$Component,
    [string]$Pipeline
)
    $deploymentConfig = Get-DeploymentConfig -environment $environment

	if (!$deploymentConfig.components) {
		throw "Unable to find the '<components>' element."
    }

    $ComponentElement = $deploymentConfig.components.component | ? {$_.name -eq $Component }
	[string]$AvailComponents = $deploymentConfig.component.name
    if (!$ComponentElement) 
	{ 
		Write-Log ERROR "Unable to find the component '$Component'."
		Write-Log INFO "Available components are: $AvailComponents"
		throw "Unable to find component." 
	}

    $pipelineElement = $ComponentElement.pipelines.ChildNodes | ? { $_.name -eq $Pipeline }
	[string]$AvailPipelines = $ComponentElement.pipelines.ChildNodes.name
    if (!$pipelineElement) 
	{
		Write-Log ERROR "Unable to find the pipeline '$Pipeline' for the component '$Component' and the environment '$Environment'."
		Write-Log INFO "Available pipelines are: $AvailPipelines"
		throw "Unable to find the pipeline" 
	}
	return $pipelineElement
}

function Get-Settings {
param(
    [Parameter(Mandatory=$True)][string]$Environment,
    [string]$key = ""
)
	$deploymentConfig = Get-DeploymentConfig -environment $environment
	
    if ([string]::IsNullOrWhiteSpace($key)) {
        $deploymentConfig.Parameters
    } else {
        $deploymentConfig.Parameters[$key]
    }
}

function Test-Setting {
param(
    [Parameter(Mandatory=$True)][string]$Environment,
    [string]$key = ""
)
	$deploymentConfig = Get-DeploymentConfig -environment $environment
	return $deploymentConfig.Parameters.Contains($key)
}

function Invoke-ComponentConfigTransformation {
param(
	[Parameter(Mandatory=$True)][string]$ConfigFile,
	[Parameter(Mandatory=$True)][string]$InstallPath,
	[Parameter(Mandatory=$True)][string]$Environment,
	[Parameter(Mandatory=$True)][string]$Component,
	[string]$ComponentVariation,	
	$Parameters = @{},
	$NewName = $ConfigFile,
	$CleanDestination = $true
)
	$configFileAbsolutePath = Join-Path $InstallPath $ConfigFile

	$source = "$configFileAbsolutePath.original"
	$destination = $configFileAbsolutePath

	# Ensure ".original" file exits
	if (!(Test-Path $source)) {
		Copy-Item $configFileAbsolutePath $source -force
	}

	$configFileName = [System.IO.Path]::GetFileName($configFileAbsolutePath)
	$configFileExtension = [System.IO.Path]::GetExtension($configFileAbsolutePath)
	$transformAllName = $ConfigFile.Replace($configFileExtension, ".ALL$configFileExtension")
	$transformEnvName = $ConfigFile.Replace($configFileExtension, ".$Environment$configFileExtension")

	$PackageFolder = Join-Path $rootPath "packages"
	$genericTransform = "$PackageFolder\generic\$Component\$transformAllName"
	$specificAllTransform = "$PackageFolder\specific\$Component\$transformAllName"
	$specificEnvTransform = "$PackageFolder\specific\$Component\$transformEnvName"
		
	$transforms = @()
	if (Test-Path $genericTransform) { $transforms += $genericTransform }
	if (Test-Path $specificAllTransform) { $transforms += $specificAllTransform }
	if (Test-Path $specificEnvTransform) { $transforms += $specificEnvTransform }

	if ($configFileName -match "\.exe\.") {
		$genericTransform = "$PackageFolder\generic\$Component\App.All.config"
		if (Test-Path $genericTransform) { $transforms += $genericTransform }

		$specificAllTransform = "$PackageFolder\specific\$Component\App.All.config"
		$specificEnvTransform = "$PackageFolder\specific\$Component\App.$($Environment).config"
		if (Test-Path $specificAllTransform) { $transforms += $specificAllTransform }
		if (Test-Path $specificEnvTransform) { $transforms += $specificEnvTransform }
	}

	if(!$transforms) { throw "No transformation file found for '$ConfigFile' from '$PackageFolder\generic' and '$PackageFolder\specific' folder." }

	$deploymentFolderPath = Get-DeploymentFolderPath 
	$paramFilePath = Get-ParametersFilePath
	
	if (![System.String]::IsNullOrWhiteSpace($ComponentVariation)) {
		Add-SpecialTokenParameters $ComponentVariation $transforms $Environment $Parameters
	}

	$transforms | % { Write-Log INFO $_.Replace($PackageFolder,".") }
	Invoke-ConfigTransformation -source $source -transform $transforms -destination $destination -parameters $Parameters -paramFilePath $paramFilePath -cleanDestination $CleanDestination
	
<#
	#ADD 'NewName' as a function parameter
	$transforms | % { Write-Log INFO $_.Replace($PackageFolder,".") }
	$destination = $configFileAbsolutePath.Replace($ConfigFile, $NewName)
	Invoke-ConfigTransformation -source $source -transform $transforms -destination $destination -parameters $parameters -paramFilePath $paramFilePath -cleanDestination $true
#>
}

function Add-SpecialTokenParameters {
param(
	[string]$ComponentVariation,
	$TransformFiles,
	[string]$environment,
	$Parameters
)
	$Parameters["Component_Variation"] = $ComponentVariation
	$TransformFiles | % {
		$fileContent = (Get-Content $_)
		$tokens = [regex]::Matches($fileContent, "{([\S-[}]]*)}")
		$tokens | % {
			$token = $_.groups[1].Value
			if ($token -match "\[ComponentVariationToken\]") { 
				$name = $token.Replace('[ComponentVariationToken]', $ComponentVariation)
				if ($Parameters.Contains($name)) {
					$Parameters[$token] = $Parameters[$name]
				} elseif (Test-Setting $environment $name) {
					$Parameters[$token] = Get-Settings $environment $name
				} else {
					Write-Log WARNING "No value: $name"
				}
			}
		}
	}
}

function Get-DeploymentConfig {
param(
	[string]$environment
)
	$deploymentConfig = $configurationHashMap.Get_Item($currentDeploymentToken)	
	if (!$deploymentConfig) 
	{
		$deploymentConfig = Invoke-DeploymentConfigGeneration $environment
		$configurationHashMap.Set_Item($currentDeploymentToken, $deploymentConfig)
	}
	return $deploymentConfig
}

function Invoke-DeploymentConfigGeneration {
param(
	[string]$environment
)
	$deploymentFolderPath = Get-DeploymentFolderPath
	if (!(Test-Path $deploymentFolderPath)) {
		New-Item -ItemType directory -Path $deploymentFolderPath | out-null
	}

	Copy-Item $rootPath\configs\generic\Topologies.xml $deploymentFolderPath 
	[xml]$topologiesXml = (Get-Content $deploymentFolderPath\Topologies.xml)

	Copy-Item "$rootPath\configs\specific\$($environment)\Infrastructure.$($environment).xml" $(Join-Path $deploymentFolderPath Infrastructure.xml)
	[xml]$infrastructureXml = (Get-Content "$deploymentFolderPath\Infrastructure.xml")	   	
	
	$parameters = Invoke-ParametersFileGeneration $environment $deploymentFolderPath
	$componentsXml = Invoke-ComponentsFileGeneration $environment $deploymentFolderPath
	
    return New-Object PSObject –Property @{Parameters=$parameters;Components=$componentsXml.components;Infrastructure=$infrastructureXml.infrastructure;Topologies=$topologiesXml.topologies}
}

function Invoke-ParametersFileGeneration {
param(
	[string]$environment,
	[string]$deploymentFolderPath
)
	$Parameters = @{}
	
	$genericParametersFilePath = Join-Path $rootPath "configs\generic\Parameters.xml"
	Read-ParametersFromFile $genericParametersFilePath $Parameters

	$specificParametersFilePath = Join-Path $rootPath "configs\specific\Parameters.All.xml"
	Read-ParametersFromFile $specificParametersFilePath $Parameters

	$envSpecificParametersFilePath = Join-Path $rootPath "configs\specific\$($environment)\Parameters.$($environment).xml"
	if (Test-Path $envSpecificParametersFilePath) {
		Read-ParametersFromFile $envSpecificParametersFilePath $Parameters
	} else {
		Write-Log ERROR "Parameters.$($environment).xml not found..."
		throw "Parameters.$($environment).xml not found..."
	}

	#Perform token replacement
	$count = 0
	Do {
		$changeOccured = $false
		@($Parameters.Keys) | % {
			$paramKey = $_
			$paramValue = $Parameters[$_]			
			[regex]::Matches($paramValue, "{([\S-[}]]*)}") | % {
				$exp = $_.groups[0].Value
				$key = $_.groups[1].Value											
				if (($key -ne $paramKey) -and ($Parameters.keys.Contains($key))) {
					$value = $Parameters[$key]
					$Parameters[$paramKey] = $paramValue.replace($exp, $value)
					$changeOccured = $true
				}	
			}
		}
		$count += 1
	} while ($changeOccured -and $count -le 1000)
	
	$paramFilePath = Get-ParametersFilePath
	
	[xml]$xml = [xml]"<?xml version='1.0' encoding='utf-8'?><parameters></parameters>"
	$Parameters.keys | Sort-Object | % { 
		$newElement = $xml.CreateElement('param')
		$newElement.SetAttribute("name", "$_") | out-null
		$newElement.SetAttribute("value", $Parameters[$_]) | out-null
		$xml.SelectSingleNode("parameters").AppendChild($newElement) | out-null
	}
	
	Save-XmlAsFormattedDocument $xml $paramFilePath | out-null
	
	if ($count -ge 1000) {
		throw "Perform token replacement failed: infinite looping. Look at '$($paramFilePath)' for guilty parameters/tokens..."
	}

	return $Parameters
}

function Invoke-ComponentsFileGeneration {
param(
 	[string]$environment,
	[string]$deploymentFolderPath
)
	Write-Log INFO "==========================================================="
	Write-Log INFO "Generating components pipelines configuration ($environment)"
	Write-Log INFO "==========================================================="

	$xmlConfigPath = Join-Path $deploymentFolderPath "Components.xml"	  

	[xml]$xml = [xml]"<?xml version='1.0' encoding='utf-8'?>"
	$newEl = $xml.CreateElement('components')
	$xml.AppendChild($newEl) | out-null
	$xml.xml = $($xml.CreateXmlDeclaration("1.0","UTF-8","")).Value
	$xml.Save($xmlConfigPath -f (get-location)) | out-null

	$transform = @()
	$transform += Join-Path $rootPath "configs\generic\Components.xml"
	$transform += Join-Path $rootPath "configs\specific\Components.All.xml"	
	if (Test-Path "$rootPath\configs\specific\$($environment)\Components.$($environment).xml") {
		$transform += Join-Path $rootPath "configs\specific\$($environment)\Components.$($environment).xml"
	} else {
		Write-Log Warning "Components.$($environment).xml not found..."
	}
	
	$deploymentFolderPath = Get-DeploymentFolderPath 
	$paramFilePath = Get-ParametersFilePath
	
	Invoke-ConfigTransformation -source $xmlConfigPath -transform $transform -destination $xmlConfigPath -paramFilePath $paramFilePath -CleanDestination $false
	
	[xml]$xmlCfg = Get-Content $xmlConfigPath -ErrorAction SilentlyContinue	
	$deploymentConfig = $xmlCfg.components	
	if (!$deploymentConfig) {  
		Write-Log ERROR "Unable to find the '<components>' element."
		Write-Log ERROR "In the file: $xmlConfigPath"
		throw "Invalid generated config file."
	}

	return [xml]$xmlCfg
}

function Read-ParametersFromFile {
param(
	[string]$xmlParameterFilePath,
	$Parameters = @{}
)	
	if (Test-Path $xmlParameterFilePath) {		
		try
		{
			[xml]$xmlParam = (Get-Content $xmlParameterFilePath -ErrorAction SilentlyContinue)
			if (!$xmlParam -or !$xmlParam.GetElementsByTagName("parameters")) {
				throw			
			}
			if ($xmlParam.parameters.param) {
				$xmlParam.parameters.param | % {
					$Parameters[$_.name] = $_.value
				}
			}
		} catch {
			throw "Unable to read the file '$xmlParameterFilePath'. Ensure that it is a valid xml file and that it minimally contains a '<parameters />' element."
		}				
	} else {
		Write-Log WARNING "Parameter file '$xmlParameterFilePath' is not existing."
	}
}

function Invoke-FolderBackup {
param (
	[Parameter(Mandatory=$true)][string]$Component,
    [Parameter(Mandatory=$true)][string]$SourcePath
) 
	if (!(Test-Path $sourcePath)) {	throw "The source folder to backup doesn't exits." }	

	$destinationPath = Join-Path (Get-BackupFolderPath) $Component

	if ( (Get-ChildItem $sourcePath).count -gt 0 )
	{	
		mkdir $destinationPath -Force
		$folderName	= (get-item $sourcePath).Name
		$zipName = $folderName + ".zip"
		$backupZipFilePath = Join-Path $destinationPath $zipName
		
		"Backing up '$sourcePath' To: '$backupZipFilePath'..."
		Invoke-ZipFile -SourceDir $sourcePath -DestinationZipFile $backupZipFilePath
	}
}

<#
.EXAMPLE
    Write-Log "This is first a log event no colors"
    Write-Log error "This is a log event, error type" 
    Write-Log warning "This is a log warning..."
    Write-Log warning "This is a warning log event not printed to screen"
#>
function Write-Log {
param (
    [Parameter(Mandatory=$false,Position=0)][string]$Type = "none",
    [Parameter(Mandatory=$false,Position=1)][string]$Message = ""
)   
    $caller = ""
    $commandName = (Get-PSCallStack)[1].Command
    if ($commandName -ne "") {
        $commandLeaf = Split-Path $commandName -Leaf -ErrorAction SilentlyContinue
    } else {
        $commandLeaf = (Get-PSCallStack)[2].Command
    }
    $scriptName	= [System.IO.Path]::GetFileNameWithoutExtension($commandLeaf)
    $caller		= "[" + $scriptName + "]"
    $timestamp	= Get-Date -Format 'yyyy-MM-dd hh:mm:ss.fff'

    switch ($Type.ToLower()) {
       "error"   { $txtcolor = "Red"    ; $logTag =  "[ERROR] " }
       "action"  { $txtcolor = "Green"  ; $logTag =  "[ACTION] " }
       "warning" { $txtcolor = "Yellow" ; $logTag =  "[WARNING] " }
       "none"    { $txtcolor = "White"  ; $logTag =  "[INFO] " }
       "message" { $txtcolor = "White"  ; $logTag =  "[MESSAGE] "  }
        default  { $txtcolor = "White"  ; $logTag =  "" }
    }

    if (($Message -eq "") -and ($Type -ne "none")) {
        $Message = $Type
    }

    $output = "$timestamp $caller $Message"
    # Writing to console only if verbode was mentionned
    Write-Host "$output" -ForegroundColor $txtcolor

	# Logging to file
	if ($env:SkipLogging -eq $false) {    
		Add-Content -Path (Get-LogFilePath) "$timestamp $caller $logTag $Message" -ErrorAction SilentlyContinue
	}
}

<#
.EXAMPLE
    # enable , or not, simultaneous output to console
    # typical usage
    Get-ChildItem c:\ -Recurse 2>&1 | Out-Log
    # typical usage verbose
    Get-ChildItem c:\ -Recurse 2>&1 | Out-Log verbose
    # This will not work:
    Write-Host "ALLOALLOALLOALLO" -BackgroundColor Black -ForegroundColor Green | Out-Log
#>
function Out-Log {
    begin
    {
        $commandName = (Get-PSCallStack)[1].Command
        if ($commandName -ne "")
        {
            $commandLeaf = Split-Path $commandName -Leaf -ErrorAction SilentlyContinue
        }
        else
        {
            $commandLeaf = (Get-PSCallStack)[2].Command
        } 
        $scriptName  = [System.IO.Path]::GetFileNameWithoutExtension($commandLeaf)
        $caller      = "[" + $scriptName + "]"
        if ($args[0] -ne $null) { $verbosity = $true }
    }
    process
    {         
        $timestamp   = Get-Date -Format 'yyyy-MM-dd hh:mm:ss.fff'
        if ($_ -ne $null) { $message = $_.ToString()} else { $message = ""}
        if ($verbosity) { write-host "$timestamp $caller $message" -ForegroundColor Cyan }
		if ($env:SkipLogging -eq $false) {    
			Add-Content -Path (Get-LogFilePath) "$timestamp $caller [OUTPUT] $message" -ErrorAction SilentlyContinue
		}
    }
}

Function Get-OvertureDatabaseConnectionString {
param(
	[Parameter(Mandatory=$true)][string]$Environment,
	[Parameter(Mandatory=$true)][string]$DatabaseName
)
	$serverName = Get-Settings -environment $Environment -key "DB-$($databaseName)_Server"
	if ([string]::IsNullOrEmpty($serverName)) { throw "Unable to find the '$databaseName' server name." }

	$dbName = Get-Settings -environment $Environment -key "DB-$($databaseName)_Database"
	if ([string]::IsNullOrEmpty($databaseName)) {  Throw "Unable to find the '$databaseName' database name." }

	$user = Get-Settings -environment $Environment -key "DB-$($databaseName)_Username"
	if ([string]::IsNullOrEmpty($user)) { throw "Unable to find the '$databaseName' user name." }

	$password = Get-Settings -environment $Environment -key "DB-$($databaseName)_Password"
	if ([string]::IsNullOrEmpty($password)) { throw "Unable to find the '$databaseName' user password." }

	return "Server=$($serverName);User Id=$($user);Password=$($password);Database=$($dbName);"
}

function Test-ServiceRunning {
param(
	$serviceName
)
	$runnning = $false
	for ($i=1; $i -le 5; $i++)
	{
		$DequeuerServiceStatus = Get-Service -Name $serviceName
		if ($DequeuerServiceStatus.Status -eq "Running")
		{
			$runnning = $true
			break
		}
		Start-Sleep -Seconds 1
	}
	return $runnning
}

function LoadAssemblyIfNeeded {
	$loaded = [System.AppDomain]::CurrentDomain.GetAssemblies() | Select -ExpandProperty Fullname
	if (-not ($loaded | Where { $_.Contains("Microsoft.Web.Administration") })) {		
		$inetsrvPath = "{0}\system32\inetsrv\" -f ${env:windir}
		[System.Reflection.Assembly]::LoadFrom( $inetsrvPath + "Microsoft.Web.Administration.dll" )
		[System.Reflection.Assembly]::LoadFrom( $inetsrvPath + "Microsoft.Web.Management.dll" )
	}
}