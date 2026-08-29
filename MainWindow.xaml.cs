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
    public partial class MainWindow : Window
    {
        #region Win32 API Constants & Imports

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WM_HOTKEY = 0x0312;

        // Modifiers for RegisterHotKey
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_NOREPEAT = 0x4000;

        // Virtual Key Codes
        private const uint VK_ESCAPE = 0x1B;
        private const uint VK_T = 0x54;

        // Hotkey IDs
        private const int HOTKEY_ID_GLOBAL_TOGGLE = 9001; // Ctrl+Shift+T (Always registered)
        private const int HOTKEY_ID_ESCAPE_RELEASE = 9002; // Esc (Registered only during Click-Through)

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hwnd, int index);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hwnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hwnd, int index, IntPtr newStyle);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static int GetWindowLong(IntPtr hwnd, int index)
        {
            if (IntPtr.Size == 8)
                return unchecked((int)GetWindowLongPtr64(hwnd, index).ToInt64());
            return GetWindowLong32(hwnd, index);
        }

        private static int SetWindowLong(IntPtr hwnd, int index, int newStyle)
        {
            if (IntPtr.Size == 8)
                return unchecked((int)SetWindowLongPtr64(hwnd, index, new IntPtr(newStyle)).ToInt64());
            return SetWindowLong32(hwnd, index, newStyle);
        }

        #endregion

        private IntPtr _hwnd;
        private HwndSource _hwndSource;
        private int _originalExStyle;
        private double _imageNativeWidth;
        private double _imageNativeHeight;
        private bool _hasImage;
        private bool _isEscHotKeyRegistered;

        public MainWindow()
        {
            InitializeComponent();
            MouseWheel += MainWindow_MouseWheel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hwnd = new WindowInteropHelper(this).Handle;
            _originalExStyle = GetWindowLong(_hwnd, GWL_EXSTYLE);

            _hwndSource = HwndSource.FromHwnd(_hwnd);
            _hwndSource?.AddHook(HwndHook);

            // Register global shortcut Ctrl+Shift+T to toggle click-through anytime
            RegisterHotKey(_hwnd, HOTKEY_ID_GLOBAL_TOGGLE, MOD_CONTROL | MOD_SHIFT | MOD_NOREPEAT, VK_T);

            // Check if clipboard already has an image on startup
            TryLoadFromClipboard();

            UpdateInfoLabel();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unregister all hotkeys on window close
            if (_hwnd != IntPtr.Zero)
            {
                UnregisterHotKey(_hwnd, HOTKEY_ID_GLOBAL_TOGGLE);
                if (_isEscHotKeyRegistered)
                {
                    UnregisterHotKey(_hwnd, HOTKEY_ID_ESCAPE_RELEASE);
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
            if (msg == WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                if (hotkeyId == HOTKEY_ID_ESCAPE_RELEASE)
                {
                    // Escape key pressed globally while in Click-Through mode -> Release click-through
                    if (ClickThroughToggle.IsChecked == true)
                    {
                        ClickThroughToggle.IsChecked = false;
                        handled = true;
                    }
                }
                else if (hotkeyId == HOTKEY_ID_GLOBAL_TOGGLE)
                {
                    // Ctrl+Shift+T pressed globally -> Toggle click-through mode
                    ClickThroughToggle.IsChecked = !(ClickThroughToggle.IsChecked == true);
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
            if (LockToggle.IsChecked != true && e.ButtonState == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void ControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (LockToggle.IsChecked != true && e.ButtonState == MouseButtonState.Pressed)
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
                if (LockToggle.IsChecked != true)
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
            else if (e.Key == Key.T && (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)))
            {
                ClickThroughToggle.IsChecked = !(ClickThroughToggle.IsChecked == true);
                e.Handled = true;
            }
            else if (e.Key == Key.L && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                LockToggle.IsChecked = !(LockToggle.IsChecked == true);
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
            else if (e.Key == Key.Space)
            {
                CheckerToggle.IsChecked = !(CheckerToggle.IsChecked == true);
                e.Handled = true;
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.OemPlus || e.Key == Key.Add))
            {
                if (LockToggle.IsChecked != true)
                {
                    Width = Math.Min(SystemParameters.PrimaryScreenWidth * 2.0, Width * 1.08);
                    Height = Math.Min(SystemParameters.PrimaryScreenHeight * 2.0, Height * 1.08);
                    UpdateInfoLabel();
                }
                e.Handled = true;
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.OemMinus || e.Key == Key.Subtract))
            {
                if (LockToggle.IsChecked != true)
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
                SetWindowLong(_hwnd, GWL_EXSTYLE, _originalExStyle | WS_EX_TRANSPARENT);
                ControlBar.Opacity = 0.45;
                OutlineBorder.BorderBrush = Brushes.OrangeRed;
                InfoLabel.Text = "👆 클릭 통과 활성 (해제: Esc 또는 Ctrl+Shift+T)";

                // Register global Esc hotkey so pressing Esc anywhere disables click-through
                if (!_isEscHotKeyRegistered)
                {
                    _isEscHotKeyRegistered = RegisterHotKey(_hwnd, HOTKEY_ID_ESCAPE_RELEASE, MOD_NOREPEAT, VK_ESCAPE);
                }
            }
            else
            {
                // Disable click-through
                SetWindowLong(_hwnd, GWL_EXSTYLE, _originalExStyle);
                ControlBar.Opacity = 1.0;
                OutlineBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0x66, 0x3B, 0x82, 0xF6));

                // Unregister global Esc hotkey so Esc functions normally for other apps
                if (_isEscHotKeyRegistered)
                {
                    UnregisterHotKey(_hwnd, HOTKEY_ID_ESCAPE_RELEASE);
                    _isEscHotKeyRegistered = false;
                }

                UpdateInfoLabel();
            }
        }

        private void LockToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (LockToggle.IsChecked == true)
            {
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            UpdateInfoLabel();
        }

        private void CheckerToggle_Changed(object sender, RoutedEventArgs e)
        {
            CheckerBorder.Visibility = CheckerToggle.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
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

        private void OpenImageFile()
        {
            var dlg = new OpenFileDialog
            {
                Title = "오버레이 이미지 선택",
                Filter = "이미지 파일|*.png;*.jpg;*.jpeg;*.bmp;*.webp;*.gif;*.tiff;*.tif;*.ico|모든 파일|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                LoadFile(dlg.FileName);
            }
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
                MessageBox.Show($"블로그 링크 열기 실패: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            int w = (int)ActualWidth;
            int h = (int)ActualHeight;
            int x = (int)Left;
            int y = (int)Top;
            string lockStatus = LockToggle?.IsChecked == true ? " [🔒고정]" : "";

            if (_hasImage && _imageNativeWidth > 0)
            {
                InfoLabel.Text = $"{w}×{h} @ ({x},{y}){lockStatus} • 원본 {_imageNativeWidth:F0}×{_imageNativeHeight:F0}";
            }
            else
            {
                InfoLabel.Text = $"{w}×{h} @ ({x},{y}){lockStatus}";
            }
        }

        #endregion
    }
}
