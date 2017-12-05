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
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;

namespace cn.lds.chatcore.pcw.Views.Page.PublicAccounts {
/// <summary>
/// PublicAccountsSettingPage.xaml 的交互逻辑
/// </summary>
public partial class PublicAccountsSettingPage : System.Windows.Controls.Page {
    public PublicAccountsSettingPage() {
        InitializeComponent();
    }
    public string appId = string.Empty;
    public event Action<string> BtnBackOnClick;
    private string tenantNo = string.Empty;
    /// <summary>
    /// 刷新方法
    /// </summary>
    private void Refesh() {
        try {
            if (appId == string.Empty) return;

            SettingsTable table = SettingService.getInstance().get(appId);

            if (table == null) return;

            ChkChatTop.IsChecked = table.top;
            ChkNoTrouble.IsChecked = table.quiet;


        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsSettingPage), ex);
        }
    }
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        if (BtnBackOnClick != null && e != null) {

            BtnBackOnClick.Invoke(appId);
        }
    }
    /// <summary>
    /// 置顶点击处理
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkChatTop_Click(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkChatTop.IsChecked.ToStr().ToBool();
            PublicAccountsService.getInstance().setTopmost(appId, check, tenantNo);
        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsSettingPage), ex);
        }
    }

    private void BtnCleanUp_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认清空记录 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                MessageService.getInstance().clearMessages(appId);
                NotificationHelper.ShowSuccessMessage("清空记录完成！");
            }
        } catch (Exception ex) {
            Log.Error(typeof(SingleChatDetailedPage), ex);
        }
    }

    private void BtnDel_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认不再关注 " + LbName.Content + "   ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                //ssssssssssssssssssssssssssssssssssssssss
                PublicAccountsService.getInstance().requestCancel(appId,"");

                NotificationHelper.ShowSuccessMessage("取消关注成功！");
                //公众号 直接跳到公众号聊天界面
                BusinessEvent<object> Businessdata = new BusinessEvent<object>();
                Businessdata.data = Constants.PUBLIC_ACCOUNT_FLAG;
                Businessdata.eventDataType = BusinessEventDataType.RequestCancelGoBack;
                EventBusHelper.getInstance().fireEvent(Businessdata);
            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsDetailedPage), ex);
        }
    }
    /// <summary>
    /// 免打扰点击处理
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkNoTrouble_Click(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkNoTrouble.IsChecked.ToStr().ToBool();
            PublicAccountsService.getInstance().setQuiet(appId, check, tenantNo);
        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsSettingPage), ex);
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e) {
        try {
            PublicAccountsTable muc = PublicAccountsService.getInstance().findByAppId(appId);
            if (muc != null) {
                tenantNo = muc.tenantNo;
                string imagePath = muc.logoId.ToStr();
                ImageHelper.loadAvatar(imagePath, Ico);
                LbName.Content = muc.name;
                BtnBack.Tag = muc.name.ToStr();
                gnjs.Text = muc.introduction.ToStr();
                zhzt.Text = muc.ownerName.ToStr();
                Refesh();
            }


        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsSettingPage), ex);
        }
    }
}
}
