using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
[Entity(TableName = "files")]
public class FilesTable : EntityBase {
    /** 默认的字段ID */
    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    [NotNull]
    public Int64 id {
        get;
        set;
    }

    /** 服务器存储ID */
    [Column(ColumnName = "fileStorageId")]
    [Length(500)]
    [Index]
    public string fileStorageId {
        get;
        set;
    }

    /** 标准图下载完成后，本地存储路径 */
    [Column(ColumnName = "localpath")]
    [Length(2000)]
    public String localpath {
        get;
        set;
    }

    /** 文件类型(枚举FileType) */
    [Column(ColumnName = "fileType")]
    [Length(24)]
    public String fileType {
        get;
        set;
    }
    /** 文件名称（业务中的名称） */
    [Column(ColumnName = "fileName")]
    [Length(200)]
    public String fileName {
        get;
        set;
    }
    /** 文件大小 */


    [Column(ColumnName = "size")]
    public Int64 size {
        get;
        set;
    }

    /** 音频、视频播放时长 */
    [Column(ColumnName = "duration")]
    public Int64 duration {
        get;
        set;
    }

    /** 消息ID或其它 */
    [Column(ColumnName = "owner")]
    [Length(24)]
    public String owner {
        get;
        set;
    }


}
}