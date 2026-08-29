# Configura MySQL local (ias_dev), User Secrets e aplica migrations.
# Uso: .\scripts\setup-dev-database.ps1
# Ou com senha: .\scripts\setup-dev-database.ps1 -Password "sua_senha"

param(
    [string]$Password,
    [string]$User = "root",
    [string]$Server = "localhost",
    [int]$Port = 3306,
    [string]$Database = "ias_dev"
)

$ErrorActionPreference = "Stop"
$mysql = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"

if (-not (Test-Path $mysql)) {
    Write-Error "mysql.exe não encontrado em '$mysql'. Ajuste o caminho no script."
}

$backendRoot = Split-Path $PSScriptRoot -Parent
$apiDir = Join-Path $backendRoot "src\IAS.Api"

if ([string]::IsNullOrWhiteSpace($Password)) {
    $secure = Read-Host "Senha do MySQL (usuário $User)" -AsSecureString
    $ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try { $Password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
}

Write-Host "Verificando serviço MySQL..."
$svc = Get-Service MySQL80 -ErrorAction SilentlyContinue
if ($svc -and $svc.Status -ne "Running") {
    Start-Service MySQL80
}

Write-Host "Criando banco $Database (se não existir)..."
$createDb = "CREATE DATABASE IF NOT EXISTS $Database CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
& $mysql -u $User -p$Password -e $createDb
if ($LASTEXITCODE -ne 0) { throw "Falha ao conectar/criar banco. Confira usuário e senha." }

$conn = "Server=$Server;Port=$Port;Database=$Database;User=$User;Password=$Password;"
Write-Host "Gravando User Secrets..."
Push-Location $apiDir
try {
    dotnet user-secrets init | Out-Null
    dotnet user-secrets set "ConnectionStrings:Default" $conn
    Write-Host "Aplicando migrations (dotnet ef database update)..."
    dotnet ef database update --project ../IAS.Infrastructure
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "Pronto. Suba a API com:"
Write-Host "  cd src\IAS.Api"
Write-Host "  dotnet run"
