using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Views.Control;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
/// <summary>
///     WinVideo.xaml 的交互逻辑
/// </summary>
public partial class WinVideo : Window {


    /// <summary>
    ///     是否在线
    /// </summary>
    public bool IsOnLine = true;

    public ViewModelVideo ViewModel;
    public WinVideo() {
        InitializeComponent();
        ViewModel = new ViewModelVideo();
        this.DataContext = ViewModel;
    }


    private void WinVideo_OnLoaded(object sender, RoutedEventArgs e) {
        var hwnd = new WindowInteropHelper(this).Handle;
        NativeWindowHelper.SetWindowNoBorder(hwnd);
        if (!this.ViewModel.IsMainWin) {
            this.Height = 120;
            this.Width = 90;
        }
    }

    private void WinVideo_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
        if (ViewModel.ButtonWin != null) {
            ViewModel.ButtonWin.WinVideo_OnMouseDoubleClick(sender, e);
        }
    }

    private void VideoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        if (AgoraEngine.Instance.VideoWindow !=null && AgoraEngine.Instance.VideoWindow.FormFocus == false) {
            e.Cancel = true;
            if (CommonMessageBox.Msg.Show("关闭窗口将终止视频聊天，确定关闭么 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                if (App.AmStatus == AVMeetingStatus.contected) {
                    AgoraEngine.Instance.VideoWindow.CloseMethod(false, true);
                } else {
                    AgoraEngine.Instance.VideoWindow.CloseMethod(true, true);
                }
            }

        } else {
            AgoraEngine.Instance.VideoWindow = null;
        }
    }
}
}