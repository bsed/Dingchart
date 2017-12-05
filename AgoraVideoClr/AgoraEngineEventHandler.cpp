#include "stdafx.h"
#include "AgoraEngineEventHandler.h"

using namespace cn::lds::chatcore::pcw::AgoraVideoClr;

AgoraEngineEventHandler::AgoraEngineEventHandler()
{
}
AgoraEngineEventHandler::~AgoraEngineEventHandler()
{
}

/********************Impl IRtcEngineEventHandler Start*****************************************/

void AgoraEngineEventHandler::onJoinChannelSuccess(const char* channel, uid_t uid, int elapsed)
{
	//LPAGE_JOINCHANNEL_SUCCESS lpData = new AGE_JOINCHANNEL_SUCCESS;

	//int nChannelLen = strlen(channel) + 1;
	//lpData->channel = new char[nChannelLen];
	//lpData->uid = uid;
	//lpData->elapsed = elapsed;

	//strcpy_s(lpData->channel, nChannelLen, channel);

	if (pOnJoinChannelSuccess)
		pOnJoinChannelSuccess(channel, uid, elapsed);

	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_JOINCHANNEL_SUCCESS), (WPARAM)lpData, 0);
}

void AgoraEngineEventHandler::onRejoinChannelSuccess(const char* channel, agora::rtc::uid_t uid, int elapsed)
{
	//LPAGE_REJOINCHANNEL_SUCCESS lpData = new AGE_REJOINCHANNEL_SUCCESS;

	//int nChannelLen = strlen(channel) + 1;
	//lpData->channel = new char[nChannelLen];
	//lpData->uid = uid;
	//lpData->elapsed = elapsed;

	//strcpy_s(lpData->channel, nChannelLen, channel);

	if (pOnRejoinChannelSuccess)
		pOnRejoinChannelSuccess(channel, uid, elapsed);

	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_REJOINCHANNEL_SUCCESS), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onWarning(int warn, const char* msg)
{
	//std::string str;
	if (pOnWarning)
		pOnWarning(warn, msg);
	//str = "onWarning";
}

void AgoraEngineEventHandler::onError(int err, const char* msg)
{
	//LPAGE_ERROR lpData = new AGE_ERROR;

	//int nMsgLen = 0;

	//// attention: the pointer of msg maybe NULL!!!
	//if (msg != NULL) {
	//	nMsgLen = strlen(msg) + 1;
	//	lpData->msg = new char[nMsgLen];
	//	strcpy_s(lpData->msg, nMsgLen, msg);
	//}
	//else
	//	lpData->msg = NULL;

	//lpData->err = err;
	if (pOnError)
		pOnError(err, msg);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_ERROR), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onAudioQuality(uid_t uid, int quality, unsigned short delay, unsigned short lost)
{
	//LPAGE_AUDIO_QUALITY lpData = new AGE_AUDIO_QUALITY;

	//lpData->uid = uid;
	//lpData->quality = quality;
	//lpData->delay = delay;
	//lpData->lost = lost;
	if (pOnAudioQuality)
		pOnAudioQuality(uid, quality, delay, lost);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_AUDIO_QUALITY), (WPARAM)lpData, 0);
}


void AgoraEngineEventHandler::onAudioVolumeIndication(const AudioVolumeInfo* speakers, unsigned int speakerNumber, int totalVolume)
{
	//LPAGE_AUDIO_VOLUME_INDICATION lpData = new AGE_AUDIO_VOLUME_INDICATION;

	//lpData->speakers = new AudioVolumeInfo[speakerNumber];
	//memcpy(lpData->speakers, speakers, speakerNumber * sizeof(AudioVolumeInfo));
	//lpData->speakerNumber = speakerNumber;
	//lpData->totalVolume = totalVolume;
	if (pOnAudioVolumeIndication)
		pOnAudioVolumeIndication(speakers, speakerNumber, totalVolume);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_AUDIO_VOLUME_INDICATION), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onLeaveChannel(const RtcStats& stat)
{
	//LPAGE_LEAVE_CHANNEL lpData = new AGE_LEAVE_CHANNEL;

	//memcpy(&lpData->rtcStat, &stat, sizeof(RtcStats));
	if (pOnLeaveChannel)
		pOnLeaveChannel(stat);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_LEAVE_CHANNEL), (WPARAM)lpData, 0);
}

void AgoraEngineEventHandler::onRtcStats(const RtcStats& stat)
{
	//std::string str;
	if (pOnRtcStats)
		pOnRtcStats(stat);
	//str = "stat";
}


