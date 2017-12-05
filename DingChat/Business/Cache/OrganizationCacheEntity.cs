using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Business.Cache {
class OrganizationCacheEntity {
    public IdNoCacheEntity id {
        get;
        set;
    }
    SortedList  members = new SortedList();


    public OrganizationTable table {
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
        if (obj.GetType() != typeof(OrganizationCacheEntity)) {
            return false;
        }
        OrganizationCacheEntity target = (OrganizationCacheEntity)obj;
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

    public void clearMembers() {
        this.members.Clear();
    }

    public List<OrgMemberCacheEntity> getMembers() {
        List<OrgMemberCacheEntity> members = new List<OrgMemberCacheEntity>();
        ICollection mems =  this.members.Keys;
        foreach(Object obj in mems) {
            if (obj == null) {
                return null;
            }
            OrgMemberCacheEntity entity = (OrgMemberCacheEntity)obj;
            members.Add(entity);
        }
        return members;
    }


    public bool addMember(OrgMemberCacheEntity orgMember) {
        lock (this.members.SyncRoot) {
            try {
                if (orgMember == null || orgMember.id == null) {
                    return false;
                }
                this.removeMember(orgMember);
            } catch (Exception e) {

            }
            this.members.Add(orgMember, orgMember);
        }
        return true;
    }



    public bool removeMember(IdNoCacheEntity id) {
        VcardCacheEntity vcard = new VcardCacheEntity();
        vcard.id = id;
        OrgMemberCacheEntity orgMember = new OrgMemberCacheEntity();
        orgMember.id = id;
        orgMember.vcard = vcard;
        this.members.Remove(orgMember);
        return true;
    }

    public bool removeMember(String id) {
        VcardCacheEntity vcard = new VcardCacheEntity();
        IdNoCacheEntity idCard = new IdNoCacheEntity();
        idCard.no = id;
        vcard.id = idCard;
        OrgMemberCacheEntity orgMember = new OrgMemberCacheEntity();
        orgMember.id = idCard;
        orgMember.vcard = vcard;
        this.members.Remove(orgMember);
        return true;
    }


    public OrgMemberCacheEntity getMember(String userIdOrNo) {
        IdNoCacheEntity idno = new IdNoCacheEntity();
        idno.id = userIdOrNo;

        foreach (var member in this.members.Keys) {
            OrgMemberCacheEntity gmember = (OrgMemberCacheEntity)member;
            if (gmember.vcard == null || gmember.vcard.id == null) {
                continue;
            }

            if (gmember.id.Equals(idno)) {
                return gmember;
            }
        }

        //orgMemberCacheEntity[] orgMemberArray = this.members.toArray(new orgMemberCacheEntity[0]);
        //for (final orgMemberCacheEntity gmember : orgMemberArray) {
        //    if (gmember.getVcard().getId().equals(idno)) {
        //        return gmember;
        //    }
        //}
        return null;
    }


    public bool removeMember(OrgMemberCacheEntity orgMember) {
        lock (this.members.SyncRoot) {
            this.members.Remove(orgMember);
        }
        return true;
    }


}
}
