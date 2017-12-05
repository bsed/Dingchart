using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.Enums {
public enum DownloadType {

    SYSTEM_IMAGE,// 头像
    SYSTEM_APP_IMAGE,// 应用头像
    SYSTEM_BARCODE,// 系统缓存(二维码)
    MSG_VOICE, //语音
    MSG_VIDEO, //视频
    MSG_FILE, //文件
    MSG_IMAGE //图片
}
}
