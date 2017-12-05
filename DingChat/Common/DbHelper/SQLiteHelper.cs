using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common {
public class SQLiteHelper {

    private static SQLiteHelper instance = null;

    private static Object lockObject = new Object();

    public static SQLiteHelper getInstance() {
        if (instance == null) {
            instance = new SQLiteHelper();
        }
        return instance;
    }

    /// <summary> 数据库事务对象
    /// </summary>
    //private IDbTransaction _sqlTrans = null;

    private bool _showSql = false;

    //private bool _isInTrans = false;

    /// <summary>
    /// 是否输出生成的SQL语句
    /// </summary>
    public bool ShowSql {
        get {
            return this._showSql;
        } set {
            this._showSql = value;
        }
    }



    private SQLiteConnection _conn;

    private SQLiteHelper() {
        //if (_dataFile == null)
        //    throw new ArgumentNullException("dataFile=null");

    }

    /// <summary>
    /// <para>打开SQLiteManager使用的数据库连接</para>
    /// </summary>
    private void Open() {
        if (_conn == null) {
            this._conn = OpenConnection();
        }
    }

    private void Close() {
        if (this._conn != null) {
            this._conn.Close();
        }
    }


    /// <summary>
    /// <para>安静地关闭连接,保存不抛出任何异常</para>
    /// </summary>
    public void CloseQuietly() {
        if (this._conn != null) {
            try {
                this._conn.Close();
            } catch (Exception e) {
                Log.Error(typeof(SQLiteHelper), e);
            }
        }
    }

    /// <summary>
    /// <para>创建一个连接到指定数据文件的SQLiteConnection,并Open</para>
    /// <para>如果文件不存在,创建之</para>
    /// </summary>
    /// <param Name="dataFile"></param>
    /// <returns></returns>
    public static SQLiteConnection OpenConnection() {


        if (App.DataBasePath == null) {
            return null;
        }
        //throw new ArgumentNullException("dataFile=null");

        if (!File.Exists(App.DataBasePath)) {
            SQLiteConnection.CreateFile(App.DataBasePath);
        }

        SQLiteConnection conn = new SQLiteConnection();

        SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();
        conStr.SyncMode = SynchronizationModes.Normal;
        conStr.JournalMode = SQLiteJournalModeEnum.Wal;
        conStr.Pooling = true;
        conStr.CacheSize = 8000;
        conStr.Version = 3;
        conStr.DataSource = App.DataBasePath;
        conn.ConnectionString = conStr.ToString();

        conn.Open();
        return conn;
    }

    /// <summary>
    /// <para>读取或设置SQLiteManager使用的数据库连接</para>
    /// </summary>
    public SQLiteConnection Connection {
        get {
            return this._conn;
        } set {
            if (value == null) {
                throw new ArgumentNullException();
            }
            this._conn = value;
        }
    }

    protected void EnsureConnection() {
        this.Open();
        if (this._conn == null) {
            throw new LdException("SQLiteManager.Connection=null");
        }
    }

    public string GetDataFile() {
        return App.DataBasePath;
    }


    /// <summary>
    /// <para>判断表table是否存在</para>
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public DataTable getTable(string tableName) {
        DataTable schemaTable = null;
        if (tableName == null)
            throw new ArgumentNullException("table=null");
        lock (lockObject) {
            try {
                this.EnsureConnection();
                string sql = "select * from " + tableName + " LIMIT 1";
                schemaTable = this.ExecuteRow(sql, null);
                schemaTable.TableName = tableName;
            } catch (Exception e) {
                Log.Error(typeof(SQLiteHelper), e);

            }
        }
        return schemaTable;
    }

    /// <summary>
    /// <para>判断表table是否存在</para>
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public bool TableExists(string table) {
        if (table == null)
            throw new ArgumentNullException("table=null");
        this.EnsureConnection();
        // SELECT count(*) FROM sqlite_master WHERE type='table' AND Name='test';
        SQLiteCommand cmd = new SQLiteCommand("SELECT count(*) as c FROM sqlite_master WHERE type='table' AND Name=@tableName ");
        cmd.Connection = this.Connection;
        cmd.Parameters.Add(new SQLiteParameter("tableName", table));
        SQLiteDataReader reader = cmd.ExecuteReader();
        reader.Read();
        int c = reader.GetInt32(0);
        reader.Close();
        reader.Dispose();
        cmd.Dispose();
        //return false;
        return c == 1;
    }

    /// <summary>
    /// <para>执行SQL,返回受影响的行数</para>
    /// <para>可用于执行表创建语句</para>
    /// <para>paramArr == null 表示无参数</para>
    /// </summary>
    /// <param Name="sql"></param>
    /// <returns></returns>
    public int ExecuteNonQuery(string sql, SQLiteParameter[] paramArr) {

        try {
            if (sql == null) {
                throw new ArgumentNullException("sql=null");
            }
            this.EnsureConnection();

            if (this.ShowSql) {
                Console.WriteLine("SQL: " + sql);
            }

            int c = -1;

            lock (lockObject) {
                //SQLiteCommand cmd = new SQLiteCommand();

                //using (SQLiteConnection conn = new SQLiteConnection(this.Connection))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(this.Connection)) {
                        cmd.CommandText = sql;
                        if (paramArr != null) {
                            foreach (SQLiteParameter p in paramArr) {
                                cmd.Parameters.Add(p);
                            }
                        }

                        c = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                //SQLiteCommand cmd = new SQLiteCommand(this.Connection);
                //{
                //    cmd.CommandText = sql;
                //    if (paramArr != null)
                //    {
                //        foreach (SQLiteParameter p in paramArr)
                //        {
                //            cmd.Parameters.Add(p);
                //        }
                //    }

                //    c = cmd.ExecuteNonQuery();
                //    cmd.Dispose();
                //}

            }
            return c;
        } catch (Exception e) {
            Log.Error(typeof(SQLiteHelper), e);
        }
        return 0;
    }

    /// <summary>
    /// <para>执行SQL,返回SQLiteDataReader</para>
    /// <para>返回的Reader为原始状态,须自行调用Read()方法</para>
    /// <para>paramArr=null,则表示无参数</para>
    /// </summary>
    /// <param Name="sql"></param>
    /// <param Name="paramArr"></param>
    /// <returns></returns>
    public SQLiteDataReader ExecuteReader(string sql, SQLiteParameter[] paramArr) {
        return (SQLiteDataReader)ExecuteReader(sql, paramArr, (ReaderWrapper)null);
    }

    /// <summary>
    /// <para>执行SQL,如果readerWrapper!=null,那么将调用readerWrapper对SQLiteDataReader进行包装,并返回结果</para>
    /// </summary>
    /// <param Name="sql"></param>
    /// <param Name="paramArr">null 表示无参数</param>
    /// <param Name="readerWrapper">null 直接返回SQLiteDataReader</param>
    /// <returns></returns>
    public object ExecuteReader(string sql, SQLiteParameter[] paramArr, ReaderWrapper readerWrapper) {
        try {
            if (sql == null) {
                throw new ArgumentNullException("sql=null");
            }
            if (this.ShowSql) {
                Console.WriteLine("SQL: " + sql);
            }

            this.EnsureConnection();

            object result = null;

            lock (lockObject) {

                SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection);
                if (paramArr != null) {
                    foreach (SQLiteParameter p in paramArr) {
                        cmd.Parameters.Add(p);
                    }
                }
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (readerWrapper != null) {
                    result = readerWrapper(reader);
                } else {
                    result = reader;
                }
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
            }
            return result;
        } catch (Exception e) {
            Log.Error(typeof(SQLiteHelper), e);
        }
        return null;
    }

    /// <summary>
    /// <para>执行SQL,返回结果集,使用RowWrapper对每一行进行包装</para>
    /// <para>如果结果集为空,那么返回空List (List.Count=0)</para>
    /// <para>rowWrapper = null时,使用WrapRowToDictionary</para>
    /// </summary>
    /// <param Name="sql"></param>
    /// <param Name="paramArr"></param>
    /// <param Name="rowWrapper"></param>
    /// <returns></returns>
    public DataTable ExecuteRow(string sql, SQLiteParameter[] paramArr) {
        try {
            if (sql == null) {
                throw new ArgumentNullException("sql=null");
            }
            if (this.ShowSql) {
                Console.WriteLine("SQL: " + sql);
            }
            this.EnsureConnection();

            lock (lockObject) {
                using (SQLiteConnection conn = new SQLiteConnection(this.Connection)) {
                    using (SQLiteCommand command = new SQLiteCommand()) {
                        DataSet ds = new DataSet();
                        PrepareCommand(command, conn, sql, paramArr);
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(SQLiteHelper), e);
        }
        return null;
    }

    private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, params object[] p) {
        if (conn.State != ConnectionState.Open)
            conn.Open();
        cmd.Parameters.Clear();
        cmd.Connection = conn;
        cmd.CommandText = cmdText;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 30;
        if (p != null) {
            foreach (SQLiteParameter parm in p) {
                cmd.Parameters.Add(parm);
            }
        }
    }
    public static object WrapRowToDictionary(int rowNum, SQLiteDataReader reader) {
        int fc = reader.FieldCount;
        Dictionary<string, object> row = new Dictionary<string, object>();
        for (int i = 0; i < fc; i++) {
            string fieldName = reader.GetName(i);
            object value = reader.GetValue(i);
            row.Add(fieldName, value);
        }
        return row;
    }

    /// <summary>
    /// <para>执行insert into语句</para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="entity"></param>
    /// <returns></returns>
    public int Save(string table, Dictionary<string, object> entity) {
        lock (this) {
            int count = -1;
            IDbTransaction _sqlTrans = null;
            try {
                _sqlTrans = _conn.BeginTransaction();
                if (table == null) {
                    throw new ArgumentNullException("table=null");
                }
                this.EnsureConnection();
                string sql = BuildInsert(table, entity);
                count = this.ExecuteNonQuery(sql, BuildParamArray(entity));
                _sqlTrans.Commit();
            } catch (Exception ex) {
                Log.Error(typeof(SQLiteHelper), ex);
                if (_sqlTrans != null) {
                    _sqlTrans.Rollback();
                }
                return 0;

            }
            return count;
        }
    }

    public int Save(string table, List<Dictionary<string, object>> entitys) {
        lock (this) {
            int count = -1;
            IDbTransaction _sqlTrans = null;
            try {
                _sqlTrans = _conn.BeginTransaction();
                if (table == null) {
                    throw new ArgumentNullException("table=null");
                }
                this.EnsureConnection();
                foreach (Dictionary<String, Object> entity in entitys) {
                    string sql = BuildInsert(table, entity);
                    count += this.ExecuteNonQuery(sql, BuildParamArray(entity));
                }

                _sqlTrans.Commit();
            } catch (Exception ex) {
                Log.Error(typeof(SQLiteHelper), ex);
                if (_sqlTrans != null) {
                    _sqlTrans.Rollback();
                }
                return 0;

            }
            return count;
        }
    }

    private static SQLiteParameter[] BuildParamArray(Dictionary<string, object> entity) {
        List<SQLiteParameter> list = new List<SQLiteParameter>();
        foreach (string key in entity.Keys) {
            list.Add(new SQLiteParameter(key, entity[key]));
        }
        if (list.Count == 0)
            return null;
        return list.ToArray();
    }

    private static string BuildInsert(string table, Dictionary<string, object> entity) {
        StringBuilder buf = new StringBuilder();
        buf.Append("insert into ").Append(table);
        buf.Append(" (");
        foreach (string key in entity.Keys) {
            buf.Append(key).Append(",");
        }
        buf.Remove(buf.Length - 1, 1); // 移除最后一个,
        buf.Append(") ");
        buf.Append("values(");
        foreach (string key in entity.Keys) {
            buf.Append("@").Append(key).Append(","); // 创建一个参数
        }
        buf.Remove(buf.Length - 1, 1);
        buf.Append(") ");

        return buf.ToString();
    }

    private static string BuildUpdate(string table, Dictionary<string, object> entity) {
        StringBuilder buf = new StringBuilder();
        buf.Append("update ").Append(table).Append(" set ");
        foreach (string key in entity.Keys) {
            buf.Append(key).Append("=").Append("@").Append(key).Append(",");
        }
        buf.Remove(buf.Length - 1, 1);
        buf.Append(" ");
        return buf.ToString();
    }

    /// <summary>
    /// <para>执行update语句</para>
    /// <para>where参数不必要包含'where'关键字</para>
    ///
    /// <para>如果where=null,那么忽略whereParams</para>
    /// <para>如果where!=null,whereParams=null,where部分无参数</para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="entity"></param>
    /// <param Name="where"></param>
    /// <param Name="whereParams"></param>
    /// <returns></returns>
    public int Update(string table, Dictionary<string, object> entity, string where, SQLiteParameter[] whereParams) {
        lock (this) {
            if (table == null) {
                throw new ArgumentNullException("table=null");
            }

            int count = -1;
            IDbTransaction _sqlTrans = null;
            try {
                this.EnsureConnection();
                _sqlTrans = _conn.BeginTransaction();

                string sql = BuildUpdate(table, entity);
                SQLiteParameter[] arr = BuildParamArray(entity);
                if (where != null) {
                    sql += " where " + where;
                    if (whereParams != null) {
                        SQLiteParameter[] newArr = new SQLiteParameter[arr.Length + whereParams.Length];
                        Array.Copy(arr, newArr, arr.Length);
                        Array.Copy(whereParams, 0, newArr, arr.Length, whereParams.Length);

                        arr = newArr;
                    }
                }
                count = this.ExecuteNonQuery(sql, arr);
                _sqlTrans.Commit();
            } catch (Exception ex) {
                Log.Error(typeof(SQLiteHelper), ex);
                if (_sqlTrans != null) {
                    _sqlTrans.Rollback();
                }
                return 0;

            }
            return count;
        }
    }


    public int Update(string table, List<Dictionary<string, object>> entitys, string where, SQLiteParameter[] whereParams) {
        lock (this) {
            if (table == null) {
                throw new ArgumentNullException("table=null");
            }
            IDbTransaction _sqlTrans = null;
            int count = -1;
            try {
                this.EnsureConnection();
                _sqlTrans = _conn.BeginTransaction();

                foreach (Dictionary<string, object> entity in entitys) {
                    string sql = BuildUpdate(table, entity);
                    SQLiteParameter[] arr = BuildParamArray(entity);
                    if (where != null) {
                        sql += " where " + where;
                        if (whereParams != null) {
                            SQLiteParameter[] newArr = new SQLiteParameter[arr.Length + whereParams.Length];
                            Array.Copy(arr, newArr, arr.Length);
                            Array.Copy(whereParams, 0, newArr, arr.Length, whereParams.Length);

                            arr = newArr;
                        }
                    }
                    count += this.ExecuteNonQuery(sql, arr);
                }
                _sqlTrans.Commit();
            } catch (Exception ex) {
                Log.Error(typeof(SQLiteHelper), ex);
                if (_sqlTrans != null) {
                    _sqlTrans.Rollback();
                }
                return 0;

            }
            return count;
        }
    }

    /// <summary>
    /// <para>查询一行记录,无结果时返回null</para>
    /// <para>conditionCol = null时将忽略条件,直接执行select * from table </para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="conditionCol"></param>
    /// <param Name="conditionVal"></param>
    /// <returns></returns>
    public DataRow QueryOne(string table, string conditionCol, object conditionVal) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select * from " + table;
        if (conditionCol != null) {
            sql += " where " + conditionCol + "=@" + conditionCol;
        }

        DataTable ds = this.ExecuteRow(sql, new SQLiteParameter[] {
            new SQLiteParameter(conditionCol,conditionVal)
        });
        if (ds.Rows.Count > 0) {
            return ds.Rows[0];
        } else {
            return null;
        }
    }


    /// <summary>
    /// <para>查询一行记录,无结果时返回null</para>
    /// <para>conditionCol = null时将忽略条件,直接执行select * from table </para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="entity"></param>
    /// <returns></returns>
    public DataRow QueryOne(string table, Dictionary<string, object> entity) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select * from " + table + " where 1=1 ";
        SQLiteParameter[] param = new SQLiteParameter[entity.Keys.Count];
        int i = 0;
        foreach (KeyValuePair<string, object> kvp in entity) {
            string conditionCol = kvp.Key;
            object conditionVal = kvp.Value;
            sql += " and " + conditionCol + "=@" + conditionCol;
            param[i] = new SQLiteParameter(conditionCol, conditionVal);
            i++;
        }



        DataTable ds = this.ExecuteRow(sql, param);
        if (ds.Rows.Count > 0) {
            return ds.Rows[0];
        } else {
            return null;
        }
    }

    /// <summary>
    /// <para>查询数量,无结果时返回0</para>
    /// <para>conditionCol = null时将忽略条件,直接执行select * from table </para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="entity"></param>
    /// <returns></returns>
    public int count(string table, Dictionary<string, object> entity) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select count(1) as count from " + table + " where 1=1 ";
        SQLiteParameter[] param = new SQLiteParameter[entity.Keys.Count];
        int i = 0;
        foreach (KeyValuePair<string, object> kvp in entity) {
            string conditionCol = kvp.Key;
            object conditionVal = kvp.Value;
            sql += " and " + conditionCol + "=@" + conditionCol;
            param[i] = new SQLiteParameter(conditionCol, conditionVal);
            i++;
        }



        DataTable ds = this.ExecuteRow(sql, param);
        if (ds.Rows.Count > 0) {
            return int.Parse(ds.Rows[0]["count"].ToString());
        } else {
            return 0;
        }
    }

    /// <summary>
    /// <para>查询数量,无结果时返回0</para>
    /// <para>conditionCol = null时将忽略条件,直接执行select * from table </para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="entity"></param>
    /// <returns></returns>
    public int count(string table, String where) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select count(1) as count from " + table + " where 1=1 " + where;



        DataTable ds = this.ExecuteRow(sql, null);
        if (ds.Rows.Count > 0) {
            return int.Parse(ds.Rows[0]["count"].ToString());
        } else {
            return 0;
        }
    }

    /// <summary>
    /// <para>查询第一行记录,无结果时返回null</para>
    /// <para>conditionCol = null时将忽略条件,直接执行select * from table </para>
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="entity"></param>
    /// <returns></returns>
    public DataRow QueryFirst(string table, Dictionary<string, object> entity, Dictionary<string, object> orders) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select * from " + table + " where 1=1 ";
        SQLiteParameter[] param = new SQLiteParameter[entity.Keys.Count];
        int i = 0;
        foreach (KeyValuePair<string, object> kvp in entity) {
            string conditionCol = kvp.Key;
            object conditionVal = kvp.Value;
            sql += " and " + conditionCol + "=@" + conditionCol;
            param[i] = new SQLiteParameter(conditionCol, conditionVal);
            i++;
        }

        if (orders != null) {
            String order = "";
            foreach (KeyValuePair<string, object> kvp in orders) {
                string conditionCol = kvp.Key;
                object conditionVal = kvp.Value;
                order += "," + conditionCol + " " + conditionVal;
            }

            if (order.Length > 0) {
                sql += " order by " + order.Substring(1);
            }
        }



        DataTable ds = this.ExecuteRow(sql, param);
        if (ds.Rows.Count > 0) {
            return ds.Rows[0];
        } else {
            return null;
        }
    }

    public DataTable QueryDt(string table, Dictionary<string, object> entity, Dictionary<string, object> orders) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select * from " + table + " where 1=1";
        SQLiteParameter[] param = null;
        if (entity != null) {
            param = new SQLiteParameter[entity.Keys.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> kvp in entity) {
                string conditionCol = kvp.Key;
                object conditionVal = kvp.Value;
                sql += " and " + conditionCol + "=@" + conditionCol;
                param[i] = new SQLiteParameter(conditionCol, conditionVal);
                i++;
            }
        }

        if (orders != null) {
            String order = "";
            foreach (KeyValuePair<string, object> kvp in orders) {
                string conditionCol = kvp.Key;
                object conditionVal = kvp.Value;
                order += "," + conditionCol + " " + conditionVal;
            }

            if (order.Length > 0) {
                sql += " order by " + order.Substring(1);
            }
        }


        DataTable ds = this.ExecuteRow(sql, param);
        return ds;
    }

    public DataTable QueryDt(string table, string conditionCol, object conditionVal) {
        if (table == null) {
            throw new ArgumentNullException("table=null");
        }
        this.EnsureConnection();

        string sql = "select * from " + table;
        if (conditionCol != null) {
            sql += " where " + conditionCol + "=@" + conditionCol;
        }
        if (this.ShowSql) {
            Console.WriteLine("SQL: " + sql);
        }

        DataTable ds = this.ExecuteRow(sql, new SQLiteParameter[] {
            new SQLiteParameter(conditionCol,conditionVal)
        });
        return ds;
    }
    /// <summary>
    /// 执行delete from table 语句
    /// where不必包含'where'关键字
    /// where=null时将忽略whereParams
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="where"></param>
    /// <param Name="whereParams"></param>
    /// <returns></returns>
    public int Delete(string table, Dictionary<string, object> entity) {
        lock (this) {
            IDbTransaction _sqlTrans = null;
            try {
                if (table == null) {
                    throw new ArgumentNullException("table=null");
                }
                this.EnsureConnection();

                int count = -1;
                _sqlTrans = _conn.BeginTransaction();

                string sql = "delete from " + table + " where 1=1";
                SQLiteParameter[] param;

                if (entity == null) {
                    param = new SQLiteParameter[0];
                } else {
                    param = new SQLiteParameter[entity.Keys.Count];
                    int i = 0;
                    foreach (KeyValuePair<string, object> kvp in entity) {
                        string conditionCol = kvp.Key;
                        object conditionVal = kvp.Value;
                        sql += " and " + conditionCol + "=@" + conditionCol;
                        param[i] = new SQLiteParameter(conditionCol, conditionVal);
                        i++;
                    }
                }
                count = this.ExecuteNonQuery(sql, param);
                _sqlTrans.Commit();

                return count;
            } catch (Exception ex) {
                Log.Error(typeof(SQLiteHelper), ex);
                if (_sqlTrans != null) {
                    _sqlTrans.Rollback();
                }
                return 0;

            }
        }
    }

    /// <summary>
    /// 执行delete from table 语句
    /// where不必包含'where'关键字
    /// where=null时将忽略whereParams
    /// </summary>
    /// <param Name="table"></param>
    /// <param Name="where"></param>
    /// <param Name="whereParams"></param>
    /// <returns></returns>
    public int Delete(string table, string where, SQLiteParameter[] whereParams) {
        lock (this) {
            if (table == null) {
                throw new ArgumentNullException("table=null");
            }

            int count = -1;
            IDbTransaction _sqlTrans = null;
            try {
                this.EnsureConnection();
                _sqlTrans = _conn.BeginTransaction();

                string sql = "delete from " + table + " ";
                if (where != null) {
                    sql += "where " + where;
                }

                count = this.ExecuteNonQuery(sql, whereParams);
                _sqlTrans.Commit();
            } catch (Exception ex) {
                Log.Error(typeof(SQLiteHelper), ex);
                if (_sqlTrans != null) {
                    _sqlTrans.Rollback();
                }
                return 0;

            }
            return count;
        }
    }



    ///// <summary> 开始一个新事务
    ///// </summary>
    //private void BeginTrans()
    //{
    //    //lock (this)
    //    //{
    //    //    if (_sqlTrans != null &&
    //    //        _sqlTrans.Connection != null)
    //    //    {
    //    //        _sqlTrans = _conn.BeginTransaction();

    //    //        _isInTrans = true;
    //    //    }
    //    //}
    //}
    ///// <summary> 提交事务
    ///// </summary>
    //private void CommitTrans()
    //{
    //    //lock (this)
    //    //{
    //    //    if (_sqlTrans != null &&
    //    //        _sqlTrans.Connection != null)
    //    //    {
    //    //        _sqlTrans.Commit();

    //    //        _isInTrans = false;
    //    //    }
    //    //}
    //}

    ///// <summary> 回滚事务
    ///// </summary>
    //private void RollbackTrans()
    //{

    //    //if (_sqlTrans != null &&
    //    //    _sqlTrans.Connection != null)
    //    //{
    //    //    _sqlTrans.Rollback();
    //    //}

    //    //_isInTrans = false;

    //}
}

/// <summary>
/// 在SQLiteManager.Execute方法中回调,将SQLiteDataReader包装成object
/// </summary>
/// <param Name="reader"></param>
/// <returns></returns>
public delegate object ReaderWrapper(SQLiteDataReader reader);

/// <summary>
/// 将SQLiteDataReader的行包装成object
/// </summary>
/// <param Name="rowNum"></param>
/// <param Name="reader"></param>
/// <returns></returns>
public delegate object RowWrapper(int rowNum, SQLiteDataReader reader);




}


