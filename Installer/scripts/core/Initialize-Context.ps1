#requires -version 3.0
$currentScriptPath = Split-Path $MyInvocation.MyCommand.Path

# load all modules
gci -Path "$currentScriptPath\..\*.psm1" -rec | where { ! $_.PSIsContainer } | % {
	#"Importing module $_"	
	Remove-Module $_ -Force -ErrorAction SilentlyContinue #ensure module reloading
	Import-Module $_ -Global
}