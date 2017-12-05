using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.Browser {
class UrlRouter {
    public static UrlAddress getURL(String url) {
        UrlAddress address = new UrlAddress();
        address.url = url;
        address.isIE = true;
        return address;
    }
}
}
