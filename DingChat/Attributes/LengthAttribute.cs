using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
class LengthAttribute : Attribute {
    int m_Length = 0;

    public int Length {
        get {
            return m_Length;
        } set {
            m_Length = value;
        }
    }

    public LengthAttribute(int length) {
        this.m_Length = length;
    }
}
}
