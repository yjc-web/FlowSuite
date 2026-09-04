# 📌 OverlayPic

<p align="center">
  <img src="Resources/kakaopay_qr.png" width="0" height="0" alt="" />
  <b>초경량 화면 오버레이 투명 이미지 뷰어 (Lightweight Screen Overlay Transparent Image Viewer)</b><br>
  <sub>Designed & Developed by <b>YJC</b></sub>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Version-v1.0.4.0-blue.svg" alt="Version" />
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

| **🎯 1px 정밀 미세 정렬** | `🎯` 버튼 클릭 시 조그 패드 표시 및 방향키로 위치/크기 1px 단위 정밀 조절 (피그마/웹 퍼블리싱/트레이싱 특화) |
| **✂️ 원터치 화면 캡처** | `Ctrl + Alt + X`로 화면의 원하는 영역을 드래그하면 그 자리에 즉시 반투명 오버레이 생성 |
| **💾 편리한 이미지 저장** | `Ctrl + S`로 현재 캡처 또는 로드된 이미지를 PNG/JPG/BMP 파일로 즉시 저장 |
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
| `🎯 버튼` | 🎯 **미세 정렬 모드 토글** (조그 패드 표시 및 방향키 1px 이동 활성화) |
| `방향키 (↑, ↓, ←, →)` | 🎯 **위치 1px 미세 이동** (Shift+방향키: 10px 빠른 이동) *미세 정렬 모드 활성 시* |
| `Ctrl + 방향키` | 🎯 **가로/세로 1px 크기 조절** (Ctrl+Shift+방향키: 10px) *미세 정렬 모드 활성 시* |
| `Ctrl + Alt + X` | ✂️ **화면 영역 드래그 캡처** (캡처 위치에 즉시 오버레이 생성) |
| `Ctrl + S` | 💾 **현재 오버레이 이미지 파일로 저장** (PNG/JPG/BMP) |
| `Ctrl + V` | 📋 클립보드 이미지 또는 이미지 파일 붙여넣기 |
| `Ctrl + O` | 📂 이미지 파일 열기 (.png, .jpg, .webp, .gif, .bmp 등) |
| `마우스 휠` / `+`, `-` | 투명도 실시간 조절 |
| `Ctrl + 마우스 휠` / `Ctrl + (+/-)` | 창 크기 비율 조절 |
| `Ctrl + Shift + T` / `Ctrl + T` | 👆 클릭 통과 모드 토글 |
| `Esc` | 👆 **미세 정렬 모드 / 클릭 통과 해제** (일반 모드 시 창 닫기) |
| `Ctrl + L` | 🔒 위치 및 크기 고정 토글 |
| `Ctrl + R` | ↻ 원본 해상도 크기로 리셋 |
| `Space` | 🏁 투명 체커보드 배경 토글 |

---

## 🚀 다운로드 및 실행 방법 (Download & Run)

1. [FlowSuite Releases](https://github.com/yjc-web/FlowSuite/releases/tag/OverlayPic) 페이지에서 `OverlayPic_v1.0.4.0_Setup.exe` (설치형) 또는 `OverlayPic_v1.0.4.zip` (무설치 포터블)을 다운로드합니다.
2. 설치 후 실행하거나, 압축을 푼 후 `OverlayPic.exe`를 실행합니다.
3. 원하는 이미지를 드래그 앤 드롭하거나, 캡처 후 `Ctrl + V`를 누르면 즉시 사용할 수 있습니다.

---

## 📋 릴리즈 노트 (Release Notes)

### 📌 v1.0.4.0
- **신규 기능**: 1px 정밀 미세 정렬 모드 (`🎯`) 추가 (방향키 1px 이동, Ctrl+방향키 1px 크기 조절)
- **UI 개선**: 화면 우상단 플로팅 조그 패드(D-Pad, 1px/10px 스텝 토글, 실시간 좌표/크기 표시, 패드 위 마우스 휠 지원)
- **안전 장치**: 잠금(`Ctrl+L`) 및 클릭 통과 활성화 시 자동 차단, Esc 키로 미세 정렬 모드 안전 해제
- **단축키 및 다국어**: 한국어/English 실시간 다국어 지원 및 안내 창 업데이트

### 📌 v1.0.2
- **신규 기능**: 캡처 및 로드된 오버레이 이미지를 파일로 즉시 저장하는 기능 추가 (`Ctrl + S`)
- **포맷 지원**: PNG (투명도 보존), JPEG/JPG (고품질), BMP 포맷 저장 지원
- **UI 개선**: 더보기 메뉴(`⋯`)에 `💾 이미지 다른 이름으로 저장...` 추가 및 단축키 안내 모달 업데이트
- **다국어 지원**: 한국어/영어 전환 시 저장 메뉴 및 알림 메시지 완벽 지원

### 📌 v1.0.1
- **신규 기능**: 원터치 화면 영역 드래그 캡처 (`Ctrl + Alt + X`) 추가
- **개선**: 한국어/English 실시간 다국어 지원 및 시스템 로캘 자동 감지
- **패키징**: MS Store MSIX 및 GitHub Setup 인스톨러 배포 지원

### 📌 v1.0.0
- 최초 공식 릴리즈
- 초경량 투명 오버레이 뷰어, 클릭 통과(Click-through), 마우스 휠 투명도 및 크기 조절 기능 구현

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
