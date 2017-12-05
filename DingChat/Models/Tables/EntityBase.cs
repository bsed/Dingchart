using System;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * Created by quwei on 2016/2/23.
 */
public class EntityBase {

    /**
     * 创建时间
     */


    [Column(ColumnName = "createTime")]
    public string createTime {
        get;
        set;
    }

    /**
     * 更新时间
     */

    [Column(ColumnName = "updateTime")]
    public string updateTime {
        get;
        set;
    }
}
}
