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
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Models.Tables;


namespace cn.lds.chatcore.pcw.DataSqlite {
class ChatSessionDao : BaseDao {
    private static ChatSessionDao instance = null;
    public static ChatSessionDao getInstance() {
        if (instance == null) {
            instance = new ChatSessionDao();
        }
        return instance;
    }
    private Object lockObject = new Object();
    /// <summary>
    /// 插入会话
    /// </summary>
    /// <param Name="dt"></param>
    public int save(ChatSessionTable table) {
        int count = -1;
        lock (lockObject) {
            try {
                Dictionary<string, object> entity = new Dictionary<string, object>();
                // 插入表
                entity.Add("Account", table.account);
                entity.Add("chattype", table.chatType);
                entity.Add("user", table.user);
                entity.Add("resource", table.resource);
                if (table.lastMessage != null && table.lastMessage != "") {
                    table.lastMessage = table.lastMessage.Replace("\r\n", "");
                    table.lastMessage = table.lastMessage.Replace("\n", "");
                }
                entity.Add("lastmessage", table.lastMessage);
                entity.Add("Timestamp", table.timestamp);
                entity.Add("Atme", table.atme);
                entity.Add("Chatdraft", table.chatdraft);
                entity.Add("messageid", table.messageId);
                entity.Add("unReadMessageCount", table.unReadMessageCount);
                if (this.isExist("chat_session", "user", table.user)) {
                    SQLiteParameter[] param = new SQLiteParameter[] {
                        new SQLiteParameter("user", table.user)
                    };
                    count = this._mgr.Update("chat_session", entity, "user=@user", param);
                } else {
                    count = this._mgr.Save("chat_session", entity);
                }

            } catch (Exception e) {
                Log.Error(typeof (ChatSessionDao), e);
            }
        }
        return count;

    }

    /// <summary>
    /// 根据传入的条件进行查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<ChatSessionTable> findAll() {
        List<ChatSessionTable> list = null;
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT t.* from (");
            sb.Append(" SELECT ");
            sb.Append("  c.Account ");
            sb.Append("  ,c.chattype ");
            sb.Append("  ,c.user ");
            sb.Append("  ,c.resource ");
            sb.Append("  ,c.lastmessage ");
            sb.Append("  ,c.Timestamp ");
            sb.Append("  ,c.Atme ");
            sb.Append("  ,c.Chatdraft ");
            sb.Append("  ,s.Top ");
            sb.Append("  ,c.Timestamp ");
            sb.Append("  ,c.unReadMessageCount ");
            sb.Append(" FROM chat_session AS c ");
            sb.Append(" LEFT OUTER JOIN setting AS s");
            sb.Append(" ON  c.user = s.no");
            sb.Append(" WHERE ");
            sb.Append(" 1=1 ");
            sb.Append("  and c.user  not like '" + Constants.PUBLIC_ACCOUNT_FLAG + "%' ");
            // 如果不强制公众号会话置顶，则需要查询公众号最新一条消息并排序
            if (!Constants.SYS_CONFIG_IS_PUBLIC_TOP) {
                sb.Append(" UNION ");
                // 公众号
                sb.Append(" SELECT ");
                sb.Append("  c.Account ");
                sb.Append("  ,c.chattype ");
                sb.Append("  ,'" + Constants.PUBLIC_ACCOUNT_FLAG+ "' as user");
                sb.Append("  ,c.resource ");
                sb.Append("  ,c.lastmessage ");
                sb.Append("  ,c.Timestamp ");
                sb.Append("  ,c.Atme ");
                sb.Append("  ,c.Chatdraft ");
                sb.Append("  ,0 AS Top ");
                sb.Append("  ,c.Timestamp ");
                sb.Append("  ,c.unReadMessageCount ");
                sb.Append(" FROM chat_session AS c ");
                sb.Append(" LEFT OUTER JOIN setting AS s");
                sb.Append(" ON  c.user = s.no");
                sb.Append(" WHERE ");
                ChatSessionTable table = this.getLastChatSessionByChatType(Constants.PUBLIC_ACCOUNT_FLAG);
                if (table != null) {
                    sb.Append(" c.user = '" + table.user + "' ");
                } else {
                    sb.Append(" c.user = '' ");
                }

            }
            sb.Append(" ) AS t ");

            sb.Append(" ORDER BY ( case when t.Top = 1 then 1 else 0 end) DESC, t.Timestamp DESC");

            list = new List<ChatSessionTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(),null);
            list = DataUtils.DataTableToModelList<ChatSessionTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return list;
    }

