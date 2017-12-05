using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace cn.lds.chatcore.pcw.Common.Enums {
public enum ConnectionStatus {
    /** Client is Connecting **/
    CONNECTING,
    /** Client is Connected **/
    CONNECTED,
    /** Client is Disconnected **/
    DISCONNECTED,
    /** Client is kicked **/
    KICKED
}
}