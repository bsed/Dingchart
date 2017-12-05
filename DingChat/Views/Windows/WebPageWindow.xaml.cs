using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Views.Page;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// WebPageWindow.xaml 的交互逻辑
/// </summary>
public partial class WebPageWindow : Window {
    public WebPageWindow() {
        InitializeComponent();


    }
    /// <summary>
    /// res://mshtml.dll/blank.htm
    /// </summary>

    WebPage web = WebPage.getInstance();
    public string TenantNo = string.Empty;
    public string Url = string.Empty;
    // 如果需要网页调用c#的方法，则设置该属性。JS调用方法为：window.external.JsCallSharp("参数1", "参数2");
    // 参考示例代码：BaiduMap.html,WebAdapter.cs
    public Object ObjectForScripting = null;
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        web.SetObjectForScripting(ObjectForScripting);
        web.TenantNo = TenantNo;
        web.Url = Url;
        Frame.Navigate(web);
        //myWeb.Navigate(Url);

    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        WebPage.getInstance();
    }
}
}
