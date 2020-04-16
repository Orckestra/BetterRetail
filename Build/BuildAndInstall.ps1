$Success = Invoke-Expression "& `"$PSScriptRoot\build.ps1`" -t dev $args"
if ($Success) {
	Invoke-Expression "& `"$PSScriptRoot\install.ps1`" $args"
}

