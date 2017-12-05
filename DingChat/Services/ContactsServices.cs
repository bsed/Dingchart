using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Services {
/// <summary>
/// 通讯录
/// </summary>
public class ContactsServices : BaseService {


    private static ContactsServices instance = null;
    public static ContactsServices getInstance() {
        if (instance == null) {
            instance = new ContactsServices();
        }
        return instance;
    }

    ContactsDao contactsDao = DataSqlite.ContactsDao.getInstance();





    DownloadServices _downloadServices = new DownloadServices();

    private TimestampDao timeDao = new TimestampDao();
    Thread _downLoadThread = null;

    Thread _downLoadQlMemberThread = null;

    private List<object> DownLoadGroupMember = new List<object>();

    /// <summary>
    /// 好友相关通知消息
    /// FriendRequest("121") 在ContactsRequestManager中处理
    /// FriendResponse("122") 在ContactsRequestManager中处理？？？
    /// ContactAdded("123")
    /// ContactDeleted("124")
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
            case MsgType.ContactAdded:
                processContactAddedMessage(message);
                break;
            case MsgType.ContactDeleted:
                processContactDeletedMessage(message);
                break;
            case MsgType.UserAvatarChanged:
                processUserAvatarChangedMessage(message);
                break;
            case MsgType.UserNicknameChanged:
                processUserNicknameChangedMessage(message);
                break;
            case MsgType.FriendResponse:
                processFriendResponseMessage(message);
                break;
            default:
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C001: 获取好友列表 friends
    /// C002: 获取好友详细 getFriend
    /// C003: 设置好友标签 changeTags
    /// C003_: 设置好友备注名 changeAlias
    /// C004: 标记为星标朋友 markFavorite
    /// C006: 添加新朋友 addFriend
    /// C007: 删除朋友 deleteFriend
    /// C008: 接受新朋友请求 acceptFriend
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            //  C001: 获取好友列表 friends
            case EventDataType.friends:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C001(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        this.Contacts();
                    } else {
                        // 否则标识加载完成，交由DataPullService来处理关键数据同步问题
                        this.FireContactsLoadOk();
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;

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
            //  C003: 设置好友标签 changeTags
            case EventDataType.changeTags:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C003(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //  C003_1: 设置好友备注名 changeAlias
            case EventDataType.changeAlias:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C003_1(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //  C004: 标记为星标朋友 markFavorite
            case EventDataType.markFavorite:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C004(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //  C006: 获取好友详细 getFriend
            case EventDataType.addFriend:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C006(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //  C007: 删除朋友 deleteFriend
            case EventDataType.deleteFriend:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C007(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C008: 接受新朋友请求 acceptFriend
            case EventDataType.acceptFriend:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C008(eventData);
                }
                // API请求失败
                else {

                }
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    ///  触发联系人拉取玩出事件
    /// </summary>
    private void FireContactsLoadOk() {
        try {
            App.ContactsLoadOk = true;
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C001: 获取好友列表 friends
    /// </summary>
    /// <param Name="data"></param>
    public void C001(EventData<Object> data) {
        try {
            Object contacts = data.data;
            List<ContactsTableBean> listContacts =
                JsonConvert.DeserializeObject<List<ContactsTableBean>>(contacts.ToStr(),
                        new convertor<ContactsTableBean>());

            if (listContacts.Count == 0) {
                this.FireContactsLoadOk();
                return;
            }

            App.FriendDatatable = listContacts;

            //下头像转移到进入到主画面
            //_downLoadThread = new Thread(new ParameterizedThreadStart(DownLoad));
            //_downLoadThread.Start(ContactsTpye.LXR);

            //插入好友列表
            List<ContactsTable> modelList = new List<ContactsTable>();
            modelList = Convertors(modelList, listContacts);
            contactsDao.InsertFriend(modelList);

            if (data.timestamp != 0) {
                //通讯录不区分租户 传空就行
                TimeServices.getInstance().SaveTime(TimestampType.CONTACT, data.timestamp,string.Empty);
            }



            for (int i = 0; i < listContacts.Count; i++) {
                ContactsTableBean bean = listContacts[i];
                // 头像下载加入下载缓存
                DownloadBean downloadBean = new DownloadBean(bean.avatarStorageRecordId, DownloadType.SYSTEM_IMAGE, null);
                DownloadServices.getInstance().DownloadCatchAdd(downloadBean);

                GetContactsDetaile(bean.id);

                //执行完事之后才能进入主界面
                if (i == listContacts.Count - 1) {
                    this.FireContactsLoadOk();
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C002: 获取好友详细 getFriend
    /// </summary>
    /// <param Name="data"></param>
    public void C002(EventData<Object> data) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C003: 设置好友标签 changeTags
    /// </summary>
    /// <param Name="data"></param>
    public void C003(EventData<Object> data) {
        try {
            // TODO 设置标签好像在这里不需要做啥
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C003_1: 设置好友备注名 changeAlias
    /// </summary>
    /// <param Name="data"></param>
    public void C003_1(EventData<Object> data) {
        try {
            Object contacts = data.data;
            // 获取跟踪参数
            String clientUserId = data.extras["id"].ToStr();
            String alias = data.extras["alias"].ToStr();

            ContactsTable table = contactsDao.FindContactsById(clientUserId.ToInt());
            if (null != table) {
                table.alias=alias;

                int count=     contactsDao.SaveFriend(table);
                if (count > 0) {
                    // 通知画面有变更。
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = table;
                    businessEvent.eventDataType = BusinessEventDataType.ContactsDetailsChangeEvent;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }


            }

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C004: 标记为星标朋友 markFavorite
    /// </summary>
    /// <param Name="data"></param>
    public void C004(EventData<Object> data) {
        try {
            // 获取跟踪参数
            String clientUserId = data.extras["id"].ToStr();
            bool favorite =data.extras["favorite"].ToStr().ToBool();

            ContactsTable table = contactsDao.FindContactsById(clientUserId.ToInt());
            if (null != table) {
                table.favorite = favorite;

                int count=     contactsDao.SaveFriend(table);
                if (count > 0) {
                    // 通知画面有变更。
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = table;
                    businessEvent.eventDataType = BusinessEventDataType.ContactsDetailsChangeEvent;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }


    /// <summary>
    /// C006: 获取好友详细 getFriend
    /// </summary>
    /// <param Name="data"></param>
    public void C006(EventData<Object> data) {
        try {
            Object contacts = data.data;
            ContactsTableBean Contacts =
                JsonConvert.DeserializeObject<ContactsTableBean>(contacts.ToStr());


            //插入好友列表
            ContactsTable table = new ContactsTable();
            table = Convertors(table, Contacts);
            //查看是否是好友
            ContactsTable  find=    contactsDao.FindContactsById(table.clientuserId.ToInt());
            if (find != null) return;

            table.flag = FriendStatusType.normal.ToStr();
            table. favorite = false;
            contactsDao.SaveFriend(table);

            GetContactsDetaile(Contacts.id);

            //下头像
            if (Contacts.avatarStorageRecordId != null) {
                DownloadServices.getInstance().DownloadMethod(Contacts.avatarStorageRecordId, DownloadType.SYSTEM_IMAGE, null);
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }

        // 通知通讯录画面更新
        BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
        Businessdata.eventDataType = BusinessEventDataType.ContactsChangedEvent;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

    /// <summary>
    ///C007: 删除朋友 deleteFriend
    /// </summary>
    /// <param Name="data"></param>
    public void C007(EventData<Object> data) {
        try {
            // 获取跟踪参数
            String id =data.extras["id"].ToStr();
            int count= contactsDao.deleteByUserId(id);
            if (count > 0) {
                BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
                Businessdata.data = id;
                Businessdata.eventDataType = BusinessEventDataType.ContactsChangedEvent_TYPE_API_DELETE_Contacts;
                EventBusHelper.getInstance().fireEvent(Businessdata);
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }


    /// <summary>
    ///C008: 接受新朋友请求 acceptFriend
    /// </summary>
    /// <param Name="data"></param>
    public void C008(EventData<Object> data) {
        try {

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// 将联系人添加到通讯录的通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processContactAddedMessage(Message message) {
        try {
            ContactAddedMessage contactAddedMessage = (ContactAddedMessage)message;

            long userId = 0;
            string userNo = string.Empty;
            string avatarId = string.Empty;
            string name = string.Empty;
            //如果消息中的人是我自己的话 就说明是我个人的同步消息
            if (contactAddedMessage.getUserId().ToStr()==App.AccountsModel.clientuserId) {
                userId = contactAddedMessage.getAddUserId();
                userNo= contactAddedMessage.getAddUserNo();
                avatarId= contactAddedMessage.getAddAvatarId().ToStr();
                name= contactAddedMessage.getAddNickname();
            } else {
                userId = contactAddedMessage.getUserId();
                userNo = contactAddedMessage.getUserNo();
                avatarId = contactAddedMessage.getAvatarStorageId().ToStr();
                name = contactAddedMessage.getNickname();
            }
            ContactsTable contactsTable = ContactsDao.getInstance().FindContactsById(userId);
            if (contactsTable == null) {
                contactsTable = new ContactsTable();
            }
            contactsTable.clientuserId = userId.ToStr();
            contactsTable.no = userNo;
            contactsTable.avatarStorageRecordId = avatarId;
            contactsTable.name = name;
            contactsTable.alias = name;
            contactsTable.favorite = false;
            contactsTable.flag = "normal";
            ContactsDao.getInstance().SaveFriend(contactsTable);

            VcardService.getInstance().request(contactsTable.clientuserId);
            // 通知通讯录画面更新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.eventDataType = BusinessEventDataType.ContactsChangedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);


        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }



    /// <summary>
    /// 对方将”我“删除好友了
    /// </summary>
    /// <param Name="message"></param>
    private void processContactDeletedMessage(Message message) {
        try {
            ContactDeletedMessage contactDeletedMessage = (ContactDeletedMessage)message;
            String userId = contactDeletedMessage.getDelUserId();
            ContactsDao.getInstance().deleteByUserId(userId);
            // 通知通讯录画面更新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.eventDataType = BusinessEventDataType.ContactsChangedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
            //ContactsTable contactsTable = ContactsDao.getInstance().FindContactsByNo(userNo);
            //if (contactsTable!=null) {
            //    contactsTable.deletedBy = true;
            //    ContactsDao.getInstance().deleteByUserId(userId);
            //}
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// 好友头像变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserAvatarChangedMessage(Message message) {
        UserAvatarChangedMessage userAvatarChangedMessage = (UserAvatarChangedMessage)message;

        try {
            // 更新联系人
            ContactsTable contactsTable = ContactsDao.getInstance().FindContactsById(userAvatarChangedMessage.getUserId());
            if (null == contactsTable) {
                return;
            }
            contactsTable.avatarStorageRecordId = userAvatarChangedMessage.getAvatarStorageId().ToStr();
            ContactsDao.getInstance().SaveFriend(contactsTable);

            // 更新群成员
            MucServices.getInstance().saveMucMemberAvatarByTable(contactsTable.no, userAvatarChangedMessage.getAvatarStorageId().ToStr());

            // 下载头像

            DownloadServices.getInstance().DownloadMethod(userAvatarChangedMessage.getAvatarStorageId(), DownloadType.SYSTEM_IMAGE, null);

            // TODO:这个地方后续处理，等待3秒再通知画面刷新,后续应该监听下载完成的事件，如果下载完成再通知画面刷新
            Thread.Sleep(3000);
            // 通知通讯录画面更新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = contactsTable;
            businessEvent.eventDataType = BusinessEventDataType.ContactsDetailsChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }

        // TODO:缓存管理 sta
        //try {
        //    CacheManager.getInstance()
        //    .getVcard(ToolsHelper.toString(userAvatarChangedMessage.getUserId()))
        //    .setAvatarId(ToolsHelper.toString(userAvatarChangedMessage.getAvatarStorageId()));
        //} catch (Exception e) {
        //    Log.Error(typeof(ContactsServices), e);
        //}
        // 缓存管理 end


    }

    /// <summary>
    /// 好友昵称变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserNicknameChangedMessage(Message message) {
        try {
            UserNicknameChangedMessage userNicknameChangedMessage = (UserNicknameChangedMessage)message;
            ContactsTable contactsTable = ContactsDao.getInstance().FindContactsById(userNicknameChangedMessage.getUserId());
            if (null == contactsTable)
                return;

            // 获取昵称修改的相关信息
            contactsTable.name = userNicknameChangedMessage.getNickname();
            // 修改联系人的昵称
            ContactsDao.getInstance().SaveFriend(contactsTable);

            // 通知通讯录画面更新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = contactsTable;
            businessEvent.eventDataType = BusinessEventDataType.ContactsDetailsChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }



        // TODO: 缓存管理 sta
        //try
        //{
        //    // 如果未设置过备注
        //    if (ToolsHelper.isNull(contactsTable.getAlias()))
        //    {
        //        CacheManager.getInstance()
        //                .getVcard(ToolsHelper.toString(userNicknameChangedMessage.getUserId()))
        //                .setNickName(userNicknameChangedMessage.getNickname());
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Log.Error(typeof(ContactsServices), e);
        //}
        // 缓存管理 end
    }

    /// <summary>
    /// 好友昵称变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processFriendResponseMessage(Message message) {
        //LogHelper.d(String.format("消息跟踪：对方接受了我的好友申请通知，正在处理"));
        //FriendResponseMessage msgAccept = (FriendResponseMessage)message;
        //this.insertByFriendId(ToolsHelper.toString(msgAccept.getClientUserId()));
        //LogHelper.d(String.format("消息跟踪：对方接受了我的好友申请通知，处理完成"));
    }

    ///// <summary>
    ///// 下载头像
    ///// </summary>
    //private void DownLoad(object type) {
    //    //群成员头像
    //    if ((ContactsTpye)type == ContactsTpye.QLMEMBER)
    //    {
    //        if (DownLoadGroupMember.Count == 0) return;
    //        for (int i = 0; i < DownLoadGroupMember.Count; i++)
    //        {
    //            if (DownLoadGroupMember[i] != null)
    //            {
    //                _downloadServices.DownloadMethod(DownLoadGroupMember[i].ToStr().ToInt());
    //            }
    //        }
    //    }

    //    ////群头像
    //    if ((ContactsTpye)type == ContactsTpye.QL)
    //    {
    //        if (_groupContactstable.Rows.Count == 0) return;
    //        for (int i = 0; i < _groupContactstable.Rows.Count; i++)
    //        {
    //            if (_groupContactstable.Rows[i]["AvatarStorageRecordId"] != null)
    //            {
    //                _downloadServices.DownloadMethod(
    //                    Convert.ToInt32(_groupContactstable.Rows[i]["AvatarStorageRecordId"]));
    //            }
    //        }
    //    }

    //    //联系人头像
    //    if ((ContactsTpye)type == ContactsTpye.LXR) {
    //        if (App.FriendDatatable == null || App.FriendDatatable.Count == 0) return;

    //        for (int i = 0; i < App.FriendDatatable.Count; i++) {
    //            if (App.FriendDatatable[i].AvatarStorageRecordId != null) {
    //                DownloadServices.getInstance().DownloadMethod(App.FriendDatatable[i].AvatarStorageRecordId, DownloadType.SYSTEM, null);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 调用查询陌生人api
    /// </summary>
    /// <param Name="id"></param>
    public void GetStranger(string id) {
        ContactsApi.getStranger(id);
    }

    /// <summary>
    /// 添加好友
    /// </summary>
    /// <param Name="friendId"></param>
    /// <param Name="channel"></param>
    public void addFriend(int friendId, String channel) {
        ContactsApi.addFriend(friendId, channel);
    }

    public void deleteFriend(int friendId,String friendNo) {
        ContactsApi.deleteFriend(friendId, friendNo);
    }
    /// <summary>
    /// 调用好友列表api
    /// </summary>
    public void Contacts() {
        long contactTime = TimeServices.getInstance().GetTime(TimestampType.CONTACT,string.Empty);
        ContactsApi.friends(contactTime);
    }

    /// <summary>
    /// 调用好友详细列表api
    /// </summary>
    public void GetContactsDetaile(long id) {
        ContactsApi.getFriend(id.ToStr());
    }


    private ContactsTable Convertors(ContactsTable model, ContactsTableBean bean) {

        //BeanUtils.copyProperties(model, bean);

        //model.id = bean.id;
        model.no = bean.no;
        model.avatarStorageRecordId = bean.avatarStorageRecordId;
        model.nickname = bean.nickname;
        model.alias = bean.alias;
        model.favorite = bean.favorite;
        model.flag = bean.flag;

        model.clientuserId = bean.id.ToStr();
        return model;
    }

    //private VcardsTable Convertors(VcardsTable model, VcardsTableBean bean) {

    //    //BeanUtils.copyProperties(model, bean);
    //    //model.id = bean.id;
    //    model.no = bean.no;
    //    model.nickname = bean.nickname;
    //    model.AvatarStorageRecordId = bean.AvatarStorageRecordId;
    //    model.mobileNumber = bean.mobileNumber;
    //    model.email = bean.email;
    //    model.birthday = bean.birthday;
    //    model.gender = bean.gender;
    //    model.country = bean.country;
    //    model.province = bean.province;
    //    model.city = bean.city;
    //    model.identity = bean.identity;

    //    model.clientuserId = bean.id.ToStr();
    //    return model;
    //}
    private List<ContactsTable> Convertors(List<ContactsTable> modelList, List<ContactsTableBean> beanList) {
        for (int i = 0; i < beanList.Count; i++) {
            ContactsTable model = new ContactsTable();
            //BeanUtils.copyProperties(model, beanList[i]);
            //model.id = beanList[i].id;
            model.no = beanList[i].no;
            model.avatarStorageRecordId = beanList[i].avatarStorageRecordId;
            model.nickname = beanList[i].nickname;
            model.alias = beanList[i].alias;
            model.favorite = beanList[i].favorite;
            model.flag = beanList[i].flag;



            model.clientuserId = beanList[i].id.ToStr();
            modelList.Add(model);
        }
        return modelList;
    }

    /// <summary>
    /// 获取联系人原始名称
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public String getContractOriginalNameByNo(String no) {
        ContactsTable contactsTable = ContactsDao.getInstance().FindContactsByNo(no);
        // 如果是好友
        if (contactsTable != null) {
            if (contactsTable.nickname!=null && !"".Equals(contactsTable.nickname)) {
                return contactsTable.nickname;
            }
            if (contactsTable.name != null && !"".Equals(contactsTable.name)) {
                return contactsTable.name;
            }
        }

        // 如果联系中没有名字信息，则优先查找vcard，如果vcard表中没有，则查询组织表
        VcardsTable vcardsTable = VcardsDao.getInstance().findByNo(no);
        if (vcardsTable != null) {
            return vcardsTable.nickname;
        } else {
            List<OrganizationMemberTable> list = OrganizationMemberDao.getInstance().FindOrganizationMemberByUserNo(no);
            if (list == null || list.Count <= 0) {
                return no;
            } else {
                return list[0].nickname;
            }
        }
    }

    /// <summary>
    /// 获取联系人名称
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public String getContractNameByNo(String no) {

        ContactsTable contactsTable = ContactsDao.getInstance().FindContactsByNo(no);
        // 如果是好友
        if (contactsTable != null) {
            // 如果设置过备注，则优先返回备注
            if (contactsTable.alias == null || "".Equals(contactsTable.alias)) {
                // 在判断nickname
                if (contactsTable.nickname == null || "".Equals(contactsTable.nickname)) {
                    return contactsTable.name;
                } else {
                    return contactsTable.nickname;
                }
            } else {
                return contactsTable.alias;
            }
            return (contactsTable.alias == null || "".Equals(contactsTable.alias)) ? contactsTable.name : contactsTable.alias;
        } else {

            VcardsTable vcardsTable = VcardsDao.getInstance().findByNo(no);
            if (vcardsTable != null) {
                return vcardsTable.nickname;
            } else {
                List<OrganizationMemberTable> list = OrganizationMemberDao.getInstance().FindOrganizationMemberByUserNo(no);
                if (list == null || list.Count <= 0) {
                    return no;
                } else {
                    return list[0].nickname;
                }
            }
        }
    }

    /// <summary>
    /// 获取群成员名称
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public String getMucMemberNameByNo(String mucNo, String operatorUserNo) {
        MucMembersTable mucMembersTable = MucMembersDao.getInstance().findByMucNoAndMemberNo(mucNo, operatorUserNo);
        // 如果有群成员信息
        if (mucMembersTable != null) {
            if (mucMembersTable.nickname == null) {
                return this.getContractNameByNo(operatorUserNo);
            } else {
                return mucMembersTable.nickname;
            }
        } else {
            return this.getContractNameByNo(operatorUserNo);
        }
    }

    /// <summary>
    /// 判断是否为好友
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public Boolean isFriend(long clientUserId) {
        ContactsTable table = ContactsDao.getInstance().FindContactsById(clientUserId);
        if (table == null) {
            return false;
        } else {
            return true;
        }
    }

    /// <summary>
    /// 通过id查找
    /// </summary>
    /// <param Name="clientUserId"></param>
    /// <returns></returns>
    public ContactsTable FindContactsById(long clientUserId) {
        try {
            return ContactsDao.getInstance().FindContactsById(clientUserId);

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
            return null;
        }
    }

    /// <summary>
    /// 通过no查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public ContactsTable FindContactsByNo(String no) {
        try {
            return ContactsDao.getInstance().FindContactsByNo(no);

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
            return null;
        }
    }

    /// <summary>
    /// 查询好友
    /// </summary>
    /// <param Name="conditionCol"></param>
    /// <param Name="conditionVal"></param>
    /// <returns></returns>
    public List<ContactsTable> FindAllFriend(string conditionCol, object conditionVal) {
        try {
            return ContactsDao.getInstance().FindAllFriend(conditionCol, conditionVal);

        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
            return null;
        }
    }
}
}
