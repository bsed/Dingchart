using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
class ModelAttribute : Attribute {
    public ModelAttribute() {
    }

    private String m_modelName = String.Empty;

    public String ModelName {
        get {
            return m_modelName;
        } set {
            m_modelName = value;
        }
    }
}
}
