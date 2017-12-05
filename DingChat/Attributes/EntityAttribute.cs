using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
class EntityAttribute : Attribute {
    public EntityAttribute() {
    }

    private String m_TableName = String.Empty;

    public String TableName {
        get {
            return m_TableName;
        } set {
            m_TableName = value;
        }
    }


}
}
