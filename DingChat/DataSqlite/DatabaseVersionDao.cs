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

namespace cn.lds.chatcore.pcw.DataSqlite {

class DatabaseVersionDao : BaseDao {

    private static DatabaseVersionDao instance = null;
    public static DatabaseVersionDao getInstance() {
        if (instance == null) {
            instance = new DatabaseVersionDao();
        }
        return instance;
    }

    public int save(DatabaseVersion table) {
        int count = -1;
        try {
            //this._mgr.Delete("accounts", null, null);

            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("databaseVersion", table.databaseVersion);

            DatabaseVersion databaseVersion = this.findOne();

            if (databaseVersion != null) {
                count = this._mgr.Update("database_version", entity, "id=@id", new System.Data.SQLite.SQLiteParameter[] {
                    new SQLiteParameter("id",databaseVersion.id)
                });
            } else {
                count = this._mgr.Save("database_version", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(DatabaseVersionDao), e);
        }

        return count;

    }


    public DatabaseVersion findOne() {
        DatabaseVersion table = null;
        try {
            table = new DatabaseVersion();
            DataRow entity = this._mgr.QueryOne("database_version", null, null);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<DatabaseVersion>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(DatabaseVersionDao), e);
        }
        return table;
    }

}
}
