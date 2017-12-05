using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
class OrganizationBean {
    public Int64 id {
        get;
        set;
    }
    public string no {
        get;
        set;
    }
    public string organizationType {
        get;
        set;
    }
    public string name {
        get;
        set;
    }
    public string logoStorageRecordId {
        get;
        set;
    }
    public string introduction {
        get;
        set;
    }
    public string leader {
        get;
        set;
    }
    public string telephone {
        get;
        set;
    }
    public string fax {
        get;
        set;
    }
    public string address {
        get;
        set;
    }
    public string postcode {
        get;
        set;
    }
    public Boolean @virtual {
        get;
        set;
    }
    public Boolean deleted {
        get;
        set;
    }
    public string parentId {
        get;
        set;
    }
    public string sortNum {
        get;
        set;
    }
}
}
