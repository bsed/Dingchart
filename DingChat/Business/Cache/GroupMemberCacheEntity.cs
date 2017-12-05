using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace cn.lds.chatcore.pcw.Business.Cache {
class GroupMemberCacheEntity {

    public String name {
        get;
        set;
    }
    public VcardCacheEntity vcard {
        get;
        set;
    }

    public String getKey() {
        return this.vcard.getKey();
    }

    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (obj.GetType() != typeof(GroupMemberCacheEntity)) {
            return false;
        }
        if (this.vcard == null) {
            return false;
        }
        GroupMemberCacheEntity target = (GroupMemberCacheEntity)obj;
        if (this.vcard.Equals(target.vcard)) {
            return true;
        }
        return false;
    }


}
}
