using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class AccountsCoreBean {

    public Int64 id {
        get;
        set;
    }
    public string avatarStorageRecordId {
        get;
        set;
    }
    public string moodMessage {
        get;
        set;
    }
    public Boolean enableNoDisturb {
        get;
        set;
    }
    public string startTimeOfNoDisturb {
        get;
        set;
    }
    public string endTimeOfNoDisturb {
        get;
        set;
    }
    public Boolean needFriendConfirmation {
        get;
        set;
    }
    public Boolean allowFindMobileContacts {
        get;
        set;
    }
    public Boolean allowFindMeByLoginId {
        get;
        set;
    }
    public Boolean allowFindMeByMobileNumber {
        get;
        set;
    }
    public string qrcodeId {
        get;
        set;
    }
    //public string member;
    //public string broker;

}
}
