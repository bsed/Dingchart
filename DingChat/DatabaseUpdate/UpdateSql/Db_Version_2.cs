using cn.lds.chatcore.pcw.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Common.DbHelper;

namespace cn.lds.chatcore.pcw.DatabaseUpdate.UpdateSql {
/// <summary>
/// 升级原因：修改表message中的content字段为text类型
/// </summary>
public class Db_Version_2 {
    public static String GetUpdateSql() {
        String sql = "";
        try {
            // 第1步：构建改表SQL
            sql += GetSQL_Step1();
            // 第2步：构建建表SQL
            sql += GetSQL_Step2();
            // 第3步：构建将旧表插入新表的SQL
            sql += GetSQL_Step3();
            // 第4步：删除掉旧表
            sql += GetSQL_Step4();
            return sql;
        } catch (Exception ex) {

        }
        return sql;
    }

    /// <summary>
    /// 第1步：构建改表SQL
    /// </summary>
    /// <returns></returns>
    private static String GetSQL_Step1() {

        try {
            return "ALTER TABLE messages RENAME TO messages_old;";
        } catch (Exception ex) {
            Log.Error(typeof(Db_Version_2), ex);
        }
        return "";
    }

    /// <summary>
    /// 第2步：构建建表SQL
    /// </summary>
    /// <returns></returns>
    private static String GetSQL_Step2() {
        String sql = "";
        try {
            DbManager dbManager = new DbManager();
            List<String> entityClassList = dbManager.GetClassesByEntityAttribute(Constants.ENTITY_CLASS_NAMESPACES);

            foreach (String classFullName in entityClassList) {
                // 只处理消息表
                if (classFullName.EndsWith("MessagesTable")) {
                    List<String> alterSqlList = null;
                    EntityAttribute entityAttribute = Type.GetType(classFullName).GetCustomAttribute<EntityAttribute>();
                    String tableName = entityAttribute.TableName;
                    DataTable dt = new DataTable(tableName);
                    dbManager.GetPropertyByColumnAttribute(classFullName, dt);
                    dbManager.GetPropertyByPrimaryKeyAttribute(classFullName, dt);
                    List<DataColumn> indexColumnList = dbManager.GetPropertyByIndexAttribute(classFullName, dt);
                    List<String> indexSQLList = dbManager.CreateIndex(dt, indexColumnList);
                    sql += dbManager.GetCreateTableSQL(dt)+";";
                    foreach (String indexSQl in indexSQLList) {
                        sql += indexSQl + ";";
                    }
                    break;
                }

            }
        } catch (Exception ex) {
            Log.Error(typeof(Db_Version_2), ex);
            sql = "";
        }
        return sql;
    }

    /// <summary>
    /// 第3步：构建将旧表插入新表的SQL
    /// </summary>
    /// <returns></returns>
    private static String GetSQL_Step3() {
        String sql = "";
        try {
            sql = "INSERT INTO messages (messageId,serverMessageId,user,resource,text,content,Timestamp,delayTimestamp,type,incoming,read,flag,sent,error,Atme,showTimestamp) SELECT messageId,serverMessageId,user,resource,text,content,Timestamp,delayTimestamp,type,incoming,read,flag,sent,error,Atme,showTimestamp FROM messages_old;";
        } catch (Exception ex) {
            Log.Error(typeof(Db_Version_2), ex);
            sql = "";
        }
        return sql;
    }

    /// <summary>
    /// 第4步：删除掉旧表
    /// </summary>
    /// <returns></returns>
    private static String GetSQL_Step4() {
        String sql = "";
        try {
            sql = "DROP TABLE messages_old";
        } catch (Exception ex) {
            Log.Error(typeof(Db_Version_2), ex);
            sql = "";
        }
        return sql;
    }
}
}
