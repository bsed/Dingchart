// AgoraVideoClr.h

#pragma once

#include <string>
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include "sdk/include/IAgoraRtcEngine.h"
#include "AgoraEngineEventHandler.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;
using namespace agora::rtc;
namespace cn {
	namespace lds {
		namespace chatcore {
			namespace pcw {
				namespace AgoraVideoClr {

					public enum class ChannelProfileClr
					{
						CHANNEL_PROFILE_COMMUNICATION = 0,
						CHANNEL_PROFILE_LIVE_BROADCASTING = 1,
						CHANNEL_PROFILE_GAME = 2,
					};

					public enum class VideoProfileTypeClr
					{                                   // res       fps  kbps
						VIDEO_PROFILE_120P = 0,         // 160x120   15   65
						VIDEO_PROFILE_120P_3 = 2,       // 120x120   15   50
						VIDEO_PROFILE_180P = 10,        // 320x180   15   140
						VIDEO_PROFILE_180P_3 = 12,      // 180x180   15   100
						VIDEO_PROFILE_180P_4 = 13,      // 240x180   15   120
						VIDEO_PROFILE_240P = 20,        // 320x240   15   200
						VIDEO_PROFILE_240P_3 = 22,      // 240x240   15   140
						VIDEO_PROFILE_240P_4 = 23,      // 424x240   15   220
						VIDEO_PROFILE_360P = 30,        // 640x360   15   400
						VIDEO_PROFILE_360P_3 = 32,      // 360x360   15   260
						VIDEO_PROFILE_360P_4 = 33,      // 640x360   30   600
						VIDEO_PROFILE_360P_6 = 35,      // 360x360   30   400
						VIDEO_PROFILE_360P_7 = 36,      // 480x360   15   320
						VIDEO_PROFILE_360P_8 = 37,      // 480x360   30   490
						VIDEO_PROFILE_360P_9 = 38,      // 640x360   15   800
						VIDEO_PROFILE_360P_10 = 39,     // 640x360   24   800
						VIDEO_PROFILE_360P_11 = 100,    // 640x360   24   1000
						VIDEO_PROFILE_480P = 40,        // 640x480   15   500
						VIDEO_PROFILE_480P_3 = 42,      // 480x480   15   400
						VIDEO_PROFILE_480P_4 = 43,      // 640x480   30   750
						VIDEO_PROFILE_480P_6 = 45,      // 480x480   30   600
						VIDEO_PROFILE_480P_8 = 47,		// 848x480   15   610
						VIDEO_PROFILE_480P_9 = 48,		// 848x480   30   930
						VIDEO_PROFILE_480P_10 = 49,		// 640x480   10   400
						VIDEO_PROFILE_720P = 50,        // 1280x720  15   1130
						VIDEO_PROFILE_720P_3 = 52,      // 1280x720  30   1710
						VIDEO_PROFILE_720P_5 = 54,      // 960x720   15   910
						VIDEO_PROFILE_720P_6 = 55,      // 960x720   30   1380
						VIDEO_PROFILE_1080P = 60,       // 1920x1080 15   2080
						VIDEO_PROFILE_1080P_3 = 62,     // 1920x1080 30   3150
						VIDEO_PROFILE_1080P_5 = 64,     // 1920x1080 60   4780
						VIDEO_PROFILE_1440P = 66,       // 2560x1440 30   4850
						VIDEO_PROFILE_1440P_2 = 67,     // 2560x1440 60   7350
						VIDEO_PROFILE_4K = 70,          // 3840x2160 30   8910
						VIDEO_PROFILE_4K_3 = 72,        // 3840x2160 60   13500
						VIDEO_PROFILE_DEFAULT = VIDEO_PROFILE_360P,
					};

					public enum class ClientRoleTypeClr
					{
						CLIENT_ROLE_BROADCASTER = 1,
						CLIENT_ROLE_AUDIENCE = 2,
					};

					public enum class RenderModeClr
					{
						RENDER_MODE_HIDDEN = 1,
						RENDER_MODE_FIT = 2,
						RENDER_MODE_ADAPTIVE = 3,
					};

					public enum class UserOfflineReasonTypeClr
					{
						USER_OFFLINE_QUIT = 0,
						USER_OFFLINE_DROPPED = 1,
						USER_OFFLINE_BECOME_AUDIENCE = 2,
					};

					public ref class AudioVolumeInfoClr
					{
					public:
						int uid;
						unsigned int volume; // [0,255]
					};

