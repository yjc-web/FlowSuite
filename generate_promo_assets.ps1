Add-Type -AssemblyName System.Drawing

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

$outDir = Join-Path $scriptDir "dist\store_assets"
if (!(Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir | Out-Null
}

$iconSrcPath = Join-Path $outDir "BoxArt_1080x1080.png"
if (-not (Test-Path $iconSrcPath)) {
    $iconSrcPath = Join-Path $outDir "AppIcon_300x300.png"
}
$srcIcon = [System.Drawing.Bitmap]::FromFile((Convert-Path $iconSrcPath))

# Sample exact background color from icon corner
$bgBase = $srcIcon.GetPixel(5, 5)
$cBgDark = [System.Drawing.Color]::FromArgb($bgBase.R, $bgBase.G, $bgBase.B)
$cWhite = [System.Drawing.Color]::FromArgb(255, 255, 255)
$cSky = [System.Drawing.Color]::FromArgb(56, 189, 248)
$cGray = [System.Drawing.Color]::FromArgb(156, 174, 204)

function Init-HQGraphics($g) {
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
}

# Helper to draw icon with smooth edge feathering
function Draw-FeatheredImage($gTarget, $img, $destX, $destY, $size) {
    $tempBmp = New-Object System.Drawing.Bitmap($size, $size)
    $tg = [System.Drawing.Graphics]::FromImage($tempBmp)
    Init-HQGraphics $tg
    $tg.DrawImage($img, 0, 0, $size, $size)
    $tg.Dispose()

    # Feather outer 20% edges to blend seamlessly into background
    $radius = $size / 2.0
    $featherStart = $radius * 0.72

    for ($y = 0; $y -lt $size; $y++) {
        for ($x = 0; $x -lt $size; $x++) {
            $dx = $x - $radius
            $dy = $y - $radius
            $dist = [Math]::Sqrt($dx * $dx + $dy * $dy)
            if ($dist -gt $featherStart) {
                $p = $tempBmp.GetPixel($x, $y)
                $factor = [Math]::Max(0.0, [Math]::Min(1.0, 1.0 - (($dist - $featherStart) / ($radius - $featherStart))))
                $newA = [int]($p.A * $factor)
                $tempBmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($newA, $p.R, $p.G, $p.B))
            }
        }
    }

    $gTarget.DrawImage($tempBmp, [float]$destX, [float]$destY, [float]$size, [float]$size)
    $tempBmp.Dispose()
}

# 1. 16:9 Super Hero Art & Title Hero Art (1920 x 1080)
function Generate-HeroArt($destPath) {
    $w = 1920
    $h = 1080
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    Init-HQGraphics $g

    # Fill base dark background
    $brushBg = New-Object System.Drawing.SolidBrush($cBgDark)
    $g.FillRectangle($brushBg, 0, 0, $w, $h)
    $brushBg.Dispose()

    # Soft glowing background lights
    $glowLeft = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(30, 37, 99, 235))
    $g.FillEllipse($glowLeft, 60, 100, 750, 750)
    $glowLeft.Dispose()

    $glowRight = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(20, 56, 189, 248))
    $g.FillEllipse($glowRight, 950, 180, 850, 650)
    $glowRight.Dispose()

    # Draw feathered Icon on left
    $iconSize = 640
    $iconX = 140
    $iconY = ($h - $iconSize) / 2
    Draw-FeatheredImage $g $srcIcon $iconX $iconY $iconSize

    # Typography
    $fontFamily = "Segoe UI"
    $titleFont = New-Object System.Drawing.Font($fontFamily, 68, [System.Drawing.FontStyle]::Bold)
    $subFont = New-Object System.Drawing.Font($fontFamily, 28, [System.Drawing.FontStyle]::Bold)
    $descFont = New-Object System.Drawing.Font($fontFamily, 20, [System.Drawing.FontStyle]::Regular)
    $badgeFont = New-Object System.Drawing.Font($fontFamily, 14, [System.Drawing.FontStyle]::Bold)

    $textX = 850

    # Title
    $brushTitle = New-Object System.Drawing.SolidBrush($cWhite)
    $g.DrawString("OverlayPic", $titleFont, $brushTitle, [float]$textX, 330)
    $brushTitle.Dispose()

    # Subtitle
    $brushSky = New-Object System.Drawing.SolidBrush($cSky)
    $g.DrawString("Screen Overlay Transparent Image Viewer", $subFont, $brushSky, [float]$textX, 450)
    $brushSky.Dispose()

    # Tagline
    $brushGray = New-Object System.Drawing.SolidBrush($cGray)
    $g.DrawString("Ultra-lightweight | Click-Through Mode | Screen Snipping & Image Export", $descFont, $brushGray, [float]$textX, 525)
    $brushGray.Dispose()

    # Feature badges
    $badges = @("PORTABLE", "CLICK-THROUGH", "SNIPPING (CTRL+ALT+X)", "SAVE (CTRL+S)")
    $badgeX = $textX
    $badgeY = 620
    foreach ($badge in $badges) {
        $size = $g.MeasureString($badge, $badgeFont)
        $bw = $size.Width + 24
        $bh = $size.Height + 12

        $badgeBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(40, 56, 189, 248))
        $badgePen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(100, 56, 189, 248), 1.5)
        
        $bRect = New-Object System.Drawing.RectangleF([float]$badgeX, [float]$badgeY, [float]$bw, [float]$bh)
        $g.FillRectangle($badgeBg, $bRect)
        $g.DrawRectangle($badgePen, $bRect.X, $bRect.Y, $bRect.Width, $bRect.Height)

        $bTextBrush = New-Object System.Drawing.SolidBrush($cSky)
        $g.DrawString($badge, $badgeFont, $bTextBrush, [float]($badgeX + 12), [float]($badgeY + 6))

        $badgeBg.Dispose()
        $badgePen.Dispose()
        $bTextBrush.Dispose()

        $badgeX += $bw + 12
    }

    # Clean up
    $titleFont.Dispose()
    $subFont.Dispose()
    $descFont.Dispose()
    $badgeFont.Dispose()
    $g.Dispose()

    $bmp.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Created: $destPath ($w x $h)" -ForegroundColor Green
}

