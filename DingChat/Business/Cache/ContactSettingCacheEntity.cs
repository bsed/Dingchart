using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Business.Cache {
class ContactSettingCacheEntity {
    public IdNoCacheEntity id {
        get;
        set;
    }
    public SettingsTable table {
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
        if (obj.GetType() != typeof(ContactSettingCacheEntity)) {
            return false;
        }
        ContactSettingCacheEntity target = (ContactSettingCacheEntity)obj;
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
