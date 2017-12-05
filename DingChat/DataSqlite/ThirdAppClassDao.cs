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
class ThirdAppClassDao : BaseDao {
    private static ThirdAppClassDao instance = null;
    public static ThirdAppClassDao getInstance() {
        if (instance == null) {
            instance = new ThirdAppClassDao();
        }
        return instance;
    }


    public int DeleteByTenantNo(String tenantNo) {
        String sql = string.Empty;
        int count = -1;
        try {
            sql = "delete from  third_app_class where  tenantNo = '" + tenantNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
        return count;
    }
    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  third_app_class where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
        return count;

    }
    /// <summary>
    /// 更新
    /// </summary>
    /// <param Name="dt"></param>
    public int Save(ThirdAppClassTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("thirdAppClassId", table.thirdAppClassId);
            entity.Add("Name", table.name);
            entity.Add("iconId", table.iconId);
            entity.Add("sortNum", table.sortNum);
            entity.Add("tenantNo", table.tenantNo);
            entity.Add("thirdAppGroupId", table.thirdAppGroupId);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.name)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.name);
                fristPinyin = PinyinHelper.getFristPinyin(table.name);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);
            Dictionary<string, object> entityExist = new Dictionary<string, object>();
            entityExist.Add("thirdAppClassId", table.thirdAppClassId);
            entityExist.Add("tenantNo", table.tenantNo);
            if (this.isExist("third_app_class", entityExist)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("thirdAppClassId",table.thirdAppClassId),
                    new SQLiteParameter("tenantNo",table.tenantNo)
                };
                count = this._mgr.Update("third_app_class", entity, "thirdAppClassId=@thirdAppClassId  and tenantNo=@tenantNo", param);
            } else {
                count = this._mgr.Save("third_app_class", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
        return count;

    }

    /// <summary>
    /// 查询全部（通过分组）
    /// </summary>
    /// <returns></returns>
    public List<ThirdAppClassTable> FindAllThirdAppClassByGroupId(String thirdAppGroupId,string tenantNo) {
        List<ThirdAppClassTable> list = null;
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM third_app_class ");
            sb.Append(" WHERE 1=1");
            sb.Append(" AND thirdAppGroupId='" + thirdAppGroupId + "'");
            sb.Append(" AND tenantNo='" + tenantNo + "'");
            sb.Append(" ORDER BY sortNum ASC");
            list = new List<ThirdAppClassTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<ThirdAppClassTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
        return list;
    }

    /// <summary>
    /// 查询全部
    /// </summary>
    /// <returns></returns>
    public List<ThirdAppClassTable> FindAllThirdAppClass(string tenantNo) {
        List<ThirdAppClassTable> list = null;
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM third_app_class ");
            sb.Append(" WHERE 1=1 ");
            sb.Append(" AND tenantNo='" + tenantNo + "'");
            sb.Append(" ORDER BY sortNum ASC");
            list = new List<ThirdAppClassTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<ThirdAppClassTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
        return list;
    }

    /// <summary>
    /// 通过ID查找
    /// </summary>
    /// <param Name="thirdAppGroupId"></param>
    /// <returns></returns>
    public ThirdAppClassTable FindThirdAppClassById(String thirdAppClassId) {
        ThirdAppClassTable table = null;
        try {
            table = new ThirdAppClassTable();
            DataRow entity = this._mgr.QueryOne("third_app_class", "thirdAppClassId", thirdAppClassId);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<ThirdAppClassTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
        return table;
    }

    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<ThirdAppClassTable> list = null;
            String sql = "select * from third_app_class where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<ThirdAppClassTable>(dt);
            if (list != null) {
                foreach (ThirdAppClassTable table in list) {
                    this.Save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(ThirdAppClassDao), e);
        }
    }
}
}
