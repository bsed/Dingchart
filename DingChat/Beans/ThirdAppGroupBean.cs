using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class ThirdAppGroupBean {
    /**
    * 第三方应用分组Id
    */

    public Int64 id {
        get;
        set;
    }

    /**
    * 分组名称
    */

    public String name {
        get;
        set;
    }

    /**
    * 分组图标
    */

    public String iconId {
        get;
        set;
    }

    /**
    * 分组排序字段
    */
    public Int64 sortNum {
        get;
        set;
    }

    /**
    * 分组key给基础应用
    */
    public String key {
        get;
        set;
    }

    /// <summary>
    /// 分组下的分类
    /// </summary>
    public List<ThirdAppClassBean> appClassifications {
        get;
        set;
    }
}
}
