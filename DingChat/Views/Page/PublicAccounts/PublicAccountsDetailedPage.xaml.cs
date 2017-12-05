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
using cn.lds.chatcore.pcw.Beans;
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
using cn.lds.chatcore.pcw.Services.core;

using EventBus;

namespace cn.lds.chatcore.pcw.Views.Page.PublicAccounts {
/// <summary>
/// QlDetailedPage.xaml 的交互逻辑
/// </summary>
public partial class PublicAccountsDetailedPage : System.Windows.Controls.Page {
    public PublicAccountsDetailedPage() {
        InitializeComponent();

        this.DataContext = muc;
    }
    private static PublicAccountsDetailedPage instance = null;
    public static PublicAccountsDetailedPage getInstance() {
        if (instance == null) {
            instance = new PublicAccountsDetailedPage();
        }
        return instance;
    }

    public string tenantNo = string.Empty;

    public PublicAccountsTable muc {
        get;
        set;
    }


    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {

        try {
            switch (data.eventDataType) {

            case BusinessEventDataType.PublicAccountDetailedEvent:
                DoPublicAccountDetailedEvent(data);
                break;
            case BusinessEventDataType.PublicAccountAvaliableEvent:
                this.Dispatcher.Invoke(new Action(() => {
                    BtnSend.Content = "进入公众号";
                }));
                break;
            }

        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsDetailedPage), ex);
        }

    }

    public void Refesh() {

    }

    private void DoPublicAccountDetailedEvent(BusinessEvent<object> data) {
        if (data.data == null) return;
        PublicAccountsBean item = data.data as PublicAccountsBean;
        if (item == null) return;

        this.Dispatcher.BeginInvoke((Action)delegate () {
            muc = new PublicAccountsTable();
            muc.appid = item.appId;
            muc.name = item.name;
            muc.logoPath = ImageHelper.loadAvatarPath(item.logoId.ToStr());
            muc.ownerName = item.ownerName;
            muc.introduction = item.introduction;
            this.DataContext = muc;
            //string imagePath = item.logoId.ToStr();
            //ImageHelper.loadAvatarImageBrush(imagePath, Ico);

            //GroupName.Content = item.name.ToStr();
            //LbCount.Text = item.introduction.ToStr();
        });
    }

    private string appId;
    public string AppId {
        get {
            return appId;
        } set {
            appId = value;
            try {
                muc = PublicAccountsService.getInstance().findByAppId(appId);
                if (muc!=null) {

                    //string imagePath =muc.logoId.ToStr();
                    muc.logoPath= ImageHelper.loadAvatarPath(muc.logoId.ToStr());

                    //GroupName.Content = muc.name.ToStr();
                    //LbCount.Text = muc.introduction.ToStr();
                    BtnSend.Content = "进入公众号";
                    this.DataContext = muc;
                } else {


                    ContactsApi.getServiceAccountDetails(appId, tenantNo);
                    BtnSend.Content = "关注公众号";
                }
            } catch (Exception ex) {
                Log.Error(typeof(PublicAccountsDetailedPage), ex);
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
            //if (BnSendClick != null && e != null) {
            //    BnSendClick.Invoke(appId, ContactsTpye.GZH);
            //}
            if(BtnSend.Content == "关注公众号") {
                //关注的时候需要把公众号的tertentno穿进去
                PublicAccountsService.getInstance().requestTake(appId, tenantNo);
            } else {
                BusinessEvent<object> businessdata = new BusinessEvent<object>();
                businessdata.data = appId;
                businessdata.eventDataType = BusinessEventDataType.RedirectPublicChatSessionEvent;
                EventBusHelper.getInstance().fireEvent(businessdata);
            }


        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsDetailedPage), ex);
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnDel_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认不再关注 " + GroupName.Content + "   ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {

                PublicAccountsTable model = PublicAccountsService.getInstance().findByAppId(appId);
                if(model!=null) {
                    tenantNo = model.tenantNo;
                }

                PublicAccountsService.getInstance().requestCancel(appId,tenantNo);


            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsDetailedPage), ex);
        }
    }
}
}
