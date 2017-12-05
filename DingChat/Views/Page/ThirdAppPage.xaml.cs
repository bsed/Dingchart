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
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Windows;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// Window2.xaml 的交互逻辑
/// </summary>
public partial class ThirdAppPage : System.Windows.Controls.Page {

    private static ThirdAppPage instance = null;
    public static ThirdAppPage getInstance() {
        if (instance == null) {
            instance = new ThirdAppPage();
        }
        return instance;
    }


    private ThirdAppPage() {
        InitializeComponent();

    }


    /// <summary>
    /// 业务事件监听
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<object> data) {
        try {
            switch (data.eventDataType) {
            //主窗体变化
            case BusinessEventDataType.LocationChanged:
                LocationChanged();
                break;
            //选择app
            case BusinessEventDataType.SelectApp:
                SelectApp(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    private void SelectApp(BusinessEvent<object> data) {
        if (data.data == null) return;
        this.Dispatcher.BeginInvoke((Action)delegate() {
            WebPage page = WebPage.getInstance();
            page.Url = data.data.ToStr();
            FrameApp.Navigate(page);
        });

    }

    private void LocationChanged() {

        this.Dispatcher.BeginInvoke((Action)delegate() {
            if (PresentationSource.FromVisual(this) == null) return;
            appWindow.Left = appWindow.Owner.Left + ((PcStart)appWindow.Owner).leftMenu.ActualWidth + 1;
            appWindow.Top = appWindow.Owner.Top + ((PcStart)appWindow.Owner).topBar.ActualHeight + 1;
            appWindow.Height = this.panel.ActualHeight;
        });

    }


    private AppWindow appWindow = AppWindow.getInstance();


    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {


        appWindow.ShowInTaskbar = false;
        appWindow.Owner = PcStart.getInstance();
        appWindow.Left = appWindow.Owner.Left + ((PcStart)appWindow.Owner).leftMenu.ActualWidth + 2;
        appWindow.Top = appWindow.Owner.Top + ((PcStart)appWindow.Owner).topBar.ActualHeight + 2;

        appWindow.Height = this.panel.ActualHeight;

        appWindow.Show();
        appWindow.HiddenOrVisiableGrid(Visibility.Visible);
        appWindow.Width = this.FindResource("AppWindowWidth").ToStr().ToInt();

        ContactsImagePage page = new ContactsImagePage();
        page.App = true;
        FrameApp.Navigate(page);

    }

    private void panel_SizeChanged(object sender, SizeChangedEventArgs e) {
        LocationChanged();
    }

    private void Page_Unloaded_1(object sender, RoutedEventArgs e) {
        appWindow.leave = false;
        appWindow.ListView.SelectedIndex = -1;
        appWindow.ClearSelect(appWindow.ListView);
        appWindow.ClearSelect(appWindow.ListViewBase);
        appWindow.Hide();

    }











}
}
