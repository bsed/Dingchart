using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Services.core {
public class VcardService {
    private static VcardService instance = null;
    public static VcardService getInstance() {
        if (instance == null) {
            instance = new VcardService();
        }
        return instance;
    }

    /// <summary>
    /// 调用群详细列表api
    /// </summary>
    public void request(String clientUserId) {
        if (ContactsServices.getInstance().isFriend(long.Parse(clientUserId))) {
            ContactsApi.getFriend(clientUserId);
        } else {
            ContactsApi.getStranger(clientUserId);
        }
    }

    /// <summary>
    /// 好友相关通知消息
    /// ContactAdded("123")
    /// UserAvatarChanged("125") 好友头像变更通知消息
    /// UserNicknameChanged("126") 好友昵称变更通知消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            // 获取消息类型
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            //case MsgType.ContactAdded:
            //    processContactAddedMessage(message);
            //    break;
            case MsgType.UserAvatarChanged:
                processUserAvatarChangedMessage(message);
                break;
            case MsgType.UserNicknameChanged:
                processUserNicknameChangedMessage(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }

    /// <summary>
    /// API请求处理
    /// C002: 获取好友详细 getFriend
    /// C002.2: 获陌生人详细 getStranger
    /// C014.2: 获取群成员详细信息 getGroupMember
    /// E001: 获取我的个人详细信息 getMyDetail
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            //  C002: 获取好友详细 getFriend
            case EventDataType.getFriend:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C002(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //  C002.2: 获陌生人详细 getStranger
            case EventDataType.getStranger:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C002_2(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C014.2: 获取群成员详细信息 getGroupMember（在VcardsManager中处理业务，这里处理拉取标志）
            case EventDataType.getGroupMember:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C014_2(eventData);
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

                }
                break;
            default:
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }

    }

    /// <summary>
    /// C002: 获取好友详细 getFriend
    /// </summary>
    /// <param Name="data"></param>
    public void C002(EventData<Object> eventData) {
        try {
            Object contacts = eventData.data;
            VcardsTableBean vsCard = JsonConvert.DeserializeObject<VcardsTableBean>(contacts.ToStr());
            if (vsCard == null) return;
            VcardsTable table = VcardsDao.getInstance().findByNo(vsCard.no);
            if (table==null) {
                table = new VcardsTable();
            }
            Convertors(table, vsCard);
            VcardsDao.getInstance().save(table);
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }

    /// <summary>
    ///  C002.2: 获陌生人详细 getStranger
    /// </summary>
    /// <param Name="data"></param>
    public void C002_2(EventData<Object> eventData) {
        try {
            Object contacts = eventData.data;
            VcardsTableBean vsCard = JsonConvert.DeserializeObject<VcardsTableBean>(contacts.ToStr());
            if (vsCard == null) return;
            VcardsTable table = VcardsDao.getInstance().findByNo(vsCard.no);
            if (table == null) {
                table = new VcardsTable();
            }
            Convertors(table, vsCard);
            VcardsDao.getInstance().save(table);
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }

    /// <summary>
    /// C014.2: 获取群成员详细信息 getGroupMember
    /// </summary>
    /// <param Name="extras"></param>
    private void C014_2(EventData<Object> eventData) {
        try {
            Object contacts = eventData.data;
            VcardsTableBean vsCard = JsonConvert.DeserializeObject<VcardsTableBean>(contacts.ToStr());
            if (vsCard == null) return;
            VcardsTable table = VcardsDao.getInstance().findByNo(vsCard.no);
            // 因为拉取群成员，需要更新本地表时，优先判断本地是否已经有vcard，如果已经有，则不更新。
            if (table == null) {
                table = new VcardsTable();
                Convertors(table, vsCard);
                VcardsDao.getInstance().save(table);
            }

        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
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

            VcardsTable vcardsTable = VcardsDao.getInstance().findByNo(accountsBean.uap.no);
            if (vcardsTable == null) {
                vcardsTable = new VcardsTable();
            }

            if (!string.IsNullOrEmpty(accountsBean.core.id.ToStr())) {
                vcardsTable.clientuserId = accountsBean.core.id.ToStr();
            }
            if (!string.IsNullOrEmpty(accountsBean.uap.no)) {
                vcardsTable.no = accountsBean.uap.no;
            }
            if (!string.IsNullOrEmpty(accountsBean.uap.nickname)) {
                vcardsTable.nickname = accountsBean.uap.nickname;
            }
            if (!string.IsNullOrEmpty(accountsBean.core.avatarStorageRecordId)) {
                vcardsTable.avatarStorageRecordId = accountsBean.core.avatarStorageRecordId;
            }
            if (!string.IsNullOrEmpty(accountsBean.uap.mobile)) {
                vcardsTable.mobileNumber = accountsBean.uap.mobile;
            }

            if (!string.IsNullOrEmpty(accountsBean.uap.email)) {
                vcardsTable.email = accountsBean.uap.email;
            }
            if (!string.IsNullOrEmpty(accountsBean.uap.gender)) {
                vcardsTable.gender = accountsBean.uap.gender;
            }
            // 以下属性在拉取登录人详情的API中不存在，所以先不处理。
            //vcardsTable.birthday = bean.birthday;
            //vcardsTable.country = bean.country;
            //vcardsTable.province = bean.province;
            //vcardsTable.city = bean.city;
            //vcardsTable.identity = bean.identity;
            //vcardsTable.desc = bean.moodMessage;

            VcardsDao.getInstance().save(vcardsTable);
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }
    /// <summary>
    /// 将联系人添加到通讯录的通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processContactAddedMessage(Message message) {
        try {
            ContactAddedMessage contactAddedMessage = (ContactAddedMessage)message;
            //请求个人详情
            VcardService.getInstance().request(contactAddedMessage.getUserId().ToStr());
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }


    /// <summary>
    /// 好友头像变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserAvatarChangedMessage(Message message) {

        try {
            UserAvatarChangedMessage userAvatarChangedMessage = (UserAvatarChangedMessage)message;
            VcardsTable vcardsTable = VcardsDao.getInstance().findByClientuserId(userAvatarChangedMessage.getUserId().ToStr());
            if (vcardsTable!=null) {
                vcardsTable.avatarStorageRecordId = userAvatarChangedMessage.getAvatarStorageId().ToStr();
                VcardsDao.getInstance().save(vcardsTable);
            }
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }

    /// <summary>
    /// 好友昵称变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserNicknameChangedMessage(Message message) {
        try {
            UserNicknameChangedMessage userNicknameChangedMessage = (UserNicknameChangedMessage)message;
            VcardsTable vcardsTable = VcardsDao.getInstance().findByClientuserId(userNicknameChangedMessage.getUserId().ToStr());
            if (vcardsTable != null) {
                vcardsTable.nickname = userNicknameChangedMessage.getNickname();
                VcardsDao.getInstance().save(vcardsTable);
            }
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
        }
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param Name="model"></param>
    /// <param Name="bean"></param>
    /// <returns></returns>
    private VcardsTable Convertors(VcardsTable model, VcardsTableBean bean) {

        if (!string.IsNullOrEmpty(bean.id.ToStr())) {
            model.clientuserId = bean.id.ToStr();
        }
        if (!string.IsNullOrEmpty(bean.no)) {
            model.no = bean.no;
        }
        if (!string.IsNullOrEmpty(bean.nickname)) {
            model.nickname = bean.nickname;
        }
        model.avatarStorageRecordId = bean.avatarStorageRecordId;

        // TODO: (#‵′) :临时的处理方法，查询好友、拉取成员详情等接口，电话的返回字段不一致。
        if (!string.IsNullOrEmpty(bean.mobileNumber)) {
            model.mobileNumber = bean.mobileNumber;
        }
        if (!string.IsNullOrEmpty(bean.mobile)) {
            model.mobileNumber = bean.mobile;
        }

        if (!string.IsNullOrEmpty(bean.email)) {
            model.email = bean.email;
        }
        model.birthday = bean.birthday;

        if (!string.IsNullOrEmpty(bean.gender)) {
            model.gender = bean.gender;
        }

        model.country = bean.country;
        model.province = bean.province;
        model.city = bean.city;
        model.identity = bean.identity;
        model.desc = bean.moodMessage;
        // 如果是当前登录人
        if (App.AccountsModel.no.Equals(bean.no)) {
            AccountsTable accountsTable = AccountsServices.getInstance().findByNo(bean.no);
            if (accountsTable != null) {
                model.mobileNumber = accountsTable.mobile;
                model.email = accountsTable.email;
                model.gender = accountsTable.gender;
            } else {
                model.mobileNumber = App.AccountsModel.mobile;
                model.email = App.AccountsModel.email;
                model.gender = App.AccountsModel.gender;
            }

        }
        return model;
    }

    /// <summary>
    /// 通过id查找
    /// </summary>
    /// <param Name="clientUserId"></param>
    /// <returns></returns>
    public VcardsTable findByClientuserId(long clientUserId) {
        try {
            return VcardsDao.getInstance().findByClientuserId(clientUserId.ToStr());
        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
            return null;
        }
    }

    /// <summary>
    /// 通过no查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public VcardsTable findByNo(String no) {
        try {
            return VcardsDao.getInstance().findByNo(no);

        } catch (Exception e) {
            Log.Error(typeof(VcardService), e);
            return null;
        }
    }
}
}
