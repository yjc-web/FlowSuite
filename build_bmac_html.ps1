$base64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes('C:\Users\yjc\.gemini\antigravity\brain\eed2e06f-1b93-43ed-a7a3-581a1e968930\overlaypic_icon_1787974675823.jpg'))

$html = @"
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>OverlayPic - Buy Me a Coffee Optimized Animated Cover</title>
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; }
  
  body {
    background: #020617;
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    overflow: hidden;
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  }

  /* 1400 x 400 Banner with Top-Focused Safe Zone for BMAC White Cards */
  .banner {
    width: 1400px;
    height: 400px;
    position: relative;
    background: linear-gradient(135deg, #070B14 0%, #0F172A 50%, #172554 100%);
    border-radius: 16px;
    overflow: hidden;
    box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.8), 0 0 0 1px rgba(56, 189, 248, 0.25);
    display: flex;
    align-items: flex-start;
    padding: 30px 50px;
    user-select: none;
  }

  /* Twinkling Night Sky Stars Canvas */
  #starsCanvas {
    position: absolute;
    inset: 0;
    width: 1400px;
    height: 400px;
    z-index: 2;
    pointer-events: none;
  }

  /* Animated Tech Grid */
  .grid-bg {
    position: absolute;
    inset: 0;
    background-size: 40px 40px;
    background-image: 
      linear-gradient(to right, rgba(56, 189, 248, 0.05) 1px, transparent 1px),
      linear-gradient(to bottom, rgba(56, 189, 248, 0.05) 1px, transparent 1px);
    animation: moveGrid 10s linear infinite;
    z-index: 1;
  }

  @keyframes moveGrid {
    0% { background-position: 0 0; }
    100% { background-position: 40px 40px; }
  }

  /* Glowing Aurora Background Orbs */
  .aurora-1 {
    position: absolute;
    width: 500px;
    height: 350px;
    left: 20px;
    top: -30px;
    background: radial-gradient(circle, rgba(56, 189, 248, 0.28) 0%, rgba(37, 99, 235, 0.12) 50%, transparent 70%);
    filter: blur(40px);
    animation: pulseAura 4s ease-in-out infinite alternate;
    z-index: 1;
  }

  .aurora-2 {
    position: absolute;
    width: 450px;
    height: 300px;
    right: 40px;
    top: -20px;
    background: radial-gradient(circle, rgba(255, 221, 0, 0.18) 0%, rgba(245, 158, 11, 0.06) 50%, transparent 70%);
    filter: blur(40px);
    animation: pulseAura 5s ease-in-out infinite alternate-reverse;
    z-index: 1;
  }

  @keyframes pulseAura {
    0% { transform: scale(0.85); opacity: 0.6; }
    100% { transform: scale(1.15); opacity: 1; }
  }

  /* Top Left: Real App Icon with Pin & Smooth Floating */
  .icon-wrapper {
    position: relative;
    z-index: 10;
    width: 140px;
    height: 140px;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-right: 30px;
    margin-top: 5px;
    animation: floatIcon 3.5s ease-in-out infinite;
  }

  .icon-img {
    width: 130px;
    height: 130px;
    border-radius: 24px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5), 0 0 35px rgba(56, 189, 248, 0.4);
  }

  @keyframes floatIcon {
    0%, 100% { transform: translateY(0px) rotate(0deg); }
    50% { transform: translateY(-8px) rotate(1deg); }
  }

  /* Center: Content & Typography (Shifted to Upper Half) */
  .content {
    position: relative;
    z-index: 10;
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 6px;
    margin-top: 5px;
  }

  .title-row {
    display: flex;
    align-items: center;
    gap: 14px;
  }

  .title {
    font-size: 44px;
    font-weight: 800;
    color: #FFFFFF;
    letter-spacing: -0.5px;
    text-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
  }

  .badge-dev {
    background: rgba(37, 99, 235, 0.35);
    border: 1px solid rgba(56, 189, 248, 0.5);
    color: #38BDF8;
    font-size: 13px;
    font-weight: 700;
    padding: 4px 12px;
    border-radius: 20px;
    backdrop-filter: blur(8px);
    box-shadow: 0 0 15px rgba(56, 189, 248, 0.2);
  }

  .subtitle {
    font-size: 15.5px;
    color: #94A3B8;
    font-weight: 400;
    margin-bottom: 6px;
  }

  /* Feature Pills with Vector SVG Icons */
  .pills {
    display: flex;
    align-items: center;
    gap: 8px;
    flex-wrap: wrap;
  }

  .pill {
    background: rgba(15, 23, 42, 0.88);
    backdrop-filter: blur(6px);
    padding: 5px 12px;
    border-radius: 8px;
    font-size: 12px;
    font-weight: 700;
    display: flex;
    align-items: center;
    gap: 6px;
  }

  .pill svg {
    width: 13px;
    height: 13px;
    flex-shrink: 0;
  }

  .pill-yellow { border: 1px solid #FCD34D; color: #FCD34D; }
  .pill-yellow svg { fill: #FCD34D; }

  .pill-cyan { border: 1px solid #38BDF8; color: #38BDF8; }
  .pill-cyan svg { fill: #38BDF8; }

  .pill-purple { border: 1px solid #A78BFA; color: #A78BFA; }
  .pill-purple svg { fill: #A78BFA; }

  .pill-green { border: 1px solid #34D399; color: #34D399; }
  .pill-green svg { fill: #34D399; }

  /* Right: Vector SVG Steaming Coffee Cup Card */
  .coffee-card {
    position: relative;
    z-index: 10;
    background: rgba(15, 23, 42, 0.85);
    border: 1.5px solid #FFDD00;
    border-radius: 16px;
    padding: 12px 18px;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 6px;
    margin-top: 5px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.4), 0 0 25px rgba(255, 221, 0, 0.18);
    animation: floatCoffee 3.8s ease-in-out infinite;
  }

  @keyframes floatCoffee {
    0%, 100% { transform: translateY(0px); }
    50% { transform: translateY(-7px); }
  }

  .coffee-svg-wrap {
    position: relative;
    width: 54px;
    height: 46px;
  }

  /* Rising Animated Steam */
  .steam-svg {
    position: absolute;
    top: -12px;
    left: 10px;
    width: 34px;
    height: 22px;
    overflow: visible;
  }

  .steam-line {
    stroke: rgba(255, 255, 255, 0.75);
    stroke-width: 2.2;
    stroke-linecap: round;
    fill: none;
    animation: steamRise 2s ease-in-out infinite alternate;
  }

  .steam-line:nth-child(2) { animation-delay: 0.4s; }
  .steam-line:nth-child(3) { animation-delay: 0.8s; }

  @keyframes steamRise {
    0% { transform: translateY(0px) scaleY(1); opacity: 0.8; }
    100% { transform: translateY(-8px) scaleY(1.3); opacity: 0.15; }
  }

  .coffee-label {
    color: #FFDD00;
    font-size: 12.5px;
    font-weight: 800;
    letter-spacing: 0.3px;
    text-shadow: 0 0 10px rgba(255, 221, 0, 0.3);
  }
</style>
</head>
<body>

<div class="banner">
  <!-- Twinkling Night Sky Stars & Tech Grid -->
  <div class="grid-bg"></div>
  <canvas id="starsCanvas" width="1400" height="400"></canvas>
  <div class="aurora-1"></div>
  <div class="aurora-2"></div>

  <!-- Left: Real App Icon with Pin 📌 -->
  <div class="icon-wrapper">
    <img src="data:image/jpeg;base64,$base64" alt="OverlayPic Icon" class="icon-img">
  </div>

  <!-- Center: Title & Vector SVG Feature Tags (Upper Half Focused) -->
  <div class="content">
    <div class="title-row">
      <div class="title">OverlayPic</div>
      <div class="badge-dev">by FlowSuite</div>
    </div>
    <div class="subtitle">Ultra-Lightweight Transparent Screen Overlay Image Viewer</div>
    <div class="pills">
      <!-- 1. Zero-Install Portable (Lightning SVG) -->
      <div class="pill pill-yellow">
        <svg viewBox="0 0 24 24"><path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/></svg>
        Zero-Install Portable
      </div>
      <!-- 2. Screen Snipping (Scissors SVG) -->
      <div class="pill pill-cyan">
        <svg viewBox="0 0 24 24"><path d="M6 3a3 3 0 1 0 2.83 4H14l5-5 1.41 1.41L15.83 8H18a3 3 0 1 1-2.83 4H14l-5 5-1.41-1.41L12.17 11H8.83A3 3 0 1 0 6 3zm0 2a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0 8a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/></svg>
        Screen Snipping
      </div>
      <!-- 3. Click-Through Mode (Pointer SVG) -->
      <div class="pill pill-purple">
        <svg viewBox="0 0 24 24"><path d="M9 2v13.59l-3.3-3.3-1.41 1.42L9.59 19H18v-2h-6.59l-2-2H13V2H9z"/></svg>
        Click-Through Mode
      </div>
      <!-- 4. Bilingual (Globe SVG) -->
      <div class="pill pill-green">
        <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 17.93c-3.95-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"/></svg>
        Bilingual (EN / KO)
      </div>
    </div>
  </div>

  <!-- Right: Beautiful Vector SVG Coffee Cup Card with Steam -->
  <div class="coffee-card">
    <div class="coffee-svg-wrap">
      <!-- Animated Steam SVG -->
      <svg class="steam-svg" viewBox="0 0 36 24">
        <path class="steam-line" d="M8 22 Q12 14 8 6" />
        <path class="steam-line" d="M18 22 Q22 14 18 6" />
        <path class="steam-line" d="M28 22 Q32 14 28 6" />
      </svg>
      
      <!-- Vector Coffee Cup SVG -->
      <svg viewBox="0 0 64 48" width="54" height="42">
        <!-- Saucer -->
        <ellipse cx="30" cy="42" rx="26" ry="4" fill="#334155" />
        <ellipse cx="30" cy="41" rx="24" ry="3" fill="#475569" />
        <!-- Cup Body -->
        <path d="M10 16 h40 c0 16 -8 24 -20 24 s-20 -8 -20 -24 z" fill="#F1F5F9" />
        <!-- Coffee Surface -->
        <ellipse cx="30" cy="16" rx="19" ry="4.5" fill="#78350F" />
        <ellipse cx="30" cy="16" rx="15" ry="3" fill="#B45309" />
        <!-- Cup Handle -->
        <path d="M48 20 C56 20 56 32 46 32" fill="none" stroke="#F1F5F9" stroke-width="4.5" stroke-linecap="round" />
      </svg>
    </div>
    <div class="coffee-label">Buy me a coffee</div>
  </div>
</div>

<!-- Twinkling Night Sky Stars Canvas Script -->
<script>
const canvas = document.getElementById('starsCanvas');
const ctx = canvas.getContext('2d');
const stars = [];

for (let i = 0; i < 80; i++) {
  stars.push({
    x: Math.random() * 1400,
    y: Math.random() * 400,
    size: Math.random() * 2.2 + 0.6,
    alpha: Math.random() * 0.8 + 0.2,
    speedX: Math.random() * 0.4 + 0.1,
    phase: Math.random() * Math.PI * 2,
    twinkleSpeed: Math.random() * 0.04 + 0.02
  });
}

function animateStars() {
  ctx.clearRect(0, 0, 1400, 400);
  
  stars.forEach(s => {
    s.x += s.speedX;
    if (s.x > 1400) s.x = 0;
    
    s.phase += s.twinkleSpeed;
    const currentAlpha = s.alpha * (0.5 + 0.5 * Math.sin(s.phase));
    
    ctx.fillStyle = `rgba(186, 230, 253, `${currentAlpha})`;
    ctx.beginPath();
    ctx.arc(s.x, s.y, s.size, 0, Math.PI * 2);
    ctx.fill();
    
    // Star sparkle cross for larger stars
    if (s.size > 1.8 && currentAlpha > 0.6) {
      ctx.strokeStyle = `rgba(255, 255, 255, `${currentAlpha * 0.7})`;
      ctx.lineWidth = 0.8;
      ctx.beginPath();
      ctx.moveTo(s.x - 4, s.y);
      ctx.lineTo(s.x + 4, s.y);
      ctx.moveTo(s.x, s.y - 4);
      ctx.lineTo(s.x, s.y + 4);
      ctx.stroke();
    }
  });
  
  requestAnimationFrame(animateStars);
}

animateStars();
</script>

</body>
</html>
"@

[System.IO.File]::WriteAllText("i:\_MyProject\Program\WpfImageEdit\OverlayPic\dist\bmac_cover_animated.html", $html, [System.Text.Encoding]::UTF8)
Write-Host "bmac_cover_animated.html updated for top-focused layout."
