using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Event.EventData;
using EventBus;

namespace cn.lds.chatcore.pcw.Event.Publisher {
class EventBusHelper {
    private static EventBusHelper instance = null;
    private SimpleEventBus eventBus = SimpleEventBus.GetDefaultEventBus();
    private EventBusHelper() {

    }

    public static EventBusHelper getInstance() {
        if (instance == null) {
            instance = new EventBusHelper();
        }
        return instance;
    }

    public void register(Object handler) {
        this.eventBus.Register(handler);
    }

    public void unRegister(Object handler) {
        this.eventBus.Deregister(handler);
    }

    public void fireEvent(Object data) {
        this.eventBus.Post(data, TimeSpan.Zero);
    }
}
}
