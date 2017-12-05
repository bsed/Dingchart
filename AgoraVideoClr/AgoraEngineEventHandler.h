#pragma once

#include "sdk/include/IAgoraRtcEngine.h"

using namespace agora::rtc;

namespace cn {
	namespace lds {
		namespace chatcore {
			namespace pcw {
namespace AgoraVideoClr {

	typedef void(__stdcall *FunOnJoinChannelSuccess)(const char* channel, uid_t uid, int elapsed);
	typedef void(__stdcall *FunOnRejoinChannelSuccess)(const char* channel, uid_t uid, int elapsed);
	typedef void(__stdcall *FunOnWarning)(int warn, const char* msg);
	typedef void(__stdcall *FunOnError)(int err, const char* msg);
	typedef void(__stdcall *FunOnAudioQuality)(uid_t uid, int quality, unsigned short delay, unsigned short lost);
	typedef void(__stdcall *FunOnAudioVolumeIndication)(const AudioVolumeInfo* speakers, unsigned int speakerNumber, int totalVolume);
	typedef void(__stdcall *FunOnLeaveChannel)(const RtcStats& stat);
	typedef void(__stdcall *FunOnRtcStats)(const RtcStats& stat);
	typedef void(__stdcall *FunOnMediaEngineEvent)(int evt);
	typedef void(__stdcall *FunOnAudioDeviceStateChanged)(const char* deviceId, int deviceType, int deviceState);
	typedef void(__stdcall *FunOnVideoDeviceStateChanged)(const char* deviceId, int deviceType, int deviceState);
	typedef void(__stdcall *FunOnLastmileQuality)(int quality);
	typedef void(__stdcall *FunOnFirstLocalVideoFrame)(int width, int height, int elapsed);
	typedef void(__stdcall *FunOnFirstRemoteVideoDecoded)(uid_t uid, int width, int height, int elapsed);
	typedef void(__stdcall *FunOnFirstRemoteVideoFrame)(uid_t uid, int width, int height, int elapsed);
	typedef void(__stdcall *FunOnUserJoined)(uid_t uid, int elapsed);
	typedef void(__stdcall *FunOnUserOffline)(uid_t uid, USER_OFFLINE_REASON_TYPE reason);
	typedef void(__stdcall *FunOnUserMuteAudio)(uid_t uid, bool muted);
	typedef void(__stdcall *FunOnUserMuteVideo)(uid_t uid, bool muted);
	typedef void(__stdcall *FunOnApiCallExecuted)(const char* api, int error);
	typedef void(__stdcall *FunOnStreamMessage)(uid_t uid, int streamId, const char* data, size_t length);
	typedef void(__stdcall *FunOnLocalVideoStats)(const LocalVideoStats& stats);
	typedef void(__stdcall *FunOnRemoteVideoStats)(const RemoteVideoStats& stats);
	typedef void(__stdcall *FunOnCameraReady)();
	typedef void(__stdcall *FunOnVideoStopped)();
	typedef void(__stdcall *FunOnConnectionLost)();
	typedef void(__stdcall *FunOnConnectionInterrupted)();
	typedef void(__stdcall *FunOnUserEnableVideo)(uid_t uid, bool enabled);
	typedef void(__stdcall *FunOnStartRecordingService)(int error);
	typedef void(__stdcall *FunOnStopRecordingService)(int error);
	typedef void(__stdcall *FunOnRefreshRecordingServiceStatus)(int status);

	public delegate void DelOnJoinChannelSuccess(const char* channel, uid_t uid, int elapsed);
	public delegate void DelOnRejoinChannelSuccess(const char* channel, uid_t uid, int elapsed);
	public delegate void DelOnWarning(int warn, const char* msg);
	public delegate void DelOnError(int err, const char* msg);
	public delegate void DelOnAudioQuality(uid_t uid, int quality, unsigned short delay, unsigned short lost);
	public delegate void DelOnAudioVolumeIndication(const AudioVolumeInfo* speakers, unsigned int speakerNumber, int totalVolume);
	public delegate void DelOnLeaveChannel(const RtcStats& stat);
	public delegate void DelOnRtcStats(const RtcStats& stat);
	public delegate void DelOnMediaEngineEvent(int evt);
	public delegate void DelOnAudioDeviceStateChanged(const char* deviceId, int deviceType, int deviceState);
	public delegate void DelOnVideoDeviceStateChanged(const char* deviceId, int deviceType, int deviceState);
	public delegate void DelOnLastmileQuality(int quality);
	public delegate void DelOnFirstLocalVideoFrame(int width, int height, int elapsed);
	public delegate void DelOnFirstRemoteVideoDecoded(uid_t uid, int width, int height, int elapsed);
	public delegate void DelOnFirstRemoteVideoFrame(uid_t uid, int width, int height, int elapsed);
	public delegate void DelOnUserJoined(uid_t uid, int elapsed);
	public delegate void DelOnUserOffline(uid_t uid, USER_OFFLINE_REASON_TYPE reason);
	public delegate void DelOnUserMuteAudio(uid_t uid, bool muted);
	public delegate void DelOnUserMuteVideo(uid_t uid, bool muted);
	public delegate void DelOnApiCallExecuted(const char* api, int error);
	public delegate void DelOnStreamMessage(uid_t uid, int streamId, const char* data, size_t length);
	public delegate void DelOnLocalVideoStats(const LocalVideoStats& stats);
	public delegate void DelOnRemoteVideoStats(const RemoteVideoStats& stats);
	public delegate void DelOnCameraReady();
	public delegate void DelOnVideoStopped();
	public delegate void DelOnConnectionLost();
	public delegate void DelOnConnectionInterrupted();
	public delegate void DelOnUserEnableVideo(uid_t uid, bool enabled);
	public delegate void DelOnStartRecordingService(int error);
	public delegate void DelOnStopRecordingService(int error);
	public delegate void DelOnRefreshRecordingServiceStatus(int status);

