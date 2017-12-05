using System;
using System.Collections.Generic;
using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Beans;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 登录人登录信息
 */
[Entity(TableName = "accounts")]
public class AccountsTable : EntityBase {
    /** 默认的字段ID */
    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    [NotNull]
    public Int64 id {
        get;
        set;
    }

    /**
     * 用户编号，对应no字段
     */
    [Column(ColumnName = "no")]
    [Length(100)]
    [NotNull]
    public String no {
        get;
        set;
    }

    /**
     * 一次性认证token
     */
    [Column(ColumnName = "nonceToken")]
    [Length(100)]
    public String nonceToken {
        get;
        set;
    }



    /**
     * 名称
     */

    [Column(ColumnName = "Name")]
    [Length(256)]
    public String name {
        get;
        set;
    }
    /**
     * 昵称
     */
    [Column(ColumnName = "nickname")]
    [Length(256)]
    public String nickname {
        get;
        set;
    }

    /**
     * 手机号
     */
    [Column(ColumnName = "mobile")]
    [Length(20)]
    public String mobile {
        get;
        set;
    }

    /**
     * 邮箱
     */
    [Column(ColumnName = "email")]
    [Length(256)]
    public String email {
        get;
        set;
    }

    /**
     * 登录名
     */
    [Column(ColumnName = "loginId")]
    [Length(256)]
    public String loginId {
        get;
        set;
    }

    /**
     * 手机App OpenId
     */
    [Column(ColumnName = "subscriptionOpenId")]
    [Length(20)]
    public String subscriptionOpenId {
        get;
        set;
    }

    /////////////////////以下字段是个人详情返回的//////////////////////////
    /**
     * 用户ID
     */
    [Column(ColumnName = "clientuserId")]
    [Length(256)]
    public String clientuserId {
        get;
        set;
    }

    /**
     * 头像
     */
    [Column(ColumnName = "AvatarStorageRecordId")]
    [Length(100)]
    public string avatarStorageRecordId {
        get;
        set;
    }

    /**
     * 生日
     */
    [Column(ColumnName = "birthday")]
    [Length(32)]
    public String birthday {
        get;
        set;
    }

    /**
     * 性别
     */
    [Column(ColumnName = "gender")]
    [Length(4)]
    public String gender {
        get;
        set;
    }

    /**
     * 国家
     */
    [Column(ColumnName = "country")]
    [Length(100)]
    public String country {
        get;
        set;
    }

    /**
     * 省份
     */
    [Column(ColumnName = "province")]
    [Length(100)]
    public String province {
        get;
        set;
    }

    /**
     * 城市
     */
    [Column(ColumnName = "city")]
    [Length(100)]
    public String city {
        get;
        set;
    }

    /**
     * 签名
     */
    [Column(ColumnName = "desc")]
    [Length(4000)]
    public String desc {
        get;
        set;
    }

    /**
     * 我的二维码
     */
    [Column(ColumnName = "qrcodeId")]
    [Length(128)]
    public string qrcodeId {
        get;
        set;
    }

    /**
     * 是否开启免打扰
     */

    [Column(ColumnName = "enableNoDisturb")]
    public Boolean enableNoDisturb {
        get;
        set;
    }

    /**
     * 免打扰开始时间，格式：HHmm
     */
    [Column(ColumnName = "startTimeOfNoDisturb")]
    [Length(100)]
    public String startTimeOfNoDisturb {
        get;
        set;
    }

    /**
     * 免打扰结束时间，格式：HHmm
     */
    [Column(ColumnName = "endTimeOfNoDisturb")]
    [Length(100)]
    public String endTimeOfNoDisturb {
        get;
        set;
    }

    /**
     * 加我为朋友时是否需要验证
     */

    [Column(ColumnName = "needFriendConfirmation")]
    public Boolean needFriendConfirmation {
        get;
        set;
    }
    /**
     * 是否允许向我推荐通讯录朋友
     */

    [Column(ColumnName = "allowFindMobileContacts")]
    public Boolean allowFindMobileContacts {
        get;
        set;
    }
    /**
     * 是否允许通过登录名称找到我
     */

    [Column(ColumnName = "allowFindMeByLoginId")]
    public Boolean allowFindMeByLoginId {
        get;
        set;
    }
    /**
     * 是否允许通过手机号码找到我
     */

    [Column(ColumnName = "allowFindMeByMobileNumber")]
    public Boolean allowFindMeByMobileNumber {
        get;
        set;
    }


    /// <summary>
    /// 租户列表
    /// </summary>
    public Dictionary<string, LoginBeanTenants> tenants {
        get;
        set;
    }
}
}


