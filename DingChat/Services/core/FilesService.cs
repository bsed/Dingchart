using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace cn.lds.chatcore.pcw.Services.core {
class FilesService {
    private static FilesService instance = null;

    public static FilesService getInstance() {
        if (instance == null) {
            instance = new FilesService();
        }
        return instance;
    }

    /// <summary>
    /// 添加文件
    /// </summary>
    /// <param Name="eventData"></param>
    public void addFile(string fileStorageId, String localpath
                        , String fileType, Int64 duration, String owner, String fileName) {
        try {
            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId);
            if (table == null) {
                table = new FilesTable();
            }
            table.fileStorageId = fileStorageId;
            table.localpath = localpath;
            table.fileType = fileType;
            table.duration = duration;
            table.owner = owner;
            table.fileName = fileName;
            FileInfo file = new FileInfo(localpath);
            table.size = file.Length;
            FilesDao.getInstance().save(table);
        } catch (Exception e) {
            Log.Error(typeof (FilesService), e);
        }
    }

    /// <summary>
    /// 添加文件
    /// </summary>
    /// <param Name="eventData"></param>
    public void addFileWhenUpload(string fileStorageId, String localpath
                                  , String fileType, Int64 duration, String owner, String fileName) {
        try {
            FilesTable table = FilesDao.getInstance().findByFileOwner(owner);
            if (table == null) {
                table = new FilesTable();
                table.fileStorageId = fileStorageId;
                table.localpath = localpath;
                table.fileType = fileType;
                table.duration = duration;
                table.owner = owner;
                table.fileName = fileName;

                FileInfo file = new FileInfo(localpath);
                table.size = file.Length;
                FilesDao.getInstance().save(table);
            }
            //    else {
            //    FilesDao.getInstance().setFileStorageIdByOwner(fileStorageId, owner);
            //}

        } catch (Exception e) {
            Log.Error(typeof(FilesService), e);
        }
    }

    /// <summary>
    /// 判断文件信息是否存在
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public Boolean existFile(string fileStorageId) {
        try {
            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId);
            if (table != null &&string.IsNullOrEmpty( table.fileStorageId)==false) {
                return true;
            }
        } catch (Exception e) {
            Log.Error(typeof (FilesService), e);
        }
        return false;
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public FilesTable getFile(string fileStorageId) {
        try {
            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId);
            return table;
        } catch (Exception e) {
            Log.Error(typeof (FilesService), e);
        }
        return null;
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param Name="owner"></param>
    /// <returns></returns>
    public FilesTable getFileByOwner(String owner) {
        try {
            FilesTable table = FilesDao.getInstance().findByFileOwner(owner);
            return table;
        } catch (Exception e) {
            Log.Error(typeof (FilesService), e);
        }
        return null;
    }

    public int setFileStorageIdByOwner(string fileStorageId, String owner) {
        try {
            return FilesDao.getInstance().setFileStorageIdByOwner(fileStorageId, owner);
        } catch (Exception e) {
            Log.Error(typeof (FilesService), e);
        }
        return 0;
    }
}
}
