<# 
.SYNOPSIS
Replace "Orckestra.Composer.CompositeC1.Mvc" by a new name on each file and directory name, and each file content, of script directory.

.DESCRIPTION 
Rename all files and directories names, and replace files content, from "ClientName" to NewName in script directory.
Will check if the script is executed in Clientname git repos directory.

If we have files and directories like :
 
 - Orckestra.Composer.CompositeC1.Mvc.txt
 - Orckestra.Composer.CompositeC1.Mvc/MyFile.txt
 - OtherDirectory/MyFileContent.txt with content like 'This is my Orckestra.Composer.CompositeC1.Mvc'

The command ".\Renamer.ps1 -NewText MyRealClientName"

Will do :

 - MyRealClientName.txt
 - MyRealClientName/MyFile.txt
 - OtherDirectory/MyFileContent.txt with content like 'This is my MyRealClientName'

.EXAMPLE

.\Renamer.ps1 -NewText MyRealClientName

#>

param (	
	[Parameter(Position=0, Mandatory=$true, HelpMessage="New name")]
	[string]
	# NewText
	$NewText
)

Push-Location $PSScriptRoot

$StartTime = $(Get-Date)
$directory = (Get-Location)
$baseDir = $directory.Path + "\src\" + $NewText;
$RenamerScriptName = $MyInvocation.MyCommand.Name
[string[]]$Excludes = @('node_modules', 'lib', 'Packages', 'obj', 'bin', $RenamerScriptName )

# This function will check if this script is executed in a ClientName repos
function CheckIfScriptIsInClientNameGitRepos(){
	$checkIsGitRepos = ((git log ClientName.sln) -match "commit")
	$isClientNameGitRepo = ($checkIsGitRepos.Length -ne 0) 

	if(!$isClientNameGitRepo){
		throw "This script expects to be executed in ClientName cloned git repository."
	}
}

# This function will rename recursively Directories, Files and Files contents from "ClientName" to $NewText
function ProcessRecursiveRenaming($subDirectory){	
	$OldText = "Orckestra.Composer.Website"
	Get-ChildItem $subDirectory -Exclude $Excludes | 
	Where-Object { $_.PSIsContainer -or ($_.Name -like -join("*",$OldText,"*")) -or (!$_.PSIsContainer -and (Get-Content($_) | Select-String -pattern $OldText)) } |
	Sort-Object -Property Length -Descending |
	ForEach-Object{
		$Item = Get-Item $_
		$PathRoot = $Item.FullName | Split-Path
		$OldName = $Item.FullName | Split-Path -Leaf
		$NewName = $OldName -replace $OldText, $NewText
		$NewPath = $PathRoot | Join-Path -ChildPath $NewName

		$ShowInGreen = $false

		if($Item.PSIsContainer){
			ProcessRecursiveRenaming($_.FullName)
		}
		else{
			if ((".msi",".dll", ".bak", ".pdb") -notcontains $Item.Extension) {
					(Get-Content $Item) | 
					ForEach-Object {
						$MustChange = ForEach-Object{$_ -match $OldText}
						if($MustChange){
							$ShowInGreen = $true
						}
						$_.replace($OldText, $NewText).replace($NewText + ".dll", $OldText + ".dll")
					} | 
					Set-Content $Item
				}
		}
				
		if ($OldName -ne $NewName) {
			Rename-Item -Path $Item.FullName -NewName $NewPath
			Write-Host $NewPath -ForegroundColor DarkGreen
		}
		else{
			if($ShowInGreen){
				Write-Host $Item.FullName -ForegroundColor DarkGreen
			}
			else{
				Write-Host $Item.FullName
			}
		}
	}
}

#Set the name of the project in an xml file to lower case
function UpdateParametersAllXml(){
	$extensionPath = "$($NewText).Extensions"
	$xmlFilePath = Join-Path $directory "src" | 
		Join-Path -ChildPath $extensionPath | 
		Join-Path -ChildPath "Parameters.All.xml"
	[xml]$xmlParameterFileContent = (Get-Content $xmlFilePath)
	
	$node = $xmlParameterFileContent.parameters.param | Where-Object {$_.name -eq 'company-name'}
	$node.value = $NewText.ToLower()
	
	$xmlParameterFileContent.Save($xmlFilePath)
}

#Fixing known issues after renaming

# After renaming namespaces from "Orckestra.Website.*" into something other
# some classes lose inherited access to the "Orckestra.Website" namespace
function FixUsings(){

    $list = New-Object Collections.Generic.List[String]
    $list.Add($baseDir + "\App_Start\ComposerConfig.cs")
    $list.Add($baseDir + "\Plugin.cs")
    $list.Add($baseDir + "\StartupHandler.cs")
    foreach ($filePath in $list) {
        $content = Get-Content -Path $filePath
        $output = @()
        $output += "using Orckestra.Composer;"
        $output += $content
        Set-Content -Path $filePath -Value $Output
    }
}

# Keeping assemply name as Orckestra.Composer.Website.dll since there are 
# dependencies based on assembly name
function FixAssemblyName(){
	$filePath = $baseDir + "\" + $NewText + ".csproj"
	$content = Get-Content -Path $filePath
	$content = $content -replace "<AssemblyName>(.+?)<\/AssemblyName>", "<AssemblyName>Orckestra.Composer.Website</AssemblyName>"
	Set-Content -Path $filePath -Value $content
}

#Start of process
Write-Host "Start of process" -ForegroundColor Green

#CheckIfScriptIsInClientNameGitRepos

ProcessRecursiveRenaming($directory)

FixUsings

FixAssemblyName

#UpdateParametersAllXml


#End of process
$ElapsedTime = $(Get-Date) - $StartTime
$TotalTime = "{0:HH:mm:ss}" -f ([DATETIME]$ElapsedTime.Ticks)
Write-Host "End of process" -ForegroundColor Green
Write-Host "Rename has been executed in: " $TotalTime -ForegroundColor Green

Pop-Location