using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class ThirdAppClassBean {

    /**
    * 第三方应用分类Id
    */

    public Int64 id {
        get;
        set;
    }

    /**
    * 分类名称
    */

    public String name {
        get;
        set;
    }

    /**
    * 分类图标
    */

    public String iconId {
        get;
        set;
    }

    /**
    * 分类排序字段
    */
    public Int64 sortNum {
        get;
        set;
    }

}
}
