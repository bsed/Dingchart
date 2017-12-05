using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class ChatSessionFilesBean {
    //==============================消息表属性 开始========================================//
    /**
     * 消息ID
     */
    public String messageId {
        get;
        set;
    }

    public String serverMessageId {
        get;
        set;
    }

    public String user {
        get;
        set;
    }

    public String resource {
        get;
        set;
    }

    /**
     * 格式化显示文字
     */
    public String text {
        get;
        set;
    }

    /**
     * 音频、视频、多图文等消息的消息体，JSON格式，客户端自行封装，与服务器格式无关
     */

    public String content {
        get;
        set;
    }

    /**
     * 时间
     */
    public String timestamp {
        get;
        set;
    }
    /**
     * 服务器消息时间，发送消息，timestamp和delay_timestamp为客户端时间
     * 接收消息：timestamp为本地时间，delay_timestamp为服务器时间
     */
    public String delayTimestamp {
        get;
        set;
    }

    /**
     * MsgType
     */
    public String type {
        get;
        set;
    }

    /**
     * 是否是收到的消息
     */
    public Boolean incoming {
        get;
        set;
    }

    /**
     * 消息已读/未读
     */

    public Boolean read {
        get;
        set;
    }

    /**
     * 音频/视频是否播放
     * 语音消息标示是否点击收听   视频消息时 标示是否下载
     */
    public Boolean flag {
        get;
        set;
    }

    /**
     * -1:失败；0：发送中；1：已发送
     * 发送消息时标示发送状态   接收消息时  标示下载状态  ==0时下载中   ==1时未下载
     */

    public String sent {
        get;
        set;
    }

    /**
     * 消息解析错误
     */

    public Boolean error {
        get;
        set;
    }

    /**
     * 消息是否是@我
     */
    public Boolean atme {
        get;
        set;
    }

    public Boolean showTimestamp {
        get;
        set;
    }

    //==============================消息表属性 结束========================================//

    //==============================文件表属性 开始========================================//

    /** 服务器存储ID */
    public long fileStorageId {
        get;
        set;
    }

    /** 标准图下载完成后，本地存储路径 */
    public String localpath {
        get;
        set;
    }

    /** 文件类型(枚举FileType) */
    public String fileType {
        get;
        set;
    }
    /** 文件名称（业务中的名称） */
    public String fileName {
        get;
        set;
    }
    public Int64 size {
        get;
        set;
    }

    /** 音频、视频播放时长 */
    public Int64 duration {
        get;
        set;
    }

    /** 消息ID或其它 */
    public String owner {
        get;
        set;
    }
    //==============================文件表属性 结束========================================//
}
}
