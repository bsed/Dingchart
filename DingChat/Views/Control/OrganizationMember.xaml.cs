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
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// ChatSession.xaml 的交互逻辑
/// </summary>
public partial class OrganizationMember : UserControl {

    public OrganizationMember() {
        InitializeComponent();
    }

    // 变量定义
    public string UserId = string.Empty;
    public string HeadPortrait = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/images/Default_avatar.jpg";
    public string Job = string.Empty;
    public string OperName = string.Empty;
    public string MemberId = string.Empty;
    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            ImageHelper.loadAvatarImageBrush(HeadPortrait, Ico);
            LbJob.Content = Job;
            LbName.Content =OperName ;
        } catch (Exception ex) {
            Log.Error(typeof(OrganizationMember), ex);
        }
    }

}
}
