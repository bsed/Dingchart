// 这是主 DLL 文件。
// SDK https://docs.agora.io/cn/user_guide/API/windows_api.html
#include "stdafx.h"

#include "AgoraVideoClr.h"

using namespace cn::lds::chatcore::pcw::AgoraVideoClr;

Agora::Agora() {

	m_bVideoEnabled = false;
	m_bSetUpLocalVideo = false;

	m_lpAgoraEngine = NULL;
	m_pGcHandleList = gcnew List<GCHandle>();
	m_lpEventHandler = new AgoraEngineEventHandler();
	initializeEventHandler();

}

Agora::~Agora() {
	
	this->Release();

	for each (GCHandle handle in m_pGcHandleList)
	{
		handle.Free();
	}

	this->!Agora();
}

Agora::!Agora()
{

}

void Agora::Release() {

	if (m_lpAgoraEngine)
		m_lpAgoraEngine->release();
	m_lpAgoraEngine = NULL;
	if (m_lpEventHandler)
		delete m_lpEventHandler;
	m_lpEventHandler = NULL;

}

bool Agora::IsInitialized() {
	return m_lpAgoraEngine != NULL;
}
bool Agora::IsVideoEnabled() {
	return m_bVideoEnabled;
}
bool Agora::IsSetUpLocalVideo() {
	return m_bSetUpLocalVideo;
}
bool Agora::IsStartLocalPreview() {
	return m_bStartLocalPreview;
}

void* Agora::RegsiterEvent(Object^ obj)
{
	m_pGcHandleList->Add(GCHandle::Alloc(obj));
	return Marshal::GetFunctionPointerForDelegate(obj).ToPointer();
}

