using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using cn.lds.chatcore.pcw.Views.Windows;

namespace cn.lds.chatcore.pcw.Views.Control {
public class CommonMessageBox {
    /// <summary>
    /// CMessageBox显示的按钮类型
    /// </summary>
    public enum MsgBtn {
        OK = 0,
        OKCancel = 1,
        YesNO = 2
    }



    /// <summary>
    /// 消息框的返回值
    /// </summary>
    public enum MsgResult {
        //用户直接关闭了消息窗口
        None = 0,
        //用户点击确定按钮
        OK = 1,
        //用户点击取消按钮
        Cancel = 2,
        //用户点击是按钮
        Yes = 3,
        //用户点击否按钮
        No = 4
    }

    public class Msg {
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param Name="cmessageBoxText">消息内容</param>
        public static MsgResult Show(string cmessageBoxText) {
            CommonMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                window = new CommonMessageBoxWindow();
            }));
            window.MessageBoxText = cmessageBoxText;
            window.OKButtonVisibility = Visibility.Visible;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                window.ShowDialog();
            }));
            return window.Result;
        }



        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param Name="cmessageBoxText">消息内容</param>
        /// <param Name="msgBtn">消息框按钮</param>
        public static MsgResult Show(string cmessageBoxText, MsgBtn msgBtn) {
            CommonMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                window = new CommonMessageBoxWindow();
            }));
            window.MessageBoxText = cmessageBoxText;
            switch (msgBtn) {
            case MsgBtn.OK: {
                window.OKButtonVisibility = Visibility.Visible;
                break;
            }
            case MsgBtn.OKCancel: {
                window.OKButtonVisibility = Visibility.Visible;
                window.CancelButtonVisibility = Visibility.Visible;
                break;
            }
            case MsgBtn.YesNO: {
                window.YesButtonVisibility = Visibility.Visible;
                window.NoButtonVisibility = Visibility.Visible;
                break;
            }

            default: {
                window.OKButtonVisibility = Visibility.Visible;
                break;
            }
            }
            Application.Current.Dispatcher.Invoke(new Action(() => {
                window.ShowDialog();
            }));
            return window.Result;
        }


    }
}
}
