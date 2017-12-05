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
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// EditGroupName.xaml 的交互逻辑
/// </summary>
public partial class CommonMessageBoxWindow : Window {

    //变量定义
    public string MessageBoxText {
        get;
        set;
    }

    public Visibility OKButtonVisibility {
        get;
        set;
    }

    public Visibility CancelButtonVisibility {
        get;
        set;
    }

    public Visibility YesButtonVisibility {
        get;
        set;
    }

    public Visibility NoButtonVisibility {
        get;
        set;
    }

    public CommonMessageBox.MsgResult Result;

    /// <summary>
    /// 构造方法
    /// </summary>
    public CommonMessageBoxWindow() {
        InitializeComponent();
        try {
            this.DataContext = this;

            OKButtonVisibility = System.Windows.Visibility.Collapsed;
            CancelButtonVisibility = System.Windows.Visibility.Collapsed;
            YesButtonVisibility = System.Windows.Visibility.Collapsed;
            NoButtonVisibility = System.Windows.Visibility.Collapsed;

            Result = CommonMessageBox.MsgResult.None;
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }


    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {

        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }

    }

    /// <summary>
    /// “是”按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void YesButton_Click(object sender, RoutedEventArgs e) {
        try {
            Result = CommonMessageBox.MsgResult.Yes;
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }

    /// <summary>
    /// “否”按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void NoButton_Click(object sender, RoutedEventArgs e) {
        try {
            Result = CommonMessageBox.MsgResult.No;
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }

    /// <summary>
    /// “确定”按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void OKButton_Click(object sender, RoutedEventArgs e) {
        try {
            Result = CommonMessageBox.MsgResult.OK;
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }

    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        try {
            Result = CommonMessageBox.MsgResult.Cancel;
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }

    /// <summary>
    /// grid的左键点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        try {
            this.DragMove();
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnClose_Click(object sender, RoutedEventArgs e) {
        try {
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(CommonMessageBoxWindow), ex);
        }
    }
}
}
