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
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Business;

namespace cn.lds.chatcore.pcw.DataSqlite {
class MucDao : BaseDao {

    private static MucDao instance = null;
    public static MucDao getInstance() {
        if (instance == null) {
            instance = new MucDao();
        }
        return instance;
    }


    private GroupCacheEntity getCacheEntity(String idOrNo) {
        GroupCacheEntity cgroup = CacheHelper.getInstance().getGroup(idOrNo);
        if (cgroup == null) {
            return null;
        }
        if (cgroup.table == null) {
            return null;
        }
        return cgroup;
    }

    private MucTable getFromCache(String idOrNo) {
        GroupCacheEntity cgroup = CacheHelper.getInstance().getGroup(idOrNo);
        if (cgroup == null)
            return null;
        if (cgroup.table == null) {
            return null;
        }
        return cgroup.table;
    }

    private void updateCache(MucTable table) {
        try {
            GroupCacheEntity cgroup = CacheHelper.getInstance().getGroup(table.no);
            if (cgroup == null) {
                cgroup = new GroupCacheEntity();
                IdNoCacheEntity idno = new IdNoCacheEntity();
                idno.id = table.mucId;
                idno.no = table.no;
                cgroup.id = idno;
            }
            cgroup.table = table;
            CacheHelper.getInstance().addGroup(cgroup);
        } catch (Exception e) {
            Log.Error(typeof(MucDao), e);
        }
    }



    /// <summary>
    /// 更新群信息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(MucTable table) {
        //this._mgr.Open();
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("mucId", table.mucId);
            entity.Add("no", table.no);
            entity.Add("Name", table.name);
            entity.Add("AvatarStorageRecordId", table.avatarStorageRecordId);
            entity.Add("activeFlag", table.activeFlag);
            entity.Add("savedAsContact", table.savedAsContact);
            entity.Add("isTopmost", table.isTopmost);
            entity.Add("enableNoDisturb", table.enableNoDisturb);
            entity.Add("count", table.count);
            entity.Add("qrcodeId", table.qrcodeId);
            entity.Add("manager", table.manager);
            if (App.AccountsModel.clientuserId == table.manager) {
                entity.Add("isOwner", true);
            } else {
                entity.Add("isOwner", false);
            }
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.name)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.name);
                fristPinyin = PinyinHelper.getFristPinyin(table.name);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);


            if (this.isExist("muc", "mucId", table.mucId)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("mucId",table.mucId)
                };
                count = this._mgr.Update("muc", entity, "mucId=@mucId", param);
            } else {
                count = this._mgr.Save("muc", entity);
            }

            this.updateCache(table);


        } catch (Exception e) {
            Log.Error(typeof(MucDao), e);
        }
        return count;

    }

    /// <summary>
    /// 插入群列表
    /// </summary>
    /// <param Name="dt"></param>
    public int InsertGroup(List<MucTable> beanList) {
        //this._mgr.Open();
        int count = -1;
        try {
            List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();
            //插入群的信息
            for (int i = 0; i < beanList.Count; i++) {
                if (beanList[i].deleteFlag == true) {
                    continue;
                }
                MucTable bean = beanList[i];
                this.save(bean);
                ////插入数据库群的信息
                //this._mgr.Delete("muc", "no=@no", new SQLiteParameter[] {
                //    new SQLiteParameter("no", bean.no)
                //});

                //Dictionary<string, object> entity = new Dictionary<string, object>();


                //entity.Add("mucId", bean.mucId);
                //entity.Add("no", bean.no);
                //entity.Add("Name", bean.Name);
                //entity.Add("AvatarStorageRecordId", bean.AvatarStorageRecordId);
                //entity.Add("isOwner", bean.isOwner);
                //entity.Add("activeFlag", bean.activeFlag);
                //entity.Add("savedAsContact", bean.savedAsContact);
                //entity.Add("isTopmost", bean.isTopmost);
                //entity.Add("enableNoDisturb", bean.enableNoDisturb);
                //entitys.Add(entity);

                //this.updateCache(bean);


            }
            //count = this._mgr.Save("muc", entitys);
        } catch (Exception e) {
            Log.Error(typeof(MucDao), e);
        }
        return count;

    }





/// <summary>
/// 查找所有通讯录
/// </summary>
/// <returns></returns>
    public List<MucTable> FindAllGroup() {
        List<MucTable> table = null;
        DataTable dt = this._mgr.ExecuteRow("select * from muc  where SavedAsContact=1  order by totalPinyin asc", null);
        table = DataUtils.DataTableToModelList<MucTable>(dt);
        return table;
    }

/// <summary>
/// 查找所有群人员
/// </summary>
/// <returns></returns>
    //public List<MucMembersTable> FindAllGroupMember() {
    //    List<MucMembersTable> table = null;
    //    DataTable dt = this._mgr.ExecuteRow("select * from muc_members", null);
    //    table = DataUtils.DataTableToModelList<MucMembersTable>(dt);
    //    return table;
    //}

    public List<MucMembersTable> FindDistictGroupMember() {
        List<MucMembersTable> table = null;
        DataTable dt = this._mgr.ExecuteRow(" select distinct Clientuserid,AvatarStorageRecordId,mucid from  muc_members  ", null);
        table = DataUtils.DataTableToModelList<MucMembersTable>(dt);
        return table;
    }
    public MucTable FindGroupById(string id) {
        MucTable cacheTable = this.getFromCache(id);
        if (cacheTable != null) {
            return cacheTable;
        } else {
            MucTable table = null;
            DataRow entity = this._mgr.QueryOne("muc", "MucId", id);
            if (entity == null) {
                return null;
            }
            table = DataUtils.DataTableToModel<MucTable>(entity.Table);

            this.updateCache(table);
            return table;
        }
    }
/// <summary>
/// 用no查找群信息
/// </summary>
/// <returns></returns>
    public MucTable FindGroupByNo(string no) {
        //MucTable cacheTable = this.getFromCache(no);
        //if (cacheTable != null) {
        //    return cacheTable;
        //} else {
        //asdasdsd
        MucTable table = null;
        DataRow entity = this._mgr.QueryOne("muc", "no", no);
        if (entity == null) {
            return null;
        }
        table = DataUtils.DataTableToModel<MucTable>(entity.Table);

        this.updateCache(table);
        return table;
        //}
    }
/// <summary>
/// 通过NO删除
/// </summary>
/// <param Name="no"></param>
/// <returns></returns>
    public int deleteByNo(String no) {
        CacheHelper.getInstance().removeGroup(no);
        //插入数据库群的信息
        int count = this._mgr.Delete("muc", "no=@no", new SQLiteParameter[] {
            new SQLiteParameter("no", no)
        });
        return count;

    }

    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<MucTable> list = null;
            String sql = "select * from muc where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<MucTable>(dt);
            if (list != null) {
                foreach (MucTable table in list) {
                    this.save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(MucDao), e);
        }
    }
}
}
