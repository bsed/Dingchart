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
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Business.Cache;

namespace cn.lds.chatcore.pcw.DataSqlite {
public class SettingDao : BaseDao {
    private static SettingDao instance = null;
    public static SettingDao getInstance() {
        if (instance == null) {
            instance = new SettingDao();
        }
        return instance;
    }


    private ContactSettingCacheEntity getCacheEntity(String idOrNo) {
        ContactSettingCacheEntity cacheEntity = CacheHelper.getInstance().getContactSetting(idOrNo);
        if (cacheEntity == null) {
            return null;
        }
        if (cacheEntity.table == null) {
            return null;
        }
        return cacheEntity;
    }

    private void updateCache(SettingsTable table) {
        try {
            if (table == null) {
                return;
            }
            ContactSettingCacheEntity cacheEntity = CacheHelper.getInstance().getContactSetting(table.no);
            if (cacheEntity == null) {
                cacheEntity = new ContactSettingCacheEntity();
                IdNoCacheEntity idno = new IdNoCacheEntity();
                idno.id = table.id.ToString();
                idno.no = table.no;
                cacheEntity.id = idno;
            }
            cacheEntity.table = table;
            CacheHelper.getInstance().addContactSetting(cacheEntity);
        } catch (Exception e) {
            Log.Error(typeof(SettingDao), e);
        }
    }


    /// <summary>
    /// 插入会话
    /// </summary>
    /// <param Name="dt"></param>
    public int save(SettingsTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("no", table.no);
            entity.Add("Top", table.top);
            entity.Add("Quiet", table.quiet);
            entity.Add("backgroundurl", table.backgroundurl);
            entity.Add("enabaledraft", table.enabaledraft);
            if (this.isExist("setting", "no", table.no)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("no",table.no)
                };
                count = this._mgr.Update("setting", entity, "no=@no", param);
            } else {
                count = this._mgr.Save("setting", entity);
            }

            this.updateCache(table);
        } catch (Exception e) {
            Log.Error(typeof(SettingDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public SettingsTable findByNo(String no) {
        SettingsTable table = null;
        try {
            ContactSettingCacheEntity cacheEntity = this.getCacheEntity(no);

            if (cacheEntity != null) {
                return cacheEntity.table;
            }
            table = new SettingsTable();
            DataRow entity = this._mgr.QueryOne("setting", "no", no);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<SettingsTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(SettingDao), e);
        }
        return table;
    }

    /// <summary>
    /// 通过NO删除
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public int deleteByNo(String no) {
        int count = -1;
        try {
            SQLiteParameter[] param = new SQLiteParameter[] {
                new SQLiteParameter("no",no)
            };
            count = this._mgr.Delete("setting", "no=@no", param);

            CacheHelper.getInstance().removeContactSetting(no);
        } catch (Exception e) {
            Log.Error(typeof(SettingDao), e);
        }
        return count;
    }

    /// <summary>
    /// 从数据库中查询所有数据
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<SettingsTable> FindAllFromDb() {
        List<SettingsTable> table = null;
        DataTable dt = this._mgr.ExecuteRow("select * from setting ", null);
        table = DataUtils.DataTableToModelList<SettingsTable>(dt);
        return table;
    }
}
}
