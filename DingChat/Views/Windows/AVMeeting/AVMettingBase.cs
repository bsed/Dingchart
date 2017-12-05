using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
public class AVMettingViewModeBase : ViewModelBase {

    /// <summary>
    /// 频道名称 规则：对方NO+时间戳
    /// </summary>
    private string channelName;
    public string ChannelName {
        set {
            channelName = value;
            RaisePropertyChanged("ChannelName");
        }
        get {
            return channelName;
        }
    }

    /// <summary>
    /// 通信 UID
    /// </summary>
    private uint uid;
    public uint Uid {
        set {
            uid = value;
            RaisePropertyChanged("Uid");
        }
        get {
            return uid;
        }
    }

    /// <summary>
    ///     通话时长，秒
    /// </summary>
    private long duration;
    public long Duration {
        set {
            duration = value;
            RaisePropertyChanged("Duration");
            RaisePropertyChanged("DurationFormat");
        }
        get {
            return duration;
        }
    }

    /// <summary>
    ///     是否接通
    /// </summary>
    private bool isConnected;
    public bool IsConnected {
        set {
            isConnected = value;
            RaisePropertyChanged("IsConnected");
        }
        get {
            return isConnected;
        }
    }



    /// <summary>
    ///     通话时长mm:ss格式串
    /// </summary>
    public string DurationFormat {
        get {
            return string.Format("{0:D2}:{1:D2}", duration / 60, duration % 60);
        }
    }

    /// <summary>
    ///     对方头像
    /// </summary>
    private BitmapImage guestAvatar;
    public BitmapImage GuestAvatar {
        set {
            guestAvatar = value;
            RaisePropertyChanged("GuestAvatar");
        }
        get {
            return guestAvatar;
        }
    }

    /// <summary>
    ///     对方姓名
    /// </summary>
    private string guestName;
    public string GuestName {
        set {
            guestName = value;
            RaisePropertyChanged("GuestName");
        }
        get {
            return guestName;
        }
    }
    /// <summary>
    ///     对方userno
    /// </summary>
    private string guestNo;
    public string GuestNo {
        set {
            guestNo = value;
            RaisePropertyChanged("GuestNo");
        }
        get {
            return guestNo;
        }
    }

}
}
