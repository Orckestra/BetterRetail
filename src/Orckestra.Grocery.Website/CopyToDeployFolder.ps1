$depoymentFolder = "$PSScriptRoot\..\..\deployment\WebSite"
$srcFolder = "$PSScriptRoot\.."

if (Test-Path $depoymentFolder) {
    Write-Host "Coping files..."

    XCopy "$srcFolder\Orckestra.Grocery.Website\UI.Package\JavaScript" "$depoymentFolder\UI.Package\JavaScript" /i /e /Y
    XCopy "$srcFolder\Orckestra.Grocery.Website\UI.Package\Sass" "$depoymentFolder\UI.Package\Sass" /i /e /Y
	XCopy "$srcFolder\Orckestra.Grocery.Website\UI.Package\LocalizedStrings" "$depoymentFolder\UI.Package\LocalizedStrings" /i /e /Y
    XCopy "$srcFolder\Orckestra.Grocery.Website\UI.Package\Typescript" "$depoymentFolder\UI.Package\Typescript" /i /e /Y
   
    #It seems this file has not been updated some time, it has to be reviewed with new possible pathes
    Write-Host "Done"
}
else {
    Write-Host "Not deployed yet. Run 'Build\BuildAndInstall.ps1'"
}
