using cn.lds.chatcore.pcw.Common;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// LoadingControl.xaml 的交互逻辑
/// </summary>
public partial class LoadingControl : UserControl {
    public LoadingControl() {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 20 });
        } catch (Exception ex) {
            Log.Error(typeof(LoadingControl), ex);
        }
    }


}
}
