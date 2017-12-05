using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Common.Enums;
using GalaSoft.MvvmLight;

namespace cn.lds.chatcore.pcw.Beans {
public class ChatSessionBean : ViewModelBase {
    /// <summary>
    /// 用户ID
    /// </summary>
    private string account;
    public String Account {
        get {
            return account;
        } set {
            account = value;
            RaisePropertyChanged("Account");
        }
    }

    private string contact;
    /// <summary>
    /// 联系人(用户、群、公众号、系统)
    /// </summary>
    public String Contact {
        get {
            return contact;
        } set {
            contact = value;
            RaisePropertyChanged("Contact");
        }
    }

    private string name;
    /// <summary>
    /// 名字
    /// </summary>
    public String Name {
        get {
            return name;
        } set {
            name = value;
            RaisePropertyChanged("Name");
        }
    }

    private string avatarStorageRecordId;
    /// <summary>
    /// 头像
    /// </summary>
    public String AvatarStorageRecordId {
        get {
            return avatarStorageRecordId;
        } set {
            avatarStorageRecordId = value;
            RaisePropertyChanged("AvatarStorageRecordId");
        }
    }

    private BitmapImage avatarPath;
    public BitmapImage AvatarPath {
        get {
            return avatarPath;
        } set {
            avatarPath = value;
            avatarPath.Freeze();
            RaisePropertyChanged("AvatarPath");

        }
    }

    private String lastMessage;
    /// <summary>
    /// 最新消息内容
    /// </summary>
    public String LastMessage {
        get {
            return lastMessage;
        } set {
            lastMessage = value;
            RaisePropertyChanged("LastMessage");
        }
    }
    private DateTime chatTime;
    /// <summary>
    /// 时间
    /// </summary>
    public DateTime ChatTime {
        get {
            return chatTime;
        } set {
            chatTime = value;
            RaisePropertyChanged("ChatTime");
        }
    }
    private long timestamp;
    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp {
        get {
            return timestamp;
        } set {
            timestamp = value;
            RaisePropertyChanged("Timestamp");
        }
    }
    private string dateStr;
    public string DateStr {
        get {
            return dateStr;
        } set {
            dateStr = value;
            RaisePropertyChanged("DateStr");
        }
    }

    private int newMsgCount;
    /// <summary>
    /// 未读消息数量
    /// </summary>
    public int NewMsgCount {
        get {
            return newMsgCount;
        } set {
            newMsgCount = value;
            RaisePropertyChanged("NewMsgCount");
        }
    }

    private Boolean top;
    /// <summary>
    /// 消息置顶
    /// </summary>
    public Boolean Top {
        get {
            return top;
        } set {
            top = value;
            RaisePropertyChanged("Top");
        }
    }

    private Boolean quiet;
    /// <summary>
    /// 消息免打扰
    /// </summary>
    public Boolean Quiet {
        get {
            return quiet;
        } set {
            quiet = value;
            RaisePropertyChanged("Quiet");
        }
    }
    private Boolean atme;
    /// <summary>
    /// 是否@我
    /// </summary>
    public Boolean Atme {
        get {
            return atme;
        } set {
            atme = value;
            RaisePropertyChanged("Atme");
        }
    }
    private Boolean showYuan;
    public Boolean ShowYuan {
        get {
            return showYuan;
        } set {
            showYuan = value;
            RaisePropertyChanged("ShowYuan");
        }
    }
    private String chatdraft;
    /// <summary>
    /// 草稿
    /// </summary>
    public String Chatdraft {
        get {
            return chatdraft;
        } set {
            chatdraft = value;
            RaisePropertyChanged("Chatdraft");
        }
    }

    private ChatSessionType chatSessionType;
    public ChatSessionType ChatSessionType {
        get {
            return chatSessionType;
        } set {
            chatSessionType = value;
            RaisePropertyChanged("ChatSessionType");
        }
    }

    public string tenantNo {
        get;
        set;
    }

    public string tenantName {
        get;
        set;
    }
}
}
