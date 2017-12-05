using System;
using System.Collections.Generic;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using cn.lds.chatcore.capture;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using com.fasterxml.jackson.annotation;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Emoji.Entity;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Windows;
using EventBus;
using Microsoft.Win32;
using cn.lds.chatcore.pcw.AgoraVideoClr;
using cn.lds.chatcore.pcw.Views.Windows.AVMeeting;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// SendMessageControl.xaml 的交互逻辑
/// </summary>
public partial class SendMessageControl : UserControl {

    public SendMessageControl() {
        InitializeComponent();
        this.EmojiTabControl.SetEmojiHitEvent(OnEmojiClick); // 选择表情事件
    }

    //变量定义
    public event Action<MessageItem, MsgType> BnSendClick;
    public event Action<bool> ClickBtnCollapsed;

    public string ToUserNo;

    /// <summary>
    /// 是否显示折叠按钮
    /// </summary>
    public bool ShowCollapsedButton = false;

    private ChatSessionType chatSessionType;

    public ChatSessionType ChatSessionType {
        get {
            return chatSessionType;
        } set {
            chatSessionType = value;
            Init();
        }
    }

    private void Init() {
        try {
            if (ChatSessionType == ChatSessionType.PUBLIC && ShowCollapsedButton == true) {
                BtnCollapsed.Visibility = Visibility.Visible;

            } else {
                BtnCollapsed.Visibility = Visibility.Collapsed;
            }

            if (ChatSessionType == ChatSessionType.CHAT && ToUserNo != App.AccountsModel.no) {
                BtnYy.Visibility = Visibility.Visible;
                BtnSp.Visibility = Visibility.Visible;
            } else if (ChatSessionType == ChatSessionType.MUC ) {
                BtnYy.Visibility = Visibility.Collapsed;
                BtnSp.Visibility = Visibility.Collapsed;
            } else {
                BtnYy.Visibility = Visibility.Collapsed;
                BtnSp.Visibility = Visibility.Collapsed;

            }
            //获取button的资源 修改资源的图片  否则需要每个按钮都建立资源 太麻烦
            //Image btnTryImage1 = CommonMethod.GetChildObject<Image>(this.btn1, "bg");
            //if (btnTryImage1 == null) return;
            //btnTryImage1.Source = new BitmapImage(new Uri("../images/Emoj.png", UriKind.RelativeOrAbsolute));

        } catch (Exception ex) {
            Log.Error(typeof (SendMessageControl), ex);
        }
    }

    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            //if(ChatSessionType== ChatSessionType.PUBLIC) {

            //    main.ColumnDefinitions[0].Width = new GridLength(32);
            //} else {
            //    main.ColumnDefinitions[0].Width = new GridLength(0);
            //}
            //获取button的资源 修改资源的图片  否则需要每个按钮都建立资源 太麻烦
            //Image btnTryImage1 = CommonMethod.GetChildObject<Image>(this.btn1, "bg");
            //if (btnTryImage1 == null) return;
            //btnTryImage1.Source = new BitmapImage(new Uri("../images/Emoj.png", UriKind.RelativeOrAbsolute));


