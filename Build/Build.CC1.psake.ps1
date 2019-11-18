$VisualStudioVersion                      = '2019'

Task CC1 -depends CC1_RestoreNugetPackages,
                  CC1_BuildAndPackage
                  #CC1_PackageNuget,
                  #CC1_PublishArtifacts
				  
Task CC1Dev -depends CC1_RestoreNugetPackages,
					 CC1_Copy-UiPackageFromNuget,
                     CC1_Compile-Solution

function CC1_InitializeVariables {

    $Build.CC1 = @{}
    $Build.CC1.RootPath = Join-path $WorkspaceRoot 'Orckestra.StarterSite\CC1'
    $Build.CC1.SourcePath = Join-path $Build.CC1.RootPath 'Source\Composer.CompositeC1'
    $Build.CC1.WebProjectPath = Join-Path $Build.CC1.SourcePath "Composer.CompositeC1.Mvc"
    $Build.CC1.PackagePath = Join-Path $Build.CC1.SourcePath "Package.C1"
    $Build.CC1.FileSystemPublishedWebSite = Join-Path $Build.CC1.SourcePath "_Published\Composer.CompositeC1.Mvc"

    $Build.CC1.Installer = Join-Path $Build.CC1.RootPath "Installer"
    $Build.CC1.InstallerMVC = Join-Path $Build.CC1.Installer "Packages\generic\C1CMS"
	
	$Build.Installer = Join-Path $WorkspaceRoot "Installer"
    $Build.InstallerMVC = Join-Path $Build.Installer "Packages\generic\C1CMS\RefApp"

    $Build.CC1.NugetFilePath = Join-Path $Build.CC1.SourcePath "NugetFiles"

    $Build.CC1.ContentPackageNumber = 200
	
	$Build.CC1.NugetPackagesRepository = Join-Path $Build.CC1.SourcePath "packages"
	
	$Build.VisualStudioVersion = "2019"
}

Task CC1_RestoreNugetPackages {    
    $nugetPackageFolder =  $Build.CC1.NugetPackagesRepository
	if(Test-Path $nugetPackageFolder) {
		if((Get-Item $nugetPackageFolder\* ).Count -gt 0){
			Write-Host "Nuget package not empty, cleaning it right now" -ForegroundColor Yellow
			Try {
			Remove-Item $nugetPackageFolder\* -Recurse -Force | Write-Verbose
			}
			Catch{
				Write-Host "Error while cleaning package folder. You may try to close your vs solution and windows explorer instances" -ForegroundColor Yellow
				BREAK
				}
			}
			Write-Host "Done" -ForegroundColor Green
		}
		

    Get-AllSolutions -RootFolder $Build.CC1.RootPath -ErrorAction SilentlyContinue | Invoke-NugetRestore -VisualStudioVersion $VisualStudioVersion | Write-Verbose
}

Task CC1_BuildAndPackage -depends   CC1_Copy-UiPackageFromNuget,
                                    CC1_Compile-Solution,
                                    CC1_Compile-CorePackage,
                                    CC1_Compile-ContentEN,
                                    CC1_Compile-ContentFR,
                                    CC1_UnitTestBackend,
                                    CC1_Copy-ChangesetIfExists,
                                    CC1_Copy-Packages,
									CC1_Copy-DeploymentFolder
                                            