	public class AgoraEngineEventHandler : public IRtcEngineEventHandler
	{
	public:
		AgoraEngineEventHandler();
		~AgoraEngineEventHandler();

	public: // callback
		FunOnJoinChannelSuccess pOnJoinChannelSuccess;
		FunOnRejoinChannelSuccess pOnRejoinChannelSuccess;
		FunOnWarning pOnWarning;
		FunOnError pOnError;
		FunOnAudioQuality pOnAudioQuality;
		FunOnAudioVolumeIndication pOnAudioVolumeIndication;
		FunOnLeaveChannel pOnLeaveChannel;
		FunOnRtcStats pOnRtcStats;
		FunOnMediaEngineEvent pOnMediaEngineEvent;
		FunOnAudioDeviceStateChanged pOnAudioDeviceStateChanged;
		FunOnVideoDeviceStateChanged pOnVideoDeviceStateChanged;
		FunOnLastmileQuality pOnLastmileQuality;
		FunOnFirstLocalVideoFrame pOnFirstLocalVideoFrame;
		FunOnFirstRemoteVideoDecoded pOnFirstRemoteVideoDecoded;
		FunOnFirstRemoteVideoFrame pOnFirstRemoteVideoFrame;
		FunOnUserJoined pOnUserJoined;
		FunOnUserOffline pOnUserOffline;
		FunOnUserMuteAudio pOnUserMuteAudio;
		FunOnUserMuteVideo pOnUserMuteVideo;
		FunOnApiCallExecuted pOnApiCallExecuted;
		FunOnStreamMessage pOnStreamMessage;
		FunOnLocalVideoStats pOnLocalVideoStats;
		FunOnRemoteVideoStats pOnRemoteVideoStats;
		FunOnCameraReady pOnCameraReady;
		FunOnVideoStopped pOnVideoStopped;
		FunOnConnectionLost pOnConnectionLost;
		FunOnConnectionInterrupted pOnConnectionInterrupted;
		FunOnUserEnableVideo pOnUserEnableVideo;
		FunOnStartRecordingService pOnStartRecordingService;
		FunOnStopRecordingService pOnStopRecordingService;
		FunOnRefreshRecordingServiceStatus pOnRefreshRecordingServiceStatus;

	public: // impl IRtcEngineEventHandler
		virtual void onJoinChannelSuccess(const char* channel, uid_t uid, int elapsed);
		virtual void onRejoinChannelSuccess(const char* channel, uid_t uid, int elapsed);
		virtual void onWarning(int warn, const char* msg);
		virtual void onError(int err, const char* msg);
		virtual void onAudioQuality(uid_t uid, int quality, unsigned short delay, unsigned short lost);
		virtual void onAudioVolumeIndication(const AudioVolumeInfo* speakers, unsigned int speakerNumber, int totalVolume);

		virtual void onLeaveChannel(const RtcStats& stat);
		virtual void onRtcStats(const RtcStats& stat);
		virtual void onMediaEngineEvent(int evt);

		virtual void onAudioDeviceStateChanged(const char* deviceId, int deviceType, int deviceState);
		virtual void onVideoDeviceStateChanged(const char* deviceId, int deviceType, int deviceState);

		virtual void onLastmileQuality(int quality);
		virtual void onFirstLocalVideoFrame(int width, int height, int elapsed);
		virtual void onFirstRemoteVideoDecoded(uid_t uid, int width, int height, int elapsed);
		virtual void onFirstRemoteVideoFrame(uid_t uid, int width, int height, int elapsed);
		virtual void onUserJoined(uid_t uid, int elapsed);
		virtual void onUserOffline(uid_t uid, USER_OFFLINE_REASON_TYPE reason);
		virtual void onUserMuteAudio(uid_t uid, bool muted);
		virtual void onUserMuteVideo(uid_t uid, bool muted);
		virtual void onApiCallExecuted(const char* api, int error);

		virtual void onStreamMessage(uid_t uid, int streamId, const char* data, size_t length);

		virtual void onLocalVideoStats(const LocalVideoStats& stats);
		virtual void onRemoteVideoStats(const RemoteVideoStats& stats);
		virtual void onCameraReady();
		virtual void onVideoStopped();
		virtual void onConnectionLost();
		virtual void onConnectionInterrupted();

		virtual void onUserEnableVideo(uid_t uid, bool enabled);

		virtual void onStartRecordingService(int error);
		virtual void onStopRecordingService(int error);
		virtual void onRefreshRecordingServiceStatus(int status);
	};
}
			}
		}
	}
}