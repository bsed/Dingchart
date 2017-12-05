using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Business.Cache {
class VcardCacheEntity {

    public IdNoCacheEntity id {
        get;
        set;
    }

    public String alias {
        get;
        set;
    }

    private bool isFriend = false;

    public VcardsTable table {
        get;
        set;
    }

    public bool IsFriend {
        get {
            return isFriend;
        } set {
            isFriend = value;
        }
    }

    public String getKey() {
        return this.id.getKey();
    }

    public override int GetHashCode() {
        return this.id.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (obj.GetType() != typeof(VcardCacheEntity)) {
            return false;
        }
        VcardCacheEntity target = (VcardCacheEntity)obj;

        if (this.id == null || this.table == null || target.id == null || target.table == null) {
            return false;
        }

        if (this.id.Equals(target.id)) {
            return true;
        }
        return false;
    }


}
}