Task CC1_PublishArtifacts {
    UploadArtifact -ArtifactName "CC1Installer" -ContainerFolder "CC1Installer" `
    -Path $Build.CC1.Installer
}

#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                    CC1_CreateAndPublishNugets
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

Task CC1_PackageNuget {
    
        Get-ChildItem -Path $Build.CC1.NugetFilePath -Filter '*.template.nuspec' | 
        Select-Object -Property FullName -ExpandProperty FullName | 
    
        Transform-Nuspec -NuVersion $nugetVersion -UseSymbols $true |

        Package-Nuget -OutputDirectory $Build.LocalNugetRepository | Write-Verbose
}

#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                    CC1_BuildAndPackage
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

Task CC1_Compile-Solution {    
    Invoke-Msbuild -Project (Join-Path $Build.CC1.SourcePath "Composer.CompositeC1.sln") `
        -Configuration $Configuration `
        -LogsDirectory $Build.CentralLogsFolder `
        -VisualStudioVersion $VisualStudioVersion `
        -MsbuildVerbosity $MsbuildVerbosity
}

Task CC1_UnitTestBackend {
	$TestDllPattern = Join-Path $Build.CC1.SourcePath "**/*.Tests.dll"
	$XmlReportName = Join-Path $Build.CentralTestsFolder "CC1_UnitTestResults.xml"

	$ErrorActionPreference = "Stop"

	Invoke-NUnit -FilePathPatternTestDlls $TestDllPattern -FilePathXmlReport `"/output:$XmlReportName`" -FullFilePathNUnitExe $Build.NUnitExe
}

Task CC1_Compile-CorePackage {    
    $corePackagePath = Join-Path $Build.CC1.PackagePath "Composer.C1.Core"

	# Copy missing items into the package
	Robocopy $Build.CC1.WebProjectPath $corePackagePath "robots*.txt" /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy $Build.CC1.WebProjectPath $corePackagePath "Web.config" /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
    Robocopy $Build.CC1.WebProjectPath $corePackagePath "Global.asax" /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy (Join-Path $Build.CC1.WebProjectPath 'App_Data') $corePackagePath\App_Data /E /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy (Join-Path $Build.CC1.WebProjectPath 'App_Config') $corePackagePath\App_Config /E /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy (Join-Path $Build.CC1.WebProjectPath 'Composite') $corePackagePath\Composite /E /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy (Join-Path $Build.CC1.WebProjectPath 'UI.Package') $corePackagePath\UI.Package /E /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Robocopy (Join-Path $Build.CC1.WebProjectPath 'Views') $corePackagePath\Views /E /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	
	Complete-RobocopyExecution($LASTEXITCODE)
	
	Robocopy (Join-Path $Build.CC1.WebProjectPath 'bin') $corePackagePath\Bin *.dll /XF Composite*.dll /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	Remove-Item $corePackagePath\Bin\Orckestra.Logging.dll -ErrorAction SilentlyContinue
	Remove-Item $corePackagePath\Bin\Microsoft.Extensions.DependencyInjection*.dll
	Remove-Item $corePackagePath\Bin\System.Reactive*.dll -ErrorAction SilentlyContinue
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.EnterpriseLibrary.Common.dll -ErrorAction SilentlyContinue
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.EnterpriseLibrary.Validation.dll -ErrorAction SilentlyContinue
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.EnterpriseLibrary.Logging.dll -ErrorAction SilentlyContinue
	Remove-Item $corePackagePath\Bin\Microsoft.Practices.ObjectBuilder.dll -ErrorAction SilentlyContinue
	Remove-Item $corePackagePath\Bin\Microsoft.Web.Infrastructure.dll -ErrorAction SilentlyContinue

    $packageZipName = Join-Path $Build.CC1.PackagePath "Composer.C1.Core.zip"
    if(Test-Path $packageZipName) {
        Write-verbose "Removing '$packageZipName'"
        Remove-Item $packageZipName -Force
    }
	
    
	& $Build.ZIPExe a -tzip "$packageZipName" -r $corePackagePath\*
    #Compress-FolderToZip -Source "$corePackagePath\" -Destination $packageZipName
}

function Build-ContentPackage($CultureName) {    
	$packageZipName = Join-Path $Build.CC1.PackagePath "Composer.C1.Content.$CultureName.zip"

    if(Test-Path $packageZipName) {
        Write-Host "Removing '$packageZipName'" -ForegroundColor DarkGray
        Remove-Item $packageZipName -Force
    }
    
	& $Build.ZIPExe a -tzip "$packageZipName" -r (Join-Path $Build.CC1.PackagePath "Composer.C1.Content.$CultureName\*") 
    #Compress-FolderToZip -Source (Join-Path $Build.CC1.PackagePath "Composer.C1.Content.$CultureName\") -Destination $packageZipName

}

