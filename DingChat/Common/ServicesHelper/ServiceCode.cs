using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.Common.Services {
class ServiceCode {

    /// <summary>
    /// 超时等待时间
    /// </summary>
    public const int RequestTimeout = 40000;

    /// <summary>
    /// 登陆URL
    /// </summary>
    public const string ServerLoginUrl ="/uap/login";

    /// <summary>
    /// 获得好友列表
    /// </summary>
    public const string ServerGetContacts = "/core/m/user/contacts?t=";

    /// <summary>
    /// 获得好友详细信息/core/m/user/contacts/{id}
    /// </summary>
    public const string ServerGetContactsDetaile = "/core/m/user/contacts/";

    /// <summary>
    /// 获得陌生人详细信息
    /// </summary>
    public const string ServerGetStrangerDetaile = "/core/m/user/stranger/";

    /// <summary>
    /// 获得群详细信息/core/m/user/groups/G1N8BLKVX5RX8
    /// </summary>
    public const string ServerGetGroupContactsDetaile = "/core/m/user/groups/";

    /// <summary>
    /// 获得群列表
    /// </summary>
    public const string ServerGetGroupContacts = "/core/m/user/groups/groupChats?t=";

    /// <summary>
    /// 下载URL
    /// </summary>
    public const string ServerDownloadUrl = "/download/";

    /// <summary>
    /// 获取我的详细信息
    /// </summary>
    public const string ServerGetMyDetail = "/core/m/user/getMyDetail";


    //-------------------------------------------------------------------------------------

    /// <summary>
    /// 服务器IP&端口
    /// </summary>
    public static String SERVER_HOST = ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port;
    public static String HTTPS_SERVER_HOST = ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port;

    // ----------------------------- IM sta-----------------------------
    /**
     * 获取IM服务器地址
     */
    public static String CONFIG_SERVER_URL = SERVER_HOST + "/tms/api/mobile/connection/registerDevice";
    /**
     * 注销
     */
    public static String UNREGISTER_CONFIG_SERVER_URL = SERVER_HOST + "/tms/api/mobile/connection/unregisterDevice";

    /**
    * 获取IM服务器地址（扫码登录用）
    */
    public static String CONFIG_SERVER_URL_QRCODE = SERVER_HOST + "/tms/api/pc/connection/qrRegisterDevice";
    /**
     * 注销（扫码登录用）
     */
    public static String UNREGISTER_CONFIG_SERVER_URL_QRCODE = SERVER_HOST + "/tms/api/pc/connection/qrUnRegisterDevice";
    /**
    * 获取登录二维码地址（扫码登录用）
    */
    public static String GET_LOGIN_QRCODE = SERVER_HOST + "/core/pc/login/qrcode/{deviceId}";
    /**
    * 发送消息
    */
    public static String SEND_DISPATCH_QR_MESSAGE = SERVER_HOST + "/dps/api/message/fromexternal/dispatchQrMessage/{deviceId}";

    // ----------------------------- IM end-----------------------------

    // ----------------------------- 登录 sta -----------------------------
    /**
    * A001: 登录
    */
    public static String login = SERVER_HOST + "/uap/login"; // 租户：tenant[]
    //    public static  String login = HTTPS_SERVER_HOST + "/uap/login";

    /**
     * A002: 校验access token
     */
    public static String verifyCredentials = SERVER_HOST + "/chat/s/oauth/Account/verify_credentials";
    /**
     * A003: 退出
     */
    public static String logout = SERVER_HOST + "/uap/logout";
    /**
     * A004: 获取客户端信息
     */
    public static String getClientInformation = SERVER_HOST + "/chat/s/chat/app/client";
    /**
     * A005: 手机注册
     */
    public static String registerByMobile = SERVER_HOST + "/uap/mobile/register";
    /**
     * A006: 用户名注册
     */
    public static String register = SERVER_HOST + "/chat/register";
    /**
     * A007: 手机找回密码
     */
    public static String findPasswordByMobile = SERVER_HOST + "/uap/user/password";
    /**
     * A008: 邮件找回密码
     */
    public static String findPasswordByMail = SERVER_HOST + "/XXX";
    /**
     * A009: 微信号登录绑定手机号
     */
    public static String boundWeixinAndMobile = SERVER_HOST + "/chat/boundWeixinAndMobile";
    /**
     * A010: 发送验证码
     */
    public static String sendCAPTCHA = SERVER_HOST + "/uap/mobile/captcha?mobile={mobile}&scope=register";

    public static String weixinLogin = SERVER_HOST + "/m/wechat/login";
    // ----------------------------- 登录 end -----------------------------

    // ----------------------------- 消息 sta -----------------------------
    /**
     * B001: 获取默认消息列表
     */
    public static String getFixMessageList = SERVER_HOST + "/XXX";
    /**
     * B002: 荣联帐号取得
     */
    public static String getUserDetail = SERVER_HOST + "/chat/s/user/getUserDetail";
    /**
     * B002_1: 获取用户详细信息（和是否为好友无关）
     */
    public static String getClientUserDetail = SERVER_HOST + "/core/w/clientUsers/details/{no}";
    /**
     * B003: 通信好友荣联VOIP帐号取得
     */
    public static String getFriendVoipAccount = SERVER_HOST + "/chat/s/user/getFriendVoipAccount";
    /**
     * B004: 用户头像取得
     */
    public static String getUserPortraitByVoipAccount = SERVER_HOST + "/chat/s/user/getUserPortraitByVoipAccount";

    /**
     * B011: 获取新消息列表
     */
    public static String getNewMessages = SERVER_HOST + "/XXX";
    /**
     * B012: 查找公众号
     */
    public static String findOfficialAccount = SERVER_HOST + "/XXX";
    /**
     * B013: 获得活动列表
     */
    public static String getCampaignList = SERVER_HOST + "/a/getCampaigns";
    /**
     * B014: 获得活动详情
     */
    public static String getCampaignDetail = SERVER_HOST + "/XXX";
    /**
     * B015: 取消关注公众号
     */
    public static String unfollowOfficialAccount = SERVER_HOST + "/XXX";
    /**
     * B016: 获取群二维码
     */
    public static String getGroupCode = SERVER_HOST + "/XXX";

    // ----------------------------- 消息 end -----------------------------

    // ----------------------------- 通讯录 sta -----------------------------
    /**
     * C001: 获取好友列表
     */
    public static String friends = SERVER_HOST + "/core/m/user/contacts?t={t}";
    /**
     * C002: 获取好友详细
     */
    public static String getFriend = SERVER_HOST + "/core/m/user/contacts/{id}";
    /**
     * C002.2: 获陌生人详细
     */
    public static String getStranger = SERVER_HOST + "/core/m/user/stranger/{id}";

    /**
     * C003: 设置好友标签
     */
    public static String changeTags = SERVER_HOST + "/core/m/user/contacts/{id}/changeTags";
    /**
     * C003: 设置好友备注名
     */
    public static String changeAlias = SERVER_HOST + "/core/m/user/contacts/{id}/changeAlias";
    /**
     * C004: 标记为星标朋友
     */
    public static String markFavorite = SERVER_HOST + "/core/m/user/contacts/{id}/markFavorite";
    /**
     * C005: 查找好友
     */
    public static String findNewFrind = SERVER_HOST + "/core/m/users/search?q={q}";
    /**
     * C005_1: 查找通讯录好友
     */
    public static String searchContacts = SERVER_HOST + "/core/m/users/search";

    /**
     * C006: 添加新朋友
     */
    public static String addFriend = SERVER_HOST + "/core/m/user/contacts/addFriend";
    /**
     * C007: 删除朋友
     */
    public static String deleteFriend = SERVER_HOST + "/core/m/user/contacts/{id}";
    /**
     * C008.1: 发起朋友身份验证请求
     */
    public static String requestFriend = SERVER_HOST + "/core/m/user/contacts/validation/requestFriend";
    /**
     * C008: 接受新朋友请求
     */
    public static String acceptFriend = SERVER_HOST + "/core/m/user/contacts/validation/{id}/acceptFriend";

    /**
     * C009: 获取单聊会话列表
     */
    public static String clientUserChats = SERVER_HOST + "/core/m/user/clientUserChats?t={t}";
    /**
     * C010: 设置单聊置顶
     */
    public static String setTopmost = SERVER_HOST + "/core/m/user/clientUserChats/{id}/setTopmost";
    /**
     * C011: 设置单聊背景图片
     */
    public static String updateBackground = SERVER_HOST + "/core/m/user/clientUserChats/{id}/updateBackground";
    /**
     * C012: 设置单聊免打扰
     */
    public static String enableNoDisturbFriend = SERVER_HOST + "/core/m/user/clientUserChats/{id}/enableNoDisturb";
    /**
     * C013: 获取群会话列表
     */
    public static String groupChats = SERVER_HOST + "/core/m/user/groups/groupChats?t={t}";
    /**
     * C013: 获取通讯录中群列表
     */
    public static String groups = SERVER_HOST + "/core/m/user/groups?t={t}";
    /**
     * C014: 获取群详细信息
     */
    public static String getGroup = SERVER_HOST + "/core/m/user/groups/{group}";
    /**
     * C014.2: 获取群成员详细信息 /core/m/user/groups/{groupid}/{clientuserid}
     */
    public static String getGroupMember = SERVER_HOST + "/core/m/user/groups/{groupid}/{clientuserid}";
    /**
     * C015: 创建群聊
     */
    public static String createGroup = SERVER_HOST + "/core/m/user/groups";
    /**
     * C016: 群保存到通讯录
     */
    public static String addGroupToAddressList = SERVER_HOST + "/core/m/user/groups/{mucid}/addGroupToAddressList";
    /**
     * C017: 更新群聊名称
     */
    public static String updateGroupName = SERVER_HOST + "/core/m/user/groups/{id}/updateGroupName";
    /**
     * C018: 删除群聊
     */
    public static String deleteGroup = SERVER_HOST + "/core/m/user/groups/{mucid}";
    /**
     * C019: 增加群聊成员
     */
    public static String addGroupMember = SERVER_HOST + "/core/m/user/groups/{mucid}/addGroupMembers";
    /**
     * C020: 移除群聊成员
     */
    public static String deleteGroupMember = SERVER_HOST + "/core/m/user/groups/{mucid}/deleteGroupMembers";
    /**
     * C021: 设置群消息免打扰
     */
    public static String enableGroupNoDisturb = SERVER_HOST + "/core/m/user/groups/{mucid}/enableNoDisturb";
    /**
     * C022: 设置群置顶聊天
     */
    public static String setGroupTopmost = SERVER_HOST + "/core/m/user/groups/{mucid}/setTopmost";
    /**
     * C023: 设置我在群中的昵称
     */
    public static String updateNicknameInGroup = SERVER_HOST + "/core/m/user/groups/{mucid}/updateNickname";
    /**
     * C024: 设置群聊天背景
     */
    public static String updateBackgroundInGroup = SERVER_HOST + "/core/m/user/groups/{mucid}/updateBackground";
    /**
     * C025: 更改群的状态
     */
    public static String updateGroupChatStatus = SERVER_HOST + "/core/m/user/groups/{mucid}/active";
    /**
     * C026: 获取标签列表和标签详细
     */
    public static String tags = SERVER_HOST + "/core/m/user/tags?t={t}";
    /**
     * C027: 新建标签
     */
    public static String createTag = SERVER_HOST + "/core/m/user/tags";
    /**
     * C028: 修改标签
     */
    public static String updateTag = SERVER_HOST + "/core/m/user/tags/{id}";
    /**
     * C029: 删除标签
     */
    public static String deleteTag = SERVER_HOST + "/core/m/user/tags/{id}";
    /**
     * C032: 获取组织
     */
    public static String getOrganization = SERVER_HOST + "/chat/s/organization";
    /**
     * C033: 获取人员信息（画像）
     */
    public static String getFriendPicture = SERVER_HOST + "/XXX";
    /**
     * C034: 通过手机号获取联系人
     */
    public static String getFriendByMobile = SERVER_HOST + "/chat/s/Contact";
    /**
     * C036: 扫描群成员的二维码加入群聊组
     */
    public static String joinGroupByNoAndMember = SERVER_HOST + "/core/m/user/groups/{MucNo}/scan/{userNo}/join";

    /**
     * C037: 查找附近的人
     */
    public static String searchNearbyUsers = SERVER_HOST + "/core/m/searchNearby?longitude={longitude}&latitude={latitude}";
    // ----------------------------- 通讯录 end -----------------------------

    // ----------------------------- 我 sta -----------------------------
    /**
     * E001: 获取我的个人详细信息
     */
    public static String getMyDetail = SERVER_HOST + "/core/m/user/getMyDetail";
    /**
     * E002: 修改我的头像
     */
    public static String changeAvatar = SERVER_HOST + "/core/m/user/changeAvatar";
    /**
     * E003: 设置免打扰
     */
    public static String enableNoDisturbMe = SERVER_HOST + "/core/m/user/changeNoDisturb";
    /**
     * E004: 设置昵称
     */
    public static String changeNickname = SERVER_HOST + "/core/m/user/changeNickname";
    /**
     * E005: 设置地区
     */
    public static String changeCity = SERVER_HOST + "/uap/m/user/changeCity";
    /**
     * E006: 设置性别
     */
    public static String chanGegender = SERVER_HOST + "/uap/m/user/changeGender";
    /**
     * E007: 设置个人签名
     */
    public static String changeMoodMessage = SERVER_HOST + "/core/m/user/changeMoodMessage";
    /**
     * E008: 加我为朋友时是否需要验证
     */
    public static String changeNeedFriendConfirmation = SERVER_HOST + "/core/m/user/changeNeedFriendConfirmation";
    /**
     * E009: 是否允许向我推荐通讯录朋友
     */
    public static String changeAllowFindMobileContacts = SERVER_HOST + "/core/m/user/changeAllowFindMobileContacts";
    /**
     * E010: 是否允许通过登录名称找到我
     */
    public static String changeAllowFindMeByLoginId = SERVER_HOST + "/core/m/user/changeAllowFindMeByLoginId";
    /**
     * E011: 是否允许通过手机号码找到我
     */
    public static String changeAllowFindMeByMobileNumber = SERVER_HOST + "/core/m/user/changeAllowFindMeByMobileNumber";
    /**
     * E012: 获取我的二维码
     */
    public static String getMyCode = SERVER_HOST + "/XXX";
    /**
     * E014: 获取最新版本
     */
    public static String getLastestVersion = SERVER_HOST + "/core/m/appversion/windows/latest";
    /**
     * E015: 更新手机号
     */
    public static String updateMoblie = SERVER_HOST + "/chat/s/user/moblie";
    /**
     * E016: 更新邮件
     */
    public static String updateEmail = SERVER_HOST + "/uap/user/bindingEmail";
    /**
     * E018: 修改密码
     */
    public static String resetPassword = SERVER_HOST + "/uap/user/password";
    /**
     * E018_1: 修改密码（通过手机）
     */
    public static String resetPasswordByMobile = SERVER_HOST + "/uap/user/password";

    /**
     * E019: 记录个人的轨迹
     */
    public static String track = SERVER_HOST + "/chat/s/user/track";
    /**
     * E020: 用户反馈使用当中的意见、建议、需求
     */
    public static String feedback = SERVER_HOST + "/core/w/useradvice";

    /**
     * E033: 设置地址
     */
    public static String changeAddress = SERVER_HOST + "/uap/m/user/changeAddress";
    /**
     * E034_Core: 设置地理位置
     */
    public static String updateLocation = SERVER_HOST + "/core/m/user/updateLocation?longitude={longitude}&latitude={latitude}";

    /**
     * MMS037: 客户会员接受经纪人赠送的代金券(移动端)
     */
    public static String acceptTicketFromBroker = SERVER_HOST + "/mms/m/tickets/member/{ticketTransferRecordId}/accept";

    // ----------------------------- 我 end -----------------------------

    // ----------------------------- 其他 sta -----------------------------
    /**
     * F001: 获取省份列表
     */
    public static String getProvides = SERVER_HOST + "/XXX";
    /**
     * F002: 获取城市列表
     */
    public static String getCities = SERVER_HOST + "/XXX";

    /**
     * F004: 发送文本消息
     */
    public static String sendTxtMessage = SERVER_HOST + "/XXX";
    /**
     * F005: 发送语音消息
     */
    public static String sendAudioMessage = SERVER_HOST + "/XXX";
    /**
     * F006: 发送图片消息
     */
    public static String sendPictureMessage = SERVER_HOST + "/XXX";
    /**
     * F007: 发送地理位置消息
     */
    public static String sendLocationMessage = SERVER_HOST + "/XXX";
    /**
     * F008: 发送视频消息
     */
    public static String sendVideoMessage = SERVER_HOST + "/XXX";
    /**
     * F009: 发送名片消息
     */
    public static String sendCardMessage = SERVER_HOST + "/XXX";
    /**
     * F010: 发送单图文消息
     */
    public static String sendSingleImageTxtMessage = SERVER_HOST + "/XXX";
    /**
     * F011: 发送多图文消息
     */
    public static String sendMultiImageTxtMessage = SERVER_HOST + "/XXX";
    /**
     * F012: 发送通知消息
     */
    public static String sendInformMessage = SERVER_HOST + "/XXX";
    /**
     * F013: 发送文件消息
     */
    public static String sendFileMessage = SERVER_HOST + "/XXX";
    /**
     * F014: 发送业务消息
     */
    public static String sendBusinessMessage = SERVER_HOST + "/XXX";
    /**
     * F015: 发送用户禁用消息
     */
    public static String sendDisableUserMessage = SERVER_HOST + "/XXX";
    /**
     * F016: 发送用户删除消息
     */
    public static String sendDeleteUserMessage = SERVER_HOST + "/XXX";
    /**
     * F017: 发送用户注销消息
     */
    public static String sendCancelUserMessage = SERVER_HOST + "/XXX";
    /**
     * F018: 发送用户打开第三方APP
     */
    public static String sendOpen3rdApp = SERVER_HOST + "/XXX";
    /**
     * F019: 发送不明消息
     */
    public static String sendUnknownMessage = SERVER_HOST + "/XXX";
    /**
     * F020: 发送语音聊天请求
     */
    public static String sendAudioChatRequest = SERVER_HOST + "/XXX";
    /**
     * F021: 同意语音聊天请求
     */
    public static String agreeAudioChatRequest = SERVER_HOST + "/XXX";
    /**
     * F022: 拒绝语音聊天请示
     */
    public static String refuseAudioChatRequest = SERVER_HOST + "/XXX";
    /**
     * F023: 语音聊天
     */
    public static String keepAudioChat = SERVER_HOST + "/XXX";
    /**
     * F024: 发送视频聊天请求
     */
    public static String sendVideoChatRequest = SERVER_HOST + "/XXX";
    /**
     * F025: 同意视频聊天请求
     */
    public static String agreeVideoChatRequest = SERVER_HOST + "/XXX";
    /**
     * F026: 拒绝视频聊天请示
     */
    public static String refuseVideoChatRequest = SERVER_HOST + "/XXX";
    /**
     * F027: 视频聊天
     */
    public static String keepVideoChat = SERVER_HOST + "/XXX";
    // ----------------------------- 其他 end -----------------------------

    // ----------------------------- 文件上传 start -----------------------------
    /**
     * Issue #21: 获取上传路径
     */
    public static String getUploadUrl = SERVER_HOST + "/uploadUrl";
    /**
     * Issue #21: 注册存储记录并执行内容标准化处理
     */
    public static String registerFile = SERVER_HOST + "/storageRecord/register";
    /**
     * Issue #21: 获取下载URL
     */
    //public static String getDownloadUrl = SERVER_HOST + "/downloadUrl/{id}";
    // ----------------------------- 其他 end -----------------------------

    public static String getDownloadUrl(String fileStorageId) {
        return SERVER_HOST + "/download/" + fileStorageId;
    }
    public static String getSkyDownloadUrl(String fileStorageId) {
        return SERVER_HOST + "/download/" + fileStorageId;
    }

    public static String getPublicAccountArticleUrl(String publicAccountNo, String articleNo) {
        return SERVER_HOST + "/open/m/service_accounts/" + publicAccountNo + "/subscription/news/" + articleNo;
    }

    // ----------------------------- 二维码 start -----------------------------
    /**
     * 001: 群二维码
     */
    public static String getGroupQRcode = SERVER_HOST + "/core/m/user/groups/{groupId}/createQRcode";
    /**
     * 002: 我的二维码
     */
    public static String getMyQRcode = SERVER_HOST + "/core/m/user/createQRcode";
    /**
     * 003: 校验二维码
     */
    public static String checkQRcode = SERVER_HOST + "/qrcode/{qrcode}";
    /**
     * 004: 下载二维码
     */
    public static String downloadQRcode = SERVER_HOST + "/qrcode/{id}/download";
    // ----------------------------- 二维码 end -----------------------------

    // ----------------------------- 公众号 start -----------------------------
    /**
     * 001: 查找开放平台服务帐号 (根据名称模糊匹配)
     */
    //public static  String searchServiceAccounts = SERVER_HOST + "/open/m/service_accounts/search?q={q}&page={page}&size={size}";
    public static String searchServiceAccounts = SERVER_HOST + "/open/m/getSubscriptionsInTenants/search?q={q}";

    /**
     * 002: 查看开放平台服务帐号详情
     */
    public static String getServiceAccountDetails = SERVER_HOST + "/open/m/service_accounts/{appId}";
    /**
     * 003: 获取开放平台服务帐号包含的公众帐号
     */
    public static String getServiceAccountSubscription = SERVER_HOST + "/open/m/service_accounts/{appId}/subscription";
    /**
     * 004: 获取开放平台服务帐号包含的网站应用
     */
    public static String getServiceAccountWebsite = SERVER_HOST + "/open/m/service_accounts/{appId}/website";
    /**
     * 005: 获取个人使用的公众帐号服务列表
     */
    public static String listMySubscriptions = SERVER_HOST + "/open/m/user/service_accounts/subscriptions?t={t}";
    /**
     * 006: 获取我关注的第三方web应用 个人用户列表
     */
    public static String listMyWebsites = SERVER_HOST + "/open/m/user/service_accounts/websites";
    /**
     * 006-1: 获取我关注的第三方web应用 个人用户列表 2016.5.6切换新接口
     */
    public static String apps = SERVER_HOST + "/open/m/user/service_accounts/apps";
    /**
     * 007: 关注开放平台服务帐号包含的公众帐号
     */
    public static String followSubscription = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/follow";
    /**
     * 008: 取消关注开放平台服务帐号包含的公众帐号
     */
    public static String unfollowSubscription = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/unfollow";
    /**
     * 009: 关注并授权开放平台帐号的第三方web应用 个人用户
     */
    public static String authorizeWebsite = SERVER_HOST + "/open/m/user/service_accounts/{appId}/website/authorize";
    /**
     * 010: 取消关注并取消授权开放平台帐号的第三方web应用 个人用户
     */
    public static String unauthorizeWebsite = SERVER_HOST + "/open/m/user/service_accounts/{appId}/website/unauthorize";
    /**
     * 011: 点击开放平台服务包含的公众帐号的自定义菜单项
     */
    public static String clickSubscriptionMenu = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/menus/{menuCode}";
    /**
     * 012: 是否启用置顶聊天
     */
    public static String enableSubscriptionTopmost = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/enableTopmost";
    /**
     * 013: 是否允许接收消息
     */
    public static String allowReceiveSubscriptionMessages = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/allowReceiveMessages";
    /**
     * 014: 查找开放平台帐号的第三方web应用 个人用户
     */
    public static String searchWebsites = SERVER_HOST + "/open/m/service_accounts/websites/search?";
    /**
     * 015: 查找全部分类（mobile）
     */
    public static String getMAppClassifications = SERVER_HOST + "/open/m/appClassification";
    /**
     * 015: 查找全部分类（mobile）2016.5.7 14:08 接口替换
     */
    public static String appClassificationGroups = SERVER_HOST + "/open/m/appClassificationGroups";


    /**
    * 015: 查找基础分类
    */
    public static String appBaseGroups = SERVER_HOST + "/open/w/builtInAppClassfications/s";

    /**
     * XXX: 通知后台进入了公众号
     */
    public static String publicNoticeEnterServie = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/enter";
    /**
     * XXX: 根据后台公众号的设置上传位置信息。
     */
    public static String publicUploadLocation = SERVER_HOST + "/open/m/user/service_accounts/{appId}/subscription/uploadLocation";

    /**
     * 001: 查找近期的（最早60天前）的待办事项
     */
    public static String getRecentTodoTasks = SERVER_HOST + "/open/m/todoTasks/recent?t={t}&status={status}";

    public static String setTenantNo = SERVER_HOST + "/w/usersessions/switchTenant?tenantNo={tenantNo}";


    /**
    * 002: 查找待办事项历史记录
    */
    public static String getHistoryTodoTasks = SERVER_HOST + "/open/m/todoTasks/history?start={start}&end={end}&status={status}";
    /**
     * 003: 处理待办事项（标记已处理）
     */
    public static String processTodoTasks = SERVER_HOST + "/open/m/todoTasks/{id}/process";
    // ----------------------------- 公众号 end -----------------------------

    // ----------------------------- 码表 sta -----------------------------
    /**
     * 同步码表
     */
    public static String syncMasterCode = SERVER_HOST + "/core/m/dicts/getChanged";

    // ----------------------------- 码表 end -----------------------------

    // ----------------------------- 其他 sta -----------------------------
    /**
     * 保持session
     */
    public static String ping = SERVER_HOST + "/ping";

    /**
     * 用户访问统计
     */
    public static String clientUserAccessInfo = SERVER_HOST + "/core/m/clientUserAccessInfo";

    /**
     * 网站应用
     */
    public static String webApp = SERVER_HOST + "/open/redirect/toThirdpart?appid={appid}&redirect_url={redirect_url}";
    /**
     * 网站应用
     */
    public static String todoTask = webApp + "&toDoTaskId={toDoTaskId}";
    /**
     * 第三方应用
     */
    public static String webAppThirdpart = SERVER_HOST + "/open/redirect/toWebapp?appid={appid}";

    /**
     * 在线下单
     */
    //public static String ecsOrderOnline = H5_SERVER_HOST + "/ecs_orderOnline/index_mp.html#/choseModel.html";

    /**
     * 设为常用网站应用接口 POST
     */
    public static String setCommonWebApp = SERVER_HOST + "/open/m/user/service_accounts/{appId}/website/setCommon";
    /**
     * 取消设为常用网站应用 POST
     */
    public static String unsetCommonWebApp = SERVER_HOST + "/open/m/user/service_accounts/{appId}/website/unsetCommon";
    /**
     * 获取打开第三方应用的authCode
     */
    public static String authCode = SERVER_HOST + "/open/m/user/service_accounts/{appId}/authCode";


    // ----------------------------- 其他 end -----------------------------


    // -----------------------------  API_活动相关(ACTIVITY) 接口 sta -----------------------------

    /**
     * ACTIVITY_016: 获取经纪人所有活动计划列表(mobile)
     */
    public static String getBrokerActivityPlanList = SERVER_HOST + "/crm/m/broker_activityPlans?";
    /**
     * ACTIVITY_017: 获取经纪人活动详细(mobile)
     */
    public static String getBrokerActivityPlanDetail = SERVER_HOST + "/crm/m/broker_activityPlans/{activityPlansId}";
    /**
     * ACTIVITY_018: 经纪人新建活动(mobile)
     */
    public static String createBrokerActivityPlan = SERVER_HOST + "/crm/m/broker_activityPlans";
    /**
     * ACTIVITY_019: 经纪人更新活动(mobile)
     */
    public static String updateBrokerActivityPlan = SERVER_HOST + "/crm/m/broker_activityPlans";
    /**
     * ACTIVITY_020: 经纪人删除活动(mobile)
     */
    public static String deleteBrokerActivityPlan = SERVER_HOST + "/crm/m/broker_activityPlans/{activityPlanId}";
    /**
     * ACTIVITY_022: 经纪人结束活动(mobile)
     */
    public static String closeBrokerActivityPlan = SERVER_HOST + "/crm/m/broker_activityPlans/{activityPlanId}/close";
    /**
     * ACTIVITY_023: 经纪人取消活动(mobile)
     */
    public static String cancleBrokerActivityPlan = SERVER_HOST + "/crm/m/broker_activityPlans/{activityPlanId}/cancle";
    /**
     * ACTIVITY_025: 获取已发布的活动列表(mobile)
     */
    public static String getMPublishedActivityList = SERVER_HOST + "/crm/m/activities?";
    /**
     * ACTIVITY_026: 查看已发布的活动详情(mobile)
     */
    public static String getMPublishedActivityDetail = SERVER_HOST + "/open/m/activities/{activityNo}";
    /**
     * ACTIVITY_027: 活动报名（系统外用户）(mobile)
     */
    public static String signupMOActivity = SERVER_HOST + "/open/m/activities/{activityNo}/signup";
    /**
     * ACTIVITY_028: 活动签到（系统外用户）(mobile)
     */
    public static String signinMOActivity = SERVER_HOST + "/open/m/activities/{activityNo}/signin";
    /**
     * ACTIVITY_029: 获取活动的已报名成员列表(mobile)
     */
    public static String getMActivityMemberList = SERVER_HOST + "/open/m/activities/{activityNo}/participants?s_hasSignup=true&s_hasSignin=true";
    /**
     * ACTIVITY_030: 活动报名（系统内用户）(mobile)
     */
    public static String signupMIActivity = SERVER_HOST + "/open/m/user/activities/{activityNo}/signup";
    /**
     * ACTIVITY_031: 活动签到（系统内用户）(mobile)
     */
    public static String signinMIActivity = SERVER_HOST + "/open/m/user/activities/{activityNo}/signin";
    /**
     * ACTIVITY_031: 活动明细列表（）(mobile)
     */
    public static String activityDetailH5 = SERVER_HOST + "/activity/index.html?activityNo=%s&scope=app";
    // -----------------------------  API_活动相关(ACTIVITY) 接口 end -----------------------------


    // ----------------------------- 组织管理 sta -----------------------------
    /**
     * P001: 获取所有组织机构（团队）列表(MOBILE)
     */
    public static String getMOrganizations = SERVER_HOST + "/core/m/organizations?t={t}"; // 租户-组织机构
    /**
     * P002: 获取所有组织用户列表(MOBILE)
     */
    public static String getMOrganizationUsers = SERVER_HOST + "/core/m/organizations/users?t={t}";
    // ----------------------------- 组织管理 end -----------------------------

    // ----------------------------- ca证书 sta -----------------------------

    /**
     * 申请ca证书
     */
    public static String requestCert = SERVER_HOST + "/client/cert/apply";
    /**
     * 使用ca证书登录
     */
    //    public static  String requsetCertLogin = SERVER_HOST + "/uap/login/ca";
    public static String requsetCertLogin = HTTPS_SERVER_HOST + "/uap/login/ca";

    // ----------------------------- ca证书 end -----------------------------
}
}
