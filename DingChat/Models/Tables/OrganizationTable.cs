using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 组织表
 */
[Entity(TableName = "organization")]
public class OrganizationTable : EntityBase {
    /** 默认的字段ID */

    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }


    /** 组织ID */
    [Column(ColumnName = "organizationId")]
    [Length(100)]
    [Index]
    public String organizationId {
        get;
        set;
    }

    /**  业务编号 */
    [Column(ColumnName = "no")]
    [Length(100)]
    [Index]
    public String no {
        get;
        set;
    }

    /** 组织名称 */
    [Column(ColumnName = "Name")]
    [Length(256)]
    [Index]
    public String name {
        get;
        set;
    }

    /** 所属组织ID */
    [Column(ColumnName = "parentId")]
    [Length(100)]
    [Index]
    public String parentId {
        get;
        set;
    }

    /** 头像 */
    [Column(ColumnName = "logoStorageRecordId")]
    [Length(100)]
    public String logoStorageRecordId {
        get;
        set;
    }
    public String logoPath {
        get;
        set;
    }
    /** 简介 */
    [Column(ColumnName = "introduction")]
    [Length(4000)]
    public String introduction {
        get;
        set;
    }

    /** 负责人 */
    [Column(ColumnName = "leader")]
    [Length(100)]
    public String leader {
        get;
        set;
    }

    /** 电话 */
    [Column(ColumnName = "telephone")]
    [Length(100)]
    public String telephone {
        get;
        set;
    }

    /** 传真 */
    [Column(ColumnName = "fax")]
    [Length(100)]
    public String fax {
        get;
        set;
    }

    /** 地址 */
    [Column(ColumnName = "address")]
    [Length(2000)]
    public String address {
        get;
        set;
    }

    /** 邮编 */
    [Column(ColumnName = "postcode")]
    [Length(10)]
    public String postcode {
        get;
        set;
    }

    public Boolean deleted {
        get;
        set;
    }


    /** 虚拟组织标记 */

    [Column(ColumnName = "virtual")]
    public Boolean @virtual {
        get;
        set;
    }

    /** 排序字段 */
    [Column(ColumnName = "sortNum")]
    public String sortNum {
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