					public ref class RtcStatsClr
					{
					public:
						unsigned int duration;
						unsigned int txBytes;
						unsigned int rxBytes;
						unsigned short txKBitRate;
						unsigned short rxKBitRate;

						unsigned short rxAudioKBitRate;
						unsigned short txAudioKBitRate;

						unsigned short rxVideoKBitRate;
						unsigned short txVideoKBitRate;

						unsigned int users;
						double cpuAppUsage;
						double cpuTotalUsage;

						RtcStatsClr(agora::rtc::RtcStats raw) {
							cpuAppUsage = raw.cpuAppUsage;
							cpuTotalUsage = raw.cpuTotalUsage;
							duration = raw.duration;
							rxBytes = raw.rxBytes;
							txBytes = raw.txBytes;
							txKBitRate = raw.txKBitRate;
							rxKBitRate = raw.rxKBitRate;
							rxAudioKBitRate = raw.rxAudioKBitRate;
							txAudioKBitRate = raw.txAudioKBitRate;
							rxVideoKBitRate = raw.rxVideoKBitRate;
							txVideoKBitRate = raw.txVideoKBitRate;
							users = raw.users;
						}
					};


					public delegate void JoinChannelSuccessHandler(String^ channel, uid_t uid, int elapsed);
					public delegate void RejoinChannelSuccessHandler(String^ channel, uid_t uid, int elapsed);
					public delegate void WarningHandler(int warn, String^ msg);
					public delegate void ErrorHandler(int err, String^ msg);
					public delegate void AudioQualityHandler(uid_t uid, int quality, unsigned short delay, unsigned short lost);
					public delegate void AudioVolumeIndicationHandler(List<AudioVolumeInfoClr^>^ speakers, unsigned int speakerNumber, int totalVolume);
					public delegate void LeaveChannelHandler(RtcStatsClr^ stat);
					public delegate void RtcStatsHandler(RtcStatsClr^ stat);
					public delegate void MediaEngineEventHandler(int evt);
					public delegate void AudioDeviceStateChangedHandler(String^ deviceId, int deviceType, int deviceState);
					public delegate void VideoDeviceStateChangedHandler(String^ deviceId, int deviceType, int deviceState);
					public delegate void LastmileQualityHandler(int quality);
					public delegate void FirstLocalVideoFrameHandler(int width, int height, int elapsed);
					public delegate void FirstRemoteVideoDecodedHandler(uid_t uid, int width, int height, int elapsed);
					public delegate void FirstRemoteVideoFrameHandler(uid_t uid, int width, int height, int elapsed);
					public delegate void UserJoinedHandler(uid_t uid, int elapsed);
					public delegate void UserOfflineHandler(uid_t uid, UserOfflineReasonTypeClr reason);
					public delegate void UserMuteAudioHandler(uid_t uid, bool muted);
					public delegate void UserMuteVideoHandler(uid_t uid, bool muted);
					public delegate void ApiCallExecutedHandler(String^ api, int error);
					public delegate void StreamMessageHandler(uid_t uid, int streamId, String^ data, size_t length);
					public delegate void LocalVideoStatsHandler(const LocalVideoStats& stats);
					public delegate void RemoteVideoStatsHandler(const RemoteVideoStats& stats);
					public delegate void CameraReadyHandler();
					public delegate void VideoStoppedHandler();
					public delegate void ConnectionLostHandler();
					public delegate void ConnectionInterruptedHandler();
					public delegate void UserEnableVideoHandler(uid_t uid, bool enabled);
					public delegate void StartRecordingServiceHandler(int error);
					public delegate void StopRecordingServiceHandler(int error);
					public delegate void RefreshRecordingServiceStatusHandler(int status);


