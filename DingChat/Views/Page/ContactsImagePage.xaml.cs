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

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// ContactsImagePage.xaml 的交互逻辑
/// </summary>
public partial class ContactsImagePage : System.Windows.Controls.Page {
    public ContactsImagePage() {
        InitializeComponent();
    }

    public bool _app = false;

    public bool _contacts=false;

    public bool _message=false;

    private bool _account = false;
    public bool Contacts {
        get {
            return _contacts;
        } set {
            _contacts = value;
            _message = false;
            _app = false;
        }
    }

    public bool Message {
        get {
            return _message;
        } set {
            _message = value;
            _contacts = false;
            _app = false;
        }
    }

    public bool App {
        get {
            return _app;
        } set {
            _app = value;
            _message = false;
            _contacts = false;
            _app = true;
        }
    }

    public bool Account {
        get {
            return _account;
        } set {
            _account = value;
            _message = false;
            _contacts = false;
            _app = false;
        }
    }


    private void Page_Loaded(object sender, RoutedEventArgs e) {
        GridMain.RowDefinitions[0].Height = (GridLength)this.FindResource("TitleHeight");
        if (_contacts == true) {
            ImageContact.Visibility = Visibility.Visible;
            ImageMessage.Visibility = Visibility.Collapsed;
            ImageApp.Visibility = Visibility.Collapsed;
            title.Text = "通讯录";
        } else if (_message == true) {
            ImageContact.Visibility = Visibility.Collapsed;
            ImageMessage.Visibility = Visibility.Visible;
            ImageApp.Visibility = Visibility.Collapsed;
            title.Text = "消息";
        } else if (_app == true) {
            ImageApp.Visibility =Visibility.Visible;
            ImageContact.Visibility = Visibility.Collapsed;
            ImageMessage.Visibility = Visibility.Collapsed;
            title.Text = "应用";
            GridMain.RowDefinitions[0].Height = new GridLength(0);
        } else if (_account == true) {
            ImageApp.Visibility = Visibility.Collapsed;
            ImageContact.Visibility = Visibility.Collapsed;
            ImageMessage.Visibility = Visibility.Collapsed;
            ImageAccount.Visibility = Visibility.Visible;
            title.Text = "";
        }
    }
}
}
