#requires -version 3.0
$thisScriptPath = Split-Path $MyInvocation.MyCommand.Path

# remove all concerned modules
gci -Path "$thisScriptPath\..\*.psm1" -rec | where { ! $_.PSIsContainer } | % {
	$filename = $_.Name
	# this will safely remove the '.ps1' extention even if name format is "aaa.bb.c.ps1"
	$filenameNoExtention = [io.path]::GetFileNameWithoutExtension($filename)
	Remove-Module $filenameNoExtention -Force -ErrorAction SilentlyContinue
}