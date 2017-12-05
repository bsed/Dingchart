using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;
using System.Reflection;

namespace cn.lds.chatcore.pcw.imtp.message {
public enum MsgType {
    [Description("UNKNOWN")]
    [Default]
    UNKNOWN = -1     /* 未知 */,
    [Description("Text")]
    Text = 1        /* 文本   对应：Text(1) */,
    [Description("Voice")]
    Voice = 2        /* 语音   对应：Voice(2) */,
    [Description("Image")]
    Image = 3       /* 图片   对应：Picture(3) */,
    [Description("Location")]
    Location = 4     /* 位置   对应：Location(4) */,
    [Description("Video")]
    Video = 5        /* 视频   对应：Video(5) */,
    [Description("VCard")]
    VCard = 6       /* 名片   对应：Vcard(6) */,
    [Description("News")]
    News = 7         /* 多图文 对应：MultiHtml(8) */,
    [Description("File")]
    File = 8         /* 文件         对应：File(9) */,
    [Description("Link")]
    Link = 9         /* 链接 */,
    [Description("Music")]
    Music = 10       /* 音乐 */,
    [Description("At")]
    At = 11         /* @我 */,
    [Description("PublicCard")]
    PublicCard = 12 /* 公众号名片消息 */,
    [Description("Activity")]
    Activity= 13    /* 活动消息 */,
    [Description("Business")]
    Business = 14,   /* 业务消息 */
    [Description("App")]
    App = 15,       /* 应用消息 */
    //通知类消息
    //系统级
    [Description("OtherLogined")]
    OtherLogined = 101       /* 未知 */,
    [Description("Cancel")]
    Cancel = 110,            /* 撤销消息 */
    [Description("FriendRequest")]
    FriendRequest = 121,     /* A加B，B收到的。好友申请通知消息 */
    [Description("FriendResponse")]
    FriendResponse = 122,    /* A加B，B通过验证或回复消息，给A发的好友添加成功通知消息 */
    [Description("ContactAdded")]
    ContactAdded = 123,      //A加B，不用验证，给B的通知
    [Description("ContactDeleted")]
    ContactDeleted = 124,    /* A删除B，给B的通知。好友删除通知消息 */
    [Description("UserAvatarChanged")]
    UserAvatarChanged = 125, /* 好友头像变更通知消息 */
    [Description("UserNicknameChanged")]
    UserNicknameChanged = 126,   /* 好友昵称变更通知消息 */
    [Description("GroupMemberAdded")]
    GroupMemberAdded= 141,      /* 群成员加入群消息     对应：GroupMemberAdded(20)*/
    [Description("GroupMemberDeleted")]
    GroupMemberDeleted = 142,    /* 群成员退出群消息     对应：GroupDeleteMember(21)*/
    [Description("GroupMemberExited")]
    GroupMemberExited = 143,     /* 用户主动退群： 群成员退出群消息 */
    [Description("GroupNameChanged")]
    GroupNameChanged = 144,      /* 群名称变更消息 */
    [Description("GroupLogoChanged")]
    GroupLogoChanged = 145,      /* 群LOGO变更消息 */
    [Description("GroupMemberNicknameChanged")]
    GroupMemberNicknameChanged = 146,    /* 群成员昵称变更消息 */
    [Description("ReadMessage")]
    ReadMessage = 147,    /* 已读消息 同步到pc或者手机 */

    [Description("GroupSavedAsContact")]
    GroupSavedAsContact = 148,    /* 群保存通讯录 */

    [Description("Ticket")]
    Ticket = 151,                    /* 代金卷消息 */
    [Description("Product")]
    Product = 152,                   /* 产品消息 */
    [Description("OrganizationChanged")]
    OrganizationChanged = 161,       /* 组织变更消息 */
    [Description("OrganizationMemberChanged")]
    OrganizationMemberChanged = 162, /* 组织成员变更消息 */
    [Description("TodoTask")]
    TodoTask = 181,                   /* 待办消息 */
    [Description("PublicListChanged")]
    PublicListChanged = 200,        /* 公众号列表变更消息 */
    [Description("PublicWebsiteListChanged")]
    PublicWebsiteListChanged = 201,        /* 应用列表变更消息 */
    [Description("ThirdAppClassChanged")]
    ThirdAppClassChanged = 202,        /* 应用分类变更消息 */
    [Description("UserDisabled")]
    UserDisabled = 210,               /* 用户禁用消息 */

    [Description("LoginRequest")]
    LoginRequest = 230,               /* 登录请求消息 */
    [Description("LoginWaitting")]
    LoginWaitting = 231,               /* 等待登录 */
    [Description("LoginWaittingCancel")]
    LoginWaittingCancel = 232,               /* 取消等待 */
    [Description("LoginAuthorization")]
    LoginAuthorization = 233,               /* 授权登录消息 */
    [Description("LoginStatus")]
    LoginStatus = 234,               /* 登录状态消息 */
    [Description("LoginQuit")]
    LoginQuit = 235,               /* 退出登录消息 */

    [Description("AVMeetingInvite")]
    AVMeetingInvite = 250,/*邀请通知*/
    [Description("AVMeetingRefuse")]
    AVMeetingRefuse = 251,/*拒绝通知*/
    [Description("AVMeetingCancel")]
    AVMeetingCancel = 252,/*取消通知*/
    [Description("AVMeetingTimeout")]
    AVMeetingTimeOut = 253,/*超时通知*/
    [Description("AVMeetingBusy")]
    AVMeetingBusy = 254,/*占线通知*/
    [Description("AVMeetingSwitch")]
    AVMeetingSwitch = 255,/*切换音频通知*/
    [Description("AVMeetingConnect")]
    AVMeetingConnect = 256,/*已接通通知*/

    [Description("MessageTypeAVGroupInvite")]
    MessageTypeAVGroupInvite = 260,/*群视频邀请*/

    [Description("MessageTypeAVGroupRefuse")]
    MessageTypeAVGroupRefuse = 261,/*群成员拒绝通知*/

    [Description("MessageTypeAVGroupCancel")]
    MessageTypeAVGroupCancel = 262,/*取消通知*/

    [Description("MessageTypeAVGroupOvertime")]
    MessageTypeAVGroupOvertime = 263,/*超时通知*/

    [Description("MessageTypeUserJoinTenant")]
    MessageTypeUserJoinTenant = 170,/*用户加入租户*/

    [Description("MessageTypeUserLeaveTenant")]
    MessageTypeUserLeaveTenant = 171,/*用户退出租户*/
    [Description("UserRestart")]
    UserRestart = 172,               /* 用户主动重启 */
    [Description("Notify")]
    Notify = 999,


}
}
