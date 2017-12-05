using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Event.EventData {
//public enum FrameEventType
//{
//}

public enum FrameEventDataType {
    /// <summary>
    /// IM 断开
    /// </summary>
    DISCONNECT_FROM_IM,
    /// <summary>
    /// IM 连接
    /// </summary>
    CONNECTED_TO_IM,
    /// <summary>
    /// 网络错误
    /// </summary>
    NETWORK_ERROR,
    /// <summary>
    /// 网络连接成功
    /// </summary>
    NETWORK_SUCCESS,
    /// <summary>
    /// IM 已连接
    /// </summary>
    IM_CONNECTED,
    /// <summary>
    /// IM 连接丢失
    /// </summary>
    IM_CONNECTION_LOST,
    /// <summary>
    /// IM连接错误
    /// </summary>
    IM_CONNECTION_ERROR,
    /// <summary>
    /// 登出系统，用户其他设备登陆
    /// </summary>
    LOGOUT_USER_KICKED,
    /// <summary>
    /// 登出系统，登陆TOKEN过期
    /// </summary>
    LOGOUT_TOKEN_INVALID
}
}
