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
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// QlDetailedPage.xaml 的交互逻辑
/// </summary>
public partial class QlDetailedPage : System.Windows.Controls.Page {
    public QlDetailedPage() {
        InitializeComponent();
    }

    // 变量定义
    //public event Action<object, ContactsTpye> BnSendClick;
    private string mucId;
    private string no;
    public string No { // the Name property
        get {
            return no;
        } set {
            no = value;
            try {
                MucTable muc = MucServices.getInstance().FindGroupByNo(no);
                if (muc!=null) {
                    mucId = muc.mucId;
                    string imagePath =muc.avatarStorageRecordId.ToStr();

                    //ImageHelper.loadAvatar(imagePath, Ico);
                    ImageHelper.loadAvatarImageBrush(imagePath, Ico);

                    GroupName.Content = muc.name.ToStr();
                    LbCount.Content = muc.count.ToStr()+"人";
                    //telphoneLb.Content = dt.Rows[0]["mobileNumber"].ToStr();
                    //mailLb.Content = dt.Rows[0]["email"].ToStr();

                }
            } catch (Exception ex) {
                Log.Error(typeof(QlDetailedPage), ex);
            }
        }
    }

    /// <summary>
    /// 发消息按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnSend_Click(object sender, RoutedEventArgs e) {
        try {
            BusinessEvent<object> businessdata = new BusinessEvent<object>();
            businessdata.data = no;
            businessdata.eventDataType = BusinessEventDataType.RedirectMucChatSessionEvent;
            EventBusHelper.getInstance().fireEvent(businessdata);

            //    if (BnSendClick != null && e != null) {
            //    BnSendClick.Invoke(no, ContactsTpye.QL);
            //}
        } catch (Exception ex) {
            Log.Error(typeof(QlDetailedPage), ex);
        }
    }

    /// <summary>
    /// 删除通讯录按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnDel_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认把 " + GroupName.Content + " 从通讯录移除  ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {

                MucServices.getInstance().deleteGroup(mucId,no);
                //CommonMessageBox.Msg.Show("删除成功", CommonMessageBox.MsgBtn.OK);
            }
        } catch (Exception ex) {
            Log.Error(typeof(QlDetailedPage), ex);
        }
    }
}
}
