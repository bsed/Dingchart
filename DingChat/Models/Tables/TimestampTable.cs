using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * Created by quwei on 2015/11/26.
 * 时间戳管理表
 */
[Entity(TableName = "Timestamp")]
public class TimestampTable : EntityBase {

    /** 默认的字段ID */
    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }



    /** 时间戳类型:见枚举 TimestampType */
    [Column(ColumnName = "type")]
    [Length(100)]
    public String type {
        get;
        set;
    }
    /** 时间戳 */

    [Column(ColumnName = "Timestamp")]
    public Int64 timestamp {
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