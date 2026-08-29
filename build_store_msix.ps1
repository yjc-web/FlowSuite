$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] MS Store용 MSIX 패키지 빌드 시작" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

# 1. Build Release binary
Write-Host "1. Release 바이너리 빌드 중 (MSBuild)..." -ForegroundColor Yellow
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
if (-not $msbuild) {
    $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
}

& $msbuild OverlayPic.csproj /p:Configuration=Release /t:Rebuild /v:m
if ($LASTEXITCODE -ne 0) {
    throw "MSBuild 빌드 실패!"
}

# 2. Package MSIX
Write-Host "`n2. MS Store용 MSIX 패키징 중..." -ForegroundColor Yellow
& .\build_msix.ps1

Write-Host "`n========================================================" -ForegroundColor Green
Write-Host "  [성공] MS Store용 MSIX 패키지 생성이 완료되었습니다!" -ForegroundColor Green
Write-Host "  위치: dist\OverlayPic_v1.0.1.0_Store.msix" -ForegroundColor White
Write-Host "========================================================`n" -ForegroundColor Green