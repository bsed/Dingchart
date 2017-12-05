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
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Services {
/// <summary>
/// 群
/// </summary>
public class MucServices : BaseService {


    private static MucServices instance = null;
    public static MucServices getInstance() {
        if (instance == null) {
            instance = new MucServices();
        }
        return instance;
    }


    MucDao mucDao = DataSqlite.MucDao.getInstance();

    private TimestampDao timeDao = new TimestampDao();
    ContactsDao contactsDao = DataSqlite.ContactsDao.getInstance();


    DownloadServices _downloadServices = new DownloadServices();


    Thread _downLoadThread = null;

    Thread _downLoadQlMemberThread = null;

    private List<object> DownLoadGroupMember = new List<object>();

    /**
    * 群详情获取记录，避免同个群多次拉取群详情。
    */
    private static Dictionary<String, String> mapReqMucPool = new Dictionary<String, String>();
    /**
     * 群成员详情获取记录，避免多次拉取群成员详情（据说群成员详情拉取的API，虽然群不同、但是结果是相同的）。
     */
    private static Dictionary<String, String> mapReqMucMemberPool = new Dictionary<String, String>();

    /**
    * 创建群时的头像缓存。
    */
    private static Dictionary<String, string> mapCreateMucAvatarPool = new Dictionary<String, string>();

    /// <summary>
    /// API请求处理
    /// C013: 获取通讯录中群列表 groups OK
    /// C014.1: 获取群详细信息 getGroup OK
    /// C014.2: 获取群成员详细信息 getGroupMember（在VcardsManager中处理业务，这里处理拉取标志）
    /// C014.3: 获取群的二维码 getGroupQRcode
    /// C015: 创建群聊 createGroup OK
    /// C016: 群保存到通讯录 addGroupToAddressList OK
    /// C017: 更新群聊名称 updateGroupName OK
    /// C018: 删除/退出群聊 deleteGroup
    /// C019: 增加群聊成员 addGroupMember OK
    /// C020: 移除群聊成员 deleteGroupMember
    /// C021: 设置群消息免打扰 enableGroupNoDisturb OK
    /// C022: 设置群置顶聊天 setGroupTopmost OK
    /// C023: 设置我在群中的昵称 updateNicknameInGroup
    /// C024: updateBackgroundInGroup 设置群聊天背景
    /// C025: updateGroupChatStatus 更改群的状态 OK
    /// C036: joinGroupByNoAndMember 扫描群成员的二维码加入群聊组
    /// E002:changeAvatar 当前登录人头像变更
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        try {
            switch (eventData.eventDataType) {
            // C013: 获取通讯录中群列表 groups OK
            case EventDataType.groups:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C013(eventData);
                }
                // API请求失败
                else {
                    // 如果是初次登录，则继续请求API
                    if (App.IsFirstLogin) {
                        this.RequestGroups();
                    } else {
                        // 否则标识加载完成，交由DataPullService来处理关键数据同步问题
                        this.FireGroupsLoadOk();
                    }
                }
                this.MarkDataLoadComplete(eventData);
                break;
            // C014.1: 获取群详细信息 getGroup OK
            case EventDataType.getGroup:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C014_1(eventData);
                }
                // API请求失败
                else {
                }
                this.removeMucPoo(eventData);
                break;
            // C014.2: 获取群成员详细信息 getGroupMember（在VcardsManager中处理业务，这里处理拉取标志）
            case EventDataType.getGroupMember:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {

                }
                // API请求失败
                else {

                }
                this.removeMucMemberPoo(eventData);
                break;
            // C015: 创建群聊 createGroup OK
            case EventDataType.createGroup://sssssss
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C015(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C016: 群保存到通讯录 addGroupToAddressList OK
            case EventDataType.addGroupToAddressList:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C016(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C017: 更新群聊名称 updateGroupName OK
            case EventDataType.updateGroupName:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C017(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C018: 删除/退出群聊 deleteGroup
            case EventDataType.deleteGroup:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C018(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C019: 增加群聊成员 addGroupMember OK
            case EventDataType.addGroupMember:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C019(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C020: 移除群聊成员 deleteGroupMember
            case EventDataType.deleteGroupMember:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C020(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C021: 设置群消息免打扰 enableGroupNoDisturb OK
            case EventDataType.enableGroupNoDisturb:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C021(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C022: 设置群置顶聊天 setGroupTopmost OK
            case EventDataType.setGroupTopmost:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C022(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C023: 设置我在群中的昵称 updateNicknameInGroup
            case EventDataType.updateNicknameInGroup:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C023(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C024: updateBackgroundInGroup 设置群聊天背景
            case EventDataType.updateBackgroundInGroup:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C024(eventData);
                }
                // API请求失败
                else {

                }
                break;
            //C025: updateGroupChatStatus 更改群的状态 OK
            case EventDataType.updateGroupChatStatus:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C025(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C014.3: getGroupQRcode 群二维码
            case EventDataType.getGroupQRcode:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C014_3(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // C036: joinGroupByNoAndMember 扫描群成员的二维码加入群聊组
            case EventDataType.joinGroupByNoAndMember:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C036(eventData);
                }
                // API请求失败
                else {

                }
                break;
            // E002:changeAvatar 当前登录人头像变更
            case EventDataType.changeAvatar:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    E002(eventData);
                }
                // API请求失败
                else {

                }
                break;

            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }

    }

    /// <summary>
    /// 处理IM消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {

        // 获取消息类型
        Message message = messageArrivedEvent.message;
        MsgType msgType = message.getType();
        switch (msgType) {
        // 增加群成员
        case MsgType.GroupMemberAdded:
            processGroupMemberAddedMessage(message);
            break;
        // 删除群成员
        case MsgType.GroupMemberDeleted:
            processGroupMemberDeletedMessage(message);
            break;
        // 群成员退群
        case MsgType.GroupMemberExited:
            processGroupMemberExitedMessage(message);
            break;
        // 群名称变更
        case MsgType.GroupNameChanged:
            processGroupNameChangedMessage(message);
            break;
        // 群头像变更
        case MsgType.GroupLogoChanged:
            processGroupLogoChangedMessage(message);
            break;
        // 群成员昵称变更
        case MsgType.GroupMemberNicknameChanged:
            processGroupMemberNicknameChangedMessage(message);
            break;
        // 群成员头像变更
        case MsgType.UserAvatarChanged:
            processUserAvatarChangedMessage(message);
            break;
        // 群昵称变更（登录用户）
        case MsgType.UserNicknameChanged:
            processUserNicknameChangedMessage(message);
            break;

        case MsgType.GroupSavedAsContact:
            processSavedAsContact(message);
            break;


        default:
            break;
        }
    }

    private void processSavedAsContact(Message message) {
        try {
            GroupSavedAsContact messageBean = (GroupSavedAsContact)message;
            if (messageBean.getDevice() == "PC") {
                return;
            }
            String mucNo = messageBean.getGroupNo();
            bool savedAsContact = messageBean.getSavedAsContact();

            MucTable mucTable = MucDao.getInstance().FindGroupByNo(mucNo);

            if (mucTable == null) {
                return;
            }
            mucTable.savedAsContact = savedAsContact;

            MucDao.getInstance().save(mucTable);


            // 发送消息变更通知，刷新聊天历史列表
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucSavedAsContactChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 给自己的其他设备发消息
    /// </summary>
    /// <param name="savedAsContact"></param>
    /// <param name="groupNo"></param>
    public void SendGroupSavedAsContactMessage( bool savedAsContact,string groupNo) {
        try {
            Message message = new GroupSavedAsContact();
            String messageId = ImClientService.getInstance().generateMessageId();
            // 构建消息基础数据
            message.setMessageId(messageId);
            // 构建消息基础数据
            message.setType(MsgType.GroupSavedAsContact);
            //message.setFrom(App.AccountsModel.no);
            message.setTo(App.AccountsModel.no);
            // 构建消息业务数据
            ((GroupSavedAsContact)message).setSavedAsContact(savedAsContact);
            ((GroupSavedAsContact)message).setGroupNo(groupNo);
            ((GroupSavedAsContact)message).setDevice("PC");
            // 发送消息
            MessageService.getInstance().doSend(message);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }
    /// <summary>
    /// 成员加入群消息处理,群成员拉人，或用户扫码加入群都会收到该消息
    /// </summary>
    /// <param Name="message"></param>
    private void processGroupMemberAddedMessage(Message message) {
        try {
            GroupMemberAddedMessage messageBean = (GroupMemberAddedMessage)message;

            // 通知类型
            String channel = messageBean.getChannel();
            String mucid = messageBean.getGroupId();
            String mucNo = messageBean.getGroupNo();
            String operatorUserNo = messageBean.getOperatorUserNo();
            String operatorUserId = messageBean.getOperatorUserId();
            String account = null;
            MucMembersTable operatorUserTable = MucMembersDao.getInstance().findByMucIdAndClientuserId(mucid, operatorUserId);
            if (operatorUserTable == null) {
                return;
            }

            List<GroupMemberDetails> addedMembers = messageBean.getMembers();
            List<String> names = new List<String>();
            List<String> userNos = new List<String>();
            for (int i = 0; i < addedMembers.Count; i++) {
                GroupMemberDetails memberDetail = addedMembers[i];
                try {
                    // 插入member数据库
                    MucMembersTable mucMembersTable = new MucMembersTable();
                    mucMembersTable.mucId = mucid;
                    mucMembersTable.mucno = mucNo;
                    mucMembersTable.no = memberDetail.userNo;
                    mucMembersTable.clientuserId = memberDetail.userId;
                    mucMembersTable.nickname = memberDetail.nickname;
                    mucMembersTable.avatarStorageRecordId = memberDetail.avatarStorageId;
                    MucMembersDao.getInstance().save(mucMembersTable);
                } catch (Exception e) {
                    Log.Error(typeof(MucServices), e);
                }
                userNos.Add(memberDetail.userNo);
                String name = ContactsServices.getInstance().getContractNameByNo(memberDetail.userNo);
                if (name == null) {
                    names.Add(memberDetail.nickname);
                } else {
                    names.Add(name);
                }
            }

            // 扫码加入群
            if (SourceType.qrCode.ToStr().Equals(channel)) {
                // 获取member的No
                String memberNo = "";
                String memberName = "";
                String operatorUserName = "";
                if (addedMembers.Count > 0) {
                    memberNo = addedMembers[0].getUserNo();
                    memberName = ContactsServices.getInstance().getContractNameByNo(memberNo);
                    // 如果未取得名字
                    if (memberName==null) {
                        memberName = addedMembers[0].getNickname();
                    }
                }
                List<MucMembersTable> oldMembers = MucMembersDao.getInstance().findByMucNo(mucNo);
                List<String> displayNames = new List<String>();
                foreach (MucMembersTable mucMembersTable in oldMembers) {
                    if (!operatorUserNo.Equals(mucMembersTable.no)) {
                        String name = ContactsServices.getInstance().getContractNameByNo(mucMembersTable.no);
                        // 如果未取得名字
                        if (name==null) {
                            displayNames.Add(mucMembersTable.nickname);
                        } else {
                            displayNames.Add(name);
                        }
                    } else {
                        operatorUserName = ContactsServices.getInstance().getContractNameByNo(operatorUserNo);
                        if (operatorUserName==null) {
                            operatorUserName = mucMembersTable.nickname;
                        }
                    }

                }
                foreach (GroupMemberDetails memberDetail in addedMembers) {
                    if (!App.AccountsModel.no.Equals(memberDetail.getUserNo())) {
                        String name = ContactsServices.getInstance().getContractNameByNo(memberDetail.getUserNo());
                        // 如果未取得名字
                        if (name==null) {
                            displayNames.Add(memberDetail.getNickname());
                        } else {
                            displayNames.Add(name);
                        }
                    }
                }

                // 扫码人的提示消息：登录人=加入的人 TODO 扫码加群处理 改为在拉取群信息 之后处理
                if (App.AccountsModel.no.Equals(memberNo)) {
//
//                LocalMessageHelper.sendJoinMucMsgByBarcode(mucNo, displayNames);
//                //发出MucChangeEvent事件通知
//                EventBus.getDefault().post(new MucChangeEvent(mucid, mucNo, MucChangeEvent.TYPE_API_JOIN_GROUP_SCAN_BARCODE));
                }
                // 被扫码人的提示消息：登录人=二维码提供者
                else if (App.AccountsModel.no.Equals(operatorUserNo)) {
                    LocalMessageHelper.sendJoinMucMsgByBarcode(mucNo, memberName, "你");
                }
                // 其他人的提示消息
                else {
                    LocalMessageHelper.sendJoinMucMsgByBarcode(mucNo, memberName, operatorUserName);
                }

                // 保存群成员
                //this.saveMucMember(addedMembers[0], mucid);

            }
            // 通讯录方式加入群
            else {

                if (App.AccountsModel.no.Equals(operatorUserNo)) {
                    //我是群主，显示消息。由于群主已经在API返回处理中，将群成员同步到本地表中了，所以此处就不用处理了，只显示消息
                    //你邀请了A、B加入了群聊
                    LocalMessageHelper.sendAddMemberMsg(mucNo, names);
                } else if (!userNos.Contains(App.AccountsModel.no)) {
                    //群里的已有成员，显示消息。同时将新成员加入到 群成员表 中。
                    //xx邀请了A、B加入了群聊

                    //存储群成员
                    foreach (GroupMemberDetails memberDetail in addedMembers) {
                        //本地成员更新
                        //saveMucMember(memberDetail, mucid);
                    }
                    LocalMessageHelper.sendAddMemberMsg(mucNo, operatorUserTable.nickname, names);
                } else {

                    List<MucMembersTable> oldMembers = MucMembersDao.getInstance().findByMucNo(mucNo);
                    List<String> displayNames = new List<String>();
                    foreach (MucMembersTable mucMembersTable in oldMembers) {
                        if (!operatorUserNo.Equals(mucMembersTable.no)) {
                            String name = ContactsServices.getInstance().getContractNameByNo(mucMembersTable.no);
                            if (name==null) {
                                displayNames.Add(mucMembersTable.nickname);
                            } else {
                                displayNames.Add(name);
                            }
                        }
                    }

                    foreach (GroupMemberDetails memberDetail in addedMembers) {
                        if (!App.AccountsModel.no.Equals(memberDetail.getUserNo())) {
                            String name = ContactsServices.getInstance().getContractNameByNo(memberDetail.getUserNo());
                            if (name==null) {
                                displayNames.Add(memberDetail.getNickname());
                            } else {
                                displayNames.Add(name);
                            }
                        }
                    }

                    //被邀请人，显示消息
                    //xx邀请你加入了群聊，群聊参与人还有A、B
                    LocalMessageHelper.sendAddMemberMsgForNewMember(mucNo, operatorUserTable.nickname, displayNames);
                    //TODO:异步拉取群详细
                    //MucManager.getInstance().requestMucDetails(mucNo, null);

                    //TODO 发出MucChangeEvent事件通知
                    //EventBus.getDefault().post(new MucChangeEvent(mucid, mucNo, MucChangeEvent.TYPE_MESSAGE_GROUP_MEMBER_CHANGE));
                    MucTable  mucTable= MucServices.getInstance().FindGroupByNo(mucNo);
                    //发出MucChangeEvent事件通知
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = mucTable;
                    businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }
            }
            foreach (GroupMemberDetails memberDetail in addedMembers) {
                //判断 是否是本人加入群聊，发送通知
                //if (App.AccountsModel.no.Equals(memberDetail.getUserNo())) {
                //发出MucChangeEvent事件通知
                MucEditTextChangedEventData mucEditTextChangedEventData = new MucEditTextChangedEventData();
                mucEditTextChangedEventData.type = "TYPE_ADD";
                mucEditTextChangedEventData.mucid = mucid;
                mucEditTextChangedEventData.mucNo = mucNo;
                mucEditTextChangedEventData.userNo = memberDetail.getUserNo();
                //EventBus.getDefault().post(new MucEditTextChangedEvent(MucEditTextChangedEvent.TYPE_ADD, mucid, mucNo, memberDetail.getUserNo()));
                //}
            }
            if(addedMembers.Count>0) {
                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = mucNo;
                businessEvent.eventDataType = BusinessEventDataType.MucEditTextChangedEvent_TYPE_ADD;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 群主踢人 消息处理、群主把群成员踢出群，会收到该消息    刷新chart
    /// </summary>
    /// <param Name="message"></param>
    private void processGroupMemberDeletedMessage(Message message) {
        try {
            GroupMemberDeletedMessage messageBean = (GroupMemberDeletedMessage)message;

            //获取关键信息
            String mucid = messageBean.getGroupId();
            String mucNo = messageBean.getGroupNo();
            if (mucNo == null || mucNo.Equals("")) {
                mucNo = messageBean.getTo();
            }
            String operatorUserNo = messageBean.getOperatorUserNo();
            String operatorUserId = messageBean.getOperatorUserId();
            String mucName = null;
            String account = null;
            MucTable mucTable = MucDao.getInstance().FindGroupByNo(mucNo);
            if ("NOTICE".Equals(operatorUserNo)) {
                mucName = "管理员";
                List<GroupMemberDetails> deletedMembers = messageBean.getMembers();
                for (int i = 0; i < deletedMembers.Count; i++) {
                    GroupMemberDetails memberDetail = deletedMembers[i];
                    if (App.AccountsModel.no.Equals(memberDetail.getUserNo())) {
                        //登录者本人被踢了，显示消息
                        LocalMessageHelper.sendGroupMemberDeletedMsg(mucNo, mucName);
                        // 从本地通讯录移除
                        try {
                            mucTable.savedAsContact = false;
                            MucDao.getInstance().save(mucTable);
                        } catch (Exception e) {
                            Log.Error(typeof(MucServices), e);
                        }
                        account = memberDetail.getUserNo();
                    }
                    // 删除群成员
                    try {
                        MucMembersDao.getInstance().deleteByClientuserId(mucid, memberDetail.getUserId());
                    } catch (Exception e) {
                        Log.Error(typeof(MucServices), e);
                    }
                    try {
                        // 判断本地是否有被踢人的信息。如果没有则拉取下
                        if (VcardsDao.getInstance().findByClientuserId(memberDetail.getUserId()) == null) {
                            ContactsApi.getStranger(memberDetail.getUserId());
                        }
                    } catch (Exception e) {
                        Log.Error(typeof(MucServices), e);
                    }
                }
            } else {
                MucMembersTable mucMembersTable = MucMembersDao.getInstance().findByMucIdAndClientuserId(mucid, operatorUserId);
                if (mucMembersTable == null) {
                    return;
                }
                mucName = mucMembersTable.nickname;

                // 构建群成员删除消息
                List<GroupMemberDetails> deletedMembers = messageBean.getMembers();
                for (int i = 0; i < deletedMembers.Count; i++) {
                    GroupMemberDetails memberDetail = deletedMembers[i];
                    String name = ContactsServices.getInstance().getContractNameByNo(memberDetail.getUserNo());
                    if (name == null) {
                        name = memberDetail.getNickname();
                    }
                    if (App.AccountsModel.no.Equals(operatorUserNo)) {
                        //发起邀请的人，显示消息
                        LocalMessageHelper.sendMucOwnerDeleteMemberMsg(mucNo, name);
                    } else if (App.AccountsModel.no.Equals(memberDetail.getUserNo())) {
                        //登录者本人被踢了，显示消息
                        LocalMessageHelper.sendGroupMemberDeletedMsg(mucNo, mucName);
                        // 从本地通讯录移除
                        try {
                            mucTable.savedAsContact = false;
                            MucDao.getInstance().save(mucTable);
                        } catch (Exception e) {
                            Log.Error(typeof(MucServices), e);
                        }
                        account = memberDetail.getUserNo();
                    } else {
                        //其它成员被踢了，显示消息
                        LocalMessageHelper.sendGroupMemberDeletedMsg(mucNo, mucName, name);
                    }
                    try {
                        MucMembersDao.getInstance().deleteByClientuserId(mucid, memberDetail.getUserId());

                    } catch (Exception e) {
                        Log.Error(typeof(MucServices), e);
                    }
                    try {
                        // 判断本地是否有被踢人的信息。如果没有则拉取下
                        if (VcardsDao.getInstance().findByClientuserId(memberDetail.getUserId()) == null) {
                            ContactsApi.getStranger(memberDetail.getUserId());
                        }
                    } catch (Exception e) {
                        Log.Error(typeof(MucServices), e);
                    }
                }
            }
            //发出MucChangeEvent事件通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE;
            EventBusHelper.getInstance().fireEvent(businessEvent);

            //判断是否是本人被踢，发送通知
            if (account != null) {

                // 从本地通讯录移除
                MucDao.getInstance().deleteByNo(mucTable.no);


                //// 发出EVENT通知
                //MucEditTextChangedEventData mucEditTextChangedEventData = new MucEditTextChangedEventData();
                //mucEditTextChangedEventData.type = "TYPE_DELETE";
                //mucEditTextChangedEventData.mucid = mucid;
                //mucEditTextChangedEventData.mucNo = mucNo;
                //mucEditTextChangedEventData.userNo = App.AccountsModel.no;

                //BusinessEvent<Object> businessEvent1 = new BusinessEvent<Object>();
                //businessEvent1.data = mucNo;
                //businessEvent1.eventDataType = BusinessEventDataType.MucEditTextChangedEvent_TYPE_ADD;
                //EventBusHelper.getInstance().fireEvent(businessEvent1);

                //// 发出第二个通知
                //BusinessEvent<Object> businessEvent2 = new BusinessEvent<Object>();
                //businessEvent2.data = mucTable;
                //businessEvent2.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE;
                //EventBusHelper.getInstance().fireEvent(businessEvent2);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 用户主动退群 消息处理、用户退出群聊后，会收到该消息
    /// </summary>
    /// <param Name="message"></param>
    private void processGroupMemberExitedMessage(Message message) {
        try {
            GroupMemberExitedMessage messageBean = (GroupMemberExitedMessage)message;

            String mucid = messageBean.getGroupId();
            String mucNo = messageBean.getGroupNo();

            String operatorUserNo = messageBean.getOperatorUserNo();
            String operatorUserId = messageBean.getOperatorUserId();

            MucTable mucTable = MucDao.getInstance().FindGroupByNo(mucNo);

            //我自己退出了群
            if(operatorUserNo==App.AccountsModel.no) {
                //删除群基本信息
                MucDao.getInstance().deleteByNo(mucNo);
                //删除群成员信息
                MucMembersDao.getInstance().deleteByMucNo(mucNo);

                MessageDao.getInstance().deleteByUser(mucNo);

                ChatSessionService.getInstance().deleteChatSessionByNo(mucNo);

                //发出MucChangeEvent事件通知
                BusinessEvent<Object> delGroupEvent = new BusinessEvent<Object>();
                delGroupEvent.data = mucNo;
                delGroupEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_DELETE_GROUP;
                EventBusHelper.getInstance().fireEvent(delGroupEvent);
                return;
            }
            MucMembersTable member = MucMembersDao.getInstance().findByMucIdAndClientuserId(mucid, operatorUserId);
            if (null != member) {
                try {
                    MucMembersDao.getInstance().deleteByClientuserId(mucid, operatorUserId);
                } catch (Exception e) {
                    Log.Error(typeof(MucServices), e);
                }
            }
            try {
                // 判断本地是否有信息。如果没有则拉取下
                if (VcardsDao.getInstance().findByClientuserId(member.clientuserId) == null) {
                    ContactsApi.getStranger(member.clientuserId);
                }
            } catch (Exception e) {
                Log.Error(typeof(MucServices), e);
            }
            //发出MucChangeEvent事件通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE;
            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 群名称变更 消息处理   通讯录  chartsession  chartsession头部显示
    /// 群成员把群名称改变后，会收到该消息。改名的成员也会收到吗？？
    /// </summary>
    /// <param Name="message"></param>
    private void processGroupNameChangedMessage(Message message) {
        try {
            GroupNameChangedMessage messageBean = (GroupNameChangedMessage)message;
            String mucid = messageBean.getGroupId().ToStr();
            String mucNo = messageBean.getGroupNo();
            String name = messageBean.getGroupName();
            String operatorUserNo = messageBean.getOperatorUserNo();
            String operatorUserId = messageBean.getOperatorUserId().ToStr();

            MucTable mucTable = MucDao.getInstance().FindGroupByNo(mucNo);

            if (mucTable == null) {
                return;
            }
            mucTable.name = name;
            try {
                MucDao.getInstance().save(mucTable);
            } catch (Exception e) {
                Log.Error(typeof(MucServices), e);
            }
            // 发送群名称变更通知消息到群聊中
            String strOperatorUserName;
            if (App.AccountsModel.no.Equals(messageBean.getOperatorUserNo())) {
                // 自己变更的群名
                strOperatorUserName = "你";
            } else {
                // 其他群组成员变更的群名
                strOperatorUserName = ContactsServices.getInstance().getMucMemberNameByNo(mucNo, messageBean.getOperatorUserNo());
            }

            LocalMessageHelper.sendGroupNameChangedMsg(mucNo, strOperatorUserName, name);

            // 发送消息变更通知，刷新聊天历史列表
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 群头像变更 消息处理
    /// 群成员发生变化后，群头像会发生变化，后台会发出该消息。
    /// </summary>
    /// <param Name="message"></param>
    private void processGroupLogoChangedMessage(Message message) {
        try {
            // 延迟1秒运行，偶尔会出现发起群聊处理比消息慢的情况。
            Thread.Sleep(4000);
            GroupLogoChangedMessage messageBean = (GroupLogoChangedMessage)message;
            String mucNo = messageBean.getGroupNo();
            string logo = messageBean.getAvatarId();
            // TODO 把变更记录到缓存、有时候头像变更消息比群创建时茶如数据库还快！
            //mapMucLogoChangePool.put(mucNo, ToolsHelper.toString(logo));
            DownloadServices.getInstance().DownloadMethod(logo, DownloadType.SYSTEM_IMAGE, null);
            MucTable mucTable = MucDao.getInstance().FindGroupByNo(mucNo);
            if (mucTable == null) {
                // 如果延迟了，创建群处理还未完成，则缓存下头像
                mapCreateMucAvatarPool[mucNo]=logo;
                return;
            }
            if (string.IsNullOrEmpty(logo)) {
                mucTable.avatarStorageRecordId = "";
            } else {
                mucTable.avatarStorageRecordId = logo.ToStr();
            }
            try {
                MucDao.getInstance().save(mucTable);
            } catch (Exception e) {
                Log.Error(typeof(MucServices), e);
            }



            // 发送消息变更通知，刷新聊天历史列表
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 群成员昵称变更 消息处理
    /// 用户可以修改自己在某个群里的昵称，其它群成员会收到消息。
    /// </summary>
    /// <param Name="message"></param>
    private void processGroupMemberNicknameChangedMessage(Message message) {
        try {
            GroupMemberNicknameChangedMessage messageBean = (GroupMemberNicknameChangedMessage)message;
            String mucid = messageBean.getGroupId().ToStr();
            String mucNo = messageBean.getGroupNo();
            String memberId = messageBean.getUserId().ToStr();
            MucMembersTable mucMembersTable = MucMembersDao.getInstance().findByMucIdAndClientuserId(mucid, memberId);
            if (mucMembersTable == null) {
                return;
            }
            String memberNo = mucMembersTable.no;


            mucMembersTable.nickname = messageBean.getNickname();
            MucMembersDao.getInstance().save(mucMembersTable);

            // 缓存处理 sta
            //CacheManager.getInstance().getGroupMember(mucid, memberId).setName(messageBean.getNickname());
            // 缓存处理 end

            // 判断是否需要发Event，解决在聊天窗口信息显示不一致的情况
            if (ContactsServices.getInstance().isFriend(long.Parse(memberId))) {
                // 如果是好友，则如果设置了备注，就需要发送Event
                ContactsTable contactsTable = ContactsDao.getInstance().FindContactsById(long.Parse(memberId));
                if (contactsTable != null) {
                    if (contactsTable.alias == null) {
                        // 发送消息变更通知，刷新聊天历史列表
                        GroupMemberNicknameChangedEventData businessData = new GroupMemberNicknameChangedEventData();
                        businessData.mucId = mucid;
                        businessData.mucNo = mucNo;
                        businessData.memberId = memberId;
                        businessData.meberNo = memberNo;
                        businessData.changeName = messageBean.getNickname();
                        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                        businessEvent.data = businessData;
                        businessEvent.eventDataType = BusinessEventDataType.GroupMemberNicknameChangedEvent;
                        EventBusHelper.getInstance().fireEvent(businessEvent);
                    }
                }
            } else {

                GroupMemberNicknameChangedEventData businessData = new GroupMemberNicknameChangedEventData();
                businessData.mucId = mucid;
                businessData.mucNo = mucNo;
                businessData.memberId = memberId;
                businessData.meberNo = memberNo;
                businessData.changeName = messageBean.getNickname();
                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = businessData;
                businessEvent.eventDataType = BusinessEventDataType.GroupMemberNicknameChangedEvent;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 好友头像变更通知消息
    /// </summary>
    /// <param Name="message"></param>
    private void processUserAvatarChangedMessage(Message message) {
        try {
            UserAvatarChangedMessage userAvatarChangedMessage = (UserAvatarChangedMessage)message;
            // 更新群成员
            MucServices.getInstance().saveMucMemberAvatarByTable(userAvatarChangedMessage.getUserNo(), userAvatarChangedMessage.getAvatarStorageId().ToStr());

        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
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
            String memberNo = contactsTable.no;
            String strOldNickName = contactsTable.name;
            String strNewNickName = userNicknameChangedMessage.getNickname();

            // 修改群成员的昵称
            this.saveByUserNicknameChangedMessage(memberNo, strOldNickName, strNewNickName);


        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }

    }

    /// <summary>
    /// 触发数据拉取完成事件
    /// </summary>
    private void FireGroupsLoadOk() {
        try {
            App.GroupsLoadOk = true;
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C013: 获取通讯录中群列表 groups OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C013(EventData<Object> eventData) {
        try {
            Object contacts = eventData.data;
            List<MucTableBean> listContacts = JsonConvert.DeserializeObject<List<MucTableBean>>(
                                                  contacts.ToStr(), new convertor<MucTableBean>());

            if (listContacts.Count == 0) {
                this.FireGroupsLoadOk();
                return;
            }

            App.GroupDatatable = listContacts;

            //下群头像
            //_downLoadThread = new Thread(new ParameterizedThreadStart(DownLoad));
            //_downLoadThread.Start(ContactsTpye.QL);

            //插入群的信息 muc表
            List<MucTable> modelList = new List<MucTable>();
            modelList = Convertors(modelList, listContacts);
            int a = mucDao.InsertGroup(modelList);

            TimeServices.getInstance().SaveTime(TimestampType.MUC, eventData.timestamp,string.Empty);

            for (int i = 0; i < listContacts.Count; i++) {
                MucTableBean bean = listContacts[i];
                // 缓存下载头像
                DownloadBean downloadBean = new DownloadBean(bean.avatarStorageRecordId, DownloadType.SYSTEM_IMAGE, null);
                DownloadServices.getInstance().DownloadCatchAdd(downloadBean);

                this.RequestGroupDetail(bean.no, null);

                //执行完事之后才能进入主界面
                if (i == listContacts.Count - 1) {
                    this.FireGroupsLoadOk();
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C014.1: 获取群详细信息 getGroup OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C014_1(EventData<Object> eventData) {
        try {



            Object contacts = eventData.data;
            MucTableBean mucTableBean = JsonConvert.DeserializeObject<MucTableBean>(contacts.ToStr());

            if (mucTableBean == null) return;

            if (mucTableBean.deleteFlag == true) {
                return;
            }
            IList<MucMembersTableBean> member = mucTableBean.members;


            List<MucMembersTable> modelList = new List<MucMembersTable>();
            modelList = Convertors(modelList, member);



            //插入muc表部分 当时没有查询到的数据
            MucTable mucTable = mucDao.FindGroupByNo(mucTableBean.no);
            if (mucTable==null) {
                mucTable = new MucTable();
                mucTable.mucId = mucTableBean.id.ToStr();
                mucTable.no = mucTableBean.no;
                mucTable.activeFlag = mucTableBean.activeFlag;
            }
            mucTable.name = mucTableBean.name;
            mucTable.avatarStorageRecordId = mucTableBean.avatarStorageRecordId;
            mucTable.count = member.Count;
            mucTable.savedAsContact = mucTableBean.savedAsContact;
            mucTable.enableNoDisturb = mucTableBean.enableNoDisturb;

            mucTable.isTopmost = mucTableBean.isTopmost;


            mucTable.manager = mucTableBean.manager;
            // 判断是否为群主
            if (mucTableBean.manager.Equals(App.AccountsModel.clientuserId)) {
                mucTable.isOwner = true;
            } else {
                mucTable.isOwner = false;
            }
            mucDao.save(mucTable);

            //MucMembersTable插入人
            MucMembersDao.getInstance().InsertGroupMember(modelList, mucTableBean.no, mucTableBean.id.ToStr());

            // 循环拉取群成员详情
            for (int i = 0; i < member.Count; i++) {
                MucMembersTableBean bean = member[i];
                // 获取群成员详情
                ContactsApi.getGroupMember(mucTableBean.id.ToStr(), bean.id.ToStr());
                // 缓存下载群成员头像
                //DownloadServices.getInstance().DownloadMethod(bean.AvatarStorageRecordId, DownloadType.SYSTEM, null);
                DownloadBean downloadBean = new DownloadBean(bean.avatarStorageRecordId, DownloadType.SYSTEM_IMAGE, null);
                DownloadServices.getInstance().DownloadCatchAdd(downloadBean);
            }

            // 获取API参数中的messageId，判断是否因第一次收到群消息而触发的API拉取
            Dictionary<String, Object> extras = eventData.extras;
            String messageId = null;
            if (null != extras && extras.ContainsKey("messageId")) {
                messageId = extras["messageId"].ToStr();
                MessageService.getInstance().processMessageAfterMucAvaliable(mucTable.no, messageId, mucTable.manager);
            }

            // 下载群的二维码
            this.RequestGroupQRcode(mucTable.mucId);
            SettingService.getInstance().setTop(mucTableBean.no, mucTable.isTopmost);
            SettingService.getInstance().setQuiet(mucTableBean.no, mucTable.enableNoDisturb);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /**
     * 移除拉取标志
     *
     * @param httpResult
     */
    private void removeMucPoo(EventData<Object> eventData) {
        try {
            // 移除拉取标志
            if (eventData.extras != null && eventData.extras.ContainsKey("mucNo")) {
                String mucNo = eventData.extras["mucNo"].ToStr();
                mapReqMucPool.Remove(mucNo);
                //Console.WriteLine("拉取控制：释放群拉取：" + mucNo);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }
    /// <summary>
    /// C014.2: 获取群成员详细信息 getGroupMember（在VcardsManager中处理业务，这里处理拉取标志）
    /// </summary>
    /// <param Name="extras"></param>
    private void removeMucMemberPoo(EventData<Object> eventData) {
        try {
            // 移除拉取标志
            if (eventData.extras != null && eventData.extras.ContainsKey("clientuserid")) {
                String clientuserid = eventData.extras["clientuserid"].ToStr();
                mapReqMucMemberPool.Remove(clientuserid);
                //Console.WriteLine("拉取控制：释放群成员拉取：" + clientuserid);
            }

        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C015: 创建群聊 createGroup OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C015(EventData<Object> eventData) {
        try {

            MucTableBean mucTableBean = JsonConvert.DeserializeObject<MucTableBean>(eventData.data.ToStr());
            if (mucTableBean != null) {
                String mucid = mucTableBean.id.ToStr();
                String mucNo = mucTableBean.no;
                String name = mucTableBean.name;

                String avatar = mucTableBean.avatarStorageRecordId;
                //如果创建群的时候没有头像、则尝试从缓存中获取头像。
                if (string.IsNullOrEmpty(avatar)) {
                    if (mapCreateMucAvatarPool.ContainsKey(mucNo)) {
                        avatar = mapCreateMucAvatarPool[mucNo].ToStr();
                    }
                }
                Boolean activeFlag = mucTableBean.activeFlag;
                String manager = string.Empty;

                manager = mucTableBean.manager;
                Boolean isTop = mucTableBean.isTopmost;
                Boolean enableNoDisturb = mucTableBean.enableNoDisturb;
                Boolean savedAsContact = mucTableBean.savedAsContact;

                // 判断自己是否是群主
                Boolean isOwner = manager.Equals(App.AccountsModel.clientuserId);

                //群成员处理
                List<MucMembersTable> mucMembersTables = new List<MucMembersTable>();

                List<MucMembersTableBean> members = (List<MucMembersTableBean>)mucTableBean.members;
                for (int i = 0; i < members.Count; i++) {
                    MucMembersTableBean member = members[i];

                    MucMembersTable mucMembersTable = new MucMembersTable();
                    mucMembersTable.mucId = mucid;
                    mucMembersTable.no = member.no;
                    mucMembersTable.clientuserId = member.id.ToStr();
                    mucMembersTable.nickname = member.nickname;
                    mucMembersTable.avatarStorageRecordId = member.avatarStorageRecordId;

                    mucMembersTables.Add(mucMembersTable);
                }

                //暴力删除群成员表并重新插入
                MucMembersDao.getInstance().InsertGroupMember(mucMembersTables, mucNo, mucid);

                // 查询本地，看是否存在群
                MucTable table = MucDao.getInstance().FindGroupByNo(mucNo);
                if (null == table) {
                    table = new MucTable();
                    table.mucId = mucid;
                    table.no = mucNo;
                    table.count = members.Count;
                    table.name = name; ;
                    table.avatarStorageRecordId = avatar;
                    table.activeFlag = activeFlag;
                    table.savedAsContact = savedAsContact;
                    if (string.IsNullOrEmpty(mucTableBean.manager)) {
                        manager = App.AccountsModel.clientuserId;
                    }
                    table.manager = manager;
                    table.isOwner = isOwner;
                } else {
                    table.name = name; ;
                    table.avatarStorageRecordId = avatar;
                    table.activeFlag = activeFlag;
                    table.savedAsContact = savedAsContact;
                    table.isOwner = isOwner;
                    table.manager = manager;
                }
                //更新群
                MucDao.getInstance().save(table);

                //TODO:群置顶、免打扰设置
                //SettingManager.getInstance().setTop(no, isTop);
                //SettingManager.getInstance().setQuiet(no, enableNoDisturb);
                LocalMessageHelper.sendCreateMucMsg(eventData.data.ToStr());
                //创建群处理完成，发出MucChangeEvent事件通知   11.14删除
                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = table;
                businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_CREATE_GROUP;
                EventBusHelper.getInstance().fireEvent(businessEvent);

                // 下载群的二维码
                this.RequestGroupQRcode(mucid);

            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C016: 群保存到通讯录 addGroupToAddressList OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C016(EventData<Object> eventData) {
        try {
            String mucid = eventData.extras["mucid"].ToStr();
            Boolean savedAsContact = eventData.extras["savedAsContact"].ToString().ToBool();

            // 查询本地，看是否存在群
            MucTable table = MucDao.getInstance().FindGroupById(mucid);
            if (table!=null) {
                table.savedAsContact = savedAsContact;
                //更新群
                MucDao.getInstance().save(table);
            }

        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C017: 更新群聊名称 updateGroupName OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C017(EventData<Object> eventData) {
        try {
            String mucId = eventData.extras["mucid"].ToStr();
            String name = eventData.extras["name"].ToStr();

            // 查询本地，看是否存在群
            MucTable table = MucDao.getInstance().FindGroupById(mucId);
            if (table!=null) {
                table.name = name;
                //更新群
                MucDao.getInstance().save(table);
                //发出MucChangeEvent事件通知
                BusinessEvent<object> businessEvent = new BusinessEvent<object>();
                businessEvent.data = table;
                businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }

    }

    /// <summary>
    /// C018: 删除/退出群聊 deleteGroup
    /// </summary>
    /// <param Name="extras"></param>
    private void C018(EventData<Object> eventData) {
        try {
            String mucid = eventData.extras["mucid"].ToStr();
            String mucNo = eventData.extras["mucNo"].ToStr();

            //删除群基本信息
            MucDao.getInstance().deleteByNo(mucNo);
            //删除群成员信息
            MucMembersDao.getInstance().deleteByMucNo(mucNo);

            //发出MucChangeEvent事件通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucNo;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_DELETE_GROUP;
            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C019: 增加群聊成员 addGroupMember OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C019(EventData<Object> eventData) {
        try {

            String mucid = eventData.extras["mucid"].ToStr();

            //string[] ss = eventData.extras["members"].ToStr().Split('|');
            List<String> memberList = new List<System.String>(eventData.extras["members"].ToStr().Split('|'));
            //List<String> memberList = (List<String>)eventData.extras["members"];
            String channel = eventData.extras["channel"].ToStr();

            // 查询本地，看是否存在群
            MucTable mucTable = MucDao.getInstance().FindGroupById(mucid);
            List<String> displayNames = new List<String>();
            mucTable.count = mucTable.count + memberList.Count;
            MucDao.getInstance().save(mucTable);
            //存储群成员
            for (int i = 0; i < memberList.Count; i++) {
                String memberId = memberList[i];
                VcardsTable vcardsTable = VcardsDao.getInstance().findByClientuserId(memberId);
                if (null != vcardsTable) {
                    String name = ContactsServices.getInstance().getContractNameByNo(vcardsTable.no);
                    if (name == null) {
                        displayNames.Add(vcardsTable.nickname);
                    } else {
                        displayNames.Add(name);
                    }

                    MucMembersTable mucMembersTable = new MucMembersTable();
                    mucMembersTable.mucId = mucid;
                    mucMembersTable.mucno = mucTable.no;

                    mucMembersTable.no = vcardsTable.no;
                    mucMembersTable.clientuserId = vcardsTable.clientuserId;
                    mucMembersTable.nickname = vcardsTable.nickname;
                    mucMembersTable.avatarStorageRecordId = vcardsTable.avatarStorageRecordId;
                    MucMembersDao.getInstance().save(mucMembersTable);
                } else {
                    List<OrganizationMemberTable> orgList = OrganizationMemberDao.getInstance().FindOrganizationMemberByUserId(memberId.ToInt());
                    if (orgList == null) return;
                    String name = ContactsServices.getInstance().getContractNameByNo(orgList[0].no);
                    if (name == null) {
                        displayNames.Add(orgList[0].nickname);
                    } else {
                        displayNames.Add(name);
                    }

                    MucMembersTable mucMembersTable = new MucMembersTable();
                    mucMembersTable.mucId = mucid;
                    mucMembersTable.mucno = mucTable.no;

                    mucMembersTable.no = orgList[0].no;
                    mucMembersTable.clientuserId = orgList[0].userId;
                    mucMembersTable.nickname = orgList[0].nickname;
                    mucMembersTable.avatarStorageRecordId = orgList[0].avatarId;
                    MucMembersDao.getInstance().save(mucMembersTable);
                }
            }

            //如果群未激活，构造提示消息; 如果已激活，靠收到的消息构造提示消息。
            if (!mucTable.activeFlag) {
                LocalMessageHelper.sendAddMemberMsg(mucTable.no, displayNames);
            }

            //增加群聊成员处理完成，发出MucChangeEvent事件通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C020: 移除群聊成员 deleteGroupMember
    /// </summary>
    /// <param Name="extras"></param>
    private void C020(EventData<Object> eventData) {
        try {
            String mucid = eventData.extras["mucid"].ToStr();
            //String mucNo = eventData.extras["mucNo"].ToStr();
            List<String> memberList = new List<System.String>(eventData.extras["members"].ToStr().Split('|'));

            // 查询本地，看是否存在群
            MucTable mucTable = MucDao.getInstance().FindGroupById(mucid);
            mucTable.count = mucTable.count - memberList.Count;
            MucDao.getInstance().save(mucTable);
            //删除群成员
            for (int i = 0; i < memberList.Count; i++) {
                try {
                    String memberId = memberList[i];
                    MucMembersTable mucMembersTable = MucMembersDao.getInstance().findByMucIdAndClientuserId(mucid, memberId);
                    //如果群未激活，构造提示消息; 如果已激活，靠收到的消息构造提示消息。
                    if (!mucTable.activeFlag) {
                        String name = ContactsServices.getInstance().getContractNameByNo(mucMembersTable.no);
                        if (name == null) {
                            LocalMessageHelper.sendMucOwnerDeleteMemberMsg(mucTable.no, mucMembersTable.nickname);
                        } else {
                            LocalMessageHelper.sendMucOwnerDeleteMemberMsg(mucTable.no, name);
                        }
                    }
                    MucMembersDao.getInstance().deleteByClientuserId(mucid, memberId);
                } catch (Exception e) {
                    Log.Error(typeof(MucServices), e);
                }
            }
            //移除群聊成员处理完成，发出MucChangeEvent事件通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = mucTable;
            businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C021: 设置群消息免打扰 enableGroupNoDisturb OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C021(EventData<Object> eventData) {
        try {
            String mucid = eventData.extras["mucid"].ToStr();
            //String mucNo = eventData.extras["mucNo"].ToStr();
            Boolean quiet = eventData.extras["Quiet"].ToStr().ToBool();

            // 查询本地，看是否存在群
            MucTable table = MucDao.getInstance().FindGroupById(mucid);
            if (table != null) {
                table.enableNoDisturb = quiet;
                //更新群
                MucDao.getInstance().save(table);

                // 更新设置表
                SettingService.getInstance().setQuiet(table.no, quiet);
            }


        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C022: 设置群置顶聊天 setGroupTopmost OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C022(EventData<Object> eventData) {
        try {
            String mucid = eventData.extras["mucid"].ToStr();
            //String mucNo = eventData.extras["mucNo"].ToStr();
            Boolean topmost = eventData.extras["topmost"].ToStr().ToBool();

            // 查询本地，看是否存在群
            MucTable table = MucDao.getInstance().FindGroupById(mucid);
            if (table != null) {
                table.isTopmost = topmost;
                //更新群
                MucDao.getInstance().save(table);

                // 更新设置表
                SettingService.getInstance().setTop(table.no, topmost);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C023: 设置我在群中的昵称 updateNicknameInGroup
    /// </summary>
    /// <param Name="extras"></param>
    private void C023(EventData<Object> eventData) {
        MucMembersTable mucMembersTable = null;
        String nickName = string.Empty;
        try {

            String mucid = eventData.extras["mucid"].ToStr();
            //String mucNo = eventData.extras["mucNo"].ToStr();
            nickName = eventData.extras["nickName"].ToStr();
            mucMembersTable = MucMembersDao.getInstance().findByMucIdAndClientuserId(mucid, App.AccountsModel.clientuserId);

            if (mucMembersTable != null) {
                mucMembersTable.nickname = nickName;
                MucMembersDao.getInstance().save(mucMembersTable);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = mucMembersTable;
        Businessdata.eventDataType = BusinessEventDataType.GroupMemberNicknameChangedEvent_TYPE_UPDATE_ME;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

    /// <summary>
    /// C024: updateBackgroundInGroup 设置群聊天背景
    /// </summary>
    /// <param Name="extras"></param>
    private void C024(EventData<Object> eventData) {
        try {
            //TODO:现在没这个业务
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C025: updateGroupChatStatus 更改群的状态 OK
    /// </summary>
    /// <param Name="extras"></param>
    private void C025(EventData<Object> eventData) {
        try {
            String mucid = eventData.extras["mucid"].ToStr();
            //String mucNo = eventData.extras["mucNo"].ToStr();

            // 查询本地，看是否存在群
            MucTable mucTable = MucDao.getInstance().FindGroupById(mucid);
            mucTable.activeFlag = true;
            MucDao.getInstance().save(mucTable);

            // 群激活后发送消息
            MessageService.getInstance().sendMucMessageAfterActive(mucTable.no);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// C014.3: getGroupQRcode 群二维码
    /// </summary>
    /// <param Name="extras"></param>
    private void C014_3(EventData<Object> eventData) {
        try {
            JObject json = JObject.Parse((String)eventData.data);

            String qrcodeId = json.GetValue("qrcodeId").ToStr();
            String groupId = eventData.extras["groupId"].ToStr();
            //下载群二维码
            Dictionary<String, Object> extras = new Dictionary<string, object>();
            extras.Add("qrcodeId", qrcodeId);
            extras.Add("groupId", groupId);

            // 保存二维码
            MucTable mucTable =  MucDao.getInstance().FindGroupById(groupId);
            if (mucTable!=null) {
                mucTable.qrcodeId = qrcodeId;
                MucDao.getInstance().save(mucTable);
            }

            // 下载群二维码
            DownloadServices.getInstance().DownloadQrcodeMethod(qrcodeId, DownloadType.SYSTEM_BARCODE, extras);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }



    /// <summary>
    /// C036: joinGroupByNoAndMember 扫描群成员的二维码加入群聊组
    /// </summary>
    /// <param Name="extras"></param>
    private void C036(EventData<Object> eventData) {
        try {
            //创建群
            this.C015(eventData);
            MucTableBean mucTableBean = JsonConvert.DeserializeObject<MucTableBean>(eventData.data.ToStr());
            if (mucTableBean != null) {
                String mucid = mucTableBean.id.ToStr();
                String mucNo = mucTableBean.no;

                List<String> displayNames = new List<String>();

                List<MucMembersTable> oldMembers = MucMembersDao.getInstance().findByMucNo(mucNo);
                for (int i = 0; i < oldMembers.Count; i++) {
                    MucMembersTable mucMembersTable = oldMembers[i];
                    //过滤自己
                    if (App.AccountsModel.no.Equals(mucMembersTable.no)) {
                        break;
                    }

                    String name = ContactsServices.getInstance().getContractNameByNo(mucMembersTable.no);
                    // 如果未取得名字
                    if (name == null) {
                        displayNames.Add(mucMembersTable.nickname);
                    } else {
                        displayNames.Add(name);
                    }
                }

                LocalMessageHelper.sendJoinMucMsgByBarcode(mucNo, displayNames);

                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                //businessEvent.data = mucTable;
                businessEvent.eventDataType = BusinessEventDataType.MucChangeEvent_TYPE_API_JOIN_GROUP_SCAN_BARCODE;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// E002:changeAvatar 当前登录人头像变更
    /// </summary>
    /// <param Name="extras"></param>
    private void E002(EventData<Object> eventData) {
        try {
            String avatarStorageId = eventData.extras["avatarStorageId"].ToStr();
            // 本人所在群的头像
            MucMembersDao.getInstance().saveMucMemberAvatarByNo(App.AccountsModel.no, avatarStorageId);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }





    private List<MucMembersTable> Convertors(List<MucMembersTable> modelList, IList<MucMembersTableBean> beanList) {
        for (int i = 0; i < beanList.Count; i++) {
            MucMembersTable model = new MucMembersTable();

            //BeanUtils.copyProperties(model, beanList[i]);
            //model.id = beanList[i].id;
            model.no = beanList[i].no;
            model.nickname = beanList[i].nickname;
            model.avatarStorageRecordId = beanList[i].avatarStorageRecordId;

            model.clientuserId = beanList[i].id.ToStr();
            modelList.Add(model);
        }
        return modelList;
    }

    private List<MucTable> Convertors(List<MucTable> modelList, List<MucTableBean> beanList) {
        for (int i = 0; i < beanList.Count; i++) {
            MucTable model = new MucTable();
            //  BeanUtils.copyProperties(model, beanList[i]);

            // model.id = beanList[i].id;
            model.no = beanList[i].no;
            model.name = beanList[i].name;
            model.avatarStorageRecordId = beanList[i].avatarStorageRecordId;
            model.manager = beanList[i].manager;
            model.activeFlag = beanList[i].activeFlag;
            model.savedAsContact = beanList[i].savedAsContact;
            model.isTopmost = beanList[i].isTopmost;
            model.enableNoDisturb = beanList[i].enableNoDisturb;
            model.deleteFlag = beanList[i].deleteFlag;
            model.count = beanList[i].count;

            model.mucId = beanList[i].id.ToStr();
            modelList.Add(model);
        }
        return modelList;
    }


    /// <summary>
    /// 拉取群二维码
    /// </summary>
    public void RequestGroupQRcode(String groupId) {
        ContactsApi.getGroupQRcode(groupId);
    }

    /// <summary>
    /// 调用群列表api
    /// </summary>
    public void RequestGroups() {
        long mucTime = TimeServices.getInstance().GetTime(TimestampType.MUC,string.Empty);
        ContactsApi.groups(mucTime);
    }

    /// <summary>
    /// 调用群详细列表api
    /// </summary>
    public void RequestGroupDetail(string no, Dictionary<String, Object> extras) {
        if (!mapReqMucPool.ContainsKey(no)) {
            mapReqMucPool.Add(no, no);
            ContactsApi.getGroup(no, extras);
        } else {
            // Console.WriteLine("拉取控制：避免重复拉取群：" + no);
        }


    }

    /// <summary>
    /// 修改群状态
    /// </summary>
    /// <param Name="mucId"></param>
    public void RequestChangeMucStatus(String mucId) {
        ContactsApi.updateGroupChatStatus(mucId);
    }

    /// <summary>
    /// 拉取群成员详情,返回结果在VcardsManager中处理
    /// </summary>
    /// <param Name="mucid"></param>
    /// <param Name="clientUserId"></param>
    public void requestMucMember(String mucid, String clientuserid) {
        if (!mapReqMucMemberPool.ContainsKey(clientuserid)) {
            mapReqMucMemberPool.Add(clientuserid, clientuserid);
            ContactsApi.getGroupMember(mucid, clientuserid);
        } else {
            //Console.WriteLine("拉取控制：避免重复拉取群：" + clientuserid);
        }
    }


    /// <summary>
    /// 保存群成员列表 头像
    /// </summary>
    public void SaveGroupMemberToVsCard() {
        MucDao mucDao = DataSqlite.MucDao.getInstance();
        List<MucMembersTable> dt = mucDao.FindDistictGroupMember();
        if (dt == null || dt.Count == 0) return;
        for (int i = 0; i < dt.Count; i++) {
            if (dt[i].avatarStorageRecordId.ToStr() == string.Empty) {
                continue;
            }
            if (DownLoadGroupMember.Contains(dt[i].avatarStorageRecordId) == false) {
                DownLoadGroupMember.Add(dt[i].avatarStorageRecordId);
            }
        }
        //DownLoadGroupMember.AddRange(dt.DtToList("Avatar"));//把集合A.B合并
        //DownLoadGroupMember = DownLoadGroupMember.Union(dt.DtToList("Avatar")).ToList<object>();          //剔除重复项


    }

    /// <summary>
    /// 保存群成员
    /// </summary>
    /// <param Name="memberDetail"></param>
    /// <param Name="mucid"></param>
    private void saveMucMember(GroupMemberDetails memberDetail, String mucid) {
        try {
            MucMembersTable mucMembersTable = new MucMembersTable();
            mucMembersTable.mucId = mucid;
            mucMembersTable.no = memberDetail.getUserNo();
            mucMembersTable.clientuserId = memberDetail.getUserId();
            mucMembersTable.nickname = memberDetail.getNickname();
            mucMembersTable.avatarStorageRecordId = memberDetail.getAvatarStorageId();
            // 保存成员
            MucMembersDao.getInstance().save(mucMembersTable);
            // 拉取成员详情
            this.requestMucMember(mucMembersTable.mucId, mucMembersTable.clientuserId);
            //this.saveMucMemberByTable(mucMembersTable, true);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 更新群成员头像
    /// </summary>
    /// <param Name="memberNo"></param>
    /// <param Name="avatar"></param>
    public void saveMucMemberAvatarByTable(String memberNo, String avatar) {
        try {
            MucMembersDao.getInstance().saveMucMemberAvatarByNo(memberNo, avatar);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }



    /// <summary>
    /// 好友昵称变更通知消息 引起的 群成员昵称变更
    /// </summary>
    /// <param Name="memberNo"></param>
    /// <param Name="strOldNickName"></param>
    /// <param Name="strNewNickName"></param>
    public void saveByUserNicknameChangedMessage(String memberNo, String strOldNickName, String strNewNickName) {
        try {
            List<MucMembersTable> list = MucMembersDao.getInstance().findByMucNo(memberNo);
            if (list != null) {
                foreach (MucMembersTable mucMembersTable in list) {
                    // 如果旧的昵称与群成员的昵称一致、代表未修改过群昵称
                    if (strOldNickName.Equals(mucMembersTable.nickname)) {
                        mucMembersTable.nickname = strNewNickName;
                        // 修改新的群昵称
                        MucMembersDao.getInstance().save(mucMembersTable);
                        // TODO:修改缓存
                        //this.saveCacheGroupMember(mucMembersTable);
                    }
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 通过id查询群
    /// </summary>
    /// <param Name="mucNo"></param>
    /// <returns></returns>
    public MucTable FindGroupByNo(String mucNo) {
        try {
            return MucDao.getInstance().FindGroupByNo(mucNo);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
            return null;
        }
    }
    /// <summary>
    /// 通过NO删除群
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public void deleteGroup(String mucid, String mucNo) {
        ContactsApi.deleteGroup( mucid,  mucNo);
    }
    /// <summary>
    /// 通过no查询群
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public MucTable FindGroupById(String id) {
        try {
            return MucDao.getInstance().FindGroupById(id);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
            return null;
        }
    }

    /// <summary>
    /// 通过群no查找群成员
    /// </summary>
    /// <param Name="mucNo"></param>
    /// <returns></returns>
    public List<MucMembersTable> findByMucNo(String mucNo) {
        try {
            return MucMembersDao.getInstance().findByMucNo(mucNo);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
            return null;
        }
    }

    /// <summary>
    /// 通过群id和成员id查找群成员
    /// </summary>
    /// <param Name="mucId"></param>
    /// <param Name="clientuserId"></param>
    /// <returns></returns>
    public MucMembersTable findByMucIdAndClientuserId(String mucId, String clientuserId) {
        try {
            return MucMembersDao.getInstance().findByMucIdAndClientuserId( mucId,  clientuserId);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
            return null;
        }
    }

    /// <summary>
    /// 通过群no和成员id查找群成员
    /// </summary>
    /// <param Name="mucNo"></param>
    /// <param Name="clientuserId"></param>
    /// <returns></returns>
    public MucMembersTable findByMucNoAndClientuserId(String mucNo, String clientuserId) {
        try {
            return MucMembersDao.getInstance().findByMucNoAndClientuserId(mucNo, clientuserId);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
            return null;
        }
    }

    /// <summary>
    /// 查询全部群
    /// </summary>
    /// <returns></returns>
    public List<MucTable> FindAllGroup() {
        try {
            return MucDao.getInstance().FindAllGroup();
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
            return null;
        }
    }

    /// <summary>
    /// 创建群聊
    /// </summary>
    /// <param Name="name">群名称</param>
    /// <param Name="memberid">群成员ID集合</param>
    public void createGroup(String name, List<String> memberid) {
        try {
            ContactsApi.createGroup(name, memberid);
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }

    /// <summary>
    /// 群成员管理
    /// </summary>
    /// <param name="MucId"></param>
    /// <param name="listAddPerson"></param>
    /// <param name="listRemovePerson"></param>
    public void ManageGroupMember(String MucId, List<String> listAddPerson, List<String> listRemovePerson) {
        try {
            Thread t = new Thread(new ThreadStart(() => {
                if (listAddPerson.Count > 0) {
                    ContactsApi.addGroupMember(MucId, listAddPerson, SourceType.addressList.ToStr());
                    Thread.Sleep(1000);
                }

                if (listRemovePerson.Count > 0) {
                    ContactsApi.deleteGroupMember(MucId, listRemovePerson);
                }
            }));
            t.IsBackground = true;
            t.Start();
        } catch (Exception e) {
            Log.Error(typeof(MucServices), e);
        }
    }
}
}
