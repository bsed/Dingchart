using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class NoticeMessageControl : MessageBase {
    public NoticeMessageControl() {
        InitializeComponent();

    }



    //聊天的时间
    public string DateTime = System.DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");

    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {
        this.HorizontalAlignment = HorizontalAlignment.Stretch;


        Label lableD = new Label();

        lableD.FontSize = 12;
        SolidColorBrush color = (SolidColorBrush)this.FindResource("foreground99");
        lableD.Foreground = color;
        lableD.SetValue(Grid.ColumnSpanProperty, 6);
        lableD.SetValue(Grid.RowProperty, 0);
        lableD.SetValue(Grid.ColumnProperty, 0);
        lableD.Content = DateTime;

        lableD.VerticalAlignment = VerticalAlignment.Stretch;
        lableD.HorizontalContentAlignment = HorizontalAlignment.Center;
        lableD.VerticalContentAlignment = VerticalAlignment.Center;
        if (Item.showTimestamp) {
            LayoutRoot.Children.Add(lableD);
        }

        lable.Content = Item.text;
    }









    /// <summary>
    /// 滑轮滚动（解决子控件滑轮不滚动问题）
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void LayoutRoot_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e) {
        var scroll =
            CommonMethod.GetVisualChild<ScrollViewer>(((this.Parent as ListView).Parent as ScrollViewer));
        if (scroll != null) {
            //指定父级ScrollViewer滚动偏移量
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
        }
    }



}
}
