using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Event.EventData {


public  enum BusinessEventDataType {
    LoadingOk,
    /// <summary>
    /// 个人详情准备完成事件
    /// </summary>
    AccountAvaliableEvent,
    /// <summary>
    /// 应用初始化完成事件
    /// </summary>
    ApplicationInitializedEvent,
    /// <summary>
    /// 提示更新未读消息总数量的事件
    /// </summary>
    ChangMessageEvent,
    /// <summary>
    /// 创建群组，选择成员事件
    /// </summary>
    CheckMucMemberEvent,
    /// <summary>
    /// IM连接状态变化事件
    /// </summary>
    ConnectionStatusChangedEvent,
    /// <summary>
    /// 设置好友备注名完成通知事件
    /// </summary>
    ContactsAliasChangeEvent,
    /// <summary>
    /// 联系人发生变化的事件     会话  通讯录
    /// </summary>
    ContactsChangedEvent,

    /// <summary>
    /// 组织机构发生变化的事件     会话  通讯录
    /// </summary>
    OrgChangedEvent,

    /// <summary>
    /// 联系人发生变化的事件 删人
    /// </summary>
    ContactsChangedEvent_TYPE_API_DELETE_Contacts,
    /// <summary>
    /// 联系人详情更新完成通知事件   联系人详情变化  通讯录  chartsession  chartpage
    /// </summary>
    ContactsDetailsChangeEvent,
    /// <summary>
    /// 我的个人信息修改
    /// </summary>
    MyDetailsChangeEvent,
    /// <summary>
    /// 联系人被添加或删除
    /// </summary>
    ContactStatusChangedEvent,
    /// <summary>
    /// ？？？？？？？？？？？？？？
    /// </summary>
    DappCommonUsedUpdateEvent,
    /// <summary>
    /// ？？？？？？？？？？？？？？
    /// </summary>
    DAppOpinionDetailsEvent,
    /// <summary>
    /// 文件准备完成的事件
    /// </summary>
    FileAvailableEvent,
    /// <summary>
    /// 二维码下载完成事件
    /// </summary>
    BarCode2DDownloadedEvent,
    /// <summary>
    /// 二维码下载失败事件
    /// </summary>
    BarCode2DDownloadErrorEvent,
    /// <summary>
    /// 二维码下载进度事件
    /// </summary>
    BarCode2DDownloadingEvent,
    /// <summary>
    /// 文件下载完成事件
    /// 收到文件类型消息（图片、语音、视频、位置等）时，会先现在，并缓存在本地，然后广播出该事件
    /// </summary>
    FileDownloadedEvent,
    /// <summary>
    /// 文件下载失败事件
    /// </summary>
    FileDownloadErrorEvent,
    /// <summary>
    /// 文件下载进度事件
    /// </summary>
    FileDownloadingEvent,
    /// <summary>
    /// 文件上传完成时间
    /// </summary>
    FileUploadedEvent,
    /// <summary>
    /// 文件上传失败事件
    /// </summary>
    FileUploadErrorEvent,
    /// <summary>
    /// 文件上传进度
    /// </summary>
    FileUploadingEvent,
    /// <summary>
    /// 异步获取聊天记录   加载未读记录
    /// </summary>
    GetMessagesEvent,
    /// <summary>
    /// 群成员昵称变更的通知     成员详情    群聊天界面
    /// </summary>
    GroupMemberNicknameChangedEvent,
    /// <summary>
    /// 群成员昵称变更的通知     成员详情    群聊天界面
    /// </summary>
    GroupMemberNicknameChangedEvent_TYPE_UPDATE_ME,
    /// <summary>
    /// 消息到达
    /// </summary>
    MessageChangedEvent,
    /// <summary>
    /// 标识新消息到达
    /// </summary>
    MessageChangedEvent_TYPE_ADD,
    /// <summary>
    /// 标识删除消息
    /// </summary>
    MessageChangedEvent_TYPE_DELETE,
    /// <summary>
    /// 标识消息状态发生改变  发消息的状态
    /// </summary>
    MessageChangedEvent_TYPE_UPDATE,
    /// <summary>
    /// 收到撤回消息
    /// </summary>
    MessageChangedEvent_TYPE_CANCLE,
    /// <summary>
    /// 群聊详情准备完成事件
    /// </summary>
    MucAvaliableEvent,
    /// <summary>
    /// C015 创建群完成事件     新出个会话列表
    /// </summary>
    MucChangeEvent_TYPE_API_CREATE_GROUP,
    /// <summary>
    /// C017 更新群聊名称完成事件  主动改群名称
    /// </summary>
    MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME,
    /// <summary>
    /// 群保存通讯录
    /// </summary>
    MucSavedAsContactChangeEvent,
    /// <summary>
    /// C018 删除/退出群聊 事件
    /// </summary>
    MucChangeEvent_TYPE_API_DELETE_GROUP,
    /// <summary>
    /// C019 增加群聊成员 事件
    /// </summary>
    MucChangeEvent_TYPE_API_ADD_GROUP_MEMBER,
    /// <summary>
    /// C020 移除群聊成员 事件
    /// </summary>
    MucChangeEvent_TYPE_API_DELETE_GROUP_MEMBER,
    /// <summary>
    /// 群成员发生变化 事件
    /// </summary>
    MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE,
    /// <summary>
    /// C036 扫描群成员的二维码加入群聊组 事件
    /// </summary>
    MucChangeEvent_TYPE_API_JOIN_GROUP_SCAN_BARCODE,
    /// <summary>
    /// 群:标识踢人消息
    /// </summary>
    MucEditTextChangedEvent_TYPE_DELETE,
    /// <summary>
    /// 群:标识加人消息
    /// </summary>
    MucEditTextChangedEvent_TYPE_ADD,
    /// <summary>
    /// 群成员更新事件
    /// </summary>
    MucMemberAvaliableEvent,
    /// <summary>
    /// 新的朋友--更新UI
    /// </summary>
    NewFriendsEvent,
    /// <summary>
    /// 公众号列表变化
    /// </summary>
    PublicAccountChangedEvent,

    /// <summary>
    /// 公众号启用事件
    /// </summary>
    PublicAccountAvaliableEvent,
    /// <summary>
    /// 公众号刷新视图事件
    /// </summary>
    PublicAccountFreshenViewEvent,
    /// <summary>
    /// 公众号查找
    /// </summary>
    PublicAccountFindEvent,
    /// <summary>
    /// 公众号被移除事件
    /// </summary>
    PublicAccountRemovedEvent,

    /// <summary>
    /// Api查看公众号详情
    /// </summary>
    PublicAccountDetailedEvent,
    /// <summary>
    /// ?????????????
    /// </summary>
    PublicWebRequestEvent,
    /// <summary>
    /// ??????????
    /// </summary>
    PublicWebSubscribeEvent,
    /// <summary>
    /// 标签事件
    /// </summary>
    TagsEvent,
    /// <summary>
    /// 应用类型事件
    /// </summary>
    ThirdAppClassRequestEvent,
    /// <summary>
    /// 待办生效事件
    /// </summary>
    TodoTaskAvailableEvent,
    /// <summary>
    /// 个人详细准本完成
    /// </summary>
    VcardAvaliableEvent,
    /// <summary>
    /// 语音播放完成事件
    /// </summary>
    VoicePlayFinishEvent,
    /// <summary>
    /// 语音录音完成事件
    /// </summary>
    VoiceRecordedEvent,
    /// <summary>
    /// charsession变更
    /// </summary>
    ChartSessionChangeEvent,
    /// <summary>
    /// 聊天置顶
    /// </summary>
    ChatTopEvent,
    /// <summary>
    /// 聊天详情设置变化
    /// </summary>
    ChatDetailedChangeEvent,
    /// <summary>
    /// 用户点击播放语音
    /// </summary>
    UserClickVoiceStart,
    /// <summary>
    /// 开始播放语音
    /// </summary>
    VoiceStartPlay,
    /// <summary>
    /// 停止播放语音
    /// </summary>
    VoiceStopPlay,
    /// <summary>
    /// 用户被禁用
    /// </summary>
    UserDisabled,
    /// <summary>
    /// 待办事项点击详情
    /// </summary>
    ClickTodoTaskXq,
    /// <summary>
    /// 位置消息点击
    /// </summary>
    ClickLocation,
    /// <summary>
    /// 网盘文件消息点击
    /// </summary>
    ClickDiskFile,
    /// <summary>
    /// 点击公众号
    /// </summary>
    ClickPublic,
    /// <summary>
    /// 点击公众号设置
    /// </summary>
    ClickPublicSetting,
    //主窗体位置或者大小变化
    LocationChanged,
    //选择应用跳转网页
    SelectApp,
    //未读数量 操作消息带红点
    CountOfUnread,
    /// <summary>
    /// 本地文件不存在事件
    /// </summary>
    LocalFileIsNotExistEvent,
    /// <summary>
    /// 跳转到聊天窗口
    /// </summary>
    RedirectChatSessionEvent,

    /// <summary>
    /// 群跳转到群聊天窗口
    /// </summary>
    RedirectMucChatSessionEvent,

    /// <summary>
    /// 跳转到公众号聊天窗口
    /// </summary>
    RedirectPublicChatSessionEvent,

    /// <summary>
    /// 公众号不再关注后返回主界面
    /// </summary>
    RequestCancelGoBack,

    //关闭未读数量
    UnreadControlClose,
    //点击未读数量
    ClickUnreadControl,

    //新消息 关闭
    NewMessageControlClose,
    //点击新消息
    ClickNewMessageControl,
    //@消息 关闭
    AtMessageControlClose,
    //点击@消息
    ClickAtMessageControl,
    /// <summary>
    /// 关闭弹出的网页窗口
    /// </summary>
    ClosePopWebPageWindow,
    /// <summary>
    /// 显示
    /// </summary>
    LoadingWaitShow,
    /// <summary>
    /// 不显示
    /// </summary>
    LoadingWaitClose,
    /// <summary>
    /// 网盘文件消息点击
    /// </summary>
    SendDiskFile,
    /// <summary>
    /// 发送名片消息
    /// </summary>
    SendVcCard,
    /// <summary>
    /// 发送公众号名片消息
    /// </summary>
    SendPublicCard,

    //来语音邀请
    AudioMeetingReceiving,
    //来视频邀请
    VideoMeetingReceiving,

    /// <summary>
    /// 占线通知
    /// </summary>
    AVMeetingBusy,
    /// <summary>
    /// 取消通知
    /// </summary>
    AVMeetingCancel,
    /// <summary>
    /// 已接通通知
    /// </summary>
    AVMeetingConnect,
    /// <summary>
    /// 邀请通知
    /// </summary>
    AVMeetingInvite,
    /// <summary>
    /// 拒绝通知
    /// </summary>
    AVMeetingRefuse,
    /// <summary>
    /// 切换音频通知
    /// </summary>
    AVMeetingSwitch,
    /// <summary>
    /// 超时通知
    /// </summary>
    AVMeetingTimeOut,
    /// <summary>
    /// 刷新视频消息状态
    /// </summary>
    AVMeetingRefresh,

    /// <summary>
    /// 群视频申请
    /// </summary>
    AVGroupMeetingReceiving,

    /// <summary>
    /// 群成员拒绝通知
    /// </summary>
    AVGroupMeetingRefuse,

    /// <summary>
    /// 群成员取消通知
    /// </summary>
    AVGroupMeetingCancel,
    /// <summary>
    /// 租户被移除
    /// </summary>
    UserLeaveTenant,

    ///// <summary>
    ///// 已读消息
    ///// </summary>
    //ReadMessage,

    DisposeIcon
}
}
