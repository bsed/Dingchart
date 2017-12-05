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

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// AllMucMemberPage.xaml 的交互逻辑
/// </summary>
public partial class AllMucMemberPage : System.Windows.Controls.Page {
    public string MucNo;
    public string Count;
    public AllMucMemberPage() {
        InitializeComponent();
    }
    public event Action<string> BtnBackOnClick;
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        try {
            if (BtnBackOnClick != null && e != null) {

                BtnBackOnClick.Invoke(MucNo);
            }
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }
    private void Page_Loaded_1(object sender, RoutedEventArgs e) {
        PersonGroupControl.FindAllMember = true;
        PersonGroupControl.MucNo = MucNo;
        Titel.Content = Count;
    }
}
}
