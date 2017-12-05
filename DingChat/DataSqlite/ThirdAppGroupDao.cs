using cn.lds.chatcore.pcw.Business;
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

namespace cn.lds.chatcore.pcw.DataSqlite {
class ThirdAppGroupDao  : BaseDao {
    private static ThirdAppGroupDao instance = null;
    public static ThirdAppGroupDao getInstance() {
        if (instance == null) {
            instance = new ThirdAppGroupDao();
        }
        return instance;
    }


    /// <summary>
    /// 更新
    /// </summary>
    /// <param Name="dt"></param>
    public int Save(ThirdAppGroupTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("thirdAppGroupId", table.thirdAppGroupId);
            entity.Add("Name", table.name);
            entity.Add("iconId", table.iconId);
            entity.Add("sortNum", table.sortNum);
            entity.Add("tenantNo", table.tenantNo);
            entity.Add("key", table.key);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.name)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.name);
                fristPinyin = PinyinHelper.getFristPinyin(table.name);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);
            Dictionary<string, object> entityExist = new Dictionary<string, object>();
            entityExist.Add("thirdAppGroupId", table.thirdAppGroupId);
            entityExist.Add("key", table.key);
            entityExist.Add("tenantNo", table.tenantNo);

            if (this.isExist("third_app_group", entityExist)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("thirdAppGroupId",table.thirdAppGroupId),
                    new SQLiteParameter("tenantNo",table.tenantNo)
                };
                if(table.key==null) {
                    count = this._mgr.Update("third_app_group", entity, "thirdAppGroupId=@thirdAppGroupId and tenantNo=@tenantNo and key is Null", param);
                } else {
                    count = this._mgr.Update("third_app_group", entity, "thirdAppGroupId=@thirdAppGroupId and tenantNo=@tenantNo and key is not Null", param);
                }

            } else {
                count = this._mgr.Save("third_app_group", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
        return count;

    }
    public int DeleteByBase() {
        String sql = string.Empty;
        int count = -1;
        try {
            sql = "delete from  third_app_group where key is not NULL ";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
        return count;
    }

    public int DeleteByTenantNo(String tenantNo) {
        String sql = string.Empty;
        int count = -1;
        try {
            sql = "delete from  third_app_group where key is NULL  AND tenantNo = '" + tenantNo + "'";

            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
        return count;
    }

    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  third_app_group where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }
    /// <summary>
    /// 查询全部
    /// </summary>
    /// <returns></returns>
    public List<ThirdAppGroupTable> FindAll(string tenantNo) {
        List<ThirdAppGroupTable> list = null;
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM third_app_group ");
            sb.Append(" WHERE 1=1");
            sb.Append(" AND tenantNo='" + tenantNo + "'");
            sb.Append(" ORDER BY sortNum ASC");
            list = new List<ThirdAppGroupTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<ThirdAppGroupTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
        return list;
    }

    /// <summary>
    /// 通过ID查找
    /// </summary>
    /// <param Name="thirdAppGroupId"></param>
    /// <returns></returns>
    public ThirdAppGroupTable FindByThirdAppGroupId(String thirdAppGroupId) {
        ThirdAppGroupTable table = null;
        try {
            table = new ThirdAppGroupTable();
            DataRow entity = this._mgr.QueryOne("third_app_group", "thirdAppGroupId", thirdAppGroupId);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<ThirdAppGroupTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
        return table;
    }

    public ThirdAppGroupTable FindByThirdAppKey(String key) {
        ThirdAppGroupTable table = null;
        try {
            table = new ThirdAppGroupTable();
            DataRow entity = this._mgr.QueryOne("third_app_group", "Key", key);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<ThirdAppGroupTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
        return table;
    }
    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<ThirdAppGroupTable> list = null;
            String sql = "select * from third_app_group where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<ThirdAppGroupTable>(dt);
            if (list != null) {
                foreach (ThirdAppGroupTable table in list) {
                    this.Save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppGroupDao), e);
        }
    }
}
}
