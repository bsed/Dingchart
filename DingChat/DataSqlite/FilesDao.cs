using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.DataSqlite {
public class FilesDao : BaseDao {
    SQLiteHelper _mgr = SQLiteHelper.getInstance();

    private static FilesDao instance = null;
    public static FilesDao getInstance() {
        if (instance == null) {
            instance = new FilesDao();
        }
        return instance;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param Name="dt"></param>
    public int save(FilesTable table) {
        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            // 插入表
            entity.Add("fileStorageId", table.fileStorageId);
            entity.Add("localpath", table.localpath);
            entity.Add("fileType", table.fileType);
            entity.Add("duration", table.duration);
            entity.Add("owner", table.owner);
            entity.Add("fileName", table.fileName);
            entity.Add("size", table.size);
            if (this.isExist("files", "fileStorageId", table.fileStorageId)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("fileStorageId",table.fileStorageId)
                };
                count = this._mgr.Update("files", entity, "fileStorageId=@fileStorageId", param);
            } else {
                count = this._mgr.Save("files", entity);
            }

        } catch (Exception e) {
            Log.Error(typeof(FilesDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过文件的存储ID查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public FilesTable findByFileStorageId(string fileStorageId) {
        FilesTable table = null;
        try {
            table = new FilesTable();
            DataRow entity = this._mgr.QueryOne("files", "fileStorageId", fileStorageId);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<FilesTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(FilesDao), e);
        }
        return table;
    }

    /// <summary>
    /// 通过文件的所有者查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public FilesTable findByFileOwner(String owner) {
        FilesTable table = null;
        try {
            table = new FilesTable();
            DataRow entity = this._mgr.QueryOne("files", "owner", owner);
            if (entity == null) return null;
            table = DataUtils.DataTableToModel<FilesTable>(entity.Table);
        } catch (Exception e) {
            Log.Error(typeof(FilesDao), e);
        }
        return table;
    }

    /// <summary>
    /// 根据文件的owner设置存储ID
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int setFileStorageIdByOwner(string fileStorageId,String owner) {
        int count = -1;
        try {
            String sql = "update files set fileStorageId = '" + fileStorageId + "'"+ " where owner ='" + owner + "'";
            count = this._mgr.ExecuteNonQuery(sql, null);
        } catch (Exception e) {

            Log.Error(typeof(FilesDao), e);
        }
        return count;
    }
}
}
