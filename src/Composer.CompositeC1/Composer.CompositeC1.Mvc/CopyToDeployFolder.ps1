$depoymentFolder = "$PSScriptRoot\..\..\..\deployment\WebSite"
$srcFolder = "$PSScriptRoot\..\.."

if (Test-Path $depoymentFolder) {
    Write-Host "Coping files..."

    Copy-Item "$srcFolder\Composer\Composer\bin\Debug\Orckestra.Composer.dll" -Destination "$depoymentFolder\Bin\Orckestra.Composer.dll"
    Copy-Item "$srcFolder\Composer\Composer.Cart\bin\Debug\Orckestra.Composer.Cart.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer\Composer.MyAccount\bin\Debug\Orckestra.Composer.MyAccount.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer\Composer.Product\bin\Debug\Orckestra.Composer.Product.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer\Composer.Search\bin\Debug\Orckestra.Composer.Search.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer\Composer.Store\bin\Debug\Orckestra.Composer.Store.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer\Orckestra.Composer.SearchQuery\bin\Debug\Orckestra.Composer.SearchQuery.dll" -Destination "$depoymentFolder\Bin"

    Copy-Item "$srcFolder\Composer.CompositeC1\Composer.CompositeC1\bin\Debug\Orckestra.Composer.CompositeC1.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Composer.CompositeC1.DataTypes\bin\Debug\Orckestra.Composer.CompositeC1.DataTypes.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Composer.CompositeC1.Mvc\bin\Orckestra.Composer.CompositeC1.Mvc.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Orckestra.Composer.Articles\bin\Debug\Orckestra.Composer.Articles.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Orckestra.Composer.C1CMS.Queries\bin\Debug\Orckestra.Composer.C1CMS.Queries.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Orckestra.Composer.ContentSearch\bin\Debug\Orckestra.Composer.ContentSearch.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Orckestra.Composer.Sitemap\bin\Debug\Orckestra.Composer.Sitemap.dll" -Destination "$depoymentFolder\Bin"
    Copy-Item "$srcFolder\Composer.CompositeC1\Composer.CompositeC1\bin\Debug\Orckestra.Composer.CompositeC1.dll" -Destination "$depoymentFolder\Bin"
	Copy-Item "$srcFolder\Composer.CompositeC1\Orckestra.Composer.HandlebarsCompiler\bin\Debug\Orckestra.Composer.HandlebarsCompiler.dll" -Destination "$depoymentFolder\Bin"
	
    Copy-Item -Path (Get-Item -Path "$srcFolder\Composer.CompositeC1\Composer.CompositeC1.Mvc\UI.Package\*" -Exclude ('Sass')).FullName -Destination "$depoymentFolder\UI.Package" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.Articles.Package\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force
	Copy-Item -Path "$srcFolder\Composer.CompositeC1\Composer.CompositeC1.Mvc\App_Data" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.C1.Core\Package\Views" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.C1CMS.Queries.Package\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.ContentSearch.Content\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.ContentSearch.Package\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force
	Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.ContentSearch.Package\Package\Views" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.SEO.Organization\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.Sitemap.Package\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.Sitemap.Package\Package\Composite" -Destination "$depoymentFolder" -Recurse -Force
    Copy-Item -Path "$srcFolder\Composer.CompositeC1\Orckestra.Composer.SEO.Organization\Package\App_Data" -Destination "$depoymentFolder" -Recurse -Force

    
    Write-Host "Done"
}
else {
    Write-Host "Not deployed yet. Run 'Build\BuildAndInstall.ps1'"
}
