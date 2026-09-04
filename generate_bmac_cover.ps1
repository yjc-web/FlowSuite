Add-Type -AssemblyName System.Drawing

# Modern Soft-Light Aesthetic Cover Banner (2400 x 700)
# Perfectly blends with Buy Me a Coffee's White / Light Theme!
$w = 2400
$h = 700
$outPath = "i:\_MyProject\Program\WpfImageEdit\OverlayPic\dist\buymeacoffee_cover.png"

$bmp = New-Object System.Drawing.Bitmap($w, $h)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality

# 1. Soft Light Gradient Background (Matches BMAC White Website Seamlessly)
$rect = New-Object System.Drawing.Rectangle(0, 0, $w, $h)
$c1 = [System.Drawing.Color]::FromArgb(248, 250, 252) # #F8FAFC (Soft Slate White)
$c2 = [System.Drawing.Color]::FromArgb(238, 246, 255) # #EEF6FF (Subtle Sky Blue)
$c3 = [System.Drawing.Color]::FromArgb(254, 249, 195) # #FEF9C3 (Warm Coffee Cream)

$brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, $c1, $c2, [System.Drawing.Drawing2D.LinearGradientMode]::ForwardDiagonal)
$g.FillRectangle($brush, $rect)
$brush.Dispose()

# 2. Warm Sun/Coffee Glow on Right & Soft Blue on Left
$glowBrush1 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(40, 56, 189, 248))
$g.FillEllipse($glowBrush1, 50, -50, 800, 500)
$glowBrush1.Dispose()

$glowBrush2 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(50, 253, 224, 71))
$g.FillEllipse($glowBrush2, 1600, -50, 750, 450)
$glowBrush2.Dispose()

# 3. Elegant Subtle Grid Pattern
$penGrid = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(18, 59, 130, 246), 1.0)
for ($x = 0; $x -lt $w; $x += 60) {
    $g.DrawLine($penGrid, [int]$x, 0, [int]$x, [int]$h)
}
for ($y = 0; $y -lt $h; $y += 60) {
    $g.DrawLine($penGrid, 0, [int]$y, [int]$w, [int]$y)
}
$penGrid.Dispose()

# 4. App Icon (With Real Pin 📌 & Clean Modern Card Shadow)
$iconPath = "C:\Users\yjc\.gemini\antigravity\brain\eed2e06f-1b93-43ed-a7a3-581a1e968930\overlaypic_icon_1787974675823.jpg"
if (Test-Path $iconPath) {
    $iconImg = [System.Drawing.Image]::FromFile($iconPath)
    $iconSize = 180
    $iconX = 160
    $iconY = 60
    
    # Soft Card Shadow
    $shadowBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(25, 15, 23, 42))
    $g.FillRectangle($shadowBrush, $iconX - 8, $iconY + 12, $iconSize + 16, $iconSize + 8)
    $shadowBrush.Dispose()
    
    $g.DrawImage($iconImg, [float]$iconX, [float]$iconY, [float]$iconSize, [float]$iconSize)
    $iconImg.Dispose()
}

# 5. Typography (Dark Slate / High Contrast on Light BG)
$fontTitle = New-Object System.Drawing.Font("Segoe UI", 52, [System.Drawing.FontStyle]::Bold)
$fontBadge = New-Object System.Drawing.Font("Segoe UI", 15, [System.Drawing.FontStyle]::Bold)
$fontSub = New-Object System.Drawing.Font("Segoe UI", 21, [System.Drawing.FontStyle]::Regular)
$fontTag = New-Object System.Drawing.Font("Segoe UI", 15, [System.Drawing.FontStyle]::Bold)

$brushTitle = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(15, 23, 42))   # #0F172A
$brushSub = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(71, 85, 105))    # #475569
$brushBlue = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(37, 99, 235))   # #2563EB

$textX = 380

# Title: "OverlayPic"
$g.DrawString("OverlayPic", $fontTitle, $brushTitle, [float]$textX, 50.0)

# "by FlowSuite" Pill Badge
$badgeX = $textX + 375
$badgeY = 70
$badgeW = 150
$badgeH = 36
$badgeBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(224, 242, 254)) # #E0F2FE
$badgePen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(56, 189, 248), 1.5)
$badgeRect = New-Object System.Drawing.Rectangle([int]$badgeX, [int]$badgeY, [int]$badgeW, [int]$badgeH)
$g.FillRectangle($badgeBg, $badgeRect)
$g.DrawRectangle($badgePen, $badgeRect)
$g.DrawString("by FlowSuite", $fontBadge, $brushBlue, [float]($badgeX + 16), [float]($badgeY + 6))
$badgeBg.Dispose()
$badgePen.Dispose()

# Subtitle
$g.DrawString("Ultra-Lightweight Transparent Screen Overlay Image Viewer", $fontSub, $brushSub, [float]$textX, 140.0)

