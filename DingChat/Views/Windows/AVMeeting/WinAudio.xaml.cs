using System;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.AgoraVideoClr;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Emoji;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using System.Runtime.InteropServices;
using cn.lds.chatcore.pcw.Views.Control;

//禁用关闭按钮需要引用这个

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
/// <summary>
///     WinAudio.xaml 的交互逻辑
/// </summary>
public partial class WinAudio : Window, AgoraEngineEventInterface {
    private Timer timerLoading = new Timer();
    private int lastVolume = 50;
    public PlaySoundBase playSound;
    public ViewModelAudio ObjViewModelAudio {
        get;
        set;
    }

    bool FormFocus = false;
    private AVMeetingTimeChargingTable chargingTable = null;

    public System.Timers.Timer timer = new System.Timers.Timer();

    private System.Timers.Timer chargingTimer = new System.Timers.Timer();

    public WinAudio(string channelName, bool isVideoToAudio = false) {
        InitializeComponent();


        ObjViewModelAudio = new ViewModelAudio();
        ObjViewModelAudio.ChannelName = channelName;
        playSound = new PlaySoundBase(AVMeetingType.audio);

        timer = new Timer();
        timer.Enabled = true;
        //设置timer
        timer.Interval = Constants.AmTimeOut+500;

        //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
        timer.AutoReset = true;
        timer.Start();
        timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
    }
    //超时没有答复发送消息
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {


        timer.Stop();
        if (App.AmStatus == AVMeetingStatus.contected) return;
        AVMeetingService.getInstance()
        .SendAVMeetingTimeOutMessage(ObjViewModelAudio.GuestNo, AVMeetingType.audio.ToStr(), App.AccountsModel.clientuserId.ToInt(), ObjViewModelAudio.ChannelName, false);
        this.Dispatcher.BeginInvoke((Action)delegate () {
            CloseMethod(false);
        });
    }


    private void WinAudio_OnLoaded(object sender, RoutedEventArgs e) {



        var hwnd = new WindowInteropHelper(this).Handle;
        NativeWindowHelper.SetWindowNoBorder(hwnd);



        if (!ObjViewModelAudio.IsConnected) {
            this.BtnMute.Visibility = Visibility.Collapsed;
            this.BtnVolume.Visibility = Visibility.Collapsed;
            this.TxtblTooltip.Visibility = Visibility.Collapsed;
            playSound.PlayWaitSound();
        } else {
            this.CanvasLoading.Visibility = Visibility.Collapsed;
        }

        timerLoading.Interval = 1000;
        timerLoading.Elapsed += TimerLoading_Elapsed;
        timerLoading.Start();

        if (ObjViewModelAudio != null)
            DataContext = ObjViewModelAudio;
    }

