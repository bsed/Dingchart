using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace cn.lds.chatcore.pcw.Beans.Convertors {
public class convertor<T> : CustomCreationConverter<T> {
    public override T Create(Type objectType) {
        return System.Activator.CreateInstance<T>();

    }
}
}
