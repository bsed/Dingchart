using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
class ModelItemAttribute : Attribute {
    public ModelItemAttribute() {
    }

    private String m_modelFieldName = String.Empty;

    public String ModelFieldName {
        get {
            return m_modelFieldName;
        } set {
            m_modelFieldName = value;
        }
    }
}
}
