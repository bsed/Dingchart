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
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// UnReadControl.xaml 的交互逻辑
/// </summary>
public partial class NewMessageControl : UserControl {
    public NewMessageControl() {
        InitializeComponent();
    }

    private string text;

    public string Text {
        get {
            return text;
        } set {
            text = value;
            textContent.Text = text;
        }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {

    }

    private void x_Click(object sender, RoutedEventArgs e) {
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        //Businessdata.data = Constants.PUBLIC_ACCOUNT_FLAG;
        Businessdata.eventDataType = BusinessEventDataType.NewMessageControlClose;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        //Businessdata.data = Constants.PUBLIC_ACCOUNT_FLAG;
        Businessdata.eventDataType = BusinessEventDataType.ClickNewMessageControl;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }
}
}
