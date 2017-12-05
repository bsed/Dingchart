using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Page;
using EventBus;
using GalaSoft.MvvmLight.Command;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// AppWindow.xaml 的交互逻辑
/// </summary>
public partial class AppWindow : Window {
    private AppWindow() {
        InitializeComponent();
        this.DataContext = this;
    }
    private static AppWindow instance = null;
    public static AppWindow getInstance() {
        if (instance == null) {
            instance = new AppWindow();
        }
        return instance;
    }

    private  ObservableCollection<PublicWebTable> myGzhItems = new ObservableCollection<PublicWebTable>();
    public  ObservableCollection<PublicWebTable> MyGzhItems {
        get {
            return myGzhItems;
        } set {
            myGzhItems = value;
        }
    }


    // 变量定义
    private Boolean BolDoubleAnimationCtrl = false;

    /// <summary>
    /// 业务事件监听
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (data.eventDataType) {
            // app 加载成功
            case BusinessEventDataType.PublicWebRequestEvent:
                PublicWebRequestEvent(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(AppWindow), ex);
        }
    }

    private void PublicWebRequestEvent(BusinessEvent<object> data) {
        this.Dispatcher.BeginInvoke((Action)delegate() {
            // Init();
        });

    }

    public void ClearSelect(ListView list) {
        for (int i = 0; i < list.Items.Count; i++) {
            AppControl control = list.Items[i] as AppControl;
            if (control == null) continue;
            control.IsChecked = false;
        }
    }


    private void ScrollViewerBook_MouseLeave(object sender, MouseEventArgs e) {
        try {

            if (App.ClickApp == true) {
                leave = true;
                HiddenOrVisiableGrid(Visibility.Collapsed);
            }
            ScrollViewerBook.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        } catch (Exception ex) {
            Log.Error(typeof(AppWindow), ex);
        }
    }

    public bool leave = false;
    private void ScrollViewerBook_MouseEnter(object sender, MouseEventArgs e) {
        try {
            leave = false;
            //if (scrollViewer1.Visibility == Visibility.Visible) return;
            ScrollViewerBook.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (App.ClickApp == true) {
                HiddenOrVisiableGrid(Visibility.Visible);
            }



            //GridMain.ColumnDefinitions[0].Width = new GridLength(GridMain.ColumnDefinitions[0].MaxWidth, GridUnitType.Pixel);

        } catch (Exception ex) {
            Log.Error(typeof(AppWindow), ex);
        }
    }


    public void HiddenOrVisiableGrid(Visibility visiable) {


        if (BolDoubleAnimationCtrl) {
            return;
        }
        BolDoubleAnimationCtrl = true;

        double maxWidth = this.FindResource("AppWindowWidth").ToStr().ToInt();
        double minWidth = this.FindResource("AppWindowMinWidth").ToStr().ToInt();
        if (visiable == Visibility.Collapsed) {

            //this.Width = 66;
            DoubleAnimation widthAnimation = new DoubleAnimation(maxWidth, minWidth, new Duration(TimeSpan.FromSeconds(0.5)));

            EventHandler handler = null;
            widthAnimation.Completed += handler =(sender, args) => {
                widthAnimation.Completed -= handler;
                BolDoubleAnimationCtrl = false;


            };

            this.BeginAnimation(Border.WidthProperty, widthAnimation, HandoffBehavior.Compose);

        } else {

            DoubleAnimation widthAnimation = new DoubleAnimation(minWidth, maxWidth, new Duration(TimeSpan.FromSeconds(0.5)));
            EventHandler handler = null;
            widthAnimation.Completed += handler = (sender, args) => {
                widthAnimation.Completed -= handler;
                BolDoubleAnimationCtrl = false;
                if (leave) {
                    HiddenOrVisiableGrid(Visibility.Collapsed);
                }
            };

            this.BeginAnimation(Border.WidthProperty, widthAnimation, HandoffBehavior.Compose);

            //this.Width = 200;
        }


        foreach (UIElement element in ListView.Items) {
            var lable =
                CommonMethod.GetVisualChild<Label>(element);
            if (lable != null) {
                lable.Visibility = visiable;
            }
        }


    }
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
            Log.Error(typeof(AppWindow), ex);
        }
    }



    private void Init() {
        try {
            ListView.Items.Clear();
            // 基础分类
            List<PublicWebTable> baseList = PublicWebService.getInstance().FindAllAppListByKey(App.CurrentTenantNo);
            MyGzhItems.Clear();
            for (int i = 0; i < baseList.Count; i++) {
                //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                bool flag = MyGzhItems.Where(q => q.appId.Equals(baseList[i].appId)).Count() > 0;
                if (flag == false) {

                    ThirdAppGroupTable dt= ThirdAppGroupAndClassService.getInstance().FindByThirdAppKey(baseList[i].appclaasificationKey.ToStr());
                    if(dt!=null) {
                        baseList[i].appclaasificationKeyName = dt.name;
                    }
                    baseList[i].logoPath = ImageHelper.getAvatarPath(baseList[i].logoId.ToStr());


                    MyGzhItems.Add(baseList[i]);
                }


            }
            //    foreach (ThirdAppGroupTable group in groupListBase) {
            //    List<PublicWebTable> baseeAppList = new List<PublicWebTable>();
            //    if (!string.IsNullOrEmpty(group.key)) {
            //        baseeAppList = PublicWebService.getInstance().FindAllAppListByKey(group.key, App.CurrentTenantNo);
            //        if (baseeAppList.Count == 0) continue;
            //        StackPanel stackClass = new StackPanel();
            //        stackClass.Width = 210;
            //        stackClass.Height = 30;
            //        stackClass.Margin = new Thickness(0, -2, 0, -2);
            //        stackClass.HorizontalAlignment = HorizontalAlignment.Stretch;
            //        stackClass.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e0e0e0"));

            //        Label lable = new Label();
            //        lable.FontSize = 14;
            //        lable.Foreground = (SolidColorBrush)this.FindResource("foreground33");
            //        lable.Margin = new Thickness(12, 0, 0, 0);
            //        lable.Content = group.name;
            //        stackClass.Children.Add(lable);
            //        ListViewBase.Items.Add(stackClass);
            //        foreach (PublicWebTable app in baseeAppList) {
            //            AppControl control = new AppControl();
            //            control.AppId = app.appId;
            //            //control.ToolTip = app.Name;
            //            ListViewBase.Items.Add(control);
            //            DoEvents();
            //        }
            //    }
            //}

            //自定义分类
            List<ThirdAppGroupTable> groupList = ThirdAppGroupAndClassService.getInstance().FindAllThirdAppGroup(App.CurrentTenantNo);
            foreach (ThirdAppGroupTable group in groupList) {

                List<PublicWebTable> baseeAppList = new List<PublicWebTable>();
                if (string.IsNullOrEmpty(group.key)) {
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
                            stackClass.Margin = new Thickness(0, -2, 0, -2);
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
                            //control.ToolTip = app.Name;
                            ListView.Items.Add(control);
                            DoEvents();
                        }
                    }
                }
            }

        } catch (Exception ex) {
            Log.Error(typeof(AppWindow), ex);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        //if (App.AppLoadOk) {
        //    Init();
        //}
        ////测试时用 正常时候屏蔽
        Init();
    }

    private void ListViewBase_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        try {
            ListView list = sender as ListView;
            if (list == null) return;
            if (list.SelectedIndex == -1) return;
            App.ClickApp = true;

            ClearSelect(ListView);



            PublicWebTable o = ListViewBase.SelectedItem as PublicWebTable;
            if (o == null)
                return;

            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = o.url;
            Businessdata.eventDataType = BusinessEventDataType.SelectApp;
            EventBusHelper.getInstance().fireEvent(Businessdata);

        } catch (Exception ex) {
            Log.Error(typeof(AppWindow), ex);
        }
    }

    private void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        try {
            ListView list = sender as ListView;
            if (list == null) return;
            if (list.SelectedIndex == -1) return;
            App.ClickApp = true;

            ClearSelect(ListView);
            ListViewBase.SelectedIndex = -1;
            object o = list.SelectedItem;
            if (o == null)
                return;
            AppControl app = o as AppControl;
            if (app == null) return;

            app.IsChecked = true;
        } catch (Exception ex) {
            Log.Error(typeof(AppWindow), ex);
        }
    }
}
}
