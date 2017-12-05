using cn.lds.im.sdk.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using Newtonsoft.Json;

namespace cn.lds.chatcore.pcw.imtp.message {
public class SelectPeopleSubViewModel  {



    private String groupId;    // 群Id
    private String groupImId;      // 群No
    private String memberId;       // 群成员
    private String memberImId;     // 群成员聊天Id
    private String nickName;       // 群成员昵称
    private String avatarStorageRecordId;  // 群成员头像
    private bool isSelected;  // 是否是已经选择了
    private long addRoomTimestemp;  //
    private AVMeetingType meetingType;//当前是否是视频状态
    public string GroupId {
        get {
            return groupId;
        } set {
            groupId = value;
        }
    }

    public string GroupImId {
        get {
            return groupImId;
        } set {
            groupImId = value;
        }
    }

    public string MemberId {
        get {
            return memberId;
        } set {
            memberId = value;
        }
    }

    public string MemberImId {
        get {
            return memberImId;
        } set {
            memberImId = value;
        }
    }

    public string NickName {
        get {
            return nickName;
        } set {
            nickName = value;
        }
    }

    public string AvatarStorageRecordId {
        get {
            return avatarStorageRecordId;
        } set {
            avatarStorageRecordId = value;
        }
    }

    public bool IsSelected {
        get {
            return isSelected;
        } set {
            isSelected = value;
        }
    }

    public long AddRoomTimestemp {
        get {
            return addRoomTimestemp;
        } set {
            addRoomTimestemp = value;
        }
    }

    public AVMeetingType MeetingType {
        get {
            return meetingType;
        } set {
            meetingType = value;
        }
    }
}
}
