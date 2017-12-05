using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using cn.lds.chatcore.pcw.Common.Enums;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Interop;
using cn.lds.chatcore.capture;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Views.Windows;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Hotkey;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using cn.lds.chatcore.pcw.Common.ServicesHelper;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using cn.lds.chatcore.pcw.HtmlAdapter;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Views.Adorners;
using Hardcodet.Wpf.TaskbarNotification;
using Cursors = System.Windows.Input.Cursors;
using Message = cn.lds.chatcore.pcw.imtp.message.Message;

namespace cn.lds.chatcore.pcw.Views {
/// <summary>
/// Win_Utilities.xaml 的交互逻辑
/// </summary>
public partial class PcStart : Window, HotkeyCallback {
    public String MainProcessName {
        set;
        get;
    }
    private System.Timers.Timer notificationTimer;

    private TaskbarIcon notifyIcon = null;

    private ObservableCollection<ButtonImageCollection> myListViewItems = new ObservableCollection<ButtonImageCollection>();
    public ObservableCollection<ButtonImageCollection> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }
    private Icon[] icons = {
        new Icon(Environment.CurrentDirectory +System.IO.Path.DirectorySeparatorChar +  @"logo16X16.ico"),
        new Icon(Environment.CurrentDirectory +System.IO.Path.DirectorySeparatorChar +  @"logo16X16_2.ico")
        //new Icon(Assembly.GetEntryAssembly().GetManifestResourceStream(@"cn.lds.chatcore.pcw.logo16X16.ico")),
        //new Icon(Assembly.GetEntryAssembly().GetManifestResourceStream(@"cn.lds.chatcore.pcw.logo16X16_2.ico"))
    };

    #region 热键定义部分

    int altd = HotKey.GlobalAddAtom("Alt-D");
    int esc = HotKey.GlobalAddAtom("Esc");
    private KeyHook keyhook = null;
    private HotKey.KeyModifiers altKey = HotKey.KeyModifiers.Alt;
    private HotKey.KeyModifiers escKey = HotKey.KeyModifiers.None;
    private int keyD = (int) System.Windows.Forms.Keys.D;
    private int keyExc = (int) System.Windows.Forms.Keys.Escape;

    #endregion

    private FrmCapture m_frmCapture;
    private AdornerLayer adornerLayer = null;
    private WinStateAdorner winStateAdorner = null;

    private AdornerLayer adornerLayerRight = null;
    private WaitAdorner waitAdorner = null;
    private WebPageWindow win;
    public ICaptureCallback captureCallback;
    private void LoadNotify() {
        notifyIcon = new TaskbarIcon();
        notifyIcon.Visibility = Visibility.Visible;
        notifyIcon.ToolTipText = this.FindResource("system_common_pageTitle").ToStr();
        notifyIcon.MenuActivation = PopupActivationMode.RightClick;
        notifyIcon.ContextMenu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifyIconMenu");
        //this.notifyIcon = new NotifyIcon();
        //this.notifyIcon.BalloonTipText = "鼎信";
        //this.notifyIcon.ShowBalloonTip(2000);
        //this.notifyIcon.Text = "鼎信";
        //this.notifyIcon.Icon = icons[0];
        //this.notifyIcon.Visible = true;
        notifyIcon.TrayMouseDoubleClick += NotifyIcon_Click;
        ////打开菜单项
        //System.Windows.Forms.ToolStripMenuItem open = new System.Windows.Forms.ToolStripMenuItem("打开");
        //open.DisplayStyle = ToolStripItemDisplayStyle.Text;
        //open.BackColor = System.Drawing.Color.White;
        ////open.Click += NotifyIcon_Click;
        ////退出菜单项
        //System.Windows.Forms.ToolStripMenuItem exit = new System.Windows.Forms.ToolStripMenuItem("退出");
        //exit.DisplayStyle = ToolStripItemDisplayStyle.Text;
        //exit.BackColor = System.Drawing.Color.White;
        ////exit.Click += (object sender, EventArgs e) => {
        ////    x_Click(sender, null);
        ////};
        ////关联托盘控件
        //System.Windows.Forms.ToolStripItem[] childen = new System.Windows.Forms.ToolStripMenuItem[] { open, exit };
        //notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
        //notifyIcon.ContextMenuStrip.Items.AddRange(childen);
        this.notifyIcon.Icon = icons[0];
        notificationTimer = new System.Timers.Timer();
        notificationTimer.Interval = 500;
        notificationTimer.Enabled = false;
        notificationTimer.Elapsed += NotificationTimer_Tick;

    }

