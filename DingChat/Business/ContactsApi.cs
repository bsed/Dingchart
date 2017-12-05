using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Common;

using EventBus;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Services;
using cn.lds.im.sdk.enums;

namespace cn.lds.chatcore.pcw.Business {
class ContactsApi {
    /// <summary>
    /// 服务器IP&端口地址
    /// </summary>
    //static String strServerHost = ProgramSettingHelper.DownLoadHost + ":" + ProgramSettingHelper.DownLoadPort;

    /// <summary>
    /// 获取好友
    /// </summary>
    /// <param Name="time"></param>
    //public static void GetContacts(long GetTime)
    //{
    //    string url = ServiceCode.ServerGetContacts + GetTime;
    //    RestRequestHelper.httpGet(url, EventDataType.GetContacts, null);
    //}


    /// <summary>
    /// 获取好友详情
    /// </summary>
    /// <param Name="time"></param>
    //public static void GetContactsDetaile(long id)
    //{
    //    string url = ProgramSettingHelper.DownLoadHost + ":" + ProgramSettingHelper.DownLoadPort +
    //            ServiceCode.ServerGetContactsDetaile + id;
    //    RestRequestHelper.httpGet(url, EventDataType.GetContactsDetaile, null);
    //}



    // ----------------------------- 公用 sta -----------------------------

    /// <summary>
    /// 注销时，通知服务器注销IM
    /// </summary>
    public static void unregister() {
        //TODO:xxxxxx
        // 构建URL
        String url = ServiceCode.UNREGISTER_CONFIG_SERVER_URL;
        // 准备参数
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        Dictionary<String, Object> body = new Dictionary<String, Object>();

        //body.Add("token",CacheHelper.getImToken());????
        body.Add("clientId", App.AccountsModel.no);
        //body.Add("deviceId",CacheHelper.getDeviceId());???
        //body.Add("deviceType",DeviceType.PHONE.getValue());
        //body.Add("osType",OsType.ANDROID.getValue());

        // 发送请求
        RestRequestHelper.post(url, EventDataType.UNREGISTER_CONFIG_SERVER_URL, parameters,body,null);
    }

