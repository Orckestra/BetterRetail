$depoymentFolder = "$PSScriptRoot\..\..\deployment\WebSite"
$srcFolder = "$PSScriptRoot\.."

if (Test-Path $depoymentFolder) {
    Write-Host "Coping files..."

    Copy-Item "$srcFolder\Orckestra.Composer\bin\Debug\Orckestra.Composer.dll" -Destination "$depoymentFolder\Bin\"
    Copy-Item "$srcFolder\Orckestra.Composer.Cart\bin\Debug\Orckestra.Composer.Cart.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.MyAccount\bin\Debug\Orckestra.Composer.MyAccount.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.Product\bin\Debug\Orckestra.Composer.Product.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.Search\bin\Debug\Orckestra.Composer.Search.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.Store\bin\Debug\Orckestra.Composer.Store.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.SearchQuery\bin\Debug\Orckestra.Composer.SearchQuery.dll" -Destination "$depoymentFolder\Bin"

    Copy-Item "$srcFolder\Orckestra.Composer.CompositeC1\bin\Debug\Orckestra.Composer.CompositeC1.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.CompositeC1.DataTypes\bin\Debug\Orckestra.Composer.CompositeC1.DataTypes.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.Website\bin\Orckestra.Composer.Website.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.Articles\bin\Debug\Orckestra.Composer.Articles.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.C1CMS.Queries\bin\Debug\Orckestra.Composer.C1CMS.Queries.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.ContentSearch\bin\Debug\Orckestra.Composer.ContentSearch.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.Sitemap\bin\Debug\Orckestra.Composer.Sitemap.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Orckestra.Composer.CompositeC1\bin\Debug\Orckestra.Composer.CompositeC1.dll" -Destination "$depoymentFolder\Bin"
	Copy-Item "$srcFolder\Orckestra.Composer.HandlebarsCompiler\bin\Debug\Orckestra.Composer.HandlebarsCompiler.dll" -Destination "$depoymentFolder\Bin"
	
    XCopy "$srcFolder\Orckestra.Composer.Website\UI.Package\JavaScript" "$depoymentFolder\UI.Package\JavaScript" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.Website\UI.Package\Tests" "$depoymentFolder\UI.Package\Tests" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.Website\UI.Package\Typings" "$depoymentFolder\UI.Package\Typings" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.Articles.Package\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
	XCopy "$srcFolder\Orckestra.Composer.Website\App_Data" "$depoymentFolder\App_Data" /i /e /Y
    #This path does not exist, probably it was forgotten to be deleted from here
    #Copy-Item -Path "$srcFolder\Orckestra.Composer.C1.Core\Package\Views" -Destination "$depoymentFolder" -Recurse -Force
    XCopy "$srcFolder\Orckestra.Composer.C1CMS.Queries.Package\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.ContentSearch.Content\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.ContentSearch.Package\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
	XCopy "$srcFolder\Orckestra.Composer.ContentSearch.Package\Package\Views" "$depoymentFolder\Views" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.SEO.Organization.Package\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
    #No any files there
    #XCopy "$srcFolder\Orckestra.Composer.SEO.Organization.Content\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.Sitemap.Package\Package\App_Data" "$depoymentFolder\App_Data" /i /e /Y
    XCopy "$srcFolder\Orckestra.Composer.Sitemap.Package\Package\Composite" "$depoymentFolder\Composite" /i /e /Y
    #It seems this file has not been updated some time, it has to be reviewed with new possible pathes
    Write-Host "Done"
}
else {
    Write-Host "Not deployed yet. Run 'Build\BuildAndInstall.ps1'"
}
