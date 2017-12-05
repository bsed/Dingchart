using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "public_accounts")]
public class PublicAccountsTable : EntityBase {
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
     * 公众号ID
     */

    [Column(ColumnName = "appid")]
    [Length(256)]
    public String appid {
        get;
        set;
    }
    /**
     * 开放平台服务帐号的所有者用户编号
     */

    [Column(ColumnName = "userno")]
    [Length(100)]
    public String userno {
        get;
        set;
    }

    /**
     * 公众号名称
     */

    [Column(ColumnName = "Name")]
    [Length(256)]
    [Index]
    public String name {
        get;
        set;
    }

    /**
     * 帐号主体
     */

    [Column(ColumnName = "ownerName")]
    [Length(1024)]
    public String ownerName {
        get;
        set;
    }

    /**
     * 头像
     */

    [Column(ColumnName = "logoId")]
    [Length(128)]
    public String logoId {
        get;
        set;
    }
    public BitmapImage logoPath {
        get;
        set;
    }
    /**
     * 介绍
     */

    [Column(ColumnName = "introduction")]
    [Length(4000)]
    public String introduction {
        get;
        set;
    }

    /**
     * 状态
     */
    [Column(ColumnName = "status")]
    public String status {
        get;
        set;
    }

    /**
     * 位置上报
     */
    [Column(ColumnName = "location")]
    public Boolean location {
        get;
        set;
    }

    /**
     * 菜单
     */

    [Column(ColumnName = "menu")]
    [Length(4000)]
    public String menu {
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

    //租户
    [Column(ColumnName = "tenantNo")]
    public string tenantNo {
        get;
        set;
    }

    public string tenantName {
        get;
        set;
    }
}
}