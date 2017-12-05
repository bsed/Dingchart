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
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common.Enums;

namespace cn.lds.chatcore.pcw.DataSqlite {
class MasterDao : BaseDao {
    private static MasterDao instance = null;
    public static MasterDao getInstance() {
        if (instance == null) {
            instance = new MasterDao();
        }
        return instance;
    }
    public int DeleteByTenantNo(String tenantNo) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  master where  tenantNo = '" + tenantNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof (PublicWebDao), e);
        }
        return count;

    }

    public int DeleteNotInTenantNo(String tenantNoList) {
        int count = -1;
        try {
            String sql = string.Empty;

            sql = "delete from  master where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }
    /// <summary>
    /// 根据码表类型和键查找显示的文本
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public String getTextByTypeAndKey(MasterType masterType, String strKey, string tenantNo) {
        String text = "";
        try {
            MasterTable table = findByTypeAndParentKey(masterType, strKey, tenantNo);
            if (table != null) {
                text = table.text;
            } else {
                text = string.Empty;
            }
        } catch (Exception e) {
            Log.Error(typeof(MasterDao), e);
        }
        return text;
    }

    /// <summary>
    /// 根据码表类型和键查找码表
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public MasterTable findByTypeAndParentKey(MasterType masterType, String strKey, string tenantNo) {
        MasterTable table = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("mastertype", masterType.ToStr());
            entity.Add("tenantNo", tenantNo);
            entity.Add("key", strKey);
            DataRow dataRow = this._mgr.QueryOne("master", entity);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<MasterTable>(dataRow.Table);
        } catch (Exception e) {
            Log.Error(typeof(MasterDao), e);
        }
        return table;
    }

    /// <summary>
    /// 根据码表类型、父键查找码表列表
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    //public List<MasterTable> findByTypeAndParentKey(MasterType masterType, String strParentKey, String strTreeLevel) {
    //    List<MasterTable> list = null;
    //    try {
    //        list = new List<MasterTable>();
    //        Dictionary<string, object> entity = new Dictionary<string, object>();
    //        entity.Add("mastertype", masterType.ToStr());
    //        entity.Add("parentkey", strParentKey);
    //        entity.Add("treelevel", strTreeLevel);
    //        DataTable dtTable = this._mgr.QueryDt("master", entity,null);
    //        list = DataUtils.DataTableToModelList<MasterTable>(dtTable);
    //    } catch (Exception e) {
    //        Log.Error(typeof(MasterDao), e);
    //    }
    //    return list;
    //}


    //public string GetKeyValue(string key) {
    //    MasterTable table = null;
    //    try {
    //        DataRow entity = this._mgr.QueryOne("master", "Key", key);
    //        if (entity == null)
    //            return "";
    //        else {
    //            table = DataUtils.DataTableToModel<MasterTable>(entity.Table);
    //            if (table != null) {
    //                return table.text;
    //            } else {
    //                return "";
    //            }

    //        }
    //    } catch (Exception e) {
    //        Log.Error(typeof(ContactsDao), e);
    //        return null;
    //    }
    //}

    public int Insert(List<MasterTable> beanList, string mastertype) {
        int count = -1;
        try {
            List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();
            for (int i = 0; i < beanList.Count; i++) {
                MasterTable bean = beanList[i];
                _mgr.Delete("master", "key=@key  and tenantNo=@tenantNo", new SQLiteParameter[] {
                    new SQLiteParameter("key", bean.key),
                    new SQLiteParameter("tenantNo", bean.tenantNo)
                });

                Dictionary<string, object> entity = new Dictionary<string, object>();
                entity.Add("mastertype", mastertype);
                entity.Add("key", bean.key);
                entity.Add("value", bean.value);
                entity.Add("text", bean.text);
                entity.Add("parentKey", bean.parentKey);
                entity.Add("sortOrder", bean.sortOrder);
                entity.Add("description",bean.description);
                entity.Add("tenantNo", bean.tenantNo);
                entitys.Add(entity);
                //this._mgr.Save("master", entity);
            }
            count = this._mgr.Save("master", entitys);
        } catch (Exception e) {
            Log.Error(typeof(MasterDao), e);
        }

        return count;
    }

}
}
