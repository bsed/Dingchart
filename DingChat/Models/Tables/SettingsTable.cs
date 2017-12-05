using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "setting")]
public class SettingsTable : EntityBase {

    /** 默认的字段ID */
    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }


    /** 好友/群组 */
    [Column(ColumnName = "no")]
    [Length(100)]
    [Index]
    public String no {
        get;
        set;
    }

    /** 置顶 */
    [Column(ColumnName = "Top")]
    public Boolean top {
        get;
        set;
    }

    /** 免打扰 */
    [Column(ColumnName = "Quiet")]
    public Boolean quiet {
        get;
        set;
    }

    /** 聊天背景 */
    [Column(ColumnName = "backgroundurl")]
    [Length(512)]
    public String backgroundurl {
        get;
        set;
    }

    /** 会话状态：临时，正式等，除群主创建群聊外，其他都是正式，群主发送第一条消息后变为正式 */
    [Column(ColumnName = "enabaledraft")]
    public Boolean enabaledraft {
        get;
        set;
    }

}
}