Task CC1_Compile-ContentEN {
    Build-ContentPackage "EN-CA"
}

Task CC1_Compile-ContentFR {
	Build-ContentPackage "FR-CA"
}

Task CC1_Copy-UiPackageFromNuget {
    Write-Host Join-Path $Build.CC1.NugetPackagesRepository "\Composer.*\"
    $composerNugetPackagePath = Join-Path $Build.CC1.NugetPackagesRepository "\Composer.*\"
    $srcFolder = (gci $composerNugetPackagePath).FullName

    Write-verbose "UI Package Location: $srcFolder"
    Write-Host "Robocopy" (Join-Path $srcFolder 'UI.Package') (Join-Path $Build.CC1.WebProjectPath 'UI.Package') /E

    Robocopy (Join-Path $srcFolder 'UI.Package') (Join-Path $Build.CC1.WebProjectPath 'UI.Package') /E | Write-Verbose

    Complete-RobocopyExecution($LASTEXITCODE)
}

Task CC1_Copy-DeploymentFolder {
    $PackagedWebSite = Join-Path $Build.Installer "Packages\generic\C1CMS\DeploymentFolder.zip"
    
    Robocopy (Join-Path $Build.CC1.WebProjectPath 'Deployment') (Join-Path $Build.CC1.FileSystemPublishedWebSite 'Deployment') *.* /E /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
    
    Invoke-MSDeploy -Source $Build.CC1.FileSystemPublishedWebSite -Destination $PackagedWebSite | Write-Verbose
}

Task CC1_Copy-Packages{
    $destinationPath = $Build.InstallerMVC
	md $destinationPath  -ErrorAction SilentlyContinue
    Robocopy $Build.CC1.PackagePath $destinationPath *.zip /NJH /NDL /NS /NC /NP | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)
	
	write-host "Copying Orckestra.Composer.C1CMS.Queries package file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.C1CMS.Queries.Package\Release\Orckestra.Composer.C1CMS.Queries.zip'
	copy $sourse $destinationPath
	
	write-host "Copying Orckestra.Composer.SEO.Organization package file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.SEO.Organization\Release\Orckestra.Composer.SEO.Organization.zip'
	copy $sourse $destinationPath
	
	write-host "Copying OOrckestra.Composer.SEO.Content Content package file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.SEO.Content\Release\Orckestra.Composer.SEO.Organization.Content.zip'
	copy $sourse $destinationPath
	
	write-host "Copying Orckestra.Composer.Languages package file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.Languages\Release\Orckestra.Composer.Languages.zip'
	copy $sourse $destinationPath
	
	write-host "Copying Orckestra.Composer.Articles file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.Articles.Package\Release\Orckestra.Composer.Articles.zip'
	copy $sourse $destinationPath
	
	write-host "Copying Orckestra.Composer.ContentSearch file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.ContentSearch.Package\Release\Orckestra.Composer.ContentSearch.zip'
	copy $sourse $destinationPath
	
	write-host "Copying Orckestra.Composer.ContentSearch.Content file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.ContentSearch.Content\Release\Orckestra.Composer.ContentSearch.Content.zip'
	copy $sourse $destinationPath

	write-host "Copying Orckestra.Composer.Sitemap file"
	$sourse = Join-Path $Build.CC1.SourcePath 'Orckestra.Composer.Sitemap.Package\Release\Orckestra.Composer.Sitemap.zip'
	copy $sourse $destinationPath
	
	write-host "Done"
}

Task CC1_Copy-ChangesetIfExists{
    $source = Join-Path $Build.CC1.RootPath "Changeset.txt"
	$destination = Join-Path $Build.CC1.FileSystemPublishedWebSite "changeset.txt"
	if((Test-Path -Path $source)) {
        Copy-Item -Path $source -Destination $destination -Force
    }
}
