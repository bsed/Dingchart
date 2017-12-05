using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Business.Cache {
class SubscribtionCacheEntity {
    public IdNoCacheEntity id {
        get;
        set;
    }

    public PublicAccountsTable table {
        get;
        set;
    }

    public override int GetHashCode() {
        return this.id.GetHashCode();
    }
    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (obj.GetType() != typeof(SubscribtionCacheEntity)) {
            return false;
        }
        SubscribtionCacheEntity target = (SubscribtionCacheEntity)obj;
        if (this.id == null || this.table == null || target.id == null || target.table == null) {
            return false;
        }

        if (this.id.Equals(target.id)) {
            return true;
        }
        return false;
    }


    public String getKey() {
        return this.id.getKey();
    }


}
}
