using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Business.Cache;

namespace cn.lds.chatcore.pcw.DataSqlite {
class OrganizationMemberDao : BaseDao {
    private static OrganizationMemberDao instance = null;
    public static OrganizationMemberDao getInstance() {
        if (instance == null) {
            instance = new OrganizationMemberDao();
        }
        return instance;
    }

    private OrgMemberCacheEntity getCacheEntity(String orgId,String memberNo) {
        OrgMemberCacheEntity org = CacheHelper.getInstance().getOrgMember(orgId,memberNo);
        if (org == null) {
            return null;
        }
        if (org.table == null) {
            return null;
        }
        return org;
    }

    private OrganizationMemberTable getFromCache(String orgId,String memberNo) {
        OrgMemberCacheEntity org = CacheHelper.getInstance().getOrgMember(orgId,memberNo);
        if (org == null)
            return null;
        if (org.table == null) {
            return null;
        }
        return org.table;
    }

    private void updateCache(OrganizationMemberTable table) {
        try {
            //首先获取组织
            OrganizationCacheEntity orgCache = CacheHelper.getInstance().getOrganization(table.organizationId);
            if(orgCache == null) {
                OrganizationDao.getInstance().FindOrganizationByOrgId(table.organizationId.ToInt());
                orgCache = CacheHelper.getInstance().getOrganization(table.organizationId);
                if (orgCache==null) {
                    return;
                }
            }

            OrgMemberCacheEntity memCache = CacheHelper.getInstance().getOrgMember(table.organizationId,table.no);
            if (memCache == null) {
                memCache = new OrgMemberCacheEntity();
                IdNoCacheEntity memIdNo = new IdNoCacheEntity();
                memIdNo.id = table.no;
                memIdNo.no = table.no;
                memCache.sortNum = int.Parse(table.sortNum);
                if (table.sortNum == null || table.sortNum.Trim().Equals("")) {
                    memCache.sortNum = 100000;
                }
                memCache.id = memIdNo;
                VcardsDao.getInstance().findByNo(table.no);
                VcardCacheEntity vcard =  CacheHelper.getInstance().getVcard(table.no);
                memCache.vcard = vcard;
                memCache.table = table;
                orgCache.addMember(memCache);
            }
            //CacheHelper.getInstance().addOrganization(orgCache);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
    }



    /// <summary>
    /// 更新信息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(OrganizationMemberTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            if (table.sortNum == null || table.sortNum.Trim().Equals("")) {
                table.sortNum = "100000";
            }

