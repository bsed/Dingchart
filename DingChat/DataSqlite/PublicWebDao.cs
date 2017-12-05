using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Business.Cache;

namespace cn.lds.chatcore.pcw.DataSqlite {
public class PublicWebDao : BaseDao {

    private static PublicWebDao _instance = null;
    public static PublicWebDao getInstance() {
        if (_instance == null) {
            _instance = new PublicWebDao();
        }
        return _instance;
    }

    private AppCacheEntity getCacheEntity(String idOrNo) {
        AppCacheEntity sub = CacheHelper.getInstance().getApp(idOrNo);
        if (sub == null) {
            return null;
        }
        if (sub.table == null) {
            return null;
        }
        return sub;
    }

    private PublicWebTable getFromCache(String idOrNo) {
        AppCacheEntity sub = CacheHelper.getInstance().getApp(idOrNo);
        if (sub == null)
            return null;
        if (sub.table == null) {
            return null;
        }
        return sub.table;
    }

    private void updateCache(PublicWebTable table) {
        try {
            AppCacheEntity sub = CacheHelper.getInstance().getApp(table.appId);
            if (sub == null) {
                sub = new AppCacheEntity();
                IdNoCacheEntity idno = new IdNoCacheEntity();
                idno.id = table.appId;
                idno.no = table.appId;
                sub.id = idno;
            }
            sub.table = table;
            CacheHelper.getInstance().addApp(sub);
        } catch (Exception e) {
            Log.Error(typeof(PublicAccountsDao), e);
        }
    }

