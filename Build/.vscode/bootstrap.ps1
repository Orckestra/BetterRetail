if (-not (Test-Path "$PSScriptRoot\..\tools\Addins\Cake.CoreCLR.0.35.0")) {
    Invoke-Expression "& `"$PSScriptRoot\..\build.ps1`" -DryRun"
}
