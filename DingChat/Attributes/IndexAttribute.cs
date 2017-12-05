using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
class IndexAttribute : Attribute {
    public IndexAttribute() {
    }


    private String m_indexColumnName = String.Empty;

    public String IndexColumnName {
        get {
            return m_indexColumnName;
        } set {
            m_indexColumnName = value;
        }
    }
}
}
