using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class TodoTaskContentBean {

    public TodoTaskAppinfoBean appInfo {
        get;
        set;
    }



    public string createTime {
        get;
        set;
    }


    public string detailUrl {
        get;
        set;
    }

    public String msgId {
        get;
        set;
    }


    public String msgType {
        get;
        set;
    }


    public String pcDetailUrl {
        get;
        set;
    }


    public  List<KeyValueBean> properties {
        get;
        set;
    }

    /**
    * NO
    */
    public String title {
        get;
        set;
    }

    /**
    * 应用编号
    */
    public String user {
        get;
        set;
    }
    public string tenantNo {
        get;
        set;
    }

}
}
