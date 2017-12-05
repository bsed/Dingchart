using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DatabaseUpdate;
using cn.lds.chatcore.pcw.Services;

namespace cn.lds.chatcore.pcw.Common.DbHelper {
public   class DbManager {



    SQLiteHelper sqliteHelper = SQLiteHelper.getInstance();

    /// <summary>
    /// 取得全部Entity对象
    /// </summary>
    /// <param Name="nameSpaces"></param>
    /// <returns></returns>
    public  List<string> GetClassesByEntityAttribute(string nameSpaces) {
        Assembly asm = Assembly.GetExecutingAssembly();

        List<string> entityClassList = new List<string>();

        foreach (Type type in asm.GetTypes()) {
            if (type.Namespace == nameSpaces) {
                EntityAttribute entityAttribute = type.GetCustomAttribute<EntityAttribute>();
                if (entityAttribute != null) {
                    entityClassList.Add(type.FullName);
                }
            }
        }

        return entityClassList;
    }


    /// <summary>
    /// 取得Entity实体对象的全部DB列属性的信息
    /// </summary>
    /// <param Name="classFullName"></param>
    /// <param Name="dt"></param>
    public  void GetPropertyByColumnAttribute(string classFullName, DataTable dt) {
        foreach (PropertyInfo property in Type.GetType(classFullName).GetProperties()) {
            ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            LengthAttribute lengthAttribute = property.GetCustomAttribute<LengthAttribute>();
            NotNullAttribute notNullAttribute = property.GetCustomAttribute<NotNullAttribute>();
            ImageAttribute ImageAttribute = property.GetCustomAttribute<ImageAttribute>();
            if (columnAttribute != null) {
                DataColumn cloumn = dt.Columns.Add();
                cloumn.ColumnName = columnAttribute.ColumnName;

                cloumn.AllowDBNull = (notNullAttribute != null ? false : true);

                if (property.PropertyType.Equals(Type.GetType("System.Int64"))) { // 数值(注:金额*100,比率*1000),时间
                    cloumn.DataType = Type.GetType("System.Int64");
                } else if (property.PropertyType.Equals(Type.GetType("System.Int32"))) { // 数值
                    cloumn.DataType = Type.GetType("System.Int32");
                } else if (property.PropertyType.Equals(Type.GetType("System.String"))) { // 文字列
                    if (ImageAttribute != null) {
                        cloumn.DataType = Type.GetType("System.Byte[]");
                    } else if (lengthAttribute != null) {
                        cloumn.MaxLength = lengthAttribute.Length;
                        cloumn.DataType = Type.GetType("System.String");
                    } else {
                        cloumn.MaxLength = 4000;
                        cloumn.DataType = Type.GetType("System.String");
                    }

                } else if (property.PropertyType.Equals(Type.GetType("System.Boolean"))) { // TRUE/FALSE
                    cloumn.DataType = Type.GetType("System.Boolean");
                }
                //else if (field.FieldType.Equals(Type.GetType("System.DateTime"))) // 无日期类型,预留
                //{
                //    cloumn.DataType = Type.GetType("System.DateTime");
                //}
                else {
                    //throw new LDSRuntimeException(null, "F0000001", new String[] { "不支持的数据类型:" + property.PropertyType.ToString() });
                }
            }

        }
    }


    /// <summary>
    /// 取得Entity实体对象的主键的信息
    /// </summary>
    /// <param Name="classFullName"></param>
    /// <param Name="dt"></param>
    public  void GetPropertyByPrimaryKeyAttribute(string classFullName, DataTable dt) {
        foreach (PropertyInfo property in Type.GetType(classFullName).GetProperties()) {
            PrimaryKeyAttribute primaryAttribute = property.GetCustomAttribute<PrimaryKeyAttribute>();
            ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (primaryAttribute != null) {
                DataColumn cloumn = dt.Columns[columnAttribute.ColumnName];
                dt.PrimaryKey = new DataColumn[] { cloumn };
                break;
            }

        }
    }

