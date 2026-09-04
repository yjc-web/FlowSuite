using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using OverlayPic.Core.Interfaces;
using OverlayPic.Core.Localization;
using OverlayPic.Core.Models;

namespace OverlayPic.App.Views
{
    public partial class CaptureWindow : Window
    {
        private readonly IPlatformService _platformService;
        private Point _startPoint;
        private bool _isDragging;
        
        public byte[]? CapturedImageBytes { get; private set; }
        public PixelRect SelectedScreenRect { get; private set; }

        public CaptureWindow() : this(new OverlayPic.App.Services.PlatformService())
        {
        }

        public CaptureWindow(IPlatformService platformService)
        {
            InitializeComponent();
            _platformService = platformService;

            var loc = LocalizationService.Instance;
            var hintMain = this.FindControl<TextBlock>("HintMainText");
            var hintSub = this.FindControl<TextBlock>("HintSubText");
            var hintBanner = this.FindControl<Border>("HintBanner");

            if (loc.CurrentLanguage == AppLanguage.English)
            {
                if (hintMain != null) hintMain.Text = "✂️ Drag area to snip screen";
                if (hintSub != null) hintSub.Text = "(Cancel: Esc / Right Click)";
            }

            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            KeyDown += OnKeyDown;

            Loaded += (_, _) =>
            {
                if (hintBanner != null)
                {
                    double screenW = Bounds.Width;
                    double bannerW = hintBanner.Bounds.Width > 0 ? hintBanner.Bounds.Width : 360;
                    Canvas.SetLeft(hintBanner, Math.Max(20, (screenW - bannerW) / 2));
                }
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close(false);
            }
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var props = e.GetCurrentPoint(this).Properties;
            if (props.IsRightButtonPressed)
            {
                Close(false);
                return;
            }

            if (props.IsLeftButtonPressed)
            {
                var selectionBox = this.FindControl<Border>("SelectionBox");
                var hintBanner = this.FindControl<Border>("HintBanner");

                _isDragging = true;
                _startPoint = e.GetPosition(this);

                if (selectionBox != null)
                {
                    Canvas.SetLeft(selectionBox, _startPoint.X);
                    Canvas.SetTop(selectionBox, _startPoint.Y);
                    selectionBox.Width = 0;
                    selectionBox.Height = 0;
                    selectionBox.IsVisible = true;
                }

                if (hintBanner != null) hintBanner.IsVisible = false;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isDragging) return;

            var currentPoint = e.GetPosition(this);
            var selectionBox = this.FindControl<Border>("SelectionBox");
            var sizeLabel = this.FindControl<TextBlock>("SizeLabel");

            double left = Math.Min(_startPoint.X, currentPoint.X);
            double top = Math.Min(_startPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - _startPoint.X);
            double height = Math.Abs(currentPoint.Y - _startPoint.Y);

            if (selectionBox != null)
            {
                Canvas.SetLeft(selectionBox, left);
                Canvas.SetTop(selectionBox, top);
                selectionBox.Width = width;
                selectionBox.Height = height;
            }

            if (sizeLabel != null)
            {
                sizeLabel.Text = $"{(int)width} × {(int)height}";
            }
        }

        private async void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!_isDragging) return;
            _isDragging = false;

            var endPoint = e.GetPosition(this);
            var selectionBox = this.FindControl<Border>("SelectionBox");
            var hintBanner = this.FindControl<Border>("HintBanner");

            double left = Math.Min(_startPoint.X, endPoint.X);
            double top = Math.Min(_startPoint.Y, endPoint.Y);
            double width = Math.Abs(endPoint.X - _startPoint.X);
            double height = Math.Abs(endPoint.Y - _startPoint.Y);

            if (width < 10 || height < 10)
            {
                if (selectionBox != null) selectionBox.IsVisible = false;
                if (hintBanner != null) hintBanner.IsVisible = true;
                return;
            }

            var scaling = RenderScaling;
            int screenX = (int)((Position.X + left) * (_platformService.IsWindows ? scaling : 1.0));
            int screenY = (int)((Position.Y + top) * (_platformService.IsWindows ? scaling : 1.0));
            int screenW = (int)(width * (_platformService.IsWindows ? scaling : 1.0));
            int screenH = (int)(height * (_platformService.IsWindows ? scaling : 1.0));

            SelectedScreenRect = new PixelRect((int)(Position.X + left), (int)(Position.Y + top), (int)width, (int)height);

            IsVisible = false;
            await Task.Delay(80);

            CapturedImageBytes = await _platformService.CaptureScreenRegionAsync(screenX, screenY, screenW, screenH);
            Close(true);
        }
    }
}
