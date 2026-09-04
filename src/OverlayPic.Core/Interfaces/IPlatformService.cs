using System;
using System.Threading.Tasks;

namespace OverlayPic.Core.Interfaces
{
    public interface IPlatformService
    {
        bool IsWindows { get; }
        bool IsMacOS { get; }
        bool IsLinux { get; }

        /// <summary>
        /// Enable or disable mouse click-through for a specific window handle
        /// </summary>
        void SetClickThrough(IntPtr windowHandle, bool enable);

        /// <summary>
        /// Capture a rectangular region from screen and return raw PNG/Bitmap bytes
        /// </summary>
        Task<byte[]?> CaptureScreenRegionAsync(int x, int y, int width, int height);

        /// <summary>
        /// Open a web URL in the system's default browser
        /// </summary>
        void OpenUrl(string url);
    }
}
