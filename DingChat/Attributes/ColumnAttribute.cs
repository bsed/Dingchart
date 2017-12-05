using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
class ColumnAttribute : Attribute {
    public ColumnAttribute() {
    }

    private String m_columnName = String.Empty;

    public String ColumnName {
        get {
            return m_columnName;
        } set {
            m_columnName = value;
        }
    }
}
}