# 2. Brand Key Art (584 x 800) - Portrait for Xbox & Store
function Generate-BrandKeyArt($destPath) {
    $w = 584
    $h = 800
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    Init-HQGraphics $g

    # Background
    $brushBg = New-Object System.Drawing.SolidBrush($cBgDark)
    $g.FillRectangle($brushBg, 0, 0, $w, $h)
    $brushBg.Dispose()

    # Ambient glow in center
    $glowBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(40, 56, 189, 248))
    $g.FillEllipse($glowBrush, 42, 80, 500, 500)
    $glowBrush.Dispose()

    # App Icon centered with feathering
    $iconSize = 390
    $iconX = ($w - $iconSize) / 2
    $iconY = 85
    Draw-FeatheredImage $g $srcIcon $iconX $iconY $iconSize

    # Typography
    $fontFamily = "Segoe UI"
    $titleFont = New-Object System.Drawing.Font($fontFamily, 40, [System.Drawing.FontStyle]::Bold)
    $subFont = New-Object System.Drawing.Font($fontFamily, 15, [System.Drawing.FontStyle]::Bold)
    $taglineFont = New-Object System.Drawing.Font($fontFamily, 13, [System.Drawing.FontStyle]::Regular)

    $formatCenter = New-Object System.Drawing.StringFormat
    $formatCenter.Alignment = [System.Drawing.StringAlignment]::Center

    # Title
    $brushTitle = New-Object System.Drawing.SolidBrush($cWhite)
    $g.DrawString("OverlayPic", $titleFont, $brushTitle, [float]($w / 2), 500, $formatCenter)
    $brushTitle.Dispose()

    # Subtitle
    $brushSky = New-Object System.Drawing.SolidBrush($cSky)
    $g.DrawString("Transparent Overlay Image Viewer", $subFont, $brushSky, [float]($w / 2), 570, $formatCenter)
    $brushSky.Dispose()

    # Tagline
    $brushGray = New-Object System.Drawing.SolidBrush($cGray)
    $g.DrawString("Ultra-lightweight | Click-Through | Portable", $taglineFont, $brushGray, [float]($w / 2), 620, $formatCenter)
    $g.DrawString("Screen Snipping & Image Overlay Tool", $taglineFont, $brushGray, [float]($w / 2), 648, $formatCenter)
    $brushGray.Dispose()

    # Clean up
    $titleFont.Dispose()
    $subFont.Dispose()
    $taglineFont.Dispose()
    $formatCenter.Dispose()
    $g.Dispose()

    $bmp.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Created: $destPath ($w x $h)" -ForegroundColor Green
}

# 3. Featured Promotional Square Art (1080 x 1080)
function Generate-FeaturedSquareArt($destPath) {
    $w = 1080
    $h = 1080
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    Init-HQGraphics $g

    # Draw full BoxArt directly
    $g.DrawImage($srcIcon, 0, 0, $w, $h)

    # Typography on lower portion
    $fontFamily = "Segoe UI"
    $titleFont = New-Object System.Drawing.Font($fontFamily, 54, [System.Drawing.FontStyle]::Bold)
    $subFont = New-Object System.Drawing.Font($fontFamily, 22, [System.Drawing.FontStyle]::Bold)

    $formatCenter = New-Object System.Drawing.StringFormat
    $formatCenter.Alignment = [System.Drawing.StringAlignment]::Center

    $brushTitle = New-Object System.Drawing.SolidBrush($cWhite)
    $g.DrawString("OverlayPic", $titleFont, $brushTitle, [float]($w / 2), 850, $formatCenter)
    $brushTitle.Dispose()

    $brushSky = New-Object System.Drawing.SolidBrush($cSky)
    $g.DrawString("Transparent Overlay Image Viewer", $subFont, $brushSky, [float]($w / 2), 935, $formatCenter)
    $brushSky.Dispose()

    $titleFont.Dispose()
    $subFont.Dispose()
    $formatCenter.Dispose()
    $g.Dispose()

    $bmp.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Created: $destPath ($w x $h)" -ForegroundColor Green
}

# --- Execute generation ---
Write-Host "Generating Seamless Microsoft Store & Xbox Promo Assets..." -ForegroundColor Cyan

# 1. 16:9 Super Hero Art (1920x1080)
$hero1Path = Join-Path $outDir "SuperHeroArt_1920x1080.png"
Generate-HeroArt $hero1Path

# 2. Title Hero Art (1920x1080)
$hero2Path = Join-Path $outDir "TitleHeroArt_1920x1080.png"
Generate-HeroArt $hero2Path

# 3. Brand Key Art (584x800)
$brandKeyPath = Join-Path $outDir "BrandKeyArt_584x800.png"
Generate-BrandKeyArt $brandKeyPath

# 4. Featured Promotional Square Art (1080x1080)
$featuredSquarePath = Join-Path $outDir "FeaturedSquareArt_1080x1080.png"
Generate-FeaturedSquareArt $featuredSquarePath

$srcIcon.Dispose()
Write-Host "`nAll 4 Promotional Assets created successfully in '$outDir'!" -ForegroundColor Green
