using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services;
using EventBus;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// TenantsWindow.xaml 的交互逻辑
/// </summary>
public partial class TenantsWindow : Window {
    private TenantsWindow() {
        InitializeComponent();
        this.DataContext = this;

        CollectionView cv = new CollectionView(App.TenantListViewItems);
        this.ListBoxLeft.ItemsSource = cv;
    }

    private static TenantsWindow instance = null;

    public static TenantsWindow getInstance() {
        if (instance == null) {
            instance = new TenantsWindow();
        }
        return instance;
    }


    /// <summary>
    /// 监听业务事件
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            case BusinessEventDataType.FileDownloadedEvent:
                DoFileDownloadedEvent(data);
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(TenantsWindow), e);
        }

    }

    /// <summary>
    /// 图片下载完成事件处理
    /// </summary>
    /// <param name="data"></param>
    private void DoFileDownloadedEvent(BusinessEvent<Object> data) {
        try {
            // todo 下载的头像显示需要先登录。
            // 获取下载的头像
            FileEventData fileBean = (FileEventData)data.data;
            foreach(LoginBeanTenants tenant in App.TenantListViewItems) {
                if (fileBean.fileStorageId == tenant.logoID) {
                    this.Dispatcher.Invoke(new Action(() => {
                        tenant.logoPath = ImageHelper.loadAvatarPath(tenant.logoID);
                    }));
                }
            }

        } catch (Exception e) {
            Log.Error(typeof(TenantsWindow), e);
        }
    }


    private void Window_Loaded(object sender, RoutedEventArgs e) {
        App.TenantListViewItems.Clear();
        foreach (var item in App.TenantNoDic) {
            LoginBeanTenants bean = new LoginBeanTenants();
            bean.tenant = item.Key;

            LoginBeanTenants value = item.Value;
            bean.logoID = value.logoID;
            bean.logoPath = ImageHelper.loadAvatarPath(value.logoID);
            bean.shortName = value.shortName;
            bean.name = value.name;
            bean.sortNum = value.sortNum;
            App.TenantListViewItems.Add(bean);
        }

    }

    private void BtnOk_Click(object sender, RoutedEventArgs e) {

        object o = ListBoxLeft.SelectedItem;
        if (o == null)
            return;

        LoginBeanTenants p = o as LoginBeanTenants;
        if (p == null)
            return;
        App.CurrentTenantNo = p.tenant;
        App.CurrentTenantNoLoadOk = true;

        LoginServices.setClientUserInfoToCookie();

        BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
        Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
        EventBusHelper.getInstance().fireEvent(Businessdata);
        this.Close();
    }
}
}
