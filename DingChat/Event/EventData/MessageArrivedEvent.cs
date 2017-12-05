using cn.lds.chatcore.pcw.imtp.message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Event.EventData {
public class MessageArrivedEvent {
    public Message message {
        get;
        set;
    }

    public MessageArrivedEvent(Message message) {
        this.message = message;
    }

}
}
