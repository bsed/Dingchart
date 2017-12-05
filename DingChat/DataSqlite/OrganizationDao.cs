using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Business;

namespace cn.lds.chatcore.pcw.DataSqlite {
class OrganizationDao : BaseDao {

    private static OrganizationDao instance = null;
    public static OrganizationDao getInstance() {
        if (instance == null) {
            instance = new OrganizationDao();
        }
        return instance;
    }

    private OrganizationCacheEntity getCacheEntity(String idOrNo) {
        OrganizationCacheEntity org = CacheHelper.getInstance().getOrganization(idOrNo);
        if (org == null) {
            return null;
        }
        if (org.table == null) {
            return null;
        }
        return org;
    }

    private OrganizationTable getFromCache(String idOrNo) {
        OrganizationCacheEntity org = CacheHelper.getInstance().getOrganization(idOrNo);
        if (org == null)
            return null;
        if (org.table == null) {
            return null;
        }
        return org.table;
    }

    private void updateCache(OrganizationTable table) {
        try {
            if (table==null) {
                return;
            }
            OrganizationCacheEntity org = CacheHelper.getInstance().getOrganization(table.no);
            if (org == null) {
                org = new OrganizationCacheEntity();
                IdNoCacheEntity idno = new IdNoCacheEntity();
                idno.id = table.organizationId;
                idno.no = table.organizationId;
                org.id = idno;
            }
            org.table = table;
            CacheHelper.getInstance().addOrganization(org);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
    }


    /// <summary>
    /// 更新信息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(OrganizationTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 组织ID
            entity.Add("organizationId", table.organizationId);
            // 业务编号
            entity.Add("no", table.no);
            // 组织名称
            entity.Add("Name", table.name);
            // 所属组织ID
            if (table.parentId == null || "".Equals(table.parentId.Trim())) {
                table.parentId = "-1";
            }
            entity.Add("parentId", table.parentId);
            // 组织LogoId
            entity.Add("logoStorageRecordId", table.logoStorageRecordId);
            // 简介
            entity.Add("introduction", table.introduction);
            // 负责人
            entity.Add("leader", table.leader);
            // 电话
            entity.Add("telephone", table.telephone);
            // 传真
            entity.Add("fax", table.fax);
            // 地址
            entity.Add("address", table.address);
            // 邮编
            entity.Add("postcode", table.postcode);
            // 虚拟组织标记
            entity.Add("virtual", table.@virtual);
            // 排序字段
            entity.Add("sortNum", table.sortNum);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.name)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.name);
                fristPinyin = PinyinHelper.getFristPinyin(table.name);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);
            entity.Add("tenantNo", table.tenantNo);
            if (this.isExist("organization", "organizationId", table.organizationId)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("organizationId",table.organizationId)
                };
                count = this._mgr.Update("organization", entity, "organizationId=@organizationId", param);
            } else {
                count = this._mgr.Save("organization", entity);
            }

            this.updateCache(table);

        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return count;

    }

    /// <summary>
    /// 查找
    /// </summary>
    /// <returns></returns>
    public List<OrganizationTable> FindAllOrganizationFromDB(string conditionCol, object conditionVal) {
        List<OrganizationTable> list = null;
        try {


            list = new List<OrganizationTable>();
            DataTable entity = this._mgr.QueryDt("organization", conditionCol, conditionVal);
            list = DataUtils.DataTableToModelList<OrganizationTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return list;
    }

    public OrganizationTable FindAllOrganizationFromDBByTentant(String tenant,string parent) {
        OrganizationTable table = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("TenantNo", tenant);
            entity.Add("ParentId", parent);
            DataRow dataRow = this._mgr.QueryOne("organization", entity);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<OrganizationTable>(dataRow.Table);
        } catch (Exception e) {

            Log.Error(typeof(OrganizationDao), e);
        }
        return table;
    }
    /// <summary>
    /// 查找
    /// </summary>
    /// <returns></returns>
    public List<OrganizationTable> FindAllOrganization(string conditionCol, object conditionVal) {
        List<OrganizationTable> list = null;
        try {
            List<OrganizationCacheEntity> orgs = CacheHelper.getInstance().getAllOrgs();
            if (orgs != null && orgs.Count > 0) {
                list = new List<OrganizationTable>();
                foreach (OrganizationCacheEntity cache in orgs) {
                    list.Add(cache.table);
                }
                return list;
            }

            list = new List<OrganizationTable>();
            DataTable entity = this._mgr.QueryDt("organization", conditionCol, conditionVal);
            list = DataUtils.DataTableToModelList<OrganizationTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return list;
    }

    /// <summary>
    /// 插入组织列表
    /// </summary>
    /// <param Name="dt"></param>
    public int InsertOrganization(List<OrganizationTable> beanList) {

        int count = -1;

        try {
            //插入组织的信息
            List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();
            for (int i = 0; i < beanList.Count; i++) {

                OrganizationTable bean = beanList[i];
                if (bean.deleted == true) {
                    this.deleteById(bean.organizationId);
                    //    CacheHelper.getInstance().removeOrganization(bean.organizationId);
                } else {
                    this.save(bean);
                }




                //插入数据库群的信息
                //this._mgr.Delete("organization", "organizationId=@organization_id", new SQLiteParameter[] {
                //    new SQLiteParameter("organization_id", bean.id)
                //});

                //Dictionary<string, object> entity = new Dictionary<string, object>();
                //entity.Add("organizationId", bean.organizationId);
                //entity.Add("no", bean.no);
                //entity.Add("Name", bean.Name);
                //entity.Add("parentId", bean.parentId);
                //entity.Add("logoStorageRecordId", bean.logoStorageRecordId);
                //entity.Add("introduction", bean.introduction);
                //entity.Add("leader", bean.leader);
                //entity.Add("telephone", bean.telephone);
                //entity.Add("fax", bean.fax);
                //entity.Add("address", bean.address);
                //entity.Add("virtual", bean.@virtual);
                //entity.Add("sortNum", bean.sortNum);
                //entitys.Add(entity);

            }
            //count=   this._mgr.Save("organization", entitys);
        } catch (Exception e) {
            count = -1;
            Log.Error(typeof(OrganizationDao), e);
        }
        return count;

    }

    /// <summary>
    /// 根据组织编号查找组织
    /// </summary>
    /// <param Name="id"></param>
    /// <returns></returns>
    public OrganizationTable FindOrganizationByOrgId(int id) {
        OrganizationTable table = null;
        try {

            OrganizationCacheEntity orgCache = this.getCacheEntity(id.ToStr());

            if (orgCache != null) {
                return orgCache.table;
            }

            DataRow dr = this._mgr.QueryOne("organization", "OrganizationId", id);
            table = DataUtils.DataRowToModel<OrganizationTable>(dr);
            this.updateCache(table);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return table;
    }

    /// <summary>
    /// 通过id删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteById(String OrganizationId) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("OrganizationId",OrganizationId)
            };
            count = this._mgr.Delete("organization", "OrganizationId=@OrganizationId", param);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        if (count > 0) {
            CacheHelper.getInstance().removeOrganization(OrganizationId);
        }

        //_mgr.CommitTrans();
        return count;

    }


    public int DeleteByTenantNo(String tenantNo) {
        int count = -1;
        List<OrganizationTable> list = FindAllOrganization("tenantNo", tenantNo);
        try {
            String sql = string.Empty;

            sql = "delete from  organization where  tenantNo = '" + tenantNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);

            if (count > 0 && list != null) {
                for (int i = 0; i < list.Count; i++) {
                    CacheHelper.getInstance().removeOrganization(list[i].organizationId);
                }
                // 通知通讯录画面更新
                BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
                Businessdata.eventDataType = BusinessEventDataType.OrgChangedEvent;
                EventBusHelper.getInstance().fireEvent(Businessdata);
            }
            CacheHelper.getInstance().loadAllOrganization();
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return count;

    }

    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;
            sql = "delete from  organization where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return count;

    }
    /// <summary>
    /// 查找
    /// </summary>
    /// <returns></returns>
    public List<OrganizationTable> FindChildOrganization(int organization_id) {
        List<OrganizationTable> list = null;
        try {
            List<OrganizationCacheEntity> orgs = CacheHelper.getInstance().getAllOrgs();
            if (orgs != null && orgs.Count > 0) {
                list = new List<OrganizationTable>();
                foreach (OrganizationCacheEntity cache in orgs) {
                    if (organization_id.ToStr().Equals(cache.table.parentId)) {
                        list.Add(cache.table);
                    }
                }
                return list;
            }
            list = new List<OrganizationTable>();
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("parentId", organization_id);

            Dictionary<string, object> orders = new Dictionary<string, object>();
            orders.Add("sortNum", "asc");

            DataTable dtDataTable = this._mgr.QueryDt("organization", entity, orders);
            list = DataUtils.DataTableToModelList<OrganizationTable>(dtDataTable);
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
        return list;
    }


    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<OrganizationTable> list = null;
            String sql = "select * from organization where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<OrganizationTable>(dt);
            if (list != null) {
                foreach (OrganizationTable table in list) {
                    this.save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(OrganizationDao), e);
        }
    }
}
}
