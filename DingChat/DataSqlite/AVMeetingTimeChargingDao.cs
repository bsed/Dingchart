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

class AVMeetingTimeChargingDao : BaseDao {
    private static AVMeetingTimeChargingDao instance = null;
    public static AVMeetingTimeChargingDao getInstance() {
        if (instance == null) {
            instance = new AVMeetingTimeChargingDao();
        }
        return instance;
    }

    private Object lockObject = new Object();
    /// <summary>
    /// 插入会话
    /// </summary>
    /// <param Name="dt"></param>
    public int save(AVMeetingTimeChargingTable table) {
        int count = -1;
        lock (lockObject) {
            try {
                Dictionary<string, object> entity = new Dictionary<string, object>();
                // 插入表
                entity.Add("chargingId", table.chargingId);
                entity.Add("agoraKey", table.agoraKey);
                entity.Add("roomId", table.roomId);
                entity.Add("avType", table.avType);
                entity.Add("callType", table.callType);
                entity.Add("fromImId", table.fromImId);
                entity.Add("toImId", table.toImId);
                entity.Add("startTime", table.startTime);
                entity.Add("endTime", table.endTime);
                entity.Add("useTime", table.useTime);
                if (this.isExist("avmeeting_time_charging", "chargingId", table.chargingId)) {
                    SQLiteParameter[] param = new SQLiteParameter[] {
                        new SQLiteParameter("chargingId", table.chargingId)
                    };
                    count = this._mgr.Update("avmeeting_time_charging", entity, "chargingId=@chargingId", param);
                } else {
                    count = this._mgr.Save("avmeeting_time_charging", entity);
                }

            } catch (Exception e) {
                Log.Error(typeof(AVMeetingTimeChargingDao), e);
            }
        }
        return count;

    }

    /// <summary>
    /// 通过chargingId查找
    /// </summary>
    /// <param Name="chargingId"></param>
    /// <returns></returns>
    public AVMeetingTimeChargingTable findByChargingId(String chargingId) {
        AVMeetingTimeChargingTable table = null;
        try {
            table = new AVMeetingTimeChargingTable();
            DataRow entity = this._mgr.QueryOne("avmeeting_time_charging", "chargingId", chargingId);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<AVMeetingTimeChargingTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingTimeChargingDao), e);
        }
        return table;
    }

    /// <summary>
    /// 查询全部
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<AVMeetingTimeChargingTable> findAll() {
        List<AVMeetingTimeChargingTable> list = null;
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * from avmeeting_time_charging ");
            list = new List<AVMeetingTimeChargingTable>();
            DataTable entity = this._mgr.ExecuteRow(sb.ToStr(), null);
            list = DataUtils.DataTableToModelList<AVMeetingTimeChargingTable>(entity);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingTimeChargingDao), e);
        }
        return list;
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    /// <param Name="dt"></param>
    public int deleteAll() {
        int count = -1;
        try {
            count = this._mgr.Delete("avmeeting_time_charging", null, null);
        } catch (Exception e) {
            Log.Error(typeof(AVMeetingTimeChargingDao), e);
        }
        return count;

    }

}
}
