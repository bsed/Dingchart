using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cn.lds.chatcore.pcw.Common {

/// <summary>
/// 获取文件的默认图标
/// </summary>
class FileIcon {


    /// <summary>
    /// 文件的默认图标
    /// </summary>
    /// <param Name="fileName">文件的完整路径</param>
    /// <returns></returns>
    public static ImageSource GetFileIcon(string fileName) {
        Icon icon = GetFileIcon(fileName, true);

        return ConvertIconToBitmapSource(icon);
    }

    /// <summary>
    /// 文件的默认图标
    /// </summary>
    /// <param Name="fileName">文件的完整路径</param>
    /// <param Name="largeIcon">是否大图标</param>
    /// <remarks>可以只是文件名，甚至只是文件的扩展名(.*)</remarks>
    /// <remarks>如果想获得.ICO文件所表示的图标，则必须是文件的完整路径。</remarks>
    /// <returns></returns>

    public static Icon GetFileIcon(string fileName, bool largeIcon) {
        SHFILEINFO info = new SHFILEINFO(true);
        int cbFileInfo = Marshal.SizeOf(info);
        SHGFI flags;
        if (largeIcon) {
            flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;
        } else {
            flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
        }


        SHGetFileInfo(fileName, 256, out info, (uint)cbFileInfo, flags);
        return Icon.FromHandle(info.hIcon);
    }

    [DllImport("Shell32.dll")]
    private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);
    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO {
        public SHFILEINFO(bool b) {
            hIcon = IntPtr.Zero;
            iIcon = 0;
            dwAttributes = 0;
            szDisplayName = "";
            szTypeName = "";
        }
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
        public string szTypeName;
    };

    /// <summary>
    /// Icon 转 BitmaoSource
    /// </summary>
    /// <param Name="value"></param>
    /// <returns></returns>
    private static ImageSource ConvertIconToBitmapSource(object value) {
        Icon icon = (Icon)value;
        Bitmap bitmap = icon.ToBitmap();
        IntPtr hBitmap = bitmap.GetHbitmap();
        ImageSource bitmapSource =
            Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        return bitmapSource;
    }

    private enum SHGFI {
        SmallIcon = 0x00000001,
        LargeIcon = 0x00000000,
        Icon = 0x00000100,
        DisplayName = 0x00000200,
        Typename = 0x00000400,
        SysIconIndex = 0x00004000,
        UseFileAttributes = 0x00000010
    }

}
}
