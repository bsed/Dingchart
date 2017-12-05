using GalaSoft.MvvmLight;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
public class ViewModelVideo : AVMettingViewModeBase {

    public ViewModelVideo() {
        isMainWin = false;
        isLocal = false;
    }

    /// <summary>
    /// 是否是主窗体，即大尺寸的视频画面
    /// </summary>
    private bool isMainWin;
    public bool IsMainWin {
        set {
            isMainWin = value;
            RaisePropertyChanged("IsMainWin");
        }
        get {
            return isMainWin;
        }
    }

    /// <summary>
    /// chat用户No
    /// </summary>
    private string userNo;
    public string UserNo {
        set {
            userNo = value;
            RaisePropertyChanged("UserNo");
        }
        get {
            return userNo;
        }
    }

    /// <summary>
    ///     是否是本地用户
    /// </summary>
    private bool isLocal;
    public bool IsLocal {
        set {
            isLocal = value;
            RaisePropertyChanged("IsLocal");
        }
        get {
            return isLocal;
        }
    }




    /// <summary>
    /// 按钮窗体对象
    /// </summary>
    private WinButton buttonWin;
    public WinButton ButtonWin {
        set {
            buttonWin = value;
            RaisePropertyChanged("ButtonWin");
        }
        get {
            return buttonWin;
        }
    }
}
}