void AgoraEngineEventHandler::onMediaEngineEvent(int evt)
{
	//LPAGE_MEDIA_ENGINE_EVENT lpData = new AGE_MEDIA_ENGINE_EVENT;

	//lpData->evt = evt;
	if (pOnMediaEngineEvent)
		pOnMediaEngineEvent(evt);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_MEDIA_ENGINE_EVENT), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onAudioDeviceStateChanged(const char* deviceId, int deviceType, int deviceState)
{
	//LPAGE_AUDIO_DEVICE_STATE_CHANGED lpData = new AGE_AUDIO_DEVICE_STATE_CHANGED;

	//int nDeviceIDLen = strlen(deviceId) + 1;

	//lpData->deviceId = new char[nDeviceIDLen];

	//strcpy_s(lpData->deviceId, nDeviceIDLen, deviceId);
	//lpData->deviceType = deviceType;
	//lpData->deviceState = deviceState;
	if (pOnAudioDeviceStateChanged)
		pOnAudioDeviceStateChanged(deviceId, deviceType, deviceState);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_AUDIO_DEVICE_STATE_CHANGED), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onVideoDeviceStateChanged(const char* deviceId, int deviceType, int deviceState)
{
	//LPAGE_VIDEO_DEVICE_STATE_CHANGED lpData = new AGE_VIDEO_DEVICE_STATE_CHANGED;

	//int nDeviceIDLen = strlen(deviceId) + 1;

	//lpData->deviceId = new char[nDeviceIDLen];

	//strcpy_s(lpData->deviceId, nDeviceIDLen, deviceId);
	//lpData->deviceType = deviceType;
	//lpData->deviceState = deviceState;
	if (pOnVideoDeviceStateChanged)
		pOnVideoDeviceStateChanged(deviceId, deviceType, deviceState);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_VIDEO_DEVICE_STATE_CHANGED), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onLastmileQuality(int quality)
{
	//LPAGE_LASTMILE_QUALITY lpData = new AGE_LASTMILE_QUALITY;

	//lpData->quality = quality;
	if (pOnLastmileQuality)
		pOnLastmileQuality(quality);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_LASTMILE_QUALITY), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onFirstLocalVideoFrame(int width, int height, int elapsed)
{
	//LPAGE_FIRST_LOCAL_VIDEO_FRAME lpData = new AGE_FIRST_LOCAL_VIDEO_FRAME;

	//lpData->width = width;
	//lpData->height = height;
	//lpData->elapsed = elapsed;
	if (pOnFirstLocalVideoFrame)
		pOnFirstLocalVideoFrame(width, height, elapsed);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_FIRST_LOCAL_VIDEO_FRAME), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onFirstRemoteVideoDecoded(uid_t uid, int width, int height, int elapsed)
{
	//LPAGE_FIRST_REMOTE_VIDEO_DECODED lpData = new AGE_FIRST_REMOTE_VIDEO_DECODED;

	//lpData->uid = uid;
	//lpData->width = width;
	//lpData->height = height;
	//lpData->elapsed = elapsed;
	if (pOnFirstRemoteVideoDecoded)
		pOnFirstRemoteVideoDecoded(uid, width, height, elapsed);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_FIRST_REMOTE_VIDEO_DECODED), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onFirstRemoteVideoFrame(uid_t uid, int width, int height, int elapsed)
{
	//LPAGE_FIRST_REMOTE_VIDEO_FRAME lpData = new AGE_FIRST_REMOTE_VIDEO_FRAME;

	//lpData->uid = uid;
	//lpData->width = width;
	//lpData->height = height;
	//lpData->elapsed = elapsed;
	if (pOnFirstRemoteVideoFrame)
		pOnFirstRemoteVideoFrame(uid, width, height, elapsed);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_FIRST_REMOTE_VIDEO_FRAME), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onUserJoined(uid_t uid, int elapsed)
{
	//LPAGE_USER_JOINED lpData = new AGE_USER_JOINED;

	//lpData->uid = uid;
	//lpData->elapsed = elapsed;


	if (pOnUserJoined)
		pOnUserJoined(uid, elapsed);

	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_USER_JOINED), (WPARAM)lpData, 0);
}

void AgoraEngineEventHandler::onUserOffline(uid_t uid, USER_OFFLINE_REASON_TYPE reason)
{
	//LPAGE_USER_OFFLINE lpData = new AGE_USER_OFFLINE;

	//lpData->uid = uid;
	//lpData->reason = reason;
	if (pOnUserOffline)
		pOnUserOffline(uid, reason);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_USER_OFFLINE), (WPARAM)lpData, 0);
}