    private System.Windows.Controls.ContextMenu NotifyIconMenu = null;
    private void NotifyIcon_Click(object sender, EventArgs e) {
        System.Windows.Forms.MouseEventArgs mouseArgs = e as System.Windows.Forms.MouseEventArgs;
        // 如果是鼠标不是右击，则直接返回
        if (null == mouseArgs || mouseArgs.Button != System.Windows.Forms.MouseButtons.Right) {
            this.ShowInTaskbar = true;
            if (this.WindowState != WindowState.Minimized) {
                this.WindowState = WindowState.Minimized;
            } else {
                this.WindowState = WindowState.Normal;
                this.Visibility = Visibility.Visible;
                this.Activate();
            }
            if (notificationTimer.Enabled) {
                notificationTimer.Enabled = false;
                notifyIcon.Icon = icons[0];
            }
        }
    }

    private void StartCapture() {

        try {
            //var a=  ChatPage.getInstance().Visibility;
            //Log.Error(typeof(PcStart), a.ToStr());
            keyhook.regediterKey(this, this, this.esc, this.escKey, this.keyExc);
            if (m_frmCapture == null || m_frmCapture.IsDisposed)
                m_frmCapture = new FrmCapture();
            m_frmCapture.SetCallback(this.captureCallback);
            m_frmCapture.IsCaptureCursor = true;
            m_frmCapture.Show();

        } catch (Exception ex) {
            Log.Error(typeof (PcStart), ex);
        }

    }

    private void StopCapture() {
        try {
            WindowInteropHelper wih = new WindowInteropHelper(this);
            if (m_frmCapture != null) {
                HotKey.UnregisterHotKey(wih.Handle, this.esc);
                m_frmCapture.Close();
            }

        } catch (Exception ex) {
            Log.Error(typeof (PcStart), ex);
        }
    }

    public void StartCapture(ICaptureCallback obj) {
        try {
            keyhook.regediterKey(this, this, this.esc, this.escKey, this.keyExc);
            if (m_frmCapture == null || m_frmCapture.IsDisposed)

                m_frmCapture = new FrmCapture();
            m_frmCapture.SetCallback(obj);
            m_frmCapture.IsCaptureCursor = true;
            m_frmCapture.Show();

        } catch (Exception ex) {
            Log.Error(typeof (PcStart), ex);
        }

    }



    private PcStart() {
        InitializeComponent();
        this.DataContext = this;

        this.StateChanged += new EventHandler(Window_StateChanged);
        this.LocationChanged += MainWindow_LocationChanged;
        //设置ie browser使用ie10
        RegistryWriter.changeToIE10();
        //设置chromium的字体
        RegistryWriter.changeChromiumFont();

        this.SourceInitialized += new EventHandler(PcStart_SourceInitialized);

    }

