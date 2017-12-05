using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Business.Cache {
class IdNoCacheEntity {
    public String id {
        get;
        set;
    }
    public String no {
        get;
        set;
    }

    public String getIdString() {
        return "_" + id + "_" + no + "_";
    }

    public String getKey() {
        return this.no;
    }

    public override int GetHashCode() {
        return this.no.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        if (obj.GetType() != typeof(IdNoCacheEntity)) {
            return false;
        }
        IdNoCacheEntity target = (IdNoCacheEntity)obj;
        return this.idNoEquals(target);

    }

    private bool idNoEquals(IdNoCacheEntity idNoCacheEntity) {
        if (idNoCacheEntity == null) {
            return false;
        }
        if (idNoCacheEntity.id != null) {
            if (idNoCacheEntity.id.Equals(this.id)
                    || idNoCacheEntity.id.Equals(this.no)) {
                return true;
            }
        }
        if (idNoCacheEntity.no != null) {
            if (idNoCacheEntity.no.Equals(this.id)
                    || idNoCacheEntity.no.Equals(this.no)) {
                return true;
            }
        }
        return false;
    }
}
}
