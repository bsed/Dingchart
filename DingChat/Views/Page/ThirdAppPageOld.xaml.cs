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

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// Window2.xaml 的交互逻辑
/// </summary>
public partial class ThirdAppPageOld : System.Windows.Controls.Page {

    private static ThirdAppPageOld instance = null;
    public static ThirdAppPageOld getInstance() {
        if (instance == null) {
            instance = new ThirdAppPageOld();
        }
        return instance;
    }

    public ThirdAppPageOld() {
        InitializeComponent();
        try {
            //_getqlWorker.DoWork += GetqlWorkerDoWork;
            //_getqlWorker.RunWorkerCompleted += GetqlWorkerRunWorkerCompleted;


            //p.ClickOrgMember += p_ClickOrgMember;
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }

    }

    // 变量定义

    //选中树 取出选中的orgid
    int  _selectedOrganizationId = 0;




    //加载联系人
    Thread lxrThread = null;

    //加载树
    Thread orgThread = null;


    /// <summary>
    /// 业务事件监听
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            // app 加载成功
            case BusinessEventDataType.PublicWebRequestEvent:
                PublicWebRequestEvent(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private void PublicWebRequestEvent(BusinessEvent<object> data) {


        this.Dispatcher.BeginInvoke((Action)delegate() {
            Init();
        });

    }






    /// <summary>
    /// TODO：这个不知道咋描述啊
    /// </summary>
    public void DoEvents() {
        try {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(delegate(object f) {
                ((DispatcherFrame)f).Continue = false;

                return null;
            }
                                                   ), frame);
            Dispatcher.PushFrame(frame);
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }




    private void Init() {
        try {
            ListView.Items.Clear();
            List<ThirdAppGroupTable> groupList = ThirdAppGroupAndClassService.getInstance().FindAllThirdAppGroup(App.CurrentTenantNo);
            foreach (ThirdAppGroupTable group in groupList) {

                List<ThirdAppClassTable> classList = ThirdAppGroupAndClassService.getInstance().FindAllThirdAppClassByGroupId(group.thirdAppGroupId.ToStr(), App.CurrentTenantNo);

                if (classList.Count > 0) {
                    //group
                    //Rectangle separator = new Rectangle();
                    //separator.Fill = (SolidColorBrush) this.FindResource("BackGroundCC");
                    //separator.HorizontalAlignment = HorizontalAlignment.Stretch;
                    //separator.Height = 10;
                    //separator.Width = 265;
                    //ListView.Items.Add(separator);
                }
                foreach (ThirdAppClassTable classDt in classList) {

                    List<PublicWebTable> appList = PublicWebService.getInstance().FindAllAppList(classDt.thirdAppClassId.ToStr().ToInt(), App.CurrentTenantNo);
                    // class
                    if (appList.Count > 0) {
                        StackPanel stackClass = new StackPanel();
                        stackClass.Width = 210;
                        stackClass.Height = 30;
                        stackClass.HorizontalAlignment = HorizontalAlignment.Stretch;
                        stackClass.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e0e0e0"));

                        Label lable = new Label();
                        lable.FontSize = 14;
                        lable.Foreground = (SolidColorBrush)this.FindResource("foreground33");
                        lable.Margin = new Thickness(12, 0, 0, 0);
                        lable.Content = classDt.name;
                        stackClass.Children.Add(lable);
                        ListView.Items.Add(stackClass);
                    }
                    foreach (PublicWebTable app in appList) {
                        AppControl control = new AppControl();
                        control.AppId = app.appId;
                        control.ToolTip = app.name;
                        ListView.Items.Add(control);
                        DoEvents();
                    }

                }
            }

        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        if (App.AppLoadOk) {
            Init();
        }
        Init();
        HiddenOrVisiableGrid(Visibility.Visible);
        ContactsImagePage page = new ContactsImagePage();
        page.App = true;
        FrameApp.Navigate(page);

        //获取GridSplitterr的cotrolTemplate中的按钮btn，必须在Loaded之后才能获取到
        Button btnGrdSplitter = gsSplitterr.Template.FindName("btnExpend", gsSplitterr) as Button;
        if (btnGrdSplitter != null)
            btnGrdSplitter.Click += new RoutedEventHandler(btnGrdSplitter_Click);
    }

    void btnGrdSplitter_Click(object sender, RoutedEventArgs e) {
        GridLength temp = GridMain.ColumnDefinitions[0].Width;
        GridLength def = new GridLength(GridMain.ColumnDefinitions[0].MinWidth,
                                        GridUnitType.Pixel);
        //double def = GridMain.ColumnDefinitions[0].MinWidth;
        if (temp.Equals(def)) {
            //恢复
            HiddenOrVisiableGrid(Visibility.Visible);
        } else {
            //折叠
            HiddenOrVisiableGrid(Visibility.Collapsed);
        }
    }

    private void HiddenOrVisiableGrid(Visibility visiable) {

        if (visiable == Visibility.Collapsed) {
            //if (App.ClickApp == false) return;
            GridMain.ColumnDefinitions[0].Width = new GridLength(GridMain.ColumnDefinitions[0].MinWidth,
                    GridUnitType.Pixel);
        } else {
            GridMain.ColumnDefinitions[0].Width = new GridLength(GridMain.ColumnDefinitions[0].MaxWidth,
                    GridUnitType.Pixel);
        }


        foreach (UIElement element in ListView.Items) {
            var lable =
                CommonMethod.GetVisualChild<Label>(element);
            if (lable != null) {
                lable.Visibility = visiable;
            }
        }


    }


    /// <summary>
    /// 鼠标进入事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ScrollViewerBook_MouseEnter(object sender, MouseEventArgs e) {
        try {
            //if (scrollViewer1.Visibility == Visibility.Visible) return;
            ScrollViewerBook.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            //GridMain.ColumnDefinitions[0].Width = new GridLength(GridMain.ColumnDefinitions[0].MaxWidth, GridUnitType.Pixel);

        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 鼠标离开事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ScrollViewerBook_MouseLeave(object sender, MouseEventArgs e) {
        try {
            //HiddenOrVisiableGrid(Visibility.Collapsed);
            ScrollViewerBook.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            if (ListView.SelectedIndex == -1) return;
            App.ClickApp = true;
            for (int i = 0; i < ListView.Items.Count; i++) {
                AppControl control = ListView.Items[i] as AppControl;
                if (control == null) continue;
                control.IsChecked = false;
            }

            object o = ListView.SelectedItem;
            if (o == null)
                return;
            AppControl p = o as AppControl;
            p.IsChecked = true;

            WebbrowserPage page = WebbrowserPage.getInstance();
            page.Url = p.Url;
            FrameApp. Navigate(page);


        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }


}
}