    void PcStart_SourceInitialized(object sender, EventArgs e) {
        IntPtr hwnd = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));
    }
    IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
        if (0x0201 == wParam.ToInt32()) {
            ;
        }

        return IntPtr.Zero;
    }



    private static PcStart instance = null;

    public static PcStart getInstance() {
        if (instance == null) {
            instance = new PcStart();
        }
        return instance;
    }



    AppWindow appWindow = AppWindow.getInstance();


    PcOA pcOa = PcOA.getInstance();
    AddressBookPage addressBookPage = AddressBookPage.getInstance();
    WebPage web = WebPage.getInstance();




    private void DoCountOfUnread(BusinessEvent<object> data) {

        this.Dispatcher.BeginInvoke((Action) delegate() {
            int count = data.data.ToStr().ToInt();
            if (listBox.Items.Count == 0) return;
            ButtonImageCollection selected =
                this.listBox.Items[0] as ButtonImageCollection;
            if (selected == null) return;


            if (count > 0) {
                selected.UnRead = true;
            } else {
                selected.UnRead = false;
            }

            //selected.ButtonImage = new BitmapImage(new Uri(imageHaveMessage, UriKind.RelativeOrAbsolute));

            //listBox.Items.Refresh();
        });

    }

/// <summary>
/// 窗口变化
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void MainWindow_LocationChanged(object sender, EventArgs e) {
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.eventDataType = BusinessEventDataType.LocationChanged;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

/// <summary>
/// 窗口变化
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void Window_StateChanged(object sender, EventArgs e) {
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.eventDataType = BusinessEventDataType.LocationChanged;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }

/// <summary>
/// 业务事件监听
/// </summary>
/// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<object> data) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (data.eventDataType) {
            // 关闭打开的网页窗口
            case BusinessEventDataType.LoadingWaitShow:
                DoLoadingWaitShow(data);
                break;
            case BusinessEventDataType.LoadingWaitClose:
                DoLoadingWaitClose(data);
                break;
            // 关闭打开的网页窗口
            case BusinessEventDataType.ClosePopWebPageWindow:
                DoClosePopWebPageWindow(data);
                break;
            // 点击待办，弹出详情
            case BusinessEventDataType.ClickTodoTaskXq:
                DoClickTodoTaskXq(data);
                break;
            // 点击位置消息
            case BusinessEventDataType.ClickLocation:
                DoClickLocation(data);
                break;
            // 点击网盘
            case BusinessEventDataType.ClickDiskFile:
                DoClickDiskFile(data);
                break;
            // 未读消息数
            case BusinessEventDataType.CountOfUnread:
                DoCountOfUnread(data);
                break;
            // 来消息
            case BusinessEventDataType.MessageChangedEvent:
                DoCountOfUnread(data);
                break;
            // 重置会话
            case BusinessEventDataType.RedirectChatSessionEvent:
                DoRedirectChatSessionEvent(data, ContactsTpye.LXR);
                break;
            // 重置群聊会话
            case BusinessEventDataType.RedirectMucChatSessionEvent:
                DoRedirectChatSessionEvent(data, ContactsTpye.QL);
                break;
            // 重置公众号会话
            case BusinessEventDataType.RedirectPublicChatSessionEvent:
                DoRedirectChatSessionEvent(data, ContactsTpye.GZH);
                break;
            case BusinessEventDataType.UserLeaveTenant:
                DoUserLeaveTenant(data);
                break;
            // 修改个人信息后
            case BusinessEventDataType.MyDetailsChangeEvent:
                DoContactsDetailsChangeEvent(data);
                break;
            case BusinessEventDataType.DisposeIcon:
                DoDisposeIcon(data);
                break;


            }
        } catch (Exception ex) {
            Log.Error(typeof (PcStart), ex);
        }
    }

    private void DoLoadingWaitClose(BusinessEvent<object> data) {
        try {
            lock(this) {
                this.Dispatcher.BeginInvoke((Action)delegate () {
                    if (data.data == null || waitAdorner.Tag.Equals(data.data)) {
                        waitAdorner.Visibility = Visibility.Hidden;
                        Console.WriteLine("ssssss"+data.data.ToStr());
                    }

                });
            }


        } catch (Exception ex) {
            Log.Error(typeof(PcStart), ex);
        }
    }

    private void DoLoadingWaitShow(BusinessEvent<object> data) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                waitAdorner.Tag = data.data.ToStr();
                waitAdorner.Visibility = Visibility.Visible;
            }));
        } catch (Exception ex) {
            Log.Error(typeof(PcStart), ex);
        }
    }

    private void DoDisposeIcon(BusinessEvent<object> data) {
        try {
            this.Dispatcher.Invoke(new Action(() => {

                if (notifyIcon == null) return;
                this.notifyIcon.Visibility=Visibility.Hidden;
            }));
        } catch (Exception ex) {
            Log.Error(typeof(PcStart), ex);
        }
    }

    /// <summary>
    /// 修改个人信息后
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsDetailsChangeEvent(BusinessEvent<Object> data) {

        try {
            this.Dispatcher.Invoke(new Action(() => {
                //加载我的头像
                LoadMyHeadAvatar();
            }));
        } catch (Exception ex) {
            Log.Error(typeof(PcStart), ex);
        }
    }
    private void DoUserLeaveTenant(BusinessEvent<object> data) {
        string name = data.data.ToStr();
        this.Dispatcher.BeginInvoke((Action)delegate () {
            if (CommonMessageBox.Msg.Show("您被移出" + name + ",需要立即重启", CommonMessageBox.MsgBtn.OK) ==
                    CommonMessageBox.MsgResult.OK) {
                ApplicationService.getInstance().ReStartApplication(MsgType.UserRestart.ToStr(), "");
            }
        });
    }

    private void DoRedirectChatSessionEvent(BusinessEvent<object> data, ContactsTpye type) {
        string idOrNo = data.data.ToStr();
        this.Dispatcher.BeginInvoke((Action) delegate() {
            pcOa.AddChartUserId = 0;
            pcOa.AddChartGroupNo = string.Empty;
            pcOa.AddPublicAccounts = string.Empty;
            ContactsServices services = new ContactsServices();
            if (type == ContactsTpye.LXR) {

                //要加载的人员
                pcOa.AddChartUserId = idOrNo.ToStr().ToInt();
            } else if (type == ContactsTpye.QL) {
                //要加载的群聊
                pcOa.AddChartGroupNo = idOrNo.ToStr();
            } else if (type == ContactsTpye.GZH) {
                //要加载的公众号
                pcOa.AddPublicAccounts = idOrNo.ToStr();

            }

            ButtonImageCollection item = this.listBox.Items[listBox.SelectedIndex] as ButtonImageCollection;
            if (item == null) return;
            if (item.Name == "消息") {
                pcOa.Refesh(string.Empty,true);
            } else {
                this.listBox.SelectedIndex = 0;
                listBox_MouseButtonClick(null, null);
            }
        });

    }

