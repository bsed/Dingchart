using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Threading;
using cn.lds.chatcore.pcw.Views;
using System.Windows;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using System.IO;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Views.Windows;



using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Page;

using cn.lds.chatcore.pcw.Services.core;
//using CefSharp;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.Views.Page.PublicAccounts;
using cn.lds.chatcore.pcw.Views.Windows.AVMeeting;
using CSCore.SoundOut;

namespace cn.lds.chatcore.pcw {
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {

    /// <summary>
    /// 主窗口进入标志
    /// </summary>
    public static bool mainWindowLoaded = false;


    //新建群选择的人员列表
    public static List<string> listCreatGroupMember = new List<string>();

    public static AVMeetingStatus AmStatus = AVMeetingStatus.no;

    public static AVMeetingCallType AmCallType = AVMeetingCallType.calling;

    //当前播放的语音的id
    public static string soundPlayingId = string.Empty;

    // 当前应用是否获取焦点
    public static bool AppActivated = false;

    // 判断登录是否成功
    public static  bool ContactsLoadOk = false;

    public static bool AppLoadOk = false;


    public static bool GroupsLoadOk = false;

    public static bool OrganizationsLoadOk = false;
    //加载的次数  因为租户多个 需要多次调api  多次都执行完之后才能进入页面
    public static int OrganizationsLoadCount = 0;

    public static bool CacheLoadOk = false;
    public static bool CurrentTenantNoLoadOk = false;
    public static bool ChatSessionLoadOk = false;

    //是否初次登录
    public static bool IsFirstLogin = false;

    //公众号
    public static bool PublicAccountsLoadOk = false;

    public static int PublicAccountsLoadCount = 0;


    public static bool ClickApp= false;

    private static string _appName;
    public static string AppName {
        get {
            if (string.IsNullOrEmpty(_appName)) {
                _appName = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1).Replace(".exe", "");
            }
            return _appName;
        }
    }

    //chartsession 除了正常聊天以外的 chart
    public static int ChartSessionCount = 0;

    /// <summary>
    /// 人员详情画面
    /// </summary>
    public static PersonDetailed PersonDetailedWindow;


    public static CreateGroup createGroup;
    /// <summary>
    /// 登陆后保存cookie
    /// </summary>
    public    static CookieContainer CookieContainer = new CookieContainer();

    /// <summary>
    /// 存储登陆人信息
    /// </summary>
    public static AccountsTable AccountsModel = new AccountsTable();

    public static string CurrentTenantNo = string.Empty;

    private static ObservableCollection<LoginBeanTenants> tenantListViewItems = new ObservableCollection<LoginBeanTenants>();
    public static ObservableCollection<LoginBeanTenants> TenantListViewItems {
        get {
            return tenantListViewItems;
        } set {
            tenantListViewItems = value;
        }
    }
    public static List<string> TenantNoList = new List<string>();

    public static Dictionary<string, LoginBeanTenants> TenantNoDic = new Dictionary<string, LoginBeanTenants>();
    /// <summary>
    /// 当前登录人的数据库路径
    /// </summary>
    public static string DataBasePath;

    /// <summary>
    /// 选择的chartsession的no
    /// </summary>
    public static string SelectChartSessionNo;

    /// <summary>
    ///好友列表
    /// </summary>
    public static List<ContactsTableBean> FriendDatatable = new List<ContactsTableBean>();

    /// <summary>
    ///群列表
    /// </summary>
    public static List<MucTableBean> GroupDatatable = new List<MucTableBean>();

    ///// <summary>
    ///// TODO：将来要删除，当前登录人的图片路径
    ///// </summary>
    public static string ImagePath;

    /// <summary>
    /// 当前登录人的缓存根目录
    /// </summary>
    public static string CacheRootPath;

    /// <summary>
    /// 当前应用的根目录
    /// </summary>
    public static string AppRootPath;

    /// <summary>
    /// 当前应用的启动方式
    /// </summary>
    public static string AppStartType;
    /// <summary>
    /// 当前应用的启动提示信息
    /// </summary>
    public static string AppStartMessage;

    /// <summary>
    /// 默认设置的缓存根目录
    /// </summary>
    public static string DefaultCacheRootPath;

    /// <summary>
    /// 默认头像
    /// </summary>
    public static string ImagePathDefault = @"pack://application:,,,/ResourceDictionary;Component/images/Default_avatar.jpg";
    //public static bool click=false;


