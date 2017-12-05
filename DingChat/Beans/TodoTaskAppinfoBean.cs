using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.util;

namespace cn.lds.chatcore.pcw.Beans {
public class TodoTaskAppinfoBean {

    public String downloadUrl {
        get;
        set;
    }



    public string iosAppOpenUrl {
        get;
        set;
    }


    public string iosDownloadUrl {
        get;
        set;
    }

    public List<KeyValueBean> parameters {
        get;
        set;
    }


    public String thirdAppName {
        get;
        set;
    }


    public String thirdAppOpenUrl {
        get;
        set;
    }

    public String thirdAppLogoId {
        get;
        set;
    }

}
}
