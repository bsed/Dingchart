using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;
using System.Reflection;

namespace cn.lds.chatcore.pcw.Common.Enums {
/**
 * Created by zhoupeng on 2016/1/18.
 */
public enum DateTimeType {

    [Description("unknown")]
    [Default]
    UNKNOWN,
    [Description("yyyy-MM-dd HH:mm:ss")]
    yyyyMMddHHmmss,
    [Description("yyyyMMddHHmmss")]
    yyyyMMddHHmmss_1,
    [Description("yyyy-MM-dd HH:mm")]
    yyyyMMddHHmm,
    [Description("yyyy年MM月dd日 HH:mm")]
    yyyyMMddHHmm_1,
    [Description("yyy-MM-dd")]
    yyyyMMdd,
    [Description("yyyy.MM.dd")]
    yyyyMMdd_1,
    [Description("HH:mm:ss")]
    HHmmss,
    [Description("HH:mm")]
    HHmm,
    [Description("HHmm")]
    HHmm_1


}

}
