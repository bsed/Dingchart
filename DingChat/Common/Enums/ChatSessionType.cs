using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Enums {
/// <summary>
/// 会话类型
/// </summary>
public enum ChatSessionType {
    /** 单聊 */
    CHAT,
    /** 群聊 */
    MUC,
    /** 公众号 */
    PUBLIC,
    /** 系统通知 */
    NOTICE,
    /** 业务消息 */
    BUSINESS_NOTICE,
    /** 待办事项 */
    TODO_TASK,
    /** 应用消息 */
    APPMSG
}
}
