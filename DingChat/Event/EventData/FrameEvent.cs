using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Event.EventData {
public class FrameEvent<T> {
    //public T data { get; set; }

    public FrameEventDataType frameEventDataType {
        get;
        set;
    }
}
}