/// <summary>
/// 监控网络
/// </summary>
/// <param Name="data"></param>
    [EventSubscriber]
    public void OnInternetEvent(FrameEvent<object> data) {
        //return;
        try {
            switch (data.frameEventDataType) {
            case FrameEventDataType.NETWORK_ERROR:

                this.Dispatcher.BeginInvoke((Action) delegate() {
                    ImageError.Visibility = Visibility.Visible;

                });
                break;

            case FrameEventDataType.NETWORK_SUCCESS:
                this.Dispatcher.BeginInvoke((Action) delegate() {
                    ImageError.Visibility = Visibility.Collapsed;

                });

                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof (PcStart), ex);
        }
    }

    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        //try {
        //    // 获取消息类型
        //    Message message = messageArrivedEvent.message;
        //    MsgType msgType = message.getType();
        //    switch (msgType) {
        //    // 用户被禁用
        //    case MsgType.UserDisabled:

        //        DoDisposeIcon(null);
        //        break;
        //    // 用户被手机端强制退出
        //    case MsgType.LoginQuit:
        //        DoDisposeIcon(null);
        //        break;
        //    }
        //} catch (Exception e) {
        //    Log.Error(typeof(ApplicationService), e);
        //}
    }

    /// <summary>
    ///  关闭弹出的网页窗口
    /// </summary>
    /// <param name="data"></param>
    private void DoClosePopWebPageWindow(BusinessEvent<object> data) {

        try {
            this.Dispatcher.BeginInvoke((Action)delegate () {

                if (this.win != null) {
                    this.win.Close();
                }
            });

        } catch (Exception ex) {
            Log.Error(typeof (PcStart), ex);
        }
    }

    private void DoClickTodoTaskXq(BusinessEvent<object> data) {


        TodoTaskContentBean bean = data.data as TodoTaskContentBean;
        if (bean == null) return;
        this.Dispatcher.BeginInvoke((Action)delegate () {

            WebPage page = WebPage.getInstance();
            page.TenantNo = bean.tenantNo;
            page.Url = bean.pcDetailUrl;
            BtnBackToFame.Visibility = Visibility.Visible;
            TextTitle.Text = string.Empty;
            frameMain.Navigate(page);
            topBar.Height = new GridLength(32);
        });



    }

    /// <summary>
    /// 点击位置消息
    /// </summary>
    /// <param name="data"></param>
    private void DoClickLocation(BusinessEvent<object> data) {
        String param = "";
        if (data.data!=null) {
            LocationEventData locationEventData = (LocationEventData)data.data;

            //param = "?scale=" + locationEventData.scale;
            param = "?scale=16";
            param += "&latitude=" + locationEventData.latitude;
            param += "&longitude=" + locationEventData.longitude;
            param += "&address=" + HttpUtility.UrlEncode(locationEventData.address);
            //Console.WriteLine("--------------->"+ param);
        }

        this.Dispatcher.BeginInvoke((Action)delegate () {
            win = new WebPageWindow();
            String url = "file:///"+ App.AppRootPath + @"/SysConfig/Html/BaiduMap.html"+ param;
            win.Url = url;
            win.ObjectForScripting = new TestAdpter();
            win.Topmost = true;
            win.Show();
        });

    }


    /// <summary>
    /// 点击网盘文件消息
    /// </summary>
    /// <param name="data"></param>
    private void DoClickDiskFile(BusinessEvent<object> data) {
        String param = "?" + DateTimeHelper.getTimeStamp();
        if (data.data != null) {
            DiskFileEventData diskFileEventData = (DiskFileEventData)data.data;
        }

        this.Dispatcher.BeginInvoke((Action)delegate () {
            win = new WebPageWindow();
            //String url = "file:///" + App.AppRootPath + @"/SysConfig/Html/Test.html" + param;
            //String url = ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port + "/oa/client/index.html#/networkFile/list.html" + param;
            String url = ProgramSettingHelper.Host + ":" + ProgramSettingHelper.Port + "/oa/client/views/networkFile/list.html" + param;

            win.Url = url;
            win.ObjectForScripting = new DiskAdapter();
            win.Topmost = true;
            win.Show();
        });

    }


    /// <summary>
    /// 根据xml加载左侧的按钮
    /// </summary>
    private void LoadButton() {
        if (listBox.Items.Count > 0) return;
        ProgramSettingHelper.initProgramSettingXmlAllContents();

        string logoImage = App.AppRootPath+ @"/homepage.png";
        //ImageHelper.loadSysImage(logoImage, Logo);
        Logo.Source = new BitmapImage(new Uri(logoImage, UriKind.RelativeOrAbsolute));

        // 加载画面的LOG菜单
        ButtonImageCollection mainSelected = null;

        foreach (ButtonImageCollection buttonImageCollection in ProgramSettingHelper.MainProcessButtonList) {

            if (buttonImageCollection.ISVisible == true) {

                bool flag = myListViewItems.Where(q => q.Name.Equals(buttonImageCollection.Name)).Count() > 0;
                if (flag == false) {
                    myListViewItems.Add(buttonImageCollection);
                }
            }

            if (buttonImageCollection.IsDefualt) {
                mainSelected = buttonImageCollection;
            }

        }

        this.listBox.MouseLeftButtonUp -= listBox_MouseButtonClick;
        this.listBox.MouseLeftButtonUp += listBox_MouseButtonClick;


        if (mainSelected != null) {
            this.listBox.SelectedItem = mainSelected;
        }

    }






    private void Window_Loaded(object sender, RoutedEventArgs e) {

        // 初始未读消息
        adornerLayer = AdornerLayer.GetAdornerLayer(GridMain);
        if (winStateAdorner != null) {
            adornerLayer.Remove(winStateAdorner);
        }
        if (adornerLayer != null) {
            // adornerLayer.Opacity = 1;
            //新消息
            winStateAdorner = new WinStateAdorner(GridMain);
            adornerLayer.Add(winStateAdorner);
            winStateAdorner.Visibility = Visibility.Visible;
        }


        //等待
        adornerLayerRight = AdornerLayer.GetAdornerLayer(TitleBar);
        if (waitAdorner != null) {
            adornerLayerRight.Remove(waitAdorner);
        }
        if (adornerLayerRight != null) {
            waitAdorner = new WaitAdorner(TitleBar);
            adornerLayerRight.Add(waitAdorner);
            waitAdorner.Visibility = Visibility.Hidden;
        }

        //注册事件
        keyhook = new KeyHook();
        keyhook.regediterKey(this, this, this.altd, this.altKey, this.keyD);
        //keyhook.regediterKey(this, this, this.esc, this.escKey, this.keyExc);


        LoadButton();
        //web.Url = HttpHelper.getFullUrl(ProgramSettingHelper.StartUrl);
        //frameMain.Navigate(web);

        Show_MainOnly();

        //加载我的头像
        LoadMyHeadAvatar();


        // 加载系统用基础数据（非关键）
        ApplicationService.getInstance().RequestSystemData();


        listBox_MouseButtonClick(null, null);

        // 发送登录状态
        LoginByBarcodeService.getInstance().sendLoginStatusMessage(PcLoginStatus.Login);

        //       NotifyIconMenu = this.NotifyIconMenuX;//(System.Windows.Controls.ContextMenu)this.FindResource("NotifyIconMenu"); // 这句是查找资源（那里的菜单风格就可以自己写了）

        LoadNotify();

    }








    private void Show_MainOnly() {
        App.ClickApp = false;
        App.SelectChartSessionNo = string.Empty;
        appWindow.Hide();

        WinStateControl.getInstance().__.Visibility = System.Windows.Visibility.Visible;
        BtnBackToFame.Visibility = Visibility.Collapsed;
        //if (this.WindowState == System.Windows.WindowState.Normal) {
        //    this.tomax.Visibility = System.Windows.Visibility.Visible;
        //    this.frommax.Visibility = System.Windows.Visibility.Hidden;
        //} else {
        //    this.frommax.Visibility = System.Windows.Visibility.Visible;
        //    this.tomax.Visibility = System.Windows.Visibility.Hidden;
        //}

    }

    #region 窗口变化


    public void DisposeIcon() {
        if (this.notifyIcon!=null) {
            //this.Dispatcher.BeginInvoke((Action)delegate () {
            this.notifyIcon.Icon.Dispose();
            this.notifyIcon.Dispose();
            this.notifyIcon = null;
            //});
        }
        // this.Close();
    }

    public void x_Click() {

        try {
            DisposeIcon(); // 释放任务栏图标资源
            App.ExitApp();
        } catch (Exception ex) {
            Log.Error(typeof(PcStart), ex);
        }
    }

    public void ___Click() {

        this.WindowState = WindowState.Minimized;
    }

