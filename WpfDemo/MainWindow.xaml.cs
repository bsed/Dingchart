using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using cn.lds.chatcore.pcw.AgoraVideoClr;

namespace WpfDemo {
/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window, AgoraEngineEventInterface {
    public MainWindow() {
        InitializeComponent();


    }

    public void ErrorHandler(int err, string msg) {
        Console.WriteLine("ErrorHandler err:{0}  msg:{1}", err, msg);
    }

    public void FirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed) {
        Console.WriteLine("FirstRemoteVideoDecodedHandler {0}", uid);
    }

    public void FirstRemoteVideoFrameHandler(uint uid, int width, int height, int elapsed) {
        Console.WriteLine("FirstRemoteVideoFrameHandler {0}", uid);
    }

    public void JoinChannelSuccessCallback(string channelName, uint uid, int elapsed) {
        Console.WriteLine("JoinChannelSuccessCallback {0}", uid);
    }

    public void UserJoinedCallback(uint uid, int elapsed) {
        this.Dispatcher.BeginInvoke(new Action(() => {
            WinRemote guestWinVideo = new WinRemote();
            guestWinVideo.Show();
            // 显示对方视频
            AgoraEngine.Instance.SetupRemoteVideo(uid, guestWinVideo);
        }));
    }

    public void UserOfflineCallback(uint uid, UserOfflineReasonTypeClr rea) {

    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {

        // 显示本地视频
        AgoraEngine.Instance.SetupLocalVideo(this, true);
        // 加入频道
        AgoraEngine.Instance.JoinVideoChannel("zwt123456", 111, this);
    }
}
}
