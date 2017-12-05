using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
 * 群聊表
 * <p/>
 * Created by quwei on 2015/11/26.
 */
[Entity(TableName = "muc")]
public class MucTable : EntityBase {
    /**
     * 默认的字段ID
     */
    [Column(ColumnName = "id")]
    [PrimaryKey]
    [Identity(1, 1)]
    public Int64 id {
        get;
        set;
    }





    /**
     * 群聊id
     */

    [Column(ColumnName = "mucId")]
    [Length(100)]
    public String mucId {
        get;
        set;
    }

    /**
     * 群编号
     */

    [Column(ColumnName = "no")]
    [Length(100)]
    [Index]
    public String no {
        get;
        set;
    }

    /**
     * 群聊名称
     */

    [Column(ColumnName = "Name")]
    [Length(512)]
    public String name {
        get;
        set;
    }

    /**
     * 头像
     */

    [Column(ColumnName = "AvatarStorageRecordId")]
    public String avatarStorageRecordId {
        get;
        set;
    }
    public BitmapImage logoPath {
        get;
        set;
    }
    [Column(ColumnName = "manager")]
    [Length(128)]
    public String manager {
        get;
        set;
    }
    /**
     * 是否群主
     */


    [Column(ColumnName = "isOwner")]
    public Boolean isOwner {
        get;
        set;
    }

    /**
     * 是否激活
     */


    [Column(ColumnName = "activeFlag")]
    public Boolean activeFlag {
        get;
        set;
    }

    /**
     * 是否加通讯录
     */


    [Column(ColumnName = "savedAsContact")]
    public Boolean savedAsContact {
        get;
        set;
    }

    /**
    * 是否置顶
    */


    [Column(ColumnName = "isTopmost")]
    public Boolean isTopmost {
        get;
        set;
    }

    /**
    * 是否勿扰模式
    */

    [Column(ColumnName = "enableNoDisturb")]
    public Boolean enableNoDisturb {
        get;
        set;
    }


    public Boolean deleteFlag {
        get;
        set;
    }


    [Column(ColumnName = "count")]
    public Int64 count {
        get;
        set;
    }

    /** 拼音全拼 */
    [Column(ColumnName = "totalPinyin")]
    [Length(256)]
    [Index]
    public String totalPinyin {
        get;
        set;
    }

    /** 拼音首字母 */
    [Column(ColumnName = "fristPinyin")]
    [Length(256)]
    [Index]
    public String fristPinyin {
        get;
        set;
    }

    /**
    * 群二维码
    */
    [Column(ColumnName = "qrcodeId")]
    [Length(128)]
    public String qrcodeId {
        get;
        set;
    }
}
}
