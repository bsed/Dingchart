using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business.Cache;

namespace cn.lds.chatcore.pcw.DataSqlite {
class VcardsDao : BaseDao {

    private static VcardsDao instance = null;
    public static VcardsDao getInstance() {
        if (instance == null) {
            instance = new VcardsDao();
        }
        return instance;
    }


    private VcardsTable getFromCache(String idOrNo) {
        VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(idOrNo);
        if (cvcard == null)
            return null;
        if (cvcard.table == null)
            return null;
        return cvcard.table;
    }

    private void updateCache(VcardsTable table) {
        VcardCacheEntity vcard = CacheHelper.getInstance().getVcard(table.no);
        if (vcard == null) {
            vcard = new VcardCacheEntity();
            IdNoCacheEntity idno = new IdNoCacheEntity();
            idno.id = table.clientuserId;
            idno.no = table.no;
            vcard.id = idno;

        }
        vcard.table = table;
        CacheHelper.getInstance().addVcard(vcard);
    }

    /// <summary>
    /// 更新信息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(VcardsTable table) {
        lock (this) {
            int count = -1;
            try {

                if (this.isExist("vcards", "clientuserId", table.clientuserId)) {
                    SQLiteParameter[] param = new SQLiteParameter[] {
                        new SQLiteParameter("clientuserId", table.clientuserId)
                    };
                    Dictionary<string, object> entity = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(table.no)) {
                        entity.Add("no", table.no);
                    }
                    if (!string.IsNullOrEmpty(table.clientuserId)) {
                        entity.Add("clientuserId", table.clientuserId);
                    }
                    if (!string.IsNullOrEmpty(table.nickname)) {
                        entity.Add("nickname", table.nickname);
                    }
                    if (!string.IsNullOrEmpty(table.avatarStorageRecordId)) {
                        entity.Add("AvatarStorageRecordId", table.avatarStorageRecordId);
                    }
                    if (!string.IsNullOrEmpty(table.mobileNumber)) {
                        entity.Add("mobileNumber", table.mobileNumber);
                    }
                    if (!string.IsNullOrEmpty(table.email)) {
                        entity.Add("email", table.email);
                    }
                    if (!string.IsNullOrEmpty(table.tel)) {
                        entity.Add("tel", table.tel);
                    }
                    if (!string.IsNullOrEmpty(table.desc)) {
                        entity.Add("desc", table.desc);
                    }
                    if (!string.IsNullOrEmpty(table.birthday)) {
                        entity.Add("birthday", table.birthday);
                    }
                    if (!string.IsNullOrEmpty(table.gender)) {
                        entity.Add("gender", table.gender);
                    }
                    if (!string.IsNullOrEmpty(table.country)) {
                        entity.Add("country", table.country);
                    }
                    if (!string.IsNullOrEmpty(table.province)) {
                        entity.Add("province", table.province);
                    }
                    if (!string.IsNullOrEmpty(table.city)) {
                        entity.Add("city", table.city);
                    }
                    if (!string.IsNullOrEmpty(table.identity)) {
                        entity.Add("identity", table.identity);
                    }
                    if (!string.IsNullOrEmpty(table.wechatid)) {
                        entity.Add("wechatid", table.wechatid);
                    }
                    count = this._mgr.Update("vcards", entity, "clientuserId=@clientuserId", param);
                } else {
                    Dictionary<string, object> entity = new Dictionary<string, object>();
                    entity.Add("no", table.no);
                    entity.Add("clientuserId", table.clientuserId);
                    entity.Add("nickname", table.nickname);
                    entity.Add("AvatarStorageRecordId", table.avatarStorageRecordId);
                    entity.Add("mobileNumber", table.mobileNumber);
                    entity.Add("email", table.email);
                    entity.Add("tel", table.tel);
                    entity.Add("desc", table.desc);
                    entity.Add("birthday", table.birthday);
                    entity.Add("gender", table.gender);
                    entity.Add("country", table.country);
                    entity.Add("province", table.province);
                    entity.Add("city", table.city);
                    entity.Add("identity", table.identity);
                    entity.Add("wechatid", table.wechatid);
                    count = this._mgr.Save("vcards", entity);
                }

                this.updateCache(table);

            } catch (Exception e) {
                Log.Error(typeof (VcardsDao), e);
            }
            return count;
        }
    }

    /// <summary>
    /// 通过clientuserId查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public VcardsTable findByClientuserId(String clientuserId) {
        VcardsTable cacheTable = this.getFromCache(clientuserId);
        if (cacheTable != null) {
            return cacheTable;
        } else {

            VcardsTable table = null;
            try {
                table = new VcardsTable();
                DataRow entity = this._mgr.QueryOne("vcards", "clientuserId", clientuserId);
                if (entity == null) return null;
                table = DataUtils.DataTableToModel<VcardsTable>(entity.Table);
                this.updateCache(table);
            } catch (Exception e) {
                Log.Error(typeof(VcardsDao), e);
            }


            return table;
        }
    }

    /// <summary>
    /// 通过NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public VcardsTable findByNoFromDb(String no) {

        VcardsTable table = null;
        try {
            table = new VcardsTable();
            DataRow entity = this._mgr.QueryOne("vcards", "no", no);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<VcardsTable>(entity.Table);

            this.updateCache(table);
        } catch (Exception e) {
            Log.Error(typeof(VcardsDao), e);
        }

        return table;
    }

    /// <summary>
    /// 通过NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public VcardsTable findByNo(String no) {
        VcardsTable cacheTable = this.getFromCache(no);
        if (cacheTable != null) {
            return cacheTable;
        } else {

            VcardsTable table = null;
            try {
                table = new VcardsTable();
                DataRow entity = this._mgr.QueryOne("vcards", "no", no);
                if (entity == null) return null;
                table = DataUtils.DataTableToModel<VcardsTable>(entity.Table);

                this.updateCache(table);
            } catch (Exception e) {
                Log.Error(typeof(VcardsDao), e);
            }

            return table;
        }
    }

    /// <summary>
    /// 通过NO删除
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public int deleteByNo(String no) {

        //插入数据库群的信息
        int count = this._mgr.Delete("vcards", "no=@no", new SQLiteParameter[] {
            new SQLiteParameter("no", no)
        });
        return count;

    }
}
}
