using cn.lds.chatcore.pcw.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common.Enums;
using java.lang;
using Boolean = System.Boolean;
using Exception = System.Exception;
using Math = System.Math;
using Object = System.Object;
using String = System.String;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.imtp.message;

namespace cn.lds.chatcore.pcw.Event.EventData {
class BusinessEventData {
}
/// <summary>
/// 群成员昵称变更
/// </summary>
public class GroupMemberNicknameChangedEventData {
    public String mucId;
    public String mucNo;
    public String memberId;
    public String meberNo;
    public String changeName;
}

/// <summary>
/// 媒体处理（语音&视频）
/// </summary>
public class MediaEventData {
    // 播放的文件ID
    public string fileStorageId;
    // 校验标识
    public string checkMark;
    // 扩展参数
    public Dictionary<String, Object> extras;

}

/// <summary>
/// 文件操作类
/// </summary>
public class FileEventData {
    public String fileName;
    // 文件编号
    public string fileStorageId;

    //已下载大小
    public long currentSize;
    //总大小
    public long totalSize;
    // 扩展参数
    public Dictionary<String, Object> extras;
    // 业务编号
    public String businessId;
    // 进度百分比
    public int percent;
}

/// <summary>
/// 文件上传
/// </summary>
public class FileUploadEventData {
    /// <summary>
    /// 文件的上传地址
    /// </summary>
    public String url;

    // 上传文件的结果
    public string id;
    public String createdBy;
    public long createdDate;
    public String lastModifiedBy;
    public long lastModifiedDate;
    public String objectType;
    public String filePath;
    public String fileName;
    public long fileSize;
    public String thumbnailFilePath;
    public String originalFilePath;
    public long duration;
    public String businessId;
    public Dictionary<String, Object> extras;
    public UploadFileType uploadFileType;
}

/// <summary>
/// 群：加人或踢人的消息通知数据
/// </summary>
public class MucEditTextChangedEventData {
    /* 标记消息的变化类型 */
    public String type;

    public String mucid;

    public String mucNo;

    public String userNo;
}

/// <summary>
/// 应用事件
/// </summary>
public class PublicWebSubscribeEventData {

    public Boolean followed;
    public String appId;
    public Boolean status;
}

/// <summary>
/// 位置消息数据
/// </summary>
public class LocationEventData {

    public String address;

    public String picture;//Base64编码缩略图内容

    public double longitude;

    public double latitude;

    public int scale;
}

/// <summary>
/// 发送网盘消息数据
/// </summary>
public class DiskFileEventData {
    // 用于点击网盘后，传递参数使用。目前是否有用还不知道，预留着吧。
}


/// <summary>
/// 发送网盘文件消息数据
/// </summary>
public class SendDiskFileEventData {

    public List<DiskFileBean> diskFiles;
}

/// <summary>
/// 占线通知数据
/// </summary>
public class AVMeetingBusyEventData {
    public AVMeetingBusyMessage message;
}

/// <summary>
/// 取消通知数据
/// </summary>
public class AVMeetingCancelEventData {
    public AVMeetingCancelMessage message;
}

/// <summary>
/// 已接通通知数据
/// </summary>
public class AVMeetingConnectEventData {
    public AVMeetingConnectMessage message;
}

/// <summary>
/// 邀请通知数据
/// </summary>
public class AVMeetingInviteEventData {
    // 呼叫类型
    public AVMeetingCallType callType;
    public AVMeetingInviteMessage message;
}

/// <summary>
/// 拒绝通知数据
/// </summary>
public class AVMeetingRefuseEventData {
    public AVMeetingRefuseMessage message;
}

/// <summary>
/// 切换音频通知数据
/// </summary>
public class AVMeetingSwitchEventData {
    public AVMeetingSwitchMessage message;
}

/// <summary>
/// 超时通知数据
/// </summary>
public class AVMeetingTimeOutEventData {
    public AVMeetingTimeOutMessage message;
}


/// <summary>
/// 邀请群视频通知数据
/// </summary>
public class MessageTypeAVGroupInviteEventData {
    // 呼叫类型
    public AVMeetingCallType callType;
    public MessageTypeAVGroupInviteMessage message;
}
}
