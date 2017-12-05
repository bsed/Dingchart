using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using cn.lds.chatcore.pcw.AgoraVideoClr;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
public class AgoraEngine:AgoraEngineEventInterface {
    //绿信     a15141befe1b400ba4c3ccd59607401c
    //鼎信    6d1f99b1a1ee45a58135c098de39a44d
    //教育部   a75eb67731c94180bcb729bae85f1798
    public static string AppKey = ProgramSettingHelper.VideoKey;
    private static readonly AgoraEngine instance = new AgoraEngine();

    static AgoraEngine() {
    }

    private AgoraEngine() {
        if (objAgora == null) {
            objAgora = new Agora();
            int ret = objAgora.Initialize(AppKey);
            if (ret != 1) {
                isInitialized = false;
                Console.WriteLine(@"Init Agora Falied!! Code: " + ret);
            } else {
                isInitialized = true;
                objAgora.SetLogFilePath(Environment.CurrentDirectory + Path.DirectorySeparatorChar);
                objAgora.EnableVideo();
                objAgora.EnableNetworkTest();
                Console.WriteLine(@"Init Agora Success!! ");
            }
        }
    }

    /// <summary>
    /// 取得实例对象
    /// </summary>
    public static AgoraEngine Instance {
        get {
            return instance;
        }
    }

    private Agora objAgora;

    /// <summary>
    /// 是否已经初始化了SDK组件
    /// </summary>
    private bool isInitialized = false;
    public bool IsInitialized {
        get {
            return isInitialized;
        }
    }
    public WinButton VideoWindow {
        get;
        set;
    }
    public WinAudio AudioWindow {
        get;
        set;
    }
    public WinReceiving ReceivingWindow {
        get;
        set;
    }


    /// <summary>
    /// 使用APP_KEY初始化组件
    /// </summary>
    /// <param name="appKey"></param>
    /// <returns></returns>
    public bool Initialize(string appKey) {
        if (!isInitialized) {
            int ret = objAgora.Initialize(appKey);
            if (ret != 1) {
                isInitialized = false;
                Console.WriteLine(@"Initialize Agora Falied!! Code: " + ret);
            } else {
                isInitialized = true;
                Console.WriteLine(@"Initialize Agora Success!! ");
            }
        }

        return isInitialized;
    }

    /// <summary>
    /// 加入语音通话
    /// </summary>
    /// <param name="channelName">频道名称</param>
    /// <param name="handler">SDK事件的回调接口</param>
    public void JoinAudioChannel(string channelName, int uid) {
        // 初始化语音组件
        Initialize(AppKey);
        objAgora.OnJoinChannelSuccess = new JoinChannelSuccessHandler(JoinChannelSuccessCallback); // 自己加入成功
        objAgora.OnUserJoined = new UserJoinedHandler(UserJoinedCallback); // 对方用户加入
        objAgora.OnUserOffline = new UserOfflineHandler(UserOfflineCallback); // 对方接通后的挂断
        //objAgora.OnUserMuteVideo = new UserMuteVideoHandler(UserMuteVideoCallback);
        objAgora.OnError = new ErrorHandler(ErrorHandler);
        objAgora.OnFirstRemoteVideoFrame = new FirstRemoteVideoFrameHandler(FirstRemoteVideoFrameHandler);
        objAgora.OnFirstRemoteVideoDecoded =new FirstRemoteVideoDecodedHandler(FirstRemoteVideoDecodedHandler);
        objAgora.DisableLocalVideo(); // 关闭本地视频
        objAgora.DisableVideo(); // 关闭视频功能
        objAgora.JoinChannel("", channelName, "", (uint)uid);
    }

    /// <summary>
    /// 关闭本地视频
    /// </summary>
    public void CloaseLocalVideo() {
        if (!isInitialized) return;
        //停止预览 关自己摄像头
        objAgora.StopLocalPreview();
        //关闭本地视频流
        objAgora.DisableLocalVideo();


        objAgora.StopScreenCapture();
    }

    public void OpenLocalVideo() {
        if (!isInitialized) return;
        objAgora.EnableVideo();
        //关闭本地视频流
        objAgora.EnableLocalVideo();

        //停止预览 关自己摄像头
        //objAgora.StartLocalPreview();
    }

    /// <summary>
    /// 加入视频通话
    /// </summary>
    /// <param name="channelName">频道名称</param>
    /// <param name="handler">SDK事件的回调接口</param>
    public void JoinVideoChannel(string channelName, int uid) {
        // 初始化视频组件
        Initialize(AppKey);
        objAgora.OnJoinChannelSuccess = new JoinChannelSuccessHandler(JoinChannelSuccessCallback);
        objAgora.OnUserJoined = new UserJoinedHandler(UserJoinedCallback);
        objAgora.OnUserOffline = new UserOfflineHandler(UserOfflineCallback);
        objAgora.OnUserMuteVideo = new UserMuteVideoHandler(UserMuteVideoCallback);
        objAgora.OnError = new ErrorHandler(ErrorHandler);
        objAgora.OnFirstRemoteVideoFrame = new FirstRemoteVideoFrameHandler(FirstRemoteVideoFrameHandler);
        objAgora.OnFirstRemoteVideoDecoded =new FirstRemoteVideoDecodedHandler(FirstRemoteVideoDecodedHandler);
        int x = objAgora.SetLocalVideoProfile(VideoProfileTypeClr.VIDEO_PROFILE_480P, false); // 设置分辨率
        x = objAgora.EnableVideo(); // 开始视频模式
        x = objAgora.JoinChannel("", channelName, "", (uint)uid); // 加入频道
    }

