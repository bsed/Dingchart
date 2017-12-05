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

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// WinStateControl.xaml 的交互逻辑
/// </summary>
public partial class WinStateControl : UserControl {
    private WinStateControl() {
        InitializeComponent();
    }

    private static WinStateControl instance = null;

    public static WinStateControl getInstance() {
        if (instance == null) {
            instance = new WinStateControl();
        }
        return instance;
    }

    private void ___Click(object sender, RoutedEventArgs e) {
        PcStart.getInstance().___Click();
    }

    private void tomax_Click(object sender, RoutedEventArgs e) {
        PcStart.getInstance().tomax_Click();
    }

    private void frommax_Click(object sender, RoutedEventArgs e) {
        PcStart.getInstance().frommax_Click();
    }
}
}
