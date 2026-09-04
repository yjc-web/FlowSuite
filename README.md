# 📌 OverlayPic

<p align="center">
  <b>Ultra-lightweight Transparent Screen Overlay Image Viewer for Windows</b><br>
  <sub>Designed & Developed by <b>YJC</b></sub>
</p>

<p align="center">
  <a href="https://github.com/yjc-web/FlowSuite/releases/tag/v1.0.4.0"><img src="https://img.shields.io/badge/Version-v1.0.4.0-blue.svg" alt="Version" /></a>
  <a href="https://github.com/yjc-web/FlowSuite/actions/workflows/build.yml"><img src="https://github.com/yjc-web/FlowSuite/actions/workflows/build.yml/badge.svg" alt="Build Status" /></a>
  <img src="https://img.shields.io/badge/.NET_Framework-4.8.1-512BD4.svg" alt=".NET" />
  <img src="https://img.shields.io/badge/Platform-Windows_10_|_11-0078D6.svg" alt="Platform" />
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-green.svg" alt="License" /></a>
  <a href="https://buymeacoffee.com/flowsuiteyjc"><img src="https://img.shields.io/badge/Buy_Me_a_Coffee-FFDD00?logo=buy-me-a-coffee&logoColor=black" alt="Buy Me A Coffee" /></a>
</p>

---

## 🌟 Overview

**OverlayPic** is an ultra-lightweight, zero-install Windows utility that floats reference images, design mockups, and documents with adjustable semi-transparency directly over your workspace.

With its native **Click-Through** mode, your mouse clicks pass straight through the overlay image, allowing you to interact with underlying applications (Figma, browser, IDE, Photoshop, Excel, etc.) without any disruption.

- 🎨 **Web & App Frontend Developers**: Place Figma/XD design comps 1:1 directly over your browser for **Pixel-Perfect layout inspections**.
- 🖌️ **Digital Artists & Illustrators**: Float anatomy, character sheets, and pose references over Clip Studio or Photoshop for precise tracing.
- 📊 **Multitasking & Office Work**: Compare receipts, invoices, drawings, and tables directly over spreadsheets without dual monitors.

---

## ✨ Key Features

| Feature | Description |
|---|---|
| 🎯 **1px Fine-Tuning & Floating Jog Window** | Nudge position in **1px increments** with Arrow keys (`↑↓←→`) and resize with `Ctrl+Arrow`. Includes a standalone, draggable floating **Jog Pad** window with D-Pad controls, 1px/10px step switch, real-time coordinate badges, and mouse wheel nudge support. |
| ✂️ **One-Touch Screen Snipping** | Press `Ctrl + Alt + X` anywhere to drag-select any screen area and spawn an instant semi-transparent overlay in place. |
| 👆 **Native Click-Through Mode** | Mouse clicks pass through the overlay to control the background app. Press `Esc` or `Ctrl + Shift + T` to exit anytime. |
| 💾 **Instant Image Saving** | Save currently loaded or snipped images directly to **PNG** (with alpha transparency preserved), **JPG**, or **BMP** via `Ctrl + S`. |
| 📋 **Instant Clipboard Paste** | Press `Ctrl + V` to immediately load images copied from browsers, capture tools, or Windows Explorer. |
| 🔍 **Real-Time Opacity Control** | Smoothly adjust transparency from 5% to 100% using the mouse wheel or `+` / `-` keys. |
| 📐 **Proportional Resizing** | Zoom in and out while preserving the original aspect ratio with `Ctrl + Mouse Wheel`. |
| 🔒 **Position & Size Lock** | Freeze the overlay window (`Ctrl + L`) to prevent accidental shifts or resizing during work. |
| 🏁 **Checkerboard Background** | Toggle transparent checkerboard grid (`Space`) to inspect PNG alpha cutouts and icon contours. |
| ↻ **1:1 Native Resolution Reset** | Instantly snap back to the image's original pixel dimensions with `Ctrl + R`. |
| 🌐 **Bilingual UI** | Seamless real-time switching between English and Korean with automatic system locale detection. |
| ⚡ **100% Pure Portable** | Single standalone `.exe` (< 300KB). Zero registry clutter, zero network telemetry, runs 100% offline. |

---

## ⌨️ Keyboard Shortcuts Reference

