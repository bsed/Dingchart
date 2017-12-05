using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Event.EventData {
public enum EventType {
    /** API请求成功 */
    HttpRequest
    /** API 请求失败 */
    ,HttpRequestError
}

public  enum EventDataType {
    //Login
    //,GetContacts
    //, GetContactsDetaile
    //, GetGroup
    //, GetGroupDetaile
    //, GetGroupMember
    //, GetGroupMemberDetaile
    /** 未知 */
    UNKNOW
    // ----------------------------- IM sta -----------------------------
    /** IM 注册 */
    ,CONFIG_SERVER_URL
    /** IM 注销 */
    ,UNREGISTER_CONFIG_SERVER_URL
    /** 发送消息 */
    , SEND_DISPATCH_QR_MESSAGE
    // ----------------------------- IM end -----------------------------
    // ----------------------------- 登录 sta -----------------------------
    /** A001: 登录*/
    , login
    /** A002: 校验access token*/
    ,verifyCredentials
    /** A003: 退出*/
    ,logout
    /** A004: 获取客户端信息*/
    ,getClientInformation
    /** A005: 手机注册*/
    ,registerByMobile
    /** A006: 用户名注册*/
    ,register
    /** A007: 手机找回密码*/
    ,findPasswordByMobile
    /** A008: 邮件找回密码*/
    ,findPasswordByMail
    /** A009: 微信号登录绑定手机号*/
    ,boundWeixinAndMobile
    /** A010: 发送验证码*/
    ,sendCAPTCHA
    /** 微信登录*/
    ,weixinLogin
    // ----------------------------- 登录 end -----------------------------

    // ----------------------------- 消息 sta -----------------------------
    /** B001: 获取默认消息列表*/
    ,getFixMessageList
    /** B002: 荣联帐号取得*/
    , getUserDetail
    /** B002_1: 获取用户详细信息（和是否为好友无关）*/
    ,getClientUserDetail
    /** B003: 通信好友荣联VOIP帐号取得*/
    , getFriendVoipAccount
    /** B004: 用户头像取得*/
    ,getUserPortraitByVoipAccount
    /** B011: 获取新消息列表*/
    ,getNewMessages
    /** B012: 查找公众号*/
    ,findOfficialAccount
    /** B013: 获得活动列表*/
    ,getCampaignList
    /** B014: 获得活动详情*/
    ,getCampaignDetail
    /** B015: 取消关注公众号*/
    ,unfollowOfficialAccount
    /** B016: 获取群二维码*/
    ,getGroupCode
    // ----------------------------- 消息 end -----------------------------

    // ----------------------------- 通讯录 sta -----------------------------
    /** C001: 获取好友列表*/
    ,friends
    /** C002: 获取好友详细*/
    ,getFriend
    /** C002.2: 获陌生人详细*/
    ,getStranger
    /** C003: 设置好友标签*/
    ,changeTags
    /** C003: 设置好友备注名*/
    ,changeAlias
    /** C004: 标记为星标朋友*/
    ,markFavorite
    /** C005: 查找好友*/
    ,findNewFrind
    /** C005_1: 查找通讯录好友*/
    ,searchContacts
    /** C006: 添加新朋友*/
    ,addFriend
    /** C007: 删除朋友*/
    ,deleteFriend
    /** C008: 发起朋友身份验证请求*/
    ,requestFriend
    /** C008: 接受新朋友请求*/
    ,acceptFriend
    /** C009: 获取单聊会话列表*/
    ,clientUserChats
    /** C010: 设置单聊置顶*/
    ,setTopmost
    /** C011: 设置单聊背景图片*/
    ,updateBackground
    /** C012: 设置单聊免打扰*/
    ,enableNoDisturbFriend
    /** C013: 获取群会话列表*/
    ,groupChats
    /** C013: 获取通讯录中群列表*/
    ,groups
    /** C014: 获取群详细信息*/
    ,getGroup
    /** C014.2: 获取群成员详细信息*/
    ,getGroupMember
    /** C015: 创建群聊*/
    ,createGroup
    /** C016: 群保存到通讯录*/
    ,addGroupToAddressList
    /** C017: 更新群聊名称*/
    ,updateGroupName
    /** C018: 删除群聊*/
    ,deleteGroup
    /** C019: 增加群聊成员*/
    ,addGroupMember
    /** C020: 移除群聊成员*/
    ,deleteGroupMember
    /** C021: 设置群消息免打扰*/
    ,enableGroupNoDisturb
    /** C022: 设置群置顶聊天*/
    ,setGroupTopmost
    /** C023: 设置我在群中的昵称*/
    ,updateNicknameInGroup
    /** C024: 设置群聊天背景*/
    ,updateBackgroundInGroup
    /** C025: 更改群的状态*/
    ,updateGroupChatStatus
    /** C026: 获取标签列表和标签详细*/
    ,tags
    /** C027: 新建标签*/
    ,createTag
    /** C028: 修改标签*/
    ,updateTag
    /** C029: 删除标签*/
    ,deleteTag
    /** C032: 获取组织*/
    ,getOrganization
    /** C033: 获取人员信息（画像）*/
    ,getFriendPicture
    /** C034: 通过手机号获取联系人*/
    ,getFriendByMobile
    /** C036: 扫描群成员的二维码加入群聊组 */
    ,joinGroupByNoAndMember
    /** C037: 查找附近的人*/
    ,searchNearbyUsers

    // ----------------------------- 通讯录 end -----------------------------

    // ----------------------------- 我 sta -----------------------------
    /** E001: 获取我的个人详细信息*/
    ,getMyDetail
    /** E002: 修改我的头像*/
    ,changeAvatar
    /** E003: 设置免打扰*/
    ,enableNoDisturbMe
    /** E004: 设置昵称*/
    ,changeNickname
    /** E005: 设置地区*/
    ,changeCity
    /** E006: 设置性别*/
    ,chanGegender
    /** E007: 设置个人签名*/
    ,changeMoodMessage
    /** E008: 加我为朋友时是否需要验证*/
    ,changeNeedFriendConfirmation
    /** E009: 是否允许向我推荐通讯录朋友*/
    ,changeAllowFindMobileContacts
    /** E010: 是否允许通过登录名称找到我*/
    ,changeAllowFindMeByLoginId
    /** E011: 是否允许通过手机号码找到我*/
    ,changeAllowFindMeByMobileNumber
    /** E012: 获取我的二维码*/
    ,getMyCode
    /** E014: 获取最新版本*/
    ,getLastestVersion
    /** E015: 更新手机号*/
    ,updateMoblie
    /** E016: 更新邮件*/
    ,updateEmail
    /** E018: 修改密码*/
    ,resetPassword
    /** E018_1: 修改密码（通过手机）*/
    ,resetPasswordByMobile
    /** E019: 记录个人的轨迹*/
    ,track
    /** E020: 用户反馈使用当中的意见、建议、需求*/
    ,feedback
    /** E033: 设置地址*/
    ,changeAddress
    /**E034: 设置地理位置	*/
    ,updateLocation
    // ----------------------------- 我 end -----------------------------
    // ----------------------------- 其他 sta -----------------------------
    /** F001: 获取省份列表*/
    ,getProvides
    /** F002: 获取城市列表*/
    ,getCities
    /** F004: 发送文本消息*/
    ,sendTxtMessage
    /** F005: 发送语音消息*/
    ,sendAudioMessage
    /** F006: 发送图片消息*/
    ,sendPictureMessage
    /** F007: 发送地理位置消息*/
    ,sendLocationMessage
    /** F008: 发送视频消息*/
    ,sendVideoMessage
    /** F009: 发送名片消息*/
    ,sendCardMessage
    /** F010: 发送单图文消息*/
    ,sendSingleImageTxtMessage
    /** F011: 发送多图文消息*/
    ,sendMultiImageTxtMessage
    /** F012: 发送通知消息*/
    ,sendInformMessage
    /** F013: 发送文件消息*/
    ,sendFileMessage
    /** F014: 发送业务消息*/
    ,sendBusinessMessage
    /** F015: 发送用户禁用消息*/
    ,sendDisableUserMessage
    /** F016: 发送用户删除消息*/
    ,sendDeleteUserMessage
    /** F017: 发送用户注销消息*/
    ,sendCancelUserMessage
    /** F018: 发送用户打开第三方APP*/
    ,sendOpen3rdApp
    /** F019: 发送不明消息*/
    ,sendUnknownMessage
    /** F020: 发送语音聊天请求*/
    ,sendAudioChatRequest
    /** F021: 同意语音聊天请求*/
    ,agreeAudioChatRequest
    /** F022: 拒绝语音聊天请示*/
    ,refuseAudioChatRequest
    /** F023: 语音聊天*/
    ,keepAudioChat
    /** F024: 发送视频聊天请求*/
    ,sendVideoChatRequest
    /** F025: 同意视频聊天请求*/
    ,agreeVideoChatRequest
    /** F026: 拒绝视频聊天请示*/
    ,refuseVideoChatRequest
    /** F027: 视频聊天*/
    ,keepVideoChat
    // ----------------------------- 其他 end -----------------------------

    // ----------------------------- 文件上传 start -----------------------------
    /** Issue #21: 获取上传路径*/
    ,getUploadUrl
    /** Issue #21: 注册存储记录并执行内容标准化处理*/
    ,registerFile
    /** Issue #21: 获取下载URL*/
    ,getDownloadUrl
    // ----------------------------- 文件上传 end -----------------------------

    // ----------------------------- 二维码 start -----------------------------
    /** 001: 群二维码 */
    ,getGroupQRcode
    /** 002: 我的二维码 */
    ,getMyQRcode
    /** 003: 校验二维码 */
    ,checkQRcode
    /** 004: 下载二维码 */
    ,downloadQRcode
    // ----------------------------- 二维码 end -----------------------------

    // ----------------------------- 公众号 start -----------------------------
    /** 001: 查找开放平台服务帐号 (根据名称模糊匹配) */
    ,searchServiceAccounts
    /** 002: 查看开放平台服务帐号详情 */
    ,getServiceAccountDetails
    /** 003: 获取开放平台服务帐号包含的公众帐号 */
    ,getServiceAccountSubscription
    /** 004: 获取开放平台服务帐号包含的网站应用 */
    ,getServiceAccountWebsite
    /** 005: 获取个人使用的公众帐号服务列表 */
    ,listMySubscriptions
    /** 006: 获取我关注的第三方web应用 个人用户列表 */
    ,listMyWebsites
    /** 006-1: 获取我关注的第三方web应用 个人用户列表 2016.5.6切换新接口*/
    ,apps
    /** 007: 关注开放平台服务帐号包含的公众帐号 */
    ,followSubscription
    /** 008: 取消关注开放平台服务帐号包含的公众帐号 */
    ,unfollowSubscription
    /** 009: 关注并授权开放平台帐号的第三方web应用 个人用户 */
    ,authorizeWebsite
    /** 010: 取消关注并取消授权开放平台帐号的第三方web应用 个人用户 */
    ,unauthorizeWebsite
    /** 011: 点击开放平台服务包含的公众帐号的自定义菜单项 */
    ,clickSubscriptionMenu
    /** 012: 是否启用置顶聊天 */
    ,enableSubscriptionTopmost
    /** 013: 是否允许接收消息 */
    ,allowReceiveSubscriptionMessages
    /** 014: 查找开放平台帐号的第三方web应用 个人用户 */
    ,searchWebsites
    /** 015: 查找全部分类（mobile） */
    ,getMAppClassifications
    /** 015-1: 查找全部分类（mobile）2016.5.7 14:08 接口替换 */
    ,appClassificationGroups
    /** 基础分类 */
    , appBaseGroups
    /**
     * XXX: 通知后台进入了公众号
     */
    , publicNoticeEnterServie
    /**
     * XXX: 根据后台公众号的设置上传位置信息。
     */
    ,publicUploadLocation
    /**
     * 001: 查找近期的（最早60天前）的待办事项
     */
    ,getRecentTodoTasks
    /**
     * 002: 查找待办事项历史记录
     */
    ,getHistoryTodoTasks
    /**
     * 003: 处理待办事项（标记已处理）
     */
    ,processTodoTasks
    // ----------------------------- 公众号 end -----------------------------

    // ----------------------------- 码表 sta -----------------------------
    /** 同步码表 */
    ,syncMasterCode
    // ----------------------------- 码表 end -----------------------------

    // ----------------------------- 组织管理 sta -----------------------------
    /** P001: 获取所有组织机构（团队）列表(MOBILE)*/
    ,getMOrganizations
    /** P002: 获取所有组织用户列表(MOBILE) */
    ,getMOrganizationUsers
    // ----------------------------- 组织管理 end -----------------------------

    // ----------------------------- 其他 sta -----------------------------
    /** ping: 保持session */
    ,ping
    /** 用户访问统计 */
    ,clientUserAccessInfo

    /** 设为常用网站应用接口 */
    ,setCommonWebApp
    /** 取消设为常用网站应用 */
    ,unsetCommonWebApp
    /** 获取打开第三方应用的authCode */
    ,authCode


    // ----------------------------- CA认证 sta -----------------------------
    /** 申请CA证书 */
    ,requestCert
    /** 使用证书登录 */
    ,requsetCertLogin
    /** 生成CSR文件 */
    ,genCSR
    // ----------------------------- CA认证 end -----------------------------

    // ----------------------------- 其他 end -----------------------------
    , setTenantNo
}
}
