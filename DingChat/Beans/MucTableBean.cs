using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class MucTableBean {
    public Int64 id {
        get;
        set;
    }
    public string name {
        get;
        set;
    }
    public string no {
        get;
        set;
    }
    public string avatarStorageRecordId {
        get;
        set;
    }
    public Boolean enableNoDisturb {
        get;
        set;
    }
    public Boolean isTopmost {
        get;
        set;
    }
    public Boolean activeFlag {
        get;
        set;
    }
    public Boolean deleteFlag {
        get;
        set;
    }
    public string manager {
        get;
        set;
    }
    public IList<MucMembersTableBean> members {
        get;
        set;
    }
    public Boolean savedAsContact {
        get;
        set;
    }
    public string qRcodeId {
        get;
        set;
    }
    public Boolean needUpdateAvatar {
        get;
        set;
    }
    public Int64 count {
        get;
        set;
    }





}
}
