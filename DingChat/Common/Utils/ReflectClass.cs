using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class ReflectClass {
    public static T createInstance<T>() where T : new() {
        return new T();
    }
}
}
