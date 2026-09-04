$base64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes('C:\Users\yjc\.gemini\antigravity\brain\eed2e06f-1b93-43ed-a7a3-581a1e968930\overlaypic_icon_1787974675823.jpg'))

$html = @"
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>OverlayPic - Windows 11 1080p Cinematic Promo Showcase</title>
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; }
  
  html, body {
    width: 100vw;
    height: 100vh;
    background: #000000;
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
    font-family: "Segoe UI", -apple-system, BlinkMacSystemFont, Roboto, sans-serif;
  }

  /* 1920 x 1080 Full HD Responsive Scaled Frame */
  #scaleWrapper {
    width: 1920px;
    height: 1080px;
    position: absolute;
    transform-origin: center center;
  }

  #videoContainer {
    width: 1920px;
    height: 1080px;
    position: relative;
    background: radial-gradient(circle at center, #0F172A 0%, #020617 100%);
    overflow: hidden;
    box-shadow: 0 0 120px rgba(0, 0, 0, 0.9);
    user-select: none;
  }

  /* Background Tech Grid & Stars */
  .tech-grid {
    position: absolute;
    inset: 0;
    background-size: 60px 60px;
    background-image: 
      linear-gradient(to right, rgba(56, 189, 248, 0.06) 1px, transparent 1px),
      linear-gradient(to bottom, rgba(56, 189, 248, 0.06) 1px, transparent 1px);
    animation: gridFlow 15s linear infinite;
  }

  @keyframes gridFlow {
    0% { background-position: 0 0; }
    100% { background-position: 60px 60px; }
  }

  #stars {
    position: absolute;
    inset: 0;
    width: 1920px;
    height: 1080px;
    z-index: 1;
  }

  /* Cinematic Lighting Glows */
  .light-glow {
    position: absolute;
    border-radius: 50%;
    filter: blur(90px);
    opacity: 0.35;
    pointer-events: none;
    z-index: 1;
  }
  .glow-blue { width: 700px; height: 700px; background: #2563EB; left: -100px; top: -100px; }
  .glow-cyan { width: 600px; height: 600px; background: #06B6D4; right: -50px; bottom: -50px; }

  /* Scene Manager */
  .scene {
    position: absolute;
    inset: 0;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    opacity: 0;
    pointer-events: none;
    transition: opacity 0.8s ease, transform 0.8s cubic-bezier(0.16, 1, 0.3, 1);
    transform: scale(0.95);
    z-index: 10;
  }

  .scene.active {
    opacity: 1;
    pointer-events: auto;
    transform: scale(1);
  }

  /* Scene 1: Epic Intro */
  .logo-3d {
    width: 220px;
    height: 220px;
    border-radius: 36px;
    box-shadow: 0 20px 60px rgba(0,0,0,0.8), 0 0 80px rgba(56, 189, 248, 0.6);
    margin-bottom: 30px;
    animation: logoFloat 4s ease-in-out infinite;
  }
  @keyframes logoFloat {
    0%, 100% { transform: translateY(0px) rotate(0deg); }
    50% { transform: translateY(-15px) rotate(1.5deg); }
  }

  .epic-title {
    font-size: 88px;
    font-weight: 900;
    color: #FFFFFF;
    letter-spacing: -1.5px;
    text-shadow: 0 10px 40px rgba(0,0,0,0.8), 0 0 30px rgba(56, 189, 248, 0.4);
    margin-bottom: 15px;
  }

  .epic-sub {
    font-size: 32px;
    font-weight: 500;
    color: #94A3B8;
    margin-bottom: 40px;
  }

  .badge-row {
    display: flex;
    gap: 16px;
  }
  .badge {
    background: rgba(15, 23, 42, 0.85);
    border: 1.5px solid #38BDF8;
    color: #38BDF8;
    padding: 12px 26px;
    border-radius: 30px;
    font-size: 20px;
    font-weight: 700;
    display: flex;
    align-items: center;
    gap: 10px;
    box-shadow: 0 0 20px rgba(56, 189, 248, 0.25);
  }
  .badge svg {
    width: 22px;
    height: 22px;
    fill: currentColor;
  }

  /* Authentic Windows 11 Desktop Workspace Frame */
  .win-desktop {
    width: 1420px;
    height: 760px;
    background: #0F172A;
    border-radius: 12px;
    border: 1.5px solid #334155;
    box-shadow: 0 30px 80px rgba(0,0,0,0.9), 0 0 60px rgba(56, 189, 248, 0.2);
    position: relative;
    overflow: hidden;
    display: flex;
    flex-direction: column;
  }

  /* Windows 11 Native Titlebar */
  .win-titlebar {
    height: 40px;
    background: #1E293B;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-left: 16px;
    border-bottom: 1px solid #334155;
  }

  .win-title-left {
    display: flex;
    align-items: center;
    gap: 10px;
    color: #94A3B8;
    font-size: 13.5px;
    font-weight: 600;
  }

  /* Windows 11 Min/Max/Close Control Buttons (Right) */
  .win-controls {
    display: flex;
    height: 100%;
  }
  .win-btn {
    width: 46px;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #94A3B8;
    font-size: 13px;
    cursor: default;
  }
  .win-btn-close {
    font-size: 14px;
  }

  .win-body {
    flex: 1;
    position: relative;
    background: #020617;
    display: flex;
    align-items: center;
    justify-content: center;
    background-image: 
      radial-gradient(circle at 50% 50%, rgba(37, 99, 235, 0.15) 0%, transparent 60%);
  }

  /* Underlying Artwork Simulation */
  .bg-art {
    width: 900px;
    height: 540px;
    background: linear-gradient(135deg, #1E1B4B 0%, #312E81 100%);
    border-radius: 10px;
    border: 2px dashed #6366F1;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #A5B4FC;
    font-size: 28px;
    font-weight: 700;
    flex-direction: column;
    gap: 16px;
  }

  /* Windows 11 OverlayPic Active Window Simulation */
  .overlay-window {
    position: absolute;
    width: 650px;
    height: 440px;
    background: rgba(15, 23, 42, 0.7);
    backdrop-filter: blur(8px);
    border: 2px solid #38BDF8;
    border-radius: 10px;
    box-shadow: 0 25px 60px rgba(0,0,0,0.8), 0 0 40px rgba(56, 189, 248, 0.4);
    display: flex;
    flex-direction: column;
    overflow: hidden;
  }

  /* Authentic OverlayPic Minimal 5-Button Toolbar */
  .overlay-bar {
    height: 42px;
    background: #0F172A;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 12px;
    border-bottom: 1px solid #1E293B;
  }

  .overlay-slider-wrap {
    display: flex;
    align-items: center;
    gap: 8px;
  }
  .fake-slider {
    width: 90px;
    height: 4px;
    background: #38BDF8;
    border-radius: 2px;
    position: relative;
  }
  .fake-slider::after {
    content: '';
    position: absolute;
    right: 20px;
    top: -5px;
    width: 14px;
    height: 14px;
    background: #FFFFFF;
    border-radius: 50%;
    box-shadow: 0 0 8px #38BDF8;
  }

  .overlay-btn-group {
    display: flex;
    gap: 4px;
  }
  .overlay-mini-btn {
    width: 26px;
    height: 26px;
    background: #1E293B;
    color: #F1F5F9;
    border-radius: 4px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 11px;
    font-weight: bold;
  }

  .feature-banner {
    position: absolute;
    bottom: 45px;
    background: rgba(15, 23, 42, 0.94);
    border: 2px solid #FFDD00;
    padding: 18px 45px;
    border-radius: 40px;
    font-size: 30px;
    font-weight: 800;
    color: #FFDD00;
    box-shadow: 0 10px 40px rgba(0,0,0,0.8), 0 0 35px rgba(255, 221, 0, 0.35);
    display: flex;
    align-items: center;
    gap: 14px;
    animation: bannerPop 0.6s cubic-bezier(0.16, 1, 0.3, 1);
  }
  @keyframes bannerPop {
    0% { transform: translateY(30px); opacity: 0; }
    100% { transform: translateY(0); opacity: 1; }
  }

  /* Progress Bar at Bottom */
  .timeline-bar {
    position: absolute;
    bottom: 0;
    left: 0;
    height: 6px;
    background: #38BDF8;
    width: 0%;
    z-index: 50;
    box-shadow: 0 0 15px #38BDF8;
  }
</style>
</head>
<body>

<div id="scaleWrapper">
  <div id="videoContainer">
    <div class="tech-grid"></div>
    <canvas id="stars" width="1920" height="1080"></canvas>
    <div class="light-glow glow-blue"></div>
    <div class="light-glow glow-cyan"></div>
    <div class="timeline-bar" id="progressBar"></div>

    <!-- SCENE 1: Intro Title -->
    <div class="scene active" id="scene1">
      <img src="data:image/jpeg;base64,$base64" alt="OverlayPic" class="logo-3d">
      <div class="epic-title">OverlayPic</div>
      <div class="epic-sub">Ultra-Lightweight Transparent Screen Overlay Image Viewer</div>
      <div class="badge-row">
        <div class="badge" style="border-color:#FCD34D; color:#FCD34D;">
          <svg viewBox="0 0 24 24"><path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/></svg>
          Zero-Install Portable
        </div>
        <div class="badge" style="border-color:#38BDF8; color:#38BDF8;">
          <svg viewBox="0 0 24 24"><path d="M6 3a3 3 0 1 0 2.83 4H14l5-5 1.41 1.41L15.83 8H18a3 3 0 1 1-2.83 4H14l-5 5-1.41-1.41L12.17 11H8.83A3 3 0 1 0 6 3zm0 2a1 1 0 1 1 0 2 1 1 0 0 1 0-2zm0 8a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/></svg>
          Screen Snipping
        </div>
        <div class="badge" style="border-color:#A78BFA; color:#A78BFA;">
          <svg viewBox="0 0 24 24"><path d="M9 2v13.59l-3.3-3.3-1.41 1.42L9.59 19H18v-2h-6.59l-2-2H13V2H9z"/></svg>
          Click-Through Mode
        </div>
        <div class="badge" style="border-color:#34D399; color:#34D399;">
          <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 17.93c-3.95-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"/></svg>
          Bilingual (EN/KO)
        </div>
      </div>
    </div>

    <!-- SCENE 2: Instant Snipping Feature (Ctrl+Alt+X) -->
    <div class="scene" id="scene2">
      <div class="win-desktop">
        <div class="win-titlebar">
          <div class="win-title-left">
            <svg width="15" height="15" viewBox="0 0 24 24" fill="#38BDF8"><path d="M4 4h7v7H4V4zm9 0h7v7h-7V4zm-9 9h7v7H4v-7zm9 0h7v7h-7v-7z"/></svg>
            <span>Windows Workspace Target (Visual Studio / Illustrator / CAD)</span>
          </div>
          <div class="win-controls">
            <div class="win-btn">─</div>
            <div class="win-btn">□</div>
            <div class="win-btn win-btn-close">✕</div>
          </div>
        </div>
        <div class="win-body">
          <div class="bg-art">
            <span>Reference Drawing / Blueprint / UI Design</span>
            <span style="font-size:20px; color:#818CF8;">Press [Ctrl+Alt+X] to Snip Any Area Instantly</span>
          </div>
          <!-- Overlay Window Simulation with Real App UI Toolbar -->
          <div class="overlay-window" style="transform: scale(0.92); opacity: 0.85;">
            <div class="overlay-bar">
              <div class="overlay-slider-wrap">
                <span style="color:#94A3B8; font-size:11px; font-weight:bold;">Opacity:</span>
                <div class="fake-slider"></div>
                <span style="color:#38BDF8; font-size:11px; font-weight:bold;">65%</span>
              </div>
              <div class="overlay-btn-group">
                <div class="overlay-mini-btn">👆</div>
                <div class="overlay-mini-btn" style="background:#2563EB;">✂</div>
                <div class="overlay-mini-btn">⋯</div>
                <div class="overlay-mini-btn">ℹ</div>
                <div class="overlay-mini-btn">─</div>
                <div class="overlay-mini-btn" style="background:#EF4444;">✕</div>
              </div>
            </div>
            <div style="flex:1; display:flex; align-items:center; justify-content:center; background:rgba(30,41,59,0.5);">
              <span style="font-size:38px; color:#FFFFFF; font-weight:bold;">Snapped Area Pinned on Top!</span>
            </div>
          </div>
        </div>
      </div>
      <div class="feature-banner">
        <span>1. Instant Screen Snipping (Ctrl + Alt + X)</span>
      </div>
    </div>

    <!-- SCENE 3: Smooth Opacity Adjustment -->
    <div class="scene" id="scene3">
      <div class="win-desktop">
        <div class="win-titlebar">
          <div class="win-title-left">
            <svg width="15" height="15" viewBox="0 0 24 24" fill="#38BDF8"><path d="M4 4h7v7H4V4zm9 0h7v7h-7V4zm-9 9h7v7H4v-7zm9 0h7v7h-7v-7z"/></svg>
            <span>1:1 Pixel Tracing & Blueprint Comparison</span>
          </div>
          <div class="win-controls">
            <div class="win-btn">─</div>
            <div class="win-btn">□</div>
            <div class="win-btn win-btn-close">✕</div>
          </div>
        </div>
        <div class="win-body">
          <div class="bg-art">
            <span>Underlying Target Image / Application</span>
            <span style="font-size:20px; color:#818CF8;">Mouse Wheel to Smoothly Adjust Opacity (5% ~ 100%)</span>
          </div>
          <div class="overlay-window" style="opacity: 0.45; border-color: #F59E0B;">
            <div class="overlay-bar">
              <div class="overlay-slider-wrap">
                <span style="color:#94A3B8; font-size:11px; font-weight:bold;">Opacity:</span>
                <div class="fake-slider" style="background:#F59E0B;"></div>
                <span style="color:#F59E0B; font-size:11px; font-weight:bold;">45%</span>
              </div>
              <div class="overlay-btn-group">
                <div class="overlay-mini-btn">👆</div>
                <div class="overlay-mini-btn">✂</div>
                <div class="overlay-mini-btn">⋯</div>
                <div class="overlay-mini-btn">ℹ</div>
                <div class="overlay-mini-btn">─</div>
                <div class="overlay-mini-btn" style="background:#EF4444;">✕</div>
              </div>
            </div>
            <div style="flex:1; display:flex; align-items:center; justify-content:center;">
              <span style="font-size:44px; color:#FCD34D; font-weight:bold;">See-Through Active Reference</span>
            </div>
          </div>
        </div>
      </div>
      <div class="feature-banner">
        <span>2. Real-Time Opacity & 1:1 Pixel Tracing</span>
      </div>
    </div>

    <!-- SCENE 4: Click-Through Mode -->
    <div class="scene" id="scene4">
      <div class="win-desktop">
        <div class="win-titlebar">
          <div class="win-title-left">
            <svg width="15" height="15" viewBox="0 0 24 24" fill="#34D399"><path d="M4 4h7v7H4V4zm9 0h7v7h-7V4zm-9 9h7v7H4v-7zm9 0h7v7h-7v-7z"/></svg>
            <span>Windows Desktop Click-Through Mode Active</span>
          </div>
          <div class="win-controls">
            <div class="win-btn">─</div>
            <div class="win-btn">□</div>
            <div class="win-btn win-btn-close">✕</div>
          </div>
        </div>
        <div class="win-body">
          <div class="bg-art">
            <span style="color:#34D399;">Click & Type Windows Beneath Freely!</span>
            <span style="font-size:20px; color:#6EE7B7;">Toggle: Ctrl+Shift+T  |  Release: Esc</span>
          </div>
          <div class="overlay-window" style="opacity: 0.55; border-style: dashed; border-color:#34D399;">
            <div class="overlay-bar">
              <div class="overlay-slider-wrap">
                <span style="color:#34D399; font-size:11px; font-weight:bold;">Click-Through ACTIVE</span>
              </div>
              <div class="overlay-btn-group">
                <div class="overlay-mini-btn" style="background:#10B981;">👆</div>
                <div class="overlay-mini-btn">✂</div>
                <div class="overlay-mini-btn">⋯</div>
                <div class="overlay-mini-btn">ℹ</div>
                <div class="overlay-mini-btn">─</div>
                <div class="overlay-mini-btn" style="background:#EF4444;">✕</div>
              </div>
            </div>
            <div style="flex:1; display:flex; align-items:center; justify-content:center;">
              <span style="font-size:38px; color:#6EE7B7; font-weight:bold;">Zero Click Blocking</span>
            </div>
          </div>
        </div>
      </div>
      <div class="feature-banner">
        <span>3. Full Click-Through Mode (Ctrl + Shift + T)</span>
      </div>
    </div>

    <!-- SCENE 5: Outro & Download Info -->
    <div class="scene" id="scene5">
      <img src="data:image/jpeg;base64,$base64" alt="OverlayPic" class="logo-3d">
      <div class="epic-title">OverlayPic v1.0.3</div>
      <div class="epic-sub" style="color:#38BDF8;">100% Free & Portable • No Ads • No Setup Required</div>
      <div style="font-size: 26px; color:#FCD34D; font-weight:bold; margin-bottom: 35px;">
        Support on Buy Me a Coffee: buymeacoffee.com/flowsuiteyjc
      </div>
      <div class="badge-row">
        <div class="badge" style="border-color:#10B981; color:#10B981;">Microsoft Store Verified</div>
        <div class="badge" style="border-color:#F59E0B; color:#F59E0B;">GitHub Releases (Portable ZIP)</div>
      </div>
    </div>
  </div>
</div>

<script>
// Auto Scale exactly for any resolution
function resizeScale() {
  const wrapper = document.getElementById('scaleWrapper');
  const scale = Math.min(window.innerWidth / 1920, window.innerHeight / 1080);
  wrapper.style.transform = 'scale(' + scale + ')';
}
window.addEventListener('resize', resizeScale);
resizeScale();

// Twinkling Stars
const canvas = document.getElementById('stars');
const ctx = canvas.getContext('2d');
const stars = [];
for (let i = 0; i < 150; i++) {
  stars.push({
    x: Math.random() * 1920,
    y: Math.random() * 1080,
    size: Math.random() * 2.5 + 0.8,
    alpha: Math.random() * 0.8 + 0.2,
    speedX: Math.random() * 0.3 + 0.1,
    phase: Math.random() * Math.PI * 2
  });
}
function drawStars() {
  ctx.clearRect(0, 0, 1920, 1080);
  stars.forEach(s => {
    s.x += s.speedX;
    if (s.x > 1920) s.x = 0;
    s.phase += 0.03;
    const a = s.alpha * (0.5 + 0.5 * Math.sin(s.phase));
    ctx.fillStyle = 'rgba(186, 230, 253, ' + a + ')';
    ctx.beginPath();
    ctx.arc(s.x, s.y, s.size, 0, Math.PI * 2);
    ctx.fill();
  });
  requestAnimationFrame(drawStars);
}
drawStars();

// Scene Director
const scenes = [
  { id: 'scene1', duration: 4500 },
  { id: 'scene2', duration: 5000 },
  { id: 'scene3', duration: 5000 },
  { id: 'scene4', duration: 5000 },
  { id: 'scene5', duration: 5500 }
];

let currentSceneIdx = 0;
const totalDuration = scenes.reduce((sum, s) => sum + s.duration, 0);
let elapsed = 0;
const progressBar = document.getElementById('progressBar');

function showScene(idx) {
  document.querySelectorAll('.scene').forEach((sc, i) => {
    sc.classList.toggle('active', i === idx);
  });
}

function nextScene() {
  currentSceneIdx = (currentSceneIdx + 1) % scenes.length;
  showScene(currentSceneIdx);
  setTimeout(nextScene, scenes[currentSceneIdx].duration);
}

setInterval(() => {
  elapsed = (elapsed + 100) % totalDuration;
  progressBar.style.width = ((elapsed / totalDuration) * 100) + '%';
}, 100);

setTimeout(nextScene, scenes[0].duration);
</script>

</body>
</html>
"@

[System.IO.File]::WriteAllText("i:\_MyProject\Program\WpfImageEdit\OverlayPic\dist\youtube_showcase_video.html", $html, [System.Text.Encoding]::UTF8)
Write-Host "Windows 11 Native Theme youtube_showcase_video.html generated cleanly."
