using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.AgoraVideoClr;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Adorners;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Control.Message;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Views.Windows;
using cn.lds.chatcore.pcw.Views.Windows.AVMeeting;
using EventBus;
using WpfAnimatedGif;
using NoticeMessageControl = cn.lds.chatcore.pcw.Views.Control.Message.NoticeMessageControl;
using TextMessageControl = cn.lds.chatcore.pcw.Views.Control.Message.TextMessageControl;

namespace cn.lds.chatcore.pcw.Views {



/// <summary>
/// ChatPage.xaml 的交互逻辑
/// </summary>
///
public partial class ChatPage : System.Windows.Controls.Page {

    private ChatPage() {
        InitializeComponent();

        Thread newMsgThread = new Thread(() => {
            List<MessageItem> x = new List<MessageItem>();

            while (true) {
                if (queuemMessageItems.Count > 0) {
                    x.Clear();
                    while (x.Count<8 && queuemMessageItems.Count>0) {
                        x.Add(queuemMessageItems.Dequeue());
                    }
                    LoadMessage(x,-1);
                    Thread.Sleep(50);
                } else {
                    //避免死循环一直占用cpu
                    Thread.Sleep(1000);
                }
            }
        });
        newMsgThread.IsBackground = true;
        newMsgThread.Start();

    }
    public static ChatPage getInstance() {
        if (instance == null) {
            instance = new ChatPage();
        }
        return instance;
    }

    //第一条未读消息的时间
    private MessagesTable firstUnreadMessage = null;

    private AdornerLayer adornerLayer = null;
    private UnreadAdorner unReadAdorner = null;



    private NewMessageAdorner newMsgAdorner = null;
    private AtMeAdorner atMeAdorner = null;

    private bool load = false;
    private long lastTimestamp = 0;
    private List<MessageItem> _messagesList = new List<MessageItem>();

    public ChatSessionType ChatSessionType;
    private string _clientuserId = string.Empty;
    private string _mucNo=string.Empty;
    private string _publicAppId = string.Empty;
    private static ChatPage instance = null;
    private string userNo = string.Empty;

    public string UserNo {
        get {
            return userNo;
        }
    }
    public string ClientuserId {
        get {
            return _clientuserId;
        } set {
            _clientuserId = value;

            if (!string.IsNullOrEmpty(_clientuserId)) {
                Init();
            }
        }
    }

    public string MucNo {
        get {
            return _mucNo;
        } set {
            _mucNo = value;

            if (!string.IsNullOrEmpty(_mucNo)) {
                Init();
            }
        }
    }

    public string PublicAppId {
        get {
            return _publicAppId;
        } set {
            _publicAppId = value;
            if (!string.IsNullOrEmpty(_publicAppId)) {
                Init();
            }
        }
    }


    [EventSubscriber]
    public void oOnBusinessEvent(BusinessEvent<Object> data) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (data.eventDataType) {
            //群成员变化
            case BusinessEventDataType.MucEditTextChangedEvent_TYPE_ADD:
                DoMucEditTextChangedEvent_TYPE_ADD(data);
                break;
            //群成员变化
            case BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE:
                DoMucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE(data);
                break;
            //群成员昵称变更
            case BusinessEventDataType.GroupMemberNicknameChangedEvent:
                DoGroupMemberNicknameChangedEvent(data);
                break;
            //头像换了
            case BusinessEventDataType.ContactsDetailsChangeEvent:
                DoContactsDetailsChangeEvent(data);
                break;
            //来消息
            case BusinessEventDataType.MessageChangedEvent:
                DoMessageChangedEvent(data);
                break;
            //撤回消息
            case BusinessEventDataType.MessageChangedEvent_TYPE_CANCLE:
                DoMessageChangedEvent_TYPE_CANCLE(data);
                break;
            //删除消息
            case BusinessEventDataType.MessageChangedEvent_TYPE_DELETE:
                DoMessageChangedEvent_TYPE_DELETE(data);
                break;
            //改群名
            case BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME:
                DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(data);
                break;
            //消息状态
            case BusinessEventDataType.MessageChangedEvent_TYPE_UPDATE:
                MessageChangedEvent_TYPE_UPDATE(data);
                break;
            // 聊天详情修改
            case BusinessEventDataType.ChatDetailedChangeEvent:
                DoChatDetailedChangeEvent(data);
                break;
            // 语音播放完成
            case BusinessEventDataType.VoiceStopPlay:
                DoVoiceStopPlay(data);
                break;
            //下载文件
            case BusinessEventDataType.FileDownloadingEvent:
                DoFileDownloadingEvent(data);
                break;
            //下载完成文件
            case BusinessEventDataType.FileDownloadedEvent:
                DoFileDownloadedEvent(data);
                break;
            //关闭未读消息提示框
            case BusinessEventDataType.UnreadControlClose:
                DoUnreadControlClose(data);
                break;
            //点击未读消息提示框
            case BusinessEventDataType.ClickUnreadControl:
                DoClickUnreadControl(data);
                break;
            //关闭新消息提示框
            case BusinessEventDataType.NewMessageControlClose:
                DoNewMessageControlClose(data);
                break;
            //点击新消息提示框
            case BusinessEventDataType.ClickNewMessageControl:
                DoClickNewMessageControl(data);
                break;
            //关闭@消息提示框
            case BusinessEventDataType.AtMessageControlClose:
                DoAtMessageControlClose(data);
                break;
            //点击@消息提示框
            case BusinessEventDataType.ClickAtMessageControl:
                DoClickAtMessageControl(data);
                break;
            //发送名片消息
            case BusinessEventDataType.SendVcCard:
                DoSendVcCard(data);
                break;
            //发送公众号名片消息
            case BusinessEventDataType.SendPublicCard:
                DoSendPublicCard(data);
                break;
            //发送网盘文件
            case BusinessEventDataType.ClickDiskFile:
                DoClickDiskFile(data);
                break;

            //接到拒绝
            case BusinessEventDataType.AVMeetingRefuse:
                DoAVMeetingRefuse(data,false);
                break;
            //接到取消
            case BusinessEventDataType.AVMeetingCancel:
                DoAVMeetingRefuse(data, false);
                break;
            //接到已在其他设备接通
            case BusinessEventDataType.AVMeetingConnect:
                DoAVMeetingRefuse(data, false);
                break;

            //接到忙消息
            case BusinessEventDataType.AVMeetingBusy:
                DoAVMeetingRefuse(data,false);
                break;
            //接到超时消息
            case BusinessEventDataType.AVMeetingTimeOut:
                DoAVMeetingRefuse(data,false);
                break;
            //接到语音申请
            case BusinessEventDataType.AudioMeetingReceiving:
                DoAudioMeetingReceiving(data);
                break;
            //接到视频申请
            case BusinessEventDataType.VideoMeetingReceiving:
                DoVideoMeetingReceiving(data);
                break;
            //接到切换音频通知
            case BusinessEventDataType.AVMeetingSwitch:
                DoAVMeetingSwitch(data);
                break;
            case BusinessEventDataType.AVMeetingRefresh:
                DoAVMeetingRefresh(data);
                break;

            //接到群语音申请
            case BusinessEventDataType.AVGroupMeetingReceiving:
                DoAVGroupMeetingReceiving(data);
                break;
            //接到群成员取消
            case BusinessEventDataType.AVGroupMeetingCancel:
                DoAVGroupMeetingCancel(data, false);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }

    }

