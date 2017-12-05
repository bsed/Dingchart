using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "third_app_class")]
class ThirdAppClassTable {

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
    * 第三方应用分类Id
    */

    [Column(ColumnName = "thirdAppClassId")]
    public Int64 thirdAppClassId {
        get;
        set;
    }

    /**
    * 分类名称
    */

    [Column(ColumnName = "Name")]
    [Length(100)]
    public String name {
        get;
        set;
    }

    /**
    * 分类图标
    */

    [Column(ColumnName = "iconId")]
    [Length(100)]
    public String iconId {
        get;
        set;
    }

    /**
    * 分类排序字段
    */

    [Column(ColumnName = "sortNum")]
    public Int64 sortNum {
        get;
        set;
    }

    /**
    * 第三方应用分组Id
    */

    [Column(ColumnName = "thirdAppGroupId")]
    public Int64 thirdAppGroupId {
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
}
}
