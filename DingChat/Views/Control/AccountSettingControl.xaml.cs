using System;
using System.Collections.Generic;
using System.Data;
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
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// 联系人控件
/// </summary>
public partial class AccountSettingControl : UserControl {

    public AccountSettingControl() {
        InitializeComponent();
    }

    // 变量定义
    private bool isChecked;

    public bool IsChecked {
        get {
            return isChecked;
        } set {
            isChecked = value;
            try {
                if (isChecked) {
                    SolidColorBrush color = (SolidColorBrush)this.FindResource("rowClickBackground");
                    this.Background = color;
                } else {
                    SolidColorBrush color = (SolidColorBrush)this.FindResource("leftBackground");
                    this.Background = color;
                }
            } catch (Exception ex) {
                Log.Error(typeof(AccountSettingControl), ex);
            }
        }
    }



    public string HeadPortrait =App.ImagePathDefault;

    public string Title = string.Empty;

    public void Init() {
        //try {
        //    ImageHelper.loadSysImage(HeadPortrait, Ico);

        //    LbTitle.Content = Title;
        //} catch (Exception ex) {
        //    Log.Error(typeof(AccountSettingControl), ex);
        //}
    }


    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            ImageHelper.loadSysImage(HeadPortrait, Ico);

            LbTitle.Content = Title;
        } catch (Exception ex) {
            Log.Error(typeof(AccountSettingControl), ex);
        }
    }

}
}
