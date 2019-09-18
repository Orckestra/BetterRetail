$VisualStudioVersion                      = '2017'

Task CF -depends CF_RestoreNugetPackages,
                 CF_BuildAndPackage,
                 CF_PackageNuget
				 
Task CFDev -depends CF_CompileBackend,
					CF_PackageNuget

Task CF_RestoreNugetPackages {
    Get-AllSolutions -RootFolder $Build.CF.RootPath -ErrorAction SilentlyContinue | Invoke-NugetRestore | Write-Verbose
}

Task CF_BuildAndPackage -depends CF_CompileFrontend,
                                 CF_CompileBackend,
								 CF_UnitTestBackend
                              

Task CF_PublishArtifacts{
    UploadArtifact -ArtifactName "CFInstaller" -ContainerFolder "CFInstaller" `
    -Path $Build.CF.Installer
}

function CF_InitializeVariables {

	$Build.CF = @{}
	$Build.CF.RootPath = Join-path $Build.WorkspaceRoot 'Orckestra.StarterSite\CF'
	$Build.CF.SourcePath = Join-path $Build.CF.RootPath 'Source'
	$Build.CF.NugetFilePath = Join-Path $Build.CF.SourcePath "NugetFiles"
	$Build.CF.Installer = Join-Path $Build.CF.RootPath "Installer"


}

#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                    CF_BuildAndPackage
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

Task CF_CompileFrontend {
	
	Push-Location -path $Build.CF.SourcePath
	try{		
		write-verbose "Cleaning node_modules folders if existing..."
		Remove-LongDirectoryStructure -Path (Join-Path $Build.CF.SourcePath 'node_modules/') > $null

		write-verbose "Install local node deps..."
		
		write-verbose "npm prune..."
		npm prune
		write-verbose "npm install..."
		
		$npmLogsPath = (Join-Path $Build.CentralLogsFolder 'CF_npmLogs.txt')

		Exec {
			$ErrorActionPreference = 'Continue'
			npm install --msvs_version=2015 *> $npmLogsPath
			$LASTEXITCODE = 0
		}
		
		write-verbose "Executing Gulp Package"
		npm run package --release | Write-Verbose
	}
	finally{
		Pop-Location
	}
}

Task CF_UnitTestsFrontend {
	write-verbose "Running Unit tests"
	
	$tsUnitTestsPath = (Join-Path $Build.CentralTestsFolder 'CF_tsUnitTests.karmalog')
	
	Exec {
		$ErrorActionPreference = 'Continue'
        cd $Build.CF.SourcePath
		npm run unitTests *>&1 | Tee-Object $tsUnitTestsPath | Write-Verbose
		if($LASTEXITCODE -ne 0)
		{
			Throw "Frontend unit tests failed. See log files for more informations: $tsUnitTestsPath"
		}
	}
	robocopy (Join-Path $Build.CF.SourcePath .temp\Tests\test-results\) (Join-Path $Build.CF.RootPath 'KarmaTests') karma.junit.xml /MIR | Write-Verbose
	Complete-RobocopyExecution($LASTEXITCODE)

	$logs = Get-Content $tsUnitTestsPath
	#$logs | Write-Verbose
	if (($logs -clike "*error TS*").Count -gt 0) { throw "'error TSxxxxxx' keyword found" }
	if (($logs -clike "TypeError:*").Count -gt 0) { throw "'TypeError' keyword found" }
	
}

Task CF_CompileBackend {
    Invoke-Msbuild -Project (Join-Path $Build.CF.SourcePath "Composer.sln") `
	 -Configuration $Configuration `
	 -LogsDirectory $Build.CentralLogsFolder `
	 -VisualStudioVersion $VisualStudioVersion `
	 -MsbuildVerbosity $MsbuildVerbosity
 }

Task CF_UnitTestBackend {
	$TestDllPattern = Join-Path $Build.CF.SourcePath "**/*.Tests.dll"
	$XmlReportName = Join-Path $Build.CentralTestsFolder "CF_UnitTestResults.xml"

	$ErrorActionPreference = "Stop"

	Invoke-NUnit -FilePathPatternTestDlls $TestDllPattern -FilePathXmlReport `"/xml=$XmlReportName`" -FullFilePathNUnitExe $Build.NUnitExe
}


#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#
#                                    CF_CreateAndPublishNugets
#
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------
#---------------------------------------------------------------------------------------

Task CF_PackageNuget {

		$nugetVersion = Get-Version-Legacy

        Get-ChildItem -Path $Build.CF.NugetFilePath -Filter '*.template.nuspec' | 

        Select-Object -Property FullName -ExpandProperty FullName | 
		
        Transform-Nuspec -NuVersion $nugetVersion -UseSymbols $true |

        Package-Nuget -OutputDirectory $Build.LocalNugetRepository | Write-Verbose

}
