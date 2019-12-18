if (-not (Test-Path "$PSScriptRoot\..\tools\Addins\Cake.CoreCLR.0.35.0")) {
    Invoke-Expression "& `"$PSScriptRoot\..\build.ps1`" -script build.cake -DryRun"
    Invoke-Expression "& `"$PSScriptRoot\..\build.ps1`" -script install.cake -DryRun"
}
