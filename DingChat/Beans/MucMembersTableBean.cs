using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class MucMembersTableBean {
    public Int64 id {
        get;
        set;
    }
    public string nickname {
        get;
        set;
    }
    public string no {
        get;
        set;
    }
    public Boolean deleteFlag {
        get;
        set;
    }
    public Boolean activeFlag {
        get;
        set;
    }
    public string avatarStorageRecordId {
        get;
        set;
    }

}
}
