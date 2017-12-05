using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * Created by quwei on 2015/11/25.
 * 聊天回话表
 */
[Entity(TableName = "chat_session")]
public class ChatSessionTable : EntityBase {
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
     * 登录帐号
     */
    [Column(ColumnName = "Account")]
    [Length(100)]
    [Index]
    public String account {
        get;
        set;
    }
    /**
     * 会话类型：单聊，群聊，公众号
     */
    [Column(ColumnName = "chatType")]
    [Length(100)]
    public String chatType {
        get;
        set;
    }
    /**
     * 会话对象编号（好友no、群no、公众号no）
     */
    [Column(ColumnName = "user")]
    [Length(100)]
    public String user {
        get;
        set;
    }
    /**
     * 说话人编号
     */
    [Column(ColumnName = "resource")]
    [Length(100)]
    public String resource {
        get;
        set;
    }
    /**
     * 会话内容(只保留最近一条)
     */
    [Column(ColumnName = "LastMessage")]
    [Image]
    public String lastMessage {
        get;
        set;
    }
    /**
     * 更新时间
     */

    [Column(ColumnName = "Timestamp")]
    public string timestamp {
        get;
        set;
    }
    /**
     * 是否有人@我
     */


    [Column(ColumnName = "Atme")]
    public Boolean atme {
        get;
        set;
    }

    /**
     * 会话草稿
     */


    [Column(ColumnName = "Chatdraft")]
    [Image]
    public String chatdraft {
        get;
        set;
    }
    /**
     * 消息id
     */
    [Column(ColumnName = "messageId")]
    [Length(100)]
    public String messageId {
        get;
        set;
    }

    /**
    * 未读消息数
    */
    [Column(ColumnName = "unReadMessageCount")]
    public Int64 unReadMessageCount {
        get;
        set;
    }

}
}
