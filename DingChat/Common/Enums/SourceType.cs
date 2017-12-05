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
public enum SourceType {
    /** 未知 */
    UNKNOWN,
    /** 来自通讯录 */
    addressList,
    /** 来自二维码 */
    qrCode
}
}