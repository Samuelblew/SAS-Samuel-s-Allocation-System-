#Requires -Version 5.1
<#
.SYNOPSIS
  Publica a API IAS no Azure App Service (zip deploy).

.PARAMETER AppName
  Nome do Web App (ex.: ias-api-demo).

.PARAMETER ResourceGroup
  Resource group do App Service.

.EXAMPLE
  .\deploy-api-azure.ps1 -ResourceGroup rg-ias-demo -AppName ias-api-demo
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory = $true)]
    [string]$AppName
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$apiDir = Join-Path $root 'Backend\src\IAS.Api'
$publishDir = Join-Path $env:TEMP "ias-api-publish-$(Get-Date -Format 'yyyyMMddHHmmss')"
$zipPath = Join-Path $env:TEMP "ias-api.zip"

Write-Host ">> dotnet publish (Release)..." -ForegroundColor Cyan
Push-Location $apiDir
try {
    dotnet publish -c Release -o $publishDir
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish falhou." }
}
finally {
    Pop-Location
}

if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Write-Host ">> Criando zip..." -ForegroundColor Cyan
Compress-Archive -Path (Join-Path $publishDir '*') -DestinationPath $zipPath

Write-Host ">> Deploy para App Service '$AppName'..." -ForegroundColor Cyan
az webapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $AppName `
    --src $zipPath

Write-Host ">> Concluído. Health: https://$AppName.azurewebsites.net/api/v1/health" -ForegroundColor Green
