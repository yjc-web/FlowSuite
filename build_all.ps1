$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] Building All Packages" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

# 1. MSBuild
Write-Host "1. Building Release binary (MSBuild)..." -ForegroundColor Yellow
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
if (-not $msbuild) {
    $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
}

& $msbuild OverlayPic.csproj /p:Configuration=Release /t:Rebuild /v:m
if ($LASTEXITCODE -ne 0) {
    Write-Host "MSBuild failed!" -ForegroundColor Red
    exit 1
}

# 2. MSIX
Write-Host "`n2. Packaging MSIX for MS Store..." -ForegroundColor Yellow
& .\build_msix.ps1

# 3. Inno Setup
Write-Host "`n3. Compiling Inno Setup Installer..." -ForegroundColor Yellow
$iscc = "C:\Program Files\Inno Setup 7\ISCC.exe"
if (-not (Test-Path $iscc)) {
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
}
if (Test-Path $iscc) {
    & $iscc "OverlayPic_Installer.iss"
}

# 4. Portable ZIP
Write-Host "`n4. Creating Portable ZIP..." -ForegroundColor Yellow
$portableDir = "dist\OverlayPic_v1.0.4"
if (Test-Path $portableDir) { Remove-Item -Recurse -Force $portableDir }
New-Item -ItemType Directory -Path $portableDir | Out-Null
Copy-Item "bin\Release\OverlayPic.exe" $portableDir
if (Test-Path "사용안내_및_단축키.txt") { Copy-Item "사용안내_및_단축키.txt" $portableDir }
Compress-Archive -Path "$portableDir\*" -DestinationPath "dist\OverlayPic_v1.0.4.zip" -Force
Write-Host "Portable ZIP created." -ForegroundColor Green

Write-Host "`n========================================================" -ForegroundColor Green
Write-Host "  [SUCCESS] All distribution packages created!" -ForegroundColor Green
Write-Host "  1. MS Store MSIX : dist\OverlayPic_v1.0.4.0_Store.msix" -ForegroundColor White
Write-Host "  2. GitHub Setup  : dist\OverlayPic_v1.0.4.0_Setup.exe" -ForegroundColor White
Write-Host "  3. Portable ZIP  : dist\OverlayPic_v1.0.4.zip" -ForegroundColor White
Write-Host "========================================================`n" -ForegroundColor Green