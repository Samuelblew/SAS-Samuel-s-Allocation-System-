#Requires -Version 5.1
<#
.SYNOPSIS
  Build do frontend e deploy na Azure Static Web Apps.

.PARAMETER ApiUrl
  URL pública da API (App Service), ex.: https://ias-api-demo.azurewebsites.net

.PARAMETER DeploymentToken
  Token de deploy da SWA (Portal → Static Web App → Manage deployment token).

.PARAMETER AppLocation
  Pasta do frontend (default: Frontend).

.EXAMPLE
  .\deploy-frontend-azure.ps1 `
    -ApiUrl "https://ias-api-demo.azurewebsites.net" `
    -DeploymentToken "xxxx"
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$ApiUrl,

    [Parameter(Mandatory = $true)]
    [string]$DeploymentToken,

    [string]$AppLocation = 'Frontend'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$frontendDir = Join-Path $root $AppLocation
$apiUrlClean = $ApiUrl.TrimEnd('/')

Write-Host ">> Build com VITE_API_URL=$apiUrlClean" -ForegroundColor Cyan
Push-Location $frontendDir
try {
    $env:VITE_API_URL = $apiUrlClean
    npm ci
    if ($LASTEXITCODE -ne 0) { throw "npm ci falhou." }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw "npm run build falhou." }

    Write-Host ">> Deploy SWA..." -ForegroundColor Cyan
    npx --yes @azure/static-web-apps-cli@latest deploy ./dist `
        --deployment-token $DeploymentToken `
        --env production
    if ($LASTEXITCODE -ne 0) { throw "swa deploy falhou." }
}
finally {
    Pop-Location
    Remove-Item Env:VITE_API_URL -ErrorAction SilentlyContinue
}

Write-Host ">> Frontend publicado." -ForegroundColor Green
