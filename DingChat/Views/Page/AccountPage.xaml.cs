using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// Window2.xaml 的交互逻辑
/// </summary>
public partial class AccountPage : System.Windows.Controls.Page {

    private static AccountPage instance = null;
    public static AccountPage getInstance() {
        if (instance == null) {
            instance = new AccountPage();
        }
        return instance;
    }

    public AccountPage() {
        InitializeComponent();

    }



    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {
            ContactsImagePage contactsImagePage = new Page.ContactsImagePage();
            contactsImagePage.Account = true;
            Frame.Navigate(contactsImagePage);

            AccountSettingControl  grControl = new AccountSettingControl();
            grControl.Title = "个人信息";
            grControl.HeadPortrait = @"Account/AccountInfo.png";
            grControl.Init();
            ListViewLxr.Items.Add(grControl);

            AccountSettingControl yjControl = new AccountSettingControl();
            yjControl.Title = "意见反馈";
            yjControl.HeadPortrait = @"Account/Common.png";
            yjControl.Init();
            ListViewLxr.Items.Add(yjControl);

            AccountSettingControl aboutControl = new AccountSettingControl();
            aboutControl.Title = "关于";
            aboutControl.HeadPortrait = @"Account/About.png";
            aboutControl.Init();
            ListViewLxr.Items.Add(aboutControl);
        } catch (Exception ex) {
            Log.Error(typeof(AccountPage), ex);
        }
    }




    /// <summary>
    /// 选择
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ListViewLxr_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            if (ListViewLxr.SelectedIndex == -1) return;
            for (int i = 0; i < ListViewLxr.Items.Count; i++) {
                AccountSettingControl control = ListViewLxr.Items[i] as AccountSettingControl;
                control.IsChecked = false;
            }


            object o = ListViewLxr.SelectedItem;
            if (o == null)
                return;
            AccountSettingControl p = o as AccountSettingControl;
            p.IsChecked = true;

            if(p.Title=="个人信息") {
                LxrDetailedPage page = LxrDetailedPage.getInstance();
                page.lxrDetailedType = LxrDetailedType.Account;
                page.tenantNo = string.Empty;
                page.memberId = string.Empty;
                page.Id = App.AccountsModel.clientuserId.ToStr().ToInt();
                Frame.Navigate(page);
            } else if (p.Title == "关于") {
                AboutPage page =new  AboutPage();
                Frame.Navigate(page);
            } else if (p.Title == "意见反馈") {
                OpinionPage page =  OpinionPage.getInstance();
                Frame.Navigate(page);
            }

            //lxrDetailedPage = new LxrDetailedPage();
            //lxrDetailedPage.Id = p.UserId;
            //FrameChat.Navigate(lxrDetailedPage);
        } catch (Exception ex) {
            Log.Error(typeof(AccountPage), ex);
        }
    }


    /// <summary>
    /// 鼠标进入事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ScrollViewerBook_MouseEnter(object sender, MouseEventArgs e) {
        try {
            ScrollViewerBook.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        } catch (Exception ex) {
            Log.Error(typeof(AccountPage), ex);
        }
    }

    /// <summary>
    /// 鼠标离开事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ScrollViewerBook_MouseLeave(object sender, MouseEventArgs e) {
        try {
            ScrollViewerBook.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        } catch (Exception ex) {
            Log.Error(typeof(AccountPage), ex);
        }
    }



    private void Button_Click(object sender, RoutedEventArgs e) {
        if (CommonMessageBox.Msg.Show("确定注销帐户 ?", CommonMessageBox.MsgBtn.YesNO) ==
                CommonMessageBox.MsgResult.Yes) {

            ApplicationService.getInstance().ReStartApplication(MsgType.UserRestart.ToStr(), "");
        }


    }


}
}
