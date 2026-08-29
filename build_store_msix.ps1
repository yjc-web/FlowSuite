$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] Building MS Store MSIX Package" -ForegroundColor Cyan
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

# MSIX Packaging
Write-Host "`n2. Packaging MSIX for MS Store..." -ForegroundColor Yellow
& .\build_msix.ps1

Write-Host "`n========================================================" -ForegroundColor Green
Write-Host "  [SUCCESS] MS Store MSIX build completed!" -ForegroundColor Green
Write-Host "  Location: dist\OverlayPic_v1.0.1.0_Store.msix" -ForegroundColor White
Write-Host "========================================================`n" -ForegroundColor Green