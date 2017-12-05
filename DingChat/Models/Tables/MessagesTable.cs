using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "messages")]
public class MessagesTable : EntityBase {
    /**
     * 默认的字段ID
     */


    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }


    /**
     * 消息ID
     */

    [Column(ColumnName = "messageId")]
    [Length(12)]
    [Index]
    public String messageId {
        get;
        set;
    }


    /**
     * 群聊ID
     */

    //[Column(ColumnName = "mucId")]
    //[Length(100)]
    //[Index]
    //public String mucId {
    //    get;
    //    set;
    //}

    /**
     * 服务器端消息ID
     */

    [Column(ColumnName = "serverMessageId")]
    [Length(13)]
    [Index]
    public String serverMessageId {
        get;
        set;
    }



    /**
     * 消息来源（好友或群组帐号）,与后台no'字段对应
     */

    [Column(ColumnName = "user")]
    [Length(100)]
    [Index(IndexColumnName = "user,read,Timestamp")]
    public String user {
        get;
        set;
    }

    /**
     * 说话的人（好友帐号）,与后台no'字段对应
     */

    [Column(ColumnName = "resource")]
    [Length(100)]
    [Index(IndexColumnName = "resource,user")]
    public String resource {
        get;
        set;
    }

    /**
     * 格式化显示文字
     */

    [Column(ColumnName = "text")]
    [Length(1000)]
    public String text {
        get;
        set;
    }

    /**
     * 音频、视频、多图文等消息的消息体，JSON格式，客户端自行封装，与服务器格式无关
     */

    [Column(ColumnName = "content")]
    [Length(5000)]
    public String content {
        get;
        set;
    }

    /**
     * 时间
     */
    [Column(ColumnName = "Timestamp")]
    [Index(IndexColumnName = "user,Timestamp")]
    public String timestamp {
        get;
        set;
    }
    /**
     * 服务器消息时间，发送消息，timestamp和delay_timestamp为客户端时间
     * 接收消息：timestamp为本地时间，delay_timestamp为服务器时间
     */


    [Column(ColumnName = "delayTimestamp")]
    public String delayTimestamp {
        get;
        set;
    }

    /**
     * MsgType
     */

    [Column(ColumnName = "type")]
    [Length(24)]
    public String type {
        get;
        set;
    }

    /**
     * 是否是收到的消息
     */


    [Column(ColumnName = "incoming")]
    public Boolean incoming {
        get;
        set;
    }

    /**
     * 消息已读/未读
     */


    [Column(ColumnName = "read")]
    [Index(IndexColumnName = "user,read")]
    public Boolean read {
        get;
        set;
    }

    /**
     * 音频/视频是否播放
     * 语音消息标示是否点击收听   视频消息时 标示是否下载
     */


    [Column(ColumnName = "flag")]
    public Boolean flag {
        get;
        set;
    }

    /**
     * -1:失败；0：发送中；1：已发送
     * 发送消息时标示发送状态   接收消息时  标示下载状态  ==0时下载中   ==1时未下载
     */


    [Column(ColumnName = "sent")]
    public String sent {
        get;
        set;
    }

    /**
     * 消息解析错误
     */


    [Column(ColumnName = "error")]
    public Boolean error {
        get;
        set;
    }

    /**
     * 消息是否是@我
     */


    [Column(ColumnName = "Atme")]
    public Boolean atme {
        get;
        set;
    }


    [Column(ColumnName = "showTimestamp")]
    public Boolean showTimestamp {
        get;
        set;
    }

        //租户
        [Column(ColumnName = "tenantNo")]
        public string tenantNo
        {
            get;
            set;
        }
    }
}