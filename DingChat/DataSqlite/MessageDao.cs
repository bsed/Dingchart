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
using System.Threading;
using cn.lds.chatcore.pcw.imtp.message;

namespace cn.lds.chatcore.pcw.DataSqlite {
class MessageDao : BaseDao {
    private static MessageDao instance = null;

    public static MessageDao getInstance() {
        if (instance == null) {
            instance = new MessageDao();
        }
        return instance;
    }


    private void saveToDb(object paramData) {
        MessagesTable table = (MessagesTable)paramData;
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表

            entity.Add("messageId", table.messageId);
            //entity.Add("mucId", table.mucId);
            entity.Add("serverMessageId", table.serverMessageId);
            entity.Add("user", table.user);
            entity.Add("resource", table.resource);
            entity.Add("text", table.text);
            entity.Add("content", table.content);
            entity.Add("Timestamp", table.timestamp);
            entity.Add("delayTimestamp", table.delayTimestamp);
            entity.Add("type", table.type);
            entity.Add("incoming", table.incoming);
            entity.Add("read", table.read);
            entity.Add("flag", table.flag);
            entity.Add("sent", table.sent);
            entity.Add("error", table.error);
            entity.Add("Atme", table.atme);
            entity.Add("showTimestamp", table.showTimestamp);
            entity.Add("createTime", table.createTime);
            entity.Add("updateTime", table.updateTime);
            entity.Add("tenantNo", table.tenantNo);
            if (this.isExist("messages", "messageId", table.messageId)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("id",table.id)
                };
                count = this._mgr.Update("messages", entity, "messageId=@messageId", param);
            } else {
                count = this._mgr.Save("messages", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
    }

    /// <summary>
    /// 保存消息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(MessagesTable table) {
        this.saveToDb(table);
        //ThreadPool.QueueUserWorkItem(saveToDb, table);
        return 1;
    }


