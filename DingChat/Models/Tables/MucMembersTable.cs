using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {

/**
 * 群成员数据库类
 * @author geese
 *
 */
[Entity(TableName = "muc_members")]
public class MucMembersTable : EntityBase {
    /** 默认的字段ID */


    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }



    /** 群no */
    [Column(ColumnName = "mucno")]
    [Length(100)]
    public String mucno {
        get;
        set;
    }
    /** 群id */
    [Column(ColumnName = "mucId")]
    [Length(100)]
    public String mucId {
        get;
        set;
    }
    /** 用户no */
    [Column(ColumnName = "no")]
    [Length(100)]
    public String no {
        get;
        set;
    }

    /** 用户id */
    [Column(ColumnName = "clientuserId")]
    [Length(100)]
    public String clientuserId {
        get;
        set;
    }

    /** 用户名 */
    [Column(ColumnName = "nickname")]
    [Length(100)]
    public String nickname {
        get;
        set;
    }

    /** 头像标识 */
    [Column(ColumnName = "AvatarStorageRecordId")]
    [Length(100)]
    public String avatarStorageRecordId {
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