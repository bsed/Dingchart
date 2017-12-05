using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;
using EventBus;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// PublicAccountsSettingPage.xaml 的交互逻辑
/// </summary>
public partial class PublicDetailed : Window {
    public PublicDetailed() {
        InitializeComponent();
    }

    private static PublicDetailed _instance = null;

    private PublicCardMessage messageBean = null;

    //public string AppId {
    //    get {
    //        return appId;
    //    } set {
    //        appId = value;
    //        Refesh();
    //    }
    //}

    public PublicCardMessage MessageBean {
        get {
            return messageBean;
        } set {
            messageBean = value;
            Refesh();
        }
    }
    /// <summary>
    ///点击关闭按钮
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnClose_Click(object sender, RoutedEventArgs e) {
        this.Close();
    }

    public static PublicDetailed getInstance() {
        if (_instance == null) {
            _instance = new PublicDetailed();
        }
        return _instance;
    }

    //private string appId = string.Empty;


    /// <summary>
    /// 刷新方法
    /// </summary>
    private void Refesh() {
        try {
            if (messageBean ==null) return;

            PublicAccountsTable muc = PublicAccountsService.getInstance().findByAppId(messageBean.appId);
            if (muc != null) {
                BtnAdd.Visibility = Visibility.Collapsed;
                BtnSend.Visibility = Visibility.Visible;
            } else {
                BtnAdd.Visibility = Visibility.Collapsed;
                // 暂时屏蔽
                //BtnAdd.Visibility = Visibility.Visible;
                BtnSend.Visibility = Visibility.Collapsed;
            }
            string imagePath = messageBean.logoId.ToStr();
            ImageHelper.loadAvatarImageBrush(imagePath, Ico);
            LbName.Content = messageBean.name;
            gnjs.Content = messageBean.introduction.ToStr();
            zhzt.Content = messageBean.ownerName.ToStr();
        } catch (Exception ex) {
            Log.Error(typeof(PublicDetailed), ex);
        }
    }
    /// <summary>
    /// 重写Close,窗口关闭时设置为隐藏。
    /// </summary>
    /// <param Name="e"></param>
    protected override void OnClosing(CancelEventArgs e) {
        try {
            LbName.Content = string.Empty;
            gnjs.Content = string.Empty;
            zhzt.Content = string.Empty;
            Hide();
            e.Cancel = true;
        } catch (Exception ex) {
            Log.Error(typeof(PublicDetailed), ex);
        }
    }
    /// <summary>
    /// 进入公众号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSend_Click(object sender, RoutedEventArgs e) {
        BusinessEvent<object> businessdata = new BusinessEvent<object>();
        businessdata.data = messageBean.appId;
        businessdata.eventDataType = BusinessEventDataType.RedirectPublicChatSessionEvent;
        EventBusHelper.getInstance().fireEvent(businessdata);

        this.Hide();
    }

    /// <summary>
    /// 关注公众号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnAdd_Click(object sender, RoutedEventArgs e) {

        PublicAccountsService.getInstance().requestTake(messageBean.appId, messageBean.tenantNo);
    }

    /// <summary>
    /// 业务事件处理
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEventEvent(BusinessEvent<Object> data) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (data.eventDataType) {
            // 添加好友之后
            case BusinessEventDataType.PublicAccountAvaliableEvent:
                this.Dispatcher.Invoke(new Action(() => {
                    BtnSend.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Collapsed;
                }));
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(PublicDetailed), e);
        }
    }
}
}
