# OverlayPic Cross-Platform Build Script (Windows & macOS)
param (
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " OverlayPic Cross-Platform Build Engine" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$appProj = Join-Path $rootDir "src/OverlayPic.App/OverlayPic.App.csproj"
$distDir = Join-Path $rootDir "dist_crossplatform"

if (Test-Path $distDir) {
    Remove-Item -Recurse -Force $distDir
}
New-Item -ItemType Directory -Path $distDir -Force | Out-Null

# 1. Build Windows (win-x64) Single File Executable
Write-Host "`n[1/3] Publishing for Windows (win-x64)..." -ForegroundColor Yellow
$winOut = Join-Path $distDir "windows_x64"
dotnet publish $appProj -c $Configuration -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o $winOut
Write-Host " -> Windows output created at: $winOut" -ForegroundColor Green

# 2. Build macOS Apple Silicon (osx-arm64)
Write-Host "`n[2/3] Publishing for macOS Apple Silicon (osx-arm64)..." -ForegroundColor Yellow
$macArmOut = Join-Path $distDir "macos_arm64"
$appArmBundle = Join-Path $macArmOut "OverlayPic.app"
$appArmContents = Join-Path $appArmBundle "Contents"
$appArmMacOS = Join-Path $appArmContents "MacOS"
$appArmResources = Join-Path $appArmContents "Resources"

New-Item -ItemType Directory -Path $appArmContents -Force | Out-Null
New-Item -ItemType Directory -Path $appArmMacOS -Force | Out-Null
New-Item -ItemType Directory -Path $appArmResources -Force | Out-Null

# Direct publish to Contents/MacOS
dotnet publish $appProj -c $Configuration -r osx-arm64 --self-contained true -o $appArmMacOS

# Copy Info.plist
Copy-Item (Join-Path $rootDir "src/OverlayPic.App/Info.plist") (Join-Path $appArmContents "Info.plist")
Write-Host " -> macOS (Apple Silicon) .app bundle created at: $appArmBundle" -ForegroundColor Green

# 3. Build macOS Intel (osx-x64)
Write-Host "`n[3/3] Publishing for macOS Intel (osx-x64)..." -ForegroundColor Yellow
$macX64Out = Join-Path $distDir "macos_x64"
$appX64Bundle = Join-Path $macX64Out "OverlayPic.app"
$appX64Contents = Join-Path $appX64Bundle "Contents"
$appX64MacOS = Join-Path $appX64Contents "MacOS"
$appX64Resources = Join-Path $appX64Contents "Resources"

New-Item -ItemType Directory -Path $appX64Contents -Force | Out-Null
New-Item -ItemType Directory -Path $appX64MacOS -Force | Out-Null
New-Item -ItemType Directory -Path $appX64Resources -Force | Out-Null

# Direct publish to Contents/MacOS
dotnet publish $appProj -c $Configuration -r osx-x64 --self-contained true -o $appX64MacOS

Copy-Item (Join-Path $rootDir "src/OverlayPic.App/Info.plist") (Join-Path $appX64Contents "Info.plist")
Write-Host " -> macOS (Intel) .app bundle created at: $appX64Bundle" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " All Platforms Published Successfully!" -ForegroundColor Green
Write-Host " Output Directory: $distDir" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
