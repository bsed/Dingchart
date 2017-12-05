using cn.lds.chatcore.pcw.Attributes;
using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.lds.chatcore.pcw.Models.Tables {
/**
* Created by quwei on 2016/06/28.
* 语音聊天&视频聊天计时表
*/
[Entity(TableName = "avmeeting_time_charging")]
public class AVMeetingTimeChargingTable : EntityBase {
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
    * 计费编号，用时间戳
    */
    [Column(ColumnName = "chargingId")]
    [Length(100)]
    public String chargingId {
        get;
        set;
    }

    /**
    * 声网的key
    */
    [Column(ColumnName = "agoraKey")]
    [Length(100)]
    public String agoraKey {
        get;
        set;
    }

    /**
    * 房间编号
    */
    [Column(ColumnName = "roomId")]
    [Length(100)]
    public String roomId {
        get;
        set;
    }
    /**
    * 通话类型 audio、video
    */
    [Column(ColumnName = "avType")]
    [Length(100)]
    public String avType {
        get;
        set;
    }
    /**
    * 呼叫类型 calling、called
    */
    [Column(ColumnName = "callType")]
    [Length(100)]
    public String callType {
        get;
        set;
    }

    /**
    * 自己的社群编号
    */
    [Column(ColumnName = "fromImId")]
    [Length(100)]
    public String fromImId {
        get;
        set;
    }

    /**
    * 对方的社群编号
    */
    [Column(ColumnName = "toImId")]
    [Length(100)]
    public String toImId {
        get;
        set;
    }

    /**
    * 开始时间
    */
    [Column(ColumnName = "startTime")]
    [Length(100)]
    public String startTime {
        get;
        set;
    }

    /**
    * 结束时间
    */
    [Column(ColumnName = "endTime")]
    [Length(100)]
    public String endTime {
        get;
        set;
    }

    /**
    * 结束时间（单位：分钟）
    */
    [Column(ColumnName = "useTime")]
    public Int64 useTime {
        get;
        set;
    }
}
}