# 6. Four Feature Pills (Modern Clean Pastel Cards)
$tags = @(
    @{ text = "[Portable] Zero-Install"; fg = [System.Drawing.Color]::FromArgb(180, 83, 9); bg = [System.Drawing.Color]::FromArgb(254, 243, 199); bd = [System.Drawing.Color]::FromArgb(251, 191, 36) },
    @{ text = "[Snipping] Screen Capture"; fg = [System.Drawing.Color]::FromArgb(3, 105, 161); bg = [System.Drawing.Color]::FromArgb(224, 242, 254); bd = [System.Drawing.Color]::FromArgb(56, 189, 248) },
    @{ text = "[Click-Through] Mode"; fg = [System.Drawing.Color]::FromArgb(109, 40, 217); bg = [System.Drawing.Color]::FromArgb(243, 232, 255); bd = [System.Drawing.Color]::FromArgb(192, 132, 252) },
    @{ text = "[Bilingual] EN / KO"; fg = [System.Drawing.Color]::FromArgb(4, 120, 87); bg = [System.Drawing.Color]::FromArgb(209, 250, 229); bd = [System.Drawing.Color]::FromArgb(52, 211, 153) }
)

$pillX = $textX
$pillY = 195
$pillH = 38

foreach ($t in $tags) {
    $tSize = $g.MeasureString($t.text, $fontTag)
    $pillW = $tSize.Width + 24
    
    $pRect = New-Object System.Drawing.Rectangle([int]$pillX, [int]$pillY, [int]$pillW, [int]$pillH)
    $pBg = New-Object System.Drawing.SolidBrush($t.bg)
    $pPen = New-Object System.Drawing.Pen($t.bd, 1.2)
    $pTextBrush = New-Object System.Drawing.SolidBrush($t.fg)
    
    $g.FillRectangle($pBg, $pRect)
    $g.DrawRectangle($pPen, $pRect)
    $g.DrawString($t.text, $fontTag, $pTextBrush, [float]($pillX + 12), [float]($pillY + 6))
    
    $pBg.Dispose()
    $pPen.Dispose()
    $pTextBrush.Dispose()
    
    $pillX += $pillW + 14
}

# 7. Right: Warm Coffee Card (Centered at Right, away from Change Cover button)
$cCardX = 1950
$cCardY = 60
$cCardW = 260
$cCardH = 175

$cCardRect = New-Object System.Drawing.Rectangle($cCardX, $cCardY, $cCardW, $cCardH)
$cCardBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(254, 249, 195)) # Warm cream
$cCardPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(234, 179, 8), 1.5)
$g.FillRectangle($cCardBg, $cCardRect)
$g.DrawRectangle($cCardPen, $cCardRect)
$cCardBg.Dispose()
$cCardPen.Dispose()

# Draw Clean Coffee Cup
$cupBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 255))
$cupPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(203, 213, 225), 2)
$coffeeBrown = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(180, 83, 9))
$steamPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(148, 163, 184), 2.2)

# Saucer
$g.FillEllipse($cupBrush, $cCardX + 80, $cCardY + 95, 100, 12)
$g.DrawEllipse($cupPen, $cCardX + 80, $cCardY + 95, 100, 12)
# Cup Body
$g.FillEllipse($coffeeBrown, $cCardX + 90, $cCardY + 50, 80, 18)
$g.FillRectangle($cupBrush, $cCardX + 90, $cCardY + 58, 80, 30)
$g.FillEllipse($cupBrush, $cCardX + 90, $cCardY + 75, 80, 20)
$g.DrawArc($cupPen, $cCardX + 155, $cCardY + 58, 25, 25, -90, 180)

# Steam
$g.DrawBezier($steamPen, 
    [System.Drawing.Point]::new($cCardX + 115, $cCardY + 45),
    [System.Drawing.Point]::new($cCardX + 110, $cCardY + 35),
    [System.Drawing.Point]::new($cCardX + 120, $cCardY + 25),
    [System.Drawing.Point]::new($cCardX + 115, $cCardY + 15))
$g.DrawBezier($steamPen, 
    [System.Drawing.Point]::new($cCardX + 135, $cCardY + 45),
    [System.Drawing.Point]::new($cCardX + 130, $cCardY + 35),
    [System.Drawing.Point]::new($cCardX + 140, $cCardY + 25),
    [System.Drawing.Point]::new($cCardX + 135, $cCardY + 15))
$g.DrawBezier($steamPen, 
    [System.Drawing.Point]::new($cCardX + 155, $cCardY + 45),
    [System.Drawing.Point]::new($cCardX + 150, $cCardY + 35),
    [System.Drawing.Point]::new($cCardX + 160, $cCardY + 25),
    [System.Drawing.Point]::new($cCardX + 155, $cCardY + 15))

$cupBrush.Dispose()
$cupPen.Dispose()
$coffeeBrown.Dispose()
$steamPen.Dispose()

# Coffee Label
$fontCoffee = New-Object System.Drawing.Font("Segoe UI", 15, [System.Drawing.FontStyle]::Bold)
$brushCoffeeText = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(161, 98, 7))
$cLabel = "Buy me a coffee"
$cLabelSize = $g.MeasureString($cLabel, $fontCoffee)
$g.DrawString($cLabel, $fontCoffee, $brushCoffeeText, [float]($cCardX + ($cCardW - $cLabelSize.Width)/2), [float]($cCardY + 125))

$fontCoffee.Dispose()
$brushCoffeeText.Dispose()

# Cleanup
$fontTitle.Dispose()
$fontBadge.Dispose()
$fontSub.Dispose()
$fontTag.Dispose()
$brushTitle.Dispose()
$brushSub.Dispose()
$brushBlue.Dispose()
$g.Dispose()

$bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

Write-Host "Modern Soft-Light Cover generated cleanly: $outPath"