void Agora::initializeEventHandler() {
	if (m_lpEventHandler != NULL) {
		m_lpEventHandler->pOnJoinChannelSuccess = FunOnJoinChannelSuccess(RegsiterEvent(gcnew DelOnJoinChannelSuccess(this, &Agora::JoinChannelSuccessCallBack)));
		m_lpEventHandler->pOnRejoinChannelSuccess = FunOnRejoinChannelSuccess(RegsiterEvent(gcnew DelOnRejoinChannelSuccess(this, &Agora::RejoinChannelSuccessCallBack)));
		m_lpEventHandler->pOnWarning = FunOnWarning(RegsiterEvent(gcnew DelOnWarning(this, &Agora::WarningCallBack)));
		m_lpEventHandler->pOnError = FunOnError(RegsiterEvent(gcnew DelOnError(this, &Agora::ErrorCallBack)));
		m_lpEventHandler->pOnAudioQuality = FunOnAudioQuality(RegsiterEvent(gcnew DelOnAudioQuality(this, &Agora::AudioQualityCallBack)));
		m_lpEventHandler->pOnAudioVolumeIndication = FunOnAudioVolumeIndication(RegsiterEvent(gcnew DelOnAudioVolumeIndication(this, &Agora::AudioVolumeIndicationCallBack)));
		m_lpEventHandler->pOnLeaveChannel = FunOnLeaveChannel(RegsiterEvent(gcnew DelOnLeaveChannel(this, &Agora::LeaveChannelCallBack)));
		m_lpEventHandler->pOnRtcStats = FunOnRtcStats(RegsiterEvent(gcnew DelOnRtcStats(this, &Agora::RtcStatsCallBack)));
		m_lpEventHandler->pOnMediaEngineEvent = FunOnMediaEngineEvent(RegsiterEvent(gcnew DelOnMediaEngineEvent(this, &Agora::MediaEngineEventCallBack)));
		m_lpEventHandler->pOnAudioDeviceStateChanged = FunOnAudioDeviceStateChanged(RegsiterEvent(gcnew DelOnAudioDeviceStateChanged(this, &Agora::AudioDeviceStateChangedCallBack)));
		m_lpEventHandler->pOnVideoDeviceStateChanged = FunOnVideoDeviceStateChanged(RegsiterEvent(gcnew DelOnVideoDeviceStateChanged(this, &Agora::VideoDeviceStateChangedCallBack)));
		m_lpEventHandler->pOnLastmileQuality = FunOnLastmileQuality(RegsiterEvent(gcnew DelOnLastmileQuality(this, &Agora::LastmileQualityCallBack)));
		m_lpEventHandler->pOnFirstLocalVideoFrame = FunOnFirstLocalVideoFrame(RegsiterEvent(gcnew DelOnFirstLocalVideoFrame(this, &Agora::FirstLocalVideoFrameCallBack)));
		m_lpEventHandler->pOnFirstRemoteVideoDecoded = FunOnFirstRemoteVideoDecoded(RegsiterEvent(gcnew DelOnFirstRemoteVideoDecoded(this, &Agora::FirstRemoteVideoDecodedCallBack)));
		m_lpEventHandler->pOnFirstRemoteVideoFrame = FunOnFirstRemoteVideoFrame(RegsiterEvent(gcnew DelOnFirstRemoteVideoFrame(this, &Agora::FirstRemoteVideoFrameCallBack)));
		m_lpEventHandler->pOnUserJoined = FunOnUserJoined(RegsiterEvent(gcnew DelOnUserJoined(this, &Agora::UserJoinedCallBack)));
		m_lpEventHandler->pOnUserOffline = FunOnUserOffline(RegsiterEvent(gcnew DelOnUserOffline(this, &Agora::UserOfflineCallBack)));
		m_lpEventHandler->pOnUserMuteAudio = FunOnUserMuteAudio(RegsiterEvent(gcnew DelOnUserMuteAudio(this, &Agora::UserMuteAudioCallBack)));
		m_lpEventHandler->pOnUserMuteVideo = FunOnUserMuteVideo(RegsiterEvent(gcnew DelOnUserMuteVideo(this, &Agora::UserMuteVideoCallBack)));
		m_lpEventHandler->pOnApiCallExecuted = FunOnApiCallExecuted(RegsiterEvent(gcnew DelOnApiCallExecuted(this, &Agora::ApiCallExecutedCallBack)));
		m_lpEventHandler->pOnStreamMessage = FunOnStreamMessage(RegsiterEvent(gcnew DelOnStreamMessage(this, &Agora::StreamMessageCallBack)));
		m_lpEventHandler->pOnLocalVideoStats = FunOnLocalVideoStats(RegsiterEvent(gcnew DelOnLocalVideoStats(this, &Agora::LocalVideoStatsCallBack)));
		m_lpEventHandler->pOnRemoteVideoStats = FunOnRemoteVideoStats(RegsiterEvent(gcnew DelOnRemoteVideoStats(this, &Agora::RemoteVideoStatsCallBack)));
		m_lpEventHandler->pOnCameraReady = FunOnCameraReady(RegsiterEvent(gcnew DelOnCameraReady(this, &Agora::CameraReadyCallBack)));
		m_lpEventHandler->pOnVideoStopped = FunOnVideoStopped(RegsiterEvent(gcnew DelOnVideoStopped(this, &Agora::VideoStoppedCallBack)));
		m_lpEventHandler->pOnConnectionLost = FunOnConnectionLost(RegsiterEvent(gcnew DelOnConnectionLost(this, &Agora::ConnectionLostCallBack)));
		m_lpEventHandler->pOnConnectionInterrupted = FunOnConnectionInterrupted(RegsiterEvent(gcnew DelOnConnectionInterrupted(this, &Agora::ConnectionInterruptedCallBack)));
		m_lpEventHandler->pOnUserEnableVideo = FunOnUserEnableVideo(RegsiterEvent(gcnew DelOnUserEnableVideo(this, &Agora::UserEnableVideoCallBack)));
		m_lpEventHandler->pOnStartRecordingService = FunOnStartRecordingService(RegsiterEvent(gcnew DelOnStartRecordingService(this, &Agora::StartRecordingServiceCallBack)));
		m_lpEventHandler->pOnStopRecordingService = FunOnStopRecordingService(RegsiterEvent(gcnew DelOnStopRecordingService(this, &Agora::StopRecordingServiceCallBack)));
		m_lpEventHandler->pOnRefreshRecordingServiceStatus = FunOnRefreshRecordingServiceStatus(RegsiterEvent(gcnew DelOnRefreshRecordingServiceStatus(this, &Agora::RefreshRecordingServiceStatusCallBack)));
	}
}

int Agora::Initialize(String^ app_id) {

	if (m_lpAgoraEngine != NULL) return 0;
	m_lpAgoraEngine = createAgoraRtcEngine();
	if (m_lpAgoraEngine == NULL) return -1;
	if (app_id->Length <= 0) return -2;
	std::string stdVendorKey = marshal_as<std::string>(app_id);
	
	RtcEngineContext context;
	context.eventHandler = m_lpEventHandler;
	context.appId = stdVendorKey.c_str();
	m_lpAgoraEngine->initialize(context);

	return 1;
}

int Agora::DisableAudio() {
	if (m_lpAgoraEngine == NULL)
		return -1;
	return m_lpAgoraEngine->disableAudio();
}

int Agora::EnableAudio() {
	if (m_lpAgoraEngine == NULL)
		return -1;
	return m_lpAgoraEngine->enableAudio();
}

