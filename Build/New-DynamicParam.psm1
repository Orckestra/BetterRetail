<#
    .SYNOPSIS
        Helper function to simplify creating dynamic parameters
    
    .DESCRIPTION
        Helper function to simplify creating dynamic parameters

        Example use cases:
            Include parameters only if your environment dictates it
            Include parameters depending on the value of a user-specified parameter
            Provide tab completion and intellisense for parameters, depending on the environment

        Please keep in mind that all dynamic parameters you create will not have corresponding variables created.
           One of the examples illustrates a generic method for populating appropriate variables from dynamic parameters
           Alternatively, manually reference $PSBoundParameters for the dynamic parameter value
    
    .EXAMPLE
    
        function Test{
            [CmdletBinding()]
            param()
            DynamicParam {
                New-DynamicParameterGroup {
                    New-DynamicParam -Name MyParam |
                        Add-Alias -Alias "Toto" |
                        Add-ValidateSet -ValidateSet 'A','B','C' |
                        Add-ParameterSet -Mandatory -Name 'Execute' -ValueFromPipeline -ValueFromPipelineByName -HelpMessage "A Message" -Position 2
                }
            }
            process{
                $PSBoundParameters.MyParam
            }
        }
        
        Test -MyParam "A"

#>
Function New-DynamicParameterGroup{
    param(
        [Parameter(Mandatory,HelpMessage="Script that return only DynamicParam objects")]
        [ScriptBlock]$Script
    )
    
    $parameters = . $Script
    
    $Dictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
    
    $parameters | % {
        $p = $_
       
        $ParameterAttribute = $p.Attributes | Where-Object { $_.GetType().fullname -eq 'System.Management.Automation.ParameterAttribute' }
        if(-not $ParameterAttribute){
            $p = $p | Add-ParameterSet
        }
        
        $rtp = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameter -ArgumentList @($p.Name, $p.Type, $p.Attributes)

        $Dictionary.Add($p.Name, $rtp)
    }
    
    return $Dictionary
}

function New-DynamicParam{
    param(
        [Parameter(Mandatory,HelpMessage="Name of the parameter to create")]
        [string]$Name,
        [System.Type]$Type = [string]
    )
    
    $param = New-Object -TypeName PSObject
    $param | Add-Member -MemberType NoteProperty -Name Name -Value $Name
    $param | Add-Member -MemberType NoteProperty -Name Type -Value $Type
    $param | Add-Member -MemberType NoteProperty -Name Attributes -Value (New-Object 'Collections.ObjectModel.Collection[System.Attribute]')

    return $param
}

function Add-Alias{
    param(
        [Parameter(Mandatory,ValueFromPipeline)]
        $DynamicParam,
        [Parameter(Mandatory,HelpMessage="Alias for the parameter")]
        [string[]]$Alias
    )    
    
    if($Alias.count -gt 0) {
            $ParamAlias = New-Object System.Management.Automation.AliasAttribute -ArgumentList $Alias
            $DynamicParam.Attributes.Add($ParamAlias)
    }    
    
    return $DynamicParam
}

function Add-ValidateSet{
    param(
        [Parameter(Mandatory,ValueFromPipeline)]
        $DynamicParam,
        [Parameter(Mandatory,HelpMessage="ValidateSet for the parameter")]
        [string[]]$ValidateSet
    )    
    
    $ParamOptions = New-Object System.Management.Automation.ValidateSetAttribute -ArgumentList $ValidateSet
    $DynamicParam.Attributes.Add($ParamOptions)
    
    return $DynamicParam
}

function Add-ParameterSet{
    param(
        [Parameter(Mandatory,ValueFromPipeline)]
        $DynamicParam,
        
        [string]$Name = "__AllParameterSets",
        [int]$Position,
              
        [switch]$Mandatory,
        [switch]$ValueFromPipeline,
        [switch]$ValueFromPipelineByName,
        [string]$HelpMessage
    )
    
    $ParamAttr = New-Object System.Management.Automation.ParameterAttribute
    $ParamAttr.ParameterSetName = $Name
    if($mandatory)
    {
        $ParamAttr.Mandatory = $True
    }
    if($Position -ne $null)
    {
        $ParamAttr.Position=$Position
    }
    if($ValueFromPipeline)
    {
        $ParamAttr.ValueFromPipeline = $True
    }
    if($ValueFromPipelineByPropertyName)
    {
        $ParamAttr.ValueFromPipelineByPropertyName = $True
    }
    if($HelpMessage)
    {
        $ParamAttr.HelpMessage = $HelpMessage
    } 
    
    $DynamicParam.Attributes.Add($ParamAttr)
    
    return $DynamicParam
}