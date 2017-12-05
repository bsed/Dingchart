using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 用户信息表
 * C002.1好友详细，C002.2陌生人详细，C014.2成员详细信息
 */
[Entity(TableName = "vcards")]
public class VcardsTable : EntityBase {
    /** 默认的字段ID */

    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }


    /** 用户编号 */
    [Column(ColumnName = "no")]
    [Length(100)]
    [Index]
    public String no {
        get;
        set;
    }

    /** 用户ID */
    [Column(ColumnName = "clientuserId")]
    [Length(100)]
    public String clientuserId {
        get;
        set;
    }

    /** 用户昵称 */
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
        get ;
        set ;
    }

    /** 电话 */
    [Column(ColumnName = "mobileNumber")]
    [Length(50)]
    public String mobileNumber {
        get;
        set;
    }

    /** 邮箱 */
    [Column(ColumnName = "email")]
    [Length(256)]
    public String email {
        get;
        set;
    }

    /** 座机 */
    [Column(ColumnName = "tel")]
    [Length(256)]
    public String tel {
        get;
        set;
    }

    /** 签名 */
    [Column(ColumnName = "desc")]
    [Length(256)]
    public String desc {
        get;
        set;
    }

    /** 生日 */
    [Column(ColumnName = "birthday")]
    [Length(32)]
    public String birthday {
        get;
        set;
    }

    /** 性别 */
    [Column(ColumnName = "gender")]
    [Length(10)]
    public String gender {
        get;
        set;
    }

    /** 国家 */
    [Column(ColumnName = "country")]
    [Length(512)]
    public String country {
        get;
        set;
    }

    /** 省份 */
    [Column(ColumnName = "province")]
    [Length(128)]
    public String province {
        get;
        set;
    }

    /** 城市 */
    [Column(ColumnName = "city")]
    [Length(512)]
    public String city {
        get;
        set;
    }

    /** 身份 */
    [Column(ColumnName = "identity")]
    [Length(128)]
    public String identity {
        get;
        set;
    }

    /** 微信号 */
    [Column(ColumnName = "wechatid")]
    [Length(256)]
    public String wechatid {
        get;
        set;
    }

}
}