int Agora::DisableVideo() {
	if (m_lpAgoraEngine == NULL)
		return -1;

	int retValue = m_lpAgoraEngine->disableVideo();
	m_bVideoEnabled = (retValue == 0);

	return retValue;
}

int Agora::EnableVideo() {
	if (m_lpAgoraEngine == NULL)
		return -1;

	int retValue = m_lpAgoraEngine->enableVideo();
	m_bVideoEnabled = (retValue == 0);

	return retValue;
}

/*
加入频道之前，调用setEncryptionSecret指定secret来启用内置的加密功能
可以不指定，如果指定则加入聊天人员都必须使用相同的密码和加密方式才能进行
*/
int Agora::SetEncryptionSecret(String^ encKey, int encType) {
	if (m_lpAgoraEngine == NULL)
		return -1;

	switch (encType)
	{
	case 0:
		m_lpAgoraEngine->setEncryptionMode("aes-128-xts");
		break;
	case 1:
		m_lpAgoraEngine->setEncryptionMode("aes-256-xts");
		break;
	default:
		m_lpAgoraEngine->setEncryptionMode("aes-128-xts");
		break;
	}
	int nRet = m_lpAgoraEngine->setEncryptionSecret(marshal_as<std::string>(encKey).c_str());

	return nRet;
}

/*
同一频道内只能同时设置一种模式。
该方法必须在加入频道前调用和进行设置，进入频道后无法再设置。
CHANNEL_PROFILE_COMMUNICATION:0 通信 (默认)
CHANNEL_PROFILE _LIVE_BROADCASTING:1 直播
CHANNEL_PROFILE _GAME:2 游戏语音
*/
int Agora::SetChannelProfile(ChannelProfileClr profile) {
	if (m_lpAgoraEngine == NULL)
		return -1;
	return m_lpAgoraEngine->setChannelProfile((CHANNEL_PROFILE_TYPE)profile);
}

/*
channelKey ""
info ""
uid 0
*/
int Agora::JoinChannel(String^ channelKey, String^ channelName, String^ info, uid_t uid) {

	if (m_lpAgoraEngine == NULL) 
		return -91;
	if (channelName->Length <= 0)
		return -92;

	return m_lpAgoraEngine->joinChannel(NULL, 
		marshal_as<std::string>(channelName).c_str(), 
		NULL, uid);
}
/*
应在调用joinChannel/startPreview前设置视频属性。
*/
int Agora::SetLocalVideoProfile(VideoProfileTypeClr profile, bool isWHSwap) {
	if (m_lpAgoraEngine == NULL)
		return -1;
	return m_lpAgoraEngine->setVideoProfile((VIDEO_PROFILE_TYPE)profile, isWHSwap);
}
int Agora::SetupLocalVideo(RenderModeClr renderMode, IntPtr^ hWnd) {
	if (m_lpAgoraEngine == NULL)
		return -91;
	if (hWnd == IntPtr::Zero) return -92;

	VideoCanvas canvas(hWnd->ToPointer(), (RENDER_MODE_TYPE)renderMode, 0);
	int retValue = m_lpAgoraEngine->setupLocalVideo(canvas);
	m_bSetUpLocalVideo = (retValue == 0);

	return retValue;
}

int Agora::SetupRemoteVideo(unsigned int uid, RenderModeClr renderMode, IntPtr^ hWnd) {
	if (m_lpAgoraEngine == NULL)
		return -91;
	if (uid == 0) return -93;
	if (hWnd == IntPtr::Zero) return -92;

	VideoCanvas canvas(hWnd->ToPointer(), (RENDER_MODE_TYPE)renderMode, uid);

	return m_lpAgoraEngine->setupRemoteVideo(canvas);
}

int Agora::StartLocalPreview() {
	// 在开启预览前，必须先调用setupLocalVideo设置预览窗口及属性，且必须调用enableVideo开启视频功能
	// 在调用leaveChannel退出频道之后，需要调用stopPreview
	if (m_lpAgoraEngine == NULL)
		return -91;
	if (!m_bVideoEnabled)
		return -92;
	if (!m_bSetUpLocalVideo)
		return -93;

	int retValue = m_lpAgoraEngine->startPreview();
	m_bStartLocalPreview = (retValue == 0);

	return retValue;
}

int Agora::StopLocalPreview()
{
	if (m_lpAgoraEngine == NULL)
		return -91;
	if (!m_bStartLocalPreview)
		return -92;

	return m_lpAgoraEngine->stopPreview();
}

int Agora::LeaveChannel() {
	if (!m_lpAgoraEngine)
		return 0;

	return m_lpAgoraEngine->leaveChannel();
}

