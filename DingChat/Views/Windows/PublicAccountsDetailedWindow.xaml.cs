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
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Views.Page.PublicAccounts;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// PublicAccountsDetailedWindow.xaml 的交互逻辑
/// </summary>
public partial class PublicAccountsDetailedWindow : Window {
    public PublicAccountsDetailedWindow() {
        InitializeComponent();
    }

    public string tenantNo = string.Empty;
    private string appId;
    public string AppId {
        get {
            return appId;
        } set {
            appId = value;
            try {
                PublicAccountsDetailedPage page =  PublicAccountsDetailedPage.getInstance();
                page.tenantNo = tenantNo;
                page.AppId = AppId;
                frame.Navigate(page);
            } catch (Exception ex) {
                Log.Error(typeof(PublicAccountsDetailedWindow), ex);
            }
        }
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) {
        this.Close();
    }
    private void Window_Loaded(object sender, RoutedEventArgs e) {

    }
}
}
