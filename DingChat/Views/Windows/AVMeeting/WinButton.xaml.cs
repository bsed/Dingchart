using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.AgoraVideoClr;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;
using Timer = System.Timers.Timer;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
/// <summary>
///     WinButton.xaml 的交互逻辑
/// </summary>
public partial class WinButton : Window, AgoraEngineEventInterface {

    private Timer timerLoading = new Timer();
    private PlaySoundBase playSound;
    public  bool FormFocus = false;
    private int lastVolume = 50;
    public AVMettingViewModeBase ObjViewModel;
    public WinButton() {
        InitializeComponent();
        WinVideos = new List<WinVideo>();
        ObjViewModel = new AVMettingViewModeBase();
        playSound = new PlaySoundBase(AVMeetingType.video);
        this.DataContext = ObjViewModel;
    }

    public List<WinVideo> WinVideos {
        set;
        get;
    }
    public MessageTypeAVGroupInviteMessage AVGroupInviteMessage {
        set;
        get;
    }

    public AVMeetingInviteMessage AVMeetingInviteMessage {
        set;
        get;
    }

    private void TitlePanel_OnMouseMove(object sender, MouseEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            DragMove();
        }
    }

    private void SetLocation(int i) {
        WinVideo mainWinVideo = this.WinVideos.Find(q => {
            return q.ViewModel.IsMainWin == true;
        });

        double leftPos = this.WinVideos[i - 1].ViewModel.IsMainWin
                         ? this.WinVideos[i - 1].Left + 10
                         : this.WinVideos[i - 1].Left + this.WinVideos[i - 1].ActualWidth + 10;
        double topPos = mainWinVideo.Top + 30;
        //this.WinVideos[WinVideos.Count - 1].ViewModel.IsMainWin ? this.WinVideos[WinVideos.Count - 1].Top + 30 : this.WinVideos[WinVideos.Count - 1].Top + this.WinVideos[WinVideos.Count - 1].ActualHeight + 30;
        if ((leftPos + 90).CompareTo(mainWinVideo.Left + mainWinVideo.ActualWidth) > 0) {
            leftPos = mainWinVideo.Left + 10;
            topPos = this.WinVideos[i - 1].Top + this.WinVideos[i - 1].ActualHeight +
                     10;

        }
        this.WinVideos[i ].Left = leftPos;
        this.WinVideos[i].Top = topPos;
    }
    private void WinButton_OnLocationChanged(object sender, EventArgs e) {
        for (int i = 0; i < WinVideos.Count; i++) {
            var video = WinVideos[i];
            if (video.ViewModel.IsMainWin) {
                video.Left = Left;
                video.Top = Top;
            } else {
                SetLocation(i);
                //video.Topmost = true;
            }
        }
    }

    private System.Timers.Timer chargingTimer = new System.Timers.Timer();
    private AVMeetingTimeChargingTable chargingTable = null;
    private System.Timers.Timer timer = new System.Timers.Timer();
    private List<SelectPeopleSubViewModel> listUser = new List<SelectPeopleSubViewModel>();
    private void WinButton_OnLoaded(object sender, RoutedEventArgs e) {

        try {
            ShowInTaskbar = false;

            TxtblTooltip.Visibility = Visibility.Collapsed;

            if (!ObjViewModel.IsConnected) playSound.PlayWaitSound();



            //设置timer可用
            timer.Enabled = true;
            //设置timer
            timer.Interval = Constants.AmTimeOut + 500;

            //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
            timer.AutoReset = true;

            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            if (AVGroupInviteMessage != null) {
                listUser = AVGroupInviteMessage.getUserCard();
                ChangeMeetingType(uint.Parse(App.AccountsModel.clientuserId), AVMeetingType.video);
                List<SelectPeopleSubViewModel> list = new List<SelectPeopleSubViewModel>();
                for (int i = 0; i < listUser.Count; i++) {
                    if (listUser[i].MemberImId != App.AccountsModel.no) {
                        list.Add(listUser[i]);
                    }
                }
                for (int i = 0; i < list.Count; i++) {

                    WinVideo guestWinVideo = new WinVideo(); // 创建视频窗口
                    guestWinVideo.ViewModel.ButtonWin = this;
                    guestWinVideo.ViewModel.Uid = uint.Parse(list[i].MemberId);
                    guestWinVideo.ShowInTaskbar = false;
                    guestWinVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(list[i].AvatarStorageRecordId);
                    guestWinVideo.Height = 120;
                    guestWinVideo.Width = 90;
                    guestWinVideo.Owner = this;

                    WinVideo mainWinVideo = this.WinVideos.Find(q => {
                        return q.ViewModel.IsMainWin == true;
                    });
                    double leftPos = this.WinVideos[WinVideos.Count - 1].ViewModel.IsMainWin
                                     ? this.WinVideos[WinVideos.Count - 1].Left + 10
                                     : this.WinVideos[WinVideos.Count - 1].Left + this.WinVideos[WinVideos.Count - 1].ActualWidth + 10;
                    double topPos = mainWinVideo.Top + 30;
                    //this.WinVideos[WinVideos.Count - 1].ViewModel.IsMainWin ? this.WinVideos[WinVideos.Count - 1].Top + 30 : this.WinVideos[WinVideos.Count - 1].Top + this.WinVideos[WinVideos.Count - 1].ActualHeight + 30;
                    if ((leftPos + 90).CompareTo(mainWinVideo.Left+mainWinVideo.ActualWidth) > 0) {
                        leftPos = mainWinVideo.Left + 10;
                        topPos = this.WinVideos[WinVideos.Count - 1].Top + this.WinVideos[WinVideos.Count - 1].ActualHeight +
                                 10;
                    }
                    guestWinVideo.Left = leftPos;
                    guestWinVideo.Top = topPos;
                    this.WinVideos.Add(guestWinVideo);
                    guestWinVideo.Show();
                }
            } else {
                WinVideo guestWinVideo = new WinVideo(); // 创建视频窗口
                guestWinVideo.ViewModel.ButtonWin = this;
                guestWinVideo.ShowInTaskbar = false;
                guestWinVideo.Height = 120;
                guestWinVideo.Width = 90;
                guestWinVideo.Owner = this;
                double leftPos = this.WinVideos[0].Left + 10;
                double topPos = this.WinVideos[0].Top + 30;
                guestWinVideo.Left = leftPos;
                guestWinVideo.Top = topPos;

                //都构造出来和群聊视频一样的list
                SelectPeopleSubViewModel model = new SelectPeopleSubViewModel();
                model.MemberId = App.AccountsModel.clientuserId;
                model.AvatarStorageRecordId = App.AccountsModel.avatarStorageRecordId;
                model.MeetingType = AVMeetingType.video;
                listUser.Add(model);

                //个人单聊视频邀请   都构造出来和群聊视频一样的list
                if (AVMeetingInviteMessage != null) {
                    model = new SelectPeopleSubViewModel();
                    model.MemberId = AVMeetingInviteMessage.getUId().ToStr();

                    VcardsTable guestModel = VcardService.getInstance().findByClientuserId(AVMeetingInviteMessage.getUId());
                    if(guestModel!=null) {
                        guestWinVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
                        model.AvatarStorageRecordId = guestModel.avatarStorageRecordId;
                    }
                    guestWinVideo.ViewModel.Uid = AVMeetingInviteMessage.getUId();



                    model.MeetingType = AVMeetingType.video;
                    listUser.Add(model);

                } else {//主动发起单聊视频邀请

                    model = new SelectPeopleSubViewModel();
                    model.MemberId = ChatPage.getInstance().ClientuserId.ToStr();

                    VcardsTable guestModel = VcardService.getInstance().findByClientuserId(ChatPage.getInstance().ClientuserId.ToInt());
                    if (guestModel != null) {
                        model.AvatarStorageRecordId = guestModel.avatarStorageRecordId;
                        guestWinVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
                    }
                    guestWinVideo.ViewModel.Uid = uint.Parse(ChatPage.getInstance().ClientuserId);
                    model.MeetingType = AVMeetingType.video;
                    listUser.Add(model);
                }

                this.WinVideos.Add(guestWinVideo);
                guestWinVideo.Show();

            }


        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        timer.Stop();
        AVMeetingService.getInstance()
        .SendAVMeetingTimeOutMessage(ObjViewModel.GuestNo, AVMeetingType.video.ToStr(), App.AccountsModel.clientuserId.ToInt(), ObjViewModel.ChannelName, false);
        this.Dispatcher.BeginInvoke((Action)delegate () {
            CloseMethod(false,true);
        });

    }
    private void BtnMin_OnClick(object sender, RoutedEventArgs e) {
        WindowState = WindowState.Minimized;
        foreach (var video in WinVideos) {
            if (video.ViewModel.IsMainWin) {
                video.WindowState = WindowState;
            }
        }
    }

    private void BtnMax_OnClick(object sender, RoutedEventArgs e) {
        if (WindowState != WindowState.Maximized) {
            WindowState = WindowState.Maximized;
            BtnMaxImg.Source =
                new BitmapImage(
                new Uri("pack://application:,,,/ResourceDictionary;Component/Images/AVMeeting/Normal.png"));
        } else {
            WindowState = WindowState.Normal;
            BtnMaxImg.Source =
                new BitmapImage(
                new Uri("pack://application:,,,/ResourceDictionary;Component/Images/AVMeeting/Maxmize.png"));
        }
    }

    private void WinButton_OnStateChanged(object sender, EventArgs e) {
        try {
            if (WindowState != WindowState.Maximized) {
                Topmost = true;
                for (int i = 0; i < WinVideos.Count; i++) {
                    var video = WinVideos[i];
                    if (video.ViewModel.IsMainWin) {
                        video.WindowState = WindowState;
                    } else {
                        SetLocation(i);
                    }
                    video.Topmost = true;
                }

            } else {
                Topmost = false;
                for (int i = 0; i < WinVideos.Count; i++) {
                    var video = WinVideos[i];
                    if (video.ViewModel.IsMainWin) {
                        video.WindowState = WindowState;
                    } else {
                        SetLocation(i);
                    }
                    video.Topmost = false;
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }

    /// <summary>
    /// 什么情况下需要发消息？  因为已经接通的情况下 关闭时不需要发消息的 jdk自己就可以直接对方挂断，只有对方申请 我拒绝了才需要发消息
    /// </summary>
    /// <param name="sendMessage"></param>
    /// <param name="LeaveChannel"></param>
    public void CloseMethod(bool sendMessage,bool LeaveChannel) {
        try {
            if (sendMessage) {
                AVMeetingService.getInstance()
                .SendAVMeetingCancelMessage(ObjViewModel.GuestNo, AVMeetingType.video.ToStr(), App.AccountsModel.clientuserId.ToInt(), ObjViewModel.ChannelName);
            }
            App.AmStatus = AVMeetingStatus.no;
            playSound.StopWaitSound();
            playSound.PlayCancelSound();
            FormFocus = true;
            timerLoading.Stop();
            timer.Stop();
            chargingTimer.Stop();
            if (LeaveChannel) {
                AgoraEngine.Instance.LeaveChannel();

            }

            foreach (var video in WinVideos) {
                video.Close();
            }
            AgoraEngine.Instance.VideoWindow = null;
            Close();
        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }
    private void BtnCancel_OnClick(object sender, RoutedEventArgs e) {
        FormFocus = true;
        if (App.AmStatus == AVMeetingStatus.contected) {
            CloseMethod(false, true);
        } else {
            CloseMethod(true, true);
        }

    }
    public void BtnClose_OnClick(object sender, RoutedEventArgs e) {
        FormFocus = true;
        if (App.AmStatus == AVMeetingStatus.contected) {
            CloseMethod(false, true);
        } else {
            CloseMethod(true, true);
        }
    }

    private void BtnTopmost_OnClick(object sender, RoutedEventArgs e) {
        Topmost = !Topmost;
        foreach (var video in WinVideos) {
            video.Topmost = !video.Topmost;
        }
        if (Topmost) {
            BtnTopmostImg.Source =
                new BitmapImage(
                new Uri("pack://application:,,,/ResourceDictionary;Component/Images/AVMeeting/PinX2.png"));
        } else {
            BtnTopmostImg.Source =
                new BitmapImage(
                new Uri("pack://application:,,,/ResourceDictionary;Component/Images/AVMeeting/Pin.png"));
        }
    }

    private void BtnMute_OnClick(object sender, RoutedEventArgs e) {
        if (BtnMute.IsChecked == false) {
            AgoraEngine.Instance.CancleMute();
        } else {
            AgoraEngine.Instance.Mute();

        }

    }

    private void BtnToVideo_OnClick(object sender, RoutedEventArgs e) {
        try {
            App.AmStatus = AVMeetingStatus.contected;
            playSound.StopWaitSound();
            FormFocus = true;
            foreach (var video in WinVideos) {
                video.Close();
            }


            AgoraEngine.Instance.AudioWindow = new WinAudio(ObjViewModel.ChannelName, true);
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.Duration = ObjViewModel.Duration + 1;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestAvatar = ObjViewModel.GuestAvatar;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.GuestName = ObjViewModel.GuestName;
            AgoraEngine.Instance.AudioWindow.ObjViewModelAudio.IsConnected = ObjViewModel.IsConnected;
            AgoraEngine.Instance.Video2Audio();
            //AgoraEngine.Instance.JoinAudioChannel(ObjViewModel.ChannelName, App.AccountsModel.clientuserId.ToInt(), App.winAudio);
            AgoraEngine.Instance.AudioWindow.Show();
            AgoraEngine.Instance.AudioWindow.timer.Stop();
            AVMeetingService.getInstance()
            .SendAVMeetingSwitchMessage(ObjViewModel.GuestNo, AVMeetingType.audio.ToStr(), App.AccountsModel.clientuserId.ToInt(), ObjViewModel.ChannelName, AVMeetingStatus.waiting.ToStr());

            AgoraEngine.Instance.VideoWindow = null;
            Close();
        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }



    private void TimerLoading_Elapsed(object sender, ElapsedEventArgs e) {
        if (this.ObjViewModel != null && this.ObjViewModel.IsConnected) {
            Dispatcher.BeginInvoke(new Action(() => {
                this.TxtblTooltip.Visibility = Visibility.Visible;
                this.GuestPanel.Visibility = Visibility.Collapsed;
            }));
            this.ObjViewModel.Duration++;
        } else {
            Dispatcher.BeginInvoke(new Action(() => {
                // 进度条
            }));
        }
    }

    public void JoinChannelSuccessCallback(string channelName, uint uid, int elapsed) {
        try {
            Console.WriteLine("JoinChannelSuccess channelName:{0} Uid:{1} Elapsed:{2}", channelName, uid, elapsed);
            this.ObjViewModel.Uid = uid;

            App.AmStatus = AVMeetingStatus.waiting;

            WinVideo localWinVideo = this.WinVideos.Find(q => {
                return q.ViewModel.IsLocal == true;
            });
            if (localWinVideo != null) {
                localWinVideo.ViewModel.Uid = uid;
            }
        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }
    private void chargingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        AVMeetingTimeChargingService.getInstance()
        .UpdateAVMeetingTimeCharging(chargingTable.chargingId, 1);

    }
    public void UserJoinedCallback(uint uid, int elapsed) {
        try {
            App.AmStatus = AVMeetingStatus.contected;
            chargingTable = AVMeetingTimeChargingService.getInstance()
                            .AddAVMeetingTimeCharging(AgoraEngine.AppKey, ObjViewModel.ChannelName, AVMeetingType.video,
                                    App.AmCallType, App.AccountsModel.no.ToStr(), ObjViewModel.GuestNo);



            timerLoading = new Timer();
            timerLoading.Interval = 1000;
            timerLoading.Elapsed += TimerLoading_Elapsed;
            timerLoading.Start();

            //设置timer
            chargingTimer = new Timer();
            chargingTimer.Interval = 60000;
            chargingTimer.Start();
            chargingTimer.Elapsed += new System.Timers.ElapsedEventHandler(chargingTimer_Elapsed);

            Console.WriteLine("UserJoined ChannelName:{0} Uid:{1} Elapsed:{2}", ObjViewModel.ChannelName, uid, elapsed);
            ObjViewModel.IsConnected = true;


            timer.Stop();
            playSound.StopWaitSound();
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.TxtblTooltip.Visibility = Visibility.Visible; // 显示时间
                this.GuestPanel.Visibility = Visibility.Collapsed; // 隐藏头像
                AgoraEngine.Instance.CancleMute();
                //WinVideo guestWinVideo = new WinVideo(); // 创建视频窗口
                //guestWinVideo.ViewModel.ButtonWin = this;
                //guestWinVideo.ViewModel.Uid = uid;
                //guestWinVideo.ShowInTaskbar = false;
                //guestWinVideo.Height = 120;
                //guestWinVideo.Width = 90;
                //guestWinVideo.Owner = this;

                //double leftPos = this.WinVideos[this.WinVideos.Count - 1].Left + 10;
                //double topPos = this.WinVideos[this.WinVideos.Count - 1].Top + 30;
                ////if ((leftPos + 90).CompareTo())
                //guestWinVideo.Left = leftPos;
                //guestWinVideo.Top = topPos;
                //this.WinVideos.Add(guestWinVideo);
                //guestWinVideo.Show();

                foreach (WinVideo guestWinVideo in  WinVideos) {
                    if(guestWinVideo.ViewModel.Uid==uid) {
                        // 显示对方视频
                        AgoraEngine.Instance.SetupRemoteVideo(uid, guestWinVideo);
                        break;
                    }
                }

            }));
        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }

    private void ChangeMeetingType(uint uid, AVMeetingType type) {
        foreach (SelectPeopleSubViewModel model in listUser) {
            if (model.MemberId == uid.ToStr()) {
                model.MeetingType = type ;
                break;
            }
        }
    }

    private AVMeetingType GetMeetingType(uint uid) {
        foreach (SelectPeopleSubViewModel model in listUser) {
            if (model.MemberId == uid.ToStr()) {
                return model.MeetingType;
            }
        }
        return AVMeetingType.no;
    }
    /// <summary>
    /// 对方关闭摄像头事件
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="close"></param>
    public void UserMuteVideoCallback(uint uid, bool close) {
        try {
            Thread.Sleep(500);
            //if (close == false) return;
            this.Dispatcher.BeginInvoke(new Action(() => {
                WinVideo WinVideo = this.WinVideos.Find(q => {
                    return q.ViewModel.Uid == uid;
                });
                VcardsTable guestModel = VcardService.getInstance().findByClientuserId(uid);
                if(close) {
                    ChangeMeetingType(uid, AVMeetingType.audio);
                    //关闭视频流
                    AgoraEngine.Instance.StopRemoteVideo(uid);
                    //取消绑定
                    AgoraEngine.Instance.SetupRemoteVideo(uid, null);

                    WinVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
                } else {
                    ChangeMeetingType(uid, AVMeetingType.video);
                    AgoraEngine.Instance.EnableRemoteVideo(uid);
                    // 显示对方视频
                    AgoraEngine.Instance.SetupRemoteVideo(uid, WinVideo);
                }

                WinVideo.Width = WinVideo.ActualWidth + 1;
                WinVideo.Width = WinVideo.ActualWidth -1;

            }));

        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }
    public void UserOfflineCallback(uint uid, UserOfflineReasonTypeClr rea) {
        try {

            this.Dispatcher.BeginInvoke(new Action(() => {
                timerLoading.Stop();
                timer.Stop();
                chargingTimer.Stop();

                Console.WriteLine(@"LeaveChannel!! ChannelName:{0} Uid:{1}", ObjViewModel.ChannelName, uid);

                playSound.StopWaitSound();
                playSound.PlayCancelSound();
                FormFocus = true;
                for(int i=0; i< WinVideos.Count; i++) {
                    if(WinVideos[i].ViewModel.Uid==uid) {
                        WinVideos[i].IsOnLine = false;
                        WinVideos[i].Visibility = Visibility.Collapsed;


                    } else {
                        WinVideos[i].IsOnLine = true;
                    }
                }

                bool flag = WinVideos.Where(q => q.IsOnLine.Equals(true)).Count() > 1;
                if(flag==false) {
                    App.AmStatus = AVMeetingStatus.no;
                    this.TxtblTooltip.Text = "对方已挂断";
                    AgoraEngine.Instance.LeaveChannel();
                    AgoraEngine.Instance.VideoWindow = null;
                    foreach (var video in WinVideos) {
                        video.Close();
                    }
                    this.Close();
                }

            }));
        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }

    public void WinVideo_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
        try {
            if (sender is WinVideo) {
                WinVideo senderWinVideo = sender as WinVideo;
                // 双击小画面
                if (!senderWinVideo.ViewModel.IsMainWin) {

                    WinVideo mainWinVideo = this.WinVideos.Find(q => {
                        return q.ViewModel.IsMainWin == true;
                    });

                    if (mainWinVideo.ViewModel.IsLocal) {
                        // 如果大画面显示的是本地视频
                        // 大画面切换为远程视频
                        if (GetMeetingType(senderWinVideo.ViewModel.Uid) != AVMeetingType.video) {
                            AgoraEngine.Instance.SetupRemoteVideo(senderWinVideo.ViewModel.Uid, null);
                        } else {
                            AgoraEngine.Instance.SetupRemoteVideo(senderWinVideo.ViewModel.Uid, mainWinVideo);
                        }

                        // 交换ViewModel
                        ViewModelVideo tempViewModelVideo = mainWinVideo.ViewModel;
                        mainWinVideo.ViewModel = senderWinVideo.ViewModel;
                        mainWinVideo.ViewModel.IsMainWin = true;
                        senderWinVideo.ViewModel = tempViewModelVideo;
                        senderWinVideo.ViewModel.IsMainWin = false;
                        // 刷新大画面
                        mainWinVideo.Width = mainWinVideo.ActualWidth + 1;
                        mainWinVideo.Width = mainWinVideo.ActualWidth - 1;

                        // 小画面切换为本地视频
                        if (GetMeetingType(senderWinVideo.ViewModel.Uid) != AVMeetingType.video) {
                            AgoraEngine.Instance.SetupLocalVideo(null);
                        } else {
                            AgoraEngine.Instance.SetupLocalVideo(senderWinVideo);
                        }
                        // 刷新小画面
                        senderWinVideo.Width = senderWinVideo.ActualWidth + 1;
                        senderWinVideo.Width = senderWinVideo.ActualWidth - 1;

                    } else if (senderWinVideo.ViewModel.IsLocal) {
                        // 如果小画面显示的是本地视频
                        // 小画面切换到远程视频
                        if (GetMeetingType(mainWinVideo.ViewModel.Uid) != AVMeetingType.video) {
                            AgoraEngine.Instance.SetupRemoteVideo(mainWinVideo.ViewModel.Uid, null);
                        } else {
                            AgoraEngine.Instance.SetupRemoteVideo(mainWinVideo.ViewModel.Uid, senderWinVideo);
                        }
                        // 交换ViewModel
                        ViewModelVideo tempModelVideo = mainWinVideo.ViewModel;
                        mainWinVideo.ViewModel = senderWinVideo.ViewModel;
                        mainWinVideo.ViewModel.IsMainWin = true;
                        senderWinVideo.ViewModel = tempModelVideo;
                        senderWinVideo.ViewModel.IsMainWin = false;

                        // 刷新小画面
                        senderWinVideo.Width = senderWinVideo.ActualWidth + 1;
                        senderWinVideo.Width = senderWinVideo.ActualWidth - 1;

                        // 大画面切换为本地视频
                        if (GetMeetingType(mainWinVideo.ViewModel.Uid) != AVMeetingType.video) {
                            AgoraEngine.Instance.SetupLocalVideo(null);
                        } else {
                            AgoraEngine.Instance.SetupLocalVideo(mainWinVideo);
                        }
                        // 刷新大画面
                        mainWinVideo.Width = mainWinVideo.ActualWidth + 1;
                        mainWinVideo.Width = mainWinVideo.ActualWidth - 1;
                    }
                }

            }

        } catch (Exception ex) {
            Log.Error(typeof(WinButton), ex);
        }
    }

    public void FirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed) {
        Console.WriteLine("FirstRemoteVideoDecoded# uid : {0} width:{1} height:{2} elapsed: {3}", uid, width, height, elapsed);
    }
    public void FirstRemoteVideoFrameHandler(uint uid, int width, int height, int elapsed) {
        Console.WriteLine("FirstRemoteVideoFrame# uid : {0} width:{1} height:{2} elapsed: {3}", uid, width, height, elapsed);
    }
    public void ErrorHandler(int err, string msg) {
        Console.WriteLine("ErrorHandler# err : {0} msg:{1}", err, msg);
    }



    private void ButtonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        //if (FormFocus == false) {
        //    e.Cancel = true;
        //    if (CommonMessageBox.Msg.Show("关闭窗口将终止视频聊天，确定关闭么 ?", CommonMessageBox.MsgBtn.YesNO) ==
        //            CommonMessageBox.MsgResult.Yes) {
        //        if (App.AmStatus == AVMeetingStatus.contected) {
        //            CloseMethod(false, true);
        //        } else {
        //            CloseMethod(true, true);
        //        }
        //    }

        //} else {
        //    AgoraEngine.Instance.VideoWindow = null;
        //}
    }

    private bool close = false;
    /// <summary>
    /// 关闭摄像头
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnCamera_Click(object sender, RoutedEventArgs e) {

        WinVideo mainWinVideo = this.WinVideos.Find(q => {
            return q.ViewModel.IsLocal == true;
        });
        VcardsTable guestModel = VcardService.getInstance().findByClientuserId(App.AccountsModel.clientuserId.ToInt());
        mainWinVideo.ViewModel.GuestAvatar = ImageHelper.loadAvatarPath(guestModel.avatarStorageRecordId);
        if (close==false) {
            AgoraEngine.Instance.CloaseLocalVideo();

            AgoraEngine.Instance.SetupLocalVideo(null);
            close = true;
        } else {
            AgoraEngine.Instance.OpenLocalVideo();

            AgoraEngine.Instance.SetupLocalVideo(mainWinVideo, true);
            close = false;
        }




        //AgoraEngine.Instance.StopRemoteVideo(uint.Parse( mainWinVideo.VideoWindow.Uid));

        // 刷新大画面
        mainWinVideo.Width = mainWinVideo.ActualWidth + 5;
        mainWinVideo.Width = mainWinVideo.ActualWidth - 5;

    }

    private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        Console.WriteLine("SliderVolume_ValueChanged : {0}", SliderVolume.Value);
        AgoraEngine.Instance.SetVolume((int)SliderVolume.Value);
    }

    private void BtnVolume_OnMouseEnter(object sender, MouseEventArgs e) {
        PopupVolume.IsOpen = false;
        PopupVolume.IsOpen = true;
        //PopupVolume.PlacementTarget = BtnVolume;
        //PopupVolume.StaysOpen = true;

        //SliderVolume.Visibility = Visibility.Visible;
        //PopupVolume.Visibility = Visibility.Visible;
    }

    private void BtnVolume_OnMouseLeave(object sender, MouseEventArgs e) {
        //PopupVolume.StaysOpen = false;
        //PopupVolume.IsOpen = false;
    }

    private void BtnVolume_OnClick(object sender, RoutedEventArgs e) {
        //PopupVolume.StaysOpen = true;
        //PopupVolume.IsOpen = true;
        //SliderVolume.Visibility = Visibility.Visible;
        //PopupVolume.Visibility = Visibility.Visible;
        // 音量按钮点，点击音量变0， 再点击恢复音量
        if (SliderVolume.Value > 0) {
            lastVolume = (int)SliderVolume.Value; // 保存音量
            SliderVolume.Value = 0;

        } else {
            SliderVolume.Value = lastVolume;
        }
    }

    private void SliderVolume_MouseLeave(object sender, MouseEventArgs e) {
        //PopupVolume.IsOpen = false;
    }
}
}