    private void TitlePanel_OnMouseMove(object sender, MouseEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            DragMove();
        }
    }

    private void BtnMin_OnClick(object sender, RoutedEventArgs e) {
        WindowState = WindowState.Minimized;
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
    public void CloseMethod(bool sendMessage) {

        if (sendMessage) {
            AVMeetingService.getInstance()
            .SendAVMeetingCancelMessage(ObjViewModelAudio.GuestNo, AVMeetingType.audio.ToStr(), App.AccountsModel.clientuserId.ToInt(), ObjViewModelAudio.ChannelName);
        }
        App.AmStatus = AVMeetingStatus.no;
        timer.Stop();
        chargingTimer.Stop();
        playSound.StopWaitSound();
        playSound.PlayCancelSound();
        timerLoading.Stop();
        AgoraEngine.Instance.LeaveChannel();
        AgoraEngine.Instance.AudioWindow = null;
        FormFocus = true;
        this.Close();
    }
    private void BtnClose_OnClick(object sender, RoutedEventArgs e) {
        FormFocus = true;
        if (App.AmStatus == AVMeetingStatus.contected) {
            CloseMethod(false);
        } else {
            CloseMethod(true);
        }

    }
    public void BtnCancel_OnClick(object sender, RoutedEventArgs e) {
        FormFocus = true;
        if (App.AmStatus == AVMeetingStatus.contected) {
            CloseMethod(false);
        } else {
            CloseMethod(true);
        }
    }
    private void BtnVolume_OnClick(object sender, RoutedEventArgs e) {

        // 音量按钮点，点击音量变0， 再点击恢复音量
        if (SliderVolume.Value > 0) {
            lastVolume = (int)SliderVolume.Value; // 保存音量
            SliderVolume.Value = 0;
        } else {
            SliderVolume.Value = lastVolume;
        }
    }

    private void TimerLoading_Elapsed(object sender, ElapsedEventArgs e) {
        if (ObjViewModelAudio != null && ObjViewModelAudio.IsConnected) {
            ObjViewModelAudio.Duration++;
        } else {
            Dispatcher.BeginInvoke(new Action(() => {
                foreach (var child in CanvasLoading.Children) {
                    if (child is Ellipse) {
                        if ((child as Ellipse).Opacity.CompareTo(1) < 0) {
                            (child as Ellipse).Opacity = 1;
                        } else {
                            (child as Ellipse).Opacity = 0.6;
                        }
                    }
                }
            }));
        }
    }



    private void TxtblUserName_OnLoaded(object sender, RoutedEventArgs e) {
        if (sender != null && sender is TextBlock)
            EmojiHelper.ReplaceEmojiToIco((sender as TextBlock).Text, sender as TextBlock);
    }
    public void JoinChannelSuccessCallback(string channelName, uint uid, int elapsed) {

        App.AmStatus = AVMeetingStatus.waiting;
        Console.WriteLine("JoinChannelSuccess channelName:{0} Uid:{1} Elapsed:{2}", channelName, uid, elapsed);
    }

    private void chargingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        AVMeetingTimeChargingService.getInstance()
        .UpdateAVMeetingTimeCharging(chargingTable.chargingId, 1);

    }
    public void UserJoinedCallback(uint uid, int elapsed) {
        chargingTable = AVMeetingTimeChargingService.getInstance()
                        .AddAVMeetingTimeCharging(AgoraEngine.AppKey, ObjViewModelAudio.ChannelName, AVMeetingType.audio,
                                App.AmCallType, App.AccountsModel.no.ToStr(), ObjViewModelAudio.GuestNo);


        App.AmStatus = AVMeetingStatus.contected;

        //设置timer
        chargingTimer = new Timer();
        chargingTimer.Interval = 60000;
        chargingTimer.Start();
        chargingTimer.Elapsed += new System.Timers.ElapsedEventHandler(chargingTimer_Elapsed);

        timer.Stop();
        playSound.StopWaitSound();
        App.AmStatus = AVMeetingStatus.contected;
        Console.WriteLine("UserJoined ChannelName:{0} Uid:{1} Elapsed:{2}", ObjViewModelAudio.ChannelName, uid, elapsed);
        ObjViewModelAudio.IsConnected = true;
        this.Dispatcher.BeginInvoke(new Action(() => {
            AgoraEngine.Instance.CancleMute();
            this.TxtblTooltip.Visibility = Visibility.Visible;
            this.CanvasLoading.Visibility = Visibility.Collapsed;
            this.BtnMute.Visibility = Visibility.Visible;
            this.BtnVolume.Visibility = Visibility.Visible;
        }));
    }

    public void UserOfflineCallback(uint uid, UserOfflineReasonTypeClr rea) {
        App.AmStatus = AVMeetingStatus.no;
        this.Dispatcher.BeginInvoke(new Action(() => {
            timerLoading.Stop();
            timer.Stop();
            chargingTimer.Stop();
            AgoraEngine.Instance.LeaveChannel();
            this.TxtblTooltip.Text = "对方已挂断";
            playSound.StopWaitSound();
            FormFocus = true;
            this.Close();
        }));
    }

    private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        Console.WriteLine("SliderVolume_ValueChanged : {0}", SliderVolume.Value);
        AgoraEngine.Instance.SetVolume((int) SliderVolume.Value);
    }

    private void BtnMute_OnClick(object sender, RoutedEventArgs e) {
        if(BtnMute.IsChecked==false) {
            AgoraEngine.Instance.CancleMute();
        } else {
            AgoraEngine.Instance.Mute();

        }

    }
    private void BtnVolume_OnMouseEnter(object sender, MouseEventArgs e) {
        PopupVolume.IsOpen = false;
        PopupVolume.IsOpen = true;
    }

    private void BtnVolume_OnMouseLeave(object sender, MouseEventArgs e) {
        //PopupVolume.StaysOpen = false;
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

    private void AudioWindow_Unloaded(object sender, RoutedEventArgs e) {

        CloseMethod(false);

    }

    private void AudioWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        if(FormFocus==false) {
            e.Cancel = true;
            if (CommonMessageBox.Msg.Show("关闭窗口将终止语音聊天，确定关闭么 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                if (App.AmStatus == AVMeetingStatus.contected) {
                    CloseMethod(false);
                } else {
                    CloseMethod(true);
                }
            }

        } else {
            AgoraEngine.Instance.AudioWindow = null;
        }
    }
}
}