using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Enums {
public enum FileType {
    /// <summary>
    /// 未知
    /// </summary>
    UNKNOWN,
    /// <summary>
    /// 图片
    /// </summary>
    IMAGES,
    /// <summary>
    /// 音频
    /// </summary>
    VOICE,
    /// <summary>
    /// 视频
    /// </summary>
    VEDIO,
    /// <summary>
    /// apk
    /// </summary>
    APK,
    /// <summary>
    /// 二维码
    /// </summary>
    QRCODE,
    /// <summary>
    /// 文件
    /// </summary>
    FILE
}
}