/// <summary>
/// 获得窗口任务栏尺寸
/// </summary>
/// <returns></returns>
    public Size getCurTaskbarSize() {
        int width = 0, height = 0;

        if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0)) {
            //taskbar bottom
            width = Screen.PrimaryScreen.WorkingArea.Width;
            height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X == 0)) {
            //taskbar right
            width = Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.WorkingArea.Width;
            height = Screen.PrimaryScreen.WorkingArea.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                   (Screen.PrimaryScreen.WorkingArea.Y > 0)) {
            //taskbar up
            width = Screen.PrimaryScreen.WorkingArea.Width;
            //height = Screen.PrimaryScreen.WorkingArea.Y;
            height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X > 0)) {
            //taskbar left
            width = Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.WorkingArea.Width;
            height = Screen.PrimaryScreen.WorkingArea.Height;
        }

        return new Size(width, height);
    }


/// <summary>
/// 获得最大化时候窗口坐标
/// </summary>
/// <param Name="windowSize"></param>
/// <returns></returns>
    public Point getLocation(Size windowSize) {
        double xPos = 0, yPos = 0;

        if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0)) {
            //taskbar bottom
            xPos = 0;
            yPos = 0;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X == 0)) {
            //taskbar right
            xPos = 0;
            yPos = 0;
        } else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                   (Screen.PrimaryScreen.WorkingArea.Y > 0)) {
            //taskbar up
            xPos = 0;
            yPos = windowSize.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X > 0)) {
            //taskbar left
            xPos = windowSize.Width;
            yPos = 0;
        }

        return new Point(xPos, yPos);
    }


    private double left = 0;
    private double top = 0;

    public void tomax_Click() {

        left = this.Left;
        top = this.Top;
        //this.WindowState = WindowState.Maximized;
        this.WindowState = System.Windows.WindowState.Normal;
        Rect rc = SystemParameters.WorkArea; //获取工作区大小
        Size curTaskbarSize = getCurTaskbarSize();
        Point p = getLocation(curTaskbarSize);
        System.Drawing.Rectangle taskbarRect = Screen.PrimaryScreen.WorkingArea;
        this.Left = p.X; //设置位置
        this.Top = p.Y;
        this.Width = rc.Width;
        this.Height = rc.Height;
        WinStateControl.getInstance().tomax.Visibility = Visibility.Collapsed;
        WinStateControl.getInstance().frommax.Visibility = Visibility.Visible;

    }

    public void frommax_Click() {
        this.WindowState = System.Windows.WindowState.Normal;
        this.Width = 1000;
        this.Height = 655;
        this.Left = left; //设置位置
        this.Top = top;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        WinStateControl.getInstance().tomax.Visibility = Visibility.Visible;
        WinStateControl.getInstance().frommax.Visibility = Visibility.Collapsed;
    }



    #endregion


    private void LoadMyHeadAvatar() {
        //加载人员信息
        if (App.AccountsModel != null) {
            this.btnUser.Tag = App.AccountsModel.nickname;
            AccountsTable table = AccountsServices.getInstance().findByNo(App.AccountsModel.no);
            if (table != null) {
                ImageHelper.loadAvatar(table.avatarStorageRecordId, Ico);
            }
        } else {
            this.btnUser.Tag = string.Empty;
        }
    }

