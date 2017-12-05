using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Models {
/// <summary>
/// API请求错误信息
/// </summary>
public class RestRequestError {
    // 错误编号
    public string errcode {
        get;
        set;
    }
    // 错误信息
    public string errmsg {
        get;
        set;
    }
}
}
