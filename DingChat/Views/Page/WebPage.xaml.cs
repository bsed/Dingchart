using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Browser;
using cn.lds.chatcore.pcw.Common.ServicesHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Views.Windows;
//using CefSharp;
using mshtml;
using Microsoft.Win32;
using Cursors = System.Windows.Input.Cursors;
using WebBrowser = System.Windows.Controls.WebBrowser;
using cn.lds.chatcore.pcw.Business;
using System.Security.Cryptography.X509Certificates;
using cn.lds.chatcore.pcw.Common.Services;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// WebbrowserPage.xaml 的交互逻辑
/// </summary>
public partial class WebPage : System.Windows.Controls.Page {
    private WebPage() {
        InitializeComponent();
        //WindowsInterop.Hook();
    }
    private static WebPage instance = null;

    public string TenantNo = string.Empty;
    public string Url {
        get {
            return url;
        } set {
            url = value;
            init();
        }
    }
    Thread lxrThread = null;

    private Object ObjectForScripting = null;
    public void SetObjectForScripting(Object obj) {
        this.ObjectForScripting = obj;
    }

    public static WebPage getInstance() {
        if (instance == null) {
            instance = new WebPage();
        }

        instance.webbrowser.Navigate("about:blank");
        instance.SetObjectForScripting(null);
        //var settings = new CefSettings() {
        //    //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
        //    CachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Dingxin\\Cache")
        //};
        //settings.Locale = "zh-CN";

        ////Perform dependency check to make sure all relevant resources are in our output directory.
        //Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        return instance;
    }
    private string url;

    private void init() {

        WebBroswerCookieHelper.getInstance(ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port,TenantNo);
        lxrThread = new Thread(LoadUrl);
        lxrThread.IsBackground = true;
        lxrThread.Start();

    }

    public void LoadUrl() {
        if (url == string.Empty) return;
        this.Dispatcher.BeginInvoke((Action)delegate() {

            //获取地址，然后打开
            UrlAddress address = UrlRouter.getURL(this.url);
            if (address.isIE) {
                this.openIE(address.url);
            } else {
                this.openChromium(address.url);
            }
        });
    }
    private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error) {
        System.Console.WriteLine("Warning, trust any certificate");
        //为了通过证书验证，总是返回true
        return true;
    }
    private void openIE(String url) {
        webbrowser.Visibility = Visibility.Visible;
        //Chromium.Visibility =  Visibility.Hidden;
        WebBroswerCookieHelper.getInstance(ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port, TenantNo);
        if (this.ObjectForScripting != null) {
            this.webbrowser.ObjectForScripting = this.ObjectForScripting;
        } else {
            this.webbrowser.ObjectForScripting = new WebAdapter();
        }
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        // Subscribe to Event(s) with the WindowsInterop Class
        //WindowsInterop.SecurityAlertDialogWillBeShown +=
        //    new GenericDelegate<Boolean, Boolean>(this.WindowsInterop_SecurityAlertDialogWillBeShown);

        //WindowsInterop.ConnectToDialogWillBeShown +=
        //    new GenericDelegate<String, String, Boolean>(this.WindowsInterop_ConnectToDialogWillBeShown);
        //url = "https://www.baidu.com";
        if(string.IsNullOrEmpty(TenantNo)) {
            TenantNo = App.CurrentTenantNo;
        }

        string header = "Content-Type: application/x-www-form-urlencoded\r\nTenant: "+ TenantNo+"\r\n";
        this.webbrowser.Navigate(new Uri(url), null, null, header);
        // this.webbrowser.Navigate(new Uri(url));
    }

    private bool WindowsInterop_ConnectToDialogWillBeShown(ref string param1, ref string param2) {
        return true;
    }

    private bool WindowsInterop_SecurityAlertDialogWillBeShown(bool param) {
        return true;
    }

    private void openChromium(String url) {
        //webbrowser.Visibility = Visibility.Hidden;
        //Chromium.Visibility = Visibility.Visible;
        ////缺少cookie
        ////var mngr = Cef.GetGlobalCookieManager();
        ////Cookie Ac = new Cookie();
        ////Ac.Name = "<Cookie Name>";
        ////Ac.Value = "<Value>";
        ////mngr.SetCookieAsync(< URL to Navigate >, Ac);
        //Chromium.Address = url;

    }
    private void Page_Loaded(object sender, RoutedEventArgs e) {


    }

    #region IE浏览器相关设置


    private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e) {
        String strUri = (e.Uri == null ? "Null" : e.Uri.ToString());

        Mouse.SetCursor(Cursors.Arrow);

        String strPageUri = (this.webbrowser.Source == null ? "Null" : this.webbrowser.Source.ToString());


        if (strUri.Equals("Null") || strPageUri.Equals("Null")) {
            return;
        }

    }


    private void WebBrowser_Navigated(object sender, NavigationEventArgs e) {
        SetSilent(sender as WebBrowser, true); // make it silent
    }

    [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IOleServiceProvider {
        [PreserveSig]
        int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
    }

    public static void SetSilent(WebBrowser browser, bool silent) {
        if (browser == null)
            throw new ArgumentNullException("browser");

        // get an IWebBrowser2 from the document
        IOleServiceProvider sp = browser.Document as IOleServiceProvider;
        if (sp != null) {
            Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
            Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

            object webBrowser;
            sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
            if (webBrowser != null) {
                webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
            }
        }
    }


    private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e) {
        //Mouse.SetCursor(Cursors.AppStarting);
    }

    #endregion

}


}
