using System;
using System.Collections.Generic;
using OverlayPic.Core.Models;

namespace OverlayPic.Core.Localization
{
    public class LocalizationService
    {
        private static LocalizationService? _instance;
        public static LocalizationService Instance => _instance ??= new LocalizationService();

        public AppLanguage CurrentLanguage { get; set; } = AppLanguage.Korean;

        public event Action? LanguageChanged;

        private readonly Dictionary<string, (string Ko, string En)> _strings = new()
        {
            ["AppTitle"] = ("OverlayPic — 화면 오버레이 투명 뷰어", "OverlayPic — Screen Overlay Transparent Image Viewer"),
            ["DragHint"] = ("드래그하여 창 이동", "Drag to move window"),
            ["Opacity"] = ("투명도:", "Opacity:"),
            ["OpacityTooltip"] = ("투명도 조절 (마우스 휠로도 가능)", "Adjust opacity (Mouse Wheel supported)"),
            ["ClickThroughTooltip"] = ("클릭 통과 모드 (뒤쪽 프로그램 클릭 가능 / 해제: Esc 또는 Ctrl+Shift+T)", "Click-Through Mode (Click windows beneath / Esc to exit)"),
            ["SnipTooltip"] = ("화면 영역 드래그 캡처 (Ctrl+Alt+X)", "Screen Snipping (Ctrl+Alt+X / ⌘⌥X)"),
            ["MoreTooltip"] = ("더보기 메뉴 (Ctrl+O, Ctrl+V, 고정, 설정 등)", "More Options (Ctrl+O, Ctrl+V, Lock, etc.)"),
            ["AboutTooltip"] = ("프로그램 정보 / 단축키 / 개발자 후원", "About / Shortcuts / Coffee Donation"),
            ["MinimizeTooltip"] = ("최소화 (Ctrl+M)", "Minimize (Ctrl+M / ⌘M)"),
            ["CloseTooltip"] = ("닫기 (Esc)", "Close (Esc)"),
            
            // Context Menu
            ["MenuOpen"] = ("📂 이미지 파일 열기... (Ctrl+O)", "📂 Open Image File... (Ctrl+O)"),
            ["MenuSave"] = ("💾 이미지 다른 이름으로 저장... (Ctrl+S)", "💾 Save Image As... (Ctrl+S)"),
            ["MenuPaste"] = ("📋 클립보드 붙여넣기 (Ctrl+V)", "📋 Paste from Clipboard (Ctrl+V)"),
            ["MenuLock"] = ("🔒 위치 및 크기 고정 (Ctrl+L)", "🔒 Lock Position & Size (Ctrl+L)"),
            ["MenuChecker"] = ("🏁 체커보드 배경 토글 (Space)", "🏁 Toggle Checkerboard Grid (Space)"),
            ["MenuReset"] = ("↻ 1:1 원본 해상도로 리셋 (Ctrl+R)", "↻ Reset to 1:1 Native Resolution (Ctrl+R)"),
            ["MenuLang"] = ("🌐 Language / 언어 전환", "🌐 Language / Switch Language"),

            // DropZone
            ["DropTitle"] = ("이미지를 여기에 드래그 & 드롭", "Drag & Drop Image Here"),
            ["DropSubtitle"] = ("또는 붙여넣기 (Ctrl+V) | 화면 캡처 (Ctrl+Alt+X)", "or Paste (Ctrl+V) | Screen Snip (Ctrl+Alt+X)"),
            ["DropHint"] = ("마우스 휠: 투명도 조절 | Ctrl+휠: 창 크기 조절 | 클릭하여 열기", "Mouse Wheel: Opacity | Ctrl+Wheel: Resize | Click to Open"),
            ["DropSuccess"] = ("마우스를 놓으면 바로 표시!", "Release mouse to load!"),

            // Status Messages
            ["ClickThroughActive"] = ("👆 클릭 통과 활성 (해제: Esc 또는 단축키)", "👆 Click-Through Active (Esc to exit)"),
            ["SavedFile"] = ("💾 저장 완료: ", "💾 Saved: "),
            ["NoImageToSave"] = ("저장할 이미지가 없습니다.", "No image loaded to save."),
            ["NoImageInClipboard"] = ("클립보드에 이미지가 없습니다.", "No image found in clipboard."),

            // About Modal
            ["AboutHeaderTitle"] = ("📌 OverlayPic", "📌 OverlayPic"),
            ["AboutSubtitle"] = ("화면 오버레이 투명 뷰어 | Made by YJC", "Screen Overlay Image Viewer | Made by YJC"),
            ["ShortcutsTitle"] = ("⌨️ 주요 단축키", "⌨️ Keyboard Shortcuts"),
            ["ShortcutSnip"] = ("• Ctrl+Alt+X / ⌘⌥X : ✂️ 화면 영역 드래그 캡처", "• Ctrl+Alt+X / ⌘⌥X : ✂️ Screen Snipping"),
            ["ShortcutSave"] = ("• Ctrl+S / ⌘S : 💾 현재 이미지 파일로 저장", "• Ctrl+S / ⌘S : 💾 Save Image to File"),
            ["ShortcutOpen"] = ("• Ctrl+O / ⌘O : 📂 이미지 파일 열기", "• Ctrl+O / ⌘O : 📂 Open Image File"),
            ["ShortcutPaste"] = ("• Ctrl+V / ⌘V : 📋 클립보드 이미지 붙여넣기", "• Ctrl+V / ⌘V : 📋 Paste Clipboard Image"),
            ["ShortcutToggle"] = ("• Ctrl+Shift+T / ⌘⇧T : 클릭 통과 토글", "• Ctrl+Shift+T / ⌘⇧T : Toggle Click-Through"),
            ["ShortcutEsc"] = ("• Esc : 클릭 통과 해제 (일반 시 창 닫기)", "• Esc : Release Click-Through / Close Window"),
            ["ShortcutMin"] = ("• Ctrl+M / ⌘M : 최소화 (Minimize)", "• Ctrl+M / ⌘M : Minimize Window"),
            ["ShortcutOpacity"] = ("• 마우스 휠 / +, - : 투명도 조절", "• Mouse Wheel / +, - : Adjust Opacity"),
            ["ShortcutResize"] = ("• Ctrl + 휠 / ⌘ + 휠 : 창 크기 조절", "• Ctrl + Wheel / ⌘ + Wheel : Resize Window"),
            ["ShortcutLock"] = ("• Ctrl+L / ⌘L : 위치/크기 고정 토글", "• Ctrl+L / ⌘L : Lock/Unlock Position & Size"),
            ["ShortcutReset"] = ("• Ctrl+R / ⌘R : 원본 해상도 크기로 리셋", "• Ctrl+R / ⌘R : Reset to 1:1 Native Resolution"),
            ["ShortcutSpace"] = ("• Space : 체커보드 배경 토글", "• Space : Toggle Checkerboard Grid"),
            ["BlogBtnText"] = ("🌐 공식 웹사이트 방문 (Blog)", "🌐 Official Website (Blog)"),
            ["CoffeeBtnText"] = ("☕ 개발자에게 커피 한 잔 후원하기", "☕ Buy Me a Coffee")
        };

        public string Get(string key)
        {
            if (_strings.TryGetValue(key, out var val))
            {
                return CurrentLanguage == AppLanguage.Korean ? val.Ko : val.En;
            }
            return key;
        }

        public void ToggleLanguage()
        {
            CurrentLanguage = CurrentLanguage == AppLanguage.Korean ? AppLanguage.English : AppLanguage.Korean;
            LanguageChanged?.Invoke();
        }

        public void SetLanguage(AppLanguage language)
        {
            if (CurrentLanguage != language)
            {
                CurrentLanguage = language;
                LanguageChanged?.Invoke();
            }
        }
    }
}