    private void DoAVGroupMeetingCancel(BusinessEvent<object> data, bool sendMsg) {
        if (data.data.ToStr() == string.Empty) return;
        List<SelectPeopleSubViewModel> item = data.data as List<SelectPeopleSubViewModel>;

        this.Dispatcher.BeginInvoke((Action)delegate () {
            //if (AgoraEngine.Instance.ReceivingWindow != null)
            //{
            //    AgoraEngine.Instance.ReceivingWindow.CloseMethod();
            //}


            if (AgoraEngine.Instance.VideoWindow.WinVideos != null) {

                foreach(SelectPeopleSubViewModel model in item) {
                    //bool flag = AgoraEngine.Instance.VideoWindow.WinVideos.Where(q => q.Uid.Equals(model.MemberId)).Count() > 0;
                    //if(flag)
                    //{
                    WinVideo win =
                        AgoraEngine.Instance.VideoWindow.WinVideos.Where(q => q.Uid.Equals(model.MemberId)) as
                        WinVideo;
                    if(win!=null) {
                        win.Close();
                    }
                    //}
                }
                //AgoraEngine.Instance.VideoWindow.CloseMethod(sendMsg, true);
            }




        });
    }

    private void DoAVMeetingSwitch(BusinessEvent<object> data) {
        if (data.data== null) return;
        //Thread.Sleep(1000);
        AVMeetingSwitchEventData item = data.data as AVMeetingSwitchEventData;
        if (item == null) return;
        AVMeetingSwitchMessage message = item.message;
        this.Dispatcher.BeginInvoke((Action)delegate () {
            AVMeetingService.getInstance()
            .SetChatMessageTextByType(AVMeetingBussinessType.AVMeetingSwitch, message.getRoomId());

            //if (App.AmStatus == AVMeetingStatus.waiting) {
            //对方给我发视频 当我没接的时候直接转语音了
            if (AgoraEngine.Instance.VideoWindow == null) {
                if (AgoraEngine.Instance.ReceivingWindow != null) {
                    AgoraEngine.Instance.ReceivingWindow.CloseMethod();
                }

                AVMeetingInviteMessage inviteMessage = new AVMeetingInviteMessage();
                inviteMessage.from = message.getFrom();
                inviteMessage.to = message.getTo();
                inviteMessage.setRoomId(message.getRoomId());
                BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                businessEvent.data = inviteMessage;
                businessEvent.eventDataType = BusinessEventDataType.AudioMeetingReceiving;
                EventBusHelper.getInstance().fireEvent(businessEvent);
            }
            //else   if (App.AmStatus == AVMeetingStatus.contected)
            //我给对方发视频 对方直接语音接听
            else if (AgoraEngine.Instance.VideoWindow!=null) {
                //AVMeetingService.getInstance()
                //.SetChatMessageTextByType(AVMeetingBussinessType.AVMeetingSwitch, message.getRoomId());

                if (AgoraEngine.Instance.VideoWindow != null) {
                    AgoraEngine.Instance.VideoWindow.CloseMethod(false,false);
                }
                VcardsTable guestModel = VcardService.getInstance().findByNo(message.getFrom());
                AgoraEngine.Instance.AudioWindow = new WinAudio(message.getRoomId());
                AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.ChannelName = message.getRoomId();
                AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestNo = guestModel.no;
                AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestName = guestModel.nickname;
                AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
                AgoraEngine.Instance.Video2Audio();
                //AgoraEngine.Instance.JoinAudioChannel(message.getRoomId(), App.AccountsModel.clientuserId.ToInt(), App.winAudio);
                AgoraEngine.Instance.AudioWindow.Show(); // 显示窗体
                AgoraEngine.Instance.AudioWindow.timer.Stop();
                AgoraEngine.Instance.AudioWindow.playSound.StopWaitSound();
                App.AmStatus = AVMeetingStatus.contected;
                AgoraEngine.Instance.CancleMute();
                AgoraEngine.Instance.AudioWindow.TxtblTooltip.Visibility = Visibility.Visible;
                AgoraEngine.Instance.AudioWindow.CanvasLoading.Visibility = Visibility.Collapsed;
                AgoraEngine.Instance.AudioWindow.BtnMute.Visibility = Visibility.Visible;
                AgoraEngine.Instance.AudioWindow.BtnVolume.Visibility = Visibility.Visible;
            }




        });
    }

    private void DoAVMeetingRefuse(BusinessEvent<object> data,bool sendMsg) {
        if (data.data.ToStr() == string.Empty) return;
        AVMeetingType item = (AVMeetingType)Enum.Parse(typeof(AVMeetingType), data.data.ToStr());

        App.AmStatus = AVMeetingStatus.no;
        this.Dispatcher.BeginInvoke((Action)delegate () {
            if (AgoraEngine.Instance.ReceivingWindow != null) {
                AgoraEngine.Instance.ReceivingWindow.CloseMethod();
            }

            if (item== AVMeetingType.video) {
                if(AgoraEngine.Instance.VideoWindow != null) {
                    AgoraEngine.Instance.VideoWindow.CloseMethod(sendMsg,true);
                }

            } else if (item == AVMeetingType.audio) {
                if(AgoraEngine.Instance.AudioWindow != null) {
                    AgoraEngine.Instance.AudioWindow.CloseMethod(sendMsg);
                }

            }

        });
    }

