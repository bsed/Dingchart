using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.Enums {
public enum LoginStatus {
    // 初次登录
    first_time_login,
    // 等登陆
    waitting_login,
    // 等待手机确认
    waitting_mobile_login,
    // 登录中
    login_ing,
    // 登录完成
    login_complete
}
}
