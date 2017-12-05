using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.AgoraVideoClr;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {

/// <summary>
///     WinVideo.xaml 的交互逻辑
/// </summary>
public partial class WinReceiving : Window {

    private AVMeetingType meetingType = AVMeetingType.audio;
    private PlaySoundBase playSound;
    bool FormFocus = false;
    /// <summary>
    /// 对方头像
    /// </summary>
    public BitmapImage GuestAvatar {
        set;
        get;
    }
    public string RoomId {
        set;
        get;
    }
    /// <summary>
    /// 对方名字
    /// </summary>
    public string GuestName {
        set;
        get;
    }
    public string GuestNo {
        set;
        get;
    }
    /// <summary>
    /// 聊天类型图片路径
    /// </summary>
    public string MeetingImg {
        set;
        get ;
    }

    public MessageTypeAVGroupInviteMessage AVGroupInviteMessage {
        set;
        get;
    }

    public AVMeetingInviteMessage AVMeetingInviteMessage {
        set;
        get;
    }

    public WinReceiving(AVMeetingType type) {
        InitializeComponent();
        playSound = new PlaySoundBase(type);
        this.DataContext = this;
        this.meetingType = type;
        if (this.meetingType.Equals(AVMeetingType.video)) {
            MeetingImg = "pack://application:,,,/ResourceDictionary;Component/Images/AVMeeting/VideoReceiving.png";
            LblMettingTip.Content = "邀您视频聊天";
            BtnToAudio.Visibility = Visibility.Visible;
        } else {
            MeetingImg = "pack://application:,,,/ResourceDictionary;Component/Images/AVMeeting/AudioReceiving.png";
            LblMettingTip.Content = "邀您语音聊天";
            BtnToAudio.Visibility = Visibility.Collapsed;
        }
    }
    private System.Timers.Timer timer = new System.Timers.Timer();
    private void WinVideo_OnLoaded(object sender, RoutedEventArgs e) {
        //设置timer可用
        timer.Enabled = true;
        //设置timer
        timer.Interval = Constants.AmTimeOut;

        //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
        timer.AutoReset = true;

        timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

        timer.Start();

        App.AmStatus = AVMeetingStatus.waiting;
        var hwnd = new WindowInteropHelper(this).Handle;
        NativeWindowHelper.SetWindowNoBorder(hwnd);
        playSound.PlayWaitSound();



    }

    //超时没有答复发送消息
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        timer.Stop();
        AVMeetingService.getInstance()
        .SendAVMeetingTimeOutMessage(GuestNo, meetingType.ToStr(), App.AccountsModel.clientuserId.ToInt(), RoomId,true);
        this.Dispatcher.BeginInvoke((Action)delegate () {
            // 1、停止提示音
            playSound.StopWaitSound();
            playSound.PlayCancelSound();
            App.AmStatus = AVMeetingStatus.no;
            FormFocus = true;
            this.Close();
        });
    }
    private void TitlePanel_OnMouseMove(object sender, MouseEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            DragMove();
        }
    }

    private void BtnClose_OnClick(object sender, RoutedEventArgs e) {
        FormFocus = true;
        CloseMethod(true);
    }

    private void BtnAccept_OnClick(object sender, RoutedEventArgs e) {
        App.AmCallType = AVMeetingCallType.called;
        // 显示视频or音频窗体
        playSound.StopWaitSound(); // 停止提示音
        timer.Stop();
        App.AmStatus = AVMeetingStatus.contected;
        //发送已接通消息
        AVMeetingService.getInstance()
        .SendAVMeetingConnectMessage(GuestNo, meetingType.ToStr(), App.AccountsModel.clientuserId.ToInt(), RoomId,"PC");
        if (this.meetingType.Equals(AVMeetingType.video)) {

            WinVideo localVideo = new WinVideo();

            VcardsTable guestModel = VcardService.getInstance().findByNo(GuestNo);
            //string channelName = "zwt123456"; //TODO: Test Used
            string channelName = RoomId;// 聊天对方No
            // 创建窗体

            localVideo.ViewModel.ChannelName = channelName;
            localVideo.ViewModel.IsMainWin = true;
            localVideo.ViewModel.IsLocal = true;
            localVideo.ViewModel.GuestName = guestModel.nickname;
            localVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
            localVideo.Height = 600;
            localVideo.Width = 345;


            AgoraEngine.Instance.VideoWindow = new WinButton();
            AgoraEngine.Instance.VideoWindow.ObjViewModel.GuestNo = GuestNo;
            AgoraEngine.Instance.VideoWindow.AVGroupInviteMessage = AVGroupInviteMessage;

            AgoraEngine.Instance.VideoWindow.AVMeetingInviteMessage = AVMeetingInviteMessage;

            AgoraEngine.Instance.VideoWindow.ObjViewModel.ChannelName = RoomId;
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
            AgoraEngine.Instance.JoinVideoChannel(channelName, App.AccountsModel.clientuserId.ToInt());

        } else {
            string channelName = RoomId; //TODO: Test Used
            VcardsTable guestModel = VcardService.getInstance().findByNo(GuestNo);

            // 弹出语音窗体
            AgoraEngine.Instance.AudioWindow = new WinAudio(channelName);
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestNo = GuestNo;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.ChannelName = RoomId;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestName = guestModel.nickname;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);

            // 加入频道
            AgoraEngine.Instance.JoinAudioChannel(channelName, App.AccountsModel.clientuserId.ToInt());
            Console.WriteLine(@"JoinChannel!! Name:{0}", channelName);
            AgoraEngine.Instance.AudioWindow.Show(); // 显示窗体
        }
        FormFocus = true;
        this.Close();
    }
    public void CloseMethod() {
        playSound.StopWaitSound();
        playSound.PlayCancelSound();
        timer.Stop();
        FormFocus = true;
        App.AmStatus = AVMeetingStatus.no;
        AgoraEngine.Instance.ReceivingWindow = null;
        this.Close();
    }
    private void BtnRejection_OnClick(object sender, RoutedEventArgs e) {
        CloseMethod(true);
    }

    public void CloseMethod(bool sendMessage) {
        // 1、停止提示音
        playSound.StopWaitSound();
        playSound.PlayCancelSound();
        timer.Stop();
        // 2、发送IM挂断消息
        //发送申请的消息
        FormFocus = true;
        if (sendMessage) {
            AVMeetingService.getInstance()
            .SendAVMeetingRefuseMessage(GuestNo, meetingType.ToStr(), App.AccountsModel.clientuserId.ToInt(), RoomId);
        }
        App.AmStatus = AVMeetingStatus.no;
        this.Close();


    }


    private void BtnToAudio_Click(object sender, RoutedEventArgs e) {

        App.AmStatus = AVMeetingStatus.contected;
        // 停止提示音
        playSound.StopWaitSound();
        timer.Stop();
        string channelName = RoomId; //TODO: Test Used

        VcardsTable guestModel = VcardService.getInstance().findByNo(GuestNo);


        // 弹出语音窗体
        AgoraEngine.Instance.AudioWindow = new WinAudio(channelName, true);
        AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestName = guestModel.nickname;
        AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
        AgoraEngine.Instance.AudioWindow.timer.Stop();
        // 加入频道
        //AgoraEngine.Instance.Video2Audio();
        AgoraEngine.Instance.JoinAudioChannel(channelName, App.AccountsModel.clientuserId.ToInt());
        //Console.WriteLine(@"JoinChannel!! Name:{0} Code:{1}", channelName);
        AgoraEngine.Instance.AudioWindow.Show(); // 显示窗体

        // 发送IM切换到语音消息
        AVMeetingService.getInstance()
        .SendAVMeetingSwitchMessage(GuestNo, AVMeetingType.audio.ToStr(), App.AccountsModel.clientuserId.ToInt(), RoomId, AVMeetingStatus.contected.ToStr());

        FormFocus = true;
        this.Close();
    }



    private void ReceivingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        if (FormFocus == false) {
            e.Cancel = true;
            if (CommonMessageBox.Msg.Show("关闭窗口将拒绝聊天，确定关闭么 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                CloseMethod(true);
            }

        } else {
            {
                AgoraEngine.Instance.ReceivingWindow = null;
            }
        }
    }
}
}