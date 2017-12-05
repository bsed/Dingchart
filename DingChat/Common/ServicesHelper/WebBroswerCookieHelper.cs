using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using cn.lds.chatcore.pcw.Common.Enums;
using System.Reflection;

namespace cn.lds.chatcore.pcw.Common.ServicesHelper {

class WebBroswerCookieHelper {

    [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

    private static WebBroswerCookieHelper webBroswerCookieHelper = null;

    private static string tenantNoCurrent = string.Empty;
    public static WebBroswerCookieHelper getInstance(String httpHost,string tenantNo) {
        if (webBroswerCookieHelper == null) {
            webBroswerCookieHelper = new WebBroswerCookieHelper();
        }
        tenantNoCurrent = tenantNo;
        webBroswerCookieHelper.setCookie(httpHost);
        return webBroswerCookieHelper;
    }

    //url为目标的域名+端口，如：http://192.168.0.10:8787
    private void setCookie(String url) {
        CookedWebClient client = CookedWebClient.getInstance();
        Uri uri = new Uri(url);
        CookieCollection cookies = client.Cookies.GetCookies(uri);
        for (int i = 0; i < cookies.Count; i++) {
            Cookie c = cookies[i];
            if(tenantNoCurrent!=string.Empty&&c.Name== "tenantNo") {
                InternetSetCookie(url, c.Name, tenantNoCurrent);
            } else {
                InternetSetCookie(url, c.Name, c.Value);
            }

        }
    }
}




//public class CookedWebClient : WebClient {
//    private static CookedWebClient cookieClient = null;
//    private static CookieContainer cookies = App.CookieContainer;
//    public CookieContainer Cookies {
//        get {
//            return cookies;
//        }
//    }

//    public static  CookedWebClient getInstance() {
//        if (cookieClient == null) {
//            cookies = App.CookieContainer;
//            cookieClient = new CookedWebClient();
//        }
//        return cookieClient;
//    }


//    protected override WebRequest GetWebRequest(Uri address) {
//        WebRequest request = base.GetWebRequest(address);
//        if (request.GetType() == typeof(HttpWebRequest))
//            ((HttpWebRequest)request).CookieContainer = cookies;
//        return request;
//    }
//}

}
