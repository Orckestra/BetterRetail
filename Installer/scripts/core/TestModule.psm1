#requires -version 3.0


function Test-Url {
	param(
	[Parameter(Mandatory=$true)][string]$Url
	)
	try
	{
		$HTTP = new-object -com msxml2.xmlhttp
		$HTTP.open("GET",$Url,$false)
		$HTTP.send()
	}
	catch
	{
		$HTTP = @{}
		$HTTP.statusText = "Invalid URL"
		$HTTP.status = 404
	}
	return $HTTP
}

