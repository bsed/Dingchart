using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.MediaHelper {
public class VideoHelper {
    /// <summary>
    /// 播放视频
    /// </summary>
    /// <param Name="localpath"></param>
    public static void StartPlayAVideo(String localpath) {
        try {
            System.Diagnostics.Process.Start(localpath);
        } catch (Exception e) {
            Log.Error(typeof(VideoHelper), e);
        }

    }

    /// <summary>
    /// 播放视频
    /// </summary>
    /// <param Name="localpath"></param>
    public static void StartPlayAVideo(FilesTable dt) {
        try {
            System.Diagnostics.Process.Start(dt.localpath);
        } catch (Exception e) {
            Log.Error(typeof(VideoHelper), e);
        }

    }
}
}
