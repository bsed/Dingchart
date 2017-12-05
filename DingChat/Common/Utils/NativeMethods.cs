using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Windows;

namespace cn.lds.chatcore.pcw.Common.Utils {
class NativeMethods {
    #region Fields

    //Flash both the window caption and taskbar button.
    //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
    public const UInt32 FLASHW_ALL = 3;

    //Flash the window caption.
    public const UInt32 FLASHW_CAPTION = 1;

    //Stop flashing. The system restores the window to its original state.
    public const UInt32 FLASHW_STOP = 0;

    //Flash continuously, until the FLASHW_STOP flag is set.
    public const UInt32 FLASHW_TIMER = 4;

    //Flash continuously until the window comes to the foreground.
    public const UInt32 FLASHW_TIMERNOFG = 12;

    //Flash the taskbar button.
    public const UInt32 FLASHW_TRAY = 2;

    #endregion Fields

    #region Methods

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    /// <summary>
    /// Flashes a window until the window comes to the foreground
    /// Receives the form that will flash
    /// </summary>
    /// <param Name="hWnd">The handle to the window to flash</param>
    /// <returns>whether or not the window needed flashing</returns>
    public static bool FlashWindowEx(Form frm) {
        IntPtr hWnd = frm.Handle;
        FLASHWINFO fInfo = new FLASHWINFO();

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
        fInfo.hwnd = hWnd;
        fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
        fInfo.uCount = UInt32.MaxValue;
        fInfo.dwTimeout = 0;

        return FlashWindowEx(ref fInfo);
    }

    /// <summary>
    /// Flashes a window
    /// </summary>
    /// <param Name="hWnd">The handle to the window to flash</param>
    /// <returns>whether or not the window needed flashing</returns>
    public static bool FlashWindowEx(IntPtr hWnd) {
        FLASHWINFO fInfo = new FLASHWINFO();

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
        fInfo.hwnd = hWnd;
        fInfo.dwFlags = FLASHW_ALL;
        fInfo.uCount = UInt32.MaxValue;
        fInfo.dwTimeout = 0;

        return FlashWindowEx(ref fInfo);
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    #endregion Methods

    #region Nested Types

    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public UInt32 dwFlags;
        public UInt32 uCount;
        public UInt32 dwTimeout;
    }

    #endregion Nested Types
}
}