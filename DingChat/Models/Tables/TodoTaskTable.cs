using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {

/// <summary>
/// 待办数据表
/// </summary>
[Entity(TableName = "todo_task")]
public class TodoTaskTable : EntityBase {
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


    /** 待办的头像 */
    [Column(ColumnName = "appLogoId")]
    [Length(100)]
    public string appLogoId {
        get;
        set;
    }

    /**
    * 待办Name
    */
    [Column(ColumnName = "appName")]
    [Length(100)]
    public String appName {
        get;
        set;
    }

    /**
    * 待办ID
    */
    [Column(ColumnName = "todoTaskId")]
    [Length(100)]
    public String todoTaskId {
        get;
        set;
    }

    /**
    * 创建人
    */
    [Column(ColumnName = "createdBy")]
    [Length(100)]
    public String createdBy {
        get;
        set;
    }

    /**
    * 创建日期
    */
    [Column(ColumnName = "createdDate")]
    [Length(100)]
    public String createdDate {
        get;
        set;
    }

    /**
    * 最后编辑者
    */
    [Column(ColumnName = "lastModifiedBy")]
    [Length(100)]
    public String lastModifiedBy {
        get;
        set;
    }

    /**
    * 最后编辑时间
    */
    [Column(ColumnName = "lastModifiedDate")]
    [Length(100)]
    public String lastModifiedDate {
        get;
        set;
    }

    /**
    * NO
    */
    [Column(ColumnName = "userNo")]
    [Length(100)]
    public String userNo {
        get;
        set;
    }

    /**
    * 应用编号
    */
    [Column(ColumnName = "appId")]
    [Length(100)]
    public String appId {
        get;
        set;
    }

    /**
    * 类型
    */
    [Column(ColumnName = "type")]
    [Length(100)]
    public String type {
        get;
        set;
    }

    /**
    * 内容
    */
    [Column(ColumnName = "content")]
    [Length(4000)]
    public String content {
        get;
        set;
    }

    /**
    * 状态
    */
    [Column(ColumnName = "status")]
    [Length(100)]
    public String status {
        get;
        set;
    }

    /**
    * 地址
    */
    [Column(ColumnName = "detailUrl")]
    [Length(100)]
    public String detailUrl {
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
