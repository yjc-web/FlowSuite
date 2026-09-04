using System;
using System.Windows;
using System.Windows.Input;

namespace OverlayPic
{
    public partial class FineTuningWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private int _jogStep = 1;

        public FineTuningWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            Owner = _mainWindow;
        }

        public int JogStep => _jogStep;

        public void UpdateCoordinates(int x, int y, int w, int h)
        {
            if (PosText != null) PosText.Text = $"X: {x}   Y: {y}";
            if (SizeText != null) SizeText.Text = $"W: {w}   H: {h}";
        }

        public void UpdateLanguage(bool isEn)
        {
            if (TitleText != null) TitleText.Text = isEn ? "Fine Tuning" : "미세 정렬";
            if (HintText != null) HintText.Text = isEn ? "Wheel: Y  |  Shift+Wheel: X" : "휠: Y이동  |  Shift+휠: X이동";
            if (StepBtn != null) StepBtn.ToolTip = isEn ? "Toggle step (1px / 10px)" : "이동 단위 변경 (1px / 10px)";
            if (UpBtn != null) UpBtn.ToolTip = isEn ? "Move Up (↑)" : "위로 이동 (↑)";
            if (DownBtn != null) DownBtn.ToolTip = isEn ? "Move Down (↓)" : "아래로 이동 (↓)";
            if (LeftBtn != null) LeftBtn.ToolTip = isEn ? "Move Left (←)" : "왼쪽으로 이동 (←)";
            if (RightBtn != null) RightBtn.ToolTip = isEn ? "Move Right (→)" : "오른쪽으로 이동 (→)";
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void Step_Click(object sender, RoutedEventArgs e)
        {
            _jogStep = (_jogStep == 1) ? 10 : 1;
            if (StepBtn != null) StepBtn.Content = $"{_jogStep}px";
            if (CenterStepText != null) CenterStepText.Text = $"{_jogStep}px";
        }

        public bool IsClosing { get; set; } = false;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (!IsClosing)
            {
                IsClosing = true;
                Close();
            }
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.NudgeWindow(0, -step);
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.NudgeWindow(0, step);
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.NudgeWindow(-step, 0);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.NudgeWindow(step, 0);
        }

        private void WidthMinus_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.ResizeWindow(-step, 0);
        }

        private void WidthPlus_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.ResizeWindow(step, 0);
        }

        private void HeightMinus_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.ResizeWindow(0, -step);
        }

        private void HeightPlus_Click(object sender, RoutedEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            _mainWindow?.ResizeWindow(0, step);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            int delta = e.Delta > 0 ? -step : step;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                _mainWindow?.ResizeWindow(-delta, -delta);
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                int xDelta = e.Delta > 0 ? step : -step;
                _mainWindow?.NudgeWindow(xDelta, 0);
            }
            else
            {
                _mainWindow?.NudgeWindow(0, delta);
            }
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!IsClosing)
                {
                    IsClosing = true;
                    Close();
                }
                e.Handled = true;
                return;
            }

            int step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 10 : _jogStep;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (e.Key == Key.Left) { _mainWindow?.ResizeWindow(-step, 0); e.Handled = true; }
                else if (e.Key == Key.Right) { _mainWindow?.ResizeWindow(step, 0); e.Handled = true; }
                else if (e.Key == Key.Up) { _mainWindow?.ResizeWindow(0, -step); e.Handled = true; }
                else if (e.Key == Key.Down) { _mainWindow?.ResizeWindow(0, step); e.Handled = true; }
            }
            else
            {
                if (e.Key == Key.Left) { _mainWindow?.NudgeWindow(-step, 0); e.Handled = true; }
                else if (e.Key == Key.Right) { _mainWindow?.NudgeWindow(step, 0); e.Handled = true; }
                else if (e.Key == Key.Up) { _mainWindow?.NudgeWindow(0, -step); e.Handled = true; }
                else if (e.Key == Key.Down) { _mainWindow?.NudgeWindow(0, step); e.Handled = true; }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsClosing = true;
            // Sync toggle state on main window asynchronously after closing completes
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_mainWindow?.JogModeToggle != null && _mainWindow.JogModeToggle.IsChecked == true)
                {
                    _mainWindow.JogModeToggle.IsChecked = false;
                }
            }));
        }
    }
}
