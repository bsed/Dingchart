using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.im.sdk.bean;
using System.Drawing;
using System.IO;
using System.Threading;
using cn.lds.chatcore.pcw.Views.Control;

namespace cn.lds.chatcore.pcw.Services.core {
public class MessageService {
    private static MessageService instance = null;

    public static MessageService getInstance() {
        if (instance == null) {
            instance = new MessageService();
            ThreadPool.SetMaxThreads(50,20);
        }
        return instance;
    }

    //新建的群，在发送第一条消息时，需要先发送API激活群，准备发的消息先缓存在这里
    public List<SendMessage> waitForMucActiveMessages = new List<SendMessage>();
    //收到消息后，如果是别人新建的群，需要进行异步API拉取群详情，待拉取成功后，并构造完通知消息后，才显示该消息
    public List<MessagesTable> waitForMucDetailsMessages = new List<MessagesTable>();

    // 会话的上一条消息时间缓存（显示时间时候记录，其他时间不记录,间隔5分钟）
    public Dictionary<String, DateTime> chatSessionLastMessageTime = new Dictionary<string, DateTime>();

    /// <summary>
    /// API请求处理
    /// C007:删除好友
    /// C018: 删除/退出群聊 deleteGroup
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
        default:
            break;
        }

    }

    /// <summary>
    /// C007 删除好友后、会话中的消息也跟着删除
    /// </summary>
    /// <param Name="extras"></param>
    private void C007(EventData<Object> eventData) {
        try {
            // 获取跟踪参数
            Dictionary<String, Object> extras = eventData.extras;
            String no = extras["no"].ToStr();
            MessageDao.getInstance().deleteByUser(no);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
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
            MessageDao.getInstance().deleteByUser(mucNo);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 业务处理通知消息
    /// </summary>
    /// <param Name="businessEvent"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> businessEvent) {
        try {
            switch (businessEvent.eventDataType) {
            // 文件上传完成
            case BusinessEventDataType.FileUploadedEvent:
                OnFileUploadedEvent(businessEvent);
                break;
            // 文件上传失败
            case BusinessEventDataType.FileUploadErrorEvent:
                OnFileUploadErrorEvent(businessEvent);
                break;
            default:
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }

    }
    /// <summary>
    /// 文件上传完成处理
    /// </summary>
    /// <param Name="businessEvent"></param>
    public void OnFileUploadedEvent(BusinessEvent<Object> businessEvent) {
        try {
            FileUploadEventData fileUploadEventData = (FileUploadEventData) businessEvent.data;
            switch (fileUploadEventData.uploadFileType) {
            // 图片消息
            case UploadFileType.MSG_IMAGE:
                try {
                    MessagesTable table = MessageDao.getInstance().findByMessageId(fileUploadEventData.businessId);
                    if (table != null) {
                        PictureMessage messageBean = new PictureMessage().toModel(table.content);
                        // 修改消息的文件存储ID
                        messageBean.imageStorageId = fileUploadEventData.id.ToStr();
                        // 修改消息的内容
                        table.content = messageBean.toContent();
                        // 修改消息的发送状态
                        table.sent = "1";
                        MessageDao.getInstance().save(table);
                        MessageItem item = convertToMessItem(table.user, table);
                        this.DoSendMessage(item);
                    }
                } catch (Exception ex) {
                    Log.Error(typeof(MessageService), ex);
                }
                break;
            // 文件消息
            case UploadFileType.MSG_FILE:
                try {
                    MessagesTable table = MessageDao.getInstance().findByMessageId(fileUploadEventData.businessId);
                    if (table != null) {
                        FileMessage messageBean = new FileMessage().toModel(table.content);
                        // 修改消息的文件存储ID
                        messageBean.fileStorageId = fileUploadEventData.id;
                        // 修改消息的内容
                        table.content = messageBean.toContent();
                        // 修改消息的发送状态
                        table.sent = "1";
                        MessageDao.getInstance().save(table);
                        MessageItem item = convertToMessItem(table.user, table);
                        this.DoSendMessage(item);
                    }
                } catch (Exception ex) {
                    Log.Error(typeof(MessageService), ex);
                }
                break;
            // 视频消息
            case UploadFileType.MSG_VIDEO:

                break;
            default:
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }
    }

    /// <summary>
    /// 文件上传失败处理
    /// </summary>
    /// <param Name="businessEvent"></param>
    public void OnFileUploadErrorEvent(BusinessEvent<Object> businessEvent) {
        try {
            FileUploadEventData fileUploadEventData = (FileUploadEventData)businessEvent.data;
            switch (fileUploadEventData.uploadFileType) {
            // 图片消息
            case UploadFileType.MSG_IMAGE:
            // 文件消息
            case UploadFileType.MSG_FILE:
            // 视频消息
            case UploadFileType.MSG_VIDEO:
                MessagesTable table = MessageDao.getInstance().findByMessageId(fileUploadEventData.businessId);
                if (table != null) {
                    // 修改消息的发送状态
                    table.sent = "-1";
                    MessageDao.getInstance().save(table);
                }
                this.FireSendMessageErrorEvent(fileUploadEventData.businessId);
                break;
            default:
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }
    }

    /// <summary>
    /// 触发消息发送失败事件
    /// </summary>
    /// <param Name="strMessageId"></param>
    private void FireSendMessageErrorEvent(String strMessageId) {
        try {
            MessageItem messageItem = MessageService.getInstance().updateSendStatus(strMessageId, -1);

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = messageItem;
            businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_UPDATE;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }



    /// <summary>
    /// 是否显示该消息的时间处理
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="time"></param>
    private void ShowMessageDateTimeCtrl(MessagesTable table) {
        try {
            // 如果曾经记录过
            DateTime time = DateTimeHelper.getDate(table.timestamp.ToStr());
            if (chatSessionLastMessageTime.ContainsKey(table.user)) {
                //Console.WriteLine("》》》》》消息时间" + time + ",上次显示时间" + chatSessionLastMessageTime[table.user]);
                TimeSpan ts = time - chatSessionLastMessageTime[table.user];
                if (ts.Minutes > 5) {
                    table.showTimestamp = true;
                    chatSessionLastMessageTime[table.user] = time;
                    //chatSessionLastMessageTime.Add(table.user, time);
                } else {
                    table.showTimestamp = false;
                }
            } else {
                // TODO 先看效果，如果连续登陆的话，需要读取会话的最后一条的聊天时间来再做判断。
                table.showTimestamp = true;
                chatSessionLastMessageTime[table.user] = time;
                //chatSessionLastMessageTime.Add(table.user, time);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 收到聊天消息
    /// </summary>
    /// <param Name="messageArrivedEvent"></param>
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
        case MsgType.Cancel:
        case MsgType.VCard:
        case MsgType.PublicCard:
        case MsgType.Ticket:
        case MsgType.Product:
        case MsgType.Activity:
        case MsgType.Business:
        case MsgType.App:
        case MsgType.AVMeetingInvite:
        case MsgType.MessageTypeAVGroupInvite:
        case MsgType.MessageTypeUserJoinTenant:
        case MsgType.MessageTypeUserLeaveTenant:
            onChatMsg(message);
            break;
        // 已读通知
        case MsgType.ReadMessage:
            ProcessReadMessage(message);
            break;
        default:
            break;
        }
    }



    private void ProcessReadMessage(Message message) {
        try {
            // 消息体转换
            ReadMessage connectMessage = (ReadMessage) message;

            if (connectMessage.getDevice() == "PC") {
                return;
            }
            setMessageRead(connectMessage.getUserNo());

            //更新chartsession
            ChatSessionTable chart = new ChatSessionTable();
            chart.user = connectMessage.getUserNo();
            //更新chartsession
            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            businessEvent.data = chart;
            businessEvent.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception e) {
            Log.Error(typeof (MessageService), e);
        }
    }

    /// <summary>
    /// 收到聊天消息处理
    /// </summary>
    public void onChatMsg(Message messageBean) {
        try {
            String formatText = "";
            String content = "";
            String serverid = "";
            //List<String> titles = new ArrayList<>();
            //List<String> images = new ArrayList<>();
            //List<String> urls = new ArrayList<>();
            String messageId = messageBean.getMessageId();
            serverid = messageId;
            String from = messageBean.getFrom();
            String to = messageBean.getTo();
            Boolean isImcoming = true;
            String sent = "0";
            Boolean read = false;
            // 是否更新会话
            Boolean isUpdateChatSession = true;
            // 如果收到了自己设备的同步消息，则将from和to调转
            if (from.Equals(App.AccountsModel.no)) {
                sent = "1";
                read = true;
                // 如果是单聊
                if (ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.CHAT
                        || ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.PUBLIC) {
                    from = messageBean.getTo();
                    to = messageBean.getFrom();

                    // 如果from 和to 相同，则代表是自己给自己发(文件传输助手)
                    if (from.Equals(to)) {
                        isImcoming = true;
                    } else {
                        isImcoming = false;
                    }
                } else
                    // 如果是群聊
                    if (ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.MUC) {
                        isImcoming = false;
                    }
            }
            String resource = messageBean.getFrom();
            MsgType msgType = messageBean.getType();
            bool isAtMe = false;
            long time = DateTimeHelper.getTimeStamp();
            switch (msgType) {
            case MsgType.Text:
                formatText = ((TextMessage)messageBean).getText();
                break;
            case MsgType.Cancel:
                VcardsTable vcardsTable = VcardService.getInstance().findByNo(resource);
                formatText = vcardsTable.nickname+ "撤回了一条消息";
                CancelMessage cancelMessage = (CancelMessage)messageBean;
                content = cancelMessage.toContent();
                serverid = cancelMessage.getServerMessageId();
                break;
            case MsgType.At:
                // @消息只能是群消息
                AtMessage atMessage = (AtMessage)messageBean;
                formatText = atMessage.getText();
                // 首先判断是否处于当前会话

                // 获取所有@的人，看看是否有自己
                List<String> lstAtNos = atMessage.getAtNos();
                foreach (String no in lstAtNos) {
                    if (App.AccountsModel.no.Equals(no) || "all".Equals(no)) {
                        isAtMe = true;
                        if (!to.Equals(App.SelectChartSessionNo)) {
                            ChatSessionService.getInstance().updateAtme(to, true);
                        }
                        break;
                    }
                }

                content = atMessage.toContent();
                break;
            case MsgType.Voice:
                formatText = Constants.MESSAGE_FORMAT_TEXT_VE;
                VoiceMessage voiceMessage = ((VoiceMessage)messageBean);
                content = voiceMessage.toContent();
                //收到语音消息后，立即开始下载
                Dictionary<String, Object> extrasVoice = new Dictionary<String, Object>();
                extrasVoice.Add("voiceMessage", voiceMessage);
                extrasVoice.Add(DownloadServices.MESSAGEID_KEY, messageId);
                DownloadServices.getInstance().DownloadMethod(voiceMessage.getVoiceStorageId(), DownloadType.MSG_VOICE, extrasVoice);
                break;
            case MsgType.Video:
                formatText = Constants.MESSAGE_FORMAT_TEXT_VO;
                VideoMessage videoMessage = (VideoMessage)messageBean;
                content = videoMessage.toContent();
                //收到视频消息后，立即开始下载
                Dictionary<String, Object> extrasVideo = new Dictionary<String, Object>();
                extrasVideo.Add("videoMessage", videoMessage);
                extrasVideo.Add(DownloadServices.DOWNLOADFILENAME_KEY, videoMessage.fileName);
                extrasVideo.Add(DownloadServices.MESSAGEID_KEY, messageId);
                DownloadServices.getInstance().DownloadMethod(videoMessage.getVideoStorageId(), DownloadType.MSG_VIDEO, extrasVideo);

                break;
            case MsgType.Location:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_LN;
                LocationMessage locationMessage = ((LocationMessage)messageBean);
                formatText = locationMessage.getAddress();
                content = locationMessage.toContent();
                //收到位置消息后，立即开始下载
                //                FileManager.getInstance().download(String.valueOf(locationMessage.getPicture()),
                // FileType.IMAGE,
                //                        FileRelativeType.MESSAGE, locationMessage.getMessageId());
                break;
            case MsgType.News:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_ML;
                NewsMessage newsMessage = (NewsMessage)messageBean;
                content = newsMessage.toContent();
                List<MultimediaEntry> articles = newsMessage.getEntries();
                if (articles != null && articles.Count > 0) {
                    formatText = articles[0].getTitle();
                } else {
                    formatText = "";
                }
                foreach (MultimediaEntry enty in articles) {
                    Dictionary<String, Object> images = new Dictionary<String, Object>();
                    images.Add(DownloadServices.MESSAGEID_KEY, messageId);
                    DownloadServices.getInstance().DownloadMethod(enty.thumbnail, DownloadType.MSG_IMAGE, images);
                }

                break;
            case MsgType.File:
                //formatText = Constants.MESSAGE_FORMAT_TEXT_FE;
                FileMessage fileMessage = (FileMessage)messageBean;
                content = fileMessage.toContent();
                formatText = fileMessage.getFileName();

                Dictionary<String, Object> extrasFile = new Dictionary<String, Object>();
                extrasFile.Add("fileMessage", fileMessage);
                extrasFile.Add(DownloadServices.DOWNLOADFILENAME_KEY, fileMessage.fileName);
                extrasFile.Add(DownloadServices.MESSAGEID_KEY, messageId);
                DownloadServices.getInstance().DownloadMethod(fileMessage.getFileStorageId(), DownloadType.MSG_FILE, extrasFile);
                break;
            case MsgType.Image:
                formatText = Constants.MESSAGE_FORMAT_TEXT_PE;
                PictureMessage pictureMessage = ((PictureMessage)messageBean);
                content = pictureMessage.toContent();
                Dictionary<String, Object> extrasImage = new Dictionary<String, Object>();
                extrasImage.Add(DownloadServices.MESSAGEID_KEY, messageId);
                DownloadServices.getInstance().DownloadMethod(pictureMessage.imageStorageId, DownloadType.MSG_IMAGE, extrasImage);
                break;
            case MsgType.VCard:
                formatText = Constants.MESSAGE_FORMAT_TEXT_VD;
                String userNo = ((VcardMessage)messageBean).getUserNo();
                String avatarStorageId = ((VcardMessage)messageBean).getAvatarStorageId().ToStr();
                content = ((VcardMessage)messageBean).toContent();
                break;
            case MsgType.PublicCard:
                formatText = Constants.MESSAGE_FORMAT_TEXT_PD;
                //String userNo = ((VcardMessage) messageBean).getUserNo();
                //String avatarStorageId = String.valueOf(((VcardMessage) messageBean).getAvatarStorageId());
                content = ((PublicCardMessage)messageBean).toContent();
                break;
            case MsgType.Notify:
                time = messageBean.getTimestamp();
                formatText = ((NotifyMessage)messageBean).getComment();
                String toUser = ((NotifyMessage)messageBean).getTo();
                // 群成员变更通知,重新强制获取一次群详情
                if (ToolsHelper.getChatSessionTypeByNo(toUser) == ChatSessionType.MUC) {
                    MucServices.getInstance().RequestGroupDetail(toUser, null);
                }
                isUpdateChatSession = false;
                break;
            case MsgType.Business:
                BusinessMessage businessMessage = (BusinessMessage)messageBean;
                formatText = businessMessage.getTitle();
                content = businessMessage.toContent();
                break;
            case MsgType.App:
                AppMessage appMessage = (AppMessage)messageBean;
                formatText = appMessage.getTitle();
                content = appMessage.toContent();
                isUpdateChatSession = false;
                break;
            case MsgType.AVMeetingInvite:
                AVMeetingInviteMessage inviteMessage = (AVMeetingInviteMessage)messageBean;

                if (inviteMessage.from == App.AccountsModel.no) return;
                // 如果大于1分钟，则无视处理
                if (!DateTimeHelper.CheckTimestampDiffByMinutes(inviteMessage.timestamp, 1)) {
                    return;
                }
                content = inviteMessage.toContent();
                //正在忙 给对方发个忙的消息
                if (App.AmStatus != AVMeetingStatus.no) {
                    AVMeetingService.getInstance()
                    .SendAVMeetingBusyMessage(from, inviteMessage.getAvType(), inviteMessage.getUId(),
                                              inviteMessage.getRoomId());
                    return;
                }

                // 视频
                if (AVMeetingType.video.ToStr().Equals(inviteMessage.getAvType())) {
                    formatText = Constants.MESSAGE_FORMAT_TEXT_VIDEO;
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = inviteMessage;
                    businessEvent.eventDataType = BusinessEventDataType.VideoMeetingReceiving;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }
                // 语音
                else {
                    formatText = Constants.MESSAGE_FORMAT_TEXT_AUDIO;
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = inviteMessage;
                    businessEvent.eventDataType = BusinessEventDataType.AudioMeetingReceiving;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }


                break;
            case MsgType.MessageTypeAVGroupInvite:
                MessageTypeAVGroupInviteMessage avGroupInviteMessage = (MessageTypeAVGroupInviteMessage)messageBean;
                // 如果大于1分钟，则无视处理
                if (!DateTimeHelper.CheckTimestampDiffByMinutes(avGroupInviteMessage.timestamp, 1)) {
                    return;
                }
                content = avGroupInviteMessage.toContent();
                //正在忙 给对方发个忙的消息
                if (App.AmStatus != AVMeetingStatus.no) {
                    AVMeetingService.getInstance()
                    .SendAVMeetingBusyMessage(from, AVMeetingType.video.ToStr(), uint.Parse( App.AccountsModel.clientuserId.ToStr()),
                                              avGroupInviteMessage.getRoomId());
                    return;
                }


                formatText = Constants.MESSAGE_FORMAT_TEXT_VIDEO;
                BusinessEvent<Object> businessGroupEvent = new BusinessEvent<Object>();
                businessGroupEvent.data = avGroupInviteMessage;
                businessGroupEvent.eventDataType = BusinessEventDataType.AVGroupMeetingReceiving;
                EventBusHelper.getInstance().fireEvent(businessGroupEvent);
                break;
            case MsgType.MessageTypeUserJoinTenant:
                MessageTypeUserJoinTenant messageTypeUserJoinTenant = (MessageTypeUserJoinTenant)messageBean;

                return;
            case MsgType.MessageTypeUserLeaveTenant:
                MessageTypeUserLeaveTenant messageTypeUserLeaveTenant = (MessageTypeUserLeaveTenant)messageBean;



                string tenant = messageTypeUserLeaveTenant.getTenantNo();
                string name = string.Empty;
                if (App.TenantNoDic.ContainsKey(tenant)) {
                    name = App.TenantNoDic[tenant].name;
                } else {
                    name = "某个租户";
                }

                BusinessEvent<Object> leaveTenantEvent = new BusinessEvent<Object>();
                leaveTenantEvent.data = name;
                leaveTenantEvent.eventDataType = BusinessEventDataType.UserLeaveTenant;
                EventBusHelper.getInstance().fireEvent(leaveTenantEvent);


                //ApplicationService.getInstance().ReStartApplication(FrameEventDataType.LOGOUT_TOKEN_INVALID.ToStr(), "您被移出 " + name + " ,需要重启");
                return;
            default:
                break;
            }

            MessagesTable table = new MessagesTable();
            table.messageId = messageId;
            //不知道对不对
            if(messageBean.getType()==MsgType.Cancel) {
                table.messageId = serverid;
            }
            table.user = from;
            table.resource = resource;
            table.text = formatText;
            table.content = content;
            table.timestamp = time.ToStr();
            table.delayTimestamp = messageBean.getTimestamp().ToStr();
            table.type = msgType.ToStr();
            table.incoming = isImcoming;
            table.read = read;
            table.flag = false;
            table.sent = sent;
            table.error = false;
            table.tenantNo = messageBean.tenantNo;

            table.serverMessageId = serverid;
            table.atme = isAtMe;


            //判断本地是否有该公众号，没有时拉取公众号信息
            if (ToolsHelper.getChatSessionTypeByNo(from)== ChatSessionType.PUBLIC) {
                PublicAccountsTable publicAccountsTable = PublicAccountsService.getInstance().findByAppId(from);
                if (publicAccountsTable == null) {

                    //重新拉去公众号消息

                    if (App.TenantNoList.Count > 0) {
                        foreach (string tenantNo in App.TenantNoList) {
                            Dictionary<String, Object> extras = new Dictionary<String, Object>();
                            extras.Add("tenantNo", tenantNo);
                            PublicAccountsService.getInstance().request(extras);
                        }
                    } else {
                        Dictionary<String, Object> extras = new Dictionary<String, Object>();
                        extras.Add("messageId", table.messageId);
                        PublicAccountsService.getInstance().request(extras);
                    }
                    waitForMucDetailsMessages.Add(table);
                    //处理是否显示时间
                    this.ShowMessageDateTimeCtrl(table);
                    MessageDao.getInstance().save(table);
                    return;
                }
            }

            //群消息比较特殊。from是说话人，to是群编号，存表时候

            if (ToolsHelper.getChatSessionTypeByNo(to) == ChatSessionType.MUC) {
                table.user = to;
                //如果是别人新建的群，本地第一次收到群消息时，在这里需要拉取群详细
                if (MucDao.getInstance().FindGroupByNo(to) == null) {
                    Dictionary<String, Object> extras = new Dictionary<string, object>();
                    extras.Add("messageId", table.messageId);
                    MucServices.getInstance().RequestGroupDetail(to, extras);
                    //需要在群拉回来之后，构造一条 新建群 的通知消息
                    waitForMucDetailsMessages.Add(table);
                    //处理是否显示时间
                    this.ShowMessageDateTimeCtrl(table);
                    MessageDao.getInstance().save(table);
                    return;
                }
            }
            //处理是否显示时间
            this.ShowMessageDateTimeCtrl(table);
            MessageDao.getInstance().save(table);
            if (isUpdateChatSession) {
                ChatSessionService.getInstance().onChatMsg(messageBean);
            }
            this.processMessage(table, true);
        } catch(Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 发出新消息达到通知，处理是否需要提示
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="isNotification"></param>
    public void processMessage(MessagesTable table, Boolean isNotification) {
        try {
            MessageItem messageItem = convertToMessItem(table.user, table);

            //发出通知
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = messageItem;
            businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);

            // 提示新消息到达
            if (!messageItem.resource.Equals(App.AccountsModel.no) && !messageItem.name.Equals(messageItem.user) && !MsgType.Notify.ToStr().Equals(messageItem.type)) {
                NotificationHelper.NewMessage(messageItem.user);
            } else if (MsgType.App.ToStr().Equals(table.type)) {
                NotificationHelper.NewMessage(Constants.APPMSG_FLAG);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

    }

    /// <summary>
    /// 拉取公众号信息完成后，继续处理公众号消息
    /// </summary>
    /// <param Name="waitedMessageId"></param>
    public void processMessageAfterPublicAvaliable(String waitedMessageId) {
        try {
            int location = -1;
            for (int i = 0; i < waitForMucDetailsMessages.Count; i++) {
                if (waitForMucDetailsMessages[i].messageId.Equals(waitedMessageId)) {
                    location = i;
                    break;
                }
            }
            if (location > -1) {
                ChatSessionService.getInstance().addChatSession(waitForMucDetailsMessages[location]);
                this.processMessage(waitForMucDetailsMessages[location], true);
                waitForMucDetailsMessages.RemoveAt(location);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 群拉取回来后，继续处理刚缓存的消息
    /// </summary>
    /// <param Name="mucNo"></param>
    /// <param Name="waitedMessageId"></param>
    /// <param Name="manager"></param>
    public void processMessageAfterMucAvaliable(String mucNo, String waitedMessageId, String manager) {
        try {
            int location = -1;
            for (int i = 0; i < waitForMucDetailsMessages.Count; i++) {
                if (waitForMucDetailsMessages[i].messageId.Equals(waitedMessageId)) {
                    location = i;
                    break;
                }
            }
            if (location > -1) {
                //构造 新建群 通知消息
                List<MucMembersTable> members = MucMembersDao.getInstance().findByMucNo(mucNo);
                List<String> names = new List<String>();
                String ownerName = "";
                foreach (MucMembersTable mucMembersTable in members) {

                    // 跳过群主
                    if (manager.Equals(mucMembersTable.clientuserId)) {
                        //群主是自己时，设置创建 群的用户名称
                        if (manager.Equals(App.AccountsModel.clientuserId)) {
                            ownerName = App.AccountsModel.nickname;
                            continue;
                        }
                        // 处理消息的名字（优先检查本地）
                        ownerName = ContactsServices.getInstance().getContractNameByNo(mucMembersTable.no);
                        if (ownerName==null) {
                            ownerName = mucMembersTable.nickname;
                        }
                        continue;
                    }

                    //跳过自己
                    if (App.AccountsModel.no.Equals(mucMembersTable.no)) {
                        continue;
                    }

                    // 处理消息的名字（优先检查本地）
                    String name = ContactsServices.getInstance().getContractNameByNo(mucMembersTable.no);
                    if (name==null) {
                        names.Add(mucMembersTable.nickname);
                    } else {
                        names.Add(name);
                    }
                }
                //TODO: 得拿到群主
                // 获取到当前会话第一条消息
                MessagesTable messagesTable = MessageDao.getInstance().getFirstMessagesByNo(mucNo);
                if (messagesTable!=null) {
                    long time = long.Parse(messagesTable.timestamp);
                    if (time == 0) {
                        time = long.Parse(waitForMucDetailsMessages[location].timestamp);
                    }
                    LocalMessageHelper.sendCreateGroupMsgForMember(mucNo, ownerName, names, time - 1000);
                    ChatSessionService.getInstance().addChatSession(waitForMucDetailsMessages[location]);
                    this.processMessage(waitForMucDetailsMessages[location], true);
                    waitForMucDetailsMessages.RemoveAt(location);
                }

            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }


    /// <summary>
    /// 设置某人的所有聊天记录为已读状态
    /// </summary>
    /// <param Name="user">好友或群组帐号</param>
    public void setMessageRead(String user) {
        try {
            MessageDao.getInstance().setMessageReadByUserNo(user);
            ChatSessionService.getInstance().ReSetUnReadMessageCountByUserNo(user);
            //查询未读数量
            countOfUnreadMessages();
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 设置消息为已读状态
    /// </summary>
    /// <param Name="user">好友或群组帐号</param>
    /// <param Name="messageId">消息的id</param>
    public void setMessageRead(String user, String messageId) {
        try {
            MessagesTable table = MessageDao.getInstance().findByMessageId(messageId);
            if (null != table) {
                table.read = true;
                MessageDao.getInstance().save(table);
            }
            //查询未读数量
            countOfUnreadMessages();
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /**
     * 设置语音/视频消息为已播放状态
     *
     * @param messageItem
     */
    public void setVedioOrVoicePlayed(MessageItem messageItem) {
        try {
            MessagesTable table = MessageDao.getInstance().findByMessageId(messageItem.messageId);
            if (null != table) {
                table.flag = true;
                MessageDao.getInstance().save(table);

                messageItem.flag = true;

                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = messageItem;
                businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_UPDATE;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// MessagesTable转换成MessageItem
    /// </summary>
    /// <param Name="account"></param>
    /// <param Name="user"></param>
    /// <param Name="table"></param>
    /// <returns></returns>
    public MessageItem convertToMessItem(String user, MessagesTable table) {
        try {
            MessageItem messageItem = new MessageItem();
            messageItem.messageId = table.messageId;
            messageItem.error = table.error;
            messageItem.id = table.id;
            messageItem.incoming = table.incoming;
            messageItem.read = table.read;
            messageItem.sent = table.sent;
            messageItem.text = table.text;
            messageItem.content = table.content;
            messageItem.type = table.type;
            messageItem.user = user;
            messageItem.resource = table.resource;
            messageItem.atme = table.atme;
            messageItem.showTimestamp = table.showTimestamp;
            messageItem.serverMessageId = table.serverMessageId;
            String avatarUrl = "";
            String userName = "";

            String resource = table.resource;
            if (ToolsHelper.getChatSessionTypeByNo(user) == ChatSessionType.MUC) {
                //群聊
                MucTable mucTable = MucDao.getInstance().FindGroupByNo(user);
                if (mucTable != null) {
                    MucMembersTable mucMembersTable = MucMembersDao.getInstance().findByMucIdAndMemberNo(mucTable.mucId, resource);
                    if (mucMembersTable != null) {
                        avatarUrl = mucMembersTable.avatarStorageRecordId;
                        userName = mucMembersTable.nickname;
                    }
                }
            }

            //else if (PublicAccountManager.getInstance().isPublicAccount(user)) {
            //    // TODO 公众号 没写完
            //    PublicAccountsTable table1 = PublicAccountManager.getInstance().findByAppId(user);
            //    if (null != table1) {
            //        avatarUrl = table1.getLogoid();
            //        userName = table1.getName();
            //    } else {
            //        userName = user;
            //        PublicAccountManager.getInstance().request();//重新初始化公共号数据
            //    }
            //}
            else {
                //单聊
                if (resource == App.AccountsModel.no) { //自己
                    AccountsTable dt = AccountsDao.getInstance().findByNo(resource);
                    //VcardsTable vcard = VcardsDao.getInstance().findByNo(resource);
                    if (dt != null) {
                        avatarUrl = dt.avatarStorageRecordId;
                        userName = dt.nickname;
                    } else {
                        userName = resource;
                    }
                } else { //别人

                    VcardsTable vcard = VcardsDao.getInstance().findByNo(resource);
                    if (vcard != null) {
                        avatarUrl = vcard.avatarStorageRecordId;
                        userName = vcard.nickname;
                    } else {
                        userName = resource;
                    }
                }
            }

            messageItem.name = userName;
            messageItem.avatar = avatarUrl;
            messageItem.timeDate = DateTimeHelper.getDate(table.timestamp.ToStr());
            messageItem.delayTimeDate = DateTimeHelper.getDate(table.delayTimestamp.ToStr());

            return messageItem;
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return null;
    }

    /// <summary>
    /// 更新消息发送状态
    /// </summary>
    /// <param Name="messageId"消息id></param>
    /// <param Name="status">-1:失败；0：发送中；1：已发送</param>
    /// <returns></returns>
    public MessageItem updateSendStatus(String messageId, int status) {
        try {
            MessagesTable table = MessageDao.getInstance().findByMessageId(messageId);
            if (null != table) {
                table.sent = status.ToStr();
                MessageDao.getInstance().save(table);
                return convertToMessItem(table.user, table);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return null;
    }

    ///**
    // * 更新消息发送状态
    // *
    // * @param messageId 消息id
    // * @param status    -1:失败；0：发送中；1：已发送
    // * @return @see MessageItem
    // */
    /// <summary>
    /// 更新消息发送状态
    /// </summary>
    /// <param Name="messageId">消息id</param>
    /// <param Name="status">-1:失败；0：发送中；1：已发送</param>
    /// <param Name="serverMessageId">服务的消息ID</param>
    /// <returns></returns>
    public MessageItem updateSendStatus(String messageId, int status, String serverMessageId) {
        try {
            MessagesTable table = MessageDao.getInstance().findByMessageId(messageId);
            if (null != table) {
                table.sent = status.ToStr();
                table.serverMessageId = serverMessageId;
                table.delayTimestamp = DateTimeHelper.getTimeStamp().ToStr();
                MessageDao.getInstance().save(table);
                return convertToMessItem(table.user, table);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

        return null;
    }

    /// <summary>
    /// 更新是否播放状态
    /// </summary>
    /// <param Name="messageId"></param>
    /// <param Name="status"></param>
    /// <returns></returns>
    public MessageItem updateFlagStatus(String messageId, Boolean status) {

        try {
            MessagesTable table = MessageDao.getInstance().findByMessageId(messageId);
            if (null != table) {
                table.flag = status;
                MessageDao.getInstance().save(table);

                return convertToMessItem(table.user, table);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return null;
    }

    /// <summary>
    /// 数据库层更新发送时间,用于长按撤回
    /// </summary>
    /// <param Name="messageId"></param>
    /// <param Name="time"></param>
    /// <returns></returns>
    public MessageItem updateDelayTime(String messageId, long time) {
        try {
            MessagesTable table = MessageDao.getInstance().findByMessageId(messageId);
            if (null != table) {
                table.delayTimestamp = time.ToStr();
                MessageDao.getInstance().save(table);

                return convertToMessItem(table.user, table);
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

        return null;
    }

    /// <summary>
    /// 所有未读消息数量
    /// </summary>
    /// <returns></returns>
    public int countOfUnreadMessages() {
        int count = 0;
        try {
            count = MessageDao.getInstance().countOfUnreadMessages("");
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = count;
        Businessdata.eventDataType = BusinessEventDataType.CountOfUnread;
        EventBusHelper.getInstance().fireEvent(Businessdata);

        return count;
    }

    /// <summary>
    /// 某会话未读消息数量
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public int countOfUnreadMessages(String user) {
        int count = 0;
        try {
            count = MessageDao.getInstance().countOfUnreadMessages(user);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return count;
    }

    /// <summary>
    /// 公众号会话未读消息数量
    /// </summary>
    /// <returns></returns>
    public int countOfPublicUnreadMessagesByType() {
        int count = 0;
        try {
            count = MessageDao.getInstance().countOfPublicUnreadMessages(null);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return count;
    }

    /// <summary>
    /// 应用消息会话未读消息数量
    /// </summary>
    /// <returns></returns>
    public int countOfAppMessageUnreadMessages() {
        int count = 0;
        try {
            count = MessageDao.getInstance().countOfAppUnreadMessages();
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return count;
    }

    /// <summary>
    /// 获取某会话的第一条未读消息
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public MessagesTable getFirstUnreadMessages(String user) {
        MessagesTable table = null;
        try {
            table =  MessageDao.getInstance().getFirstUnreadMessagesByNo(user);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return table;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param Name="account"></param>
    /// <param Name="message"></param>
    public void send(Message message) {

        // 修改会话内容 需要早点更新到好像是在chartsession上
        ChatSessionService.getInstance().addChatSession(message);

        ThreadPool.QueueUserWorkItem(doSend, message);


    }

    /// <summary>
    /// 真正发送消息
    /// </summary>
    /// <param Name="messageItem"></param>
    public void DoSendMessage(MessageItem messageItem) {
        try {
            var aa = EnumHelper.getTypeByDescription(typeof (MsgType), messageItem.type);
            MsgType msgType = (MsgType)EnumHelper.getTypeByDescription(typeof(MsgType), messageItem.type);

            Message messageBean = null;
            switch (msgType) {
            // 发送聊天消息
            case MsgType.Text:
                messageBean = this.createTextMessage(messageItem);
                break;
            case MsgType.Cancel:
                messageBean = this.createCancelMessage(messageItem);
                break;
            case MsgType.At:
                messageBean = this.createAtMessage(messageItem);
                break;
            case MsgType.Voice:
                messageBean = this.createVoiceMessage(messageItem);
                break;
            case MsgType.Video:
                messageBean = this.createVideoMessage(messageItem);
                break;
            case MsgType.Location:
                messageBean = this.createLocationMessage(messageItem);
                break;
            case MsgType.News:
                messageBean = this.createNewsMessage(messageItem);
                break;
            case MsgType.File:
                messageBean = this.createFileMessage(messageItem);
                break;
            case MsgType.Image:
                messageBean = this.createPictureMessage(messageItem);
                break;
            case MsgType.VCard:
                messageBean = this.createVCardMessage(messageItem);
                break;
            case MsgType.PublicCard:
                messageBean = this.createPublicCardMessage(messageItem);
                break;
            case MsgType.Business:
                messageBean = this.createBusinessMessage(messageItem);
                break;
            case MsgType.AVMeetingInvite:
                messageBean = this.createAVMeetingInviteMessage(messageItem);
                break;
            case MsgType.MessageTypeAVGroupInvite:
                messageBean = this.createAVGroupInviteMessage(messageItem);
                break;
            default:
                break;
            }
            // 发送消息
            if (messageBean != null) {
                send(messageBean);
            }

        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param Name="param"></param>
    public void doSend(object param) {
        Thread.Sleep(200);
        Message message = (Message)param;
        String strFromClientId = App.AccountsModel.no;
        String strToClientId = message.getTo();
        message.setFrom(App.AccountsModel.no);
        message.setTimestamp(DateTimeHelper.getTimeStamp());
        String sendingMsg = message.createContentJsonStr();
        SendMessage sendMessage = new SendMessage();
        sendMessage.setFromClientId(strFromClientId);
        sendMessage.setToClientId(strToClientId);
        sendMessage.setMessageId(message.getMessageId());
        sendMessage.setMessage(sendingMsg);
        sendMessage.setMessageType(message.getType().GetHashCode());
        sendMessage.setTime(message.getTimestamp());


        //System.out.println("消息跟踪： 消息内容：" + sendMessage.toString());
        //如果给群发消息，先判断群的激活状态
        if (ToolsHelper.getChatSessionTypeByNo(strToClientId) == ChatSessionType.MUC) {
            MucTable table = MucDao.getInstance().FindGroupByNo(strToClientId);
            // 如果群未激活
            if (!table.activeFlag) {
                // 激活群处理
                this.waitForMucActiveMessages.Add(sendMessage);
                // 激活群
                MucServices.getInstance().RequestChangeMucStatus(table.mucId);
            } else {
                ImClientService.getInstance().sendMessage(sendMessage);
            }
        } else if (ToolsHelper.getChatSessionTypeByNo(strToClientId) == ChatSessionType.PUBLIC) {
            PublicAccountsTable dt= PublicAccountsService.getInstance().findByAppId(strToClientId);
            if(dt!=null) {
                if (dt.tenantNo != String.Empty) {
                    sendMessage.setTenantNo(dt.tenantNo);
                }
            }

            ImClientService.getInstance().sendMessage(sendMessage);

        } else {
            ImClientService.getInstance().sendMessage(sendMessage);
        }

        // 修改会话内容
        //ChatSessionService.getInstance().addChatSession(message);


    }

    /// <summary>
    /// 创建消息table
    /// </summary>
    /// <param Name="messageId"></param>
    /// <param Name="account"></param>
    /// <param Name="user"></param>
    /// <param Name="resource"></param>
    /// <param Name="text"></param>
    /// <param Name="content"></param>
    /// <param Name="Timestamp"></param>
    /// <param Name="delay_timestamp"></param>
    /// <param Name="type"></param>
    /// <param Name="incoming"></param>
    /// <param Name="read"></param>
    /// <param Name="flag"></param>
    /// <param Name="sent"></param>
    /// <param Name="error"></param>
    /// <returns></returns>
    private MessagesTable createMessagesTable(String messageId, String user
            , String resource, String text, String content, long timestamp
            , long delay_timestamp, String type, Boolean incoming, Boolean read
            , Boolean flag, int sent, Boolean error) {
        MessagesTable table = new MessagesTable();
        table.messageId = messageId;
        table.user = user;
        table.resource = resource;
        table.text = text;
        table.content = content;
        table.timestamp = timestamp.ToStr();
        table.delayTimestamp = delay_timestamp.ToStr();
        table.type = type;
        table.incoming = incoming;
        table.read = read;
        table.flag = flag;
        table.sent = sent.ToStr();
        table.error = error;

        ShowMessageDateTimeCtrl(table);
        return table;
    }

    /// <summary>
    /// 转发消息
    /// </summary>
    /// <param Name="sourceMessageId"></param>
    /// <param Name="to"></param>
    public void ForwardMessage(String sourceMessageId, String to) {
        try {
            Thread t = new Thread(new ThreadStart(() => {
                this.DoForwardMessage(sourceMessageId, to);
            }));
            t.IsBackground = true;
            t.Start();
        } catch (Exception ex) {
            Log.Error(typeof (MessageService), ex);
        }
    }

    /// <summary>
    /// 转发消息（实际执行）
    /// </summary>
    /// <param Name="sourceMessageId"></param>
    /// <param Name="to"></param>
    private void DoForwardMessage(String sourceMessageId, String to) {
        try {

            // 获取发送时间和消息编号
            long time = DateTimeHelper.getTimeStamp();
            String messageId = ImClientService.getInstance().generateMessageId();

            // 获取转发的消息
            MessagesTable table = MessageDao.getInstance().findByMessageId(sourceMessageId);
            if (table==null) {
                return;
            }
            // 重写发送时间、消息编号、发送人、发送给谁等信息
            table.messageId = messageId;
            table.serverMessageId = null;
            table.user = to;
            table.resource = App.AccountsModel.no;
            //table.text = text;
            //table.content = content;
            table.timestamp = time.ToStr();
            table.delayTimestamp = time.ToStr();
            //table.type = type;
            table.incoming = false;
            table.read = true;
            table.flag = true;
            table.sent = "0";
            table.error = false;
            table.atme = false;
            table.showTimestamp = true;

            // 根据转发消息的类型，做特殊的处理
            MsgType msgType = (MsgType)EnumHelper.getTypeByDescription(typeof(MsgType), table.type);
            switch (msgType) {
            // 收到聊天消息
            case MsgType.Text:
                break;
            //case MsgType.Cancel:
            //    break;
            case MsgType.At:
                // 转发@消息时、将类型变为普通的文本消息
                table.type = MsgType.Text.ToStr();
                break;
            case MsgType.Voice:
                break;
            case MsgType.Video:
                break;
            case MsgType.Location:
                break;
            case MsgType.News:
                break;
            case MsgType.File:
                break;
            case MsgType.Image:
                break;
            case MsgType.VCard:
                break;
            case MsgType.PublicCard:
                break;
            //case MsgType.Ticket:
            //    break;
            //case MsgType.Product:
            //    break;
            //case MsgType.Activity:
            //    break;
            case MsgType.Business:
                break;
            //case MsgType.App:
            //    break;
            default:
                break;
            }

            // 保存消息到本地数据库
            MessageDao.getInstance().save(table);

            // 转换并发送
            MessageItem messageItem = convertToMessItem(to, table);
            this.DoSendMessage(messageItem);

            // 通知画面刷新
            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.data = messageItem;
            businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }
    }

    /// <summary>
    /// 发送文本消息
    /// </summary>
    /// <param Name="account">登录帐号</param>
    /// <param Name="to">聊天对象</param>
    /// <param Name="resource">说话的人</param>
    /// <param Name="text">文本消息</param>
    /// <returns></returns>
    public MessageItem sendTextMessage(String to, String resource, String text) {
        try {
            long time = DateTimeHelper.getTimeStamp();
            String messageId = ImClientService.getInstance().generateMessageId();
            MessagesTable table = this.createMessagesTable(messageId, to
                                  , resource, text, "", time, time
                                  , MsgType.Text.ToStr(), false, true, true, 0, false);
            // 保存消息到本地数据库
            MessageDao.getInstance().save(table);
            return convertToMessItem(to, table);
        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }
        return null;
    }



    /// <summary>
    /// 发送图片消息
    /// </summary>
    /// <param Name="user">会话NO（人或群）</param>
    /// <param Name="resource">发送人NO</param>
    /// <param Name="imageStorageId">文件存储ID</param>
    /// <param Name="thumbnail">图片缩略图</param>
    /// <param Name="thumbnailWidth">宽</param>
    /// <param Name="thumbnailHeight">高</param>
    /// <returns></returns>
    public MessageItem sendPictureMessage(String messageId,String user, String imageStorageId,String thumbnail, int thumbnailWidth, int thumbnailHeight) {

        long time = DateTimeHelper.getTimeStamp();
        //String messageId = ImClientService.getInstance().generateMessageId();

        PictureMessage pictureMessage = new PictureMessage();
        pictureMessage.setImageStorageId(imageStorageId);
        pictureMessage.setThumbnail(thumbnail);
        pictureMessage.setThumbnailWidth(thumbnailWidth);
        pictureMessage.setThumbnailHeight(thumbnailHeight);


        MessagesTable table = this.createMessagesTable(messageId, user, App.AccountsModel.no, Constants.MESSAGE_FORMAT_TEXT_PE,
                              pictureMessage.toContent(), time, time, MsgType.Image.ToStr(), false,
                              true, true, 0, false);
        //存数据库
        MessageDao.getInstance().save(table);
        return convertToMessItem(user, table);
    }
    /// <summary>
    /// 图片消息Bean，@see TextMessage
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createPictureMessage(MessageItem messageItem) {
        Message messageBean = new PictureMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.Image);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 生成文本消息Bean，@see TextMessage
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createTextMessage(MessageItem messageItem) {
        Message messageBean = new TextMessage();
        ((TextMessage)messageBean).setText(messageItem.text);
        ((TextMessage)messageBean).setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.Text);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 发送撤回消息
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    public MessageItem sendCancelMessage(MessageItem messageItem) {

        //String messageId = ImClientService.getInstance().generateMessageId();
        // 撤销消息处理的时候，需要使用之前消息的ID
        String messageId = messageItem.messageId;

        //查找本地表
        MessagesTable table = MessageDao.getInstance().findByMessageId(messageItem.messageId);
        if (table == null) {
            table = this.createMessagesTable(messageId,
                                             messageItem.user, messageItem.resource,
                                             "你撤回了一条消息", "",
                                             DateTimeHelper.getLong(messageItem.timeDate), DateTimeHelper.getLong(messageItem.timeDate),
                                             MsgType.Notify.ToStr(), false, true, true, 0, false);
        } else {
            table.text = "你撤回了一条消息";
            table.content = "";
            table.type = MsgType.Notify.ToStr();
            table.sent = "0";
            table.error = false;
        }

        // 写入本地
        MessageDao.getInstance().save(table);

        // 注意，存储数据的时候，撤销消息的类型记录成Notify，便于画面UI显示。
        // 但是后续的业务处理还是需要把类型设置成Cancel处理
        table.type = MsgType.Cancel.ToStr();



        // 发送
        //send(messageBean);
        return convertToMessItem(messageItem.user, table);


    }

    /// <summary>
    /// 生成撤销消息Bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createCancelMessage(MessageItem messageItem) {
        // 生成消息Bean
        Message cancelMessage = new CancelMessage();
        cancelMessage.setTimestamp(DateTimeHelper.getLong(messageItem.timeDate));
        cancelMessage.setType(MsgType.Cancel);
        String severid = "";//刚发送出去的消息，若是没有serverid 要从DB 获取
        if (messageItem.serverMessageId == null) {
            MessagesTable messagesTable = MessageDao.getInstance().findByMessageId(messageItem.messageId);
            if (messagesTable != null) {
                severid = messagesTable.serverMessageId;
            }
        } else {
            severid = messageItem.serverMessageId;
        }
        ((CancelMessage)cancelMessage).setServerMessageId(severid);
        cancelMessage.setTo(messageItem.user);
        cancelMessage.setMessageId(messageItem.messageId);
        return cancelMessage;
    }
    public void sendReadMessage(String userNo) {


        Message message = new ReadMessage();
        String messageId = ImClientService.getInstance().generateMessageId();
        // 构建消息基础数据
        message.setMessageId(messageId);
        message.setType(MsgType.ReadMessage);
        message.setTo(App.AccountsModel.no);
        // 构建消息业务数据
        ((ReadMessage)message).setUserNo(userNo);
        ((ReadMessage)message).setSendTimestamp(DateTimeHelper.getTimeStamp());
        ((ReadMessage)message).setDevice("PC");
        // 发送消息
        MessageService.getInstance().doSend(message);
    }
    /// <summary>
    /// 发送@文本消息
    /// </summary>
    /// <param Name="account">登录帐号</param>
    /// <param Name="to">聊天对象</param>
    /// <param Name="resource">说话的人</param>
    /// <param Name="text">文本消息</param>
    /// <param Name="list">@的人员列表</param>
    /// <returns></returns>
    public MessageItem sendAtMessage( String to, String text, List<String> atNos) {
        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();

        AtMessage messageBean = new AtMessage();
        messageBean.setText(text);
        messageBean.setMessageId(messageId);
        messageBean.setAtNos(atNos);
        String strSenderGroupNickname = ContactsServices.getInstance().getMucMemberNameByNo(to, App.AccountsModel.no);
        messageBean.setSenderGroupNickname(strSenderGroupNickname);

        MessagesTable table = this.createMessagesTable(messageId, to, App.AccountsModel.no
                              , text, messageBean.createContentJsonStr(), time, time, MsgType.At.ToStr(), false, true, true, 0, false);
        // 写入本地
        MessageDao.getInstance().save(table);
        // 生成消息Bean
        //List<String> atNos = new List<String>();
        //for (Map<String, String> map : list) {
        //    Dictionary<String, String> map
        //    atNos.add(map.get("no"));
        //}
        //Message messageBean = createAtMessage(messageId, to, text, atNos);
        //messageBean.setMessageId(messageId);
        //// 发送
        //send(messageBean);

        return convertToMessItem(to, table);
    }

    /// <summary>
    /// 生成带@的消息Bean
    /// </summary>
    /// <param Name="messageId"></param>
    /// <param Name="to"></param>
    /// <param Name="text"></param>
    /// <param Name="atNos"></param>
    /// <returns></returns>
    private Message createAtMessage(MessageItem messageItem) {
        Message messageBean = new AtMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.At);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// TODO:发送语音消息，目前PC端不支持语音消息发送，保留方法备用
    /// 先将语音文件上传到服务器，拿到资源URL后
    /// MessageManager#onEventBackgroundThread(FileUploadedEvent) 中将消息发出去
    /// </summary>
    /// <param Name="account">登录帐号</param>
    /// <param Name="to">聊天对象</param>
    /// <param Name="resource">说话的人</param>
    /// <param Name="path">录音的路径</param>
    /// <returns></returns>
    public MessageItem sendVoiceMessage(String account, String to, String resource, String path) {
        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();

        VoiceMessage voiceMessage = new VoiceMessage();
        voiceMessage.setVoiceStorageId(string.Empty);
        // TODO:duration的计算未实现
        //long duration = VoiceManager.getVoiceManager(MyApplication.getInstance()).getVoiceLength(path);
        long duration = 0;
        voiceMessage.setDuration(duration);

        MessagesTable table = this.createMessagesTable(messageId, to, resource, Constants.MESSAGE_FORMAT_TEXT_VE,
                              voiceMessage.toContent(), time, time, MsgType.Voice.ToStr(), false, true, true, 0, false);
        // 写入本地
        MessageDao.getInstance().save(table);
        // TODO:记录文件
        // FileManager.getInstance().insert("", path, FileType.VOICE, 0, duration, messageId).getId();
        // TODO:异步上传语音文件
        //FileManager.getInstance().uploadFile(table.getMessageId(), path);
        return convertToMessItem(to, table);
    }

    /// <summary>
    /// 生成语音消息bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createVoiceMessage(MessageItem messageItem) {
        Message messageBean = new VoiceMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.Voice);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }


    public MessageItem SendAVGroupInviteMessage(String to, List<SelectPeopleSubViewModel> userCard, String roomId) {
        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();
        MessageTypeAVGroupInviteMessage avGroupInviteMessage = new MessageTypeAVGroupInviteMessage();
        // 构建消息业务数据
        avGroupInviteMessage.setUserName(App.AccountsModel.name);
        avGroupInviteMessage.setUserImId(App.AccountsModel.no);
        avGroupInviteMessage.setUserCard(userCard);
        avGroupInviteMessage.setRoomId(roomId);
        long timestamp = DateTimeHelper.getTimeStamp();
        avGroupInviteMessage.setTimestamp(timestamp);

        // 构建消息表
        String formatText = App.AccountsModel.nickname+Constants.GROUPMESSAGE_FORMAT_TEXT_VIDEO;


        MessagesTable table = this.createMessagesTable(messageId, to, App.AccountsModel.no, formatText,
                              avGroupInviteMessage.toContent(), time, time, MsgType.MessageTypeAVGroupInvite.ToStr(), false, true, true, 0, false);
        // 写入本地
        MessageDao.getInstance().save(table);
        return convertToMessItem(to, table);
    }


    /// <summary>
    /// 发送语音聊天或视频聊天邀请消息  单人
    /// </summary>
    /// <param name="to"></param>
    /// <param name="avType"></param>
    /// <param name="uId"></param>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public MessageItem SendAVMeetingInviteMessage(String to, AVMeetingType avType, uint uId, String roomId) {
        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();
        AVMeetingInviteMessage aVMeetingInviteMessage = new AVMeetingInviteMessage();
        // 构建消息业务数据
        aVMeetingInviteMessage.setAvType(avType.ToStr());
        aVMeetingInviteMessage.setUId(uId);
        aVMeetingInviteMessage.setRoomId(roomId);
        long timestamp = DateTimeHelper.getTimeStamp();
        aVMeetingInviteMessage.setTimestamp(timestamp);

        // 构建消息表
        String formatText;
        // 视频
        if (AVMeetingType.video == avType) {
            formatText = Constants.MESSAGE_FORMAT_TEXT_VIDEO;
        }
        // 语音
        else {
            formatText = Constants.MESSAGE_FORMAT_TEXT_AUDIO;
        }

        MessagesTable table = this.createMessagesTable(messageId, to, App.AccountsModel.no, formatText,
                              aVMeetingInviteMessage.toContent(), time, time, MsgType.AVMeetingInvite.ToStr(), false, true, true, 0, false);
        // 写入本地
        MessageDao.getInstance().save(table);
        return convertToMessItem(to, table);
    }


    /// <summary>
    /// 生成语音聊天或视频聊天邀请bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createAVGroupInviteMessage(MessageItem messageItem) {
        Message messageBean = new MessageTypeAVGroupInviteMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.MessageTypeAVGroupInvite);
        messageBean.setTo(messageItem.user);


        return messageBean;
    }


    /// <summary>
    /// 生成语音聊天或视频聊天邀请bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createAVMeetingInviteMessage(MessageItem messageItem) {
        Message messageBean = new AVMeetingInviteMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.AVMeetingInvite);
        messageBean.setTo(messageItem.user);

        //发出业务事件通知
        AVMeetingInviteEventData data = new AVMeetingInviteEventData();
        data.message = (AVMeetingInviteMessage)messageBean;
        data.callType = AVMeetingCallType.calling;

        //BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
        //businessEvent.data = data;
        //businessEvent.eventDataType = BusinessEventDataType.AVMeetingInvite;

        //EventBusHelper.getInstance().fireEvent(businessEvent);

        return messageBean;
    }

    /// <summary>
    /// 重新发送消息
    /// </summary>
    /// <param Name="messageItem"></param>
    public void ReSendMessage(MessageItem messageItem) {
        ThreadPool.QueueUserWorkItem(ReSend, messageItem);
    }

    /// <summary>
    /// 重新发送消息、发送时间等都不改变，原封不动发出去
    /// </summary>
    /// <param Name="messageItem"></param>
    private void ReSend(Object obj) {
        try {
            MessageItem messageItem = (MessageItem)obj;
            MessagesTable messageTable = MessageDao.getInstance().findByMessageId(messageItem.messageId);
            // 查询发送的消息
            //MessagesTable messageTable = MessageDao.getInstance().findByMessageId(msgId);
            //if (null == messageTable) {
            //    return;
            //}
            // 文件是否已经上传的标识
            Boolean isUpload = true;
            Message messageBean = null;
            long delaytime = DateTimeHelper.getTimeStamp();
            // 根据消息类型进行转换
            MsgType msgType = (MsgType)EnumHelper.getTypeByDescription(typeof(MsgType), messageItem.type);
            switch (msgType) {
            // 文本消息
            case MsgType.Text:
                TextMessage textMessage = new TextMessage();
                textMessage.setText(messageItem.text);
                messageBean = textMessage;
                break;
            case MsgType.At:
                AtMessage atMessage = new AtMessage().toModel(messageItem.content);
                messageBean = atMessage;
                break;
            // 语音消息，PC端重新发送语音消息？？
            case MsgType.Voice:
                // TODO 为啥语音消息不用判断是否上传成功？
                VoiceMessage voiceMessage = new VoiceMessage().toModel(messageItem.content);
                messageBean = voiceMessage;
                break;
            // 视频消息
            case MsgType.Video:
                VideoMessage videoMessage = new VideoMessage().toModel(messageItem.content);
                if (string.IsNullOrEmpty( videoMessage.getVideoStorageId())) {
                    isUpload = false;
                } else {
                    isUpload = true;
                }
                messageBean = videoMessage;
                break;
            // 文件消息
            case MsgType.File:
                FileMessage fileMessage = new FileMessage().toModel(messageItem.content);
                if (fileMessage.getFileStorageId()==string.Empty) {
                    isUpload = false;
                } else {
                    isUpload = true;
                }
                messageBean = fileMessage;
                break;
            // 图片消息
            case MsgType.Image:

                PictureMessage pictureMessage = new PictureMessage().toModel(messageTable.content);
                if (pictureMessage.getImageStorageId() == null || "".Equals(pictureMessage.getImageStorageId())) {
                    isUpload = false;
                } else {
                    isUpload = true;
                }
                messageBean = pictureMessage;
                break;
            //位置消息
            case MsgType.Location:
                LocationMessage locationMessage = new LocationMessage().toModel(messageItem.content);
                messageBean = locationMessage;
                break;
            // 名片消息
            case MsgType.VCard:
                VcardMessage vcardMessage = new VcardMessage().toModel(messageItem.content);
                messageBean = vcardMessage;
                break;
            // 公众号名片
            case MsgType.PublicCard:
                PublicCardMessage publicCardMessage = new PublicCardMessage().toModel(messageItem.content);
                messageBean = publicCardMessage;
                break;
            // 业务消息
            case MsgType.Business:
                BusinessMessage businessMessage = new BusinessMessage().toModel(messageItem.content);
                messageBean = businessMessage;
                break;
            default:
                break;
            }

            messageBean.setMessageId(messageItem.messageId);
            messageBean.setTimestamp(long.Parse(messageItem.timestamp));
            messageBean.setTo(messageItem.user);
            messageBean.setType(msgType);

            // 如果需要上传的消息，文件上传已经完成，但是只是发送失败,则直接发送
            if (isUpload) {
                send(messageBean);
            } else {

                // 更新消息发送状态
                messageTable.sent = "0";
                MessageDao.getInstance().save(messageTable);
                // 获取上传文件的本地路径
                FilesTable locafile = FilesDao.getInstance().findByFileOwner(messageTable.messageId);
                // 判断文件是否存在，如果有则加载文件
                if (!File.Exists(locafile.localpath)) {
                    //
                    //发送event事件
                    BusinessEvent<Object> businessEventLocalFileIsNotExist = new BusinessEvent<Object>();
                    businessEventLocalFileIsNotExist.data = messageItem;
                    businessEventLocalFileIsNotExist.eventDataType = BusinessEventDataType.LocalFileIsNotExistEvent;
                    EventBusHelper.getInstance().fireEvent(businessEventLocalFileIsNotExist);
                    return;
                }

                // 文件存在，继续处理
                // 定义上传变量

                UploadFileType uploadFileType = UploadFileType.UNKNOW;
                // 判断上传类型
                switch (msgType) {
                case MsgType.Video: //上传视频
                    uploadFileType = UploadFileType.MSG_VIDEO;
                    break;
                case MsgType.File: //上传文件
                    uploadFileType = UploadFileType.MSG_FILE;
                    break;
                case MsgType.Image:
                    uploadFileType = UploadFileType.MSG_IMAGE;

                    break;
                }
                // 执行上传
                if (uploadFileType != UploadFileType.UNKNOW) {
                    Dictionary<String, Object> param = new Dictionary<string, object>();
                    param.Add("item", messageItem);
                    UploadFileService uploadFileService = new UploadFileService(uploadFileType, messageItem.messageId, locafile.localpath, param);
                    uploadFileService.uploadAsync();
                }
            }
            //数据库层更新发送时间,用于长按撤回
            updateDelayTime(messageItem.messageId, delaytime);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 群激活后，发送第一条消息
    /// </summary>
    /// <param Name="mucNo"></param>
    public void sendMucMessageAfterActive(String mucNo) {
        int location = -1;
        for (int i = 0; i < waitForMucActiveMessages.Count; i++) {
            if (waitForMucActiveMessages[i].getToClientId().Equals(mucNo)) {
                location = i;
                break;
            }
        }
        if (location > -1) {
            ImClientService.getInstance().sendMessage(waitForMucActiveMessages[location]);
            waitForMucActiveMessages.RemoveAt(location);
        }
    }

    /// <summary>
    /// 发送位置消息
    /// </summary>
    /// <param Name="account">登录帐号</param>
    /// <param Name="to">聊天对象</param>
    /// <param Name="latitude">纬度</param>
    /// <param Name="longitude">经度</param>
    /// <param Name="address">地址</param>
    /// <param Name="path">地图截图本地路径</param>
    /// <returns></returns>
    public MessageItem sendLocationMessage(String account, String to, double latitude, double longitude, String
                                           address, String path) {
        // 生成位置消息的JSON结构，存在MessageTable的content字段
        LocationMessage locationMessage = new LocationMessage();
        locationMessage.setType(MsgType.Location);
        locationMessage.setAddress(address);
        locationMessage.setLongitude(longitude);
        locationMessage.setLatitude(latitude);
        locationMessage.setScale(0);
        locationMessage.setTo(to);

        //生成缩略图
        Bitmap bitmap = null;
        try {
            // TODO:暂时无缩略图的共通处理
            // bitmap = GraphicHelper.compressImage(path, 400);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        //TODO:暂时无图片的处理
        //locationMessage.setPicture(GraphicHelper.bitmapToBase64(bitmap));

        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();
        String content = locationMessage.toContent();

        MessagesTable table = this.createMessagesTable(messageId, to, account, locationMessage.getAddress(),
                              content, time, time, MsgType.Location.ToStr(), false, true, true, 0, false);

        // 写入本地
        MessageDao.getInstance().save(table);

        // 生成消息Bean
        Message messageBean = locationMessage;

        messageBean.setMessageId(messageId);
        // 发送
        send(messageBean);
        //        //上传那图片
        //        FileManager.getInstance().uploadMessageFile(fileTableId, path, table.getMessageId());
        return convertToMessItem(to, table);
    }

    /// <summary>
    /// 生成位置消息bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createLocationMessage(MessageItem messageItem) {
        Message messageBean = new LocationMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.Location);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 发送公众号名片消息
    /// </summary>
    /// <param Name="user"></param>
    /// <param Name="publicAccountsTable"></param>
    /// <returns></returns>
    public MessageItem sendPublicCardMessage(string appId, String to) {

        PublicAccountsTable publicAccountsTable = PublicAccountsService.getInstance().findByAppId(appId);
        if (publicAccountsTable == null) return null;
        PublicCardMessage publicCardMessage = new PublicCardMessage();
        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();
        publicCardMessage.setTo(to);
        publicCardMessage.setType(MsgType.PublicCard);
        publicCardMessage.setMessageId(messageId);
        // id
        publicCardMessage.setAppId(publicAccountsTable.appid);
        // 名称
        publicCardMessage.setName(publicAccountsTable.name);
        // 头像
        publicCardMessage.setLogoId(publicAccountsTable.logoId);
        // 功能介绍
        publicCardMessage.setIntroduction(publicAccountsTable.introduction);
        // 帐号主体
        publicCardMessage.setOwnerName(publicAccountsTable.ownerName);

        String content = publicCardMessage.toContent();
        MessagesTable table = this.createMessagesTable(messageId, to, "", Constants.MESSAGE_FORMAT_TEXT_PD,
                              content, time, time, MsgType.PublicCard.ToStr(), false, true, true, 0, false);
        // 写入本地
        MessageDao.getInstance().save(table);
        //send(publicCardMessage);
        return convertToMessItem(to, table);
    }

    /// <summary>
    /// 公众号名片消息Bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createPublicCardMessage(MessageItem messageItem) {
        Message messageBean = new PublicCardMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.PublicCard);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 发送文件消息
    /// </summary>
    /// <param Name="user"></param>
    /// <param Name="resource"></param>
    /// <param Name="path"></param>
    /// <param Name="filename"></param>
    /// <param Name="filesize"></param>
    /// <returns></returns>
    public MessageItem sendFileMessage(String messageId,String user, String resource, String path,
                                       String filename, long filesize) {

        long time = DateTimeHelper.getTimeStamp();
        //String messageId = ImClientService.getInstance().generateMessageId();

        FileMessage fileMessage = new FileMessage();
        fileMessage.setTo(user);
        fileMessage.setType(MsgType.File);
        fileMessage.setMessageId(messageId);
        // fileName
        fileMessage.setFileName(filename);
        // fileSize
        fileMessage.setFileSize(filesize);
        // storageId
        fileMessage.setFileStorageId(string.Empty);
        String content = fileMessage.toContent();
        MessagesTable table = this.createMessagesTable(messageId, user, resource, fileMessage.getFileName(),
                              content, time, time, MsgType.File.ToStr(), false, true, true, 0, false);
        MessageDao.getInstance().save(table);

        // TODO ：这个没写完，应该是在上传完成的事件处理中发送消息
        //将视频记录存入File表中
        // FileManager.getInstance().insert("", path, FileType.FILE, filesize, 0, messageId).getId();
        //上传视频
        // FileManager.getInstance().uploadFile(table.getMessageId(), path);

        return convertToMessItem(user, table);
    }

    /// <summary>
    /// 文件消息Bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createFileMessage(MessageItem messageItem) {
        Message messageBean = new FileMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.File);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 发送网盘中的文件消息
    /// </summary>
    /// <param Name="user"></param>
    /// <param Name="resource"></param>
    /// <param Name="path"></param>
    /// <param Name="filename"></param>
    /// <param Name="filesize"></param>
    /// <returns></returns>
    public MessageItem sendDiskFileMessage(String user, String resource, string fileStorageId,
                                           String filename, long filesize) {

        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();

        FileMessage fileMessage = new FileMessage();

        fileMessage.setTo(user);
        fileMessage.setType(MsgType.File);
        fileMessage.setMessageId(messageId);
        // fileName
        fileMessage.setFileName(filename);
        // fileSize
        fileMessage.setFileSize(filesize);
        // storageId
        fileMessage.setFileStorageId(fileStorageId);
        String content = fileMessage.toContent();
        MessagesTable table =this.createMessagesTable(messageId, user, resource, fileMessage.getFileName(),
                             content, time, time, MsgType.File.ToStr(), false, true, true, 0, false);
        MessageDao.getInstance().save(table);
        //下载
        Dictionary<String, Object> extrasFile = new Dictionary<String, Object>();
        extrasFile.Add("fileMessage", fileMessage);
        extrasFile.Add(DownloadServices.DOWNLOADFILENAME_KEY, fileMessage.fileName);
        extrasFile.Add(DownloadServices.MESSAGEID_KEY, messageId);
        DownloadServices.getInstance().DownloadMethod(fileMessage.getFileStorageId(), DownloadType.MSG_FILE, extrasFile);
        return convertToMessItem(user, table);
    }

    /**
     * 发送名片消息为登录者本人的处理
     *
     * @param user     名片发送给谁
     * @param sentUser 被发送者的用户
     * @return
     */
    public MessageItem sendVCardAccountMessage(String user) {

        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();

        VcardMessage vcardMessage = new VcardMessage();
        vcardMessage.setTo(user);
        vcardMessage.setType(MsgType.VCard);
        vcardMessage.setMessageId(messageId);

        AccountsTable accountsTable = AccountsDao.getInstance().findByNo(App.AccountsModel.no);
        // id
        vcardMessage.setUserId(long.Parse(accountsTable.clientuserId.ToStr()));
        // No
        vcardMessage.setUserNo(accountsTable.no);
        // 昵称
        vcardMessage.setNickname(accountsTable.nickname);
        // 性别
        vcardMessage.setGender(accountsTable.gender);
        // 头像地址
        if (accountsTable.avatarStorageRecordId!=null) {
            vcardMessage.setAvatarStorageId(accountsTable.avatarStorageRecordId);
        }
        // 国家
        vcardMessage.setCountry(accountsTable.country);
        // 省份
        vcardMessage.setProvince(accountsTable.province);
        // 城市
        vcardMessage.setCity(accountsTable.city);
        // 签名
        vcardMessage.setMoodMessage(accountsTable.desc);

        String content = vcardMessage.toContent();
        MessagesTable table = this.createMessagesTable(messageId,  user, "", Constants.MESSAGE_FORMAT_TEXT_VD,
                              content, time, time, MsgType.VCard.ToStr(), false, true, true, 0, false);
        MessageDao.getInstance().save(table);
        //send(vcardMessage);
        return convertToMessItem(user, table);
    }


    /// <summary>
    /// 发送名片消息（非登录者本人）
    /// </summary>
    /// <param Name="user">名片发送给谁</param>
    /// <param Name="sendUser">被发送者的用户</param>
    /// <returns></returns>
    public MessageItem sendVCardMessage(String user, String sendUser) {
        long time = DateTimeHelper.getTimeStamp();
        String messageId = ImClientService.getInstance().generateMessageId();

        VcardMessage vcardMessage = new VcardMessage();
        vcardMessage.setTo(user);
        vcardMessage.setType(MsgType.VCard);
        vcardMessage.setMessageId(messageId);

        VcardsTable vcardsTable = VcardsDao.getInstance().findByNo(sendUser);
        if (vcardsTable!=null) {
            // id
            vcardMessage.setUserId(long.Parse(vcardsTable.clientuserId));
            // No
            vcardMessage.setUserNo(vcardsTable.no);
            // 昵称
            vcardMessage.setNickname(vcardsTable.nickname);
            // 性别
            vcardMessage.setGender(vcardsTable.gender);
            // 头像地址
            if (vcardsTable.avatarStorageRecordId != null) {
                vcardMessage.setAvatarStorageId(vcardsTable.avatarStorageRecordId);
            }
            // 国家
            vcardMessage.setCountry(vcardsTable.country);
            // 省份
            vcardMessage.setProvince(vcardsTable.province);
            // 城市
            vcardMessage.setCity(vcardsTable.city);
            // 签名
            vcardMessage.setMoodMessage(vcardsTable.desc);
        } else {
            List<OrganizationMemberTable> list= OrganizationMemberDao.getInstance().FindOrganizationMemberByUserNo(sendUser);
            if (list != null && list.Count > 0) {
                OrganizationMemberTable organizationMemberTable = list[0];
                // id
                vcardMessage.setUserId(long.Parse(organizationMemberTable.userId));
                // No
                vcardMessage.setUserNo(organizationMemberTable.no);
                // 昵称
                vcardMessage.setNickname(organizationMemberTable.nickname);
                // 性别
                //vcardMessage.setGender(organizationMemberTable.gender);
                // 头像地址
                if (organizationMemberTable.avatarId != null) {
                    vcardMessage.setAvatarStorageId(organizationMemberTable.avatarId);
                }
                // 国家
                //vcardMessage.setCountry(organizationMemberTable.country);
                // 省份
                //vcardMessage.setProvince(organizationMemberTable.province);
                // 城市
                //vcardMessage.setCity(organizationMemberTable.city);
                // 签名
                //vcardMessage.setMoodMessage(organizationMemberTable.desc);
            }
        }


        String content = vcardMessage.toContent();
        MessagesTable table = this.createMessagesTable(messageId, user, "", Constants.MESSAGE_FORMAT_TEXT_VD,
                              content, time, time, MsgType.VCard.ToStr(), false, true, true, 0, false);
        MessageDao.getInstance().save(table);
        //send( vcardMessage);
        return convertToMessItem(user, table);
    }

    /// <summary>
    /// 名片消息Bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createVCardMessage(MessageItem messageItem) {
        Message messageBean = new VcardMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.VCard);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// TODO:发送视频消息，PC端发送视频消息的时候，需要改造该方法
    /// </summary>
    /// <param Name="account"></param>
    /// <param Name="user"></param>
    /// <param Name="resource"></param>
    /// <param Name="imagePath"></param>
    /// <param Name="path"></param>
    /// <param Name="filename"></param>
    /// <param Name="filesize"></param>
    /// <returns></returns>
    public MessageItem sendVideoMessage(String account, String user, String resource, String imagePath, String path,
                                        String filename, long filesize) {

        //long time = DateTimeHelper.getTimeStamp();
        //String messageId = ImClientService.getInstance().generateMessageId();

        //Bitmap bitmap = null;
        //try
        //{
        //    bitmap = GraphicHelper.compressImage(imagePath, 400);
        //    if (bitmap == null)
        //    {
        //        return null;
        //    }
        //    bitmap = GraphicHelper.imageZoom(bitmap, 20);
        //}
        //catch (Exception e)
        //{
        //    Log.Error(typeof(MessageService), e);
        //}
        //int thumbnailWidth = 0;
        //int thumbnailHeight = 0;
        //if (null != bitmap)
        //{
        //    thumbnailWidth = bitmap.getWidth();
        //    thumbnailHeight = bitmap.getHeight();
        //}

        //VideoMessage videoMessage = new VideoMessage();
        //videoMessage.setVideoStorageId(-1L);
        //videoMessage.setFileName(filename);
        //videoMessage.setFileSize(filesize);
        //long duration = VoiceManager.getVoiceManager(MyApplication.getInstance()).getVoiceLength(path);
        //videoMessage.setDuration(duration);
        //videoMessage.setFirstFrame(GraphicHelper.bitmapToBase64(bitmap));
        //videoMessage.setFirstFrameWidth(thumbnailWidth);
        //videoMessage.setFirstFrameHeight(thumbnailHeight);

        //MessagesTable table = this.createMessagesTable(messageId, user, resource, Constants.MESSAGE_FORMAT_TEXT_VO,
        //                      videoMessage.toContent(), time, time, MsgType.Video.ToStr(), false,
        //                      true, true, 0, false);

        //MessageDao.getInstance().save(table);

        //// TODO：真正的消息发送应该放在图片上传完成的事件中来处理
        ////将视频记录存入File表中
        //FileManager.getInstance().insert("", path, FileType.VEDIO, 0, duration, messageId).getId();
        ////上传视频
        //FileManager.getInstance().uploadFile(table.getMessageId(), path);
        //return convertToMessItem(user, table);
        return null;
    }

    /// <summary>
    /// 视频消息Bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createVideoMessage(MessageItem messageItem) {
        Message messageBean = new VideoMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.Video);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 生成多图文消息bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createNewsMessage(MessageItem messageItem) {
        Message messageBean = new NewsMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.News);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    /// 生成业务消息bean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private Message createBusinessMessage(MessageItem messageItem) {
        Message messageBean = new BusinessMessage().toModel(messageItem.content);
        messageBean.setMessageId(messageItem.messageId);
        messageBean.setType(MsgType.Business);
        messageBean.setTo(messageItem.user);
        return messageBean;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param Name="user"></param>
    /// <param Name="dbModel"></param>
    /// <returns></returns>
    public MessageItem MessageTableToMessageItem(String user, MessagesTable dbModel) {
        MessageItem messageItem = null;
        try {
            messageItem = new MessageItem();
            messageItem.tenantNo = dbModel.tenantNo;
            messageItem.error = dbModel.error;
            messageItem.id = dbModel.id;
            messageItem.messageId = dbModel.messageId;
            messageItem.incoming = dbModel.incoming;
            messageItem.read = dbModel.read; ;
            messageItem.flag = dbModel.flag;
            messageItem.sent = dbModel.sent;
            messageItem.text = dbModel.text;
            messageItem.content = dbModel.content;
            messageItem.type = dbModel.type;
            messageItem.user = user;
            messageItem.serverMessageId = dbModel.serverMessageId;
            messageItem.showTimestamp= dbModel.showTimestamp;
            String avatarStorageId = "";
            String userName = "";
            String resource = dbModel.resource;
            //公众号
            if (ToolsHelper.getChatSessionTypeByNo(resource) == ChatSessionType.PUBLIC) {
                // TODO:公众号的处理没写完
                //PublicAccountsTable publicAccountsTable = PublicAccountManager.getInstance().findByAppId(resource);
                //if (null != publicAccountsTable) {
                //    userName = publicAccountsTable.getName();
                //    avatarStorageId = publicAccountsTable.getLogoid();
                //}
            }
            // 群聊
            else if (ToolsHelper.getChatSessionTypeByNo(user) == ChatSessionType.MUC) {
                // 缓存读取标志
                Boolean isReadFromCache = false;
                // TODO:读取缓存的处理目前没有
                /*
                GroupMemberCacheEntity groupMemberCacheEntity = CacheManager.getInstance().getGroupMember(user, resource);
                if (groupMemberCacheEntity != null) {
                    VcardCacheEntity vcardCacheEntity = groupMemberCacheEntity.getVcard();
                    String nickName = null;
                    if (vcardCacheEntity != null) {
                        // 处理头像
                        avatarStorageId = vcardCacheEntity.getAvatarId();
                        isReadFromCache = true;
                        nickName = groupMemberCacheEntity.getVcard().getNickName();
                    }
                    //优先显示修改后的备注姓名
                    if (!ToolsHelper.isNull(nickName)) {
                        userName = nickName;
                    } else {
                        userName = groupMemberCacheEntity.getName();
                    }
                }
                */
                // 缓存不存在，继续数据库查询处理方式
                if (!isReadFromCache) {
                    MucTable mucTable = MucDao.getInstance().FindGroupByNo(user);
                    if (mucTable != null) {
                        // 处理头像
                        MucMembersTable member = MucMembersDao.getInstance().findByMucIdAndMemberNo(mucTable.mucId, resource);
                        if (null != member) {
                            avatarStorageId = member.avatarStorageRecordId;
                        } else {
                            VcardsTable vcardsTable = VcardsDao.getInstance().findByNo(resource);
                            if (null != vcardsTable) {
                                avatarStorageId = vcardsTable.avatarStorageRecordId;
                            }
                        }
                        // 处理名字
                        userName = ContactsServices.getInstance().getMucMemberNameByNo(user, resource);
                    }
                }
            }
            //单聊
            else {
                //TODO: 读取缓存处理未实现
                /*
                VcardCacheEntity vcardCacheEntity = CacheManager.getInstance().getVcard(resource);
                if (vcardCacheEntity != null) {
                    avatarStorageId = vcardCacheEntity.getAvatarId();
                    userName = vcardCacheEntity.getNickName();
                } else {
                    // 缓存不存在，继续数据库查询处理方式
                     VcardsTable vcard = VcardsDao.getInstance().findByNo(resource);
                    if (null != vcard) {
                        avatarStorageId = vcard.AvatarStorageRecordId;
                    }
                    userName = ContactsServices.getInstance().getContractNameByNo(resource);
                }
                */

                // 缓存不存在，继续数据库查询处理方式
                VcardsTable vcard = VcardsDao.getInstance().findByNo(resource);
                if (null != vcard) {
                    avatarStorageId = vcard.avatarStorageRecordId;
                }
                userName = ContactsServices.getInstance().getContractNameByNo(resource);
            }

            messageItem.resource = resource;
            messageItem.name = userName;
            messageItem.avatar = avatarStorageId;
            messageItem.timeDate = DateTimeHelper.getDate(dbModel.timestamp);
            messageItem.delayTimeDate = DateTimeHelper.getDate(dbModel.delayTimestamp);
            messageItem.timestamp = dbModel.timestamp;
            messageItem.displayTimeFlag = dbModel.showTimestamp;//是否显示时间戳
        } catch (Exception ex) {
            Log.Error(typeof(MessageService), ex);
        }
        return messageItem;
    }
    /// <summary>
    /// 填充检索的历史消息
    /// </summary>
    /// <param Name="account"></param>
    /// <param Name="user"></param>
    /// <param Name="dbModels"></param>
    /// <param Name="list"></param>
    /// <returns></returns>
    private List<MessageItem> setMessages(String user, List<MessagesTable> dbModels, List<MessageItem> list) {
        foreach (MessagesTable dbModel in dbModels) {
            list.Add(this.MessageTableToMessageItem(user,dbModel));
        }
        return list;
    }

    /// <summary>
    /// 分页检索历史消息
    /// </summary>
    /// <param Name="account"></param>
    /// <param Name="user"></param>
    /// <param Name="lastTimestamp">最后一条历史消息时间戳</param>
    /// <returns></returns>
    public List<MessageItem> getMessagesByPage(String user, long lastTimestamp) {
        List<MessageItem> list = new List<MessageItem>();

        try {

            List<MessagesTable> dbModels = MessageDao.getInstance().findMessagesByPage(user, lastTimestamp);
            if (null == dbModels) {
                return list;
            }
            //填充数据
            list = setMessages(user, dbModels, list);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

        //倒序排列；
        // Collections.reverse(list);

        //发送event事件
        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
        businessEvent.data = list;
        businessEvent.eventDataType = BusinessEventDataType.GetMessagesEvent;
        EventBusHelper.getInstance().fireEvent(businessEvent);
        //EventBus.getDefault().post(new GetMessagesEvent(list, 1));

        return list;
    }

    public MessagesTable findLastAtMessagesByUser(string user) {
        try {
            MessagesTable  dbModels = MessageDao.getInstance().findLastAtMessagesByUser(user);
            return dbModels;
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
            return null;
        }
    }

    public MessagesTable findLastAppMessages() {
        try {
            MessagesTable dbModels = MessageDao.getInstance().findLastAppMessages();
            return dbModels;
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
            return null;
        }
    }
    /// <summary>
    /// TODO 这个是简单粗暴的临时写法。将来考虑如何优化，一次全查出来有点豪放啊
    /// TODO 将来做会话的图片查看也有用
    /// 查询某个会话的全部图片消息
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public List<MessageItem> findAllImageMessagesByUser(String user) {
        List<MessageItem> list = new List<MessageItem>();

        try {

            List<MessagesTable> dbModels = MessageDao.getInstance().findAllImageMessagesByUser(user);
            if (null == dbModels) {
                return list;
            }
            //填充数据
            list = setMessages(user, dbModels, list);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

        //倒序排列；
        // Collections.reverse(list);

        //发送event事件
        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
        businessEvent.data = list;
        businessEvent.eventDataType = BusinessEventDataType.GetMessagesEvent;
        EventBusHelper.getInstance().fireEvent(businessEvent);
        //EventBus.getDefault().post(new GetMessagesEvent(list, 1));

        return list;
    }


    /// <summary>
    /// 分页检索历史消息
    /// </summary>
    /// <param Name="account"></param>
    /// <param Name="user"></param>
    /// <param Name="lastTimestamp">最后一条历史消息时间戳</param>
    /// <param Name="targetTimestamp">第一条未读消息的时间戳</param>
    /// <returns></returns>
    public List<MessageItem> getAllUnreadMessage(String user, long lastTimestamp, long targetTimestamp) {
        List<MessageItem> list = new List<MessageItem>();
        try {

            String strSql = "SELECT * FROM messages WHERE user = '" + user + "' and Timestamp < '" + lastTimestamp + "' and Timestamp >= '" + targetTimestamp
                            + "'  ORDER BY Timestamp DESC ";
            List<MessagesTable> dbModels = MessageDao.getInstance().findAllUnreadMessage(user, lastTimestamp, targetTimestamp);
            if (null == dbModels) {
                return list;
            }
            //填充数据
            list = setMessages(user, dbModels, list);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }

        //倒序排列；
        // Collections.reverse(list);

        // 增加分割控件
        MessageItem item = this.getCopyNewItem(list[list.Count-1]);

        list.Add(item);

        //发送event事件

        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
        businessEvent.data = list;
        businessEvent.eventDataType = BusinessEventDataType.GetMessagesEvent;
        EventBusHelper.getInstance().fireEvent(businessEvent);

        //EventBus.getDefault().post(new GetMessagesEvent(list, 0));
        return list;
    }

    public void DeleteByTenantNo(string tenantNo) {
        try {
            MasterDao.getInstance().DeleteByTenantNo(tenantNo);
            OrganizationDao.getInstance().DeleteByTenantNo(tenantNo);
            OrganizationMemberDao.getInstance().DeleteByTenantNo(tenantNo);
            PublicAccountsDao.getInstance().DeleteByTenantNo(tenantNo);
            PublicWebDao.getInstance().DeleteByTenantNo(tenantNo);
            ThirdAppClassDao.getInstance().DeleteByTenantNo(tenantNo);
            ThirdAppGroupDao.getInstance().DeleteByTenantNo(tenantNo);
            TodoTaskDao.getInstance().DeleteByTenantNo(tenantNo);

        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }
    /// <summary>
    /// 复制前一条
    /// 在聊天界面，点击查看所有未读消息时，插入一条提示信息用
    /// </summary>
    /// <param Name="itemFrom"></param>
    /// <returns></returns>
    public MessageItem getCopyNewItem(MessageItem itemFrom) {
        MessageItem item = new MessageItem();
        /** 说话人姓名 */
        item.name = itemFrom.name;
        /** 说话人头像 */
        item.avatar = itemFrom.avatar;;
        /** 说话人头像本地地址 */
        item.localAvatar = itemFrom.localAvatar;
        item.timeDate = itemFrom.timeDate;
        //Item.delayTimeDate = Item.delayTimeDate;
        /** 显示时间标识 */
        //Item.displayTimeFlag = Item.displayTimeFlag;
        /**服务器端消息ID*/
        item.messageId = "1001";
        /**消息来源（好友或群组帐号）,与后台no'字段对应*/
        item.user = itemFrom.user;
        /**说话的人（好友帐号）,与后台no'字段对应*/
        item.resource = itemFrom.resource;
        /**格式化显示文字*/
        item.text = "  ──────  以下为新消息  ──────  ";
        /**音频、视频、多图文等消息的消息体，JSON格式，客户端自行封装，与服务器格式无关*/
        item.content = itemFrom.content;
        /**时间*/
        item.timestamp = itemFrom.timestamp;
        /**延迟时间*/
        //Item.delayTimestamp = Item.delayTimestamp;
        /**MsgType*/
        item.type = MsgType.Notify.ToStr();
        /**是否是收到的消息*/
        item.incoming = itemFrom.incoming;
        /**消息已读/未读*/
        item.read = true;
        /**
         * 音频/视频是否播放
         * 语音消息标示是否点击收听   视频消息时 标示是否下载
         */
        item.flag = itemFrom.flag;
        /**
         * -1:失败；0：发送中；1：已发送
         * 发送消息时标示发送状态   接收消息时  标示下载状态  ==0时下载中   ==1时未下载
         */
        item.sent = "1";
        /**消息解析错误*/
        item.error = false;
        /**
         * 消息是否是@我
         */
        item.atme = false;
        return item;
    }

    /// <summary>
    /// 清空某用户消息记录
    /// </summary>
    /// <param Name="user">对象no</param>
    public void clearMessages(String user) {
        this.clearMessages(user, false);
    }
    /// <summary>
    /// 清空某用户消息记录
    /// </summary>
    /// <param Name="user">对象no</param>
    /// <param Name="delflag">删除记录标识</param>
    public void clearMessages(String user, Boolean delflag) {
        try {
            if (delflag) {
                ChatSessionService.getInstance().deleteChatSessionByNo(user);
            } else {
                //判断user开头是否为SA公众号
                if (ToolsHelper.getChatSessionTypeByNo(user) == ChatSessionType.PUBLIC) {
                    ChatSessionService.getInstance().updateLastMessageByNo(user,null, "", 0, "");//制空处理
                } else {
                    ChatSessionService.getInstance().updateLastMessageByNo(user,null, "", DateTimeHelper.getTimeStamp(), "");//制空处理
                }

            }

            // TODO 查找所有多媒体消息  方便删除本地文件
            //List<MessagesTable> t = DbHelper.getDbUtils().findAll(Selector.from(MessagesTable.class).where("Account", "=", getAccount()).and("user", "=", user).and(WhereBuilder.b("type", "=", MsgType.Video.Name()).or("type", "=", MsgType.Voice.Name()).or("type", "=", MsgType.Image.Name())));
            //删除所有消息表相关消息
            MessageDao.getInstance().deleteByUser(user);


            // TODO 删除本地的聊天文件
            /*
            if (null != t) {
                for (MessagesTable ta : t) {
                    if (ta.isIncoming() && MsgType.Image.Name().equals(ta.getType())) {//清除所有收到的图片文件缓存  发送的图片文件不用清除
                        BitmapUtils bitmapUtils = BitmapHelper.getBitmapUtils(null);
                        PictureMessage pictureMessage = new PictureMessage().toModel(ta.getContent());
                        bitmapUtils.clearCache(Constants.getCoreUrls().getDownloadUrl(pictureMessage.getImageStorageId()) + "?type=original");
                    }
                    //根据messageid查找对应文件
                    FilesTable file = DbHelper.getDbUtils().findFirst(Selector.from(FilesTable.class).where("owner", "=", ta.getMessageId()));
                    if (null == file)
                        continue;
                    if (!FileType.IMAGE.equals(file.getFileType())) {//图片消息已经处理过了  这里不再处理
                        FileHelper.deleteFile(file.getPath());//根据path删除本地文件
                    }
                    DbHelper.getDbUtils().delete(file);//清除files表相关数据
                }
            }
            */
            //通知界面修改
            //更新chartsession
            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            businessEvent.data = user;
            businessEvent.eventDataType = BusinessEventDataType.ChatDetailedChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);

        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /**
     * 删除某条消息记录
     *
     * @param messageId
     */
    public void deleteByMessageId(String messageId) {
        try {
            MessagesTable messageTable =  MessageDao.getInstance().findByMessageId(messageId);
            if (messageTable != null) {
                ChatSessionTable chatSessionTable = ChatSessionService.getInstance().findByNo(messageTable.user);
                if (chatSessionTable != null) {
                    MessageDao.getInstance().deleteByMessageId(messageId);
                    // 如果删除的是最后一条消息，需要更新会话，否则会话表不用处理
                    if (messageId.Equals(chatSessionTable.messageId)) {
                        MessagesTable lastMessageTable = MessageDao.getInstance().findLastMessagesByUser(messageTable.user);
                        String userNo = messageTable.user;
                        String resource = "";
                        String lastMessage = "";
                        long time = DateTimeHelper.getTimeStamp();
                        String msgId = "";
                        if (lastMessageTable != null) {
                            resource = lastMessageTable.resource;
                            lastMessage = lastMessageTable.text;
                            time = long.Parse(lastMessageTable.timestamp);
                            msgId = lastMessageTable.messageId;
                        }
                        ChatSessionService.getInstance().updateLastMessageByNo(userNo, resource, lastMessage, time, msgId);
                    }

                }

            }
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// 刚登陆系统时，把所有的消息状态是发送中的改为发送失败
    /// </summary>
    public void setAllSentFlagError() {
        try {
            MessageDao.getInstance().setAllSentFlagError();
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
    }

    /// <summary>
    /// TODO 这个是简单粗暴的临时写法。将来考虑如何优化，一次全查出来有点豪放啊，例如翻页什么的
    /// 查询某个会话的全部聊天文件（包括图片、文件、视频）
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public List<ChatSessionFilesBean> findAllFilesByUser(String user) {
        List<ChatSessionFilesBean> list = null;
        try {
            list = MessageDao.getInstance().findAllFilesByUser(user);
        } catch (Exception e) {
            Log.Error(typeof(MessageService), e);
        }
        return list;
    }
}
}
