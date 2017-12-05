using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Event.EventData {
public class EventData<T> {
    /*API请求成功数据*/
    public T data {
        get;
        set;
    }
    /*API请求结果：成功 or 失败*/
    public EventType eventType {
        get;
        set;
    }
    /*API请求类型*/
    public EventDataType eventDataType {
        get;
        set;
    }
    /*API请求返回的数据*/
    public Dictionary<String, Object> extras {
        get;
        set;
    }
    /*API请求失败信息*/
    public List<RestRequestError> errors {
        get;
        set;
    }
    /*API请求返回的时间戳*/
    public long timestamp {
        get;
        set;
    }

}
}
