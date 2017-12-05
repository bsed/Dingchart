using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class PublicAccountsBean {

    /**
     * 公众号ID
     */
    public String appId {
        get;
        set;
    }
    /**
     * 开放平台服务帐号的所有者用户编号
     */
    public String userNo {
        get;
        set;
    }
    /**
     * 公众号名称
     */
    public String name {
        get;
        set;
    }
    /**
    * 头像
    */
    public String logoId {
        get;
        set;
    }
    public String logoPath {
        get;
        set;
    }

    /**
     * 介绍
     */
    public String introduction {
        get;
        set;
    }
    /**
    * 状态
    */
    public String status {
        get;
        set;
    }
    /**
    * 是否提供公众帐号服务
    */
    public Boolean includeSubscription {
        get;
        set;
    }
    /**
    * 是否提供网站应用服务
    */
    public Boolean includeWebsite {
        get;
        set;
    }
    /**
    * 是否开启地理位置上报功能
    */
    public Boolean enableLocationReporting {
        get;
        set;
    }
    /**
    * 是否开启自定义菜单功能
    */
    public Boolean enableCustomMenu {
        get;
        set;
    }
    /**
    * 菜单
    */
    public List<PublicMenus> menus {
        get;
        set;
    }

    /**
    * 菜单
    */
    public String ownerName {
        get;
        set;
    }

    public String tenantNo {
        get;
        set;
    }
    ///**
    // * 主题
    // */
    //public String department;


    ///**
    // * 位置上报
    // */
    //public Boolean location;






}
}
