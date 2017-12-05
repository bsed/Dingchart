using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class TodoTaskBean {
    /**
    * 待办ID
    */
    public String id {
        get;
        set;
    }


    /** 待办的头像 */
    public string appLogoId {
        get;
        set;
    }

    /**
    * 待办Name
    */
    public String appName {
        get;
        set;
    }
    /**
    * 创建人
    */
    public String createdBy {
        get;
        set;
    }

    /**
    * 创建日期
    */
    public String createdDate {
        get;
        set;
    }

    /**
    * 最后编辑者
    */
    public String lastModifiedBy {
        get;
        set;
    }

    /**
    * 最后编辑时间
    */
    public String lastModifiedDate {
        get;
        set;
    }

    /**
    * NO
    */
    public String userNo {
        get;
        set;
    }

    /**
    * 应用编号
    */
    public String appId {
        get;
        set;
    }

    /**
    * 类型
    */
    public String type {
        get;
        set;
    }

    /**
    * 内容
    */
    public TodoTaskContentBean content {
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
    * 地址
    */
    public String detailUrl {
        get;
        set;
    }

    //租户
    public string tenantNo {
        get;
        set;
    }
}
}
