param(
    [string]$PackageName = "FlowSuiteYJC.OverlayPic",
    [string]$Version = "1.0.2.0",
    [string]$Publisher = "CN=8EFD2B12-10C1-4962-8D35-5EFCA941AC84",
    [string]$PublisherDisplayName = "FlowSuiteYJC",
    [string]$DisplayName = "OverlayPic"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "=== MSIX Packaging for MS Store: $PackageName v$Version ===" -ForegroundColor Cyan

# 1. Paths
$makeAppx = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\makeappx.exe"
$signTool = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\signtool.exe"

$buildLayout = Join-Path $scriptDir "msix_layout"
$assetsDir = Join-Path $buildLayout "Assets"
$distDir = Join-Path $scriptDir "dist"
$msixPath = Join-Path $distDir "OverlayPic_v$Version.msix"

if (Test-Path $buildLayout) { Remove-Item -Recurse -Force $buildLayout }
if (!(Test-Path $distDir)) { New-Item -ItemType Directory -Path $distDir }

New-Item -ItemType Directory -Path $buildLayout | Out-Null
New-Item -ItemType Directory -Path $assetsDir | Out-Null

# 2. Copy binaries
Copy-Item "bin\Release\OverlayPic.exe" $buildLayout
if (Test-Path "bin\Release\OverlayPic.pdb") { Copy-Item "bin\Release\OverlayPic.pdb" $buildLayout }

# 3. Generate Asset Logos from icon source
Add-Type -AssemblyName System.Drawing

$srcIconPath = "dist\store_assets\AppIcon_300x300.png"
if (Test-Path $srcIconPath) {
    $srcImg = [System.Drawing.Image]::FromFile((Convert-Path $srcIconPath))
} else {
    $ico = New-Object System.Drawing.Icon((Convert-Path "Resources\app.ico"))
    $srcImg = $ico.ToBitmap()
    $ico.Dispose()
}

function Save-ResizedImage($img, $w, $h, $dest) {
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.DrawImage($img, 0, 0, $w, $h)
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $bmp.Dispose()
}

Save-ResizedImage $srcImg 44 44 (Join-Path $assetsDir "Square44x44Logo.png")
Save-ResizedImage $srcImg 150 150 (Join-Path $assetsDir "Square150x150Logo.png")
Save-ResizedImage $srcImg 50 50 (Join-Path $assetsDir "StoreLogo.png")
Save-ResizedImage $srcImg 310 150 (Join-Path $assetsDir "Wide310x150Logo.png")
Save-ResizedImage $srcImg 44 44 (Join-Path $assetsDir "SmallTile.png")
Save-ResizedImage $srcImg 150 150 (Join-Path $assetsDir "MediumTile.png")

$srcImg.Dispose()

Write-Host "Assets generated successfully." -ForegroundColor Green

# 4. Create AppxManifest.xml
$manifestContent = @"
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">

  <Identity
    Name="$PackageName"
    Publisher="$Publisher"
    Version="$Version"
    ProcessorArchitecture="neutral" />

  <Properties>
    <DisplayName>$DisplayName</DisplayName>
    <PublisherDisplayName>$PublisherDisplayName</PublisherDisplayName>
    <Logo>Assets\StoreLogo.png</Logo>
    <Description>Lightweight Screen Overlay Transparent Image Viewer</Description>
  </Properties>

  <Resources>
    <Resource Language="ko-KR" />
    <Resource Language="en-US" />
  </Resources>

  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.22621.0" />
  </Dependencies>

  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>

  <Applications>
    <Application
      Id="OverlayPicApp"
      Executable="OverlayPic.exe"
      EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements
        DisplayName="OverlayPic"
        Description="Screen Overlay Transparent Image Viewer"
        BackgroundColor="#0F172A"
        Square150x150Logo="Assets\Square150x150Logo.png"
        Square44x44Logo="Assets\Square44x44Logo.png">
        <uap:DefaultTile Wide310x150Logo="Assets\Wide310x150Logo.png" Square71x71Logo="Assets\SmallTile.png" />
      </uap:VisualElements>
    </Application>
  </Applications>

</Package>
"@

$manifestPath = Join-Path $buildLayout "AppxManifest.xml"
[System.IO.File]::WriteAllText($manifestPath, $manifestContent, [System.Text.Encoding]::UTF8)

Write-Host "AppxManifest.xml created." -ForegroundColor Green

# 5. Pack Store MSIX (Unsigned)
$storeMsixPath = Join-Path $distDir "OverlayPic_v$Version`_Store.msix"
$sideloadMsixPath = Join-Path $distDir "OverlayPic_v$Version`_Sideload.msix"

if (Test-Path $storeMsixPath) { Remove-Item -Force $storeMsixPath }
if (Test-Path $sideloadMsixPath) { Remove-Item -Force $sideloadMsixPath }

Write-Host "Packing Unsigned MSIX for Microsoft Store..." -ForegroundColor Yellow
& $makeAppx pack /d $buildLayout /p $storeMsixPath /nv

if (!(Test-Path $storeMsixPath)) {
    throw "MakeAppx failed to create MSIX package."
}

Copy-Item $storeMsixPath $sideloadMsixPath
Copy-Item $storeMsixPath $msixPath -Force

Write-Host "Store MSIX package (Unsigned) created: $storeMsixPath" -ForegroundColor Green

# 6. Sign Sideload version
$certPath = Join-Path $distDir "OverlayPic_Dev.pfx"
$cerPath = Join-Path $distDir "OverlayPic_InstallCert.cer"
$pfxPassword = ConvertTo-SecureString "1234" -AsPlainText -Force

Write-Host "Creating/Exporting Certificate for $Publisher..." -ForegroundColor Yellow

$existingCert = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -eq $Publisher } | Select-Object -First 1

if (-not $existingCert) {
    $newCert = New-SelfSignedCertificate -Type Custom `
        -Subject $Publisher `
        -KeyUsage DigitalSignature `
        -FriendlyName "OverlayPic Developer Certificate" `
        -CertStoreLocation "Cert:\CurrentUser\My" `
        -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3")
    
    $existingCert = $newCert
}

Export-PfxCertificate -Cert $existingCert -FilePath $certPath -Password $pfxPassword | Out-Null
Export-Certificate -Cert $existingCert -FilePath $cerPath | Out-Null

Write-Host "Signing Sideload MSIX package for local installation..." -ForegroundColor Yellow
& $signTool sign /fd SHA256 /a /f $certPath /p "1234" $sideloadMsixPath

Write-Host "`n=== Packaging Complete ===" -ForegroundColor Cyan
Write-Host "[MS Store MSIX]     : $storeMsixPath" -ForegroundColor Green
Write-Host "[Local Sideload MSIX]: $sideloadMsixPath" -ForegroundColor Yellow
Write-Host "[Local Certificate]  : $cerPath" -ForegroundColor Gray