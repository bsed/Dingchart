using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Business.Cache {
class OrgMemberCacheEntity : IComparable {
    public IdNoCacheEntity id {
        get;
        set;
    }
    public VcardCacheEntity vcard {
        get;
        set;
    }
    public OrganizationMemberTable table {
        get;
        set;
    }
    public int sortNum {
        get;
        set;
    }
    public int CompareTo(object obj) {
        if (obj == null) return -1;
        if (!obj.GetType().Equals(typeof(OrgMemberCacheEntity))) {
            return -1;
        }
        OrgMemberCacheEntity entity = (OrgMemberCacheEntity)obj;
        if (entity.id.Equals(this.id)) {
            return 0;
        } else {
            if (this.sortNum > entity.sortNum) {
                return 1;
            } else {
                return -1;
            }
        }

    }

    public override int GetHashCode() {
        return this.id.GetHashCode();
    }
    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (obj.GetType() != typeof(OrgMemberCacheEntity)) {
            return false;
        }
        OrgMemberCacheEntity target = (OrgMemberCacheEntity)obj;
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