    /// <summary>
    /// 一次性登录token
    /// </summary>
    public static string nonceToken;

    private Object locker = new Object();
    Timer stateTimer = null;



    public App() {

    }


    public static void ExitApp(Boolean isMainWindow=true) {
        try {

            if (isMainWindow) {
                VoiceHelper.StopPlayASound(false);
                LoginByBarcodeService.getInstance().sendLoginStatusMessage(PcLoginStatus.Logout);
                SetLastLoginUser();
            }
            Thread t = new Thread(new ThreadStart(() => {

                //PcStart.getInstance().DisposeIcon();
                Internet.getInstance().Stop();
                ImClientService.getInstance().disConnectFromIm();
                //关闭chromium process进程
                // Cef.Shutdown();

                //String processName = "CefSharp.BrowserSubprocess";
                //Process[] processes = Process.GetProcessesByName(processName);

                //if (processes.Length > 0) {
                //    foreach (var process in processes) {
                //        process.Kill();
                //    }
                //}
            }));
            t.IsBackground = true;
            t.Start();


        } catch (Exception e) {
            Log.Error(typeof(App),e);
        }
        try {
            // 当前进程
            //Process cur = Process.GetCurrentProcess();
            //cur.Kill();
        } catch (Exception e) {
            Log.Error(typeof(App), e);
        }
        try {

            Environment.Exit(0);
        } catch (Exception e) {
            Log.Error(typeof(App), e);
        }


    }


    protected override void OnActivated(System.EventArgs e) {
        AppActivated = true;
        this.initEventBus();
        base.OnActivated(e);
        //Console.WriteLine(" app OnActivated:AppActivated =" + AppActivated);
    }

    protected override void OnDeactivated(System.EventArgs e) {
        AppActivated = false;
        base.OnDeactivated(e);
        //Console.WriteLine(" app OnDeactivated:AppActivated =" + AppActivated);
    }
    /// <summary>
    ///
    /// 整个应用的定时执行的任务
    /// </summary>
    /// <param Name="stateInfo"></param>
    private void timeToDo(Object stateInfo) {
        this.initEventBus();
    }


    private void startAppTimer() {
        if (stateTimer == null) {
            var autoEvent = new AutoResetEvent(false);
            stateTimer = new Timer(new System.Threading.TimerCallback(timeToDo),
                                   autoEvent, 1000, 60*1000);

        }
    }

    private void unRegisterEventBus() {

        EventBusHelper eventbus = EventBusHelper.getInstance();
        eventbus.unRegister(ContactsServices.getInstance());
        eventbus.unRegister(MucServices.getInstance());
        eventbus.unRegister(AccountsServices.getInstance());
        eventbus.unRegister(OrganizationServices.getInstance());
        eventbus.unRegister(OrganizationMemberService.getInstance());
        eventbus.unRegister(MasterServices.getInstance());
        eventbus.unRegister(AddressBookPage.getInstance());
        eventbus.unRegister(PersonDetailed.getInstance());
        eventbus.unRegister(MessageService.getInstance());
        eventbus.unRegister(PcOA.getInstance());
        eventbus.unRegister(LxrDetailedPage.getInstance());
        eventbus.unRegister(GroupChatDetailedPage.getInstance());
        eventbus.unRegister(ChatPage.getInstance());
        eventbus.unRegister(ChatSessionService.getInstance());
        eventbus.unRegister(SettingService.getInstance());
        // eventbus.unRegister(UploadServices.getInstance());
        eventbus.unRegister(VcardService.getInstance());
        eventbus.unRegister(PublicAccountsService.getInstance());
        //eventbus.unRegister(ContactsServices.getInstance());

        eventbus.unRegister(PcStart.getInstance());
        eventbus.unRegister(TodoTaskService.getInstance());
        eventbus.unRegister(ThirdAppPage.getInstance());
        eventbus.unRegister(AppMsgPage.getInstance());
        eventbus.unRegister(TodoTaskPage.getInstance());
        eventbus.unRegister(TenantsWindow.getInstance());
        eventbus.unRegister(LoginWindow.getInstance());
        //eventbus.register(TextMessageControl.getInstance());

        eventbus.unRegister(ThirdAppGroupAndClassService.getInstance());

        eventbus.unRegister(PublicWebService.getInstance());
        eventbus.unRegister(ApplicationService.getInstance());
        eventbus.unRegister(AppWindow.getInstance());
        //eventbus.register(Window1.getInstance());
        eventbus.unRegister(AVMeetingService.getInstance());
        eventbus.unRegister(PublicDetailed.getInstance());
        //eventbus.unRegister(AccountDetailed.getInstance());

        eventbus.unRegister(OpinionPage.getInstance());

        eventbus.unRegister(PublicAccountsFindPage.getInstance());
        eventbus.unRegister(PublicAccountsDetailedPage.getInstance());

    }


