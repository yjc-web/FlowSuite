using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using OverlayPic.App.Services;
using OverlayPic.Core.Interfaces;
using OverlayPic.Core.Localization;
using OverlayPic.Core.Models;

namespace OverlayPic.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly IPlatformService _platformService;
        private readonly GlobalHotKeyManager _hotKeyManager;
        private Bitmap? _currentBitmap;
        private double _imageNativeWidth;
        private double _imageNativeHeight;
        private bool _isLocked;
        private bool _hasImage;

        public MainWindow() : this(new PlatformService(), new GlobalHotKeyManager())
        {
        }

        public MainWindow(IPlatformService platformService, GlobalHotKeyManager hotKeyManager)
        {
            InitializeComponent();
            _platformService = platformService;
            _hotKeyManager = hotKeyManager;

            SetupEvents();
            SetupHotKeys();
            ApplyLanguage();

            LocalizationService.Instance.LanguageChanged += () => Dispatcher.UIThread.Post(ApplyLanguage);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupEvents()
        {
            PointerPressed += OnWindowPointerPressed;
            PointerWheelChanged += OnWindowPointerWheelChanged;
            KeyDown += OnWindowKeyDown;

            OpacitySlider.PropertyChanged += (s, e) =>
            {
                if (e.Property == Slider.ValueProperty)
                {
                    double val = OpacitySlider.Value;
                    OverlayImage.Opacity = val;
                    OpacityLabel.Text = $"{(int)(val * 100)}%";
                }
            };

            ClickThroughToggle.IsCheckedChanged += OnClickThroughChanged;
            SnipBtn.Click += (_, _) => StartScreenCapture();

            MenuOpen.Click += (_, _) => OpenImageFile();
            MenuSave.Click += (_, _) => SaveImageFile();
            MenuPaste.Click += (_, _) => PasteFromClipboard();
            MenuLock.Click += (_, _) => ToggleLock();
            MenuChecker.Click += (_, _) => ToggleChecker();
            MenuReset.Click += (_, _) => ResetToNativeResolution();
            MenuLang.Click += (_, _) => LocalizationService.Instance.ToggleLanguage();

            DropZone.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    OpenImageFile();
                }
            };

            AboutBtn.Click += (_, _) => AboutModal.IsVisible = true;
            CloseAboutBtn.Click += (_, _) => AboutModal.IsVisible = false;
            MinimizeBtn.Click += (_, _) => WindowState = WindowState.Minimized;
            CloseBtn.Click += (_, _) => Close();

            BlogBtn.Click += (_, _) => _platformService.OpenUrl("https://blog.naver.com/nds-macro");
            BuyCoffeeBtn.Click += (_, _) => _platformService.OpenUrl("https://buymeacoffee.com/flowsuiteyjc");

            AddHandler(DragDrop.DropEvent, OnFileDrop);
            AddHandler(DragDrop.DragOverEvent, OnDragOver);

            PositionChanged += (_, _) => UpdateInfoLabel();
            ScalingChanged += (_, _) => UpdateInfoLabel();
            SizeChanged += (_, _) => UpdateInfoLabel();
        }

        private void SetupHotKeys()
        {
            _hotKeyManager.OnToggleClickThrough += () => Dispatcher.UIThread.Post(() =>
            {
                ClickThroughToggle.IsChecked = !ClickThroughToggle.IsChecked;
            });

            _hotKeyManager.OnStartSnip += () => Dispatcher.UIThread.Post(StartScreenCapture);

            _hotKeyManager.OnEscape += () => Dispatcher.UIThread.Post(() =>
            {
                if (ClickThroughToggle.IsChecked == true)
                {
                    ClickThroughToggle.IsChecked = false;
                }
                else if (AboutModal.IsVisible)
                {
                    AboutModal.IsVisible = false;
                }
            });

            _hotKeyManager.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotKeyManager.Dispose();
            base.OnClosed(e);
        }

        private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!_isLocked && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var hit = e.Source as Visual;
                if (hit != null && !IsDescendantOfInteractive(hit))
                {
                    BeginMoveDrag(e);
                }
            }
        }

        private bool IsDescendantOfInteractive(Visual visual)
        {
            var current = visual;
            while (current != null && current != this)
            {
                if (current is Button || current is ToggleButton || current is Slider || current is MenuItem)
                    return true;
                current = current.GetVisualParent();
            }
            return false;
        }

        private void OnWindowPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control) || e.KeyModifiers.HasFlag(KeyModifiers.Meta))
            {
                if (!_isLocked)
                {
                    double factor = e.Delta.Y > 0 ? 1.08 : 0.92;
                    Width = Math.Max(MinWidth, Width * factor);
                    Height = Math.Max(MinHeight, Height * factor);
                    UpdateInfoLabel();
                }
            }
            else
            {
                double step = e.Delta.Y > 0 ? 0.05 : -0.05;
                OpacitySlider.Value = Math.Round(Math.Clamp(OpacitySlider.Value + step, 0.05, 1.0), 2);
            }
            e.Handled = true;
        }

        private void OnWindowKeyDown(object? sender, KeyEventArgs e)
        {
            var isCtrlOrCmd = e.KeyModifiers.HasFlag(KeyModifiers.Control) || e.KeyModifiers.HasFlag(KeyModifiers.Meta);

            if (isCtrlOrCmd && e.Key == Key.O)
            {
                OpenImageFile();
                e.Handled = true;
            }
            else if (isCtrlOrCmd && e.Key == Key.S)
            {
                SaveImageFile();
                e.Handled = true;
            }
            else if (isCtrlOrCmd && e.Key == Key.V)
            {
                PasteFromClipboard();
                e.Handled = true;
            }
            else if (isCtrlOrCmd && e.Key == Key.L)
            {
                ToggleLock();
                e.Handled = true;
            }
            else if (isCtrlOrCmd && e.Key == Key.R)
            {
                ResetToNativeResolution();
                e.Handled = true;
            }
            else if (isCtrlOrCmd && e.Key == Key.M)
            {
                WindowState = WindowState.Minimized;
                e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                ToggleChecker();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (AboutModal.IsVisible)
                {
                    AboutModal.IsVisible = false;
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
        }

        private void OnClickThroughChanged(object? sender, RoutedEventArgs e)
        {
            var handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            bool enable = ClickThroughToggle.IsChecked == true;

            if (handle != IntPtr.Zero)
            {
                _platformService.SetClickThrough(handle, enable);
            }

            if (enable)
            {
                ControlBar.Opacity = 0.45;
                OutlineBorder.BorderBrush = Brushes.OrangeRed;
                InfoLabel.Text = LocalizationService.Instance.Get("ClickThroughActive");
            }
            else
            {
                ControlBar.Opacity = 1.0;
                OutlineBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0x66, 0x3B, 0x82, 0xF6));
                UpdateInfoLabel();
            }
        }

        public void SetOverlayImage(Bitmap bitmap)
        {
            _currentBitmap = bitmap;
            OverlayImage.Source = bitmap;
            _imageNativeWidth = bitmap.PixelSize.Width;
            _imageNativeHeight = bitmap.PixelSize.Height;
            _hasImage = true;

            DropZone.IsVisible = false;

            double w = _imageNativeWidth;
            double h = _imageNativeHeight;
            if (w > 800 || h > 600)
            {
                double ratio = Math.Min(800.0 / w, 600.0 / h);
                w *= ratio;
                h *= ratio;
            }

            Width = Math.Max(MinWidth, w);
            Height = Math.Max(MinHeight, h);
            UpdateInfoLabel();
        }

        private async void OpenImageFile()
        {
            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage == null) return;

            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = LocalizationService.Instance.Get("MenuOpen"),
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Image Files")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.webp", "*.gif", "*.ico" }
                    }
                }
            });

            if (files.Count > 0)
            {
                await using var stream = await files[0].OpenReadAsync();
                var bitmap = new Bitmap(stream);
                SetOverlayImage(bitmap);
            }
        }

        private async void SaveImageFile()
        {
            if (!_hasImage || _currentBitmap == null)
            {
                InfoLabel.Text = LocalizationService.Instance.Get("NoImageToSave");
                return;
            }

            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage == null) return;

            var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = LocalizationService.Instance.Get("MenuSave"),
                DefaultExtension = "png",
                SuggestedFileName = $"OverlayPic_{DateTime.Now:yyyyMMdd_HHmmss}.png",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("PNG Image") { Patterns = new[] { "*.png" } },
                    new FilePickerFileType("JPEG Image") { Patterns = new[] { "*.jpg", "*.jpeg" } }
                }
            });

            if (file != null)
            {
                await using var stream = await file.OpenWriteAsync();
                _currentBitmap.Save(stream);
                InfoLabel.Text = LocalizationService.Instance.Get("SavedFile") + file.Name;
            }
        }

        private async void PasteFromClipboard()
        {
            try
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard == null) return;

                var formats = await clipboard.GetFormatsAsync();
                if (formats.Contains(DataFormats.Files))
                {
                    var files = await clipboard.GetDataAsync(DataFormats.Files) as System.Collections.IEnumerable;
                    if (files != null)
                    {
                        foreach (var f in files)
                        {
                            string? path = f is IStorageItem item ? item.Path.LocalPath : f?.ToString();
                            if (path != null && File.Exists(path))
                            {
                                using var stream = File.OpenRead(path);
                                var bmp = new Bitmap(stream);
                                SetOverlayImage(bmp);
                                return;
                            }
                        }
                    }
                }

                InfoLabel.Text = LocalizationService.Instance.Get("NoImageInClipboard");
            }
            catch (Exception ex)
            {
                InfoLabel.Text = $"Paste error: {ex.Message}";
            }
        }

        public async void StartScreenCapture()
        {
            double prevOpacity = Opacity;
            Opacity = 0;
            await Task.Delay(80);

            var captureWin = new CaptureWindow(_platformService);
            var result = await captureWin.ShowDialog<bool>(this);
            Opacity = prevOpacity;

            if (result && captureWin.CapturedImageBytes != null)
            {
                using var ms = new MemoryStream(captureWin.CapturedImageBytes);
                var bitmap = new Bitmap(ms);
                SetOverlayImage(bitmap);

                var rect = captureWin.SelectedScreenRect;
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Position = new PixelPoint(rect.X, rect.Y);
                    Width = Math.Max(MinWidth, rect.Width);
                    Height = Math.Max(MinHeight, rect.Height);
                }
            }
        }

        private void ToggleLock()
        {
            _isLocked = !_isLocked;
            MenuLock.Header = _isLocked
                ? "🔓 " + (_platformService.IsMacOS ? "Unlock (⌘L)" : "위치/크기 잠금 해제 (Ctrl+L)")
                : LocalizationService.Instance.Get("MenuLock");
            InfoLabel.Text = _isLocked ? "🔒 Locked (위치/크기 고정)" : "🔓 Unlocked (고정 해제)";
        }

        private void ToggleChecker()
        {
            CheckerGrid.IsVisible = !CheckerGrid.IsVisible;
        }

        private void ResetToNativeResolution()
        {
            if (_imageNativeWidth > 0 && _imageNativeHeight > 0)
            {
                Width = Math.Max(MinWidth, _imageNativeWidth);
                Height = Math.Max(MinHeight, _imageNativeHeight);
                UpdateInfoLabel();
            }
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        private async void OnFileDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles()?.ToList();
                if (files != null && files.Count > 0)
                {
                    var file = files[0];
                    if (file is IStorageFile storageFile)
                    {
                        await using var stream = await storageFile.OpenReadAsync();
                        var bmp = new Bitmap(stream);
                        SetOverlayImage(bmp);
                    }
                    else
                    {
                        string localPath = file.Path.LocalPath;
                        if (File.Exists(localPath))
                        {
                            using var stream = File.OpenRead(localPath);
                            var bmp = new Bitmap(stream);
                            SetOverlayImage(bmp);
                        }
                    }
                }
            }
        }

        private void UpdateInfoLabel()
        {
            if (ClickThroughToggle.IsChecked == true) return;

            int w = (int)Bounds.Width;
            int h = (int)Bounds.Height;
            int x = Position.X;
            int y = Position.Y;

            string text = $"Pos: ({x}, {y}) | Size: {w} × {h}";
            if (_hasImage)
            {
                text += $" | Native: {(int)_imageNativeWidth} × {(int)_imageNativeHeight}";
            }
            InfoLabel.Text = text;
        }

        private void ApplyLanguage()
        {
            var loc = LocalizationService.Instance;
            Title = loc.Get("AppTitle");

            OpacityTitle.Text = loc.Get("Opacity");
            ToolTip.SetTip(OpacitySlider, loc.Get("OpacityTooltip"));
            ToolTip.SetTip(ClickThroughToggle, loc.Get("ClickThroughTooltip"));
            ToolTip.SetTip(SnipBtn, loc.Get("SnipTooltip"));
            ToolTip.SetTip(MoreBtn, loc.Get("MoreTooltip"));
            ToolTip.SetTip(AboutBtn, loc.Get("AboutTooltip"));
            ToolTip.SetTip(MinimizeBtn, loc.Get("MinimizeTooltip"));
            ToolTip.SetTip(CloseBtn, loc.Get("CloseTooltip"));

            MenuOpen.Header = loc.Get("MenuOpen");
            MenuSave.Header = loc.Get("MenuSave");
            MenuPaste.Header = loc.Get("MenuPaste");
            MenuLock.Header = loc.Get("MenuLock");
            MenuChecker.Header = loc.Get("MenuChecker");
            MenuReset.Header = loc.Get("MenuReset");
            MenuLang.Header = loc.Get("MenuLang");

            DropTitle.Text = loc.Get("DropTitle");
            DropSubtitle.Text = loc.Get("DropSubtitle");
            DropHint.Text = loc.Get("DropHint");

            AboutHeaderTitle.Text = loc.Get("AboutHeaderTitle");
            AboutSubtitle.Text = loc.Get("AboutSubtitle");
            ShortcutsTitle.Text = loc.Get("ShortcutsTitle");
            ShortcutSnip.Text = loc.Get("ShortcutSnip");
            ShortcutSave.Text = loc.Get("ShortcutSave");
            ShortcutOpen.Text = loc.Get("ShortcutOpen");
            ShortcutPaste.Text = loc.Get("ShortcutPaste");
            ShortcutToggle.Text = loc.Get("ShortcutToggle");
            ShortcutEsc.Text = loc.Get("ShortcutEsc");
            ShortcutOpacity.Text = loc.Get("ShortcutOpacity");
            ShortcutResize.Text = loc.Get("ShortcutResize");
            ShortcutLock.Text = loc.Get("ShortcutLock");
            ShortcutReset.Text = loc.Get("ShortcutReset");
            ShortcutSpace.Text = loc.Get("ShortcutSpace");
            BlogBtn.Content = loc.Get("BlogBtnText");
            BuyCoffeeBtn.Content = loc.Get("CoffeeBtnText");

            UpdateInfoLabel();
        }
    }
}
