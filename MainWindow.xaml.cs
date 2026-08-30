using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace OverlayPic
{
    internal static class NativeMethods
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WM_HOTKEY = 0x0312;

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_NOREPEAT = 0x4000;

        public const uint VK_ESCAPE = 0x1B;
        public const uint VK_T = 0x54;
        public const uint VK_X = 0x58;

        public const int HOTKEY_ID_GLOBAL_TOGGLE = 9001;
        public const int HOTKEY_ID_ESCAPE_RELEASE = 9002;
        public const int HOTKEY_ID_GLOBAL_SNIP = 9003;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    public enum AppLanguage { Korean, English }

    public partial class MainWindow : Window
    {
        private IntPtr _hwnd;
        private HwndSource _hwndSource;
        private int _originalExStyle;
        private double _imageNativeWidth;
        private double _imageNativeHeight;
        private bool _hasImage;
        private bool _isEscHotKeyRegistered;
        private bool _isLocked = false;
        private AppLanguage _currentLanguage = AppLanguage.Korean;

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += MainWindow_MouseWheel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hwnd = new WindowInteropHelper(this).Handle;
            _originalExStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE);

            _hwndSource = HwndSource.FromHwnd(_hwnd);
            _hwndSource?.AddHook(HwndHook);

            // Register global shortcut Ctrl+Shift+T to toggle click-through anytime
            NativeMethods.RegisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_GLOBAL_TOGGLE, NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT | NativeMethods.MOD_NOREPEAT, NativeMethods.VK_T);

            // Register global shortcut Ctrl+Alt+X for instant screen snippet capture
            NativeMethods.RegisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_GLOBAL_SNIP, NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT | NativeMethods.MOD_NOREPEAT, NativeMethods.VK_X);

            // Detect system language (Korean vs English)
            bool isKo = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ko", StringComparison.OrdinalIgnoreCase);
            _currentLanguage = isKo ? AppLanguage.Korean : AppLanguage.English;
            ApplyLanguage(_currentLanguage);

            // Check if clipboard already has an image on startup
            TryLoadFromClipboard();

            UpdateInfoLabel();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unregister all hotkeys on window close
            if (_hwnd != IntPtr.Zero)
            {
                NativeMethods.UnregisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_GLOBAL_TOGGLE);
                NativeMethods.UnregisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_GLOBAL_SNIP);
                if (_isEscHotKeyRegistered)
                {
                    NativeMethods.UnregisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_ESCAPE_RELEASE);
                    _isEscHotKeyRegistered = false;
                }
            }

            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(HwndHook);
                _hwndSource = null;
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// Win32 Message Hook to receive global hotkeys even when window is transparent/unfocused
        /// </summary>
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                if (hotkeyId == NativeMethods.HOTKEY_ID_ESCAPE_RELEASE)
                {
                    // Escape key pressed globally while in Click-Through mode -> Release click-through
                    if (ClickThroughToggle.IsChecked == true)
                    {
                        ClickThroughToggle.IsChecked = false;
                        handled = true;
                    }
                }
                else if (hotkeyId == NativeMethods.HOTKEY_ID_GLOBAL_TOGGLE)
                {
                    // Ctrl+Shift+T pressed globally -> Toggle click-through mode
                    ClickThroughToggle.IsChecked = !(ClickThroughToggle.IsChecked == true);
                    handled = true;
                }
                else if (hotkeyId == NativeMethods.HOTKEY_ID_GLOBAL_SNIP)
                {
                    // Ctrl+Alt+X pressed globally -> Start Screen Capture Snippet
                    StartScreenCapture();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Set overlay image and resize window to match image aspect ratio
        /// </summary>
        public void SetOverlayImage(BitmapSource image)
        {
            if (image == null) return;

            OverlayImage.Source = image;
            _imageNativeWidth = image.PixelWidth;
            _imageNativeHeight = image.PixelHeight;
            _hasImage = true;

            DropZone.Visibility = Visibility.Collapsed;

            // Auto-size window to fit image (capped at 75% screen size)
            double screenW = SystemParameters.PrimaryScreenWidth * 0.75;
            double screenH = SystemParameters.PrimaryScreenHeight * 0.75;

            double w = _imageNativeWidth;
            double h = _imageNativeHeight;

            if (w > screenW || h > screenH)
            {
                double ratio = Math.Min(screenW / w, screenH / h);
                w *= ratio;
                h *= ratio;
            }

            Width = Math.Max(MinWidth, w);
            Height = Math.Max(MinHeight, h);

            UpdateInfoLabel();
        }

        #region Mouse & Touch Interaction

        private void RootGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isLocked && e.ButtonState == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void ControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isLocked && e.ButtonState == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void DropZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenImageFile();
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // Ctrl + Wheel: Resize window (if not locked)
                if (!_isLocked)
                {
                    double factor = e.Delta > 0 ? 1.08 : 0.92;
                    double newW = Width * factor;
                    double newH = Height * factor;

                    if (newW >= MinWidth && newH >= MinHeight &&
                        newW <= SystemParameters.PrimaryScreenWidth * 2.0 &&
                        newH <= SystemParameters.PrimaryScreenHeight * 2.0)
                    {
                        Width = newW;
                        Height = newH;
                        UpdateInfoLabel();
                    }
                }
            }
            else
            {
                // Normal Wheel: Adjust Opacity
                double step = e.Delta > 0 ? 0.05 : -0.05;
                double newOpacity = Math.Round(OpacitySlider.Value + step, 2);
                newOpacity = Math.Max(OpacitySlider.Minimum, Math.Min(OpacitySlider.Maximum, newOpacity));
                OpacitySlider.Value = newOpacity;
            }
            e.Handled = true;
        }

        #endregion

        #region Drag & Drop

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                DropIcon.Text = "⬇️";
                DropTitle.Text = "마우스를 놓으면 바로 표시!";
            }
            e.Handled = true;
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            DropIcon.Text = "📌";
            DropTitle.Text = "이미지를 여기에 드래그 & 드롭";
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            DropIcon.Text = "📌";
            DropTitle.Text = "이미지를 여기에 드래그 & 드롭";

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (paths != null && paths.Length > 0 && File.Exists(paths[0]))
            {
                LoadFile(paths[0]);
            }
        }

        private void LoadFile(string filePath)
        {
            try
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(filePath);
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                bi.Freeze();
                SetOverlayImage(bi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 로드 실패: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Keyboard Shortcuts

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (AboutModal.Visibility == Visibility.Visible)
                {
                    AboutModal.Visibility = Visibility.Collapsed;
                }
                else if (ClickThroughToggle.IsChecked == true)
                {
                    ClickThroughToggle.IsChecked = false;
                }
                else
                {
                    Close();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.X && Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                StartScreenCapture();
                e.Handled = true;
            }
            else if (e.Key == Key.T && (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)))
            {
                ClickThroughToggle.IsChecked = !(ClickThroughToggle.IsChecked == true);
                e.Handled = true;
            }
            else if (e.Key == Key.L && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ToggleLock();
                e.Handled = true;
            }
            else if (e.Key == Key.M && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                WindowState = WindowState.Minimized;
                e.Handled = true;
            }
            else if (e.Key == Key.R && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ResetSize_Click(this, null);
                e.Handled = true;
            }
            else if (e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                PasteFromClipboard();
                e.Handled = true;
            }
            else if (e.Key == Key.O && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                OpenImageFile();
                e.Handled = true;
            }
            else if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                SaveImageFile();
                e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                ToggleChecker();
                e.Handled = true;
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.OemPlus || e.Key == Key.Add))
            {
                if (!_isLocked)
                {
                    Width = Math.Min(SystemParameters.PrimaryScreenWidth * 2.0, Width * 1.08);
                    Height = Math.Min(SystemParameters.PrimaryScreenHeight * 2.0, Height * 1.08);
                    UpdateInfoLabel();
                }
                e.Handled = true;
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.OemMinus || e.Key == Key.Subtract))
            {
                if (!_isLocked)
                {
                    Width = Math.Max(MinWidth, Width * 0.92);
                    Height = Math.Max(MinHeight, Height * 0.92);
                    UpdateInfoLabel();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                OpacitySlider.Value = Math.Min(1.0, Math.Round(OpacitySlider.Value + 0.05, 2));
                e.Handled = true;
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                OpacitySlider.Value = Math.Max(0.05, Math.Round(OpacitySlider.Value - 0.05, 2));
                e.Handled = true;
            }
        }

        #endregion

        #region Controls & Actions

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OverlayImage == null || OpacityLabel == null) return;
            OverlayImage.Opacity = OpacitySlider.Value;
            OpacityLabel.Text = $"{(int)(OpacitySlider.Value * 100)}%";
        }

        private void ClickThrough_Changed(object sender, RoutedEventArgs e)
        {
            if (_hwnd == IntPtr.Zero) return;

            if (ClickThroughToggle.IsChecked == true)
            {
                // Enable click-through: Mouse clicks pass through to windows beneath
                NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE, _originalExStyle | NativeMethods.WS_EX_TRANSPARENT);
                ControlBar.Opacity = 0.45;
                OutlineBorder.BorderBrush = Brushes.OrangeRed;
                InfoLabel.Text = (_currentLanguage == AppLanguage.English)
                    ? "👆 Click-Through Active (Esc / Ctrl+Shift+T to exit)"
                    : "👆 클릭 통과 활성 (해제: Esc 또는 Ctrl+Shift+T)";

                // Register global Esc hotkey so pressing Esc anywhere disables click-through
                if (!_isEscHotKeyRegistered)
                {
                    _isEscHotKeyRegistered = NativeMethods.RegisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_ESCAPE_RELEASE, NativeMethods.MOD_NOREPEAT, NativeMethods.VK_ESCAPE);
                }
            }
            else
            {
                // Disable click-through
                NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE, _originalExStyle);
                ControlBar.Opacity = 1.0;
                OutlineBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0x66, 0x3B, 0x82, 0xF6));

                // Unregister global Esc hotkey so Esc functions normally for other apps
                if (_isEscHotKeyRegistered)
                {
                    NativeMethods.UnregisterHotKey(_hwnd, NativeMethods.HOTKEY_ID_ESCAPE_RELEASE);
                    _isEscHotKeyRegistered = false;
                }

                UpdateInfoLabel();
            }
        }

        private void More_Click(object sender, RoutedEventArgs e)
        {
            if (MoreBtn.ContextMenu != null)
            {
                MoreBtn.ContextMenu.PlacementTarget = MoreBtn;
                MoreBtn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                MoreBtn.ContextMenu.IsOpen = true;
            }
        }

        private void MenuLock_Click(object sender, RoutedEventArgs e)
        {
            ToggleLock(MenuLock.IsChecked);
        }

        private void ToggleLock(bool? forceState = null)
        {
            _isLocked = forceState ?? !_isLocked;
            if (MenuLock != null) MenuLock.IsChecked = _isLocked;

            ResizeMode = _isLocked ? ResizeMode.NoResize : ResizeMode.CanResizeWithGrip;
            UpdateInfoLabel();
        }

        private void MenuChecker_Click(object sender, RoutedEventArgs e)
        {
            ToggleChecker(MenuChecker.IsChecked);
        }

        private void ToggleChecker(bool? forceState = null)
        {
            bool isVisible = (CheckerBorder.Visibility == Visibility.Visible);
            bool target = forceState ?? !isVisible;

            CheckerBorder.Visibility = target ? Visibility.Visible : Visibility.Collapsed;
            if (MenuChecker != null) MenuChecker.IsChecked = target;
        }

        private void Paste_Click(object sender, RoutedEventArgs e) => PasteFromClipboard();

        private void TryLoadFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    var img = Clipboard.GetImage();
                    if (img != null)
                    {
                        SetOverlayImage(img);
                        return;
                    }
                }

                if (Clipboard.ContainsFileDropList())
                {
                    StringCollection files = Clipboard.GetFileDropList();
                    if (files != null && files.Count > 0 && File.Exists(files[0]))
                    {
                        string ext = Path.GetExtension(files[0]).ToLowerInvariant();
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" ||
                            ext == ".webp" || ext == ".gif" || ext == ".tiff" || ext == ".ico")
                        {
                            LoadFile(files[0]);
                        }
                    }
                }
            }
            catch { }
        }

        private void PasteFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    var img = Clipboard.GetImage();
                    if (img != null)
                    {
                        SetOverlayImage(img);
                        return;
                    }
                }

                if (Clipboard.ContainsFileDropList())
                {
                    StringCollection files = Clipboard.GetFileDropList();
                    if (files != null && files.Count > 0 && File.Exists(files[0]))
                    {
                        LoadFile(files[0]);
                        return;
                    }
                }

                MessageBox.Show("클립보드에 이미지나 이미지 파일이 없습니다.", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"붙여넣기 실패: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e) => OpenImageFile();

        private void Save_Click(object sender, RoutedEventArgs e) => SaveImageFile();

        private void SaveImageFile()
        {
            if (!_hasImage || !(OverlayImage.Source is BitmapSource bitmapSource))
            {
                string msg = _currentLanguage == AppLanguage.English ? "No image loaded to save." : "저장할 이미지가 없습니다.";
                MessageBox.Show(msg, "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = _currentLanguage == AppLanguage.English ? "Save Image As" : "다른 이름으로 이미지 저장",
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|BMP Image (*.bmp)|*.bmp",
                DefaultExt = ".png",
                FileName = $"OverlayPic_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                    BitmapEncoder encoder;

                    if (ext == ".jpg" || ext == ".jpeg")
                    {
                        encoder = new JpegBitmapEncoder { QualityLevel = 95 };
                    }
                    else if (ext == ".bmp")
                    {
                        encoder = new BmpBitmapEncoder();
                    }
                    else
                    {
                        encoder = new PngBitmapEncoder();
                    }

                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                    using (var stream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(stream);
                    }

                    string info = _currentLanguage == AppLanguage.English
                        ? $"💾 Saved: {Path.GetFileName(dlg.FileName)}"
                        : $"💾 저장 완료: {Path.GetFileName(dlg.FileName)}";
                    InfoLabel.Text = info;
                }
                catch (Exception ex)
                {
                    string errMsg = _currentLanguage == AppLanguage.English ? $"Failed to save image: {ex.Message}" : $"이미지 저장 실패: {ex.Message}";
                    MessageBox.Show(errMsg, "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void OpenImageFile()
        {
            var dlg = new OpenFileDialog
            {
                Title = _currentLanguage == AppLanguage.English ? "Select Overlay Image" : "오버레이 이미지 선택",
                Filter = "이미지 파일|*.png;*.jpg;*.jpeg;*.bmp;*.webp;*.gif;*.tiff;*.tif;*.ico|모든 파일|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                LoadFile(dlg.FileName);
            }
        }

        private void Snip_Click(object sender, RoutedEventArgs e) => StartScreenCapture();

        public void StartScreenCapture()
        {
            try
            {
                // Temporarily hide this window so it doesn't get captured in the screenshot
                double prevOpacity = Opacity;
                Opacity = 0;

                // Allow UI to process the opacity change
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Render,
                    new Action(() => { }));

                System.Threading.Thread.Sleep(60);

                var snipWin = new CaptureWindow(_currentLanguage == AppLanguage.English);
                bool? result = snipWin.ShowDialog();

                // Restore opacity
                Opacity = prevOpacity;

                if (result == true && snipWin.CapturedBitmapSource != null)
                {
                    SetOverlayImage(snipWin.CapturedBitmapSource);

                    // Position and size the window exactly at the dragged snippet area
                    Rect rect = snipWin.SelectedScreenRect;
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Left = rect.Left;
                        Top = rect.Top;
                        Width = Math.Max(MinWidth, rect.Width);
                        Height = Math.Max(MinHeight, rect.Height);
                        UpdateInfoLabel();
                    }

                    Activate();
                }
            }
            catch (Exception ex)
            {
                Opacity = 1.0;
                string errMsg = _currentLanguage == AppLanguage.English ? $"Screen capture failed: {ex.Message}" : $"캡처 실행 실패: {ex.Message}";
                MessageBox.Show(errMsg, "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Lang_Click(object sender, RoutedEventArgs e)
        {
            _currentLanguage = (_currentLanguage == AppLanguage.Korean) ? AppLanguage.English : AppLanguage.Korean;
            ApplyLanguage(_currentLanguage);
        }

        private void ApplyLanguage(AppLanguage lang)
        {
            bool isEn = (lang == AppLanguage.English);

            // Window Title & About Header
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            AboutHeaderTitle.Text = $"📌 OverlayPic v{ver}";
            Title = isEn ? "OverlayPic — Screen Overlay Transparent Image Viewer" : "OverlayPic — 화면 오버레이 투명 뷰어";

            // Control Bar
            DragHintIcon.ToolTip = isEn ? "Drag to move window" : "드래그하여 창 이동";
            OpacityTitle.Text = isEn ? "Opacity:" : "투명도:";
            OpacitySlider.ToolTip = isEn ? "Adjust opacity (Mouse Wheel supported)" : "투명도 조절 (마우스 휠로도 가능)";

            ClickThroughToggle.ToolTip = isEn ? "Click-Through Mode (Click windows beneath / Esc to exit)" : "클릭 통과 모드 (뒤쪽 프로그램 클릭 가능 / 해제: Esc 또는 Ctrl+Shift+T)";
            SnipBtn.ToolTip = isEn ? "Screen Snipping (Ctrl+Alt+X)" : "화면 영역 드래그 캡처 (Ctrl+Alt+X)";
            MoreBtn.ToolTip = isEn ? "More Options (Ctrl+O, Ctrl+V, Lock, etc.)" : "더보기 메뉴 (Ctrl+O, Ctrl+V, 고정, 설정 등)";
            AboutBtn.ToolTip = isEn ? "About / Shortcuts / Coffee Donation" : "프로그램 정보 / 단축키 / 개발자 후원";
            MinimizeBtn.ToolTip = isEn ? "Minimize (Ctrl+M)" : "최소화 (Ctrl+M)";
            CloseBtn.ToolTip = isEn ? "Close (Esc)" : "닫기 (Esc)";

            // More Context Menu Items
            MenuOpen.Header = isEn ? "📂 Open Image File... (Ctrl+O)" : "📂 이미지 파일 열기... (Ctrl+O)";
            MenuSave.Header = isEn ? "💾 Save Image As... (Ctrl+S)" : "💾 이미지 다른 이름으로 저장... (Ctrl+S)";
            MenuPaste.Header = isEn ? "📋 Paste from Clipboard (Ctrl+V)" : "📋 클립보드 붙여넣기 (Ctrl+V)";
            MenuLock.Header = isEn ? "🔒 Lock Position & Size (Ctrl+L)" : "🔒 위치 및 크기 고정 (Ctrl+L)";
            MenuChecker.Header = isEn ? "🏁 Toggle Checkerboard Grid (Space)" : "🏁 체커보드 배경 토글 (Space)";
            MenuReset.Header = isEn ? "↻ Reset to 1:1 Native Resolution (Ctrl+R)" : "↻ 1:1 원본 해상도로 리셋 (Ctrl+R)";
            MenuLang.Header = isEn ? "🌐 Language / 언어 전환 (English ➔ 한국어)" : "🌐 Language / 언어 전환 (한국어 ➔ English)";

            // DropZone
            DropTitle.Text = isEn ? "Drag & Drop Image Here" : "이미지를 여기에 드래그 & 드롭";
            DropSubtitle.Text = isEn ? "or Paste (Ctrl+V)  |  Screen Snip (Ctrl+Alt+X)" : "또는 붙여넣기 (Ctrl+V)  |  화면 캡처해서 바로 넣기 (Ctrl+Alt+X)";
            DropHint.Text = isEn ? "Mouse Wheel: Opacity  |  Ctrl+Wheel: Resize  |  Click to Open" : "마우스 휠: 투명도 조절  |  Ctrl+휠: 창 크기 조절  |  클릭하여 파일 열기";

            // About Modal
            AboutSubtitle.Text = isEn ? "Screen Overlay Image Viewer | Made by YJC" : "화면 오버레이 투명 뷰어 | Made by YJC";
            ShortcutsTitle.Text = isEn ? "⌨️ Keyboard Shortcuts" : "⌨️ 주요 단축키";
            ShortcutSnip.Text = isEn ? "• Ctrl+Alt+X : ✂️ Screen Snipping" : "• Ctrl+Alt+X : ✂️ 화면 영역 드래그 캡처";
            ShortcutSave.Text = isEn ? "• Ctrl+S : 💾 Save Image to File" : "• Ctrl+S : 💾 현재 이미지 파일로 저장";
            ShortcutOpen.Text = isEn ? "• Ctrl+O : 📂 Open Image File" : "• Ctrl+O : 📂 이미지 파일 열기";
            ShortcutPaste.Text = isEn ? "• Ctrl+V : 📋 Paste Clipboard Image/File" : "• Ctrl+V : 📋 클립보드 이미지/파일 붙여넣기";
            ShortcutToggle.Text = isEn ? "• Ctrl+Shift+T / Ctrl+T : Toggle Click-Through" : "• Ctrl+Shift+T / Ctrl+T : 클릭 통과 토글";
            ShortcutEsc.Text = isEn ? "• Esc : Release Click-Through / Close Window" : "• Esc : 클릭 통과 해제 (일반 시 창 닫기)";
            ShortcutMin.Text = isEn ? "• Ctrl+M : Minimize Window" : "• Ctrl+M : 최소화 (Minimize)";
            ShortcutOpacity.Text = isEn ? "• Mouse Wheel / +, - : Adjust Opacity" : "• 마우스 휠 / +, - : 투명도 조절";
            ShortcutResize.Text = isEn ? "• Ctrl + Wheel / Ctrl+(+,-) : Resize Window" : "• Ctrl + 휠 / Ctrl+(+,-) : 창 크기 조절";
            ShortcutLock.Text = isEn ? "• Ctrl+L : Lock/Unlock Position & Size" : "• Ctrl+L : 위치/크기 고정 토글";
            ShortcutReset.Text = isEn ? "• Ctrl+R : Reset to 1:1 Native Resolution" : "• Ctrl+R : 원본 해상도 크기로 리셋";
            ShortcutSpace.Text = isEn ? "• Space : Toggle Checkerboard Grid" : "• Space : 체커보드 배경 토글";

            BlogBtn.Content = isEn ? "🌐 Official Website (Blog)" : "🌐 공식 블로그 방문 (nds-macro)";

            // Switch donation cards based on language
            if (isEn)
            {
                KakaoPayCard.Visibility = Visibility.Collapsed;
                GlobalCoffeeCard.Visibility = Visibility.Visible;
            }
            else
            {
                KakaoPayCard.Visibility = Visibility.Visible;
                GlobalCoffeeCard.Visibility = Visibility.Collapsed;
            }

            UpdateInfoLabel();
        }

        private void BuyCoffee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://buymeacoffee.com/flowsuiteyjc",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Buy Me a Coffee: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenGitHub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/yjc-web/FlowSuite",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open GitHub: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutModal.Visibility = Visibility.Visible;
        }

        private void CloseAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutModal.Visibility = Visibility.Collapsed;
        }

        private void AboutModal_BackgroundClick(object sender, MouseButtonEventArgs e)
        {
            AboutModal.Visibility = Visibility.Collapsed;
        }

        private void AboutModal_InnerClick(object sender, MouseButtonEventArgs e)
        {
            // Prevent modal from closing when clicking inside the content box
            e.Handled = true;
        }

        private void OpenBlog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://blog.naver.com/nds-macro",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                string errMsg = _currentLanguage == AppLanguage.English ? $"Failed to open website: {ex.Message}" : $"블로그 링크 열기 실패: {ex.Message}";
                MessageBox.Show(errMsg, "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetSize_Click(object sender, RoutedEventArgs e)
        {
            if (_imageNativeWidth > 0 && _imageNativeHeight > 0)
            {
                Width = Math.Max(MinWidth, _imageNativeWidth);
                Height = Math.Max(MinHeight, _imageNativeHeight);
                UpdateInfoLabel();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        #endregion

        #region Size & Position Label

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateInfoLabel();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            UpdateInfoLabel();
        }

        private void UpdateInfoLabel()
        {
            if (InfoLabel == null) return;
            if (ClickThroughToggle?.IsChecked == true) return;

            bool isEn = (_currentLanguage == AppLanguage.English);
            int w = (int)ActualWidth;
            int h = (int)ActualHeight;
            int x = (int)Left;
            int y = (int)Top;
            string lockStatus = _isLocked ? (isEn ? " [🔒Locked]" : " [🔒고정]") : "";
            string origText = isEn ? "Native" : "원본";

            if (_hasImage && _imageNativeWidth > 0)
            {
                InfoLabel.Text = $"{w}×{h} @ ({x},{y}){lockStatus} • {origText} {_imageNativeWidth:F0}×{_imageNativeHeight:F0}";
            }
            else
            {
                InfoLabel.Text = $"{w}×{h} @ ({x},{y}){lockStatus}";
            }
        }

        #endregion
    }
}
