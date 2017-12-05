using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Event.EventData;

namespace cn.lds.chatcore.pcw.Event {

public class BusinessEvent<T> {
    public T data {
        get;
        set;
    }

    public BusinessEventDataType eventDataType {
        get;
        set;
    }
}
}