| Shortcut | Action |
|---|---|
| `🎯 Button` | **Toggle Fine Tuning Mode** (opens draggable Jog Window & enables arrow key nudging) |
| `Arrow Keys (↑, ↓, ←, →)` | **Nudge position by 1px** (Hold `Shift`: 10px fast movement) *in Fine-Tuning Mode* |
| `Ctrl + Arrow Keys` | **Resize width/height by 1px** (Hold `Ctrl + Shift`: 10px) *in Fine-Tuning Mode* |
| `Ctrl + Alt + X` | ✂️ **Screen Snipping Tool** (creates an instant overlay over the dragged region) |
| `Ctrl + S` | 💾 **Save current image to file** (PNG with transparency / JPG / BMP) |
| `Ctrl + V` | 📋 Paste image from clipboard or copied file |
| `Ctrl + O` | 📂 Open image file (`.png`, `.jpg`, `.webp`, `.gif`, `.bmp`) |
| `Mouse Wheel` / `+`, `-` | Adjust opacity (0.05 to 1.0) |
| `Ctrl + Mouse Wheel` | Scale window size proportionally |
| `Ctrl + Shift + T` / `Ctrl + T` | 👆 Toggle Click-Through mode |
| `Esc` | 👆 **Exit Fine Tuning / Exit Click-Through** (Closes window only in default mode) |
| `Ctrl + L` | 🔒 Lock / Unlock position and dimensions |
| `Ctrl + R` | ↻ Reset window to 1:1 original image resolution |
| `Space` | 🏁 Toggle transparency checkerboard background |
| `Ctrl + M` | Minimize window |

---

## 🚀 Download & Installation

### Option 1: Portable Standalone Executable (Recommended)
1. Download `OverlayPic_v1.0.4.zip` from [GitHub Releases](https://github.com/yjc-web/FlowSuite/releases/tag/v1.0.4.0).
2. Extract the archive anywhere (USB, Desktop, Tools folder).
3. Run `OverlayPic.exe` directly — zero setup required!

### Option 2: Windows Installer
1. Download `OverlayPic_v1.0.4.0_Setup.exe` from [GitHub Releases](https://github.com/yjc-web/FlowSuite/releases/tag/v1.0.4.0).
2. Follow the standard installation wizard. Desktop shortcut and Start Menu entries will be created.

### Option 3: Microsoft Store (MSIX)
- Search for **OverlayPic** in the Microsoft Store or install `OverlayPic_v1.0.4.0_Store.msix`.

---

## 🛠️ Build from Source

OverlayPic is built with pure C# and WPF on .NET Framework 4.8.1 with zero third-party dependencies.

### Prerequisites
- Windows 10 or 11
- Visual Studio 2019 / 2022 (with .NET Desktop Development workload) or MSBuild Tools

### Build Instructions
```bash
# 1. Clone repository
git clone https://github.com/yjc-web/FlowSuite.git
cd FlowSuite

# 2. Build Release using MSBuild
msbuild OverlayPic.sln /p:Configuration=Release /p:Platform="Any CPU"
```
The compiled standalone binary will be generated at:
```
bin\Release\OverlayPic.exe
```

---

## 📋 Release Notes

### 📌 v1.0.4.0
- **New Feature**: Added 1px Fine-Tuning mode (`🎯`) with keyboard arrow nudging (1px default, Shift: 10px).
- **New Floating Window**: Separated the Jog Pad controller into an independent, draggable floating window (`FineTuningWindow`) that never obstructs the underlying image canvas.
- **Safety Guards**: Automatically disables nudging when the window is locked (`Ctrl+L`) or in click-through mode. `Esc` safely closes the fine-tuning window first.
- **CI / CD**: Added automated GitHub Actions cloud build workflow with transparent build provenance.

### 📌 v1.0.2
- **New Feature**: Added `Ctrl + S` instant file saving with full alpha transparency preservation for PNG.
- **UI Enhancements**: Added More menu (`⋯`) with direct image export options.

### 📌 v1.0.1
- **New Feature**: Added `Ctrl + Alt + X` one-touch screen drag snip.
- **Localization**: Added full English and Korean bilingual support with automatic system locale detection.

### 📌 v1.0.0
- Official first public release: ultra-lightweight overlay viewer, native Win32 click-through, and mouse wheel controls.

---

## 📄 Documentation

- 📖 [User Manual (Web HTML)](userManual.html) — Comprehensive interactive visual guide
- 📝 [Release Notes (Web HTML)](releasenotes.html) — Full patch history & changelog

---

## 👨‍💻 Author & Community

- **Developer**: YJC
- **Official Blog**: [https://blog.naver.com/nds-macro](https://blog.naver.com/nds-macro)
- **Support & Donations**: [Buy Me a Coffee](https://buymeacoffee.com/flowsuiteyjc)
- **Bug Reports & Feedback**: Please file an issue on [GitHub Issues](https://github.com/yjc-web/FlowSuite/issues).

---

## ⚖️ License

This project is open source and available under the terms of the [MIT License](LICENSE).
