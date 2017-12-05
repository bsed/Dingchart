
using System.Windows;
using System.Windows.Controls;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;
using System;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// EditGroupName.xaml 的交互逻辑
/// </summary>
public partial class EditGender : Window {
    public EditGender() {
        InitializeComponent();
    }
    //变量定义
    public int ClientuserId=0;
    private TextBox txt = null;

    /// <summary>
    /// 窗体加载处理
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {
            Button btnOk = CommonMethod.GetChildObject<Button>(this, "OkButton");
            if (btnOk == null) return;
            btnOk.Click += btnOk_Click;

            VcardsTable model = VcardService.getInstance().findByClientuserId(ClientuserId);
            if (model != null) {
                string sex = GetDescription.description((Sex)Enum.Parse(typeof(Sex), model.gender.ToStr()));
                if (sex=="男") {
                    RadioM.IsChecked = true;
                } else if (sex == "女") {
                    RadioW.IsChecked = true;
                }
                if (sex == "未知") {
                    RadioN.IsChecked = true;
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(EditGender), ex);
        }
    }

    /// <summary>
    /// 确定按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    void btnOk_Click(object sender, RoutedEventArgs e) {
        try {
            string sex = string.Empty;
            if (RadioM.IsChecked==true) {
                sex =Sex.male.ToStr();
            } else if (RadioW.IsChecked == true) {
                sex = Sex.female.ToStr();
            } else if (RadioN.IsChecked == true) {
                sex = Sex.unknown.ToStr();
            } else {
                return;
            }

            ContactsApi.chanGegender(sex);
            //CommonMessageBox.Msg.Show("保存成功", CommonMessageBox.MsgBtn.OK);
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(EditGender), ex);
        }
    }
}
}
