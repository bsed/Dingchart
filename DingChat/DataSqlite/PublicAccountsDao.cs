using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Business.Cache;

namespace cn.lds.chatcore.pcw.DataSqlite {
class PublicAccountsDao : BaseDao {
    private static PublicAccountsDao instance = null;
    public static PublicAccountsDao getInstance() {
        if (instance == null) {
            instance = new PublicAccountsDao();
        }
        return instance;
    }

    private SubscribtionCacheEntity getCacheEntity(String idOrNo) {
        SubscribtionCacheEntity sub = CacheHelper.getInstance().getSubscription(idOrNo);
        if (sub == null) {
            return null;
        }
        if (sub.table == null) {
            return null;
        }
        return sub;
    }

    private PublicAccountsTable getFromCache(String idOrNo) {
        SubscribtionCacheEntity sub = CacheHelper.getInstance().getSubscription(idOrNo);
        if (sub == null)
            return null;
        if (sub.table == null) {
            return null;
        }
        return sub.table;
    }

    private void updateCache(PublicAccountsTable table) {
        try {
            SubscribtionCacheEntity sub = CacheHelper.getInstance().getSubscription(table.appid);
            if (sub == null) {
                sub = new SubscribtionCacheEntity();
                IdNoCacheEntity idno = new IdNoCacheEntity();
                //idno.id = table.id.ToString();
                idno.id = table.appid;
                idno.no = table.appid;
                sub.id = idno;
            }
            sub.table = table;
            CacheHelper.getInstance().addSubscription(sub);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
    }


    /// <summary>
    /// 保存
    /// </summary>
    /// <param Name="dt"></param>
    public int save(PublicAccountsTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("appid", table.appid);
            entity.Add("userno", table.userno);
            entity.Add("Name", table.name);
            entity.Add("ownerName", table.ownerName);
            entity.Add("logoId", table.logoId);
            entity.Add("introduction", table.introduction);
            entity.Add("status", table.status);
            entity.Add("location", table.location);
            entity.Add("menu", table.menu);
            entity.Add("tenantNo", table.tenantNo);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.name)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.name);
                fristPinyin = PinyinHelper.getFristPinyin(table.name);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);

            if (this.isExist("public_accounts", "appid", table.appid)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("appid",table.appid)
                };
                count = this._mgr.Update("public_accounts", entity, "appid=@appid", param);
            } else {
                count = this._mgr.Save("public_accounts", entity);
            }
            this.updateCache(table);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        return count;

    }

    /// <summary>
    /// 查找所有
    /// </summary>
    /// <returns></returns>
    public List<PublicAccountsTable> FindAll(String strWhere) {
        List<PublicAccountsTable> table = null;

        try {
            //List<SubscribtionCacheEntity> subs = CacheHelper.getInstance().getAllSubscribtions();
            //if (subs != null && subs.Count > 0) {
            //    table = new List<PublicAccountsTable>();
            //    foreach (SubscribtionCacheEntity sub in subs) {
            //        table.Add(sub.table);
            //    }
            //    return table;
            //}

            String sql = "select * from public_accounts where status=1 " + strWhere + " order by totalPinyin asc";

            DataTable dt = this._mgr.ExecuteRow(sql, null);
            table = DataUtils.DataTableToModelList<PublicAccountsTable>(dt);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        return table;
    }


    /// <summary>
    /// 查找所有
    /// </summary>
    /// <returns></returns>
    public List<PublicAccountsTable> FindAllFromDB(String strWhere) {
        List<PublicAccountsTable> table = null;

        try {
            String sql = "select * from public_accounts where 1=1 " + strWhere + " order by totalPinyin asc";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            table = DataUtils.DataTableToModelList<PublicAccountsTable>(dt);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        return table;
    }



    /// <summary>
    /// 通过appid查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public PublicAccountsTable findByAppid(String appid) {

        PublicAccountsTable table = null;

        try {
            table = this.getFromCache(appid);
            if (table != null) {
                return table;
            }
            table = new PublicAccountsTable();
            DataRow entity = this._mgr.QueryOne("public_accounts", "appid", appid);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<PublicAccountsTable>(entity.Table);

            this.updateCache(table);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        return table;
    }



    /// <summary>
    /// 通过appId删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByAppId(String appId) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("appId",appId)
            };
            count = this._mgr.Delete("public_accounts", "appId=@appId", param);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        CacheHelper.getInstance().removeSubscription(appId);
        return count;

    }
    public int DeleteByTenantNo(String tenantNo) {
        int count = -1;

        List<PublicAccountsTable> list =FindAllFromDB("and tenantNo = '" + tenantNo + "'");
        try {
            String sql = string.Empty;

            sql = "delete from  public_accounts where  tenantNo = '" + tenantNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        if(count>0&& list!=null) {
            for(int i=0; i<list.Count; i++) {
                CacheHelper.getInstance().removeSubscription(list[i].appid);
            }
        }
        return count;

    }

    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  public_accounts where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
        return count;

    }
    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<PublicAccountsTable> list = null;
            String sql = "select * from public_accounts where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<PublicAccountsTable>(dt);
            if (list != null) {
                foreach (PublicAccountsTable table in list) {
                    this.save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
    }
}
}