    /// <summary>
    /// 根据本地生成的消息ID查找消息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public MessagesTable findByMessageId(String messageId) {
        MessagesTable table = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("messageId", messageId);
            DataRow dataRow = this._mgr.QueryOne("messages", entity);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<MessagesTable>(dataRow.Table);
        } catch (Exception e) {

            Log.Error(typeof(MessageDao), e);
        }
        return table;
    }

    /// <summary>
    /// 根据服务器返回的消息ID查找消息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public MessagesTable findByServerMessageId(String serverMessageId) {
        MessagesTable table = null;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("serverMessageId", serverMessageId);
            DataRow dataRow = this._mgr.QueryOne("messages", entity);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<MessagesTable>(dataRow.Table);
        } catch (Exception e) {

            Log.Error(typeof(MessageDao), e);
        }
        return table;
    }

    /// <summary>
    /// 获取某会话的第一条未读消息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public MessagesTable getFirstUnreadMessagesByNo(String user) {
        MessagesTable table = null;
        try {
            // 查询条件
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("user", user);
            entity.Add("read", false);
            // 排序
            Dictionary<string, object> orders = new Dictionary<string, object>();
            orders.Add("Timestamp", "asc");
            DataRow dataRow = this._mgr.QueryFirst("messages", entity, orders);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<MessagesTable>(dataRow.Table);
        } catch (Exception e) {

            Log.Error(typeof(MessageDao), e);
        }
        return table;
    }

    /// <summary>
    /// 获取某会话的第一条消息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public MessagesTable getFirstMessagesByNo(String user) {
        MessagesTable table = null;
        try {
            // 查询条件
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("user", user);
            // 排序
            Dictionary<string, object> orders = new Dictionary<string, object>();
            orders.Add("Timestamp", "desc");
            DataRow dataRow = this._mgr.QueryFirst("messages", entity, orders);
            if (dataRow == null) return null;
            table = DataUtils.DataTableToModel<MessagesTable>(dataRow.Table);
        } catch (Exception e) {

            Log.Error(typeof(MessageDao), e);
        }
        return table;
    }

    /// <summary>
    /// 设置某人的消息为已读
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int setMessageReadByUserNo(String userNo) {
        int count = -1;
        try {

            String sql = "update messages set read = 1 where user like '" + userNo + "%'";
            count = this._mgr.ExecuteNonQuery(sql,null);
        } catch (Exception e) {

            Log.Error(typeof(MessageDao), e);
        }
        return count;
    }

    /// <summary>
    /// 获取未读的消息数，如果不输入user的话，代表获取所有的未读消息数
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public int countOfUnreadMessages(String user) {

        if(user==string.Empty) {
            StringBuilder sb = new StringBuilder();
            sb.Append("  and read = 0");
            sb.Append("  and user not like '" + Constants.PUBLIC_ACCOUNT_FLAG + "%'");
            sb.Append("  and Type not like '" + MsgType.Notify + "%'");
            return this._mgr.count("messages", sb.ToStr());
        } else {
            // 查询条件
            Dictionary<string, object> entity = new Dictionary<string, object>();
            if (user != null) {
                entity.Add("user", user);
            }
            entity.Add("read", false);

            return this._mgr.count("messages", entity);
        }



    }



    /// <summary>
    /// 获取未读的公众号消息数，如果不输入user的话，代表获取所有的未读消息数
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public int countOfPublicUnreadMessages(String user) {
        // 查询条件
        Dictionary<string, object> entity = new Dictionary<string, object>();
        if (user != null) {
            entity.Add("user", user);
            entity.Add("read", false);
            return this._mgr.count("messages", entity);
        } else {
            StringBuilder sb = new StringBuilder();
            sb.Append("  and read = 0");
            sb.Append("  and user like '" + Constants.PUBLIC_ACCOUNT_FLAG + "%'");
            return this._mgr.count("messages", sb.ToStr());
        }

    }

    /// <summary>
    /// 获取未读的应用消息数
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public int countOfAppUnreadMessages() {
        // 查询条件
        StringBuilder sb = new StringBuilder();
        sb.Append("  and read = 0");
        sb.Append("  and user like '" + Constants.APPMSG_FLAG + "%'");
        return this._mgr.count("messages", sb.ToStr());

    }

    ///// <summary>
    ///// 设置某人的某一条消息为已读
    ///// </summary>
    ///// <param Name="table"></param>
    ///// <returns></returns>
    //public int setMessageReadById(String userNo,String id) {
    //    int count = -1;
    //    try {
    //        String sql = "update messages set read = 1 where user = '" + userNo + "' and id=" + id;
    //        count = this._mgr.ExecuteNonQuery(sql, null);
    //    } catch (Exception e) {

    //        Log.Error(typeof(MessageDao), e);
    //    }
    //    return count;
    //}

    ///// <summary>
    ///// 设置语音/视频消息为已播放状态
    ///// </summary>
    ///// <param Name="table"></param>
    ///// <returns></returns>
    //public int setVedioOrVoicePlayedById(String messageId) {
    //    int count = -1;
    //    try {
    //        String sql = "update messages set flag = 1 where messageId = '" + messageId + "'";
    //        count = this._mgr.ExecuteNonQuery(sql, null);
    //    } catch (Exception e) {

    //        Log.Error(typeof(MessageDao), e);
    //    }
    //    return count;
    //}


    /// <summary>
    /// 分页查询消息
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<MessagesTable> findMessagesByPage(String user, long lastTimestamp) {
        List<MessagesTable> list = new List<MessagesTable>();
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            if (user.Equals(Constants.APPMSG_FLAG)) {
                sb.Append("  WHERE user like '" + user + "%'");
            } else {
                sb.Append("  WHERE user = '" + user + "'");
            }
            sb.Append("  and Timestamp < '" + lastTimestamp + "' ");
            sb.Append("  ORDER BY Timestamp DESC ");
            sb.Append("  LIMIT " + Constants.SYS_CONFIG_MESSAGE_NUM_PERPAGE);


            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
        return list;
    }

    /// <summary>
    /// 查询所有未读消息
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<MessagesTable> findAllUnreadMessage(String user, long lastTimestamp, long targetTimestamp) {
        List<MessagesTable> list = new List<MessagesTable>();
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            if (user.Equals(Constants.APPMSG_FLAG)) {
                sb.Append("  WHERE user like '" + user + "%'");
            } else {
                sb.Append("  WHERE user = '" + user + "'");
            }
            sb.Append("  and Timestamp < '" + lastTimestamp + "' and Timestamp >= '" + targetTimestamp + "'");
            sb.Append("  ORDER BY Timestamp DESC ");
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
        return list;
    }

    /// <summary>
    /// TODO 这个是简单粗暴的临时写法。将来考虑如何优化，一次全查出来有点豪放啊
    /// TODO 将来做会话的图片查看也有用
    /// 查询某个会话的全部图片消息
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<MessagesTable> findAllImageMessagesByUser(String user) {
        List<MessagesTable> list = new List<MessagesTable>();
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            sb.Append("  WHERE user = '" + user + "'");
            sb.Append("  and type = '" + MsgType.Image.ToStr() + "'");
            sb.Append("  ORDER BY Timestamp DESC ");
            //sb.Append("  LIMIT " + Constants.SYS_CONFIG_MESSAGE_NUM_PERPAGE);
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
        return list;
    }

    /// <summary>
    /// 查找最后一条消息（通知消息除外）
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public MessagesTable findLastMessagesByUser(String user) {
        List<MessagesTable> list = new List<MessagesTable>();
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            sb.Append("  WHERE user = '" + user + "'");
            sb.Append("  and type <> '" + MsgType.Notify.ToStr() + "'");
            sb.Append("  ORDER BY Timestamp DESC  ");
            sb.Append("  LIMIT " + 1);
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);

            if (list.Count > 0) {
                return list[0];
            } else {
                return null;
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
            return null;
        }


    }
    /// <summary>
    /// 查找最后一条应用 消息
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public MessagesTable findLastAppMessages() {
        List<MessagesTable> list = new List<MessagesTable>();
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            sb.Append("  WHERE type = '" + MsgType.App.ToStr() + "'");
            sb.Append("  ORDER BY Timestamp DESC  ");
            sb.Append("  LIMIT " + 1);
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);

            if (list.Count > 0) {
                return list[0];
            } else {
                return null;
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
            return null;
        }


    }
    /// <summary>
    /// 查找最后一条@ 消息
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public MessagesTable findLastAtMessagesByUser(String user) {
        List<MessagesTable> list = new List<MessagesTable>();
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            sb.Append("  WHERE user = '" + user + "'");
            sb.Append("  and type = '" + MsgType.At.ToStr() + "'");
            sb.Append("  ORDER BY Timestamp DESC  ");
            sb.Append("  LIMIT " + 1);
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);

            if(list.Count>0) {
                return list[0];
            } else {
                return null;
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
            return null;
        }


    }

    /// <summary>
    /// 通过会话对象的NO删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByUser(String user) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("user",user)
            };
            count = this._mgr.Delete("messages", "user=@user", param);
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过messageId删除
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteByMessageId(String messageId) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("messageId",messageId)
            };
            count = this._mgr.Delete("messages", "messageId=@messageId", param);
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
        return count;

    }

    /// <summary>
    /// 刚登陆系统时，把所有的消息状态是发送中的改为发送失败
    /// </summary>
    /// <param Name="dt"></param>
    public int setAllSentFlagError() {
        // TODO:this._mgr.Update("messages", entity, "sent=@sent", param);居然不好用
        //int count = -1;
        //try {
        //    SQLiteParameter[] param = new SQLiteParameter[] {
        //        new SQLiteParameter("sent","0")
        //    };
        //    Dictionary<string, object> entity = new Dictionary<string, object>();
        //    entity.Add("sent","-1");
        //    count = this._mgr.Update("messages", entity, "sent=@sent", param);
        //} catch (Exception e) {
        //    Log.Error(typeof(MessageDao), e);
        //}
        //return count;

        int count = -1;
        try {

            String sql = "update messages set sent = '-1' where sent = '0'";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {

            Log.Error(typeof(MessageDao), e);
        }
        return count;

    }

    /// <summary>
    /// TODO 这个是简单粗暴的临时写法。将来考虑如何优化，一次全查出来有点豪放啊，例如翻页什么的
    /// 查询某个会话的全部聊天文件（包括图片、文件、视频）
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<ChatSessionFilesBean> findAllFilesByUser(String user) {
        List<ChatSessionFilesBean> list = new List<ChatSessionFilesBean>();
        try {

            StringBuilder sb = new StringBuilder();
            sb.Append(" select m.*, f.*  ");
            sb.Append(" from messages m  ");
            sb.Append(" left join files f on m.MessageId = f.Owner  ");
            sb.Append(" where m.User = '" + user + "'  ");
            sb.Append(" and m.Type in ('" + MsgType.Image.ToStr() + "' ,'" + MsgType.File.ToStr() + "','" + MsgType.Video.ToStr() + "')");
            sb.Append(" and f.FileStorageId is not null ");
            sb.Append("  order by m.Timestamp desc ");
            //sb.Append("  LIMIT " + Constants.SYS_CONFIG_MESSAGE_NUM_PERPAGE);
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<ChatSessionFilesBean>(entity);
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
        }
        return list;
    }

    /// <summary>
    /// 根据roomid查找最后一条语音聊天或视频聊天消息
    /// </summary>
    /// <param Name="user"></param>
    /// <returns></returns>
    public MessagesTable findLastAVMeetingInviteMessagesByRoomId(String roomId) {
        List<MessagesTable> list = new List<MessagesTable>();
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM messages ");
            sb.Append("  WHERE content like '%" + roomId + "%'");
            sb.Append("  and type = '" + MsgType.AVMeetingInvite.ToStr() + "'");
            sb.Append("  ORDER BY Timestamp DESC  ");
            sb.Append("  LIMIT " + 1);
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<MessagesTable>(entity);

            if (list.Count > 0) {
                return list[0];
            } else {
                return null;
            }
        } catch (Exception e) {
            Log.Error(typeof(MessageDao), e);
            return null;
        }


    }
}
}
