param(
	[Parameter(Mandatory = $true)]$file
)

if (Test-Path $file) {
	Remove-Item $file -Force
}

Get-Date -Format 'yyyy-MM-dd HH:mm' | Set-Content $file