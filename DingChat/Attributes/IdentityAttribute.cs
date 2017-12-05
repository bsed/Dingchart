using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
class IdentityAttribute : Attribute {
    // 开始值
    int m_StartValue = 0;

    public int StartValue {
        get {
            return m_StartValue;
        } set {
            m_StartValue = value;
        }
    }

    // 每回增加值
    int m_Increase = 0;

    public int Increase {
        get {
            return m_Increase;
        } set {
            m_Increase = value;
        }
    }

    public IdentityAttribute(int startValue, int increase) {
        this.m_StartValue = startValue;
        this.m_Increase = increase;
    }
}
}
