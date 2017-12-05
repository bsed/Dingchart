using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.DatabaseUpdate.UpdateSql;

namespace cn.lds.chatcore.pcw.DatabaseUpdate {
public class DatabaseUpdateScript {
    private static DatabaseUpdateScript databaseUpdateScript = null;

    private static SortedDictionary<int, String> afterUpdateDbStructScript = null;

    public static DatabaseUpdateScript getInstance() {
        if (databaseUpdateScript == null) {
            databaseUpdateScript = new DatabaseUpdateScript();
        }
        afterUpdateDbStructScript = new SortedDictionary<int, string>();

        CreateAfterDbStructUpdateScript();
        return databaseUpdateScript;
    }

    private static void CreateAfterDbStructUpdateScript() {
        //数据库版本号，要执行的sql，以;分割
        afterUpdateDbStructScript.Add(2, Db_Version_2.GetUpdateSql());
        afterUpdateDbStructScript.Add(9, Db_Version_9.GetUpdateSql());
    }


    public String[] GetAfterDbStructUpdateScript(int currentDbVersion, long databaseDbVersion) {
        List<String> rtSql = new List<string>();
        SortedDictionary<int, String>.KeyCollection keys = afterUpdateDbStructScript.Keys;

        foreach (int key in keys) {
            if (key > databaseDbVersion && key <= currentDbVersion) {
                String sqlString = "";
                afterUpdateDbStructScript.TryGetValue(key,out sqlString);
                if (sqlString != null && !"".Equals(sqlString)) {
                    String[] sqlArray = sqlString.Split(';');
                    rtSql.AddRange(sqlArray.ToList());
                }
            }
        }
        return rtSql.ToArray();
    }


}
}
