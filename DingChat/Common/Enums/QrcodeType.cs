using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Enums {
/**
 * 时间戳类型
 */
public enum QrcodeType {
    [Description("personal")]
    [Default]
    personal = 0,	/** 个人 */
    [Description("group")]
    group=1,			/** 群 */
    [Description("subscription")]
    subscription=2,	/** 订阅号 */
    [Description("activity")]
    activity=3		/** 活动 */


             //private String value = "001";

             //private QrcodeType(final String value) {
             //    this.value = value;
             //}

             //public static QrcodeType getValue(final String value) {
             //    switch (value) {
             //        case "personal":
             //            return personal;
             //        case "group":
             //            return group;
             //        case "subscription":
             //            return subscription;
             //        case "activity":
             //            return activity;
             //        default:
             //            return personal;
             //    }
             //}

             //public String value() {
             //    return this.value;
             //}
}
}