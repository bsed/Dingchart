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
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Services;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// GroupChatDetailed.xaml 的交互逻辑
/// </summary>
public partial class SingleChatDetailedPage : System.Windows.Controls.Page {


    public SingleChatDetailedPage() {
        InitializeComponent();
    }

    //变量定义
    private string ClientuserId = string.Empty;
    public  string userNo = string.Empty;

    /// <summary>
    /// 刷新方法
    /// </summary>
    private void Refesh() {
        try {
            if (ClientuserId == string.Empty) return;
            personGroup.UserNo = userNo;
            SettingsTable table = SettingService.getInstance().get(userNo);

            if (table == null) return;

            ChkChatTop.IsChecked = table.top;
            ChkNoTrouble.IsChecked = table.quiet;

            //MucMembersTable mucMembersTable = MucMembersDao.getInstance().findByClientuserId(table.mucId, App.AccountsModel.clientuserId);
            //if (mucMembersTable != null)
            //{
            //    GroupNickname.Tag = mucMembersTable.nickname;
            //}
        } catch (Exception ex) {
            Log.Error(typeof(SingleChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Page_Loaded(object sender, RoutedEventArgs e) {
        try {
            VcardsTable dt = VcardService.getInstance().findByNo(userNo);
            ClientuserId = dt.clientuserId;
            //BtnBack.Tag = dt.nickname;
            BtnBack.Tag = ContactsServices.getInstance().getContractNameByNo(userNo);
            Refesh();
        } catch (Exception ex) {
            Log.Error(typeof(SingleChatDetailedPage), ex);
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
            SettingService.getInstance().SetTopmost(ClientuserId, check);
        } catch (Exception ex) {
            Log.Error(typeof(SingleChatDetailedPage), ex);
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
            SettingService.getInstance().EnableNoDisturbFriend(ClientuserId, check);
        } catch (Exception ex) {
            Log.Error(typeof(SingleChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 清空聊天记录点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnCleanUp_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认清空聊天记录 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                MessageService.getInstance().clearMessages(userNo);
                NotificationHelper.ShowSuccessMessage("清空聊天记录完成！");
            }
        } catch (Exception ex) {
            Log.Error(typeof(SingleChatDetailedPage), ex);
        }

    }

    public event Action<string> BtnBackOnClick;
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        if (BtnBackOnClick != null && e != null) {

            BtnBackOnClick.Invoke(ClientuserId);
        }
    }

}
}