int Agora::DisableLocalAudio() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteLocalAudioStream(true);
}
int Agora::EnableLocalAudio() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteLocalAudioStream(false);
}
int Agora::DisableRemoteAudio(unsigned int uid) {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteRemoteAudioStream(uid, true);
}
int Agora::EnableRemoteAudio(unsigned int uid) {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteRemoteAudioStream(uid, false);
}
int Agora::DisableAllRemoteAudio() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteAllRemoteAudioStreams(true);
}
int Agora::EnableAllRemoteAudio() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteAllRemoteAudioStreams(false);
}

int Agora::DisableLocalVideo() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	
	return rep.enableLocalVideo(false); 
	 //return rep.muteLocalVideoStream(true);
}
int Agora::EnableLocalVideo() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.enableLocalVideo(true); 
	 //return rep.muteLocalVideoStream(false);
}
int Agora::DisableRemoteVideo(unsigned int uid) {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteRemoteVideoStream(uid, true);
}
int Agora::EnableRemoteVideo(unsigned int uid) {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteRemoteVideoStream(uid, false);
}
int Agora::DisableAllRemoteVideo() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteAllRemoteVideoStreams(true);
}
int Agora::EnableAllRemoteVideo() {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.muteAllRemoteVideoStreams(false);
}

int Agora::SetVolume(int volume) {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.setPlaybackDeviceVolume(volume);
}

int Agora::SetLogFilePath(String^ path) {
	if (!m_lpAgoraEngine)
		return -1;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.setLogFile(marshal_as<std::string>(path + "AgoraSDK.log").c_str());
}


int Agora::EnableNetworkTest() {
	if (!m_lpAgoraEngine)
		return -1;

	return m_lpAgoraEngine->enableLastmileTest();
}

int Agora::DisableNetworkTest() {
	if (!m_lpAgoraEngine)
		return -1;

	return m_lpAgoraEngine->disableLastmileTest();
}

int Agora::StartScreenCapture(IntPtr^ hWnd) {
	if (!m_lpAgoraEngine)
		return -1;
	if (hWnd == IntPtr::Zero) return -92;

	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.startScreenCapture((HWND)hWnd->ToPointer(), 15 , NULL);
}

//int Agora::SetScreenCaptureWindow(IntPtr^ hWnd) {
//	if (!m_lpAgoraEngine)
//		return -1;
//	if (hWnd == IntPtr::Zero) return -92;
//
//	RtcEngineParameters rep(*m_lpAgoraEngine);
//	return rep.setScreenCaptureWindow((HWND)hWnd->ToPointer());
//}

int Agora::StopScreenCapture() {
	if (!m_lpAgoraEngine)
		return -1;
	RtcEngineParameters rep(*m_lpAgoraEngine);
	return rep.stopScreenCapture();
}

