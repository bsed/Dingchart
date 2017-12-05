using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using cn.lds.chatcore.pcw.Services;

namespace cn.lds.chatcore.pcw.DataSqlite {
public class TodoTaskDao : BaseDao {
    private static TodoTaskDao instance = null;
    public static TodoTaskDao getInstance() {
        if (instance == null) {
            instance = new TodoTaskDao();
        }
        return instance;
    }

    public int DeleteByTenantNo(String tenantNo) {
        String sql = string.Empty;
        int count = -1;
        try {
            sql = "delete from  todo_task where  tenantNo = '" + tenantNo + "'";

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

            sql = "delete from  todo_task where  tenantNo not in  (" + tenantNoList + ")";
            count = this._mgr.ExecuteNonQuery(sql, null);

        } catch (Exception e) {
            Log.Error(typeof(PublicWebDao), e);
        }
        return count;

    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param Name="dt"></param>
    public int save(TodoTaskTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("todoTaskId", table.todoTaskId);
            entity.Add("createdBy", table.createdBy);
            entity.Add("createdDate", table.createdDate);
            entity.Add("lastModifiedBy", table.lastModifiedBy);
            entity.Add("lastModifiedDate", table.lastModifiedDate);
            entity.Add("userNo", table.userNo);
            entity.Add("appId", table.appId);
            entity.Add("type", table.type);
            entity.Add("content", table.content);
            entity.Add("status", table.status);
            entity.Add("detailUrl", table.detailUrl);
            entity.Add("appLogoId", table.appLogoId);
            entity.Add("appName", table.appName);
            entity.Add("tenantNo", table.tenantNo);
            if (this.isExist("todo_task", "todoTaskId", table.todoTaskId)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("todoTaskId",table.todoTaskId)
                };
                count = this._mgr.Update("todo_task", entity, "todoTaskId=@todoTaskId", param);
            } else {
                count = this._mgr.Save("todo_task", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(TodoTaskDao), e);
        }
        return count;

    }

    /// <summary>
    /// 统计待办数
    /// </summary>
    /// <returns></returns>
    public int countOfTodoTaskPending() {
        try {
            // 查询条件
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("status", TodoTaskStatusType.pending.ToStr());
            return this._mgr.count("todo_task", entity);
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskDao), e);
            return 0;
        }
    }

    /// <summary>
    /// 通过待办ID获取待办数据
    /// </summary>
    /// <param Name="todoTaskId"></param>
    /// <returns></returns>
    public TodoTaskTable getTodoTaskTable(int todoTaskId, string tenantNo) {
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("todoTaskId", todoTaskId);
            entity.Add("tenantNo", tenantNo);
            DataRow dataRow = this._mgr.QueryOne("todo_task", entity);
            if (dataRow == null) return null;
            TodoTaskTable table = DataUtils.DataTableToModel<TodoTaskTable>(dataRow.Table);
            return table;
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskDao), e);
            return null;
        }
    }

    /// <summary>
    /// 根据应用状态查询
    /// </summary>
    /// <param Name="status"></param>
    /// <returns></returns>
    public List<TodoTaskTable> getTodoTaskTablesByStatus(TodoTaskStatusType status) {
        List<TodoTaskTable> list = new List<TodoTaskTable>();
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT t.* from todo_task  t");
            sb.Append(" where status = '" + status.ToStr() + "' ");
            sb.Append(" order by CreatedDate desc ");
            DataTable dtTable = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<TodoTaskTable>(dtTable);

        } catch (Exception e) {
            Log.Error(typeof(TodoTaskDao), e);
        }
        return list;
    }

    /// <summary>
    /// 根据应用状态分页查询
    /// </summary>
    /// <param Name="status"></param>
    /// <returns></returns>
    public List<TodoTaskTable> getTodoTaskTablesByStatusPager(TodoTaskStatusType status, long lastTimestamp) {
        List<TodoTaskTable> list = new List<TodoTaskTable>();
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * from todo_task ");
            sb.Append(" where status = '" + status.ToStr() + "' ");
            if (lastTimestamp>0) {
                sb.Append("  and CreatedDate < '" + lastTimestamp + "' ");
            }
            sb.Append(" order by CreatedDate desc ");
            sb.Append("  LIMIT " + Constants.SYS_CONFIG_MESSAGE_NUM_PERPAGE);
            DataTable dtTable = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<TodoTaskTable>(dtTable);

        } catch (Exception e) {
            Log.Error(typeof(TodoTaskDao), e);
        }
        return list;
    }


    /// <summary>
    /// 通过todoTaskId删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByTodoTaskId(String todoTaskId, string tenantNo) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("todoTaskId",todoTaskId),
                new SQLiteParameter("tenantNo",tenantNo)
            };
            count = this._mgr.Delete("todo_task", "todoTaskId=@todoTaskId", param);
        } catch (Exception e) {
            Log.Error(typeof(TodoTaskDao), e);
        }
        return count;

    }
}
}