/// <summary>
/// 列表点击
/// </summary>
    private void listBox_MouseButtonClick(object sender, MouseButtonEventArgs e) {

        if (listBox.SelectedIndex == -1) {
            return;
        }
        LoadMyHeadAvatar();
        ButtonImageCollection item =
            this.listBox.Items[listBox.SelectedIndex] as ButtonImageCollection;
        if (item == null) return;

        if (item.Name != "消息") {
            //清空没有发消息的chartsession
            pcOa.AddChartUserId = 0;
            pcOa.AddChartGroupNo = string.Empty;
            pcOa.AddPublicAccounts = string.Empty;
            topBar.Height = new GridLength(0);
        }
        if (item.Name == "通讯录") {

            ChatPage.getInstance().panelList.Items.Clear();
            Show_MainOnly();

            this.TextTitle.Text = addressBookPage.Title;
            //加载右侧的聊天界面

            frameMain.Navigate(addressBookPage);
            BtnBackToFame.Visibility = Visibility.Collapsed;
            topBar.Height = new GridLength(0);
        } else if (item.Name == "消息") {
            Show_MainOnly();

            this.TextTitle.Text = pcOa.Title;
            //加载右侧的聊天界面

            frameMain.Navigate(pcOa);
            topBar.Height = new GridLength(0);
        } else if (item.Name == "应用") {
            ChatPage.getInstance().panelList.Items.Clear();
            Show_MainOnly();
            topBar.Height = new GridLength(0);
            this.TextTitle.Text = ThirdAppPage.getInstance().Title;
            //加载右侧的聊天界面

            frameMain.Navigate(ThirdAppPage.getInstance());
            // appWindow.HiddenOrVisiableGrid(Visibility.Visible);
            appWindow.Show();
            appWindow.Width = this.FindResource("AppWindowWidth").ToStr().ToInt();

        } else {

            ChatPage.getInstance().panelList.Items.Clear();
            Show_MainOnly();
            ButtonImageCollection selected =
                this.listBox.Items[listBox.SelectedIndex] as ButtonImageCollection;

            if (selected == null) return;


            String processUrl = selected.Url;
            this.TextTitle.Text = selected.Name;

            web.Url = HttpHelper.getFullUrl(processUrl);
            frameMain.Navigate(web);
            //resetNaviCount(ProgramSettingHelper.StartUrl);
        }

    }




