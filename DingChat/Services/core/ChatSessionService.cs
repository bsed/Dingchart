using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Views.Control.Message;
using java.util;

namespace cn.lds.chatcore.pcw.Services.core {
/// <summary>
/// 会话管理类
/// </summary>
public class ChatSessionService {
    private static ChatSessionService instance = null;
    public static ChatSessionService getInstance() {
        if (instance == null) {
            instance = new ChatSessionService();
        }
        return instance;
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
        // 收到聊天消息
        case MsgType.Text:
        case MsgType.At:
        case MsgType.Voice:
        case MsgType.Video:
        case MsgType.Location:
        case MsgType.News:
        case MsgType.File:
        case MsgType.Image:
        case MsgType.VCard:
        case MsgType.PublicCard:
        case MsgType.Ticket:
        case MsgType.Product:
        case MsgType.Activity:
        case MsgType.Business:
        case MsgType.App:
            //onChatMsg(message);
            break;
        default:
            break;
        }
    }

    /// <summary>
    /// API请求处理
    /// C007:删除好友
    /// C015:创建群
    /// C018: 删除/退出群聊 deleteGroup
    /// C036:扫描群成员的二维码加入群聊组
    /// </summary>
    /// <param Name="eventData"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        switch (eventData.eventDataType) {

        // 删除好友
        case EventDataType.deleteFriend:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                C007(eventData);
            }
            // API请求失败
            else {

            }
            break;
        // 创建群
        case EventDataType.createGroup://sssssss
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
                C015(eventData);
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
        // 扫描群成员的二维码加入群聊组
        case EventDataType.joinGroupByNoAndMember:
            // API请求成功
            if (eventData.eventType == EventType.HttpRequest) {
            }
            // API请求失败
            else {

            }
            break;
        default:
            break;
        }

    }

    /// <summary>
    /// 删除好友后、会话也跟着删除
    /// </summary>
    /// <param Name="extras"></param>
    private void C007(EventData<Object> eventData) {
        try {
            // 获取跟踪参数
            Dictionary<String, Object> extras = eventData.extras;
            String no = extras["no"].ToStr();
            this.deleteChatSessionByNo(no);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 创建群聊成功后，建立会话
    /// </summary>
    /// <param Name="eventData"></param>
    private void C015(EventData<Object> eventData) {
        try {
            MucTableBean mucTableBean = JsonConvert.DeserializeObject<MucTableBean>(eventData.data.ToStr());
            if (mucTableBean != null) {
                ChatSessionTable table = new ChatSessionTable();
                table.account = App.AccountsModel.no;
                table.chatType = ChatSessionType.MUC.ToStr();
                table.user = mucTableBean.no;
                table.resource = mucTableBean.no;
                table.timestamp = DateTimeHelper.getTimeStamp().ToStr();
                table.lastMessage = LocalMessageHelper.getCreateMucMsg(eventData.data);
                table.unReadMessageCount = 0;
                this.addChatSession(table);
            }


        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
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
            this.deleteChatSessionByNo(mucNo);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }



    /// <summary>
    /// 收到聊天消息处理
    /// </summary>
    public void onChatMsg(Message messageBean) {
        try {
            Boolean isAddUnReadMessageCount = true;
            String from = messageBean.getFrom();
            String to = messageBean.getTo();
            // 如果收到了自己设备的同步消息，则将from和to调转
            if (from.Equals(App.AccountsModel.no)) {
                isAddUnReadMessageCount = false;
                // 如果是单聊
                if (ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.CHAT
                        || ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.PUBLIC) {
                    from = messageBean.getTo();
                    to = messageBean.getFrom();
                    // 如果from 和to 相同，则代表是自己给自己发(文件传输助手)
                    if (from.Equals(to)) {
                    } else {
                    }
                } else
                    // 如果是群聊
                    if (ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.MUC) {
                    }
            }

            MsgType msgType = messageBean.getType();
            // 获取显示的最后信息内容
            String formatText = "";
            switch (msgType) {
            case MsgType.Text:
                formatText = ((TextMessage)messageBean).getText();
                break;
            case MsgType.At:
                formatText = ((AtMessage)messageBean).getText();
                break;
            case MsgType.Voice:
                formatText = Constants.MESSAGE_FORMAT_TEXT_VE;
                break;
            case MsgType.Video:
                formatText = Constants.MESSAGE_FORMAT_TEXT_VO;
                break;
            case MsgType.Location:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_LN;
                formatText = ((LocationMessage)messageBean).getAddress();
                break;
            case MsgType.News:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_ML;
                List<MultimediaEntry> articles = ((NewsMessage)messageBean).getEntries();
                if (articles != null && articles.Count > 0) {
                    formatText = articles[0].getTitle();
                } else {
                    formatText = "";
                }
                break;
            case MsgType.File:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_FE;
                formatText = ((FileMessage)messageBean).getFileName();
                break;
            case MsgType.Image:
                formatText = Constants.MESSAGE_FORMAT_TEXT_PE;
                break;
            case MsgType.VCard:
                formatText = Constants.MESSAGE_FORMAT_TEXT_VD;
                break;
            case MsgType.PublicCard:
                formatText = Constants.MESSAGE_FORMAT_TEXT_PD;
                break;
            case MsgType.Ticket:
                formatText = Constants.MESSAGE_FORMAT_TEXT_TICKET;
                break;
            case MsgType.Business:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_BUSINESS;
                formatText = ((BusinessMessage)messageBean).getTitle();
                break;
            case MsgType.AVMeetingInvite:
                AVMeetingInviteMessage inviteMessage = (AVMeetingInviteMessage)messageBean;
                // 视频
                if (AVMeetingType.video.ToStr().Equals(inviteMessage.getAvType())) {
                    formatText = Constants.MESSAGE_FORMAT_TEXT_VIDEO;
                }
                // 语音
                else {
                    formatText = Constants.MESSAGE_FORMAT_TEXT_AUDIO;
                }
                break;
            default:
                break;
            }


            ChatSessionType chatSessionType = ToolsHelper.getChatSessionTypeByNo(to);

            // TODO：这里是直接构建了用于更新表的table对象，需要用model处理下。
            ChatSessionTable chatSessionTable = new ChatSessionTable();
            chatSessionTable.account = App.AccountsModel.no;
            String name = "";
            if (ChatSessionType.MUC==chatSessionType) {
                //群消息比较特殊。from是说话人，to是群编号，存表时候
                name = ContactsServices.getInstance().getMucMemberNameByNo(to, from);
                if (name!=null) {
                    name = name + ": ";
                }
                chatSessionTable.user = to;
                chatSessionTable.chatType = chatSessionType.ToString();
            } else {
                chatSessionTable.user = from;
                chatSessionType = ToolsHelper.getChatSessionTypeByNo(from);
                chatSessionTable.chatType = chatSessionType.ToString();
            }

            chatSessionTable.resource = from;
            chatSessionTable.timestamp = messageBean.getTimestamp().ToStr();
            chatSessionTable.lastMessage = name + formatText;
            chatSessionTable.messageId = messageBean.getMessageId();

            // 如果不是同步的消息，并且画面不在当前的会话，则未读消息数+1
            if (isAddUnReadMessageCount && !chatSessionTable.user.Equals(App.SelectChartSessionNo)) {
                chatSessionTable.unReadMessageCount = 1 ;
            } else {
                chatSessionTable.unReadMessageCount = 0;
            }
            //更新 会话表
            this.addChatSession(chatSessionTable);

        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }


    /// <summary>
    /// 新增会话消息(没有就新增，有则修改)
    /// </summary>
    /// <param Name="newSession"></param>
    public void addChatSession(ChatSessionTable newSession) {
        lock (this) {
            try {
                ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().findByNo(newSession.user);
                if (chatSessionTable == null) {
                    chatSessionTable = newSession;
                } else {
                    chatSessionTable.lastMessage = newSession.lastMessage;
                    chatSessionTable.messageId = newSession.messageId;
                    chatSessionTable.timestamp = DateTimeHelper.getTimeStamp().ToStr();
                    chatSessionTable.unReadMessageCount = 0+chatSessionTable.unReadMessageCount + newSession.unReadMessageCount;
                    //chatSessionTable.resource = newSession.resource;
                    //chatSessionTable.user = newSession.user;
                    //chatSessionTable.Atme = newSession.Atme;
                    //chatSessionTable.Chatdraft = newSession.Chatdraft;
                    //chatSessionTable.chatType = newSession.chatType;

                }
                ChatSessionDao.getInstance().save(chatSessionTable);


                //更新chartsession
                BusinessEvent<object> businessEventChart = new BusinessEvent<object>();
                businessEventChart.data = newSession;
                businessEventChart.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
                EventBusHelper.getInstance().fireEvent(businessEventChart);
            } catch (Exception e) {
                Log.Error(typeof(ChatSessionService), e);
            }
        }

    }



    /// <summary>
    /// 根据用户的No删除会话
    /// </summary>
    /// <param Name="userNo">用户、群、公众号编号</param>
    public void deleteChatSessionByNo(String userNo) {
        try {
            // 删除会话数据
            ChatSessionDao.getInstance().deleteByNo(userNo);
            // 通知画面刷新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 清空会话
    /// </summary>
    public void clearChatSession() {
        try {
            // 清空会话数据
            ChatSessionDao.getInstance().deleteAll();
            // 通知画面刷新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 更新会话的最后一条消息
    /// </summary>
    /// <param Name="userNo">好友、群、公众号编号</param>
    /// <param Name="LastMessage">最后的聊天消息</param>
    /// <param Name="time">时间</param>
    /// <param Name="msgId">消息编号</param>
    /// <param Name="isIncome">是否是收消息</param>
    public void updateLastMessageByNo(String userNo,String resource, String lastMessage, long time, String msgId) {
        try {
            ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().findByNo(userNo);
            if (chatSessionTable==null) {
                chatSessionTable = new ChatSessionTable();
                if (ToolsHelper.getChatSessionTypeByNo(userNo) == ChatSessionType.CHAT) {
                    chatSessionTable.chatType = ChatSessionType.CHAT.ToStr();
                } else if (ToolsHelper.getChatSessionTypeByNo(userNo) == ChatSessionType.MUC) {
                    chatSessionTable.chatType = ChatSessionType.MUC.ToStr();
                } else if (ToolsHelper.getChatSessionTypeByNo(userNo) == ChatSessionType.PUBLIC) {
                    chatSessionTable.chatType = ChatSessionType.PUBLIC.ToStr();
                }
                chatSessionTable.user = userNo;
                if (resource!=null) {
                    chatSessionTable.resource = resource;
                }
            }
            chatSessionTable.lastMessage = lastMessage;
            chatSessionTable.messageId = msgId;
            chatSessionTable.timestamp = time.ToStr();
            chatSessionTable.unReadMessageCount = 0;
            ChatSessionDao.getInstance().save(chatSessionTable);

            //更新chartsession
            BusinessEvent<object> businessEventChart = new BusinessEvent<object>();
            businessEventChart.data = chatSessionTable;
            businessEventChart.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEventChart);

        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 更新草稿
    /// </summary>
    /// <param Name="userNo">好友、群、公众号编号</param>
    /// <param Name="Chatdraft">草稿</param>
    public void updateChatdraft(String userNo, String chatdraft) {
        try {
            ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().findByNo(userNo);
            if (chatSessionTable != null) {
                chatSessionTable.chatdraft = chatdraft;
                ChatSessionDao.getInstance().save(chatSessionTable);
            }

        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 更新是否@我
    /// </summary>
    /// <param Name="userNo"></param>
    /// <param Name="Atme"></param>
    public void updateAtme(String userNo, Boolean atme) {
        try {
            ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().findByNo(userNo);
            if (chatSessionTable != null) {
                chatSessionTable.atme = atme;
                ChatSessionDao.getInstance().save(chatSessionTable);
            }
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 根据会话对象的编号查找会话记录
    /// </summary>
    /// <param Name="userNo">好友、群、公众号等编号</param>
    /// <returns></returns>
    public ChatSessionTable findByNo(String userNo) {
        ChatSessionTable table = null;
        try {
            table = ChatSessionDao.getInstance().findByNo(userNo);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
        return table;
    }

    ///// <summary>
    ///// TODO:理论上不应该有这个方法提供给上层用。查找全部
    ///// </summary>
    ///// <param Name="userNo"></param>
    ///// <returns></returns>
    //public List<ChatSessionTable> findAll() {
    //    try {
    //        return ChatSessionDao.getInstance().findAll();
    //    } catch (Exception e) {
    //        Log.Error(typeof(ChatSessionService), e);
    //        return null;
    //    }
    //}

    /// <summary>
    /// 新增会话消息(没有就新增，有则修改)
    /// </summary>
    /// <param Name="newSession"></param>
    public void addChatSession(Message messageBean) {
        try {
            ChatSessionTable chatSessionTable = this.messageToChatSessionTable(messageBean);
            // 如果是撤销消息，需要判断是不是处理的会话中的最新一条记录
            if (MsgType.Cancel == messageBean.getType()) {
                ChatSessionTable chatSessionTableFromDb = ChatSessionDao.getInstance().findByNo(chatSessionTable.user);
                if (chatSessionTableFromDb != null) {
                    // 如果不是在撤销最新的消息，则不修改会话
                    if (!chatSessionTableFromDb.messageId.Equals(chatSessionTable.messageId)) {
                        return;
                    }
                }
            }
            this.addChatSession(chatSessionTable);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 新增会话消息(没有就新增，有则修改)
    /// </summary>
    /// <param Name="newSession"></param>
    public void addChatSession(MessagesTable table) {
        try {
            ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().findByNo(table.user);
            if (chatSessionTable==null) {
                chatSessionTable = new ChatSessionTable();
                chatSessionTable.resource = table.resource;
                chatSessionTable.user = table.user;
                chatSessionTable.atme = table.atme;
                //chatSessionTable.Chatdraft = newSession.Chatdraft;
                chatSessionTable.chatType = table.type;
                chatSessionTable.unReadMessageCount = 1;
            }
            chatSessionTable.lastMessage = table.text;
            chatSessionTable.messageId = table.messageId;
            chatSessionTable.timestamp = DateTimeHelper.getTimeStamp().ToStr();
            this.addChatSession(chatSessionTable);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
    }

    /// <summary>
    /// 将发送的消息转换成会话表
    /// </summary>
    /// <param Name="messageBean"></param>
    /// <returns></returns>
    private ChatSessionTable messageToChatSessionTable(Message messageBean) {

        MsgType msgType = messageBean.getType();
        String formatText = "";
        switch (msgType) {
        case MsgType.Text:
            formatText = ((TextMessage)messageBean).getText();
            break;
        case MsgType.At:
            formatText = ((AtMessage)messageBean).getText();
            break;
        case MsgType.Voice:
            formatText = Constants.MESSAGE_FORMAT_TEXT_VE;
            break;
        case MsgType.Video:
            formatText = Constants.MESSAGE_FORMAT_TEXT_VO;
            break;
        case MsgType.Location:
            //formatText = Constants.MESSAGE_FORMAT_TEXT_LN;
            formatText = ((LocationMessage)messageBean).getAddress();
            break;
        case MsgType.News:
            //formatText = Constants.MESSAGE_FORMAT_TEXT_ML;
            List<MultimediaEntry> articles = ((NewsMessage)messageBean).getEntries();
            if (articles != null && articles.Count > 0) {
                formatText = articles[0].getTitle();
            } else {
                formatText = "";
            }
            break;
        case MsgType.File:
            //formatText = Constants.MESSAGE_FORMAT_TEXT_FE;
            formatText = ((FileMessage)messageBean).getFileName();
            break;
        case MsgType.Image:
            formatText = Constants.MESSAGE_FORMAT_TEXT_PE;
            break;
        case MsgType.VCard:
            formatText = Constants.MESSAGE_FORMAT_TEXT_VD;
            break;
        case MsgType.PublicCard:
            formatText = Constants.MESSAGE_FORMAT_TEXT_PD;
            break;
        case MsgType.Ticket:
            formatText = Constants.MESSAGE_FORMAT_TEXT_TICKET;
            break;
        //case MsgType.Product:
        //    formatText = ((ProductMessage)messageBean).getTitle();
        //    break;
        //case MsgType.Activity:
        //    formatText = ((ActivityMessage)messageBean).getTitle();
        //    break;
        case MsgType.Business:
            formatText = ((BusinessMessage)messageBean).getTitle();
            break;
        case MsgType.Cancel:
            String user = ((CancelMessage)messageBean).getFrom();
            if (App.AccountsModel.no.Equals(user) || string.IsNullOrEmpty(user))
                formatText = "你撤回了一条消息";
            else {
                VcardsTable table = VcardsDao.getInstance().findByNo(user);
                formatText = table.nickname + "撤回了一条消息";
            }
            break;
        case MsgType.AVMeetingInvite:
            AVMeetingInviteMessage item = (AVMeetingInviteMessage) messageBean;
            // 视频
            if (item.getAvType()== AVMeetingType.video.ToStr()) {
                formatText = Constants.MESSAGE_FORMAT_TEXT_VIDEO;
            }
            // 语音
            else {
                formatText = Constants.MESSAGE_FORMAT_TEXT_AUDIO;
            }
            break;
        default:
            break;
        }

        ChatSessionType chatSessionType = ToolsHelper.getChatSessionTypeByNo(messageBean.getTo());
        ChatSessionTable chatSessionTable = new ChatSessionTable();
        chatSessionTable .account = App.AccountsModel.no;
        chatSessionTable.chatType = chatSessionType.ToStr();
        chatSessionTable.user = messageBean.getTo();
        chatSessionTable.resource = messageBean.getFrom();
        chatSessionTable.timestamp = DateTimeHelper.getTimeStamp().ToStr();
        chatSessionTable.lastMessage = formatText;
        chatSessionTable.messageId = messageBean.getMessageId();
        return chatSessionTable;
    }

    /**
     * 查询所有会话
     *
     * @return List<ChatHistoryInfo>
     */
    public List<ChatSessionBean> findAllChatSession() {
        List<ChatSessionBean> infos = new List<ChatSessionBean>();
        int num_read_messages = 0;
        // 判断是否手动加入文件传输助手
        Boolean isAddFileTransportHelper = true;
        App.ChartSessionCount = 2;
        try {
            List<ChatSessionTable> dbModels = null;

            // 查看全部
            dbModels = ChatSessionDao.getInstance().findAll();
            if (null == dbModels)
                return infos;

            for (int i=0; i<dbModels.Count; i++) {

                ChatSessionTable table = dbModels[i];
                String name_public = "";
                ChatSessionBean info = new ChatSessionBean();

                // 联系人编号
                String user = table.user;
                info.Contact = user;
                // 会话类型
                ChatSessionType type = ToolsHelper.getChatSessionTypeByNo(user);
                // 群聊
                if (ChatSessionType.MUC == type) {
                    MucTable mucTable = MucDao.getInstance().FindGroupByNo(user);
                    if (null != mucTable) {
                        info.Name = mucTable.name;
                        info.AvatarStorageRecordId = mucTable.avatarStorageRecordId;
                        info.AvatarPath = ImageHelper.loadAvatarPath(info.AvatarStorageRecordId);

                        // 如果群聊也可以设置头像，在这里完成
                    } else {
                        // 如果未查询到群的数据，则直接报No赋值给名字
                        info.Name = user;
                    }
                    info.ChatSessionType = ChatSessionType.MUC;
                }
                // 公众号
                else if (ChatSessionType.PUBLIC == type) {
                    // TODO：公众号的处理延后写
                    PublicAccountsTable publicAccountsTable = PublicAccountsService.getInstance().findByAppId(table.user);
                    if(publicAccountsTable!=null) {
                        name_public = publicAccountsTable.name + "：";
                    }
                    info.ChatSessionType = ChatSessionType.PUBLIC;
                    //// 查看全部
                    //if ("".equals(chat_type_action))
                    //{
                    //    if (null != publicAccountsTable)
                    //    {
                    //        info.setAvatar(Constants.PUBLIC_ACCOUNT_FLAG);
                    //        name_public = publicAccountsTable.Name + "：";
                    //    }
                    //    else
                    //    {
                    //        info.setName(user);
                    //    }
                    //}
                    //// 公众号
                    //else if ("chat_type_public".equals(chat_type_action))
                    //{
                    //    if (null != publicAccountsTable)
                    //    {
                    //        info.setName(publicAccountsTable.getName());
                    //        info.setAvatar(publicAccountsTable.getLogoid());//TODO 加载图片的方法 以后更改
                    //    }
                    //    else
                    //    {
                    //        info.setName(user);
                    //    }
                    //}
                }
                // 系统通知 TODO 这个可能有点问题
                else if (ChatSessionType.NOTICE == type) {
                    info.Name = "系统通知";
                    info.ChatSessionType = ChatSessionType.NOTICE;
                    info.AvatarPath = ImageHelper.loadSysImageBrush(@"Notice.png");
                    if (table.lastMessage != null) {
                        info.LastMessage =  table.lastMessage;
                    }
                    info.Timestamp = long.Parse(table.timestamp);
                    info.ChatTime = DateTimeHelper.getDate(table.timestamp);
                    info.Top = false;
                    info.Quiet = false;
                    info.Atme = false;
                    info.Chatdraft = string.Empty;
                }
                // 单聊
                else if (ChatSessionType.CHAT == type) {
                    // 如果是自己给自己发消息，而不是同步消息|| App.AccountsModel.no.Equals(table.resource)
                    if (App.AccountsModel.no.Equals(user)) {
                        info.Name = "文件传输助手";
                        info.Contact = App.AccountsModel.no;
                        info.AvatarPath = ImageHelper.loadSysImageBrush(@"FileSend.png");
                        isAddFileTransportHelper = false;
                        info.Top = false;
                    } else {
                        VcardsTable vcardsTable = VcardsDao.getInstance().findByNo(user);
                        if (null != vcardsTable) {
                            info.Name = ContactsServices.getInstance().getContractNameByNo(user);
                            info.AvatarStorageRecordId = vcardsTable.avatarStorageRecordId;
                            info.AvatarPath = ImageHelper.loadAvatarPath(info.AvatarStorageRecordId);
                        } else {
                            info.Name = user;
                        }
                    }
                    info.ChatSessionType = ChatSessionType.CHAT;
                }
                // 查看全部
                if (ChatSessionType.PUBLIC == type) {

                    info.Name = "公众号";
                    info.AvatarPath = ImageHelper.loadSysImageBrush(@"PublicAccounts.png");
                    if (table.lastMessage != null) {
                        info.LastMessage = name_public + table.lastMessage;
                    }
                    info.ChatSessionType = ChatSessionType.PUBLIC;
                    info.Timestamp = long.Parse(table.timestamp);
                    info.ChatTime = DateTimeHelper.getDate(table.timestamp);
                    //int count = MessageService.getInstance().countOfPublicUnreadMessagesByType();
                    int count = (int)table.unReadMessageCount;
                    info.NewMsgCount = count;
                    num_read_messages = num_read_messages + count;
                    info.Top=false;
                    info.Quiet=false;
                    info.Atme=false;
                    info.Chatdraft=string.Empty;
                } else {
                    info.LastMessage=table.lastMessage;
                    info.Timestamp=long.Parse(table.timestamp);
                    info.ChatTime=DateTimeHelper.getDate(table.timestamp);
                    //int count = MessageService.getInstance().countOfUnreadMessages(user);
                    int count = (int)table.unReadMessageCount;
                    info.NewMsgCount=count;
                    num_read_messages = num_read_messages + count;
                    info.Top=SettingService.getInstance().isTop(user);
                    if(info.Top) {
                        App.ChartSessionCount++;
                    }
                    info.Quiet=SettingService.getInstance().isQuiet(user);
                    info.Atme=table.atme;
                    info.Chatdraft=table.chatdraft;
                    info.DateStr =DateTimeHelper.ChartGetDate(info.Timestamp.ToStr());
                    if (info.Quiet == true && info.NewMsgCount != 0) {
                        info.ShowYuan = true;
                    } else {
                        info.ShowYuan = false;
                    }
                }


                // 排除为拉取到数据的会话
                if (!info.Contact.Equals(info.Name)) {
                    infos.Add(info);
                }

            }

        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }

        // TODO：为会话排序
        //ChatSessionComparator comparator = new ChatSessionComparator();
        //Collections.sort(infos, comparator);

        // 查看全部

        //如果业务要求强制公众号置顶，则手动插入
        if (Constants.SYS_CONFIG_IS_PUBLIC_TOP) {
            // TODO 待处理
            // infos.Insert(0, this.getChatSessionPublic());

        }
        //如果业务要求强制应用消息置顶，则手动插入
        if (Constants.SYS_CONFIG_SHOW_APPMSG) {
            // TODO 待处理
            infos.Insert(0, this.getChatSessionAppMsg());

        }
        //如果业务要求强制待办置顶，则手动插入
        if (Constants.SYS_CONFIG_SHOW_TODO) {
            // TODO 待处理
            infos.Insert(0, this.getChatSessionTodoTask());

        }
        // 判断是否需要手工插入文件传输助手
        if (isAddFileTransportHelper) {
            infos.Insert(App.ChartSessionCount, this.getChatSessionFileTransportHelper());
        }
        // 设置未读的消息
        int intMsgCount = 0;
        foreach(ChatSessionBean chatSessionBean in infos) {
            intMsgCount -= chatSessionBean.NewMsgCount;
            intMsgCount += chatSessionBean.NewMsgCount;
        }
        //MyApplication.getInstance().setNum_read_messages(intMsgCount);
        return infos;
    }

    /**
     * 查询所有会话
     *
     * @return List<ChatHistoryInfo>
     */
    public List<ChatSessionBean> findPublic(bool top) {
        List<ChatSessionBean> infos = new List<ChatSessionBean>();
        int num_read_messages = 0;
        try {
            List<ChatSessionTable> dbModels = null;
            // 公众号
            dbModels = ChatSessionDao.getInstance().findPublic(top);
            if (null == dbModels)
                return infos;

            for (int i = 0; i < dbModels.Count; i++) {

                ChatSessionTable table = dbModels[i];
                String name_public = "";
                ChatSessionBean info = new ChatSessionBean();

                // 联系人编号
                String user = table.user;
                info.Contact = user;
                // 会话类型
                ChatSessionType type = ToolsHelper.getChatSessionTypeByNo(user);

                // TODO：公众号的处理延后写
                //PublicAccountsTable publicAccountsTable = PublicAccountManager.getInstance().findByAppId(user);
                //// 查看全部
                //if ("".equals(chat_type_action)) {
                //    if (null != publicAccountsTable) {
                //        info.setAvatar(Constants.PUBLIC_ACCOUNT_FLAG);
                //        name_public = publicAccountsTable.getName() + "：";
                //    } else {
                //        info.setName(user);
                //    }
                //}
                //// 公众号
                //else if ("chat_type_public".equals(chat_type_action)) {
                //    if (null != publicAccountsTable) {
                //        info.setName(publicAccountsTable.getName());
                //        info.setAvatar(publicAccountsTable.getLogoid());//TODO 加载图片的方法 以后更改
                //    } else {
                //        info.setName(user);
                //    }
                //}
                if (table.timestamp == "0") {
                    continue;
                }

                // 公众号
                info.LastMessage=table.lastMessage;
                if(table.timestamp!="0") {
                    info.Timestamp=long.Parse(table.timestamp);
                    info.ChatTime=DateTimeHelper.getDate(table.timestamp);
                    info.DateStr = DateTimeHelper.ChartGetDate(info.Timestamp.ToStr());
                }

                int count = MessageService.getInstance().countOfUnreadMessages(user);
                info.NewMsgCount=count;
                num_read_messages = num_read_messages + count;
                info.Top=SettingService.getInstance().isTop(user);
                info.Quiet=SettingService.getInstance().isQuiet(user);
                info.Atme=table.atme;
                info.Chatdraft=table.chatdraft;
                PublicAccountsTable dr = PublicAccountsService.getInstance().findByAppId(user);

                if (null != dr) {
                    info.AvatarStorageRecordId = dr.logoId.ToStr();
                    info.AvatarPath = ImageHelper.loadAvatarPath(info.AvatarStorageRecordId);
                    info.Name= dr.name.ToStr();
                    info.tenantNo = dr.tenantNo.ToStr();
                    LoginBeanTenants bean = App.TenantNoDic[info.tenantNo];
                    if (bean != null) {
                        info.tenantName = bean.name;
                    }
                }
                // 排除为拉取到数据的会话
                if (!info.Contact.Equals(info.Name)) {
                    infos.Add(info);
                }
                if(top) {
                    info.tenantNo = "置顶";
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }

        // TODO：为会话排序
        //ChatSessionComparator comparator = new ChatSessionComparator();
        //Collections.sort(infos, comparator);

        return infos;
    }

    /// <summary>
    /// 手动构建公众号会话（当强制公众号会话置顶时的处理）
    /// </summary>
    /// <returns></returns>
    public ChatSessionBean getChatSessionPublic() {
        ChatSessionBean info = new ChatSessionBean();
        try {

            info.Account=App.AccountsModel.no;
            info.AvatarPath = ImageHelper.loadSysImageBrush(@"PublicAccounts.png");
            info.Name="公众号";
            info.ChatSessionType=ChatSessionType.PUBLIC;
            info.Contact=Constants.PUBLIC_ACCOUNT_FLAG;
            //info.setChatTime(null);
            info.LastMessage="";
            // 获取最近的公众号会话
            ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().getLastChatSessionByChatType(Constants.PUBLIC_ACCOUNT_FLAG);
            if (chatSessionTable!=null) {
                String name_public = "";
                // 联系人编号
                String user = chatSessionTable.user;
                info.Contact=user;
                // TODO 公众号的这个延后处理
                PublicAccountsTable publicAccountsTable = PublicAccountsService.getInstance().findByAppId(user);

                if (null != publicAccountsTable) {
                    name_public = publicAccountsTable.name + "：";
                    if (chatSessionTable.lastMessage != null && chatSessionTable.lastMessage!=string.Empty) {
                        info.LastMessage=name_public + chatSessionTable.lastMessage;
                    }
                    if(chatSessionTable.timestamp!="0") {
                        info.Timestamp=long.Parse(chatSessionTable.timestamp);
                        info.ChatTime=DateTimeHelper.getDate(chatSessionTable.timestamp);
                    }

                }
            }

            int count = MessageService.getInstance().countOfPublicUnreadMessagesByType();
            info.NewMsgCount=count;
            info.Top=true;
            info.Quiet=false;
            info.Atme=false;
            info.Chatdraft=string.Empty;
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
        return info;
    }

    /// <summary>
    /// 手动构建待办的会话
    /// </summary>
    /// <returns></returns>
    public ChatSessionBean getChatSessionTodoTask() {
        ChatSessionBean info = new ChatSessionBean();
        try {
            // TODO：待办的处理延后做
            TodoTaskTable todoTaskTable = TodoTaskService.getInstance().GetLastPendingTodoTask();
            info.Account=App.AccountsModel.no;
            info.AvatarStorageRecordId="-1";
            info.ChatSessionType = ChatSessionType.TODO_TASK;
            info.Contact=Constants.TODO_TASK_FLAG;
            info.AvatarPath = ImageHelper.loadSysImageBrush(@"Todo/Todo_task.png");
            info.Name="待办事项";
            if (todoTaskTable != null) {
                TodoTaskContentBean todoTaskBean= JsonConvert.DeserializeObject<TodoTaskContentBean>(todoTaskTable.content.ToStr());
                if(todoTaskBean != null) {
                    info.LastMessage = todoTaskBean.title;
                }

                info.Timestamp= long.Parse(todoTaskTable.createdDate);
                info.ChatTime=DateTimeHelper.getDate(todoTaskTable.createdDate);
                info.DateStr = DateTimeHelper.ChartGetDate(todoTaskTable.createdDate);
            }
            info.NewMsgCount=TodoTaskService.getInstance().countOfTodoTaskPending();
            info.Top=true;
            info.Quiet=false;
            info.Atme=false;
            info.Chatdraft=string.Empty;
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }

        return info;
    }

    /// <summary>
    /// 手动构建文件传输助手
    /// </summary>
    /// <returns></returns>
    public ChatSessionBean getChatSessionFileTransportHelper() {
        ChatSessionBean info = new ChatSessionBean();
        try {
            info.Account=App.AccountsModel.no;
            info.AvatarStorageRecordId="-1";
            info.Contact=App.AccountsModel.no;
            info.Name="文件传输助手";
            info.ChatSessionType = ChatSessionType.CHAT;
            info.AvatarPath = ImageHelper.loadSysImageBrush(@"FileSend.png");
            info.LastMessage="";
            //info.setTimestamp(long.Parse(todoTaskTable.createdDate));
            //info.setChatTime(DateTimeHelper.getDate(todoTaskTable.createdDate));
            info.NewMsgCount=0;
            info.Top=false;
            info.Quiet=false;
            info.Atme=false;
            info.Chatdraft=string.Empty;
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }

        return info;
    }

    /// <summary>
    /// 手动插入一个chartsession
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public void getChatSessionUserOrMuc(string no) {
        ChatSessionTable chatSessionTable = ChatSessionDao.getInstance().findByNo(no);
        if (chatSessionTable == null) {
            chatSessionTable = new ChatSessionTable();
            chatSessionTable.account = App.AccountsModel.no;
            chatSessionTable.user = no;
            chatSessionTable.timestamp = "0";
            if (ToolsHelper.getChatSessionTypeByNo(no) == ChatSessionType.CHAT) {
                chatSessionTable.chatType = ChatSessionType.CHAT.ToStr();
            } else if (ToolsHelper.getChatSessionTypeByNo(no) == ChatSessionType.MUC) {
                chatSessionTable.chatType = ChatSessionType.MUC.ToStr();
            }

            ChatSessionDao.getInstance().save(chatSessionTable);
        }
    }

    /// <summary>
    /// 手动构建应用消息的会话
    /// </summary>
    /// <returns></returns>
    public ChatSessionBean getChatSessionAppMsg() {
        ChatSessionBean info = new ChatSessionBean();
        try {
            info.Account=App.AccountsModel.no;
            info.AvatarStorageRecordId="-1";
            info.Name="应用消息";
            info.ChatSessionType = ChatSessionType.APPMSG;
            info.AvatarPath = ImageHelper.loadSysImageBrush(@"Todo/AppMessage.png");
            info.Contact=Constants.APPMSG_FLAG;
            info.LastMessage="";
            // 获取最近的应用消息
            MessagesTable messageTable = MessageService.getInstance().findLastAppMessages();
            if (messageTable != null) {
                info.LastMessage = messageTable.text;
                info.Timestamp = long.Parse(messageTable.timestamp);
                info.ChatTime = DateTimeHelper.getDate(messageTable.timestamp);
                info.DateStr = DateTimeHelper.ChartGetDate(messageTable.timestamp);
            }

            int count = MessageService.getInstance().countOfAppMessageUnreadMessages();
            info.NewMsgCount=count;
            info.Top=true;
            info.Quiet=false;
            info.Atme=false;
            info.Chatdraft=string.Empty;
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }

        return info;
    }

    /// <summary>
    ///  重置会话的未读消息数量
    /// </summary>
    /// <param name="userNo"></param>
    /// <returns></returns>
    public int ReSetUnReadMessageCountByUserNo(String userNo) {
        int count = 0;
        try {
            count = ChatSessionDao.getInstance().ReSetUnReadMessageCountByUserNo(userNo);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionService), e);
        }
        return count;
    }
}
}
