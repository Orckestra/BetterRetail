cls

$SourcePath = 'D:\DEV\ComposerAll\CF\Source'

function Repack-PackagesConfigFile(){
    Copy-Item .\Composer.Mvc.Sample\packages.config .\Composer.Mvc.Sample\packages.config.original -Force

    $packagesConfigFiles = Get-ChildItem -Path .\ -Include "packages.config" -Recurse -ErrorAction SilentlyContinue -ErrorVariable WillNotUseIt | ? { $_.FullName -inotlike '*.*Tests\*' }
    $allPackages = $packagesConfigFiles | % { [string[]](Get-Content $_) } | ? {$_ -ilike '*<package *'} | select -uniq | sort

    $finalPackagesConfigFile = ''

    $allPackages | % { $finalPackagesConfigFile += "$_`n"}
    $finalPackagesConfigFile = '<?xml version="1.0" encoding="utf-8"?>'+"`n"+'<packages>'+"`n" + $finalPackagesConfigFile + "</packages>"
    Set-Content -value $finalPackagesConfigFile -Path ".\Composer.Mvc.Sample\packages.config" -encoding UTF8
    Set-Content -value $finalPackagesConfigFile -Path ".\Composer.Mvc.Sample\packages.config.new" -encoding UTF8

    $originalFile = [string[]](Get-Content ".\Composer.Mvc.Sample\packages.config.original")
    $newFile = [string[]](Get-Content ".\Composer.Mvc.Sample\packages.config.new")
    "" > .\Composer.Mvc.Sample\NugetToReAddByHand.txt
    $newFile | % {if(-not $originalFile.Contains($_)){ $_ >> .\Composer.Mvc.Sample\NugetToReAddByHand.txt} }
}

function Repack-Dlls(){
    robocopy .\Composer.Mvc.Sample\bin .\Composer.Mvc.Sample\lib Orckestra.Composer.*.dll /E /XF Orckestra.Composer.Mvc.Sample.*
    robocopy .\Composer.Mvc.Sample\bin .\Composer.Mvc.Sample\lib Orckestra.Composer.dll /E /XF Orckestra.Composer.Mvc.Sample.*
    robocopy ..\Dependencies\ .\Composer.Mvc.Sample\lib * /E
}

function Regenerate-CsProj(){
    $csproj = get-content ".\Composer.Mvc.Sample\Composer.Mvc.Sample.csproj" -raw

    $regex = '(<ProjectReference Include="\.\.\\)(Composer\..+)\\Composer\..*\.csproj">\r\n.*\r\n.*\r\n.*'
    $replacement = '<Reference Include="Orckestra.$2">'+"`n"+'      <SpecificVersion>False</SpecificVersion>'+"`n"+'      <HintPath>.\lib\Orckestra.$2.dll</HintPath>'+"`n"+'    </Reference>'
    $csproj = $csproj -replace $regex, $replacement

    $regex = '(<ProjectReference Include="\.\.\\)(Composer)\\Composer\.csproj">\r\n.*\r\n.*\r\n.*'
    $replacement = '<Reference Include="Orckestra.$2">'+"`n"+'      <SpecificVersion>False</SpecificVersion>'+"`n"+'      <HintPath>.\lib\Orckestra.$2.dll</HintPath>'+"`n"+'    </Reference>'
    $csproj = $csproj -replace $regex, $replacement

    $regex = '<Compile Include="\.\.\\Solution Items\\SharedAssemblyInfo.cs">\r\n.*\r\n.*'
    $replacement = ''
    $csproj = $csproj -replace $regex, $replacement

    $regex = '(<HintPath>)(\.)(\.\\packages)'
    $replacement = '$1$3'
    $csproj = $csproj -replace $regex, $replacement

    $regex = '(<HintPath>)(\.\.\\\.\.\\Dependencies)'
    $replacement = '$1.\lib'
    $csproj = $csproj -replace $regex, $replacement

    Set-Content -value $csproj -Path ".\Composer.Mvc.Sample\Composer.Mvc.csproj" -encoding UTF8
}

Repack-Dlls
Regenerate-CsProj

Repack-PackagesConfigFile

Remove-Item -Path ./Composer.Mvc.zip -Force -ErrorAction SilentlyContinue -ErrorVariable WillNotUseIt
7z a Composer.Mvc.zip ./Composer.Mvc.Sample/* "-x!Composer.Mvc.Sample.dll" "-x!Composer.Mvc.Sample.csproj*" "-x!Composer.Mvc.Sample.csproj*" "-x!Orckestra.Composer.Mvc.Sample.dll" "-x!Orckestra.Composer.Mvc.Sample.csproj*" "-x!Orckestra.Composer.Mvc.Sample.csproj*" "-x!bin\*" "-x!obj\*" "-x!packages\*"

Copy-Item .\Composer.Mvc.Sample\packages.config.bak .\Composer.Mvc.Sample\packages.config -Force
Remove-Item .\Composer.Mvc.Sample\Composer.Mvc.csproj -Force -ErrorAction SilentlyContinue -ErrorVariable WillNotUseIt