using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 好友基本信息
 * 好友详细信息在vcards中
 */
[Entity(TableName = "contacts")]
public class ContactsTable : EntityBase {
    /** 默认的字段ID */


    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }



    /** 好友ID */
    [Column(ColumnName = "clientuserId")]
    [Length(100)]
    public String clientuserId {
        get;
        set;
    }

    /** 好友编号 */
    [Column(ColumnName = "no")]
    [Length(100)]
    [Index]
    public String no {
        get;
        set;
    }

    /** 好友的头像 */
    [Column(ColumnName = "AvatarStorageRecordId")]
    [Length(100)]
    public string avatarStorageRecordId {
        get;
        set;
    }
    public BitmapImage logoPath {
        get;
        set;
    }
    /** 好友的用户姓名 */
    [Column(ColumnName = "Name")]
    [Length(256)]
    [Index]
    public String name {
        get;
        set;
    }

    /** 好友的用户昵称 */
    [Column(ColumnName = "nickname")]
    [Length(256)]
    public String nickname {
        get;
        set;
    }

    [Column(ColumnName = "alias")]
    [Length(256)]
    public String alias {
        get;
        set;
    }

    /** 是否为我的星标朋友 */


    [Column(ColumnName = "favorite")]
    public Boolean favorite {
        get;
        set;
    }

    /** 好友状态 */
    [Column(ColumnName = "flag")]
    [Length(32)]
    public String flag {
        get;
        set;
    }

    /** 被对方删好友了 */


    [Column(ColumnName = "deletedBy")]
    public Boolean deletedBy {
        get;
        set;
    }

    /** 拼音全拼 */
    [Column(ColumnName = "totalPinyin")]
    [Length(256)]
    [Index]
    public String totalPinyin {
        get;
        set;
    }

    /** 拼音首字母 */
    [Column(ColumnName = "fristPinyin")]
    [Length(256)]
    [Index]
    public String fristPinyin {
        get;
        set;
    }

}
}