void AgoraEngineEventHandler::onUserMuteAudio(uid_t uid, bool muted)
{
	//LPAGE_USER_MUTE_AUDIO lpData = new AGE_USER_MUTE_AUDIO;

	//lpData->uid = uid;
	//lpData->muted = muted;
	if (pOnUserMuteAudio)
		pOnUserMuteAudio(uid, muted);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_USER_MUTE_AUDIO), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onUserMuteVideo(uid_t uid, bool muted)
{
	//LPAGE_USER_MUTE_VIDEO lpData = new AGE_USER_MUTE_VIDEO;

	//lpData->uid = uid;
	//lpData->muted = muted;
	if (pOnUserMuteVideo)
		pOnUserMuteVideo(uid, muted);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_USER_MUTE_VIDEO), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onStreamMessage(uid_t uid, int streamId, const char* data, size_t length)
{
	//LPAGE_STREAM_MESSAGE lpData = new AGE_STREAM_MESSAGE;

	//lpData->uid = uid;
	//lpData->streamId = streamId;
	//lpData->data = new char[length];
	//lpData->length = length;

	//memcpy_s(lpData->data, length, data, length);
	if (pOnStreamMessage)
		pOnStreamMessage(uid, streamId, data, length);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_STREAM_MESSAGE), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onApiCallExecuted(const char* api, int error)
{
	//LPAGE_APICALL_EXECUTED lpData = new AGE_APICALL_EXECUTED;

	//strcpy_s(lpData->api, 128, api);
	//lpData->error = error;
	if (pOnApiCallExecuted)
		pOnApiCallExecuted(api, error);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_APICALL_EXECUTED), (WPARAM)lpData, 0);
}

void AgoraEngineEventHandler::onLocalVideoStats(const LocalVideoStats& stats)
{
	//LPAGE_LOCAL_VIDEO_STAT lpData = new AGE_LOCAL_VIDEO_STAT;

	//lpData->sentBitrate = stats.sentBitrate;
	//lpData->sentFrameRate = stats.sentFrameRate;
	if (pOnLocalVideoStats)
		pOnLocalVideoStats(stats);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_LOCAL_VIDEO_STAT), (WPARAM)lpData, 0);

}

void AgoraEngineEventHandler::onRemoteVideoStats(const RemoteVideoStats& stats)
{
	//LPAGE_REMOTE_VIDEO_STAT lpData = new AGE_REMOTE_VIDEO_STAT;

	//lpData->uid = stats.uid;
	//lpData->delay = stats.delay;
	//lpData->width = stats.width;
	//lpData->height = stats.height;
	//lpData->receivedFrameRate = stats.receivedFrameRate;
	//lpData->receivedBitrate = stats.receivedBitrate;
	//lpData->receivedFrameRate = stats.receivedFrameRate;
	if (pOnRemoteVideoStats)
		pOnRemoteVideoStats(stats);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_REMOTE_VIDEO_STAT), (WPARAM)lpData, 0);
}

void AgoraEngineEventHandler::onCameraReady()
{
	if (pOnCameraReady)
		pOnCameraReady();
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_CAMERA_READY), 0, 0);

}

void AgoraEngineEventHandler::onVideoStopped()
{
	if (pOnVideoStopped)
		pOnVideoStopped();
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_VIDEO_STOPPED), 0, 0);
}

void AgoraEngineEventHandler::onConnectionLost()
{
	if (pOnConnectionLost)
		pOnConnectionLost();
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_CONNECTION_LOST), 0, 0);
}

void AgoraEngineEventHandler::onConnectionInterrupted()
{
	//std::string str;
	if (pOnConnectionInterrupted)
		pOnConnectionInterrupted();
	//str = "onConnectionInterrupted";
}

void AgoraEngineEventHandler::onUserEnableVideo(uid_t uid, bool enabled)
{
	if (pOnUserEnableVideo)
		pOnUserEnableVideo(uid, enabled);
	//	if (m_hMainWnd != NULL)
	//		::PostMessage(m_hMainWnd, WM_MSGID(EID_CONNECTION_LOST), 0, 0);

}

void AgoraEngineEventHandler::onStartRecordingService(int error)
{
	if (pOnStartRecordingService)
		pOnStartRecordingService(error);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_START_RCDSRV), 0, 0);

}

void AgoraEngineEventHandler::onStopRecordingService(int error)
{
	if (pOnStopRecordingService)
		pOnStopRecordingService(error);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_STOP_RCDSRV), 0, 0);

}

void AgoraEngineEventHandler::onRefreshRecordingServiceStatus(int status)
{
	//LPAGE_RCDSRV_STATUS lpData = new AGE_RCDSRV_STATUS;

	//lpData->status = status;
	if (pOnRefreshRecordingServiceStatus)
		pOnRefreshRecordingServiceStatus(status);
	//if (m_hMainWnd != NULL)
	//	::PostMessage(m_hMainWnd, WM_MSGID(EID_REFREASH_RCDSRV), (WPARAM)lpData, 0);
}

/**********************************End Impl**************************************************/