# 📌 OverlayPic

<p align="center">
  <img src="Resources/kakaopay_qr.png" width="0" height="0" alt="" />
  <b>초경량 화면 오버레이 투명 이미지 뷰어 (Lightweight Screen Overlay Transparent Image Viewer)</b><br>
  <sub>Designed & Developed by <b>YJC</b></sub>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Version-v1.0.1-blue.svg" alt="Version" />
  <img src="https://img.shields.io/badge/.NET_Framework-4.8.1-512BD4.svg" alt=".NET" />
  <img src="https://img.shields.io/badge/Platform-Windows-0078D6.svg" alt="Platform" />
  <img src="https://img.shields.io/badge/License-MIT-green.svg" alt="License" />
</p>

---

## 🌟 개요 (Overview)

**OverlayPic**은 화면 위에 원하는 이미지를 반투명하게 띄워두고, **클릭 통과(Click-through)** 모드를 통해 뒤쪽 프로그램을 그대로 조작할 수 있는 초경량 Windows 유틸리티입니다.

무설치 단일 실행 파일(`.exe`)로 언제 어디서나 가볍고 빠르게 사용할 수 있습니다.

- 🎨 **웹/앱 프론트엔드 개발자 & 디자이너**: Figma/디자인 시안을 웹 브라우저 위에 1:1로 얹어놓고 픽셀 단위 검수 (Pixel-Perfect Check)
- 🖌️ **일러스트레이터 & 원화가**: 포토샵, 클립스튜디오 작업 시 참고 포즈/구도 이미지를 띄워두고 트레이싱 및 대고 그리기
- 📊 **일반 업무 & 학생**: 듀얼 모니터 없이 한 화면에서 문서, 영수증, 도면 대조 작업

---

## ✨ 주요 기능 (Key Features)

| 기능 | 설명 |
|---|---|
| **✂️ 원터치 화면 캡처** | `Ctrl + Alt + X`로 화면의 원하는 영역을 드래그하면 그 자리에 즉시 반투명 오버레이 생성 |
| **📋 원클릭 붙여넣기** | `Ctrl + V`로 클립보드 캡처 이미지나 탐색기 복사 파일을 즉시 화면에 로드 |
| **👆 클릭 통과 (Click-Through)** | 마우스 클릭이 뒤쪽 프로그램으로 통과되어 이미지 아래 창을 자유롭게 조작 (`Esc`로 즉시 해제) |
| **🔍 실시간 투명도 조절** | 마우스 휠 또는 `+` / `-` 키로 불투명도(0.05 ~ 1.0)를 부드럽게 조절 |
| **📐 직관적인 창 크기 조절** | `Ctrl + 마우스 휠` 또는 `Ctrl + (+/-)`로 비율 유지하며 확대/축소 |
| **🔒 위치 및 크기 잠금 (Lock)** | 작업 중 실수로 창이 밀리거나 크기가 변하지 않도록 원클릭 고정 (`Ctrl + L`) |
| **🏁 체커보드 배경 지원** | 투명 PNG 이미지의 외곽선 확인을 위한 체커보드 배경 토글 (`Space`) |
| **↻ 1:1 원본 크기 복원** | 클릭 한 번으로 원본 이미지의 1:1 해상도로 창 크기 복구 (`Ctrl + R`) |
| **⚡ 무설치 포터블** | 레지스트리 건드리지 않는 100% 무설치 단일 실행 파일 |

---

## ⌨️ 단축키 안내 (Keyboard Shortcuts)

| 단축키 | 동작 |
|---|---|
| `Ctrl + Alt + X` | ✂️ **화면 영역 드래그 캡처** (캡처 위치에 즉시 오버레이 생성) |
| `Ctrl + V` | 클립보드 이미지 또는 이미지 파일 붙여넣기 |
| `Ctrl + O` | 이미지 파일 열기 (.png, .jpg, .webp, .gif, .bmp 등) |
| `마우스 휠` / `+`, `-` | 투명도 실시간 조절 |
| `Ctrl + 마우스 휠` / `Ctrl + (+/-)` | 창 크기 비율 조절 |
| `Ctrl + Shift + T` / `Ctrl + T` | 👆 클릭 통과 모드 토글 |
| `Esc` | 👆 클릭 통과 해제 (일반 모드 시 창 닫기) |
| `Ctrl + L` | 🔒 위치 및 크기 고정 토글 |
| `Ctrl + R` | ↻ 원본 해상도 크기로 리셋 |
| `Space` | 🏁 투명 체커보드 배경 토글 |

---

## 🚀 다운로드 및 실행 방법 (Download & Run)

1. [FlowSuite Releases](https://github.com/yjc-web/FlowSuite/releases/tag/OverlayPic) 페이지에서 `OverlayPic_v1.0.1_Setup.exe` (설치형) 또는 `OverlayPic_v1.0.1.zip` (무설치 포터블)을 다운로드합니다.
2. 설치 후 실행하거나, 압축을 푼 후 `OverlayPic.exe`를 실행합니다.
3. 원하는 이미지를 드래그 앤 드롭하거나, 캡처 후 `Ctrl + V`를 누르면 즉시 사용할 수 있습니다.

---

## 🛠️ 빌드 방법 (Build from Source)

- **요구 사양**: Visual Studio 2019 이상 / .NET Framework 4.8.1
```bash
# 저장소 복제
git clone https://github.com/yjc-web/OverlayPic.git

# MSBuild로 릴리즈 빌드
msbuild OverlayPic.csproj /p:Configuration=Release
```
빌드된 파일은 `bin/Release/OverlayPic.exe`에서 확인하실 수 있습니다.

---

## 👨‍💻 제작자 & 문의 (Author & Feedback)

- **Developer**: YJC
- **Official Blog**: [https://blog.naver.com/nds-macro](https://blog.naver.com/nds-macro)
- 버그 제보 및 기능 제안은 GitHub Issue 또는 블로그 안부글/댓글을 통해 남겨주세요.

---

## 📄 라이선스 (License)

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.
