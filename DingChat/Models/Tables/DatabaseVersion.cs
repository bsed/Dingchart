using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 数据库版本表
 * <p/>
 * Created by quwei on 2015/11/26.
 */
[Entity(TableName = "database_version")]
public class DatabaseVersion : EntityBase {
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



    [Column(ColumnName = "databaseVersion")]
    public Int64 databaseVersion {
        get;
        set;
    }

}
}
