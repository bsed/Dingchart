using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Services {
public  class TimeServices {

    //static TimestampDao dao = new TimestampDao();
    //static   SQLiteHelper _mgr = SQLiteHelper.getInstance();

    private static TimeServices instance = null;
    public static TimeServices getInstance() {
        if (instance == null) {
            instance = new TimeServices();
        }
        return instance;
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <param Name="type"></param>
    /// <returns></returns>
    public long GetTime(TimestampType type,string tenantNo) {
        long timestamp = 0;
        try {
            TimestampTable timestampTable = TimestampDao.getInstance().findByType(type.ToStr(), tenantNo);
            if (timestampTable != null) {
                timestamp= timestampTable.timestamp;
            }
        } catch (Exception e) {
            Log.Error(typeof(TimeServices), e);
            timestamp = 0;
        }
        return timestamp;

    }

    /// <summary>
    /// 保存时间戳
    /// </summary>
    /// <param Name="type"></param>
    /// <param Name="Timestamp"></param>
    public void SaveTime(TimestampType type, long timestamp, string tenantNo) {
        try {
            TimestampTable timestampTable = TimestampDao.getInstance().findByType(type.ToStr(), tenantNo);
            if (timestampTable == null) {
                timestampTable = new TimestampTable();
                timestampTable.type = type.ToStr();
            }
            timestampTable.timestamp = timestamp;
            timestampTable.tenantNo = tenantNo;
            TimestampDao.getInstance().save(timestampTable);
        } catch (Exception e) {
            Log.Error(typeof(TimeServices), e);
        }
    }

    /// <summary>
    /// 保存时间戳(不推荐使用)
    /// </summary>
    /// <param Name="type"></param>
    /// <param Name="Timestamp"></param>
    public void SaveTime(String type, long timestamp, string tenantNo) {
        try {
            TimestampTable timestampTable = TimestampDao.getInstance().findByType(type, tenantNo);
            if (timestampTable == null) {
                timestampTable = new TimestampTable();
                timestampTable.type = type.ToStr();
            }
            timestampTable.timestamp = timestamp;
            timestampTable.tenantNo = tenantNo;
            TimestampDao.getInstance().save(timestampTable);
        } catch (Exception e) {
            Log.Error(typeof(TimeServices), e);
        }
    }

}
}
