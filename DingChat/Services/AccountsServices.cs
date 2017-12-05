using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event.EventData;
using EventBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Services {
public class AccountsServices : BaseService {
    private static AccountsServices instance = null;

    public static AccountsServices getInstance() {
        if (instance == null) {
            instance = new AccountsServices();
        }
        return instance;
    }

    /// <summary>
    /// 监听业务事件
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {

            case BusinessEventDataType.FileDownloadedEvent:
                DoFileDownloadedEvent(data);
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }

    }

    /// <summary>
    /// 图片下载完成事件处理（处理最后登录人头像用）
    /// </summary>
    /// <param Name="data"></param>
    private void DoFileDownloadedEvent(BusinessEvent<Object> data) {
        try {
            // todo 下载的头像显示需要先登录。
            // 获取下载的头像
            FileEventData fileBean = (FileEventData)data.data;
            // 保存当前登录人的头像到上次登录人记录中
            if (App.AccountsModel != null) {
                if (fileBean.fileStorageId.ToStr().Equals(App.AccountsModel.avatarStorageRecordId)) {

                    FilesTable filesTable =  FilesService.getInstance().getFile(fileBean.fileStorageId);
                    if (filesTable != null) {
                        String copyToPath = filesTable.localpath.Replace(App.AccountsModel.no, "default");
                        String copyToForder = copyToPath.Replace(fileBean.fileStorageId+".jpg", "");
                        //如果不存在就创建file文件夹
                        if (Directory.Exists(copyToForder) == false) {
                            Directory.CreateDirectory(copyToForder);
                        }
                        System.IO.File.Copy(filesTable.localpath, copyToPath, true);
                    }
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    public AccountsTable findByNo(String no) {
        AccountsTable table = AccountsDao.getInstance().findByNo(no);
        return table;
    }

    /// <summary>
    /// A001: 登录/自动登录 login
    /// A002: 校验access token verifyCredentials
    /// A003: 退出 logout
    /// E001: 获取我的个人详细信息 getMyDetail
    /// E002: 修改我的头像 changeAvatar
    /// E003: 设置免打扰 enableNoDisturb
    /// E004: 设置昵称 changeNickname
    /// E005: 设置地区 changeCity
    /// E006: 设置性别 chanGegender
    /// E007: 设置个人签名 changeMoodMessage
    /// E008: 加我为朋友时是否需要验证 changeNeedFriendConfirmation
    /// E009: 是否允许向我推荐通讯录朋友 changeAllowFindMobileContacts
    /// E010: 是否允许通过登录名称找到我 changeAllowFindMeByLoginId
    /// E011: 是否允许通过手机号码找到我 changeAllowFindMeByMobileNumber
    /// E015: 更新手机号 updateMoblie
    /// E016: 更新邮件 updateEmail
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            // A001: 登录/自动登录 login
            case EventDataType.login:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    // TODO：登录的处理方式和安卓不一样了
                    //A001(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // A002: 校验access token verifyCredentials
            case EventDataType.verifyCredentials:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    // TODO：不处理
                    //A002(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // A003: 退出 logout
            case EventDataType.logout:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    // TODO：不处理
                    //A003(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E001: 获取我的个人详细信息
            case EventDataType.getMyDetail:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E001(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        this.GetMyDetail();
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;
            // E002: 修改我的头像 changeAvatar
            case EventDataType.changeAvatar:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E002(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E003: 设置免打扰 enableNoDisturbMe
            case EventDataType.enableNoDisturbMe:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E003(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E004: 设置昵称 changeNickname
            case EventDataType.changeNickname:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E004(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E005: 设置地区 changeCity
            case EventDataType.changeCity:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E005(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E006: 设置性别 chanGegender
            case EventDataType.chanGegender:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E006(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E007: 设置个人签名 changeMoodMessage
            case EventDataType.changeMoodMessage:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E007(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E008: 加我为朋友时是否需要验证 changeNeedFriendConfirmation
            case EventDataType.changeNeedFriendConfirmation:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E008(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E009: 是否允许向我推荐通讯录朋友 changeAllowFindMobileContacts
            case EventDataType.changeAllowFindMobileContacts:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E009(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E010: 是否允许通过登录名称找到我 changeAllowFindMeByLoginId
            case EventDataType.changeAllowFindMeByLoginId:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E010(eventData);
                }
                // API请求失败
                else {

                }
                break;

            // E011: 是否允许通过手机号码找到我 changeAllowFindMeByMobileNumber
            case EventDataType.changeAllowFindMeByMobileNumber:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E011(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E015: 更新手机号 updateMoblie
            case EventDataType.updateMoblie:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E015(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E016: 更新邮件 updateEmail
            case EventDataType.updateEmail:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E016(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // Q002: 拉取我的二维码 getMyQRcode
            case EventDataType.getMyQRcode:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    Q002(eventData);
                }
                // API请求失败
                else {

                }
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }

    }

    /// <summary>
    /// E001: 获取我的个人详细信息
    /// </summary>
    /// <param Name="eventData"></param>
    public void E001(EventData<Object> eventData) {
        try {
            Object contacts = eventData.data;

            AccountsBean accountsBean = JsonConvert.DeserializeObject<AccountsBean>(contacts.ToStr());
            if (accountsBean == null) return;

            AccountsTable table = AccountsDao.getInstance().findByNo(accountsBean.uap.no);

            if (table == null) {
                table = new AccountsTable();
            }

            table = Convertors(table, accountsBean);

            AccountsDao.getInstance().save(table);

            App.AccountsModel = table;
            App.AccountsModel.clientuserId = accountsBean.core.id.ToStr();

            LoginServices.setClientUserInfoToCookie();
            ///
            //下头像
            if (table.avatarStorageRecordId != null) {
                DownloadServices.getInstance().DownloadMethod(table.avatarStorageRecordId, DownloadType.SYSTEM_IMAGE, null);
            }
            // 拉取我的二维码(转移到登录后下载）
            //this.getMyQRcode();
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    private AccountsTable Convertors(AccountsTable model, AccountsBean accountsBean) {
        //BeanUtils.copyProperties(model, accountsBean.core);
        //BeanUtils.copyProperties(model, accountsBean.uap);
        //model.id = accountsBean.core.id;
        model.avatarStorageRecordId = accountsBean.core.avatarStorageRecordId;
        model.qrcodeId = accountsBean.core.qrcodeId;
        model.enableNoDisturb = accountsBean.core.enableNoDisturb;
        model.startTimeOfNoDisturb = accountsBean.core.startTimeOfNoDisturb;
        model.endTimeOfNoDisturb = accountsBean.core.endTimeOfNoDisturb;
        model.needFriendConfirmation = accountsBean.core.needFriendConfirmation;
        model.allowFindMobileContacts = accountsBean.core.allowFindMobileContacts;
        model.allowFindMeByLoginId = accountsBean.core.allowFindMeByLoginId;
        model.allowFindMeByMobileNumber = accountsBean.core.allowFindMeByMobileNumber;

        model.no = accountsBean.uap.no;
        model.nickname = accountsBean.uap.nickname;
        model.mobile = accountsBean.uap.mobile;
        model.email = accountsBean.uap.email;
        model.loginId = accountsBean.uap.loginId;
        model.gender = accountsBean.uap.gender;

        model.clientuserId = accountsBean.core.id.ToStr();
        return model;
    }

    /// <summary>
    /// E002: 修改我的头像 changeAvatar
    /// </summary>
    /// <param Name="eventData"></param>
    public void E002(EventData<Object> eventData) {
        try {

            String avatarStorageId = eventData.extras["avatarStorageId"].ToStr();

            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table!=null) {
                table.avatarStorageRecordId = avatarStorageId.ToStr();
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E003: 设置免打扰 enableNoDisturb
    /// </summary>
    /// <param Name="eventData"></param>
    public void E003(EventData<Object> eventData) {
        try {
            Boolean enableNoDisturb = eventData.extras["enableNoDisturb"].ToStr().ToBool();
            String startTimeOfNoDisturb = eventData.extras["startTimeOfNoDisturb"].ToStr();
            String endTimeOfNoDisturb = eventData.extras["endTimeOfNoDisturb"].ToStr();

            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.enableNoDisturb = enableNoDisturb;
                table.startTimeOfNoDisturb = startTimeOfNoDisturb;
                table.endTimeOfNoDisturb = endTimeOfNoDisturb;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E004: 设置昵称 changeNickname
    /// </summary>
    /// <param Name="eventData"></param>
    public void E004(EventData<Object> eventData) {
        try {
            String nickname = eventData.extras["nickname"].ToStr();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.nickname = nickname;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E005: 设置地区 changeCity
    /// </summary>
    /// <param Name="eventData"></param>
    public void E005(EventData<Object> eventData) {
        try {
            String country = eventData.extras["country"].ToStr();
            String province = eventData.extras["province"].ToStr();
            String city = eventData.extras["city"].ToStr();

            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.country = country;
                table.province = province;
                table.city = city;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E006: 设置性别 chanGegender
    /// </summary>
    /// <param Name="eventData"></param>
    public void E006(EventData<Object> eventData) {
        try {
            String gender = eventData.extras["gender"].ToStr();

            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.gender = gender;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E007: 设置个人签名 changeMoodMessage
    /// </summary>
    /// <param Name="eventData"></param>
    public void E007(EventData<Object> eventData) {
        try {
            String desc = eventData.extras["desc"].ToStr();

            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.desc = desc;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E008: 加我为朋友时是否需要验证 needFriendConfirmation
    /// </summary>
    /// <param Name="eventData"></param>
    public void E008(EventData<Object> eventData) {
        try {
            Boolean needFriendConfirmation = eventData.extras["needFriendConfirmation"].ToStr().ToBool();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.needFriendConfirmation = needFriendConfirmation;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E009: 是否允许向我推荐通讯录朋友 allowFindMobileContacts
    /// </summary>
    /// <param Name="eventData"></param>
    public void E009(EventData<Object> eventData) {
        try {
            Boolean allowFindMobileContacts = eventData.extras["allowFindMobileContacts"].ToStr().ToBool();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.allowFindMobileContacts = allowFindMobileContacts;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E010: 是否允许通过登录名称找到我 changeAllowFindMeByLoginId
    /// </summary>
    /// <param Name="eventData"></param>
    public void E010(EventData<Object> eventData) {
        try {
            Boolean allowFindMeByLoginId = eventData.extras["allowFindMeByLoginId"].ToStr().ToBool();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.allowFindMeByLoginId = allowFindMeByLoginId;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E011: 是否允许通过手机号码找到我 allowFindMeByMobileNumber
    /// </summary>
    /// <param Name="eventData"></param>
    public void E011(EventData<Object> eventData) {
        try {
            Boolean allowFindMeByMobileNumber = eventData.extras["allowFindMeByMobileNumber"].ToStr().ToBool();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.allowFindMeByMobileNumber = allowFindMeByMobileNumber;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E015: 更新手机号 updateMoblie
    /// </summary>
    /// <param Name="eventData"></param>
    public void E015(EventData<Object> eventData) {
        try {
            String mobile = eventData.extras["mobile"].ToStr();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.mobile = mobile;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// E016: 更新邮件 updateEmail
    /// </summary>
    /// <param Name="eventData"></param>
    public void E016(EventData<Object> eventData) {
        try {
            String email = eventData.extras["email"].ToStr();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.email = email;
                AccountsDao.getInstance().save(table);
            }
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// Q002: 拉取我的的二维码 getMyQRcode
    /// </summary>
    /// <param Name="eventData"></param>
    public void Q002(EventData<Object> eventData) {
        try {
            JObject jsonData = JObject.Parse(eventData.data.ToStr());
            String qrcodeId = jsonData.GetValue("qrcodeId").ToStr();
            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);
            table.qrcodeId = qrcodeId;
            AccountsDao.getInstance().save(table);
            // 下载二维码   /qrcode/{id}/download

            //DownloadServices.getInstance().DownloadMethod(qrcodeId, DownloadType.SYSTEM_IMAGE, null);
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// 处理IM消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {

        // 获取消息类型
        MsgType msgType = messageArrivedEvent.message.getType();
        switch (msgType) {
        case MsgType.UserAvatarChanged:
            processUserAvatarChangedMessage(messageArrivedEvent.message);
            break;
        // 用户禁用消息
        case MsgType.UserDisabled:
            onUserDisabledMsg();
            break;
        default:
            break;
        }
    }
    private void processUserAvatarChangedMessage(Message message) {
        UserAvatarChangedMessage userAvatarChangedMessage = (UserAvatarChangedMessage)message;

        try {
            // 更新联系人
            if (App.AccountsModel.clientuserId != userAvatarChangedMessage.getUserId().ToStr()) {
                return;
            }

            String avatarStorageId = userAvatarChangedMessage.getAvatarStorageId();

            AccountsTable table = AccountsDao.getInstance().findByNo(App.AccountsModel.no);

            if (table != null) {
                table.avatarStorageRecordId = avatarStorageId.ToStr();
                AccountsDao.getInstance().save(table);
            }
            // 下载头像

            DownloadServices.getInstance().DownloadMethod(userAvatarChangedMessage.getAvatarStorageId(), DownloadType.SYSTEM_IMAGE, null);

            // TODO:这个地方后续处理，等待3秒再通知画面刷新,后续应该监听下载完成的事件，如果下载完成再通知画面刷新
            Thread.Sleep(3000);
            // 通知通讯录画面更新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.eventDataType = BusinessEventDataType.MyDetailsChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }



    }
    /// <summary>
    /// 用户禁用消息处理
    /// </summary>
    public void onUserDisabledMsg() {
        try {

            // TODO：强行退出系统，这个的处理再考虑，不一定需要发事件通知。

            // 设置用户状态为被禁用
            // 退出系统到登录画面并提示用户被禁用
            //BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            //businessEvent.eventDataType = BusinessEventDataType.UserDisabled;
            //EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(AccountsServices), e);
        }
    }

    /// <summary>
    /// 使用一次性Token登录
    /// </summary>
    public void autoLoginWithNonceToken() {
        //String nonceToken = CacheHelper.getAccessToken();
        //if (!ToolsHelper.isNull(nonceToken))
        //{
        //    LoginRequestModel loginRequestModel = new LoginRequestModel();
        //    loginRequestModel.setLoginType(LoginType.nonceToken);
        //    loginRequestModel.setNonceToken(nonceToken);
        //    CoreHttpApi.login(loginRequestModel, true);
        //}
    }

    /// <summary>
    /// E001: 获取我的个人详细信息 getMyDetail
    /// </summary>
    public void GetMyDetail() {
        ContactsApi.getMyDetail();
    }

    /// <summary>
    /// Q002:请求二维码
    /// </summary>
    public void getMyQRcode() {
        ContactsApi.getMyQRcode();
    }
}
}
