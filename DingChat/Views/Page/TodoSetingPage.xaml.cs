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
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// TodoSetingPage.xaml 的交互逻辑
/// </summary>
public partial class TodoSetingPage : System.Windows.Controls.Page {
    private  TodoSetingPage() {
        InitializeComponent();
    }
    private static TodoSetingPage instance = null;
    public static TodoSetingPage getInstance() {
        if (instance == null) {
            instance = new TodoSetingPage();
        }
        return instance;
    }
    public ChatSessionType ChatSessionType;

    public event Action<string> BtnBackOnClick;
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        if (BtnBackOnClick != null && e != null) {
            BtnBackOnClick.Invoke("");
        }
    }



    /// <summary>
    /// 置顶
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkChatTop_Checked(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkChatTop.IsChecked.ToStr().ToBool();
            ContactsApi.setGroupTopmost(ChatSessionType.TODO_TASK.ToStr(), check);
        } catch (Exception ex) {
            Log.Error(typeof(TodoSetingPage), ex);
        }


    }
    /// <summary>
    /// 消息免打扰
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkNoTrouble_Checked(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkNoTrouble.IsChecked.ToStr().ToBool();
            ContactsApi.enableGroupNoDisturb(ChatSessionType.TODO_TASK.ToStr(), check);
        } catch (Exception ex) {
            Log.Error(typeof(TodoSetingPage), ex);
        }

    }


    private void Page_Loaded(object sender, RoutedEventArgs e) {
        if (ChatSessionType == ChatSessionType.TODO_TASK) {
            BtnBack.Tag = "待办事项";
        } else if (ChatSessionType == ChatSessionType.APPMSG) {
            BtnBack.Tag = "应用消息";
        }
    }

    private void BtnBack_Click_1(object sender, RoutedEventArgs e) {

    }
}
}
