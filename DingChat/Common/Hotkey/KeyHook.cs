using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace cn.lds.chatcore.pcw.Common.Hotkey {
class KeyHook {
    private HotkeyCallback callback = null;
    public void regediterKey(Window window, HotkeyCallback callback,int keyId,HotKey.KeyModifiers keyModifiers, int vk) {

        this.callback = callback;
        //这句话导致 office控件透明
        //AeroHelper.ExtendGlassFrame(window, new Thickness(-1));
        //注册键盘事件开始
        HwndSource hWndSource;
        WindowInteropHelper wih = new WindowInteropHelper(window);
        hWndSource = HwndSource.FromHwnd(wih.Handle);
        //添加处理程序
        hWndSource.AddHook(MainWindowProc);
        HotKey.RegisterHotKey(wih.Handle, keyId, keyModifiers, vk);
        //注册键盘事件结束
    }

    private IntPtr MainWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
        switch (msg) {
        case HotKey.WM_HOTKEY: {
            int sid = wParam.ToInt32();
            //if (sid == altd) {
            //    //System.Windows.MessageBox.Show("按下Alt+D");
            //    StartCapture();
            //}
            this.callback.KeyCallBack(sid);
            handled = true;
            break;
        }
        }

        return IntPtr.Zero;
    }
}
}
