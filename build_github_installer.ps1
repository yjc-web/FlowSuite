$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] GitHub 배포용 Setup.exe 빌드 시작" -ForegroundColor Cyan
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

# 2. Inno Setup Compiler
Write-Host "`n2. Inno Setup 인스톨러 컴파일 중..." -ForegroundColor Yellow
$iscc = "C:\Program Files\Inno Setup 7\ISCC.exe"
if (-not (Test-Path $iscc)) {
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
}
if (-not (Test-Path $iscc)) {
    $iscc = (Get-Command iscc.exe -ErrorAction SilentlyContinue).Source
}

if (-not (Test-Path $iscc)) {
    throw "Inno Setup 컴파일러(ISCC.exe)를 찾을 수 없습니다."
}

& $iscc "OverlayPic_Installer.iss"
if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup 컴파일 실패!"
}

Write-Host "`n========================================================" -ForegroundColor Green
Write-Host "  [성공] GitHub 배포용 Setup.exe 생성이 완료되었습니다!" -ForegroundColor Green
Write-Host "  위치: dist\OverlayPic_v1.0.1_Setup.exe" -ForegroundColor White
Write-Host "========================================================`n" -ForegroundColor Green