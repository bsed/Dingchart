using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * Created by quwei on 2015/12/24.
 */
[Entity(TableName = "master")]
public class MasterTable : EntityBase {
    /** 默认的字段ID */

    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }


    /** 码表类型 */
    [Column(ColumnName = "mastertype")]
    [Length(100)]
    public String mastertype {
        get;
        set;
    }

    /** 键 */
    [Column(ColumnName = "key")]
    [Length(100)]
    [Index]
    public String key {
        get;
        set;
    }

    /** 键值 */
    [Column(ColumnName = "value")]
    [Length(4000)]
    public String value {
        get;
        set;
    }

    /** 键名 */
    [Column(ColumnName = "text")]
    [Length(4000)]
    public String text {
        get;
        set;
    }

    /** 父键 */
    [Column(ColumnName = "parentKey")]
    [Length(100)]
    public String parentKey {
        get;
        set;
    }

    /** 排序 */


    [Column(ColumnName = "sortOrder")]
    public String sortOrder {
        get;
        set;
    }

    /** 描述 */
    [Column(ColumnName = "description")]
    [Length(4000)]
    public String description {
        get;
        set;
    }

    /** 层级 */
    [Column(ColumnName = "treelevel")]
    [Length(10)]
    public String treelevel {
        get;
        set;
    }

    //租户
    [Column(ColumnName = "tenantNo")]
    public string tenantNo {
        get;
        set;
    }

}
}