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
public partial class CheckPersonControl : UserControl {

    public CheckPersonControl() {
        InitializeComponent();
    }

    private bool isChecked = false;
    public bool IsChecked {
        get {
            return isChecked;
        } set {
            isChecked = value;
        }
    }
    /// <summary>
    /// 变量定义
    /// </summary>
    public bool Del = false;
    public BitmapImage HeadPortraitId ;
    public string Name = string.Empty;
    public string Id = string.Empty;
    public string No = string.Empty;
    //添加事件代理
    public event Action<CheckPersonControl, bool> AX;



    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            //右侧列表
            if (Del == false) {
                ChkButton.Style = (Style)this.FindResource("PersonCheckBoxStyle");
            } else {
                ChkButton.Style = (Style)this.FindResource("PersonDelBoxStyle");
            }
            ChkButton.Content = Name;

            ChkButton.Tag = HeadPortraitId;
            ChkButton.Margin = new Thickness(0);
            this.MouseLeftButtonUp += CheckPersonControl_MouseLeftButtonUp;
        } catch (Exception ex) {
            Log.Error(typeof(CheckPersonControl), ex);
        }
    }

    private void CheckPersonControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        //if(ChkButton.IsChecked==true) {
        //    ChkButton.IsChecked = false;
        //    AX.Invoke(this, false);
        //} else {
        //    ChkButton.IsChecked = true;
        //    AX.Invoke(this, true);
        //}

    }

    /// <summary>
    /// 选择框选中
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkButton_Checked(object sender, RoutedEventArgs e) {
        //左侧列表
        if (Del == false) {
            if (AX != null) {
                AX.Invoke(this, (bool)ChkButton.IsChecked);
            }
        } else {
            if (AX != null) {
                AX.Invoke(this, true);
            }
        }

    }

}
}
