using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Models.Tables;
namespace cn.lds.chatcore.pcw.Business.Cache {
class GroupCacheEntity {


    public IdNoCacheEntity id {
        get;
        set;
    }

    IList members = ArrayList.Synchronized(new ArrayList());

    public MucTable table {
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
        if (obj.GetType() != typeof(GroupCacheEntity)) {
            return false;
        }
        GroupCacheEntity target = (GroupCacheEntity)obj;
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

    public IList getMembers() {
        return this.members;
    }
    public bool addMember(GroupMemberCacheEntity groupMember) {
        lock (this.members.SyncRoot) {
            try {
                if (groupMember == null || groupMember.vcard == null) {
                    return false;
                }
                this.removeMember(groupMember);
            } catch (Exception e) {

            }
            this.members.Add(groupMember);
        }
        return true;
    }



    public bool removeMember(IdNoCacheEntity id) {
        VcardCacheEntity vcard = new VcardCacheEntity();
        vcard.id = id;
        GroupMemberCacheEntity groupMember = new GroupMemberCacheEntity();
        groupMember.vcard = vcard;
        this.members.Remove(groupMember);
        return true;
    }

    public bool removeMember(String groupIdOrNo,String id) {
        VcardCacheEntity vcard = new VcardCacheEntity();
        IdNoCacheEntity idCard = new IdNoCacheEntity();
        //VcardsTable vcardsTable = VcardsDao.getInstance().findByClientuserId(id);
        GroupMemberCacheEntity groupMember = CacheHelper.getInstance().getGroupMember(groupIdOrNo, id);
        idCard.no = id;
        vcard.id = idCard;
        //GroupMemberCacheEntity groupMember = new GroupMemberCacheEntity();
        //groupMember.vcard = vcard;
        this.members.Remove(groupMember);
        return true;
    }


    public GroupMemberCacheEntity getMember(String userIdOrNo) {
        IdNoCacheEntity idno = new IdNoCacheEntity();
        idno.id = userIdOrNo;

        foreach (var member in this.members) {
            GroupMemberCacheEntity gmember = (GroupMemberCacheEntity)member;
            if (gmember.vcard == null || gmember.vcard.id == null) {
                continue;
            }

            if (gmember.vcard.id.Equals(idno)) {
                return gmember;
            }
        }

        //GroupMemberCacheEntity[] groupMemberArray = this.members.toArray(new GroupMemberCacheEntity[0]);
        //for (final GroupMemberCacheEntity gmember : groupMemberArray) {
        //    if (gmember.getVcard().getId().equals(idno)) {
        //        return gmember;
        //    }
        //}
        return null;
    }


    public bool removeMember(GroupMemberCacheEntity groupMember) {
        lock (this.members.SyncRoot) {
            this.members.Remove(groupMember);
        }
        return true;
    }


}
}