/******************************************Event**********************************/
void Agora::JoinChannelSuccessCallBack(const char* channel, uid_t uid, int elapsed) {
	if (OnJoinChannelSuccess) 
		OnJoinChannelSuccess(marshal_as<String^>(channel), uid, elapsed);
}
void Agora::RejoinChannelSuccessCallBack(const char* channel, uid_t uid, int elapsed) {
	if (OnRejoinChannelSuccess) 
		OnRejoinChannelSuccess(marshal_as<String^>(channel), uid, elapsed);
}
void Agora::WarningCallBack(int warn, const char* msg) {
	if (OnWarning) 
		OnWarning(warn, marshal_as<String^>(msg));
}
void Agora::ErrorCallBack(int err, const char* msg) {
	if (OnError) 
		OnError(err, marshal_as<String^>(msg));
}
void Agora::AudioQualityCallBack(uid_t uid, int quality, unsigned short delay, unsigned short lost) {
	if (OnAudioQuality) 
		OnAudioQuality(uid, quality, delay, lost);
}
void Agora::AudioVolumeIndicationCallBack(const AudioVolumeInfo* speakers, unsigned int speakerNumber, int totalVolume) {
	if (OnAudioVolumeIndication) {
		List<AudioVolumeInfoClr^> ^listSpeakers = gcnew List<AudioVolumeInfoClr^>();
		for (unsigned int i = 0; i < speakerNumber; i++) {
			AudioVolumeInfoClr ^info = gcnew AudioVolumeInfoClr();
			info->uid = speakers[i].uid;
			info->volume = speakers[i].volume;
			listSpeakers->Add(info);
		}
		OnAudioVolumeIndication(listSpeakers, speakerNumber, totalVolume);
	}
}
void Agora::LeaveChannelCallBack(const RtcStats& stat) {
	if (OnLeaveChannel) {
		RtcStatsClr ^rtcStats = gcnew RtcStatsClr(stat);
		OnLeaveChannel(rtcStats);
	}
}
void Agora::RtcStatsCallBack(const RtcStats& stat) {
	if (OnRtcStats) {
		RtcStatsClr ^rtcStats = gcnew RtcStatsClr(stat);
		OnRtcStats(rtcStats);
	}
}
void Agora::MediaEngineEventCallBack(int evt) {
	if (OnMediaEngineEvent) 
		OnMediaEngineEvent(evt);
}
void Agora::AudioDeviceStateChangedCallBack(const char* deviceId, int deviceType, int deviceState) {
	if (OnAudioDeviceStateChanged) 
		OnAudioDeviceStateChanged(marshal_as<String^>(deviceId), deviceType, deviceState);
}
void Agora::VideoDeviceStateChangedCallBack(const char* deviceId, int deviceType, int deviceState) {
	if (OnVideoDeviceStateChanged) 
		OnVideoDeviceStateChanged(marshal_as<String^>(deviceId), deviceType, deviceState);
}
void Agora::LastmileQualityCallBack(int quality) {
	if (OnLastmileQuality) OnLastmileQuality(quality);
}
void Agora::FirstLocalVideoFrameCallBack(int width, int height, int elapsed) {
	if (OnFirstLocalVideoFrame) 
		OnFirstLocalVideoFrame(width, height, elapsed);
}
void Agora::FirstRemoteVideoDecodedCallBack(uid_t uid, int width, int height, int elapsed) {
	if (OnFirstRemoteVideoDecoded) 
		OnFirstRemoteVideoDecoded(uid, width, height, elapsed);
}
void Agora::FirstRemoteVideoFrameCallBack(uid_t uid, int width, int height, int elapsed) {
	if (OnFirstRemoteVideoFrame) 
		OnFirstRemoteVideoFrame(uid, width, height, elapsed);
}
void Agora::UserJoinedCallBack(uid_t uid, int elapsed) {
	if (OnUserJoined) 
		OnUserJoined(uid, elapsed);
}
void Agora::UserOfflineCallBack(uid_t uid, USER_OFFLINE_REASON_TYPE reason) {
	if (OnUserOffline) {
		//UserOfflineReasonTypeClr ^reasonType = gcnew UserOfflineReasonTypeClr(reason);
		OnUserOffline(uid, (UserOfflineReasonTypeClr)reason);
	}

}
void Agora::UserMuteAudioCallBack(uid_t uid, bool muted) {
	if (OnUserMuteAudio) 
		OnUserMuteAudio(uid, muted);
}
void Agora::UserMuteVideoCallBack(uid_t uid, bool muted) {
	if (OnUserMuteVideo) 
		OnUserMuteVideo(uid, muted);
}
void Agora::ApiCallExecutedCallBack(const char* api, int error) {
	if (OnApiCallExecuted) 
		OnApiCallExecuted(marshal_as<String^>(api), error);
}
void Agora::StreamMessageCallBack(uid_t uid, int streamId, const char* data, size_t length) {
	if (OnStreamMessage) 
		OnStreamMessage(uid, streamId, marshal_as<String^>(data), length);
}
void Agora::LocalVideoStatsCallBack(const LocalVideoStats& stats) {
	if (OnLocalVideoStats) 
		OnLocalVideoStats(stats);
}
void Agora::RemoteVideoStatsCallBack(const RemoteVideoStats& stats) {
	if (OnRemoteVideoStats) 
		OnRemoteVideoStats(stats);
}
void Agora::CameraReadyCallBack() {
	if (OnCameraReady) OnCameraReady();
}
void Agora::VideoStoppedCallBack() {
	if (OnVideoStopped) 
		OnVideoStopped();
}
void Agora::ConnectionLostCallBack() {
	if (OnConnectionLost) 
		OnConnectionLost();
}
void Agora::ConnectionInterruptedCallBack() {
	if (OnConnectionInterrupted) 
		OnConnectionInterrupted();
}
void Agora::UserEnableVideoCallBack(uid_t uid, bool enabled) {
	if (OnUserEnableVideo) 
		OnUserEnableVideo(uid, enabled);
}
void Agora::StartRecordingServiceCallBack(int error) {
	if (OnStartRecordingService) 
		OnStartRecordingService(error);
}
void Agora::StopRecordingServiceCallBack(int error) {
	if (OnStopRecordingService) 
		OnStopRecordingService(error);
}
void Agora::RefreshRecordingServiceStatusCallBack(int status) {
	if (OnRefreshRecordingServiceStatus) 
		OnRefreshRecordingServiceStatus(status);
}
