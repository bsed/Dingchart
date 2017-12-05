using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class VcardsTableBean {
    public Int64 id {
        get;
        set;
    }
    public string no {
        get;
        set;
    }
    public string nickname {
        get;
        set;
    }
    public string alias {
        get;
        set;
    }
    public string avatarStorageRecordId {
        get;
        set;
    }
    public string[] tags {
        get;
        set;
    }
    public string mobileNumber {
        get;
        set;
    }
    // TODO: (#‵′) :临时的处理方法，查询好友、拉取成员详情等接口，电话的返回字段不一致。
    public string mobile {
        get;
        set;
    }
    public string birthday {
        get;
        set;
    }
    public string gender {
        get;
        set;
    }
    public string country {
        get;
        set;
    }
    public string province {
        get;
        set;
    }
    public string city {
        get;
        set;
    }
    public string moodMessage {
        get;
        set;
    }
    public Boolean favorite {
        get;
        set;
    }
    public string flag {
        get;
        set;
    }
    public string email {
        get;
        set;
    }
    public string identity {
        get;
        set;
    }
    public string weChatId {
        get;
        set;
    }

}
}
