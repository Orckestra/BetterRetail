$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet tool restore --tool-manifest "$($PSScriptRoot)\.config\dotnet-tools.json"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Invoke-Expression "& `"$($PSScriptRoot)\build.ps1`" -script install.cake $args"
