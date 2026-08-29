#Requires -Version 5.1
<#
.SYNOPSIS
  Aplica migrations EF Core no MySQL Azure.

.PARAMETER ConnectionString
  Connection string completa (SslMode=Required).

.EXAMPLE
  .\apply-migrations-azure.ps1 -ConnectionString "Server=....mysql.database.azure.com;..."
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionString
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$apiDir = Join-Path $root 'Backend\src\IAS.Api'

Write-Host ">> dotnet ef database update..." -ForegroundColor Cyan
Push-Location $apiDir
try {
    $env:IAS_CONNECTION_STRING = $ConnectionString
    dotnet ef database update --project ../IAS.Infrastructure
    if ($LASTEXITCODE -ne 0) { throw "Migration falhou." }
}
finally {
    Pop-Location
    Remove-Item Env:IAS_CONNECTION_STRING -ErrorAction SilentlyContinue
}

Write-Host ">> Migrations aplicadas." -ForegroundColor Green
