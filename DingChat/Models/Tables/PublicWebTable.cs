using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "public_web")]
public class PublicWebTable {

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
    * 公众号ID
    */

    [Column(ColumnName = "appId")]
    [Length(256)]
    public String appId {
        get;
        set;
    }

    /**
    * 开放平台服务帐号的所有者用户编号
    */

    [Column(ColumnName = "userNo")]
    [Length(100)]
    public String userNo {
        get;
        set;
    }

    /**
    * 公众号名称
    */

    [Column(ColumnName = "Name")]
    [Length(100)]
    public String name {
        get;
        set;
    }


    /**
    * 开放平台服务帐号的功能介绍
    */

    [Column(ColumnName = "introduction")]
    [Length(500)]
    public String introduction {
        get;
        set;
    }

    /**
    * 头像
    */
    //长度修改
    [Column(ColumnName = "logoId")]
    [Length(100)]
    public String logoId {
        get;
        set;
    }

    public String logoPath {
        get;
        set;
    }
    /**
    * 开放平台服务帐号的状态
    */

    [Column(ColumnName = "status")]
    [Length(20)]
    public String status {
        get;
        set;
    }

    /**
    * 是否提供公众帐号服务
    */

    [Column(ColumnName = "includeSubscription")]
    public Boolean includeSubscription {
        get;
        set;
    }

    /**
    * 是否提供网站应用服务
    */

    [Column(ColumnName = "includeWebsite")]
    public Boolean includeWebsite {
        get;
        set;
    }

    /**
    * 是否提供第三方本地app
    */

    [Column(ColumnName = "clientType")]
    [Length(100)]
    public String clientType {
        get;
        set;
    }

    /**
    * 是否提供第三方本地app
    */

    [Column(ColumnName = "includeMobileApp")]
    public Boolean includeMobileApp {
        get;
        set;
    }

    /**
    * 第三方应用包名
    */

    [Column(ColumnName = "androidAppOpenUrl")]
    [Length(1000)]
    public String androidAppOpenUrl {
        get;
        set;
    }

    /**
    * 第三方应用下载地址
    */

    [Column(ColumnName = "androidDownloadUrl")]
    [Length(1000)]
    public String androidDownloadUrl {
        get;
        set;
    }

    /**
    * 第三方应用启动参数
    */

    [Column(ColumnName = "mobileAppParameters")]
    public String mobileAppParameters {
        get;
        set;
    }

    /**
    * 是否已关注
    */

    [Column(ColumnName = "followed")]
    public Boolean followed {
        get;
        set;
    }

    /**
    * 本地排序
    */

    [Column(ColumnName = "sort")]
    [Length(20)]
    public Int64 sort {
        get;
        set;
    }

    /**
    * 服务器排序
    */

    [Column(ColumnName = "appSortIndex")]
    [Length(20)]
    public Int64 appSortIndex {
        get;
        set;
    }

    /**
    * 应用分类ID
    */

    [Column(ColumnName = "appClassificationId")]
    [Length(100)]
    public String appClassificationId {
        get;
        set;
    }

    [Column(ColumnName = "appclaasificationKey")]
    [Length(100)]
    public String appclaasificationKey {
        get;
        set;
    }
    //基础应用的名字
    public String appclaasificationKeyName {
        get;
        set;
    }

    /**
    * 应用分类name
    */

    [Column(ColumnName = "appClassificationName")]
    [Length(100)]
    public String appClassificationName {
        get;
        set;
    }

    /**
    * 应用地址（用于排序，有地址的排前面，无地址的排后面）
    */

    [Column(ColumnName = "url")]
    [Length(1000)]
    public String url {
        get;
        set;
    }

    /**
    * 所有人
    */

    [Column(ColumnName = "ownerName")]
    [Length(100)]
    public String ownerName {
        get;
        set;
    }

    /**
    * 网站应用的状态(inUse--服务中, inConstruction--建设中
    */

    [Column(ColumnName = "websiteStatus")]
    [Length(100)]
    public String websiteStatus {
        get;
        set;
    }

    /**
    * 是否置顶
    */

    [Column(ColumnName = "enableTopmost")]
    public Boolean enableTopmost {
        get;
        set;
    }

    /**
    * 是否允许收消息
    */

    [Column(ColumnName = "allowReceiveMessages")]
    public Boolean allowReceiveMessages {
        get;
        set;
    }

    /**
    * 是否允许分享地址
    */

    [Column(ColumnName = "allowShareMyLocation")]
    public Boolean allowShareMyLocation {
        get;
        set;
    }

    /**
    * 是否是常用应用
    */

    [Column(ColumnName = "commonWebsite")]
    public Boolean commonWebsite {
        get;
        set;
    }

    /**
    * 设为常用时间
    */

    [Column(ColumnName = "commonWebsiteTime")]
    [Length(100)]
    public String commonWebsiteTime {
        get;
        set;
    }

    /**
    * 是否包含应用内组件
    */

    [Column(ColumnName = "includeComponent")]
    public Boolean includeComponent {
        get;
        set;
    }

    /**
    * 应用内组件的Url
    */

    [Column(ColumnName = "componentPhoneUrl")]
    [Length(1000)]
    public String componentPhoneUrl {
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
