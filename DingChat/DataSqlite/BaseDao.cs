using cn.lds.chatcore.pcw.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.DataSqlite {
public class BaseDao {
    public SQLiteHelper _mgr = SQLiteHelper.getInstance();

    /// <summary>
    /// 判断数据是否存在
    /// </summary>
    /// <param Name="tableName">表名</param>
    /// <param Name="conditionCol">字段名</param>
    /// <param Name="conditionVal">字段值</param>
    /// <returns></returns>
    public Boolean isExist(String tableName,String conditionCol, Object conditionVal) {
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add(conditionCol, conditionVal);
            if (this._mgr.count(tableName, entity) > 0) {
                return true;
            } else {
                return false;
            }
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return false;
    }

    /// <summary>
    /// 判断数据是否存在
    /// </summary>
    /// <param Name="tableName">表名</param>
    /// <param Name="entity">条件</param>
    /// <returns></returns>
    public Boolean isExist(String tableName, Dictionary<string, object> entity) {
        try {
            if (this._mgr.count(tableName, entity) > 0) {
                return true;
            } else {
                return false;
            }
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return false;
    }
}
}
