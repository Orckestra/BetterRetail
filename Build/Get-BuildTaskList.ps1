Import-Module (Join-Path $PSScriptRoot '..\lib\psake\psake') -Verbose:$false
$psakeScript = Join-Path $PSScriptRoot 'Build.psake.ps1'

. (Join-Path $PSScriptRoot .\Load-BootstrapPackages.ps1)

$Items = Invoke-psake $psakeScript -structuredDocs -nologo 

try {
    $Items | Select-Object -Property name -ExpandProperty name -ErrorAction Stop
}
catch {
    throw $Items
}