/// <summary>
/// 点击人员
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void btnUser_Click(object sender, RoutedEventArgs e) {

        listBox.SelectedIndex = -1;


        Show_MainOnly();


        this.TextTitle.Text = pcOa.Title;
        //加载右侧的聊天界面
        AccountPage accountPage = new AccountPage();
        frameMain.Navigate(accountPage);
        BtnBackToFame.Visibility = Visibility.Collapsed;
        TextTitle.Text = "帐号";
        topBar.Height = new GridLength(0);
    }




/// <summary>
///返回
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void BtnBackToPage(object sender, RoutedEventArgs e) {
        BtnBackToFame.Visibility = Visibility.Collapsed;
        WebPage.getInstance();
        try {
            if (frameMain.NavigationService.CanGoBack) {
                frameMain.NavigationService.GoBack();
                TextTitle.Text = "消息";
                topBar.Height = new GridLength(0);
            }
        } catch (Exception ex) {
            Log.Error(typeof (PcOA), ex);
        }
    }


    public void KeyCallBack(int key) {
        if (key == this.esc) {
            this.StopCapture();
        }
        if (key == altd) {
            StartCapture();
        }
    }

    public void CaptureCallback(Bitmap bitmap) {
        //截图程序的回调，如果当前窗口有发送消息的控件，则应该吧bitmap加入richtext中
        //Console.WriteLine("==================截图完成，回调");
    }

    public void StartTaskBarFlash() {
        this.Dispatcher.Invoke(() => {
            if (!this.IsActive) {
                if (this.notifyIcon != null)  WindowExtensions.FlashWindow(this, 5, 500);
            }
        });
    }

    public void StartNotifyIconFlash() {
        this.Dispatcher.Invoke(() => {
            if(!this.IsActive) {
                if (this.notifyIcon != null ) this.notificationTimer.Enabled = true;
            }
        });
    }

    private void NotificationTimer_Tick(object sender, EventArgs e) {
        if (notifyIcon.Icon.Equals(icons[0])) {
            this.notifyIcon.Icon = icons[1];
        } else {
            this.notifyIcon.Icon = icons[0];
        }
    }

    private void PcStart_OnActivated(object sender, EventArgs e) {
        if (this.IsActive) {
            if (this.notificationTimer != null) {
                if (notificationTimer.Enabled) {
                    notificationTimer.Enabled = false; // 停止闪烁通知区域图标
                    notifyIcon.Icon = icons[0];
                }
                WindowExtensions.StopFlashingWindow(this); // 停止闪烁任务栏
            }
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        this.ShowInTaskbar = false;
        this.WindowState = WindowState.Minimized;
        e.Cancel = true;
    }

    private void MenuOpen_OnClick(object sender, RoutedEventArgs e) {
        this.NotifyIcon_Click(sender, null);
    }

    private void MenuExit_OnClick(object sender, RoutedEventArgs e) {
        this.x_Click();
    }

}
}