    /// <summary>
    /// 显示本地视频
    /// </summary>
    /// <param name="localVideo">视频窗体</param>
    /// <param name="startPreview">是否开启预览</param>
    public void SetupLocalVideo(System.Windows.Window localVideo, bool startPreview = false) {
        if (!isInitialized) return;
        IntPtr hwnd = IntPtr.Zero;
        if (localVideo != null) {
            hwnd = new WindowInteropHelper(localVideo).Handle;
            objAgora.EnableVideo();
            objAgora.EnableLocalVideo();
        }
        objAgora.SetupLocalVideo(RenderModeClr.RENDER_MODE_ADAPTIVE, hwnd);
        if (startPreview) objAgora.StartLocalPreview();
    }




    public void EnableRemoteVideo(uint uid) {
        if (!isInitialized) return;

        objAgora.EnableRemoteVideo(uid);
    }

    /// <summary>
    /// 显示对方视频
    /// </summary>
    /// <param name="uid">对方的uid</param>
    /// <param name="remoteVideo">视频窗体</param>
    public void SetupRemoteVideo(uint uid, System.Windows.Window remoteVideo) {
        if (!isInitialized) return;
        IntPtr hwnd = IntPtr.Zero;
        if (remoteVideo != null) {
            hwnd = new WindowInteropHelper(remoteVideo).Handle;
            //objAgora.EnableVideo();
        }
        objAgora.SetupRemoteVideo(uid, RenderModeClr.RENDER_MODE_ADAPTIVE, hwnd);
    }

    /// <summary>
    /// 关闭视频流
    /// </summary>
    /// <param name="uid"></param>
    public void StopRemoteVideo(uint uid) {
        if (!isInitialized) return;
        //IntPtr hwnd = new WindowInteropHelper(remoteVideo).Handle;
        objAgora.DisableRemoteVideo(uid);
    }

    /// <summary>
    /// 挂断
    /// </summary>
    public void LeaveChannel() {
        if (!isInitialized) return;
        objAgora.LeaveChannel();
        objAgora.StopLocalPreview();
        //关闭本地视频流
        objAgora.DisableLocalVideo();
        objAgora.DisableLocalAudio();
        objAgora.DisableVideo();
        objAgora.StopScreenCapture();
    }

    /// <summary>
    /// 视频转音频
    /// </summary>
    /// <param name="handler">SDK事件的回调接口</param>
    public void Video2Audio() {
        if (!isInitialized) return;
        objAgora.StopLocalPreview();
        objAgora.DisableLocalVideo();
        objAgora.DisableVideo();
    }

    /// <summary>
    /// 调节音量
    /// </summary>
    /// <param name="volume">音量值 0～255</param>
    public void SetVolume(int volume) {
        objAgora.SetVolume(volume);
    }

    /// <summary>
    /// 静音
    /// </summary>
    public void Mute() {
        objAgora.DisableLocalAudio();

    }

    public void CancleMute() {
        objAgora.EnableLocalAudio();
    }
    public void JoinChannelSuccessCallback(string channelName, uint uid, int elapsed) {
        if (VideoWindow != null) {
            VideoWindow.JoinChannelSuccessCallback(channelName, uid, elapsed);
        } else if (AudioWindow != null) {
            AudioWindow.JoinChannelSuccessCallback(channelName, uid, elapsed);
        }
    }

    public void UserJoinedCallback(uint uid, int elapsed) {
        if (VideoWindow != null) {
            VideoWindow.UserJoinedCallback(uid, elapsed);
        } else if (AudioWindow != null) {
            AudioWindow.UserJoinedCallback(uid, elapsed);
        }
    }

    public void UserMuteVideoCallback(uint uid, bool open) {
        if (VideoWindow != null) {
            VideoWindow.UserMuteVideoCallback(uid, open);
        }

    }
    public void UserOfflineCallback(uint uid, UserOfflineReasonTypeClr rea) {
        if (VideoWindow != null) {
            VideoWindow.UserOfflineCallback(uid, rea);
        } else if (AudioWindow != null) {
            AudioWindow.UserOfflineCallback(uid, rea);
        }
    }

    public void FirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed) {
        if (VideoWindow != null) {
            VideoWindow.FirstRemoteVideoDecodedHandler(uid, width, height, elapsed);
        } else if (AudioWindow != null) {
            AudioWindow.FirstRemoteVideoDecodedHandler(uid, width, height, elapsed);
        }
    }

    public void FirstRemoteVideoFrameHandler(uint uid, int width, int height, int elapsed) {
        if (VideoWindow != null) {
            VideoWindow.FirstRemoteVideoFrameHandler(uid, width, height, elapsed);
        } else if (AudioWindow != null) {
            AudioWindow.FirstRemoteVideoFrameHandler(uid, width, height, elapsed);
        }
    }

    public void ErrorHandler(int err, string msg) {
        if (VideoWindow != null) {
            VideoWindow.ErrorHandler(err, msg);
        } else if (AudioWindow != null) {
            AudioWindow.ErrorHandler(err, msg);
        }
    }
}

}
