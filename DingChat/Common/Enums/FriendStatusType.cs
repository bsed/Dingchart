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
public enum FriendStatusType {
    [Description("")]
    [Default]
    UNKNOWN = 0,	/** 未知 */
    [Description("normal")]
    normal=1,	/** 正常 */
    [Description("deleted")]
    deleted=2,	/** 删除 */
    [Description("invalid")]
    invalid=3	/** 无效，一般是单方面删除了好友后，对方的好友状态为invalid */


            //private String value = "";

            //private FriendStatusType(final String value) {
            //    this.value = value;
            //}

            //public static FriendStatusType getValue(final String value) {
            //    switch (value) {
            //    case "":
            //        return UNKNOWN;
            //    case "normal":
            //        return normal;
            //    case "deleted":
            //        return deleted;
            //    case "invalid":
            //        return invalid;
            //    default:
            //        return UNKNOWN;
            //    }
            //}

            //public String value() {
            //    return this.value;
            //}
}
}