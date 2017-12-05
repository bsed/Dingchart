using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.MediaHelper {
public class VoiceHelper {

    private static IWaveSource soundSource;
    private static ISoundOut soundOut;
    private static Boolean IsforceStop = false;

    /// <summary>
    ///返回播放状态
    /// </summary>
    /// <returns></returns>
    public static PlaybackState GetPlayState() {
        try {
            if (soundOut != null) {
                return soundOut.PlaybackState;
            }
        } catch (Exception e) {
            Log.Error(typeof(VoiceHelper), e);
        }
        return PlaybackState.Stopped;
    }


    /// <summary>
    /// 停止播放
    /// </summary>
    public static void StopPlayASound(Boolean isforceStop) {
        try {
            IsforceStop = isforceStop;
            if (soundOut != null) {
                if (soundOut.PlaybackState == PlaybackState.Playing || soundOut.PlaybackState == PlaybackState.Paused) {
                    soundOut.Stop();
                }
                soundOut.Dispose();
                soundOut = null;
            }
            if (soundSource != null) {
                soundSource.Dispose();
                soundSource = null;
            }

        } catch (Exception e) {
            Log.Error(typeof(VoiceHelper), e);
        }
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param Name="fileStorageId"></param>
    public static void StartPlayASound(string fileStorageId,string checkMark, Dictionary<String, Object> extras) {

        try {
            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId);
            if (table!=null) {
                //Contains the sound to play
                soundSource = GetSoundSource(table.localpath);
                soundOut = GetSoundOut();
                soundOut.Initialize(soundSource);
                soundOut.Stopped += (s, eve) => {
                    try {
                        if (extras==null) {
                            extras = new Dictionary<string, object>();
                        }
                        extras.Add("IsforceStop", IsforceStop);
                        pushVoiceEvent(fileStorageId, checkMark,BusinessEventDataType.VoiceStopPlay, extras);
                        //Console.WriteLine("播放停止：" + fileStorageId);
                        IsforceStop = false;

                    } catch (Exception e) {
                        Log.Error(typeof(VoiceHelper), e);
                    }
                };

                soundOut.Play();
                // 语音开始播放事件
                pushVoiceEvent(fileStorageId,checkMark, BusinessEventDataType.VoiceStartPlay, extras);

            }
        } catch (Exception e) {
            Log.Error(typeof(VoiceHelper), e);
        }
    }

    /// <summary>
    /// 抛出语音播放事件
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <param Name="type"></param>
    /// <param Name="extras"></param>
    public static void pushVoiceEvent(string fileStorageId,string checkMark,BusinessEventDataType type, Dictionary<String, Object> extras) {
        try {
            //通知语音开始播放
            BusinessEvent<Object> businessdata = new BusinessEvent<Object>();
            MediaEventData mediaBean = new MediaEventData();
            mediaBean.fileStorageId = fileStorageId;
            mediaBean.checkMark = checkMark;
            mediaBean.extras = extras;
            businessdata.data = mediaBean;
            businessdata.eventDataType = type;
            EventBusHelper.getInstance().fireEvent(businessdata);
        } catch (Exception e) {
            Log.Error(typeof(VoiceHelper), e);
        }
    }

    /// <summary>
    /// 播放系统声音
    /// </summary>
    /// <param Name="fileStorageId"></param>
    public static void StartPlayASysSound(String localpath) {

        try {
            // 如果有播放的声音，不播放
            if (soundOut != null) {
                if (soundOut.PlaybackState == PlaybackState.Playing || soundOut.PlaybackState == PlaybackState.Paused) {
                    return;
                }
            }

            if (soundOut != null) {
                soundOut.Dispose();
                soundOut = null;
            }
            if (soundSource != null) {
                soundSource.Dispose();
                soundSource = null;
            }

            //Contains the sound to play
            soundSource = GetSoundSource(localpath);
            soundOut = GetSoundOut();
            soundOut.Initialize(soundSource);
            soundOut.Stopped += (s, eve) => {
                try {
                    //Console.WriteLine("播放停止：" + fileStorageId);
                } catch (Exception e) {
                    Log.Error(typeof(VoiceHelper), e);
                }
            };

            soundOut.Play();
        } catch (Exception e) {
            Log.Error(typeof(VoiceHelper), e);
        }
    }

    private static ISoundOut GetSoundOut() {
        if (soundOut !=null) {
            soundOut.Dispose();
            soundOut = null;
        }
        if (WasapiOut.IsSupportedOnCurrentPlatform)
            return new WasapiOut();
        else
            return new DirectSoundOut();
    }

    private static IWaveSource GetSoundSource(String localpath) {
        //return any source ... in this example, we'll just play a mp3 file
        if (soundSource != null) {
            soundSource.Dispose();
            soundSource = null;
        }
        return CodecFactory.Instance.GetCodec(@localpath);
    }

    //public static void CoverArmToWmv(long fileStorageId) {
    //    try {
    //        FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId);
    //        if (table != null) {
    //            if (".amr".Equals(table.fileType)) {
    //                String path = @table.localpath;
    //                String outpath = path.Replace(".amr", ".wmv");
    //                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
    //                ffMpeg.ConvertMedia(path, outpath, Format.wmv);
    //                ffMpeg.ConvertProgress += (sender, args) => {
    //                    // TODO 转换进度
    //                };
    //                // 转换完成后更新记录表
    //                table.localpath = outpath;
    //                table.fileType = ".wmv";
    //                FilesDao.getInstance().save(table);
    //            }
    //        }

    //    } catch (Exception e) {
    //        Log.Error(typeof(VoiceHelper), e);
    //    }
    //}
}
}
