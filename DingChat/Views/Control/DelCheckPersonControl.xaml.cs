using System;
using System.Collections.Generic;
using System.IO;
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
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// PersonControl.xaml 的交互逻辑
/// </summary>
public partial class DelCheckPersonControl : UserControl {

    public DelCheckPersonControl() {
        InitializeComponent();
    }

    // 变量定义
    public string HeadPortraitPath = "";
    public string Name = string.Empty;
    //添加事件代理
    public event Action<DelCheckPersonControl, bool> AX;

    /// <summary>
    /// 画面初始化
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            ChkButton.Content = Name;
            ChkButton.Tag = ImageHelper.loadAvatarPath(HeadPortraitPath);
        } catch (Exception ex) {
            Log.Error(typeof(GroupStaffControl), ex);
        }
    }

    /// <summary>
    /// 选择按钮勾选
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkButton_Checked(object sender, RoutedEventArgs e) {
        try {
            if (AX != null) {
                AX.Invoke(this, (bool)ChkButton.IsChecked);
            }
        } catch (Exception ex) {
            Log.Error(typeof(DelCheckPersonControl), ex);
        }
    }


}
}