    private void DoAVMeetingRefresh(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            MessagesTable item = data.data as MessagesTable;
            if (item == null) return;

            this.Dispatcher.BeginInvoke((Action)delegate () {


                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c = panelList.Items[i] as MessageBase;
                    if (c == null) continue;
                    if (c.Item.messageId == item.messageId) {
                        AmMessageControl control = panelList.Items[i] as AmMessageControl;
                        if (control == null) return;
                        control.Item = MessageService.getInstance().convertToMessItem(c.Item.user, item);
                        control.Refresh();
                    }
                }
            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private void DoClickDiskFile(BusinessEvent<object> data) {
        if (data.data.ToStr() == string.Empty) return;
        SendDiskFileEventData item = data.data as SendDiskFileEventData;
        if (item == null) return;

        this.Dispatcher.BeginInvoke((Action)delegate () {
            foreach(DiskFileBean bean in item.diskFiles) {
                //生成messageid


                MessageItem messageItem= MessageService.getInstance()
                                         .sendDiskFileMessage(sendMessage.ToUserNo, App.AccountsModel.no,  bean.fileStorageId,
                                                 bean.filename, bean.filesize);

                if (item != null) {
                    sendMessage_BnSendClick(messageItem, MsgType.File);
                }
                MessageService.getInstance().DoSendMessage(messageItem);
            }

        });
    }

    private void DoSendPublicCard(BusinessEvent<object> data) {
        if (data.data.ToStr() == string.Empty) return;
        MessageItem item = data.data as MessageItem;
        if (item == null) return;

        this.Dispatcher.BeginInvoke((Action)delegate () {
            sendMessage_BnSendClick(item, MsgType.PublicCard);
        });
    }

    private void DoSendVcCard(BusinessEvent<object> data) {
        if (data.data.ToStr() == string.Empty) return;
        MessageItem item = data.data as MessageItem;
        if (item == null) return;

        this.Dispatcher.BeginInvoke((Action)delegate () {
            sendMessage_BnSendClick(item, MsgType.VCard);
        });
    }

    private void DoAVGroupMeetingReceiving(BusinessEvent<object> eventData) {
        if (eventData.data.ToStr() == string.Empty) return;
        MessageTypeAVGroupInviteMessage item = eventData.data as MessageTypeAVGroupInviteMessage;
        if (item == null) return;
        App.AmStatus = AVMeetingStatus.waiting;
        this.Dispatcher.BeginInvoke((Action)delegate () {
            // 播放声音
            AgoraEngine.Instance.ReceivingWindow = new WinReceiving(AVMeetingType.video);
            VcardsTable guestModel = VcardService.getInstance().findByNo(item.from);
            AgoraEngine.Instance.ReceivingWindow.GuestName = guestModel.nickname;
            AgoraEngine.Instance.ReceivingWindow.GuestNo = item.from;
            AgoraEngine.Instance.ReceivingWindow.AVGroupInviteMessage = item;
            AgoraEngine.Instance.ReceivingWindow.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            AgoraEngine.Instance.ReceivingWindow.RoomId = item.getRoomId();
            AgoraEngine.Instance.ReceivingWindow.Left = 0;
            AgoraEngine.Instance.ReceivingWindow.Top = 0;

            AgoraEngine.Instance.ReceivingWindow.Show();
            AgoraEngine.Instance.ReceivingWindow.Activate();
            //if (winReceiving.ShowDialog() == true) {
            //    // 接受
            //} else {
            //    // 拒绝
            //}

        });
    }
    private void DoAudioMeetingReceiving(BusinessEvent<object> eventData) {
        if (eventData.data.ToStr() == string.Empty) return;
        AVMeetingInviteMessage item = eventData.data as AVMeetingInviteMessage;
        if (item == null) return;
        App.AmStatus = AVMeetingStatus.waiting;
        this.Dispatcher.BeginInvoke((Action)delegate () {
            // 播放声音
            AgoraEngine.Instance.ReceivingWindow = new WinReceiving(AVMeetingType.audio);
            VcardsTable guestModel = VcardService.getInstance().findByNo(item.from);
            AgoraEngine.Instance.ReceivingWindow.GuestName = guestModel.nickname;
            AgoraEngine.Instance.ReceivingWindow.GuestNo = item.from;
            AgoraEngine.Instance.ReceivingWindow.AVMeetingInviteMessage = item;
            AgoraEngine.Instance.ReceivingWindow.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            AgoraEngine.Instance.ReceivingWindow.RoomId = item.getRoomId();
            AgoraEngine.Instance.ReceivingWindow.Left = 0;
            AgoraEngine.Instance.ReceivingWindow.Top = 0;
            AgoraEngine.Instance.ReceivingWindow.Show();
            AgoraEngine.Instance.ReceivingWindow.Activate();
            //if (winReceiving.ShowDialog() == true) {
            //    // 接受
            //} else {
            //    // 拒绝
            //}

        });
    }


    private void DoVideoMeetingReceiving(BusinessEvent<object> eventData) {
        if (eventData.data.ToStr() == string.Empty) return;
        AVMeetingInviteMessage item = eventData.data as AVMeetingInviteMessage;
        if (item == null) return;
        App.AmStatus = AVMeetingStatus.waiting;
        this.Dispatcher.BeginInvoke((Action)delegate () {
            VcardsTable guestModel = VcardService.getInstance().findByNo(item.from);
            AgoraEngine.Instance.ReceivingWindow = new WinReceiving(AVMeetingType.video);
            AgoraEngine.Instance.ReceivingWindow.GuestName = guestModel.nickname;
            AgoraEngine.Instance.ReceivingWindow.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            AgoraEngine.Instance.ReceivingWindow.GuestNo = item.from;
            AgoraEngine.Instance.ReceivingWindow.RoomId = item.getRoomId();
            AgoraEngine.Instance.ReceivingWindow.Left = 0;
            AgoraEngine.Instance.ReceivingWindow.Top = 0;
            AgoraEngine.Instance.ReceivingWindow.Show();
            AgoraEngine.Instance.ReceivingWindow.Activate();
            //if (winReceiving.ShowDialog() == true) {
            //    // 接受
            //} else {
            //    // 拒绝
            //}

        });
    }

    private void DoClickAtMessageControl(BusinessEvent<object> data) {


        MessagesTable message = MessageService.getInstance().findLastAtMessagesByUser(_mucNo);
        this.Dispatcher.BeginInvoke((Action)delegate () {
            for (int i = 0; i < panelList.Items.Count; i++) {
                MessageBase c = panelList.Items[i] as MessageBase;
                if (c.Item.messageId == message.messageId) {
                    panelList.SelectedItem = c;
                    panelList.ScrollIntoView(panelList.SelectedItem);
                    break;
                }
            }

            atMeAdorner.Visibility = Visibility.Hidden;


        });

    }

    private void DoAtMessageControlClose(BusinessEvent<object> data) {
        this.Dispatcher.BeginInvoke((Action)delegate () {
            atMeAdorner.Visibility = Visibility.Hidden;
        });
    }

    private void DoClickNewMessageControl(BusinessEvent<object> data) {

        this.Dispatcher.BeginInvoke((Action)delegate () {
            for (int i = 0; i < panelList.Items.Count; i++) {
                MessageBase c = panelList.Items[i] as MessageBase;
                if (c.Item == LocalMessageItem) {
                    panelList.SelectedItem = c;
                    panelList.ScrollIntoView(panelList.SelectedItem);
                    break;
                }
            }

            newMsgAdorner.Visibility = Visibility.Hidden;
            count = 0;

        });


    }

    private void DoNewMessageControlClose(BusinessEvent<object> data) {
        this.Dispatcher.BeginInvoke((Action)delegate () {
            newMsgAdorner.Visibility = Visibility.Hidden;
        });
    }

    private void DoClickUnreadControl(BusinessEvent<object> data) {
        if (firstUnreadMessage == null) return;

        long  firstUnreadMessageTime = long.Parse(firstUnreadMessage.timestamp);


        this.Dispatcher.BeginInvoke((Action)delegate () {

            //当前显示页面的第一条消息时间
            long localFirstTimestamp = 0;

            MessageBase c = panelList.Items[0] as MessageBase;
            if (c != null) {
                localFirstTimestamp = long.Parse(c.Item.timestamp);
            }

            //加载所有
            _messagesList = MessageService.getInstance().getAllUnreadMessage(sendMessage.ToUserNo, localFirstTimestamp, firstUnreadMessageTime);

            LoadMessage(_messagesList, 0);
            if (_messagesList.Count == 0) return;
            if (_messagesList[_messagesList.Count - 1].timestamp != null) {
                lastTimestamp = long.Parse(_messagesList[_messagesList.Count - 1].timestamp);
            }
            unReadAdorner.Visibility = Visibility.Hidden;
            scrollViewer1.ScrollToHome();
            //for (int i=0;i<panelList.Items.Count;i++)
            //{
            //    MessageBase message = panelList.Items[i] as MessageBase;
            //   if(message.Item.messageId== firstUnreadMessage.messageId)
            //    {
            //        scrollViewer1.s(ListBoxLeft.SelectedItem);
            //    }
            //}
        });

    }

    private void DoUnreadControlClose(BusinessEvent<object> data) {
        this.Dispatcher.BeginInvoke((Action)delegate () {


            unReadAdorner.Visibility = Visibility.Hidden;

        });

    }

    private void DoFileDownloadedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            FileEventData item = data.data as FileEventData;
            if (item == null) return;

            string messageId = item.businessId;
            this.Dispatcher.BeginInvoke((Action)delegate () {

                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c = panelList.Items[i] as MessageBase;
                    if (c == null) continue;
                    if (c.Item.messageId == messageId) {
                        if (c.Item.type == MsgType.File.ToStr()) {
                            FileMessageControl file = panelList.Items[i] as FileMessageControl;
                            if (file == null) continue;
                            file.bProcess.Visibility = Visibility.Collapsed;
                            file.bProcessR.Visibility = Visibility.Collapsed;
                            return;
                        } else if (c.Item.type == MsgType.Video.ToStr()) {
                            VideoMessageControl video = panelList.Items[i] as VideoMessageControl;
                            if (video == null) continue;
                            video.bProcess.Visibility = Visibility.Collapsed;
                            video.imagePlay.Visibility = Visibility.Visible;
                            return;
                        }

                    }
                }


            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }
    private void DoFileDownloadingEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            FileEventData item = data.data as FileEventData;
            if (item == null) return;

            string messageId = item.businessId;
            this.Dispatcher.BeginInvoke((Action)delegate () {

                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c = panelList.Items[i] as MessageBase;
                    if (c == null) continue;
                    if (c.Item.messageId == messageId) {
                        if(c.Item.type==MsgType.File.ToStr()) {
                            FileMessageControl file = panelList.Items[i] as FileMessageControl;
                            if (file == null) continue;
                            file.UpdateProcess(item.percent);
                            return;
                        } else if (c.Item.type == MsgType.Video.ToStr()) {
                            VideoMessageControl video = panelList.Items[i] as VideoMessageControl;
                            if (video == null) continue;
                            video.UpdateProcess(item.percent);
                            return;
                        }


                    }
                }


            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }


    private void DoVoiceStopPlay(BusinessEvent<object> data) {
        bool goOnPlay = false;

        try {
            if (data.data.ToStr() == string.Empty) return;
            MediaEventData item = data.data as MediaEventData;
            bool IsforceStop = item.extras["IsforceStop"].ToStr().ToBool();
            if (item == null) return;

            string messageId = item.checkMark;
            this.Dispatcher.BeginInvoke((Action)delegate () {

                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c = panelList.Items[i] as MessageBase;
                    if (c == null) continue;
                    if (c.Item.messageId == messageId) {
                        VoiceMessageControl voice = panelList.Items[i] as VoiceMessageControl;
                        voice.PlayStopped();
                        c.Item.flag = true;
                        goOnPlay = true;
                        continue;
                    }


                    if (c.Item.type == MsgType.Voice.ToStr() && c.Item.flag == false && goOnPlay) {
                        if (IsforceStop && goOnPlay) return;
                        VoiceMessageControl voice = panelList.Items[i] as VoiceMessageControl;
                        voice.PlayStart();
                        goOnPlay = false;
                    }
                }


            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }


    /// <summary>
    /// 聊天详情修改
    /// </summary>
    /// <param Name="data"></param>
    public void DoChatDetailedChangeEvent(BusinessEvent<object> data) {
        try {
            string user = data.data.ToStr();
            if (user == string.Empty) return;
            if (user == App.SelectChartSessionNo) {
                this.Dispatcher.Invoke(new Action(() => {
                    Init();
                }));
            }

        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }


    private void MessageChangedEvent_TYPE_UPDATE(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            MessageItem item = data.data as MessageItem;
            if (item == null) return;
            if (item.user != App.SelectChartSessionNo) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {

                // -1:失败；0：发送中；1：已发送
                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c =
                        panelList.Items[i] as MessageBase;
                    if (c == null) continue;
                    if (c.Item.messageId == item.messageId) {
                        if (item.sent == "0" || item.sent == string.Empty) {
                            c.imageStatus.Visibility = Visibility.Visible;
                            c.imageStatus.Source = new BitmapImage(new Uri(ImageHelper.getSysImagePath("Message/Sending.png"),UriKind.RelativeOrAbsolute));
                            //var image = new BitmapImage();
                            //image.BeginInit();
                            //image.UriSource = new Uri(ImageHelper.getSysImagePath("Message/Sending.gif"));
                            //image.EndInit();
                            //ImageBehavior.SetAnimatedSource(c.imageStatus, image);

                        }
                        if (item.sent == "1") {
                            c.imageStatus.Visibility = Visibility.Collapsed;
                        } else if (item.sent == "-1") {
                            c.imageStatus.Visibility = Visibility.Visible;
                            c.imageStatus.Source =new BitmapImage(new Uri(ImageHelper.getSysImagePath("Message/Send_error.png"),UriKind.RelativeOrAbsolute));
                            //var image = new BitmapImage();
                            //image.BeginInit();
                            //image.UriSource = new Uri(ImageHelper.getSysImagePath("Message/Send_error.png"));
                            //image.EndInit();
                            //ImageBehavior.SetAnimatedSource(c.imageStatus, image);

                        }
                    }

                }


            });
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }
    private void DoMessageChangedEvent_TYPE_DELETE(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            MessageItem item = data.data as MessageItem;
            if (item == null) return;
            if (item.user != App.SelectChartSessionNo) return;

            panelList.Dispatcher.BeginInvoke(new Action(delegate {
                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c =
                        panelList.Items[i] as MessageBase;

                    if (c.Item.messageId == item.messageId) {
                        panelList.Items.Remove(c);
                    }

                }

            }));

        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private void DoMessageChangedEvent_TYPE_CANCLE(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            MessageItem item = data.data as MessageItem;
            if (item == null) return;
            if (item.user != App.SelectChartSessionNo) return;

            panelList.Dispatcher.BeginInvoke(new Action(delegate {

                if (item.type == MsgType.Text.ToStr()) {

                }

                for (int i = 0; i < panelList.Items.Count; i++) {
                    MessageBase c =
                        panelList.Items[i] as MessageBase;

                    if (c.Item.messageId == item.messageId) {
                        panelList.Items.Remove(c);

                        NoticeMessageControl notice = new NoticeMessageControl();

                        notice.Item = item;
                        panelList.Items.Insert(i, notice);
                    }

                }

            }));

        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private Queue<MessageItem> queuemMessageItems = new Queue<MessageItem>();
    private void DoMessageChangedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            MessageItem item = data.data as MessageItem;
            if (item == null) return;

            //查询未读数量
            MessageService.getInstance().countOfUnreadMessages();

            //是本人的消息 显示出来
            if (item.user == App.SelectChartSessionNo) {

                if(item.type== MsgType.Cancel.ToStr()) {
                    //发出通知
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = item;
                    businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_CANCLE;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                    return;
                }

                //this.Dispatcher.BeginInvoke((Action)delegate() {


                //List<MessageItem> list = new List<MessageItem>();
                //list.Add(item);
                //LoadMessage(list, -1);
                queuemMessageItems.Enqueue(item);

                //设置已读
                MessageService.getInstance().setMessageRead(item.user);

                showNewMsg(item);
                showAtMe(item);
                //});
            }



            //更新chartsession
            //BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            //businessEvent.data = item;
            //businessEvent.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
            //EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private void DoContactsDetailsChangeEvent(BusinessEvent<object> data) {
        Thread.Sleep(1000);
        try {
            if (data.data.ToStr() == string.Empty) return;
            ContactsTable dt = data.data as ContactsTable;
            if (dt == null) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                Init();
            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private void DoGroupMemberNicknameChangedEvent(BusinessEvent<object> data) {
        Thread.Sleep(1000);
        try {
            if (data.data.ToStr() == string.Empty) return;
            GroupMemberNicknameChangedEventData dt = data.data as GroupMemberNicknameChangedEventData;
            if (dt == null) return;
            if (string.IsNullOrEmpty(_mucNo)) return;
            if (dt.mucNo != _mucNo) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                GroupCon.MucNo = _mucNo;
                //Init();
            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private void DoMucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE(BusinessEvent<object> data) {
        Thread.Sleep(1000);
        try {
            if (data.data.ToStr() == string.Empty) return;
            MucTable dt = data.data as MucTable;
            if (dt == null) return;
            if (string.IsNullOrEmpty(_mucNo)) return;
            if (dt.no != _mucNo) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                GroupCon.MucNo = _mucNo;
                //Init();
            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }

    private void DoMucEditTextChangedEvent_TYPE_ADD(BusinessEvent<object> data) {
        try {

            Thread.Sleep(1000);
            if (data.data.ToStr() == string.Empty) return;
            string dtMucNo = data.data.ToStr();
            if (dtMucNo == string.Empty) return;
            if (string.IsNullOrEmpty(_mucNo)) return;
            if (dtMucNo != _mucNo) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                GroupCon.MucNo = _mucNo;
                //Init();
            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }


    public void DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(BusinessEvent<object> data) {

        try {
            if (data.data.ToStr() == string.Empty) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                MucTable dt = data.data as MucTable;
                if (dt == null) return;
                if (dt.no == _mucNo) {
                    Titel.Content = dt.name;
                }

            });
        } catch (Exception ex) {
            Log.Error(typeof(ChatPage), ex);
        }
    }


    public void DoEvents() {
        DispatcherFrame frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
        new DispatcherOperationCallback(delegate(object f) {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
                                               ), frame);
        Dispatcher.PushFrame(frame);
    }

    //测试用
    //private MessageItem itema = new MessageItem();
    /// <summary>
    /// 加载聊天记录
    /// </summary>
    /// <param Name="messagesList"></param>
    private void LoadMessage(List<MessageItem> messagesList,int insertIndex) {
        for (int i = 0; i < messagesList.Count; i++) {

            MessageItem item = messagesList[i];
            if (item == null) return;
            if(firstUnreadMessage!=null && item.messageId==firstUnreadMessage.messageId) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    unReadAdorner.Visibility = Visibility.Hidden;
                }));
            }
            this.Dispatcher.BeginInvoke(new Action(() => {
                if (item.type == MsgType.Text.ToStr()|| item.type == MsgType.At.ToStr()) {
                    TextMessageControl textControl = new TextMessageControl();
                    textControl.ChatSessionType = ChatSessionType;
                    textControl.Item = item;

                    textControl.DxClick -= control_DxClick;
                    textControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(textControl);
                    } else {
                        panelList.Items.Insert(insertIndex, textControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.Notify.ToStr()) {
                    NoticeMessageControl noticeControl = new NoticeMessageControl();

                    //MessageItem item1 = new MessageItem();
                    //item1.showTimestamp = true;
                    //item1.text = "aa";
                    //item1.timestamp = "11111";
                    //item1.timeDate = DateTime.Now;
                    noticeControl.Item = item;
                    if (insertIndex < 0) {
                        panelList.Items.Add(noticeControl);
                    } else {
                        panelList.Items.Insert(insertIndex, noticeControl);
                    }

                } else  if (item.type == MsgType.Image.ToStr()) {
                    ImageMessageControl imageControl = new ImageMessageControl();
                    imageControl.ChatSessionType = ChatSessionType;
                    imageControl.Item = item;

                    imageControl.DxClick -= control_DxClick;
                    imageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(imageControl);
                    } else {
                        panelList.Items.Insert(insertIndex, imageControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.News.ToStr()) {
                    ImageTextControl imageTextControl = new ImageTextControl();

                    imageTextControl.Item = item;


                    //imageControl.DxClick -= control_DxClick;
                    //imageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(imageTextControl);
                    } else {
                        panelList.Items.Insert(insertIndex, imageTextControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.File.ToStr()) {
                    FileMessageControl fileMessageControl = new FileMessageControl();
                    fileMessageControl.ChatSessionType = ChatSessionType;
                    fileMessageControl.Item = item;


                    fileMessageControl.DxClick -= control_DxClick;
                    fileMessageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(fileMessageControl);
                    } else {
                        panelList.Items.Insert(insertIndex, fileMessageControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.Video.ToStr()) {
                    VideoMessageControl videoMessageControl = new VideoMessageControl();
                    videoMessageControl.ChatSessionType = ChatSessionType;
                    videoMessageControl.Item = item;


                    videoMessageControl.DxClick -= control_DxClick;
                    videoMessageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(videoMessageControl);
                    } else {
                        panelList.Items.Insert(insertIndex, videoMessageControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.Voice.ToStr()) {
                    VoiceMessageControl voiceControl = new VoiceMessageControl();
                    voiceControl.ChatSessionType = ChatSessionType;
                    voiceControl.Item = item;


                    voiceControl.DxClick -= control_DxClick;
                    voiceControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(voiceControl);
                    } else {
                        panelList.Items.Insert(insertIndex, voiceControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.VCard.ToStr()) {
                    VCardMessageControl vCardMessageControl = new VCardMessageControl();
                    vCardMessageControl.ChatSessionType = ChatSessionType;
                    vCardMessageControl.Item = item;

                    vCardMessageControl.DxClick -= control_DxClick;
                    vCardMessageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(vCardMessageControl);
                    } else {
                        panelList.Items.Insert(insertIndex, vCardMessageControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.PublicCard.ToStr()) {
                    PublicCardMessageControl publicCardMessageControl = new PublicCardMessageControl();
                    publicCardMessageControl.ChatSessionType = ChatSessionType;
                    publicCardMessageControl.Item = item;

                    publicCardMessageControl.DxClick -= control_DxClick;
                    publicCardMessageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(publicCardMessageControl);
                    } else {
                        panelList.Items.Insert(insertIndex, publicCardMessageControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.Location.ToStr()) {
                    LocationMessageControl locationMessageControl = new LocationMessageControl();
                    locationMessageControl.ChatSessionType = ChatSessionType;
                    locationMessageControl.Item = item;

                    locationMessageControl.DxClick -= control_DxClick;
                    locationMessageControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(locationMessageControl);
                    } else {
                        panelList.Items.Insert(insertIndex, locationMessageControl);
                    }
                    //DoEvents();
                } else if (item.type == MsgType.AVMeetingInvite.ToStr()) { //对方申请语音
                    AVMeetingInviteMessage messageBean = new AVMeetingInviteMessage().toModel(item.content);

                    AmMessageControl textControl = new AmMessageControl();
                    textControl.ChatSessionType = ChatSessionType;
                    textControl.Item = item;

                    textControl.DxClick -= control_DxClick;
                    textControl.DxClick += control_DxClick;
                    if (insertIndex < 0) {
                        panelList.Items.Add(textControl);
                    } else {
                        panelList.Items.Insert(insertIndex, textControl);
                    }

                }
            }));
            //if (insertIndex==0) {
            //取最上面一条的时间
            //if (i == messagesList.Count - 1) {
            //    //itema = item;
            //    if (item.timestamp!=null) {
            //        lastTimestamp = long.Parse(item.timestamp);
            //    }
            //}
            //}


        }


        //ImageMessageControl c = new ImageMessageControl();
        //c.ImagePath = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/images/Default_avatar.png";
        //c.Left = true;

        //c.Item = itema;
        //c.DxClick -= control_DxClick;
        //c.DxClick += control_DxClick;
        //panelList.Items.Add(c);
    }


    /// <summary>
    /// 显示@我的消息提醒
    /// </summary>
    /// <param Name="no"></param>
    private void showAtMe(MessageItem item) {

        string userName = string.Empty;
        if (item==null) {
            ChatSessionTable dt = ChatSessionService.getInstance().findByNo(_mucNo);
            if (dt != null) {
                if (dt.atme) {
                    MessagesTable message = MessageService.getInstance().findLastAtMessagesByUser(_mucNo);

                    userName = ContactsServices.getInstance().getContractOriginalNameByNo(message.resource);
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        atMeAdorner.AtMeControl.Text = userName + Constants.MESSAGE_AT_IN_MUC;
                        atMeAdorner.Visibility = Visibility.Visible;
                        unReadAdorner.Visibility = Visibility.Hidden;
                    }));
                }
                //    else {
                //    atMeAdorner.Visibility = Visibility.Hidden;
                //    return;
                //}
            }
        } else {
            if(item.type==MsgType.At.ToStr() && item.atme) {
                userName = ContactsServices.getInstance().getContractOriginalNameByNo(item.resource);
                this.Dispatcher.BeginInvoke(new Action(() => {
                    atMeAdorner.AtMeControl.Text = userName + Constants.MESSAGE_AT_IN_MUC;
                    atMeAdorner.Visibility = Visibility.Visible;
                    unReadAdorner.Visibility = Visibility.Hidden;
                }));
            }
            //    else {
            //    atMeAdorner.Visibility = Visibility.Hidden;
            //    return;
            //}
        }




    }

    /// <summary>
    /// 显示未读信息数量
    /// </summary>
    /// <param Name="no"></param>
    private void showUnread(string no) {

        //未读大于20 显示ssssssssss
        int UnredCount = MessageService.getInstance().countOfUnreadMessages(no);
        if (UnredCount > Constants.SYS_CONFIG_MESSAGE_NUM_PERPAGE) {
            unReadAdorner.UnReadControl.Text = UnredCount + "条新消息";
            unReadAdorner.Visibility = Visibility.Visible;
        }
    }

    private MessageItem LocalMessageItem = null;
    private int count = 0;
    private void showNewMsg(MessageItem item) {
        scrollViewer1.Dispatcher.BeginInvoke(new Action(() => {
            if(scrollViewer1.ScrollableHeight- scrollViewer1.VerticalOffset> Constants.SYS_ScHight) {
                if (count == 0) {
                    LocalMessageItem = item;
                }
                count++;
                newMsgAdorner.NewMessageControl.Text= count + "条新消息";
                newMsgAdorner.Visibility = Visibility.Visible;
            } else {
                count = 0;
                scrollViewer1.ScrollToBottom();
                newMsgAdorner.Visibility = Visibility.Hidden;
            }
        }));
    }
    private void Init() {

        queuemMessageItems = new Queue<MessageItem>();

        //到时候根据id查询聊天记录
        panelList.Items.Clear();

        // 初始未读消息
        adornerLayer = AdornerLayer.GetAdornerLayer(scrollViewer1);

        //未读消息
        if (unReadAdorner!=null) {
            adornerLayer.Remove(unReadAdorner);
        }
        //新消息
        if (newMsgAdorner != null) {
            adornerLayer.Remove(newMsgAdorner);
        }
        //@消息
        if (atMeAdorner != null) {
            adornerLayer.Remove(atMeAdorner);
        }

        if (adornerLayer != null) {
            adornerLayer.Opacity = 1;
            //未读消息
            unReadAdorner = new UnreadAdorner(scrollViewer1);
            adornerLayer.Add(unReadAdorner);
            unReadAdorner.Visibility = Visibility.Hidden;

            //新消息
            newMsgAdorner = new NewMessageAdorner(scrollViewer1);
            adornerLayer.Add(newMsgAdorner);
            newMsgAdorner.Visibility = Visibility.Hidden;


            //@消息
            atMeAdorner = new AtMeAdorner(scrollViewer1);
            adornerLayer.Add(atMeAdorner);
            atMeAdorner.Visibility = Visibility.Hidden;
        }

        sendMessage.TxtMessage.Clear();
        //sendMessage.Height = 145;
        BtnSingleChatDetailed.Visibility = Visibility.Hidden;
        BtnGroupChatDetailed.Visibility = Visibility.Hidden;
        BtnSetting.Visibility = Visibility.Hidden;
        BtnBack.Visibility = Visibility.Hidden;
        BtnAddPerson.Visibility = Visibility.Hidden;
        SendMessage_ClickBtnCollapsed(true);
        bool ShowCollapsedButton = false;

        if (ChatSessionType == ChatSessionType.NOTICE) {


            Titel.Content = "系统通知";
            BtnSingleChatDetailed.Visibility = Visibility.Collapsed;
            sendMessage.ToUserNo = Constants.SYSTEM_NOTICE_FLAG;
            sendMessage.YyPanel.Visibility = Visibility.Collapsed;

            chiren.RowDefinitions[1].Height = new GridLength(0);
            chiren.RowDefinitions[1].MaxHeight = 0;
            chiren.RowDefinitions[1].MinHeight = 0;
            //加载所有
            _messagesList = MessageService.getInstance().getMessagesByPage(sendMessage.ToUserNo, DateTimeHelper.getTimeStamp());


            GroupCon.Visibility = Visibility.Collapsed;

            showUnread(userNo);


        } else  if (ChatSessionType ==ChatSessionType.CHAT) {

            if (!string.IsNullOrEmpty(_clientuserId)) {
                VcardsTable model = VcardService.getInstance().findByClientuserId(long.Parse(_clientuserId));
                if (_clientuserId == App.AccountsModel.clientuserId) {
                    Titel.Content = "文件传输助手";
                    BtnSingleChatDetailed.Visibility = Visibility.Collapsed;
                    sendMessage.ToUserNo = App.AccountsModel.no;
                    sendMessage.YyPanel.Visibility = Visibility.Collapsed;
                    userNo = App.AccountsModel.no;
                } else {
                    Titel.Content = ContactsServices.getInstance().getContractNameByNo(model.no);
                    //Titel.Content = model.nickname;
                    BtnSingleChatDetailed.Visibility = Visibility.Visible;
                    sendMessage.ToUserNo = model.no;
                    sendMessage.YyPanel.Visibility = Visibility.Visible;
                    userNo = model.no;
                }


                //加载所有
                _messagesList = MessageService.getInstance().getMessagesByPage(sendMessage.ToUserNo, DateTimeHelper.getTimeStamp());

            }
            GroupCon.Visibility = Visibility.Collapsed;

            showUnread(userNo);


        } else if (ChatSessionType == ChatSessionType.MUC) {

            if (!string.IsNullOrEmpty(_mucNo)) {
                MucTable dt=   MucServices.getInstance().FindGroupByNo(_mucNo);
                Titel.Content = dt.name;
                sendMessage.ToUserNo = _mucNo;
                //加载所有
                _messagesList = MessageService.getInstance().getMessagesByPage(_mucNo, DateTimeHelper.getTimeStamp());
                GroupCon.Visibility = Visibility.Visible;
                GroupCon.MucNo = _mucNo;

                BtnGroupChatDetailed.Visibility = Visibility.Visible;

                showUnread(_mucNo);


                showAtMe(null);
                //TODO 功能未开发，临时注释
                //BtnAddPerson.Visibility = Visibility.Visible;
            }

        } else if (ChatSessionType == ChatSessionType.PUBLIC) {

            if (!string.IsNullOrEmpty(_publicAppId)) {
                PublicAccountsTable dt = PublicAccountsService.getInstance().findByAppId(_publicAppId);
                if (dt == null) return;


                //如果没配 就显示聊天的控件 否则就显示公众号菜单
                string menu = dt.menu;
                if (menu == "null" || menu.ToStr() == string.Empty) {
                    SendMessage_ClickBtnCollapsed(true);
                    ShowCollapsedButton = false;
                } else {
                    SendMessage_ClickBtnCollapsed(false);
                    ShowCollapsedButton = true;
                }
                sendMessage.ToUserNo = _publicAppId;
                publicMessage.AppId = _publicAppId;

                Titel.Content = dt.name;

                //加载所有
                _messagesList = MessageService.getInstance().getMessagesByPage(_publicAppId, DateTimeHelper.getTimeStamp());
                GroupCon.Visibility = Visibility.Collapsed;


                BtnSetting.Visibility = Visibility.Visible;
                // BtnBack.Visibility = Visibility.Visible;
                //TODO 功能未开发，临时注释
                //BtnAddPerson.Visibility = Visibility.Visible;

            }
        }



        //查询第一条未读消息
        if (!string.IsNullOrEmpty( userNo)) {
            firstUnreadMessage = MessageService.getInstance().getFirstUnreadMessages(userNo);
        } else {
            firstUnreadMessage = MessageService.getInstance().getFirstUnreadMessages(_mucNo);
        }



        sendMessage.ShowCollapsedButton = ShowCollapsedButton;
        sendMessage.ChatSessionType = ChatSessionType;
        //if (ChatSessionType == ChatSessionType.PUBLIC) {
        //    sendMessage.main.ColumnDefinitions[0].Width = new GridLength(32);
        //} else {
        //    sendMessage.main.ColumnDefinitions[0].Width = new GridLength(0);
        //}
        sendMessage.ClickBtnCollapsed -= SendMessage_ClickBtnCollapsed;
        sendMessage.ClickBtnCollapsed += SendMessage_ClickBtnCollapsed;
        sendMessage.BnSendClick -= sendMessage_BnSendClick;
        sendMessage.BnSendClick += sendMessage_BnSendClick;

        publicMessage.ClickBtnUnfold -= SendMessage_ClickBtnCollapsed;
        publicMessage.ClickBtnUnfold += SendMessage_ClickBtnCollapsed;
        gridSet.Visibility = Visibility.Collapsed;

        if (_messagesList.Count == 0) return;
        for (int i = _messagesList.Count - 1;  i > -1; i--) {
            MessageItem a = _messagesList[i];
            queuemMessageItems.Enqueue(a);
        }
        if (_messagesList[_messagesList.Count - 1].timestamp != null) {
            lastTimestamp = long.Parse(_messagesList[_messagesList.Count-1].timestamp);
        }
        //LoadMessage(_messagesList,0);

        load = true;
        scrollViewer1.ScrollToBottom();

    }

    /// <summary>
    /// 是否显示聊天输入框
    /// </summary>
    /// <param Name="visiable"></param>
    private void SendMessage_ClickBtnCollapsed(bool visiable) {
        if(visiable == false) {
            sendMessage.Visibility = Visibility.Collapsed;
            publicMessage.Visibility = Visibility.Visible;
            chiren.RowDefinitions[1].Height = new GridLength(30);
            chiren.RowDefinitions[1].MaxHeight = 30;
            chiren.RowDefinitions[1].MinHeight = 30;
        } else {
            sendMessage.Visibility = Visibility.Visible;
            publicMessage.Visibility = Visibility.Collapsed;
            chiren.RowDefinitions[1].MinHeight = 150;
            chiren.RowDefinitions[1].MaxHeight =300;
            chiren.RowDefinitions[1].Height = GridLength.Auto;

        }



    }

    void sendMessage_BnSendClick(MessageItem arg1, MsgType arg2) {
        if (arg1!=null && arg2 == MsgType.Text) {
            TextMessageControl b =new  TextMessageControl();

            b.Item = arg1;
            b.Left = false;
            panelList.Items.Add(b);
            if (arg1 != null) {
                MessageService.getInstance().DoSendMessage(arg1);
            }
        }  else   if (arg1 != null && arg2 == MsgType.Image) {
            ImageMessageControl b = new ImageMessageControl();
            b.Item = arg1;
            b.Left = false;
            panelList.Items.Add(b);
        } else if (arg1 != null && arg2 == MsgType.File) {
            FileMessageControl c= new FileMessageControl();
            c.Item = arg1;
            c.Left = false;
            panelList.Items.Add(c);

        } else if (arg1 != null && arg2 == MsgType.VCard) {
            VCardMessageControl c = new VCardMessageControl();
            c.Item = arg1;
            c.Left = false;
            panelList.Items.Add(c);
            MessageService.getInstance().DoSendMessage(arg1);
        } else if (arg1 != null && arg2 == MsgType.PublicCard) {
            PublicCardMessageControl c = new PublicCardMessageControl();
            c.Item = arg1;
            c.Left = false;
            panelList.Items.Add(c);

            MessageService.getInstance().DoSendMessage(arg1);

        } else if (arg1 != null && arg2 == MsgType.AVMeetingInvite) {
            AmMessageControl b = new AmMessageControl();

            b.Item = arg1;
            b.Left = false;
            panelList.Items.Add(b);
            if (arg1 != null) {
                MessageService.getInstance().DoSendMessage(arg1);
            }
        } else if (arg1 != null && arg2 == MsgType.MessageTypeAVGroupInvite) {
            NoticeMessageControl b = new NoticeMessageControl();
            b.Item = arg1;
            panelList.Items.Add(b);
            if (arg1 != null) {
                MessageService.getInstance().DoSendMessage(arg1);
            }
        }
        scrollViewer1.ScrollToBottom();




        //发出事件通知
        //BusinessEvent<object> businessEvent = new BusinessEvent<object>();
        //businessEvent.data = arg1;
        //businessEvent.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
        //EventBusHelper.getInstance().fireEvent(businessEvent);


    }

    //多选
    void control_DxClick(object sender, EventArgs e) {
        gridSet.Visibility = Visibility.Visible;
        foreach (MessageBase control in panelList.Items) {
            if (control.Item.type == MsgType.Text.ToStr()) {
                TextMessageControl textControl = control as TextMessageControl;
                if (textControl == null) continue;
                if (textControl.Left) {
                    textControl.RadioLeft.IsChecked = false;
                    textControl.RadioLeft.Visibility = Visibility.Visible;
                } else {
                    textControl.RadioRight.IsChecked = false;
                    textControl.RadioRight.Visibility = Visibility.Visible;
                }
            } else   if (control.Item.type == MsgType.Image.ToStr()) {
                ImageMessageControl imageControl = control as ImageMessageControl;
                if (imageControl == null)  continue;
                if ( imageControl.Left) {
                    imageControl.RadioLeft.IsChecked = false;
                    imageControl.RadioLeft.Visibility = Visibility.Visible;
                } else {

                    imageControl.RadioRight.IsChecked = false;
                    imageControl.RadioRight.Visibility = Visibility.Visible;

                }
            }


        }

    }



    //取消多选
    private void btnCancle_Click_1(object sender, RoutedEventArgs e) {
        gridSet.Visibility = Visibility.Collapsed;
        foreach (MessageBase control in panelList.Items) {
            if (control.Item.type == MsgType.Text.ToStr()) {
                TextMessageControl textControl = control as TextMessageControl;
                if (textControl == null) continue;
                if (textControl.Left) {
                    textControl.RadioLeft.IsChecked = false;
                    textControl.RadioLeft.Visibility = Visibility.Collapsed;
                } else {
                    textControl.RadioRight.IsChecked = false;
                    textControl.RadioRight.Visibility = Visibility.Collapsed;
                }
            } else  if (control.Item.type == MsgType.Image.ToStr()) {
                ImageMessageControl imageControl = control as ImageMessageControl;
                if (imageControl == null) continue;
                if (imageControl.Left) {
                    imageControl.RadioLeft.IsChecked = false;
                    imageControl.RadioLeft.Visibility = Visibility.Collapsed;
                } else {
                    imageControl.RadioRight.IsChecked = false;
                    imageControl.RadioRight.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    //删除
    private void btnDel_Click_1(object sender, RoutedEventArgs e) {
        for (int i = panelList.Items.Count-1; i > -1; i--) {
            MessageBase baseControl = panelList.Items[i] as MessageBase;
            if (baseControl != null && baseControl.Item.type == MsgType.Text.ToStr()) {
                TextMessageControl control = panelList.Items[i] as TextMessageControl;

                if (control != null && (control.RadioLeft.IsChecked == true || control.RadioRight.IsChecked == true)) {
                    MessageService.getInstance().deleteByMessageId(baseControl.Item.messageId);
                    panelList.Items.RemoveAt(i);
                }
            }
        }





    }

    /// <summary>
    /// 滚动条自动滚到最后
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void panel_SizeChanged_1(object sender, SizeChangedEventArgs e) {
        //if (load == false) return;
        //double d = this.panel.ActualHeight + this.panel.ViewportHeight + this.panel.ExtentHeight;
        //this.scrollViewer1.ScrollToVerticalOffset(d);
    }



    private void scrollViewer1_MouseWheel_1(object sender, MouseWheelEventArgs e) {

        double high = scrollViewer1.ScrollableHeight - scrollViewer1.VerticalOffset;
        if (high == Constants.SYS_ScHight|| high< Constants.SYS_ScHight) {

            newMsgAdorner.Visibility = Visibility.Hidden;
            count = 0;

        }
        //向下就return
        if (e.Delta < 0) return;
        if (load == false) return;


        //滚到最底
        //if (scrollViewer1.ScrollableHeight == e.VerticalOffset && e.VerticalOffset != 0)
        //滚到最顶
        if (scrollViewer1.ScrollableHeight > 0 && scrollViewer1.VerticalOffset == 0 ) {

            //加载所有
            _messagesList = MessageService.getInstance().getMessagesByPage(sendMessage.ToUserNo, lastTimestamp);


            LoadMessage(_messagesList,0);
            if (_messagesList.Count == 0) return;
            if (_messagesList[_messagesList.Count - 1].timestamp != null) {
                lastTimestamp = long.Parse(_messagesList[_messagesList.Count - 1].timestamp);
            }
            //scrollViewer1.ScrollToHome();
        }
    }

    public event Action<string> BtnAddPersonClick;
    private void BtnAddPerson_OnClick(object sender, RoutedEventArgs e) {
        if (BtnAddPersonClick != null && e != null) {

            BtnAddPersonClick.Invoke(_mucNo);
        }
    }

    public event Action<string> BtnGroupChatDetailedClick;
    private void BtnGroupChatDetailed_Click(object sender, RoutedEventArgs e) {
        if (BtnGroupChatDetailedClick != null && e != null) {

            BtnGroupChatDetailedClick.Invoke(_mucNo);
        }
    }

    public event Action<string> BtnSingleChatDetailedClick;
    private void BtnSingleChatDetailed_Click(object sender, RoutedEventArgs e) {
        if (BtnSingleChatDetailedClick != null && e != null) {

            BtnSingleChatDetailedClick.Invoke(_clientuserId);
        }
    }

    /// <summary>
    /// 退出当前页面
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Page_Unloaded(object sender, RoutedEventArgs e) {
        // Todo  保存草稿
        sendMessage.TxtMessage.Clear();
        //if(clearPaneList) {
        //    panelList.Items.Clear();
        //}

    }

    /// <summary>
    /// 公众号设置
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnSetting_Click(object sender, RoutedEventArgs e) {
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = PublicAppId;
        Businessdata.eventDataType = BusinessEventDataType.ClickPublicSetting;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }
    public event Action<string> BtnBackOnClick;
    /// <summary>
    /// 返回公众号
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        if (BtnBackOnClick != null && e != null) {

            BtnBackOnClick.Invoke(ClientuserId);
        }
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e) {
        publicMessage.CreatViewSize();
    }
}
}