    /// <summary>
    /// 取得Entity实体对象的索引的信息
    /// </summary>
    /// <param Name="classFullName"></param>
    /// <param Name="dt"></param>
    public  List<DataColumn> GetPropertyByIndexAttribute(string classFullName, DataTable dt) {
        List<DataColumn> columnList = new List<DataColumn>();
        foreach (PropertyInfo property in Type.GetType(classFullName).GetProperties()) {
            IndexAttribute indexAttribute = property.GetCustomAttribute<IndexAttribute>();
            ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (indexAttribute != null) {
                DataColumn cloumn = dt.Columns[columnAttribute.ColumnName];
                if(!indexAttribute.IndexColumnName.Trim().Equals(String.Empty))
                    cloumn.ExtendedProperties.Add("IndexColumnName", indexAttribute.IndexColumnName);
                columnList.Add(cloumn);
            }
        }
        return columnList;
    }
    /// <summary>
    /// 根据Entity对象的定义,取得创建DB实体的SQL文
    /// </summary>
    /// <param Name="dt"></param>
    /// <returns></returns>
    public  String GetCreateTableSQL(DataTable dt) {
        //获取创建表的SQL
        String strSql = "Create Table [" + dt.TableName.Trim() + "] ( ";

        StringBuilder strSqlContents = new StringBuilder();
        foreach (DataColumn dc in dt.Columns) { //循环列
            //获取创建表的SQL
            if (strSqlContents.Length > 0) {
                strSqlContents.Append(",");
            }

            String fieldname = dc.ColumnName.Trim();

            // 项目的首字母大写
            if (fieldname.Length > 1) {
                fieldname = fieldname.Substring(0, 1).ToUpper() + fieldname.Substring(1);
            } else {
                fieldname = fieldname.ToUpper();
            }

            strSqlContents.Append("[").Append(fieldname).Append("] ");
            if (fieldname.ToUpper().Equals("ID")) {
                strSqlContents.Append("INTEGER  NOT NULL PRIMARY KEY  autoincrement");
            } else {
                strSqlContents.Append(getDBTypeStr(dc.DataType, dc.MaxLength, dc.AllowDBNull));
            }
        }
        strSql += strSqlContents.ToString() + ")";

        return strSql;
    }


    public  String getDBTypeStr(Type csType, int maxLength, Boolean allowDBNull) {
        if (csType.Equals(Type.GetType("System.Int64"))) { // 数值(注:金额*100,比率*1000),时间
            return "INTEGER" + (allowDBNull ? "" : " NOT NULL ");
        } else if (csType.Equals(Type.GetType("System.Long"))) { // 数值
            return "INTEGER" + (allowDBNull ? "" : " NOT NULL ");
        } else if (csType.Equals(Type.GetType("System.Int32"))) { // 数值
            return "INTEGER" + (allowDBNull ? "" : " NOT NULL ");
        } else if (csType.Equals(Type.GetType("System.String"))) { // 文字列
            if (maxLength > 4000) {
                return "TEXT" + (allowDBNull ? "" : " NOT NULL ");
            } else {
                return "NVARCHAR(" + maxLength + ")" + (allowDBNull ? "" : " NOT NULL ");
            }

        } else if (csType.Equals(Type.GetType("System.Boolean"))) { // TRUE/FALSE
            return "BOOLEAN" + (allowDBNull ? "" : " NOT NULL ");
        } else if (csType.Equals(Type.GetType("System.Byte[]"))) { // TRUE/FALSE
            return " NTEXT ";
        }
        //else if (csType.Equals(Type.GetType("System.DateTime"))) // 无日期类型,预留
        //{
        //    return "NUMERIC(15)";
        //}
        else {
            //throw new LDSRuntimeException(null, "F0000001", new String[] { "不支持的数据类型:" + csType.ToString() });
        }
        return "";
    }


    /// <summary>
    /// 根据Entity对象的定义,取得创建Index的SQL文
    /// </summary>
    /// <param Name="dt"></param>
    /// <param Name="indexColumnList"></param>
    /// <returns></returns>
    public  List<String> CreateIndex(DataTable dt, List<DataColumn> indexColumnList) {
        List<String> indexSQLList = new List<String>();

        //获取创建表的SQL
        // String strSql = "Create Index [" + dt.TableName.Trim() + "_INDEX_@1@] On [" + dt.TableName.Trim() + "] (@2@)";
        String strSql = "CREATE INDEX IF NOT EXISTS index_" + dt.TableName.Trim() + "_@1@ ON " + dt.TableName.Trim() + " (@2@)";
        foreach (DataColumn dc in indexColumnList) { //循环列
            String strSqlContents = strSql;

            String fieldname = dc.ColumnName.Trim();
            String indexColumnName = fieldname;

            bool hasIndexColumnName = dc.ExtendedProperties.ContainsKey("IndexColumnName");
            if (hasIndexColumnName) {
                indexColumnName = dc.ExtendedProperties["IndexColumnName"].ToStr();
            }


            strSqlContents = strSqlContents.Replace("@1@", fieldname);
            strSqlContents = strSqlContents.Replace("@2@", (indexColumnName));

            indexSQLList.Add(strSqlContents);
        }

        return indexSQLList;
    }

    /// <summary>
    /// 执行多条更新SQL
    /// </summary>
    /// <param Name="sqls"></param>
    /// <returns>处理件数</returns>
    public  int ExcuteSQL(String[] sqls) {
        if (sqls == null || sqls.Length == 0) {
            return 0;
        }

        int resultCount = 0;

        foreach (String sql in sqls) {
            resultCount = sqliteHelper.ExecuteNonQuery(sql, null);
        }
        return resultCount;
    }