    /// <summary>
    /// 查询公众号的会话
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<ChatSessionTable> findPublic(bool top) {
        List<ChatSessionTable> list = null;
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT ");
            sb.Append("  c.chattype ");
            sb.Append("  ,c.user ");
            sb.Append("  ,c.resource ");
            sb.Append("  ,c.lastmessage ");
            sb.Append("  ,c.Timestamp ");
            sb.Append("  ,c.Atme ");
            sb.Append("  ,c.Chatdraft ");
            sb.Append(" FROM chat_session AS c ");
            sb.Append(" LEFT OUTER JOIN setting AS s");
            sb.Append(" ON c.user = s.no");
            sb.Append(" WHERE 1=1");
            if(top) {
                sb.Append("  and s.Top=1 ");
            } else {
                sb.Append("  and (s.Top=0  or  s.Top is null)");
            }
            sb.Append("  and c.user like '" + Constants.PUBLIC_ACCOUNT_FLAG + "%' ");
            sb.Append(" ORDER BY ( case when s.Top = 1 then 1 else 0 end) DESC, c.Timestamp DESC");

            list = new List<ChatSessionTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<ChatSessionTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return list;
    }

    /// <summary>
    /// 获取某中聊天类别中，最近的一条会话记录的NO
    /// TODO:这个SQL可以改造，直接只查询一条
    /// </summary>
    /// <param Name="chat_type_flag"></param>
    /// <returns></returns>
    public ChatSessionTable getLastChatSessionByChatType(String chat_type_flag) {
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * from chat_session ");
            sb.Append(" WHERE user like '" + chat_type_flag + "%' ");
            sb.Append("  order by Timestamp desc ");
            DataTable ds = this._mgr.ExecuteRow(sb.ToStr(),null);
            if (ds.Rows.Count > 0) {
                ChatSessionTable table = DataUtils.DataTableToModel<ChatSessionTable>(ds.Rows[0].Table);
                return table;
            }

        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return null;
    }

    /// <summary>
    /// 通过会话对象的NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public ChatSessionTable findByNo(String no) {
        ChatSessionTable table = null;
        try {
            table = new ChatSessionTable();
            DataRow entity = this._mgr.QueryOne("chat_session", "user", no);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<ChatSessionTable>(entity.Table);
        } catch (Exception e) {

            Log.Error(typeof(ChatSessionDao), e);
        }
        return table;
    }

    /// <summary>
    /// 通过会话对象的NO删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByNo(String no) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("no",no)
            };
            count = this._mgr.Delete("chat_session", "user=@no", param);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        //_mgr.CommitTrans();
        return count;

    }

    /// <summary>
    /// 清空会话
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteAll() {
        int count = -1;
        try {
            count = this._mgr.Delete("chat_session",null, null);
        } catch (Exception e) {
            Log.Error(typeof(ChatSessionDao), e);
        }
        return count;

    }


    /// <summary>
    /// 重置会话未读消息数
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int ReSetUnReadMessageCountByUserNo(String userNo) {
        int count = -1;
        try {

            String sql = "update chat_session set unReadMessageCount = 0 where user = '" + userNo + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {

            Log.Error(typeof(ChatSessionDao), e);
        }
        return count;
    }
}
}
