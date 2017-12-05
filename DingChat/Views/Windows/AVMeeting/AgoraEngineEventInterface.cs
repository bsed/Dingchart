using cn.lds.chatcore.pcw.AgoraVideoClr;


namespace cn.lds.chatcore.pcw.Views.Windows.AVMeeting {
public interface AgoraEngineEventInterface {

    void JoinChannelSuccessCallback(string channelName, uint uid, int elapsed);

    void UserJoinedCallback(uint uid, int elapsed);

    void UserOfflineCallback(uint uid, UserOfflineReasonTypeClr rea);

    void FirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed);
    void FirstRemoteVideoFrameHandler(uint uid, int width, int height, int elapsed);

    void ErrorHandler(int err, string msg);
}

}