            entity.Add("email", table.email);
            entity.Add("memberId", table.memberId);
            entity.Add("userId", table.userId);
            entity.Add("deleted", table.deleted);
            entity.Add("no", table.no);
            entity.Add("nickname", table.nickname);
            entity.Add("avatarId", table.avatarId);
            entity.Add("jobDescription", table.jobDescription);
            entity.Add("organizationId", table.organizationId);
            entity.Add("post", table.post);
            entity.Add("office", table.office);
            entity.Add("officeTel", table.officeTel);
            entity.Add("sortNum", table.sortNum);
            entity.Add("location", table.location);
            entity.Add("empno", table.empno);
            entity.Add("tenantNo", table.tenantNo);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.nickname)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.nickname);
                fristPinyin = PinyinHelper.getFristPinyin(table.nickname);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);

            Dictionary<string, object> entityExist = new Dictionary<string, object>();
            entityExist.Add("organizationId", table.organizationId);
            entityExist.Add("userId", table.userId);
            entityExist.Add("no", table.no);

            if (this.isExist("organization_member", entityExist)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("organizationId",table.organizationId),
                    new SQLiteParameter("userId",table.userId),
                    new SQLiteParameter("no",table.no)
                };
                count = this._mgr.Update("organization_member", entity, "userId=@userId", param);
            } else {
                count = this._mgr.Save("organization_member", entity);
            }

            this.updateCache(table);

        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
        return count;


    }
    /// <summary>
    /// 插入组织成员
    /// </summary>
    /// <param Name="dt"></param>
    public int InsertOrganizationMember(List<OrganizationMemberTable> beanList) {

        int count = -1;
        List<string> listDel = new List<string>();
        for (int i = 0; i < beanList.Count; i++) {
            if (!listDel.Contains(beanList[i].organizationId)) {
                listDel.Add(beanList[i].organizationId);
            }
        }

        try {

            for (int i = 0; i < listDel.Count; i++) {
                //删除
                this._mgr.Delete("organization_member", "Organizationid=@Organization_id", new SQLiteParameter[] {
                    new SQLiteParameter("Organization_id", listDel[i])
                });
                try {
                    OrganizationCacheEntity orgcache =  CacheHelper.getInstance().getOrganization(listDel[i]);
                    if (orgcache != null) {
                        orgcache.clearMembers();
                    }
                } catch (Exception ex) {
                    Log.Error(typeof(OrganizationMemberDao), ex);
                }
            }

            List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();

            //插入信息
            for (int i = 0; i < beanList.Count; i++) {
                if (beanList[i].deleted == true) {
                    continue;
                }
                OrganizationMemberTable bean = beanList[i];
                this.save(bean);

                //Dictionary<string, object> entity = new Dictionary<string, object>();
                //entity.Add("memberid", bean.memberId);
                //entity.Add("userid", bean.userId);
                //entity.Add("no", bean.no);
                //entity.Add("nickname", bean.nickname);
                //entity.Add("avatarId", bean.avatarId);
                //entity.Add("jobDescription", bean.jobDescription);
                ////entity.Add("remark", bean.r);
                //entity.Add("organizationId", bean.organizationId);
                //entity.Add("post", bean.post);
                //entity.Add("office", bean.office);
                //entity.Add("officeTel", bean.officeTel);
                //entity.Add("sortNum", bean.sortNum);

                //entitys.Add(entity);


            }
            //count = this._mgr.Save("organization_member", entitys);
            //执行完事之后才能进入主界面


            CacheHelper.getInstance().loadAllOrganization();



        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过组织ID查找成员
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByOrgId(int id) {

        List<OrganizationMemberTable> table = new List<OrganizationMemberTable>();
        // 缓存中处理
        try {
            IList members = CacheHelper.getInstance().getOrganization(id.ToStr()).getMembers();
            if(members != null && members.Count > 0) {
                foreach(Object obj in members) {
                    OrgMemberCacheEntity mem = (OrgMemberCacheEntity)obj;
                    table.Add(mem.table);
                }
                return table;
            }

        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
        // 查询DB
        try {

            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("Organizationid", id);

            Dictionary<string, object> orders = new Dictionary<string, object>();
            orders.Add("CAST (sortNum as int)", "asc");
            orders.Add("totalPinyin", "asc");

            DataTable dt = this._mgr.QueryDt("organization_member", entity, orders);

            table = DataUtils.DataTableToModelList<OrganizationMemberTable>(dt);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
        return table;
    }


    /// <summary>
    /// 通过组织ID查找成员
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByOrgIdFromDB(int id) {

        List<OrganizationMemberTable> table = null;

        try {

            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("Organizationid", id);

            Dictionary<string, object> orders = new Dictionary<string, object>();
            orders.Add("CAST (sortNum as int)", "asc");
            orders.Add("totalPinyin", "asc");

            DataTable dt = this._mgr.QueryDt("organization_member", entity,orders);

            table = DataUtils.DataTableToModelList<OrganizationMemberTable>(dt);
            return table;
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
            return null;
        }
    }

    /// <summary>
    /// 通过成员ID查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByUserId(int id) {
        List<OrganizationMemberTable> table = null;
        DataTable dt = this._mgr.QueryDt("organization_member", "Userid", id);

        table = DataUtils.DataTableToModelList<OrganizationMemberTable>(dt);
        return table;
    }

    public List<OrganizationMemberTable> FindOrganizationMemberByUserIdAndTent(int id,string tent) {

        List<OrganizationMemberTable> table = null;
        Dictionary<string, object> entity = new Dictionary<string, object>();
        entity.Add("Userid", id);
        entity.Add("TenantNo", tent);

        Dictionary<string, object> orders = new Dictionary<string, object>();
        orders.Add("CAST (sortNum as int)", "asc");
        orders.Add("totalPinyin", "asc");

        DataTable dt = this._mgr.QueryDt("organization_member", entity, orders);


        table = DataUtils.DataTableToModelList<OrganizationMemberTable>(dt);
        return table;
    }
    /// <summary>
    /// 通过成员ID查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public OrganizationMemberTable FindOrganizationMemberByMemberId(String memberId) {
        OrganizationMemberTable table = null;
        DataRow entity = this._mgr.QueryOne("organization_member", "memberId", memberId);
        if (entity == null) {
            return null;
        }
        table = DataUtils.DataTableToModel<OrganizationMemberTable>(entity.Table);
        return table;
    }

    /// <summary>
    /// 通过成员ID查找
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public List<OrganizationMemberTable> FindOrganizationMemberByUserNo(String no) {
        List<OrganizationMemberTable> table = null;
        DataTable dt = this._mgr.QueryDt("organization_member", "no", no);

        table = DataUtils.DataTableToModelList<OrganizationMemberTable>(dt);
        return table;
    }

    /// <summary>
    /// 通过id删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByOrgIdAndUserNo(String orgId,String userNo) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("organizationId",orgId),
                new SQLiteParameter("no",userNo)
            };
            count = this._mgr.Delete("organization_member", "organizationId=@organizationId and no=@no", param);
            CacheHelper.getInstance().removeOrgMember(orgId, userNo);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }

        //_mgr.CommitTrans();
        return count;

    }
    public int DeleteByTenantNo(String tenantNo) {
        int count = -1;
        try {
            String sql = string.Empty;
            sql = "delete from  organization_member where  tenantNo = '" + tenantNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
        CacheHelper.getInstance().loadAllOrganization();
        return count;

    }

    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  organization_member where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }
    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<OrganizationMemberTable> list = null;
            String sql = "select * from organization_member where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<OrganizationMemberTable>(dt);
            if (list != null) {
                foreach (OrganizationMemberTable table in list) {
                    this.save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationMemberDao), e);
        }
    }
}
}
