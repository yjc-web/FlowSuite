Add-Type -AssemblyName System.Drawing

$srcPath = "C:\Users\yjc\.gemini\antigravity\brain\eed2e06f-1b93-43ed-a7a3-581a1e968930\overlaypic_icon_1787974675823.jpg"
$outDir = "i:\_MyProject\Program\WpfImageEdit\OverlayPic\dist\store_assets"

if (!(Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir | Out-Null
}

$srcImg = [System.Drawing.Image]::FromFile($srcPath)

function Create-StoreImage($w, $h, $dest, $bgFill = $true) {
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality

    if ($bgFill) {
        # Fill dark navy background #0F172A
        $brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(15, 23, 42))
        $g.FillRectangle($brush, 0, 0, $w, $h)
        $brush.Dispose()
    }

    # Center the square icon
    $iconSize = [Math]::Min($w, $h)
    $destX = ($w - $iconSize) / 2
    $destY = ($h - $iconSize) / 2

    $g.DrawImage($srcImg, [float]$destX, [float]$destY, [float]$iconSize, [float]$iconSize)
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $bmp.Dispose()
    Write-Host "Created: $dest ($w x $h)"
}

# 1. 1:1 Box Art (1080 x 1080)
Create-StoreImage 1080 1080 (Join-Path $outDir "BoxArt_1080x1080.png")

# 2. 9:16 Poster Art (720 x 1080)
Create-StoreImage 720 1080 (Join-Path $outDir "PosterArt_720x1080.png")

# 3. 1:1 App Icon (300 x 300)
Create-StoreImage 300 300 (Join-Path $outDir "AppIcon_300x300.png")

# 4. 1:1 App Icon (150 x 150)
Create-StoreImage 150 150 (Join-Path $outDir "AppIcon_150x150.png")

# 5. 1:1 App Icon (71 x 71)
Create-StoreImage 71 71 (Join-Path $outDir "AppIcon_71x71.png")

$srcImg.Dispose()
Write-Host "All Store assets created successfully in $outDir"
