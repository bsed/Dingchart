using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
/// <summary>
/// 使用API发送消息用
/// </summary>
public class SendMessageBean {
    public String messageId;
    public long time;
    public String fromClientId;
    public String toClientId;
    public int messageType;
    public String message;
}
}
