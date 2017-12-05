using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class MasterBean {

    public string key {
        get;
        set;
    }
    public string value {
        get;
        set;
    }
    public string text {
        get;
        set;
    }
    public string parentKey {
        get;
        set;
    }
    public string order {
        get;
        set;
    }
    public string description {
        get;
        set;
    }
    public string[] children {
        get;
        set;
    }


}
}