    /// <summary>
    /// 连接IM时，注册IM
    /// </summary>
    public static void registerDevice() {
        //TODO:xxxxxx
        // 构建URL
        String url = ServiceCode.CONFIG_SERVER_URL;
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        Dictionary<String, Object> body = new Dictionary<String, Object>();
        body.Add("clientId", App.AccountsModel.no);
        //body.Add("deviceId",CacheHelper.getDeviceId());????
        //body.Add("deviceType",DeviceType.PHONE.getValue());
        //body.Add("osType",OsType.ANDROID.getValue());

        // 发送请求
        RestRequestHelper.post(url, EventDataType.CONFIG_SERVER_URL, parameters,body,null);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public static void sendDispatchQrMessage(SendMessageBean messageBean) {
        // 构建URL
        String url = ServiceCode.SEND_DISPATCH_QR_MESSAGE;
        url = url.Replace("{deviceId}", messageBean.toClientId);
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        //Dictionary<String, Object> body = new Dictionary<String, Object>();
        //body.Add("clientId", App.AccountsModel.no);
        // 发送请求
        RestRequestHelper.post(url, EventDataType.SEND_DISPATCH_QR_MESSAGE, parameters, messageBean, null);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <param Name="url"></param>
    /// <param Name="fileType"></param>
    /// <param Name="owner"></param>
    public static void fileDownload(String fileStorageId, String url, FileType fileType,  String owner) {
        //TODO:xxxxxx
        //RestRequestHelper.download(fileStorageId, url, fileType, owner, null);
    }

    /// <summary>
    /// 下载文件 自定义名称
    /// </summary>
    /// <param Name="fileName"></param>
    /// <param Name="fileStorageId"></param>
    /// <param Name="url"></param>
    /// <param Name="fileType"></param>
    /// <param Name="owner"></param>
    public static void fileDownload(String fileName, String fileStorageId, String url, FileType fileType,  String owner) {
        //TODO:xxxxxx
        //RestRequestHelper.download(fileStorageId, url, fileType, owner, fileName);
    }


    /// <summary>
    /// 文件上传
    /// </summary>
    /// <param Name="uploadUrl"></param>
    /// <param Name="path"></param>
    /// <param Name="extras"></param>
    public static void fileUpload(String uploadUrl, String path, Dictionary<String, Object> extras) {
        //TODO:xxxxxx
        //RestRequestHelper.upload(uploadUrl, path, extras);
    }

    // ----------------------------- 公用 end -----------------------------

    // ----------------------------- 登录 sta -----------------------------

    /// <summary>
    /// A001: 重新登录（目前只使用了重登陆的处理，其他的没用）
    /// </summary>
    /// <param Name="loginRequestModel"></param>
    /// <param Name="background"></param>
    public static void login(LoginRequestModel loginRequestModel) {
        try {
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            Dictionary<String, Object> json = new Dictionary<String, Object>();

            switch (loginRequestModel.loginType) {
            // loginId+密码登录
            case LoginType.id_pass:
                json.Add("loginId", loginRequestModel.loginId);
                json.Add("password", loginRequestModel.password);
                break;
            // mobile+密码登录
            case LoginType.mobile_pass:
                json.Add("mobile", loginRequestModel.mobile);
                json.Add("password", loginRequestModel.password);
                break;
            // mobile+短信验证码登录
            case LoginType.mobile_captcha:
                json.Add("mobile", loginRequestModel.mobile);
                json.Add("captcha", loginRequestModel.captcha);
                break;
            // 一次性token自动登录
            case LoginType.nonceToken:
                if (loginRequestModel.nonceToken == null) {
                    return;
                }
                json.Add("nonceToken", loginRequestModel.nonceToken);
                break;
            }
            json.Add("deviceId", Computer.DeviceId);
            json.Add("deviceType", DeviceType.PC.ToStr());
            json.Add("osType", OsType.WINDOWS.ToStr());
            json.Add("osVersion", Computer.SystemType);
            //json.Add("softwareType", loginRequestModel.softwareType);
            //json.Add("softwareVersion", loginRequestModel.softwareVersion);

            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            //extras.Add("loginType", loginRequestModel.loginType);
            //extras.Add("loginId", loginRequestModel.loginId);
            //extras.Add("background", background);
            RestRequestHelper.post(ServiceCode.login, EventDataType.login,parameters, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi),e);
        }

    }

    /// <summary>
    /// A002: 校验access token
    /// </summary>
    public static void verifyCredentials() {
        //TODO:xxxxxx
    }

    /// <summary>
    /// A003: 退出
    /// </summary>
    public static void logout() {
        // 构建URL
        String url = ServiceCode.logout;
        // 发送请求
        RestRequestHelper.get(url, EventDataType.logout,null,null, false);
    }

    /// <summary>
    /// A004: 获取客户端信息
    /// </summary>
    public static void getClientInformation() {
        //TODO:xxxxxx
    }

    /// <summary>
    /// A005: 手机注册
    /// </summary>
    /// <param Name="mobile"></param>
    /// <param Name="captcha"></param>
    /// <param Name="nickname"></param>
    /// <param Name="password"></param>
    /// <param Name="isWeixinRegister"></param>
    public static void registerByMobile(String mobile, String captcha, String nickname, String password, Boolean isWeixinRegister) {
        try {
            // 构建URL
            String url = ServiceCode.registerByMobile;
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("mobile", mobile);
            json.Add("captcha", captcha);
            json.Add("nickname", nickname);
            json.Add("password", password);
            json.Add("binding", isWeixinRegister);
            // 发送请求
            RestRequestHelper.post(url, EventDataType.registerByMobile,parameters, json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// A006: 用户名注册
    /// </summary>
    public static void register() {
        //TODO:xxxxxx

    }

    /// <summary>
    /// A007: 手机找回密码 TODO:该API已经作废，最后需要删除
    /// </summary>
    /// <param Name="mobile"></param>
    /// <param Name="captcha"></param>
    /// <param Name="password"></param>
    public static void findPasswordByMobile(String mobile, String captcha, String password) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// A008: 邮件找回密码
    /// </summary>
    public static void findPasswordByMail() {


    }

    /// <summary>
    /// A009: 微信号登录绑定手机号
    /// </summary>
    public static void boundWeixinAndMobile() {


    }

    /// <summary>
    /// A010: 发送验证码
    /// </summary>
    /// <param Name="mobile"></param>
    public static void sendCAPTCHA(String mobile) {
        // 构建URL
        String url = ServiceCode.sendCAPTCHA;
        url = url.Replace("{mobile}", mobile);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.sendCAPTCHA, null,null, false);
    }

    /// <summary>
    /// 使用微信登录
    /// </summary>
    /// <param Name="loginRequestModel"></param>
    public static void weixinLogin(LoginRequestModel loginRequestModel) {
        // 构建URL
        try {

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("code", loginRequestModel.authorizationCode);
            json.Add("deviceId", loginRequestModel.deviceId);
            //json.Add("deviceType", loginRequestModel.deviceType.getValue());
            //json.Add("osType", loginRequestModel.osType.getValue());
            json.Add("osVersion", loginRequestModel.osVersion);
            json.Add("softwareType", loginRequestModel.softwareType);
            json.Add("softwareVersion", loginRequestModel.softwareVersion);
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("loginType", "mobile_captcha");
            RestRequestHelper.post(ServiceCode.weixinLogin, EventDataType.weixinLogin,null,json,extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    // ----------------------------- 登录 end -----------------------------

    // ----------------------------- 通讯录 sta -----------------------------

    /// <summary>
    /// C001: 获取好友列表
    /// </summary>
    /// <param Name="Timestamp"></param>
    public static void friends(long timestamp) {
        // 构建URL
        String url = ServiceCode.friends;
        url = url.Replace("{t}",timestamp.ToStr());
        // 发送请求

        RestRequestHelper.get(url, EventDataType.friends,null,null, false);

    }

    /// <summary>
    /// C002: 获取好友详细
    /// </summary>
    /// <param Name="id"></param>
    public static void getFriend(String id) {
        // 构建URL
        String url = ServiceCode.getFriend;
        url = url.Replace("{id}", id);
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("id", id);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.getFriend,null, extras, false);

    }

    /// <summary>
    /// C002.2: 获陌生人详细
    /// </summary>
    /// <param Name="id"></param>
    public static void getStranger(String id) {
        // 构建URL
        String url = ServiceCode.getStranger;
        url = url.Replace("{id}", id);
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("id", id);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.getStranger,null, extras, false);
    }

    /// <summary>
    /// C003: 设置好友标签
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="tags"></param>
    public static void changeTags( String id, List<String> tags) {
        try {
            // 构建URL
            String url = ServiceCode.changeTags;
            url = url.Replace("{id}", id);
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id);
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("tags", tags);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.changeTags,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    /// C003: 设置好友备注名
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="alias"></param>
    public static void changeAlias(int id, String alias) {
        try {
            // 构建URL
            String url = ServiceCode.changeAlias;
            url = url.Replace("{id}", id.ToStr());
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id.ToStr());
            extras.Add("alias", alias);

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("alias", alias);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.changeAlias,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    /// C004: 标记为星标朋友
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="favorite"></param>
    public static void markFavorite(String id, Boolean favorite) {

        try {
            // 构建URL
            String url = ServiceCode.markFavorite;
            url = url.Replace("{id}", id);
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id);

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("favorite", favorite);

            // 发送请求
            RestRequestHelper.put(url, EventDataType.markFavorite,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C005: 查找好友
    /// </summary>
    /// <param Name="q"></param>
    /// <param Name="page"></param>
    /// <param Name="size"></param>
    public static void findNewFrind(String q, String page, String size) {
        // 构建URL
        String url = ServiceCode.findNewFrind;
        url = url.Replace("{q}", q);
        url = url.Replace("{page}", page);
        url = url.Replace("{size}", size);
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("q", q);
        extras.Add("page", page);
        extras.Add("size", size);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.findNewFrind, null,extras, false);
    }

    /// <summary>
    /// C005_1: 查找通讯录好友
    /// </summary>
    /// <param Name="mobiles"></param>
    public static void searchContacts(List<String> mobiles) {
        try {
            // 构建URL
            String url = ServiceCode.searchContacts;
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("mobiles", mobiles);
            // 发送请求
            RestRequestHelper.post(url, EventDataType.searchContacts,null,json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C006: 添加新朋友
    /// </summary>
    /// <param Name="friendId"></param>
    /// <param Name="channel"></param>
    public static void addFriend(int friendId, String channel) {
        try {
            // 构建URL
            String url = ServiceCode.addFriend;
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("friendId", friendId);
            extras.Add("showLoading", true);

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("friendId", friendId);
            if (channel!=null) {
                json.Add("channel", channel);
            }
            // 发送请求
            RestRequestHelper.post(url, EventDataType.addFriend,null,json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    /// C007: 删除朋友
    /// </summary>
    /// <param Name="id"></param>
    public static void deleteFriend(int id,String no) {
        // 构建URL
        String url = ServiceCode.deleteFriend;
        url = url.Replace("{id}", id.ToStr());
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("id", id);
        extras.Add("no", no);
        // 发送请求
        RestRequestHelper.delete(url, EventDataType.deleteFriend,null,null,extras);
    }

    /// <summary>
    /// C008.1 发起朋友身份验证请求
    /// </summary>
    /// <param Name="friendId"></param>
    /// <param Name="message"></param>
    /// <param Name="channel"></param>
    public static void requestFriend(int friendId, String message, String channel) {
        try {
            // 构建URL
            String url = ServiceCode.requestFriend;
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("friendId", friendId);

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("friendId", friendId);
            json.Add("message", message);
            json.Add("channel", channel);

            RestRequestHelper.post(url, EventDataType.requestFriend,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C008: 接受新朋友请求
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="clientUserId"></param>
    /// <param Name="message"></param>
    public static void acceptFriend(int id, String clientUserId, String message) {
        try {
            // 构建URL
            String url = ServiceCode.acceptFriend;
            url = url.Replace("{id}", id.ToStr());
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id);
            extras.Add("clientUserId", clientUserId);

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            if (message!=null) {
                json.Add("message", message);
            }

            // 发送请求
            RestRequestHelper.put(url, EventDataType.acceptFriend,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C009: 获取单聊会话列表
    /// </summary>
    public static void clientUserChats() {
        // 构建URL
        String url = ServiceCode.clientUserChats;
        // TODO 需要替换成实际的时间戳
        url = url.Replace("{t}", "1448337545000");
        // 发送请求
        RestRequestHelper.get(url, EventDataType.clientUserChats,null,null, false);

    }

    /// <summary>
    /// C010: 设置单聊置顶
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="topmost"></param>
    public static void setTopmost(String id, Boolean topmost) {
        try {
            // 构建URL
            String url = ServiceCode.setTopmost;
            url = url.Replace("{id}", id.ToStr());
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id);
            extras.Add("topmost", topmost);
            extras.Add("showLoading", true);
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("topmost", topmost);

            // 发送请求
            RestRequestHelper.put(url, EventDataType.setTopmost,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C011: 设置单聊背景图片
    /// </summary>
    public static void updateBackground() {


    }

    /// <summary>
    /// C012: 设置单聊免打扰
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="enableNoDisturb"></param>
    public static void enableNoDisturbFriend(String id, Boolean quiet) {
        try {
            // 构建URL
            String url = ServiceCode.enableNoDisturbFriend;
            url = url.Replace("{id}", id.ToStr());
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id);
            extras.Add("Quiet", quiet);
            extras.Add("showLoading", true);
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("Quiet", quiet);

            // 发送请求
            RestRequestHelper.put(url, EventDataType.enableNoDisturbFriend, null,json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C013: 获取群会话列表
    /// </summary>
    public static void groupChats() {
        // 构建URL
        String url = ServiceCode.groupChats;
        // TODO 需要替换成实际的时间戳
        url = url.Replace("{t}", "1448337545000");
        // 准备参数
        // 发送请求
        RestRequestHelper.get(url, EventDataType.groupChats,null,null, false);
    }

    /// <summary>
    /// C013: 获取通讯录中群列表
    /// </summary>
    /// <param Name="Timestamp"></param>
    public static void groups(long timestamp) {
        // 构建URL
        String url = ServiceCode.groups;
        url =url.Replace("{t}",timestamp.ToStr());
        // 发送请求
        RestRequestHelper.get(url, EventDataType.groups,null,null, false);
    }

    /// <summary>
    /// C014: 获取群详细信息
    /// </summary>
    /// <param Name="no"></param>
    /// <param Name="extras"></param>
    public static void getGroup(String no, Dictionary<String, Object> extras) {
        // 构建URL
        String url = ServiceCode.getGroup;
        url = url.Replace("{group}",no);
        if (extras == null) {
            extras = new Dictionary<String, Object>();
        }
        extras.Add("mucNo", no);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.getGroup,null, extras, false);

    }

    /// <summary>
    /// C014.2: 获取群成员详细信息
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="clientuserid"></param>
    public static void getGroupMember(String mucid, String clientuserid) {
        // 构建URL
        String url = ServiceCode.getGroupMember;
        url = url.Replace("{groupid}",mucid);
        url = url.Replace("{clientuserid}",clientuserid);
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("clientuserid", clientuserid);

        // 发送请求
        RestRequestHelper.get(url, EventDataType.getGroupMember, null,extras, false);

    }

    /// <summary>
    /// C015: 创建群聊
    /// </summary>
    /// <param Name="name"></param>
    /// <param Name="memberid"></param>
    public static void createGroup(String name, List<String> memberid) {
        try {
            // 构建URL
            String url = ServiceCode.createGroup;
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("name", name);
            json.Add("members", memberid);
            // 发送请求
            RestRequestHelper.post(url, EventDataType.createGroup, null,json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C016: 群保存到通讯录,mucid、savedAsContact做为额外参数传递
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="savedAsContact"></param>
    public static void addGroupToAddressList(String mucid, Boolean savedAsContact) {
        try {
            // 构建URL
            String url = ServiceCode.addGroupToAddressList;
            url = url.Replace("{mucid}",mucid);

            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("mucid", mucid);
            extras.Add("savedAsContact", savedAsContact);
            extras.Add("showLoading", true);
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("savedAsContact", savedAsContact);

            // 发送请求
            RestRequestHelper.put(url, EventDataType.addGroupToAddressList, null,json, extras);
        } catch (Exception ex) {
            Log.Error(typeof(ContactsApi), ex);
        }
    }

    /// <summary>
    /// C017: 更新群聊名称
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="name"></param>
    public static void updateGroupName(String mucid, String name) {
        try {
            // 构建URL
            String url = ServiceCode.updateGroupName;
            url = url.Replace("{id}", mucid);

            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("mucid", mucid);
            extras.Add("name", name);

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("name", name);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.updateGroupName, null,json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C018: 删除群聊
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="mucNo"></param>
    public static void deleteGroup(String mucid, String mucNo) {
        // 构建URL
        String url = ServiceCode.deleteGroup;
        url = url.Replace("{mucid}",mucid);

        // 额外参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("mucid", mucid);
        extras.Add("mucNo", mucNo);
        extras.Add("showLoading", true);
        // 发送请求
        RestRequestHelper.delete(url, EventDataType.deleteGroup,null,null, extras);



        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = mucNo;
        Businessdata.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_DELETE_GROUP;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

    /// <summary>
    ///  C019: 增加群聊成员
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="members"></param>
    /// <param Name="channel"></param>
    public static void addGroupMember(String mucid, List<String> members, String channel) {
        // 构建URL
        String url = ServiceCode.addGroupMember;
        url = url.Replace("{mucid}",mucid);

        // 额外参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("mucid", mucid);
        extras.Add("members", listToString(members));
        extras.Add("channel", channel);
        // 构建JSON
        Dictionary<String, Object> data = new Dictionary<String, Object>();
        data.Add("members", members);
        data.Add("channel", channel);
        // 发送请求
        RestRequestHelper.post(url, EventDataType.addGroupMember,null, data, extras);
    }

    /// <summary>
    /// C020: 移除群聊成员
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="members"></param>
    public static void deleteGroupMember(String mucid, List<String> members) {
        // 构建URL
        String url = ServiceCode.deleteGroupMember;
        url = url.Replace("{mucid}",mucid);

        // 额外参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("mucid", mucid);
        extras.Add("members", listToString(members));

        Dictionary<String, Object> data = new Dictionary<String, Object>();
        data.Add("members", members);

        // 发送请求
        RestRequestHelper.delete(url, EventDataType.deleteGroupMember,null, data, extras);

    }

    /// <summary>
    /// C021: 设置群消息免打扰
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="enableNoDisturb"></param>
    public static void enableGroupNoDisturb(String mucid, Boolean quiet) {
        // 构建URL
        String url = ServiceCode.enableGroupNoDisturb;
        url = url.Replace("{mucid}",mucid);

        // 额外参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("mucid", mucid);
        extras.Add("Quiet", quiet);
        extras.Add("showLoading", true);
        // 构建JSONs
        Dictionary<String, Object> body = new Dictionary<String, Object>();
        body.Add("Quiet", quiet);

        // 发送请求
        RestRequestHelper.put(url, EventDataType.enableGroupNoDisturb,null, body, extras);

    }

    /// <summary>
    ///  C022: 设置群置顶聊天
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="topmost"></param>
    public static void setGroupTopmost(String mucid, Boolean topmost) {
        // 构建URL
        String url = ServiceCode.setGroupTopmost;
        url = url.Replace("{mucid}",mucid);

        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("mucid", mucid);
        extras.Add("topmost", topmost);
        extras.Add("showLoading", true);
        // 构建JSON
        Dictionary<String, Object> body = new Dictionary<String, Object>();
        body.Add("topmost", topmost);

        // 发送请求
        RestRequestHelper.put(url, EventDataType.setGroupTopmost,null, body, extras);

    }

    /// <summary>
    /// C023: 设置我在群中的昵称
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="nickName"></param>
    public static void updateNicknameInGroup(String mucid, String nickName) {
        try {
            // 构建URL
            String url = ServiceCode.updateNicknameInGroup;
            url = url.Replace("{mucid}",mucid);

            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("mucid", mucid);
            extras.Add("nickName", nickName);

            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("nickname", nickName);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.updateNicknameInGroup,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    /// C024: 设置群聊天背景
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="url"></param>
    public static void updateBackgroundInGroup(String id, String url) {

    }

    /// <summary>
    /// C025: 更改群的状态
    /// </summary>
    /// <param Name="mucid"></param>
    public static void updateGroupChatStatus(String mucid) {
        // 构建URL
        String url = ServiceCode.updateGroupChatStatus;
        url = url.Replace("{mucid}",mucid);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("mucid", mucid);

        // 发送请求
        RestRequestHelper.put(url, EventDataType.updateGroupChatStatus, null, null, extras);

    }

    /// <summary>
    /// C026: 获取标签列表和标签详细
    /// </summary>
    public static void tags() {
        // 构建URL
        String url = ServiceCode.tags;
        // TODO:asdfasdfsadf??????????????????
        long longTimestamp = 0;
        url = url.Replace("{t}", longTimestamp.ToStr());
        // 发送请求
        RestRequestHelper.get(url, EventDataType.tags,null,null, false);
    }

    /// <summary>
    /// C027: 新建标签
    /// </summary>
    /// <param Name="tag"></param>
    /// <param Name="contacts"></param>
    public static void createTag(String tag, List<int> contacts) {
        try {
            // 构建URL
            String url = ServiceCode.createTag;
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("tag", tag);
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("tag", tag);
            json.Add("contacts", contacts);
            // 发送请求
            RestRequestHelper.post(url, EventDataType.createTag,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C028: 修改标签
    /// </summary>
    /// <param Name="id"></param>
    /// <param Name="tag"></param>
    /// <param Name="contacts"></param>
    public static void updateTag(int id, String tag, List<int> contacts) {
        try {
            // 构建URL
            String url = ServiceCode.updateTag;
            url = url.Replace("{id}", id.ToStr());
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("id", id);
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("tag", tag);
            json.Add("contacts", contacts);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.updateTag, null,json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    ///  C029: 删除标签
    /// </summary>
    /// <param Name="id"></param>
    public static void deleteTag(int id) {
        // 构建URL
        String url = ServiceCode.deleteTag;
        url = url.Replace("{id}", id.ToStr());
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("id", id);
        // 发送请求
        RestRequestHelper.delete(url, EventDataType.deleteTag,null,null, extras);
    }

    ///// <summary>
    ///// C032: 获取组织
    ///// </summary>
    //public static void getOrganization() {

    //}

    ///// <summary>
    ///// C033: 获取人员信息（画像）
    ///// </summary>
    //public static void getFriendPicture() {

    //}

    ///// <summary>
    ///// C034: 通过手机号获取联系人
    ///// </summary>
    //public static void getFriendByMobile() {

    //}

    /// <summary>
    /// C036: 扫描群成员的二维码加入群聊组
    /// </summary>
    /// <param Name="groupNo"></param>
    /// <param Name="userNo"></param>
    public static void joinGroupByNoAndMember(String groupNo, String userNo) {
        try {
            // 构建URL
            String url = ServiceCode.joinGroupByNoAndMember;
            url = url.Replace("{MucNo}", groupNo);
            url = url.Replace("{userNo}", userNo);
            // 构建JSON
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("MucNo",groupNo);
            extras.Add("userNo",userNo);
            // 发送请求
            RestRequestHelper.post(url, EventDataType.joinGroupByNoAndMember,null, null, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// C037: 查找附近的人
    /// </summary>
    /// <param Name="latitude"></param>
    /// <param Name="longitude"></param>
    public static void searchNearbyUsers(String latitude, String longitude) {
        // 构建URL
        String url = ServiceCode.searchNearbyUsers;
        url = url.Replace("{latitude}", latitude);
        url = url.Replace("{longitude}", longitude);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.searchNearbyUsers,null,null, false);

    }
    // ----------------------------- 通讯录 end -----------------------------

    // ----------------------------- 我 sta -----------------------------

    /// <summary>
    /// E001: 获取我的个人详细信息
    /// </summary>
    public static void getMyDetail() {
        // 构建URL
        String url = ServiceCode.getMyDetail;
        // 发送请求
        RestRequestHelper.get(url, EventDataType.getMyDetail,null,null, false);
    }

    /// <summary>
    /// E002: 修改我的头像
    /// </summary>
    /// <param Name="avatarStorageId"></param>
    /// <param Name="fileName"></param>
    public static void changeAvatar(String avatarStorageId, String fileName) {
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("AvatarStorageRecordId", avatarStorageId);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("avatarStorageId", avatarStorageId);
        extras.Add("fileName", fileName);

        RestRequestHelper.put(ServiceCode.changeAvatar, EventDataType.changeAvatar,null, json,extras);
    }

    /// <summary>
    /// E003: 设置免打扰
    /// </summary>
    /// <param Name="enableNoDisturb"></param>
    /// <param Name="startTimeOfNoDisturb"></param>
    /// <param Name="endTimeOfNoDisturb"></param>
    public static void enableNoDisturbMe(Boolean enableNoDisturb, String startTimeOfNoDisturb,
                                         String endTimeOfNoDisturb) {
        // 构建URL
        String url = ServiceCode.enableNoDisturbMe;
        // 准备参数
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("enableNoDisturb", enableNoDisturb);
        json.Add("startTimeOfNoDisturb", startTimeOfNoDisturb);
        json.Add("endTimeOfNoDisturb", endTimeOfNoDisturb);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("enableNoDisturb", enableNoDisturb);
        extras.Add("startTimeOfNoDisturb", startTimeOfNoDisturb);
        extras.Add("endTimeOfNoDisturb", endTimeOfNoDisturb);

        // 发送请求
        RestRequestHelper.put(url, EventDataType.enableNoDisturbMe,null, json, extras);

    }

    /// <summary>
    /// E004: 设置昵称
    /// </summary>
    /// <param Name="nickname"></param>
    public static void changeNickname(String nickname) {
        try {
            // 构建URL
            String url = ServiceCode.changeNickname;
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("nickname", nickname);
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("nickname", nickname);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.changeNickname,null,json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// E005: 设置地区
    /// </summary>
    /// <param Name="country"></param>
    /// <param Name="province"></param>
    /// <param Name="city"></param>
    public static void changeCity(String country, String province, String city) {
        // 构建URL
        String url = ServiceCode.changeCity;
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("country", country);
        json.Add("province", province);
        json.Add("city", city);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("country", country);
        extras.Add("province", province);
        extras.Add("city", city);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.changeCity, null,json, extras);

    }

    /// <summary>
    /// E006: 设置性别
    /// </summary>
    /// <param Name="gender"></param>
    public static void chanGegender(String gender) {
        // 构建URL
        String url = ServiceCode.chanGegender;
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("gender", gender);
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("gender", gender);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.chanGegender,null, json, extras);

    }

    /// <summary>
    /// E007: 设置个人签名
    /// </summary>
    /// <param Name="moodMessage"></param>
    public static void changeMoodMessage(String moodMessage) {
        try {
            String url = ServiceCode.changeMoodMessage;
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("moodMessage", moodMessage);
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("desc", moodMessage);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.changeMoodMessage,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    /// E008: 加我为朋友时是否需要验证
    /// </summary>
    /// <param Name="needFriendConfirmation"></param>
    public static void changeNeedFriendConfirmation(Boolean needFriendConfirmation) {
        // 构建URL
        String url = ServiceCode.changeNeedFriendConfirmation;
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("changeNeedFriendConfirmation", needFriendConfirmation);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("changeNeedFriendConfirmation", needFriendConfirmation);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.changeNeedFriendConfirmation, null,json, extras);

    }

    /// <summary>
    /// E009: 是否允许向我推荐通讯录朋友
    /// </summary>
    /// <param Name="allowFindMobileContacts"></param>
    public static void changeAllowFindMobileContacts(Boolean allowFindMobileContacts) {
        // 构建URL
        String url = ServiceCode.changeAllowFindMobileContacts;
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("allowFindMobileContacts", allowFindMobileContacts);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("allowFindMobileContacts", allowFindMobileContacts);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.changeAllowFindMobileContacts,null, json, extras);

    }

    /// <summary>
    ///  E010: 是否允许通过登录名称找到我
    /// </summary>
    /// <param Name="allowFindMeByLoginId"></param>
    public static void changeAllowFindMeByLoginId( Boolean allowFindMeByLoginId) {
        // 构建URL
        String url = ServiceCode.changeAllowFindMeByLoginId;
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("allowFindMeByLoginId", allowFindMeByLoginId);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("allowFindMeByLoginId", allowFindMeByLoginId);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.changeAllowFindMeByLoginId,null, json, extras);
    }

    /// <summary>
    /// E011: 是否允许通过手机号码找到我
    /// </summary>
    /// <param Name="allowFindMeByMobileNumber"></param>
    public static void changeAllowFindMeByMobileNumber( Boolean allowFindMeByMobileNumber) {
        // 构建URL
        String url = ServiceCode.changeAllowFindMeByMobileNumber;
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("allowFindMeByMobileNumber", allowFindMeByMobileNumber);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("allowFindMeByMobileNumber", allowFindMeByMobileNumber);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.changeAllowFindMeByMobileNumber,null, json, extras);
    }

    /// <summary>
    /// E012: 获取我的二维码
    /// </summary>
    public static void getMyCode() {

    }

    /// <summary>
    /// E014: 获取最新版本
    /// </summary>
    public static void getLastestVersion() {
        // 构建URL
        String url = ServiceCode.getLastestVersion;
        // 发送请求
        RestRequestHelper.get(url, EventDataType.getLastestVersion,null,null, false);

    }

    /// <summary>
    /// E015: 更新手机号
    /// </summary>
    public static void updateMoblie() {

    }

    /// <summary>
    /// E016: 更新邮件（X009）
    /// </summary>
    /// <param Name="email"></param>
    public static void updateEmail( String email) {
        try {
            // 构建URL
            String url = ServiceCode.updateEmail;
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("email", email);
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("email", email);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.updateEmail,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }

    }

    /// <summary>
    /// E018: 修改密码
    /// </summary>
    /// <param Name="currentPassword"></param>
    /// <param Name="password"></param>
    /// <param Name="confirmPassword"></param>
    public static void resetPassword(String currentPassword, String password, String confirmPassword) {
        try {
            // 构建URL
            String url = ServiceCode.resetPassword;
            // 准备参数
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("currentPassword", currentPassword);
            json.Add("password", password);
            json.Add("confirmPassword", confirmPassword);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.resetPassword,null, json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// E018_1: 修改密码（通过手机）
    /// </summary>
    /// <param Name="mobile"></param>
    /// <param Name="password"></param>
    /// <param Name="captcha"></param>
    public static void resetPasswordByMobile(String mobile, String password, String captcha) {
        try {
            // 构建URL
            String url = ServiceCode.resetPasswordByMobile;
            // 准备参数
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("mobile", mobile);
            json.Add("password", password);
            json.Add("captcha", captcha);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.resetPasswordByMobile,null, json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /**
     * E019: 记录个人的轨迹
     */
    public static void track() {

    }

    /**
     * E020: 用户反馈使用当中的意见、建议、需求
     */
    public static void feedback(int userId, String advice, String contact, List<int> files) {

        try {
            // 构建URL
            String url = ServiceCode.feedback;
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("userId", userId);
            json.Add("advice", advice);
            json.Add("contact", contact);
            json.Add("files", files);


            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("showLoading", true);
            // 发送请求
            RestRequestHelper.post(url, EventDataType.feedback, null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// E033: 设置地址
    /// </summary>
    /// <param Name="address"></param>
    public static void changeAddress(String address) {
        try {
            // 构建URL
            String url = ServiceCode.changeAddress;
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("address", address);
            // 准备参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("address", address);
            // 发送请求
            RestRequestHelper.put(url, EventDataType.changeAddress,null, json, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// E034: 设置地理位置
    /// </summary>
    /// <param Name="latitude"></param>
    /// <param Name="longitude"></param>
    public static void updateLocation(String latitude, String longitude) {
        String url = ServiceCode.updateLocation;
        url = url.Replace("{latitude}", latitude);
        url = url.Replace("{longitude}", longitude);
        RestRequestHelper.put(url, EventDataType.updateLocation,null,null,null);
    }
    // ----------------------------- 我 end -----------------------------

    // ----------------------------- 其他 sta -----------------------------

    /**
     * F001: 获取省份列表
     */
    public static void getProvides() {


    }

    /**
     * F002: 获取城市列表
     */
    public static void getCities() {


    }

    /**
     * F004: 发送文本消息
     */
    public static void sendTxtMessage() {


    }

    /**
     * F005: 发送语音消息
     */
    public static void sendAudioMessage() {


    }

    /**
     * F006: 发送图片消息
     */
    public static void sendPictureMessage() {


    }

    /**
     * F007: 发送地理位置消息
     */
    public static void sendLocationMessage() {


    }

    /**
     * F008: 发送视频消息
     */
    public static void sendVideoMessage() {


    }

    /**
     * F009: 发送名片消息
     */
    public static void sendCardMessage() {


    }

    /**
     * F010: 发送单图文消息
     */
    public static void sendSingleImageTxtMessage() {


    }

    /**
     * F011: 发送多图文消息
     */
    public static void sendMultiImageTxtMessage() {


    }

    /**
     * F012: 发送通知消息
     */
    public static void sendInformMessage() {


    }

    /**
     * F013: 发送文件消息
     */
    public static void sendFileMessage() {


    }

    /**
     * F014: 发送业务消息
     */
    public static void sendBusinessMessage() {


    }

    /**
     * F015: 发送用户禁用消息
     */
    public static void sendDisableUserMessage() {


    }

    /**
     * F016: 发送用户删除消息
     */
    public static void sendDeleteUserMessage() {


    }

    /**
     * F017: 发送用户注销消息
     */
    public static void sendCancelUserMessage() {

    }

    /**
     * F018: 发送用户打开第三方APP
     */
    public static void sendOpen3rdApp() {


    }

    /**
     * F019: 发送不明消息
     */
    public static void sendUnknownMessage() {


    }

    /**
     * F020: 发送语音聊天请求
     */
    public static void sendAudioChatRequest() {

    }

    /**
     * F021: 同意语音聊天请求
     */
    public static void agreeAudioChatRequest() {

    }

    /**
     * F022: 拒绝语音聊天请示
     */
    public static void refuseAudioChatRequest() {

    }

    /**
     * F023: 语音聊天
     */
    public static void keepAudioChat() {


    }

    /**
     * F024: 发送视频聊天请求
     */
    public static void sendVideoChatRequest() {

    }

    /**
     * F025: 同意视频聊天请求
     */
    public static void agreeVideoChatRequest() {

    }

    /**
     * F026: 拒绝视频聊天请示
     */
    public static void refuseVideoChatRequest() {

    }

    /**
     * F027: 视频聊天
     */
    public static void keepVideoChat() {

    }
    // ----------------------------- 其他 end -----------------------------


    public static String listToString(List<String> tmp) {
        String str = "";
        for (int i = 0; i < tmp.Count; i++) {
            if (i == 0) {
                str = tmp[i];
            } else {
                str += "|" + tmp[i];
            }
        }
        return str;
    }

    /**
     * 获取上传路径
     *
     * @param extras
     */
    public static void getUploadUrl(Dictionary<String, Object> extras) {
        RestRequestHelper.get(ServiceCode.getUploadUrl, EventDataType.getUploadUrl,null, extras, false);
    }

    /**
     * 获取上传路径
     *
     * @param extras
     */
    public static void registerFile(Dictionary<String, Object> json, Dictionary<String, Object> extras) {
        RestRequestHelper.post(ServiceCode.registerFile, EventDataType.registerFile,null, json, extras);
    }

    // ----------------------------- 二维码 start -----------------------------

    /**
     * 001: 群二维码
     */
    public static void getGroupQRcode(String groupId) {
        // 构建URL
        String url = ServiceCode.getGroupQRcode;
        url = url.Replace("{groupId}", groupId);
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("groupId", groupId);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.getGroupQRcode, null, null, extras);
    }

    /// <summary>
    /// 002: 我的二维码
    /// </summary>
    public static void getMyQRcode() {
        // 构建URL
        String url = ServiceCode.getMyQRcode;
        // 发送请求
        RestRequestHelper.put(url, EventDataType.getMyQRcode,null,null,null);
    }

    /// <summary>
    ///  003: 校验二维码
    /// </summary>
    /// <param Name="qrcode"></param>
    public static void checkQRcode( String qrcode) {
        // 构建URL
        String url = ServiceCode.checkQRcode;
        url = url.Replace("{qrcode}", qrcode);
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("qrcode", qrcode);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.checkQRcode,null, extras, false);
    }

    /// <summary>
    /// 004: 下载二维码
    /// </summary>
    /// <param Name="qrcodeId"></param>
    /// <param Name="no"></param>
    public static void downloadQRcode(String qrcodeId, String no) {
        // 构建URL
        String url = ServiceCode.downloadQRcode;
        url = url.Replace("{id}", qrcodeId);
        //url += "?fileSaveName=" + UUID.randomUUID() + ".jpg";
        url += "?fileSaveName=" + no + "_" + qrcodeId + ".jpg";

        // TODO:asdfasdf???
        // 发送请求
        //RestRequestHelper.download(qrcodeId, url, FileType.QRCODE, no, null);
    }
    // ----------------------------- 二维码 end -----------------------------

    // ----------------------------- 公众号 start -----------------------------

    /// <summary>
    /// 001: 查找开放平台服务帐号 (根据名称模糊匹配)
    /// </summary>
    /// <param Name="q"></param>
    /// <param Name="page"></param>
    /// <param Name="size"></param>
    public static void searchServiceAccounts(String q, int page, int size) {
        // 构建URL
        String url = ServiceCode.searchServiceAccounts;
        url = url.Replace("{q}", q);
        url = url.Replace("{page}", page.ToStr());
        url = url.Replace("{size}", size.ToStr());
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", App.CurrentTenantNo);
        RestRequestHelper.get(url, EventDataType.searchServiceAccounts,null, extras, false);
    }

    /// <summary>
    /// 002: 查看开放平台服务帐号详情
    /// </summary>
    /// <param Name="appId"></param>
    public static void getServiceAccountDetails(String appId,string tenantNo) {
        // 构建URL
        String url = ServiceCode.getServiceAccountDetails;
        url = url.Replace("{appId}", appId);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);

        RestRequestHelper.get(url, EventDataType.getServiceAccountDetails,null, extras, false);
    }

    /// <summary>
    /// 003: 获取开放平台自定义菜单
    /// </summary>
    /// <param Name="appId"></param>
    public static void getServiceAccountSubscription(String appId,string tenantNo) {
        // 构建URL
        String url = ServiceCode.getServiceAccountSubscription;
        url = url.Replace("{appId}", appId);
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        extras.Add("tenantNo", tenantNo);
        RestRequestHelper.get(url, EventDataType.getServiceAccountSubscription,null, extras, false);
    }

    /// <summary>
    /// 004: 获取开放平台服务帐号包含的网站应用
    /// </summary>
    /// <param Name="appId"></param>
    public static void getServiceAccountWebsite(String appId) {
        // 构建URL
        String url = ServiceCode.getServiceAccountWebsite;
        url = url.Replace("{appId}", appId);
        RestRequestHelper.get(url, EventDataType.getServiceAccountWebsite,null,null, false);
    }

    /// <summary>
    /// 005: 获取个人使用的公众帐号服务列表
    /// </summary>
    /// <param Name="Timestamp"></param>
    /// <param Name="extras"></param>
    public static void listMySubscriptions(long timestamp, Dictionary<String, Object> extras) {
        // 构建URL
        String url = ServiceCode.listMySubscriptions;
        url = url.Replace("{t}",timestamp.ToStr());
        if (extras == null) {
            extras = new Dictionary<String, Object>();
        }

        RestRequestHelper.get(url, EventDataType.listMySubscriptions,null, extras, false);
    }

    /// <summary>
    /// 006: 获取我关注的第三方web应用 个人用户列表
    /// </summary>
    public static void listMyWebsites(string  tenantNo) {
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);
        // 构建URL
        String url = ServiceCode.listMyWebsites;
        RestRequestHelper.get(url, EventDataType.listMyWebsites,null, extras, false);
    }

    /// <summary>
    /// 006-1: 获取我关注的第三方web应用 个人用户列表 2016.5.6切换新接口
    /// </summary>
    public static void apps(string tenantNo) {
        // 构建URL
        String url = ServiceCode.apps;

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);

        RestRequestHelper.get(url, EventDataType.apps,null, extras, false);
    }

    /// <summary>
    /// 007: 关注开放平台服务帐号包含的公众帐号
    /// </summary>
    /// <param Name="appId"></param>
    public static void followSubscription(String appId,string tenantNo) {
        // 构建URL
        String url = ServiceCode.followSubscription;
        url = url.Replace("{appId}", appId);
        // 构建返回参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        extras.Add("tenantNo", tenantNo);
        RestRequestHelper.post(url, EventDataType.followSubscription, null, null, extras);
    }

    /// <summary>
    /// 008: 取消关注开放平台服务帐号包含的公众帐号
    /// </summary>
    /// <param Name="appId"></param>
    public static void unfollowSubscription(String appId,string tenantNo) {
        // 构建URL
        String url = ServiceCode.unfollowSubscription;
        url = url.Replace("{appId}", appId);
        // 构建返回参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        extras.Add("tenantNo", tenantNo);
        RestRequestHelper.post(url, EventDataType.unfollowSubscription, null, null, extras);
    }

    /// <summary>
    /// 009: 关注并授权开放平台帐号的第三方web应用 个人用户
    /// </summary>
    /// <param Name="appId"></param>
    public static void authorizeWebsite(String appId) {
        // 构建URL
        String url = ServiceCode.authorizeWebsite;
        url = url.Replace("{appId}", appId);
        // 构建返回参数
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        RestRequestHelper.post(url, EventDataType.authorizeWebsite, parameters,null, extras);
    }

    /// <summary>
    /// 取消关注并取消授权开放平台帐号的第三方web应用 个人用户
    /// </summary>
    /// <param Name="appId"></param>
    public static void unauthorizeWebsite(String appId) {
        // 构建URL
        String url = ServiceCode.unauthorizeWebsite;
        url = url.Replace("{appId}", appId);
        // 构建返回参数
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        RestRequestHelper.post(url, EventDataType.unauthorizeWebsite, parameters,null, extras);
    }

    /// <summary>
    /// 011: 点击开放平台服务包含的公众帐号的自定义菜单项
    /// </summary>
    /// <param Name="appId"></param>
    /// <param Name="menuCode"></param>
    public static void clickSubscriptionMenu(String appId, String menuCode,string tenantNo) {
        // 构建URL
        String url = ServiceCode.clickSubscriptionMenu;
        url = url.Replace("{appId}", appId);
        url = url.Replace("{menuCode}", menuCode);
        // 构建返回参数
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        extras.Add("menuCode", menuCode);
        extras.Add("tenantNo", tenantNo);
        RestRequestHelper.get(url, EventDataType.clickSubscriptionMenu, parameters, extras, false);
    }

    /// <summary>
    /// 012: 是否启用置顶聊天
    /// </summary>
    /// <param Name="enableFlag"></param>
    /// <param Name="appId"></param>
    public static void enableSubscriptionTopmost(Boolean enableFlag, String appId,string tenantNo) {
        // 构建URL
        String url = ServiceCode.enableSubscriptionTopmost;
        url = url.Replace("{appId}", appId);
        // 准备参数
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        parameters.Add("appId", appId);
        parameters.Add("enableFlag", enableFlag);
        parameters.Add("tenantNo", tenantNo);

        parameters.Add("showLoading", true);
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("enableFlag", enableFlag);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.enableSubscriptionTopmost,null, json, parameters);
    }

    /**
     * 013: 是否允许接收消息
     */
    public static void allowReceiveSubscriptionMessages(Boolean allowFlag, String appId,string tenantNo) {
        // 构建URL
        String url = ServiceCode.allowReceiveSubscriptionMessages;
        url = url.Replace("{appId}", appId);
        // 准备参数
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        parameters.Add("appId", appId);
        parameters.Add("allowFlag", allowFlag);
        parameters.Add("tenantNo", tenantNo);
        parameters.Add("showLoading", true);
        // 构建JSON
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        json.Add("allowFlag", allowFlag);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.allowReceiveSubscriptionMessages, null, json, parameters);
    }

    /// <summary>
    /// 014: 查找开放平台帐号的第三方web应用 个人用户
    /// </summary>
    /// <param Name="searchKey"></param>
    /// <param Name="page"></param>
    /// <param Name="size"></param>
    public static void searchWebsites(String searchKey, int page, int size) {
        //构建URL
        String url = ServiceCode.searchWebsites;
        //构建参数
        String parameters = "";
        if (searchKey!=null) {
            parameters = "&q = " + searchKey;
        }
        if (page > 0) {
            parameters = "&page=" + page;
        } else {
            parameters = "&page=0";
        }
        if (size > 0) {
            parameters = "&size=" + size;
        }
        url = url + parameters.Substring(1);
        RestRequestHelper.get(url, EventDataType.searchWebsites,null,null, false);
    }

    ///// <summary>
    ///// 015: 查找全部分类（mobile）
    ///// </summary>
    //public static void getDictionarypClassifications() {
    //    //构建URL
    //    String url = ServiceCode.getDictionarypClassifications;
    //    RestRequestHelper.get(url, EventDataType.getDictionarypClassifications,null,null);
    //}

    /// <summary>
    /// 015-1: 查找全部分类（mobile）2016.5.7 14:08 接口替换
    /// </summary>
    public static void appClassificationGroups(string tenantNo) {
        //构建URL
        String url = ServiceCode.appClassificationGroups;
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);
        RestRequestHelper.get(url, EventDataType.appClassificationGroups,null, extras, false);
    }

    /// <summary>
    /// 基础分类
    /// </summary>
    /// <param name="tenantNo"></param>
    public static void appBaseGroups(string tenantNo) {
        //构建URL
        String url = ServiceCode.appBaseGroups;
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        RestRequestHelper.post(url, EventDataType.appBaseGroups, null, json, extras);
    }



    /// <summary>
    /// XXX: 通知后台进入了公众号
    /// </summary>
    /// <param Name="appId"></param>
    public static void publicNoticeEnterServie(String appId) {
        try {
            // 构建URL
            String url = ServiceCode.publicNoticeEnterServie;
            url = url.Replace("{appId}", appId);
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("appId", appId);
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            // 发送请求
            RestRequestHelper.post(url, EventDataType.publicNoticeEnterServie, parameters,json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// XXX: 根据后台公众号的设置上传位置信息。
    /// </summary>
    /// <param Name="appId"></param>
    /// <param Name="latitude"></param>
    /// <param Name="longitude"></param>
    /// <param Name="scale"></param>
    /// <param Name="address"></param>
    public static void publicUploadLocation(String appId, String latitude, String longitude, String scale, String address) {
        try {
            // 构建URL
            String url = ServiceCode.publicUploadLocation;
            url = url.Replace("{appId}", appId);
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("appId", appId);
            // 构建JSON
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("latitude", latitude);
            json.Add("longitude", longitude);
            json.Add("address", address);

            // 发送请求
            RestRequestHelper.post(url, EventDataType.publicUploadLocation, parameters,json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /// <summary>
    /// 001: 查找近期的（最早60天前）的待办事项
    /// </summary>
    /// <param Name="t"></param>
    /// <param Name="status"></param>
    public static void getRecentTodoTasks(long t, String status, String tenantNo) {
        //构建URL
        String url = ServiceCode.getRecentTodoTasks;
        url = url.Replace("{t}", t.ToStr());
        url = url.Replace("{status}", status);

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);

        RestRequestHelper.get(url, EventDataType.getRecentTodoTasks,null, extras, false);
    }

    /// <summary>
    /// 告诉后台租户
    /// </summary>
    /// <param name="tenantNo"></param>
    public static void setTenantNo(String tenantNo) {
        //构建URL
        String url = ServiceCode.setTenantNo;
        url = url.Replace("{tenantNo}", tenantNo);
        RestRequestHelper.get(url, EventDataType.setTenantNo, null, null, false);
    }


    /// <summary>
    /// 002: 查找待办事项历史记录
    /// </summary>
    /// <param Name="start"></param>
    /// <param Name="end"></param>
    /// <param Name="status"></param>
    public static void getHistoryTodoTasks(long start, long end, String status) {
        //构建URL
        String url = ServiceCode.getHistoryTodoTasks;
        url = url.Replace("{start}", start.ToStr());
        url = url.Replace("{end}", end.ToStr());
        url = url.Replace("{status}", status);
        RestRequestHelper.get(url, EventDataType.getHistoryTodoTasks,null,null, false);
    }

    /// <summary>
    /// 003: 处理待办事项（标记已处理）
    /// </summary>
    /// <param Name="id"></param>
    public static void processTodoTasks(String id,string tenantNo) {
        // 构建URL
        String url = ServiceCode.processTodoTasks;
        url = url.Replace("{id}", id);
        // 准备参数
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("id", id);
        extras.Add("tenantNo", tenantNo);
        // 发送请求
        RestRequestHelper.put(url, EventDataType.processTodoTasks, null, null, extras);
    }
    // ----------------------------- 公众号 end -----------------------------

    // ----------------------------- 码表 sta -----------------------------

    /// <summary>
    /// 017 同步码表
    /// </summary>
    public static void syncMasterCode(TimestampType timestampType,string tenantNo) {
        try {
            //// 构建URL
            String url = ServiceCode.syncMasterCode;
            //// 构建返回参数
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            Dictionary<String, Object> body = new Dictionary<String, Object>();
            //foreach (MasterType type in Enum.GetValues(typeof(MasterType)))
            // {
            //Dictionary<String, Object> json = new Dictionary<String, Object>();
            IList<Dictionary<String, Object>> list = new List<Dictionary<String, Object>>();
            body.Add("category", timestampType.ToStr());
            body.Add("updatedTime", TimeServices.getInstance().GetTime(timestampType, tenantNo));
            list.Add(body);

            extras.Add("tenantNo", tenantNo);
            //jsonArray.put(Dictionary<String, Object>);
            //}
            RestRequestHelper.post(url, EventDataType.syncMasterCode, null, list, extras);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }
    // ----------------------------- 码表 end -----------------------------

    // ----------------------------- 组织管理 sta -----------------------------

    /// <summary>
    /// P001: 获取所有组织机构（团队）列表(MOBILE)
    /// </summary>
    public static void getMOrganizations(long time,string tenantNo) {
        // 构建URL
        String url = ServiceCode.getMOrganizations;
        //long longTimestamp = 0;
        // TODO ASDFASDFSDF
        //TimestampManager.getInstance().getTimestamp(TimestampType.Organization);
        url = url.Replace("{t}", time.ToStr());

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.getMOrganizations,null, extras, false);
    }

    /// <summary>
    /// P002: 获取所有组织用户列表(MOBILE)
    /// </summary>
    public static void getMOrganizationUsers(long time,string tenantNo) {
        // 构建URL
        String url = ServiceCode.getMOrganizationUsers;
        //long longTimestamp = 0;
        // TODO ASDFASDFASDFASDF
        //TimestampManager.getInstance().getTimestamp(TimestampType.OrganizationMember);
        url = url.Replace("{t}", time.ToStr());

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("tenantNo", tenantNo);

        // 发送请求
        RestRequestHelper.get(url, EventDataType.getMOrganizationUsers,null, extras, false);
    }
    // ----------------------------- 组织管理 end -----------------------------

    // ----------------------------- 其他 sta -----------------------------

    /// <summary>
    /// ping
    /// </summary>
    public static void ping() {
        // 构建URL
        String url = ServiceCode.ping;
        RestRequestHelper.get(url, EventDataType.ping,null,null, false);
    }

    /// <summary>
    /// 用户访问统计
    /// </summary>
    public static void clientUserAccessInfo() {
        // 构建URL
        String url = ServiceCode.clientUserAccessInfo;
        RestRequestHelper.post(url, EventDataType.clientUserAccessInfo,null,null,null);
    }

    /// <summary>
    /// 设为常用网站应用接口
    /// </summary>
    /// <param Name="appId"></param>
    public static void setCommonWebApp(String appId) {
        // 构建URL
        String url = ServiceCode.setCommonWebApp;
        url = url.Replace("{appId}", appId);
        //构建数据 TODO 参数？
        Dictionary<String, Object> json = new Dictionary<String, Object>();

        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        // 发送请求
        RestRequestHelper.post(url, EventDataType.setCommonWebApp, null,json, extras);
    }

    /// <summary>
    /// 取消设为常用网站应用
    /// </summary>
    /// <param Name="appId"></param>
    public static void unsetCommonWebApp(String appId) {
        // 构建URL
        String url = ServiceCode.unsetCommonWebApp;
        url = url.Replace("{appId}", appId);
        //构建数据 TODO 参数？
        Dictionary<String, Object> json = new Dictionary<String, Object>();
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        // 发送请求
        RestRequestHelper.post(url, EventDataType.unsetCommonWebApp, null,json, extras);
    }

    /**
     * 获取打开第三方应用的authCode
     */
    public static void authCode(String appId, String packagename, String todotaskid, int position) {
        // 构建URL
        String url = ServiceCode.authCode;
        url = url.Replace("{appId}", appId);

        //构建数据
        Dictionary<String, Object> parameters = new Dictionary<String, Object>();
        Dictionary<String, Object> extras = new Dictionary<String, Object>();
        extras.Add("appId", appId);
        extras.Add("packagename", packagename);
        extras.Add("todotaskid", todotaskid);
        extras.Add("position", position);
        // 发送请求
        RestRequestHelper.get(url, EventDataType.authCode, parameters, extras, false);
    }


    // ----------------------------- 其他 end -----------------------------


    // ----------------------------- CA验证-----------------------------


    /// <summary>
    /// 申请证书
    /// </summary>
    /// <param Name="csr"></param>
    public static void requestCert(String csr) {
        try {
            String url = ServiceCode.requestCert;
            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("csr", csr);
            RestRequestHelper.post(url, EventDataType.requestCert, null,json,null);
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }

    /**
     * 使用证书登录
     */
    public static void requsetCertLogin(LoginRequestModel loginRequestModel, Boolean background) {
        try {
            //String url = ServiceCode.requsetCertLogin;
            //Dictionary<String, Object> json = new Dictionary<String, Object>();
            //String GetTime = DateHelper.getDate(System.currentTimeMillis(), DateTimeType.yyyyMMddHHmm);
            //String random = new Random().nextInt(1000) + "";//设置一个1000以内的随机数
            //String signMessage = CAManager.getInstance().signMessage(GetTime + random);//添加签名
            //json.Add("GetTime", GetTime);
            //json.Add("random", random);
            //json.Add("signMessage", signMessage);

            //Dictionary<String, Object> extras = new Dictionary<String, Object>();
            //extras.Add("background", String.valueOf(background));
            //extras.Add("loginType", loginRequestModel.getLoginType().value());

            //RestRequestHelper.post(url, EventDataType.requsetCertLogin, json, extras);//测试环境先走http
            ////                HttpsHelper.getHttpsHelper().post(url, json, EventDataType.requsetCertLogin, extras);//正式环境先走https
        } catch (Exception e) {
            Log.Error(typeof(ContactsApi), e);
        }
    }




}
}
