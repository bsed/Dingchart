using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace cn.lds.chatcore.pcw.Common.Enums {
/**
 * 码表类型类型
 * 注意：如果新增加了枚举，必须在TimestampType.java中也添加对应的枚举用于同步数据。
 */
public enum MasterType {
    //Region             /**地区*/
    location,

    Sexuality,          /**性别*/

    post               /**职务*/
}
}
