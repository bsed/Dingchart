using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.DataSqlite {

class TimestampDao : BaseDao {

    private static TimestampDao instance = null;
    public static TimestampDao getInstance() {
        if (instance == null) {
            instance = new TimestampDao();
        }
        return instance;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param Name="dt"></param>
    public int save(TimestampTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("type", table.type);
            entity.Add("Timestamp", table.timestamp);
            entity.Add("tenantNo", table.tenantNo);

            Dictionary<string, object> entityExist = new Dictionary<string, object>();
            entityExist.Add("type", table.type);
            entityExist.Add("tenantNo", table.tenantNo);


            if (this.isExist("Timestamp", entityExist)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("type",table.type),
                    new SQLiteParameter("tenantNo",table.tenantNo)
                };
                count = this._mgr.Update("Timestamp", entity, "type=@type  and  tenantNo=@tenantNo", param);
            } else {
                count = this._mgr.Save("Timestamp", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过会话对象的NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public TimestampTable findByType(string type,string tenantNo) {
        TimestampTable table = null;
        try {
            table = new TimestampTable();
            Dictionary<string, object> entity = new Dictionary<string, object>();
            if(tenantNo!=string.Empty) {
                entity.Add("tenantNo", tenantNo);
            }

            entity.Add("type", type);
            DataRow dataRow = this._mgr.QueryOne("Timestamp", entity);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<TimestampTable>(dataRow.Table);
        } catch (Exception e) {
            Log.Error(typeof(TimestampDao), e);
        }
        return table;
    }
}
}
