using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Services;

namespace cn.lds.chatcore.pcw.Common.ServicesHelper {
public class CookedWebClient : WebClient {


    private static CookedWebClient cookieClient = null;
    private static CookieContainer cookies = App.CookieContainer;
    public CookieContainer Cookies {
        get {
            return cookies;
        }
    }

    public static CookedWebClient getInstance() {
        if (cookieClient == null) {
            cookies = App.CookieContainer;
            cookieClient = new CookedWebClient();
        }
        return cookieClient;
    }


    protected override WebRequest GetWebRequest(Uri address) {
        WebRequest request = base.GetWebRequest(address);
        LoginServices.setClientUserInfoToCookie();
        if (request.GetType() == typeof(HttpWebRequest))
            ((HttpWebRequest)request).CookieContainer = cookies;
        return request;
    }
}
}
