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
using System.Windows.Navigation;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.update;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Services;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// AboutPage.xaml 的交互逻辑
/// </summary>
public partial class AboutPage : System.Windows.Controls.Page {
    public AboutPage() {
        InitializeComponent();
    }
    private int doubleClickTimes = 0;
    private void Page_Loaded(object sender, RoutedEventArgs e) {
        try {
            Lbversion.Content = "当前版本：" + Updater.getInstance().CurrentVersion;
        } catch (Exception ex) {
            Log.Error(typeof(AboutPage), ex);
        }
    }

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        try {
            if (e.ClickCount == 2) {
                //双击时执行
                doubleClickTimes++;
                if (doubleClickTimes>2) {
                    String msg = "";
                    NotificationHelper.ShowInfoMessage(ServiceCode.SERVER_HOST);
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(AboutPage), ex);
        }
    }
}
}
