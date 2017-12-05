using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 组织人员表
 */
[Entity(TableName = "organization_member")]
public class OrganizationMemberTable : EntityBase {
    /** 默认的字段ID */

    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }

    /** email */
    [Column(ColumnName = "email")]
    [Length(100)]
    public String email {
        get;
        set;
    }


    /** 组织用户Id */
    [Column(ColumnName = "memberId")]
    [Length(100)]
    public String memberId {
        get;
        set;
    }

    /** 社群用户Id */
    [Column(ColumnName = "userId")]
    [Length(100)]
    public String userId {
        get;
        set;
    }
    [Column(ColumnName = "deleted")]
    public Boolean deleted {
        get;
        set;
    }
    /** 社群用户编号 */
    [Column(ColumnName = "no")]
    [Length(100)]
    public String no {
        get;
        set;
    }

    /** 组织用户名称 */
    [Column(ColumnName = "nickname")]
    [Length(256)]
    public String nickname {
        get;
        set;
    }

    /** 组织用户头像Id */
    //长度修改
    [Column(ColumnName = "avatarId")]
    [Length(100)]
    public String avatarId {
        get;
        set;
    }
    public String avatarPath {
        get;
        set;
    }
    /** 职位描述（枚举：mainJob/主职，auxiliaryJob/副职） */
    [Column(ColumnName = "jobDescription")]
    [Length(4000)]
    public String jobDescription {
        get;
        set;
    }

    /** 备注 */
    [Column(ColumnName = "remark")]
    [Length(4000)]
    public String remark {
        get;
        set;
    }

    /** 所属组织id */
    [Column(ColumnName = "organizationId")]
    [Length(100)]
    public String organizationId {
        get;
        set;
    }

    /** 职称 */
    [Column(ColumnName = "post")]
    [Length(256)]
    public String post {
        get;
        set;
    }

    public String postName {
        get;
        set;
    }

    /** 所属部门id */
    [Column(ColumnName = "office")]
    [Length(100)]
    public String office {
        get;
        set;
    }

    /** 办公电话 */
    [Column(ColumnName = "officeTel")]
    [Length(100)]
    public String officeTel {
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

    /** 地区 */
    [Column(ColumnName = "location")]
    [Length(100)]
    public String location {
        get;
        set;
    }


    /** 工号 */
    [Column(ColumnName = "empno")]
    [Length(100)]
    public String empno {
        get;
        set;
    }
}

}
