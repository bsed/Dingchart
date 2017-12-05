using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using cn.lds.chatcore.pcw.AgoraVideoClr;

namespace WpfDemo {
public class AgoraEngine {

    private static readonly AgoraEngine instance = new AgoraEngine();

    static AgoraEngine() {
    }

    private AgoraEngine() {
        if (objAgora == null) {
            objAgora = new Agora();
            int ret = objAgora.Initialize("5ff1c2425be74b47885ddcf65c9f56ab");
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
    public void JoinAudioChannel(string channelName, int uid, AgoraEngineEventInterface handler) {
        // 初始化语音组件
        Initialize("5ff1c2425be74b47885ddcf65c9f56ab");
        objAgora.OnJoinChannelSuccess = new JoinChannelSuccessHandler(handler.JoinChannelSuccessCallback); // 自己加入成功
        objAgora.OnUserJoined = new UserJoinedHandler(handler.UserJoinedCallback); // 对方用户加入
        objAgora.OnUserOffline = new UserOfflineHandler(handler.UserOfflineCallback); // 对方接通后的挂断
        objAgora.OnError = new ErrorHandler(handler.ErrorHandler);
        objAgora.OnFirstRemoteVideoFrame = new FirstRemoteVideoFrameHandler(handler.FirstRemoteVideoFrameHandler);
        objAgora.OnFirstRemoteVideoDecoded = new FirstRemoteVideoDecodedHandler(handler.FirstRemoteVideoDecodedHandler);
        objAgora.DisableLocalVideo(); // 关闭本地视频
        objAgora.DisableVideo(); // 关闭视频功能
        objAgora.JoinChannel("", channelName, "", (uint)uid);
    }

    /// <summary>
    /// 加入视频通话
    /// </summary>
    /// <param name="channelName">频道名称</param>
    /// <param name="handler">SDK事件的回调接口</param>
    public void JoinVideoChannel(string channelName, int uid, AgoraEngineEventInterface handler) {
        // 初始化视频组件
        Initialize("5ff1c2425be74b47885ddcf65c9f56ab");
        objAgora.OnJoinChannelSuccess = new JoinChannelSuccessHandler(handler.JoinChannelSuccessCallback);
        objAgora.OnUserJoined = new UserJoinedHandler(handler.UserJoinedCallback);
        objAgora.OnUserOffline = new UserOfflineHandler(handler.UserOfflineCallback);
        objAgora.OnError = new ErrorHandler(handler.ErrorHandler);
        objAgora.OnFirstRemoteVideoFrame = new FirstRemoteVideoFrameHandler(handler.FirstRemoteVideoFrameHandler);
        objAgora.OnFirstRemoteVideoDecoded = new FirstRemoteVideoDecodedHandler(handler.FirstRemoteVideoDecodedHandler);
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
        IntPtr hwnd = new WindowInteropHelper(localVideo).Handle;
        objAgora.SetupLocalVideo(RenderModeClr.RENDER_MODE_ADAPTIVE, hwnd);
        if (startPreview) objAgora.StartLocalPreview();
    }

    /// <summary>
    /// 显示对方视频
    /// </summary>
    /// <param name="uid">对方的uid</param>
    /// <param name="remoteVideo">视频窗体</param>
    public void SetupRemoteVideo(uint uid, System.Windows.Window remoteVideo) {
        if (!isInitialized) return;
        IntPtr hwnd = new WindowInteropHelper(remoteVideo).Handle;
        objAgora.SetupRemoteVideo(uid, RenderModeClr.RENDER_MODE_ADAPTIVE, hwnd);
    }

    /// <summary>
    /// 挂断
    /// </summary>
    public void LeaveChannel() {
        if (!isInitialized) return;
        objAgora.LeaveChannel();
    }

    /// <summary>
    /// 视频转音频
    /// </summary>
    /// <param name="handler">SDK事件的回调接口</param>
    public void Video2Audio(AgoraEngineEventInterface handler = null) {
        if (!isInitialized) return;
        objAgora.DisableLocalVideo();
        objAgora.DisableVideo();
        if (handler != null) {
            objAgora.OnJoinChannelSuccess = new JoinChannelSuccessHandler(handler.JoinChannelSuccessCallback);
            objAgora.OnUserJoined = new UserJoinedHandler(handler.UserJoinedCallback);
            objAgora.OnUserOffline = new UserOfflineHandler(handler.UserOfflineCallback);
            objAgora.OnError =  new ErrorHandler(handler.ErrorHandler);
            objAgora.OnFirstRemoteVideoFrame = new FirstRemoteVideoFrameHandler(handler.FirstRemoteVideoFrameHandler);
            objAgora.OnFirstRemoteVideoDecoded = new FirstRemoteVideoDecodedHandler(handler.FirstRemoteVideoDecodedHandler);
        }
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

}

}
