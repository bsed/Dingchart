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
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// EditGroupName.xaml 的交互逻辑
/// </summary>
public partial class EditGroupNotice : Window {
    public EditGroupNotice() {
        InitializeComponent();
    }


    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {
            TextBox txt = CommonMethod.GetChildObject<TextBox>(this, "txt");
            if (txt == null) return;
            txt.Height=82;
            txt.Margin = new Thickness(10, 10, 10, 10);

            Button btnOk = CommonMethod.GetChildObject<Button>(this, "OkButton");
            if (btnOk == null) return;
            btnOk.Click += btnOk_Click;
        } catch (Exception ex) {
            Log.Error(typeof(EditGroupNotice), ex);
        }
    }

    /// <summary>
    /// 确定按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    void btnOk_Click(object sender, RoutedEventArgs e) {
        try {
            //CommonMessageBox.Msg.Show("保存成功", CommonMessageBox.MsgBtn.OK);
            NotificationHelper.ShowSuccessMessage("保存成功！");
        } catch (Exception ex) {
            Log.Error(typeof(EditGroupNotice), ex);
        }
    }
}
}