    /// <summary>
    /// 根据用户建立DB连接
    /// </summary>
    /// <param Name="user">用户ID,该参数为空时,连接模板DB</param>
    public  void createDatabaseFile(String user) {
        if (user == string.Empty)
            user = "local";

        //如果不存在就创建file文件夹
        if ( Directory.Exists(System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" +user) == false) {
            Directory.CreateDirectory(System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" + user);
        }
        if (File.Exists(System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" + user + "/" + user + ".db") == false) {
            string path = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/template/LocalDBTemplete.db";
            string path2 = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" + user + "/" + user + ".db";
            FileInfo fi1 = new FileInfo(path);
            FileInfo fi2 = new FileInfo(path2);
            try {
                ////Ensure that the target does not exist.
                //fi2.Delete();
                fi1.CopyTo(path2, true);
            } catch(Exception e) {
                Log.Error(typeof (DbManager), e);
            }
        }

        App.DataBasePath = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" + user + "/" + user + ".db";
    }



    private List<String> createAlterTableSql(DataTable modelDataTable,DataTable dbDataTable) {
        List<String> alterSqls = new List<String>();
        if (!modelDataTable.TableName.Equals(dbDataTable.TableName)) {
            return null;
        }
        foreach (DataColumn modelColumn in modelDataTable.Columns) {
            bool modelColumnExistInDb = false;
            foreach (DataColumn dbColumn in dbDataTable.Columns) {
                if (dbColumn.ColumnName.ToLower().Equals(modelColumn.ColumnName.ToLower())) {
                    modelColumnExistInDb = true;
                    break;
                }
            }
            if (!modelColumnExistInDb) {
                //构建alter语句
                String alterSql = this.getAlterColumnSql(modelColumn);

                if (alterSql != null) {
                    alterSqls.Add(alterSql);
                }
            }

        }

        return alterSqls;
    }

    private String getAlterColumnSql(DataColumn column) {
        String tableName = column.Table.TableName;
        String columnTypeString = this.getDBTypeStr(column.DataType, column.MaxLength, column.AllowDBNull);
        columnTypeString = columnTypeString.Replace("NOT NULL", "");
        String sql = "ALTER TABLE " + tableName + " ADD " + " " + column.ColumnName + " " + columnTypeString;
        if (columnTypeString == null || columnTypeString.ToString().Equals("")) {
            return null;
        }
        return sql;
    }

    public void updateDatabase() {
        long dbVersion = DatabaseVersionServices.getInstance().getCurrentVersion();
        if (DatabaseVersion.DATABASE_VERSION > dbVersion) {
            this.updateDatabaseStruct(true);
            try {
                String[] sqls = DatabaseUpdateScript.getInstance().GetAfterDbStructUpdateScript(DatabaseVersion.DATABASE_VERSION, dbVersion);
                if (sqls != null && sqls.Length > 0) {
                    ExcuteSQL(sqls);
                }
            } catch (Exception ex) {
                Log.Error(typeof(DbManager),ex);
            }

            DatabaseVersionServices.getInstance().save(DatabaseVersion.DATABASE_VERSION);
        }




    }

/// <summary>
/// 创建所有DB
/// </summary>
    public void updateDatabaseStruct(bool needUpdate) {

        createDatabaseFile(App.AccountsModel.no);
        if (needUpdate) {
            //sqliteHelper.Open();
            List<String> entityClassList = GetClassesByEntityAttribute(Constants.ENTITY_CLASS_NAMESPACES);
            foreach (String classFullName in entityClassList) {
                List<String> alterSqlList = null;
                EntityAttribute entityAttribute = Type.GetType(classFullName).GetCustomAttribute<EntityAttribute>();
                String tableName = entityAttribute.TableName;
                DataTable dt = new DataTable(tableName);
                GetPropertyByColumnAttribute(classFullName, dt);
                GetPropertyByPrimaryKeyAttribute(classFullName, dt);

                try {
                    //判断表是否已经存在
                    if (sqliteHelper.TableExists(tableName) == true) {
                        //continue;
                        DataTable dbDataTable = SQLiteHelper.getInstance().getTable(tableName);
                        alterSqlList = this.createAlterTableSql(dt, dbDataTable);
                        if (alterSqlList != null && alterSqlList.ToArray().Length > 0) {
                            ExcuteSQL(alterSqlList.ToArray());
                        }
                        continue;
                    }
                } catch (Exception e) {
                    Log.Error(typeof(DbManager), e);
                    //sqliteHelper.Close();
                    return;
                }


                List<DataColumn> indexColumnList = GetPropertyByIndexAttribute(classFullName, dt);

                List<String> createSqlList = new List<String>();

                createSqlList.Add(GetCreateTableSQL(dt));
                //sqlList.Add(CreatePrimaryKey(dt));
                List<String> indexSQLList = CreateIndex(dt, indexColumnList);


                ExcuteSQL(createSqlList.ToArray());
                ExcuteSQL(indexSQLList.ToArray());
            }
        }
    }
}
}
