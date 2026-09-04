using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SharpHook;

namespace OverlayPic.App.Services
{
    public class GlobalHotKeyManager : IDisposable
    {
        private readonly TaskPoolGlobalHook _hook;
        private bool _isDisposed;

        public event Action? OnToggleClickThrough;
        public event Action? OnStartSnip;
        public event Action? OnEscape;

        public GlobalHotKeyManager()
        {
            _hook = new TaskPoolGlobalHook();
            _hook.KeyPressed += Hook_KeyPressed;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _hook.RunAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[GlobalHotKeyManager] Run error: {ex.Message}");
                }
            });
        }

        private void Hook_KeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            var mask = e.RawEvent.Mask;
            var keyCode = e.Data.KeyCode;

            // Check if key code string matches or enum
            string keyName = keyCode.ToString();

            bool isCtrlOrCmd = mask.ToString().Contains("Ctrl") || mask.ToString().Contains("Meta");
            bool isShift = mask.ToString().Contains("Shift");
            bool isAlt = mask.ToString().Contains("Alt");

            if (keyName == "VcEscape")
            {
                OnEscape?.Invoke();
            }
            else if (isCtrlOrCmd && isShift && keyName == "VcT")
            {
                OnToggleClickThrough?.Invoke();
            }
            else if (isCtrlOrCmd && isAlt && keyName == "VcX")
            {
                OnStartSnip?.Invoke();
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _hook.KeyPressed -= Hook_KeyPressed;
                _hook.Dispose();
            }
        }
    }
}
