using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OverlayPic.Core.Interfaces;

namespace OverlayPic.App.Services
{
    public class PlatformService : IPlatformService
    {
        public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        #region Windows Native Methods
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            return new IntPtr(GetWindowLong32(hWnd, nIndex));
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }
        #endregion

        #region macOS Native Methods (Objective-C Runtime)
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
        private static extern IntPtr sel_registerName(string name);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        private static extern void objc_msgSend_bool(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.Bool)] bool arg);
        #endregion

        public void SetClickThrough(IntPtr windowHandle, bool enable)
        {
            if (windowHandle == IntPtr.Zero) return;

            try
            {
                if (IsWindows)
                {
                    IntPtr currentStyle = GetWindowLongPtr(windowHandle, GWL_EXSTYLE);
                    long styleVal = currentStyle.ToInt64();

                    if (enable)
                    {
                        styleVal |= WS_EX_TRANSPARENT;
                    }
                    else
                    {
                        styleVal &= ~WS_EX_TRANSPARENT;
                    }

                    SetWindowLongPtr(windowHandle, GWL_EXSTYLE, new IntPtr(styleVal));
                }
                else if (IsMacOS)
                {
                    // Call [nsWindow setIgnoresMouseEvents:enable]
                    IntPtr selector = sel_registerName("setIgnoresMouseEvents:");
                    objc_msgSend_bool(windowHandle, selector, enable);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PlatformService] SetClickThrough error: {ex.Message}");
            }
        }

#pragma warning disable CA1416
        public async Task<byte[]?> CaptureScreenRegionAsync(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0) return null;

            return await Task.Run(() =>
            {
                try
                {
                    if (IsWindows)
                    {
                        using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                        }

                        using var ms = new MemoryStream();
                        bmp.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                    else if (IsMacOS)
                    {
                        string tempFile = Path.Combine(Path.GetTempPath(), $"overlay_snip_{Guid.NewGuid():N}.png");
                        var psi = new ProcessStartInfo
                        {
                            FileName = "screencapture",
                            Arguments = $"-R{x},{y},{width},{height} -x \"{tempFile}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using var proc = Process.Start(psi);
                        proc?.WaitForExit(3000);

                        if (File.Exists(tempFile))
                        {
                            byte[] bytes = File.ReadAllBytes(tempFile);
                            try { File.Delete(tempFile); } catch { }
                            return bytes;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[PlatformService] CaptureScreenRegionAsync error: {ex.Message}");
                }
                return null;
            });
        }
#pragma warning restore CA1416

        public void OpenUrl(string url)
        {
            try
            {
                if (IsWindows)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                else if (IsMacOS)
                {
                    Process.Start("open", $"\"{url}\"");
                }
                else
                {
                    Process.Start("xdg-open", $"\"{url}\"");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PlatformService] OpenUrl error: {ex.Message}");
            }
        }
    }
}