					public ref class Agora
					{
					public:
						Agora();
						~Agora();
						!Agora();

					private:
						IRtcEngine	             *m_lpAgoraEngine;
						AgoraEngineEventHandler  *m_lpEventHandler;
						List<GCHandle>^          m_pGcHandleList;

						bool                     m_bVideoEnabled;
						bool                     m_bSetUpLocalVideo;
						bool                     m_bStartLocalPreview;

						void initializeEventHandler();
						void *RegsiterEvent(Object^ obj);

					public:
						int Initialize(String^ app_id);
						void Release();
						int SetChannelProfile(ChannelProfileClr profile);
						int SetEncryptionSecret(String^ encKey, int encType);
						int DisableAudio();
						int EnableAudio();
						int DisableVideo();
						int EnableVideo();
						int SetLocalVideoProfile(VideoProfileTypeClr profile, bool isWHSwap);
						int SetupLocalVideo(RenderModeClr renderMode, IntPtr^ hWnd);
						int StartLocalPreview();
						int StopLocalPreview();
						int SetupRemoteVideo(unsigned int uid, RenderModeClr renderMode, IntPtr^ hWnd);

						int JoinChannel(String^ channelKey, String^ channelName, String^ info, uid_t uid);


						int LeaveChannel();


						int DisableLocalAudio();
						int EnableLocalAudio();
						int DisableRemoteAudio(unsigned int uid);
						int EnableRemoteAudio(unsigned int uid);
						int DisableAllRemoteAudio();
						int EnableAllRemoteAudio();

						int DisableLocalVideo();
						int EnableLocalVideo();
						int DisableRemoteVideo(unsigned int uid);
						int EnableRemoteVideo(unsigned int uid);
						int DisableAllRemoteVideo();
						int EnableAllRemoteVideo();

						int SetVolume(int volume);
						int SetLogFilePath(String^ path);
						int EnableNetworkTest();
						int DisableNetworkTest();
						int StartScreenCapture(IntPtr^ hWnd);
						//int SetScreenCaptureWindow(IntPtr^ hWnd);
						int StopScreenCapture();
						
						// 业务处理
						bool IsInitialized();
						bool IsVideoEnabled();
						bool IsSetUpLocalVideo();
						bool IsStartLocalPreview();

					private: // CallBack
						void JoinChannelSuccessCallBack(const char* channel, uid_t uid, int elapsed);
						void RejoinChannelSuccessCallBack(const char* channel, uid_t uid, int elapsed);
						void WarningCallBack(int warn, const char* msg);
						void ErrorCallBack(int err, const char* msg);
						void AudioQualityCallBack(uid_t uid, int quality, unsigned short delay, unsigned short lost);
						void AudioVolumeIndicationCallBack(const AudioVolumeInfo* speakers, unsigned int speakerNumber, int totalVolume);
						void LeaveChannelCallBack(const RtcStats& stat);
						void RtcStatsCallBack(const RtcStats& stat);
						void MediaEngineEventCallBack(int evt);
						void AudioDeviceStateChangedCallBack(const char* deviceId, int deviceType, int deviceState);
						void VideoDeviceStateChangedCallBack(const char* deviceId, int deviceType, int deviceState);
						void LastmileQualityCallBack(int quality);
						void FirstLocalVideoFrameCallBack(int width, int height, int elapsed);
						void FirstRemoteVideoDecodedCallBack(uid_t uid, int width, int height, int elapsed);
						void FirstRemoteVideoFrameCallBack(uid_t uid, int width, int height, int elapsed);
						void UserJoinedCallBack(uid_t uid, int elapsed);
						void UserOfflineCallBack(uid_t uid, USER_OFFLINE_REASON_TYPE reason);
						void UserMuteAudioCallBack(uid_t uid, bool muted);
						void UserMuteVideoCallBack(uid_t uid, bool muted);
						void ApiCallExecutedCallBack(const char* api, int error);
						void StreamMessageCallBack(uid_t uid, int streamId, const char* data, size_t length);
						void LocalVideoStatsCallBack(const LocalVideoStats& stats);
						void RemoteVideoStatsCallBack(const RemoteVideoStats& stats);
						void CameraReadyCallBack();
						void VideoStoppedCallBack();
						void ConnectionLostCallBack();
						void ConnectionInterruptedCallBack();
						void UserEnableVideoCallBack(uid_t uid, bool enabled);
						void StartRecordingServiceCallBack(int error);
						void StopRecordingServiceCallBack(int error);
						void RefreshRecordingServiceStatusCallBack(int status);
					public: // Event
						JoinChannelSuccessHandler^ OnJoinChannelSuccess;
						RejoinChannelSuccessHandler^ OnRejoinChannelSuccess;
						WarningHandler^ OnWarning;
						ErrorHandler^ OnError;
						AudioQualityHandler^ OnAudioQuality;
						AudioVolumeIndicationHandler^ OnAudioVolumeIndication;
						LeaveChannelHandler^ OnLeaveChannel;
						RtcStatsHandler^ OnRtcStats;
						MediaEngineEventHandler^ OnMediaEngineEvent;
						AudioDeviceStateChangedHandler^ OnAudioDeviceStateChanged;
						VideoDeviceStateChangedHandler^ OnVideoDeviceStateChanged;
						LastmileQualityHandler^ OnLastmileQuality;
						FirstLocalVideoFrameHandler^ OnFirstLocalVideoFrame;
						FirstRemoteVideoDecodedHandler^ OnFirstRemoteVideoDecoded;
						FirstRemoteVideoFrameHandler^ OnFirstRemoteVideoFrame;
						UserJoinedHandler^ OnUserJoined;
						UserOfflineHandler^ OnUserOffline;
						UserMuteAudioHandler^ OnUserMuteAudio;
						UserMuteVideoHandler^ OnUserMuteVideo;
						ApiCallExecutedHandler^ OnApiCallExecuted;
						StreamMessageHandler^ OnStreamMessage;
						LocalVideoStatsHandler^ OnLocalVideoStats;
						RemoteVideoStatsHandler^ OnRemoteVideoStats;
						CameraReadyHandler^ OnCameraReady;
						VideoStoppedHandler^ OnVideoStopped;
						ConnectionLostHandler^ OnConnectionLost;
						ConnectionInterruptedHandler^ OnConnectionInterrupted;
						UserEnableVideoHandler^ OnUserEnableVideo;
						StartRecordingServiceHandler^ OnStartRecordingService;
						StopRecordingServiceHandler^ OnStopRecordingService;
						RefreshRecordingServiceStatusHandler^ OnRefreshRecordingServiceStatus;

					};
				}
			}
		}
	}
}