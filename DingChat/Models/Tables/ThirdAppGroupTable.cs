using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "third_app_group")]
public class ThirdAppGroupTable {

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
    * 第三方应用分组Id
    */

    [Column(ColumnName = "thirdAppGroupId")]
    public Int64 thirdAppGroupId {
        get;
        set;
    }

    /**
    * 分组名称
    */

    [Column(ColumnName = "Name")]
    [Length(100)]
    public String name {
        get;
        set;
    }

    /** key */
    [Column(ColumnName = "key")]
    [Length(256)]
    public String key {
        get;
        set;
    }

    /**
    * 分组图标
    */

    [Column(ColumnName = "iconId")]
    [Length(100)]
    public String iconId {
        get;
        set;
    }

    /**
    * 分组排序字段
    */

    [Column(ColumnName = "sortNum")]
    public Int64 sortNum {
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
