using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
class JsonMessageBodyAttribute : Attribute {
    public JsonMessageBodyAttribute() {
    }

    private Int32 m_messageType = -1;

    public Int32 MessageType {
        get {
            return m_messageType;
        } set {
            m_messageType = value;
        }
    }
    private String m_description = "-1";

    public String Description {
        get {
            return m_description;
        } set {
            m_description = value;
        }
    }
}
}
