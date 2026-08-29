using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace OverlayPic
{
    public partial class CaptureWindow : Window
    {
        private Bitmap _screenBitmap;
        private Point _startPoint;
        private bool _isDragging;
        private double _dpiScaleX = 1.0;
        private double _dpiScaleY = 1.0;

        public BitmapSource CapturedBitmapSource { get; private set; }
        public Rect SelectedScreenRect { get; private set; }

        public CaptureWindow()
        {
            InitializeComponent();

            // Cover all monitors (Virtual Screen)
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Position the Hint Banner in the center top of the PRIMARY screen (not virtual screen center)
            HintBanner.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            double bannerW = HintBanner.DesiredSize.Width > 0 ? HintBanner.DesiredSize.Width : 340;

            double primaryLeftInCanvas = -SystemParameters.VirtualScreenLeft;
            double primaryTopInCanvas = -SystemParameters.VirtualScreenTop;

            double bannerLeft = primaryLeftInCanvas + (SystemParameters.PrimaryScreenWidth - bannerW) / 2;
            double bannerTop = primaryTopInCanvas + 40;

            Canvas.SetLeft(HintBanner, bannerLeft);
            Canvas.SetTop(HintBanner, bannerTop);

            // Get DPI scaling
            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null)
            {
                _dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                _dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }

            CaptureScreen();
        }

        private void CaptureScreen()
        {
            try
            {
                int vx = (int)SystemParameters.VirtualScreenLeft;
                int vy = (int)SystemParameters.VirtualScreenTop;
                int vw = (int)SystemParameters.VirtualScreenWidth;
                int vh = (int)SystemParameters.VirtualScreenHeight;

                // Capture full virtual screen bitmap
                int pxW = (int)(vw * _dpiScaleX);
                int pxH = (int)(vh * _dpiScaleY);

                _screenBitmap = new Bitmap(pxW, pxH, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(_screenBitmap))
                {
                    g.CopyFromScreen(
                        (int)(vx * _dpiScaleX),
                        (int)(vy * _dpiScaleY),
                        0, 0,
                        new System.Drawing.Size(pxW, pxH),
                        CopyPixelOperation.SourceCopy
                    );
                }

                // Show as background
                using (var ms = new MemoryStream())
                {
                    _screenBitmap.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    bi.Freeze();
                    BackgroundImage.Source = bi;
                    BackgroundImage.Width = Width;
                    BackgroundImage.Height = Height;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"화면 캡처 실패: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
                DialogResult = false;
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                // Right click cancels
                CancelCapture();
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = true;
                _startPoint = e.GetPosition(this);

                Canvas.SetLeft(SelectionBox, _startPoint.X);
                Canvas.SetTop(SelectionBox, _startPoint.Y);
                SelectionBox.Width = 0;
                SelectionBox.Height = 0;
                SelectionBox.Visibility = Visibility.Visible;
                HintBanner.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            Point currentPoint = e.GetPosition(this);

            double x = Math.Min(_startPoint.X, currentPoint.X);
            double y = Math.Min(_startPoint.Y, currentPoint.Y);
            double w = Math.Abs(currentPoint.X - _startPoint.X);
            double h = Math.Abs(currentPoint.Y - _startPoint.Y);

            Canvas.SetLeft(SelectionBox, x);
            Canvas.SetTop(SelectionBox, y);
            SelectionBox.Width = w;
            SelectionBox.Height = h;

            SizeLabel.Text = $"{(int)w} × {(int)h}";
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging) return;
            _isDragging = false;

            Point currentPoint = e.GetPosition(this);
            double x = Math.Min(_startPoint.X, currentPoint.X);
            double y = Math.Min(_startPoint.Y, currentPoint.Y);
            double w = Math.Abs(currentPoint.X - _startPoint.X);
            double h = Math.Abs(currentPoint.Y - _startPoint.Y);

            // Minimum selection size threshold
            if (w < 10 || h < 10)
            {
                SelectionBox.Visibility = Visibility.Collapsed;
                HintBanner.Visibility = Visibility.Visible;
                return;
            }

            // Calculate crop rect in screen pixels
            int cropX = (int)(x * _dpiScaleX);
            int cropY = (int)(y * _dpiScaleY);
            int cropW = (int)(w * _dpiScaleX);
            int cropH = (int)(h * _dpiScaleY);

            // Clamp
            cropX = Math.Max(0, Math.Min(cropX, _screenBitmap.Width - 1));
            cropY = Math.Max(0, Math.Min(cropY, _screenBitmap.Height - 1));
            cropW = Math.Max(1, Math.Min(cropW, _screenBitmap.Width - cropX));
            cropH = Math.Max(1, Math.Min(cropH, _screenBitmap.Height - cropY));

            try
            {
                using (var cropped = _screenBitmap.Clone(new Rectangle(cropX, cropY, cropW, cropH), _screenBitmap.PixelFormat))
                using (var ms = new MemoryStream())
                {
                    cropped.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    bi.Freeze();

                    CapturedBitmapSource = bi;

                    // Absolute screen coordinates of the selection
                    double screenLeft = Left + x;
                    double screenTop = Top + y;
                    SelectedScreenRect = new Rect(screenLeft, screenTop, w, h);

                    // Also copy to clipboard for user convenience
                    try { Clipboard.SetImage(bi); } catch { }

                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"영역 잘라내기 실패: {ex.Message}", "OverlayPic", MessageBoxButton.OK, MessageBoxImage.Warning);
                DialogResult = false;
            }
            finally
            {
                Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelCapture();
                e.Handled = true;
            }
        }

        private void CancelCapture()
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _screenBitmap?.Dispose();
            _screenBitmap = null;
            base.OnClosed(e);
        }
    }
}
