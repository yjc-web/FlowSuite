$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] 전체 배포 패키지 빌드 시작" -ForegroundColor Cyan
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

# 2. MSIX & Inno Setup
Write-Host "`n2. MSIX 및 Inno Setup 패키징 중..." -ForegroundColor Yellow
& .\build_msix.ps1

# 3. Portable ZIP
Write-Host "`n3. 포터블 ZIP 압축 생성 중..." -ForegroundColor Yellow
$portableDir = "dist\OverlayPic_v1.0.1"
if (Test-Path $portableDir) { Remove-Item -Recurse -Force $portableDir }
New-Item -ItemType Directory -Path $portableDir | Out-Null
Copy-Item "bin\Release\OverlayPic.exe" $portableDir
if (Test-Path "사용안내_및_단축키.txt") { Copy-Item "사용안내_및_단축키.txt" $portableDir }
Compress-Archive -Path "$portableDir\*" -DestinationPath "dist\OverlayPic_v1.0.1.zip" -Force
Write-Host "포터블 ZIP 생성 완료." -ForegroundColor Green

Write-Host "`n========================================================" -ForegroundColor Green
Write-Host "  [성공] 모든 배포 패키지 생성이 완료되었습니다!" -ForegroundColor Green
Write-Host "  1. MS Store용 MSIX : dist\OverlayPic_v1.0.1.0_Store.msix" -ForegroundColor White
Write-Host "  2. GitHub 설치파일  : dist\OverlayPic_v1.0.1_Setup.exe" -ForegroundColor White
Write-Host "  3. 포터블 ZIP 압축 : dist\OverlayPic_v1.0.1.zip" -ForegroundColor White
Write-Host "========================================================`n" -ForegroundColor Green