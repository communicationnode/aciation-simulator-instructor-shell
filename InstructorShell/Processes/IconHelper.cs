using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;

namespace InstructorShell.Processes {

    [SkipLocalsInit]
    public static class IconHelper {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            out SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;

        public static BitmapSource GetFileIcon(string filePath, bool largeIcon = true) {
            try {
                SHFILEINFO shinfo = new SHFILEINFO();
                uint flags = SHGFI_ICON | (largeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON);

                IntPtr hImg = SHGetFileInfo(filePath, 0, out shinfo, (uint)Marshal.SizeOf(shinfo), flags);

                if (shinfo.hIcon != IntPtr.Zero) {
                    Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                    BitmapSource bs = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    // Важно: уничтожить иконку после использования
                    DestroyIcon(shinfo.hIcon);
                    return bs;
                }
            }
            catch { }
            return null;
        }
        public static BitmapSource GetProcessIcon(int processId) {
            try {
                Process process = Process.GetProcessById(processId);
                if (process?.MainModule?.FileName != null) {
                    return IconHelper.GetFileIcon(process.MainModule.FileName);
                }
            }
            catch { }
            return null;
        }

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }
}
