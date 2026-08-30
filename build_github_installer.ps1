$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] Building GitHub Inno Setup Installer" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

# MSBuild
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

# Inno Setup Compiler
Write-Host "`n2. Compiling Inno Setup script..." -ForegroundColor Yellow
$iscc = "C:\Program Files\Inno Setup 7\ISCC.exe"
if (-not (Test-Path $iscc)) {
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
}
if (-not (Test-Path $iscc)) {
    $iscc = (Get-Command iscc.exe -ErrorAction SilentlyContinue).Source
}

if (-not (Test-Path $iscc)) {
    Write-Host "Inno Setup Compiler (ISCC.exe) not found!" -ForegroundColor Red
    exit 1
}

& $iscc "OverlayPic_Installer.iss"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Inno Setup compilation failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================================" -ForegroundColor Green
Write-Host "  [SUCCESS] GitHub Installer setup file created!" -ForegroundColor Green
Write-Host "  Location: dist\OverlayPic_v1.0.2_Setup.exe" -ForegroundColor White
Write-Host "========================================================`n" -ForegroundColor Green