            this.TxtMessage.Clear();
            this.TxtMessage.SendMsgEvent += BtnSend_Click;
        } catch (Exception ex) {
            Log.Error(typeof (SendMessageControl), ex);
        }
    }



    #region 按钮事件

    private void BtnCollapsed_Click(object sender, RoutedEventArgs e) {
        if (ClickBtnCollapsed != null) {
            ClickBtnCollapsed.Invoke(false);
        }
    }

    /// <summary>
    /// 消息发送按钮点击
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnSend_Click(object sender, RoutedEventArgs e) {
        try {

            if (BnSendClick != null) {
                List<MessageItem> listMsg = this.TxtMessage.ProcessSendMsg();
                if (listMsg.Count == 0) return;

                if(ChatSessionType == ChatSessionType.MUC) {
                    MucTable dt= MucServices.getInstance().FindGroupByNo(ToUserNo);
                    if(dt==null) {
                        NotificationHelper.ShowInfoMessage("您已退出该群！");
                        return;
                    }
                } else if (ChatSessionType == ChatSessionType.CHAT) {
                    ContactsTable dt = ContactsServices.getInstance().FindContactsByNo(ToUserNo);
                    if (dt == null && ToUserNo!=App.AccountsModel.no) {
                        NotificationHelper.ShowInfoMessage("非好友状态不可以聊天！");
                        return;
                    }
                } else if (ChatSessionType == ChatSessionType.PUBLIC) {
                    PublicAccountsTable dtPub = PublicAccountsService.getInstance().findByAppId(ToUserNo);
                    if (dtPub == null || dtPub.status == "0") {
                        NotificationHelper.ShowInfoMessage("该公众号不可用");
                        return;
                    }
                }
                foreach (MessageItem msg in listMsg) {
                    MessageItem item = null;
                    if (msg.type.Equals(MsgType.Text.ToStr())) {
                        if(String.IsNullOrEmpty(msg.user)) {
                            // 文字
                            item = MessageService.getInstance()
                                   .sendTextMessage(ToUserNo, App.AccountsModel.no, msg.content);

                        } else {
                            // @消息
                            List<string> userList=  msg.user.Split(new string [] { ","},StringSplitOptions.RemoveEmptyEntries).ToList();
                            item = MessageService.getInstance()
                                   .sendAtMessage(ToUserNo, msg.content, userList);
                        }
                        if (item != null) {
                            BnSendClick.Invoke(item, MsgType.Text);
                        }
                    } else if (msg.type.Equals(MsgType.Image.ToStr())) {

                        //生成messageid
                        String messageId = ImClientService.getInstance().generateMessageId();

                        //生成base64
                        Bitmap image = new Bitmap(this.TxtMessage.DicSelectedFiles[msg.content]);

                        //构建item为了展示
                        string base64 = ImageHelper.ImageToBase64(image, ImageFormat.Jpeg);
                        item = MessageService.getInstance()
                               .sendPictureMessage(messageId, ToUserNo, string.Empty, base64, image.Width, image.Height);

                        Dictionary<String, Object> param = new Dictionary<string, object>();
                        msg.user = ToUserNo;
                        msg.messageId = messageId;
                        param.Add("item", msg);
                        if (item != null) {
                            BnSendClick.Invoke(item, MsgType.Image);
                        }
                        // 上传图片
                        UploadFileService uploadFileService = new UploadFileService(UploadFileType.MSG_IMAGE,
                                messageId, msg.content, param);
                        uploadFileService.uploadAsync();


                    } else if (msg.type.Equals(MsgType.File.ToStr())) {

                        //生成messageid
                        String messageId = ImClientService.getInstance().generateMessageId();
                        string filePath = msg.content;
                        FileInfo file = new FileInfo(filePath);
                        item = MessageService.getInstance()
                               .sendFileMessage(messageId, ToUserNo, App.AccountsModel.no, msg.content, file.Name, file.Length);
                        item.filePath = filePath;
                        Dictionary<String, Object> param = new Dictionary<string, object>();
                        msg.user = ToUserNo;
                        msg.messageId = messageId;
                        param.Add("item", msg);
                        if (item != null) {
                            BnSendClick.Invoke(item, MsgType.File);
                        }
                        // 上传图片
                        UploadFileService uploadFileService = new UploadFileService(UploadFileType.MSG_FILE,
                                messageId, msg.content, param);
                        uploadFileService.uploadAsync();


                    }
                    //if (item != null) {
                    //    BnSendClick.Invoke(item, (MessageType)Enum.Parse(typeof(MessageType), msg.type)  );
                    //}
                }
                this.TxtMessage.Clear();
            }
        } catch (Exception ex) {
            Log.Error(typeof (SendMessageControl), ex);
        }
    }


    /// <summary>
    /// 插入图片
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnTT_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        //openFileDialog.Filter = "图片 |*.png;*.jpg;*.jpeg;*.bmp|全部文件 (*.*)|*.*";
        openFileDialog.Filter = "图片 |*.png;*.jpg;*.jpeg;*.bmp;*.gif";
        openFileDialog.RestoreDirectory = false;
        openFileDialog.Multiselect = true;
        if (openFileDialog.ShowDialog() == true) {
            foreach (string path in openFileDialog.FileNames) {
                if (this.TxtMessage.CountSelectedFiles() >= 5) {
                    NotificationHelper.ShowWarningMessage("您最多只能选择5个文件！");
                    break;
                }
                this.TxtMessage.InsertPicture(path);
            }
        }
    }

    /// <summary>
    /// 截图
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnJt_Click(object sender, RoutedEventArgs e) {
        PcStart pcStart = PcStart.getInstance();
        pcStart.StartCapture(this.TxtMessage);
    }

    #endregion

    private void BtnBq_Click(object sender, RoutedEventArgs e) {
        this.EmojiPopup.IsOpen = true;
    }

    private void OnEmojiClick(object sender, RoutedEventArgs e) {
        EmojiItem item = ((EmojiHitEventArgs) e).TargetEmojiItem;
        this.EmojiPopup.IsOpen = false;
        if (item.Type.Equals(EmojiType.Defaults)) {
            this.TxtMessage.InsertEmoji(item.ImgPath);
        } else {
            this.TxtMessage.InsertPicture(item.ImgPath);
        }
    }

    private void BtnWj_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "全部文件 (*.*)|*.*";
        openFileDialog.RestoreDirectory = false;
        openFileDialog.Multiselect = true;
        if (openFileDialog.ShowDialog() == true) {
            foreach (string path in openFileDialog.FileNames) {
                if (this.TxtMessage.CountSelectedFiles() >= 5) {
                    NotificationHelper.ShowWarningMessage("您最多只能选择5个文件！");
                    break;
                }
                String fileExtension = path.Substring(path.LastIndexOf("."));
                if (TxtMessage.ImagesExtension.Contains(fileExtension.ToLower())) {
                    this.TxtMessage.InsertPicture(path);
                } else {
                    this.TxtMessage.InsertFile(path);
                }
            }
        }
    }

    /// <summary>
    /// 点击定位
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnDw_Click(object sender, RoutedEventArgs e) {

        // 构建位置信息 TODO测试的代码哦。
        //LocationEventData locationEventData = new LocationEventData();
        //locationEventData.address = "辽宁省大连市甘井子区小平岛路";
        //locationEventData.latitude = 38.850965266489958;
        //locationEventData.longitude = 121.517656885609;
        //locationEventData.scale = 16;

        // 不发送消息，直接显示画面定位
        LocationEventData locationEventData = null;

        //发送位置通知
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = locationEventData;
        Businessdata.eventDataType = BusinessEventDataType.ClickLocation;
        EventBusHelper.getInstance().fireEvent(Businessdata);

    }
    /// <summary>
    /// 发送网盘文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnYp_Click(object sender, RoutedEventArgs e) {

        DiskFileEventData diskFileEventData = null;
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = diskFileEventData;
        Businessdata.eventDataType = BusinessEventDataType.ClickDiskFile;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

    /// <summary>
    /// 名片
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnMp_Click(object sender, RoutedEventArgs e) {
        SendCardWindow win = new SendCardWindow();
        win.ToUserNo = ToUserNo;
        win.ShowDialog();
    }


    private void BtnYy_OnClick(object sender, RoutedEventArgs e) {
        if (AgoraEngine.Instance.AudioWindow != null) return;
        App.AmCallType = AVMeetingCallType.calling;
        ChatPage chatPage = ChatPage.getInstance();
        App.AmStatus = AVMeetingStatus.waiting;
        if (chatPage.ChatSessionType.Equals(ChatSessionType.CHAT)) {
            // 生成频道名
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000;
            string channelName = chatPage.UserNo + ticks.ToString();// 聊天对方No
            //string channelName = "zwt123456"; //TODO: Test Used
            VcardsTable guestModel = VcardService.getInstance().findByNo(chatPage.UserNo);

            //App.AccountsModel.no; // 当前用户No

            //发送申请的消息
            MessageItem item = MessageService.getInstance()
                               .SendAVMeetingInviteMessage(ToUserNo, AVMeetingType.audio, uint.Parse(App.AccountsModel.clientuserId), channelName);
            if (item != null) {
                BnSendClick.Invoke(item, MsgType.AVMeetingInvite);
            }

            // 弹出语音窗体
            AgoraEngine.Instance.AudioWindow = new WinAudio(channelName);
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.ChannelName = channelName;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestNo = guestModel.no;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestName = guestModel.nickname;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            AgoraEngine.Instance.JoinAudioChannel(channelName, App.AccountsModel.clientuserId.ToInt());
            AgoraEngine.Instance.AudioWindow.Show(); // 显示窗体

        } else if (chatPage.ChatSessionType.Equals(ChatSessionType.MUC)) {


            // 生成频道名
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000;
            string channeName = chatPage.MucNo + ticks.ToString();// 聊天对方No

            List<MucMembersTable> members = MucServices.getInstance().findByMucNo(ToUserNo);
            MucTable muc= MucServices.getInstance().FindGroupByNo(ToUserNo);
            List<SelectPeopleSubViewModel> userCard = new List<SelectPeopleSubViewModel>();
            foreach (MucMembersTable member in members) {
                SelectPeopleSubViewModel model = new SelectPeopleSubViewModel();
                model.AddRoomTimestemp = 0;
                model.AvatarStorageRecordId = member.avatarStorageRecordId;
                model.GroupId = muc.mucId;
                model.GroupImId = muc.no;
                model.MemberId = member.clientuserId;
                model.MemberImId = member.no;
                model.NickName = member.nickname;
                model.MeetingType = AVMeetingType.audio;
                model.AddRoomTimestemp = DateTimeHelper.getTimeStamp();
                userCard.Add(model);
            }
            //    List<SelectPeopleSubViewModel> userCard = new List<SelectPeopleSubViewModel>();
            //SelectPeopleSubViewModel model = new SelectPeopleSubViewModel();
            //model.AddRoomTimestemp = 0;
            //model.AvatarStorageRecordId = App.AccountsModel.avatarStorageRecordId;
            //model.GroupId = muc.mucId;
            //model.GroupImId = muc.no;
            //model.MemberId = App.AccountsModel.clientuserId;
            //model.MemberImId = App.AccountsModel.no;
            //model.NickName = App.AccountsModel.name;
            //model.MeetingType = AVMeetingType.audio;
            //userCard.Add(model);
            //发送申请的消息
            MessageItem item = MessageService.getInstance()
                               .SendAVGroupInviteMessage(ToUserNo, userCard, channeName);
            if (item != null) {
                BnSendClick.Invoke(item, MsgType.MessageTypeAVGroupInvite);
            }

            // 创建窗体
            WinVideo localVideo = new WinVideo();
            localVideo.ViewModel.ChannelName = channeName;
            localVideo.ViewModel.IsMainWin = true;
            localVideo.ViewModel.IsLocal = true;
            localVideo.ViewModel.GuestNo = App.AccountsModel.no;
            localVideo.ViewModel.GuestName = App.AccountsModel.nickname;
            localVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(App.AccountsModel.avatarStorageRecordId);
            localVideo.Height = 600;
            localVideo.Width = 345;

            AgoraEngine.Instance.VideoWindow = new WinButton();
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestNo = ToUserNo;

            imtp.message.Message messageBean = new MessageTypeAVGroupInviteMessage().toModel(item.content);
            messageBean.setMessageId(item.messageId);
            messageBean.setType(MsgType.MessageTypeAVGroupInvite);
            messageBean.setTo(item.user);
            AgoraEngine.Instance.VideoWindow.AVGroupInviteMessage = messageBean as MessageTypeAVGroupInviteMessage;


            AgoraEngine.Instance.VideoWindow.ObjViewModel.ChannelName = channeName;
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestName = App.AccountsModel.nickname;
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestAvatar = ImageHelper.loadAvatarPath(App.AccountsModel.avatarStorageRecordId);
            AgoraEngine.Instance.VideoWindow.WinVideos.Add(localVideo);
            localVideo.ViewModel.ButtonWin = AgoraEngine.Instance.VideoWindow;

            // 弹出视频窗体
            localVideo.Show();
            AgoraEngine.Instance.VideoWindow.Owner = localVideo;
            AgoraEngine.Instance.VideoWindow.Width = localVideo.ActualWidth;
            AgoraEngine.Instance.VideoWindow.Height = localVideo.ActualHeight;
            AgoraEngine.Instance.VideoWindow.Left = localVideo.Left;
            AgoraEngine.Instance.VideoWindow.Top = localVideo.Top;
            AgoraEngine.Instance.VideoWindow.Show();
            // 显示本地视频
            AgoraEngine.Instance.SetupLocalVideo(localVideo, true);
            // 加入频道
            AgoraEngine.Instance.JoinVideoChannel(channeName, App.AccountsModel.clientuserId.ToInt());

        }
    }

    private void BtnSp_OnClick(object sender, RoutedEventArgs e) {
        if (AgoraEngine.Instance.VideoWindow != null) return;
        App.AmCallType = AVMeetingCallType.calling;
        ChatPage chatPage = ChatPage.getInstance();
        App.AmStatus = AVMeetingStatus.waiting;
        if (chatPage.ChatSessionType.Equals(ChatSessionType.CHAT)) {
            // 生成频道名
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000;
            string channeName = chatPage.UserNo + ticks.ToString();// 聊天对方No
            //string channeName = "zwt123456";
            VcardsTable guestModel = VcardService.getInstance().findByNo(chatPage.UserNo);

            //发送申请的消息
            MessageItem item = MessageService.getInstance()
                               .SendAVMeetingInviteMessage(ToUserNo, AVMeetingType.video, uint.Parse(App.AccountsModel.clientuserId), channeName);
            if (item != null) {
                BnSendClick.Invoke(item, MsgType.AVMeetingInvite);
            }

            // 创建窗体
            WinVideo localVideo = new WinVideo();
            localVideo.ViewModel.ChannelName = channeName;
            localVideo.ViewModel.IsMainWin = true;
            localVideo.ViewModel.IsLocal = true;
            localVideo.ViewModel.GuestNo = guestModel.no;
            localVideo.ViewModel.GuestName = guestModel.nickname;
            localVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            localVideo.Height = 600;
            localVideo.Width = 345;

            AgoraEngine.Instance.VideoWindow  = new WinButton();
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestNo = ToUserNo;
            AgoraEngine.Instance.VideoWindow.ObjViewModel.ChannelName = channeName;
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestName = guestModel.nickname;
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            AgoraEngine.Instance.VideoWindow.WinVideos.Add(localVideo);
            localVideo.ViewModel.ButtonWin = AgoraEngine.Instance.VideoWindow;

            // 弹出视频窗体
            localVideo.Show();
            AgoraEngine.Instance.VideoWindow.Owner = localVideo;
            AgoraEngine.Instance.VideoWindow.Width = localVideo.ActualWidth;
            AgoraEngine.Instance.VideoWindow.Height = localVideo.ActualHeight;
            AgoraEngine.Instance.VideoWindow.Left = localVideo.Left;
            AgoraEngine.Instance.VideoWindow.Top = localVideo.Top;
            AgoraEngine.Instance.VideoWindow.Show();
            // 显示本地视频
            AgoraEngine.Instance.SetupLocalVideo(localVideo, true);
            // 加入频道
            AgoraEngine.Instance.JoinVideoChannel(channeName, App.AccountsModel.clientuserId.ToInt());
        }
    }
}
}
