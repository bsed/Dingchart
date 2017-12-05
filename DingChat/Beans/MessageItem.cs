using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class  MessageItem {

    /// <summary>
    /// 默认的字段ID
    /// </summary>
    public Int64 id;

    /// <summary>
    /// 消息ID
    /// </summary>
    public String messageId;

    /// <summary>
    /// 服务器端消息ID
    /// </summary>
    public String serverMessageId;

    /// <summary>
    /// 消息来源（好友或群组帐号）,与后台no'字段对应
    /// </summary>
    public String user;

    /// <summary>
    /// 说话的人（好友帐号）,与后台no'字段对应
    /// </summary>
    public String resource;

    /// <summary>
    /// 格式化显示文字
    /// </summary>
    public String text;

    /// <summary>
    /// 音频、视频、多图文等消息的消息体，JSON格式，客户端自行封装，与服务器格式无关
    /// </summary>
    public String content;

    /// <summary>
    /// 时间
    /// </summary>
    public String timestamp;

    /// <summary>
    /// 服务器消息时间，发送消息，timestamp和delay_timestamp为客户端时间
    /// 接收消息：timestamp为本地时间，delay_timestamp为服务器时间
    /// </summary>
    public String delayTimestamp;

    /// <summary>
    /// MsgType
    /// </summary>
    public String type;

    /// <summary>
    /// 是否是收到的消息
    /// </summary>
    public Boolean incoming;

    /// <summary>
    /// 消息已读/未读
    /// </summary>
    public Boolean read;

    /// <summary>
    /// 音频/视频是否播放
    /// 语音消息标示是否点击收听   视频消息时 标示是否下载
    /// </summary>
    public Boolean flag;

    /// <summary>
    /// -1:失败；0：发送中；1：已发送
    /// 发送消息时标示发送状态   接收消息时  标示下载状态  ==0时下载中   ==1时未下载
    /// </summary>
    public String sent;

    /// <summary>
    /// 消息解析错误
    /// </summary>
    public Boolean error;

    /// <summary>
    /// 消息是否是@我
    /// </summary>
    public Boolean atme;

    /// <summary>
    ///
    /// </summary>
    public Boolean showTimestamp;


    /// <summary>
    /// 说话人姓名
    /// </summary>
    public String name;

    /// <summary>
    /// 说话人头像
    /// </summary>
    public String avatar;

    /// <summary>
    /// 说话人头像本地地址
    /// </summary>
    public String localAvatar;

    /// <summary>
    ///
    /// </summary>
    public DateTime timeDate;

    /// <summary>
    ///
    /// </summary>
    public DateTime delayTimeDate;

    /// <summary>
    /// 显示时间标识
    /// </summary>
    public Boolean displayTimeFlag;

    //自己发送文件的时候记录文件路径
    public string filePath;


    public string tenantNo;
}
}
