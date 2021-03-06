﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Attributes

{
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
class ReceivedMessageProcesserAttribute : Attribute {
    String m_name = String.Empty;

    public String Name {
        get {
            return m_name;
        } set {
            m_name = value;
        }
    }

    public ReceivedMessageProcesserAttribute() {
    }
}
}
