using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using cn.lds.chatcore.pcw.Common.ServicesHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using mshtml;


namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// WebbrowserPage.xaml 的交互逻辑
/// </summary>
public partial class WebbrowserPage : System.Windows.Controls.Page {
    private WebbrowserPage() {
        InitializeComponent();
    }
    private static WebbrowserPage instance = null;

    public string Url {
        get {
            return url;
        } set {
            url = value;
            init();
        }
    }
    Thread lxrThread = null;
    public static WebbrowserPage getInstance() {
        if (instance == null) {
            instance = new WebbrowserPage();
        }
        return instance;
    }
    private string url;

    private void init() {


        //web.Url = new Uri(url);

        WebBroswerCookieHelper.getInstance(ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port,"");
        lxrThread = new Thread(new ThreadStart(LoadLxr));
        lxrThread.IsBackground = true;
        lxrThread.Start();

    }

    public void LoadLxr() {

        this.Dispatcher.BeginInvoke((Action)delegate() {
            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost();
            this.web.Navigate(Url);
            host.Child = web;
            web.DocumentCompleted -= webBrowser_DocumentCompleted;
            web.DocumentCompleted += webBrowser_DocumentCompleted;
            web.NewWindow -= webBrowser1_NewWindow;
            web.NewWindow += webBrowser1_NewWindow;
            //设置webBrowser
            web.ScriptErrorsSuppressed = true; //禁用错误脚本提示
            web.AllowWebBrowserDrop = false;//禁止拖拽
            this.grid.Children.Add(host);
        });
    }
    System.Windows.Forms.WebBrowser web = new System.Windows.Forms.WebBrowser();
    private void Page_Loaded(object sender, RoutedEventArgs e) {


    }
    private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
        //将所有的链接的目标，指向本窗体
        foreach (HtmlElement archor in this.web.Document.Links) {
            archor.SetAttribute("target", "_self");
        }


        //将所有的FORM的提交目标，指向本窗体
        foreach (HtmlElement form in this.web.Document.Forms) {
            form.SetAttribute("target", "_self");
        }

    }

    private void webBrowser1_NewWindow(object sender, CancelEventArgs e) {
        e.Cancel = true;
    }



}
}