    private void initEventBus() {
        lock (this.locker) {
            EventBusHelper eventbus = EventBusHelper.getInstance();
            eventbus.register(ContactsServices.getInstance());
            eventbus.register(MucServices.getInstance());
            eventbus.register(AccountsServices.getInstance());
            eventbus.register(OrganizationServices.getInstance());
            eventbus.register(OrganizationMemberService.getInstance());
            eventbus.register(MasterServices.getInstance());
            eventbus.register(AddressBookPage.getInstance());
            eventbus.register(PersonDetailed.getInstance());
            eventbus.register(MessageService.getInstance());
            eventbus.register(PcOA.getInstance());
            eventbus.register(LxrDetailedPage.getInstance());
            eventbus.register(GroupChatDetailedPage.getInstance());
            eventbus.register(ChatPage.getInstance());
            eventbus.register(ChatSessionService.getInstance());
            eventbus.register(SettingService.getInstance());
            //eventbus.register(UploadServices.getInstance());
            eventbus.register(VcardService.getInstance());
            eventbus.register(PublicAccountsService.getInstance());
            //eventbus.unRegister(ContactsServices.getInstance());

            eventbus.register(PcStart.getInstance());
            eventbus.register(TodoTaskService.getInstance());
            eventbus.register(ThirdAppPage.getInstance());
            eventbus.register(AppMsgPage.getInstance());
            eventbus.register(TodoTaskPage.getInstance());

            eventbus.register(TenantsWindow.getInstance());
            eventbus.register(LoginWindow.getInstance());
            //eventbus.register(TextMessageControl.getInstance());

            eventbus.register(ThirdAppGroupAndClassService.getInstance());

            eventbus.register(PublicWebService.getInstance());
            eventbus.register(ApplicationService.getInstance());
            eventbus.register(AppWindow.getInstance());
            eventbus.register(AVMeetingService.getInstance());

            eventbus.register(PublicDetailed.getInstance());
            //eventbus.register(AccountDetailed.getInstance());
            eventbus.register(OpinionPage.getInstance());
            eventbus.register(PublicAccountsFindPage.getInstance());
            eventbus.register(PublicAccountsDetailedPage.getInstance());
        }

    }

    private static void SetLastLoginUser() {
        try {
            if (App.AccountsModel!=null && !string.IsNullOrEmpty(App.AccountsModel.no)) {
                LocalCacheHelper.SetLastLoginUser();
                if (App.AccountsModel.avatarStorageRecordId == null) return;
                FilesTable filesTable =
                    FilesService.getInstance().getFile(App.AccountsModel.avatarStorageRecordId);
                if (filesTable != null) {
                    String copyToPath = filesTable.localpath.Replace(App.AccountsModel.no, "default");
                    String copyToForder = copyToPath.Replace(App.AccountsModel.avatarStorageRecordId + ".jpg", "");
                    //如果不存在就创建file文件夹
                    if (Directory.Exists(copyToForder) == false) {
                        Directory.CreateDirectory(copyToForder);
                    }
                    System.IO.File.Copy(filesTable.localpath, copyToPath, true);
                }
            }

        } catch (Exception ex) {
            Log.Error(typeof(App), ex);
        }
    }
    protected override void OnExit(System.Windows.ExitEventArgs e) {
        this.stateTimer.Dispose();
        this.stateTimer = null;
        this.unRegisterEventBus();
        base.OnExit(e);
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        //var settings = new CefSettings() {
        //    //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
        //    CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Dingxin\\Cache")
        //};
        //settings.Locale = "zh-CN";

        ////Perform dependency check to make sure all relevant resources are in our output directory.
        //Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

        this.startAppTimer();
        //初始化eventbus
        this.initEventBus();


        // 初期化
        StartLogic.init(e.Args);

        // 启动
        //Application.Current.MainWindow.Show();

        //LoginWindow a = new LoginWindow();

        LoginWindow loginwindow = LoginWindow.getInstance();
        loginwindow.Show();

    }
}
}