    /// <summary>
    /// 更新信息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(PublicWebTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("appId", table.appId);
            entity.Add("userNo", table.userNo);
            entity.Add("Name", table.name);
            entity.Add("introduction", table.introduction);
            entity.Add("logoId", table.logoId);
            entity.Add("status", table.status);
            entity.Add("includeSubscription", table.includeSubscription);
            entity.Add("includeWebsite", table.includeWebsite);
            entity.Add("clientType", table.clientType);
            entity.Add("includeMobileApp", table.includeMobileApp);
            entity.Add("androidAppOpenUrl", table.androidAppOpenUrl);
            entity.Add("androidDownloadUrl", table.androidDownloadUrl);
            entity.Add("mobileAppParameters", table.mobileAppParameters);
            entity.Add("followed", table.followed);
            entity.Add("sort", table.sort);
            entity.Add("appSortIndex", table.appSortIndex);
            entity.Add("appClassificationId", table.appClassificationId);
            entity.Add("appclaasificationKey", table.appclaasificationKey);
            entity.Add("appClassificationName", table.appClassificationName);
            entity.Add("url", table.url);
            entity.Add("ownerName", table.ownerName);
            entity.Add("websiteStatus", table.websiteStatus);
            entity.Add("enableTopmost", table.enableTopmost);
            entity.Add("allowReceiveMessages", table.allowReceiveMessages);
            entity.Add("allowShareMyLocation", table.allowShareMyLocation);
            entity.Add("commonWebsite", table.commonWebsite);
            entity.Add("commonWebsiteTime", table.commonWebsiteTime);
            entity.Add("includeComponent", table.includeComponent);
            entity.Add("componentPhoneUrl", table.componentPhoneUrl);
            entity.Add("tenantNo", table.tenantNo);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.name)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.name);
                fristPinyin = PinyinHelper.getFristPinyin(table.name);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);

            Dictionary<string, object> entityExist = new Dictionary<string, object>();
            entityExist.Add("appId", table.appId);
            entityExist.Add("tenantNo", table.tenantNo);

            if (this.isExist("public_web", entityExist)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("appId",table.appId),
                    new SQLiteParameter("tenantNo",table.tenantNo)
                };
                count = this._mgr.Update("public_web", entity, "appId=@appId and tenantNo=@tenantNo", param);
            } else {
                count = this._mgr.Save("public_web", entity);
            }
            // TODO:临时注释掉缓存处理机制
            this.updateCache(table);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;
    }

    /// <summary>
    /// 删除全部数据
    /// </summary>
    /// <returns></returns>
    public int DeleteAll() {
        int count = -1;
        try {
            count = this._mgr.Delete("public_web", null);
            // TODO:临时注释掉缓存处理机制
            //CacheHelper.getInstance().clearApps();
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }

        return count;
    }

    public int DeleteByTenantNo(String tenantNo) {
        int count = -1;
        try {
            String sql = string.Empty;
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 构建查询条件
            entity.Add("tenantNo", tenantNo);
            List<PublicWebTable> tables = FindAll(entity, null);

            sql = "delete from  public_web where  tenantNo = '" + tenantNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);

            foreach (PublicWebTable dt in tables) {
                CacheHelper.getInstance().removeApp(dt.appId);
            }

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }
    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  public_web where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }
    /// <summary>
    /// 根据指定条件查找
    /// </summary>
    /// <returns></returns>
    public List<PublicWebTable> FindAllFromDb(Dictionary<string, object> entity, Dictionary<string, object> orders) {

        List<PublicWebTable> list = null;
        try {
            list = new List<PublicWebTable>();
            DataTable dtTable = this._mgr.QueryDt("public_web", entity, orders);
            list = DataUtils.DataTableToModelList<PublicWebTable>(dtTable);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return list;
    }

    private List<PublicWebTable> getAllFromCache(Dictionary<string, object> entity, Dictionary<string, object> orders) {
        Object thirdAppClassIdConditionObj = null;
        Object commonWebsiteConditionObj = null;
        Object websiteStatusConditionObj = null;
        if (entity.ContainsKey("appClassificationId")) {
            thirdAppClassIdConditionObj = entity["appClassificationId"];
        }
        if (entity.ContainsKey("commonWebsite")) {
            commonWebsiteConditionObj = entity["commonWebsite"];
        }
        if (entity.ContainsKey("websiteStatus")) {
            websiteStatusConditionObj = entity["websiteStatus"];
        }

        List<PublicWebTable> list = null;
        try {
            // TODO:临时注释掉缓存处理机制
            List<AppCacheEntity> subs = CacheHelper.getInstance().getAllApps();
            if (subs != null && subs.Count > 0) {
                list = new List<PublicWebTable>();
                foreach (AppCacheEntity sub in subs) {
                    bool jdg = true;
                    if (thirdAppClassIdConditionObj != null) {
                        int classId = (int)thirdAppClassIdConditionObj;
                        if (!classId.ToStr().Equals(sub.table.appClassificationId)) {
                            jdg = false;
                        }
                    }

                    if (commonWebsiteConditionObj != null) {
                        int commonWebsite = (int)commonWebsiteConditionObj;
                        if (!commonWebsite.ToStr().Equals(sub.table.commonWebsite)) {
                            jdg = false;
                        }
                    }

                    if (websiteStatusConditionObj != null) {
                        string commonWebsite = (string)websiteStatusConditionObj;
                        if (!commonWebsite.ToStr().Equals(sub.table.websiteStatus)) {
                            jdg = false;
                        }
                    }
                    if (jdg) {
                        list.Add(sub.table);
                    }
                }
                //开始进行排序
                IEnumerable<PublicWebTable> query = list.AsEnumerable<PublicWebTable>();
                IOrderedEnumerable<PublicWebTable> order = null;
                if (orders.ContainsKey("sort")) {
                    String sortType = orders["sort"].ToStr();
                    if (order == null) {
                        if (sortType.ToLower().Equals("asc")) {
                            order = query.OrderBy(value => value.sort);
                        } else {
                            order = query.OrderByDescending(value => value.sort);
                        }
                    } else {
                        if (sortType.ToLower().Equals("asc")) {
                            order = order.ThenBy(value => value.sort);
                        } else {
                            order = order.ThenByDescending(value => value.sort);
                        }
                    }

                }

                if (orders.ContainsKey("websiteStatus")) {
                    String sortType = orders["websiteStatus"].ToStr();
                    if (order == null) {
                        if (sortType.ToLower().Equals("asc")) {
                            order = query.OrderBy(value => value.websiteStatus);
                        } else {
                            order = query.OrderByDescending(value => value.websiteStatus);
                        }
                    } else {
                        if (sortType.ToLower().Equals("asc")) {
                            order = order.ThenBy(value => value.websiteStatus);
                        } else {
                            order = order.ThenByDescending(value => value.websiteStatus);
                        }
                    }
                }

                if (orders.ContainsKey("commonWebsiteTime")) {
                    String sortType = orders["commonWebsiteTime"].ToStr();
                    if (order == null) {
                        if (sortType.ToLower().Equals("asc")) {
                            order = query.OrderBy(value => value.commonWebsiteTime);
                        } else {
                            order = query.OrderByDescending(value => value.commonWebsiteTime);
                        }
                    } else {
                        if (sortType.ToLower().Equals("asc")) {
                            order = order.ThenBy(value => value.commonWebsiteTime);
                        } else {
                            order = order.ThenByDescending(value => value.commonWebsiteTime);
                        }
                    }
                }

                if (orders.ContainsKey("appSortIndex")) {
                    String sortType = orders["appSortIndex"].ToStr();
                    if (order == null) {
                        if (sortType.ToLower().Equals("asc")) {
                            order = query.OrderBy(value => value.appSortIndex);
                        } else {
                            order = query.OrderByDescending(value => value.appSortIndex);
                        }
                    } else {
                        if (sortType.ToLower().Equals("asc")) {
                            order = order.ThenBy(value => value.appSortIndex);
                        } else {
                            order = order.ThenByDescending(value => value.appSortIndex);
                        }
                    }
                }

                if (orders.ContainsKey("Name")) {
                    String sortType = orders["Name"].ToStr();
                    if (order == null) {
                        if (sortType.ToLower().Equals("asc")) {
                            order = query.OrderBy(value => value.name);
                        } else {
                            order = query.OrderByDescending(value => value.name);
                        }
                    } else {
                        if (sortType.ToLower().Equals("asc")) {
                            order = order.ThenBy(value => value.name);
                        } else {
                            order = order.ThenByDescending(value => value.name);
                        }
                    }
                }

                list = query.ToList<PublicWebTable>();
                //.OrderBy(a => a.websiteStatus);

                //query = from items in list orderby items.websiteStatus ascending select items;

                return list;
            }
            return null;

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return list;
    }

    /// <summary>
    /// 根据指定条件查找
    /// </summary>
    /// <returns></returns>
    public List<PublicWebTable> FindAll(Dictionary<string, object> entity, Dictionary<string, object> orders) {

        List<PublicWebTable> list = null;
        try {
            // TODO:临时注释掉缓存处理机制
            //list = this.getAllFromCache(entity, orders);
            //if (list != null && list.Count > 0) {
            //    return list;
            //}

            list = new List<PublicWebTable>();
            DataTable dtTable = this._mgr.QueryDt("public_web", entity, orders);
            list = DataUtils.DataTableToModelList<PublicWebTable>(dtTable);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return list;
    }

    ///// <summary>
    ///// 根据指定条件查找第一个
    ///// </summary>
    ///// <returns></returns>
    //public PublicWebTable FindFirst(Dictionary<string, object> entity, Dictionary<string, object> orders) {
    //    PublicWebTable table = null;
    //    try {
    //        table = new PublicWebTable();
    //        DataRow dataRow = this._mgr.QueryFirst("public_web", entity, orders);
    //        if (entity == null) return null;
    //        table = DataUtils.DataTableToModel<PublicWebTable>(dataRow.Table);
    //    } catch (Exception e) {
    //        Log.Error(typeof(PublicWebDao), e);
    //    }
    //    return table;
    //}

    /// <summary>
    /// 通过id删除
    /// </summary>
    /// <param Name="dt"></param>
    public int DeleteByAppId(String appId) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("appId",appId)
            };
            count = this._mgr.Delete("public_web", "appId=@appId", param);
            // TODO:临时注释掉缓存处理机制
            CacheHelper.getInstance().removeApp(appId);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过ID查找
    /// </summary>
    /// <param Name="appid"></param>
    /// <returns></returns>
    public PublicWebTable FindByAppId(String appId) {
        PublicWebTable table = null;
        try {
            table = this.getFromCache(appId);
            if (table != null) {
                return table;
            }

            table = new PublicWebTable();
            DataRow entity = this._mgr.QueryOne("public_web", "appId", appId);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<PublicWebTable>(entity.Table);
            // TODO:临时注释掉缓存处理机制
            this.updateCache(table);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return table;
    }

    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<PublicWebTable> list = null;
            String sql = "select * from public_web where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<PublicWebTable>(dt);
            if (list != null) {
                foreach (PublicWebTable table in list) {
                    this.save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
    }


    public List<PublicWebTable> findPublicWeb(string tenantNo) {
        List<PublicWebTable> list = null;
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT  *");
            sb.Append(" FROM public_web AS c ");
            sb.Append(" WHERE c.status='1' ");
            sb.Append("  and c.AppclaasificationKey  is not null ");
            sb.Append("  and c.tenantNo = '" + tenantNo + "' ");
            sb.Append(" ORDER BY c.appSortIndex asc, c.totalPinyin asc");
            list = new List<PublicWebTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<PublicWebTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return list;
    }
}
}
