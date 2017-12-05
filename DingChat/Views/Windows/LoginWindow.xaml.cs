using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.DbHelper;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Control;
using EventBus;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DatabaseUpdate;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.update;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.im.sdk.api;
using cn.lds.im.sdk.bean;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Models.Tables;
using RestSharp;

namespace cn.lds.chatcore.pcw.Views.Windows {

/// <summary>
/// 登录
/// </summary>
public partial class LoginWindow : Window {

    private static LoginWindow instance = null;

    public static LoginWindow getInstance() {
        if (instance == null) {
            instance = new LoginWindow();
        }
        return instance;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    public LoginWindow() {
        InitializeComponent();
        this.DataContext = this;
    }



    Thread Thread = null;

    Timer stateTimer = null;
    int interval = 1000;

    bool jdg = false;

    //当前登录类型
    private LoginType loginType = LoginType.barCode;
    // 当前登录状态
    private LoginStatus loginStatus = LoginStatus.first_time_login;
    // 登录人头像ID
    private string loingUserAvatarId;
    private RelayCommand command1 = null;

    public RelayCommand Command1 {
        get {
            if (command1 == null) {
                command1 = new RelayCommand(() => {
                    MessageBox.Show("xxx");
                });
            }
            return command1;
        }
    }

    /// <summary>
    /// 登陆画面加载处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoginWindow_OnLoaded(object sender, RoutedEventArgs e) {
        try {
            // 设置缓存的默认目录，登录完成后会修改成基于用户NO的。
            App.CacheRootPath =
                App.DefaultCacheRootPath =
                    System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/default/";
            // 判断是否有上回登录人
            LastLoginUserBean lastLoginUserBean = LocalCacheHelper.GetLastLoginUser();
            if (lastLoginUserBean == null) {
                //根据登录类型控制画面显示
                this.PageControlByLoginType();
            } else {
                this.loginStatus = LoginStatus.waitting_login;
                //ImageAvatar.Source = Visibility.Hidden;
                TxtUser.Text = lastLoginUserBean.mobile;
                LblLoginUserName.Content = lastLoginUserBean.userName;
                ImageHelper.loadLoginUserAvatar(lastLoginUserBean.avatarStorageId, ImageAvatar);
                LoginByBarcodeService.getInstance().toClientId = lastLoginUserBean.userNo;
                //根据登录状态控制画面显示
                this.PageControlByLoginStatus();
            }

            // 为了扫码登录、简单粗暴的先连接上IM再说
            ThreadPool.QueueUserWorkItem(conectToImQrcode);

            // 登录时，简单粗暴的kill掉其他的进程，避免 一台机器启动两个以上
            //ApplicationService.getInstance().KillAllOtherProcess();

            // TODO：后续需要优化，例如先判断是否有同名进程已经启动，然后弹出个提示框是否启动，启动则kill，不启动则自己close
            if (string.IsNullOrEmpty(App.AppStartType)) {
                if (ApplicationService.getInstance().IsApplicationExist()) {
                    if (CommonMessageBox.Msg.Show("程序已运行！", CommonMessageBox.MsgBtn.OK) ==
                            CommonMessageBox.MsgResult.OK) {
                        BtnClose_Click(null, null);
                    }
                }
            } else {
                // 用户被禁用
                if (MsgType.UserDisabled.ToStr().Equals(App.AppStartType)) {
                    this.SetErrorMessage(App.AppStartMessage);
                }
                // 用户登录token过期
                else if (FrameEventDataType.LOGOUT_TOKEN_INVALID.ToStr().Equals(App.AppStartType)) {
                    this.SetErrorMessage(App.AppStartMessage);
                }
                // 用户被踢
                else if (FrameEventDataType.LOGOUT_USER_KICKED.ToStr().Equals(App.AppStartType)) {
                    if (CommonMessageBox.Msg.Show(App.AppStartMessage, CommonMessageBox.MsgBtn.OK) ==
                            CommonMessageBox.MsgResult.OK) {

                    }
                }
                // 用户手机强制退出
                else if (MsgType.LoginQuit.ToStr().Equals(App.AppStartType)) {
                    if (CommonMessageBox.Msg.Show(App.AppStartMessage, CommonMessageBox.MsgBtn.OK) ==
                            CommonMessageBox.MsgResult.OK) {
                    }
                }

            }
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 收到通知消息
    /// </summary>
    /// <param name="messageArrivedEvent"></param>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {
        try {
            if (this.loginStatus == LoginStatus.first_time_login
                    || this.loginStatus == LoginStatus.waitting_login
                    || this.loginStatus == LoginStatus.waitting_mobile_login) {

                // 获取消息类型
                Message message = messageArrivedEvent.message;
                MsgType msgType = message.getType();
                switch (msgType) {
                // 等待登录
                case MsgType.LoginWaitting:
                    OnLoginWaitting(message);
                    break;
                // 取消等待
                case MsgType.LoginWaittingCancel:
                    OnLoginWaittingCancel(message);
                    break;
                // 授权登录
                case MsgType.LoginAuthorization:
                    OnLoginAuthorization(message);
                    break;
                default:
                    break;
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 等待登录
    /// </summary>
    public void OnLoginWaitting(Message messageBean) {
        try {
            LoginWaittingMessage loginWaittingMessage = (LoginWaittingMessage) messageBean;
            this.Dispatcher.Invoke(new Action(() => {
                this.LblLoginUserName.Content =
                    loginWaittingMessage.nickname;
            }));

            this.loingUserAvatarId = loginWaittingMessage.avatarStorageId;
            DownloadServices.getInstance()
            .DownloadMethod(loginWaittingMessage.avatarStorageId, DownloadType.SYSTEM_IMAGE, null);
            this.loginStatus = LoginStatus.waitting_mobile_login;
            this.PageControlByLoginStatus();
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 取消等待登录
    /// </summary>
    public void OnLoginWaittingCancel(Message messageBean) {
        try {
            if (this.loginStatus == LoginStatus.waitting_mobile_login) {
                LoginWaittingCancelMessage loginWaittingCancelMessage = (LoginWaittingCancelMessage) messageBean;
                this.loginStatus = LoginStatus.first_time_login;
                this.loginType = LoginType.barCode;
                this.PageControlByLoginType();
            }

        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 授权登录
    /// </summary>
    public void OnLoginAuthorization(Message messageBean) {
        try {
            if (this.loginStatus == LoginStatus.waitting_mobile_login) {
                LoginAuthorizationMessage loginAuthorizationMessage = (LoginAuthorizationMessage) messageBean;
                this.ItemsCtrl(true);
                // 隐藏按钮
                this.PageControlBtnChangeLoginType(Visibility.Hidden);
                this.DoLogin(null, null, loginAuthorizationMessage.mobile, loginAuthorizationMessage.captcha);
            }
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// TODO：(这个代码放这里啥意思)保存群成员列表 头像
    /// </summary>
    private void Download() {
        MucServices.getInstance().SaveGroupMemberToVsCard();
    }

    /// <summary>
    /// 运行IM连接（登陆后连接真正的IM）
    /// </summary>
    /// <param name="ojb"></param>
    private void conectToImQrcode(object ojb) {
        try {
            //进行IM连接（扫码登录用）
            LoginByBarcodeService.getInstance().conectToIm();
            // 获取登录二维码(每次都获取二维码，因为二维码可能过期)
            LoginByBarcodeService.getInstance().GetQrcode();

            //String loginBarcode = LocalCacheHelper.GetLoginBarcode();
            //if (string.IsNullOrEmpty(loginBarcode)) {
            //    // 获取登录二维码
            //    LoginByBarcodeService.getInstance().GetQrcode();
            //} else {
            //    this.SetLoginBarCode2D(long.Parse(loginBarcode));
            //}

        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }
    }

    /// <summary>
    /// 运行IM连接（登陆后连接真正的IM）
    /// </summary>
    /// <param name="ojb"></param>
    private void conectToIm(object ojb) {
        try {
            LoginByBarcodeService.getInstance().disConectToIm();
            ImClientService.getInstance().setImtpConnectType(ImtpConnectType.real);
            ImClientService.getInstance().connectToIm();
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }
    }


    /// <summary>
    /// 监听业务事件
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            // 数据加载完成
            case BusinessEventDataType.LoadingOk:
                DoLoadingOk(data);
                break;
            case BusinessEventDataType.FileDownloadedEvent:
                DoFileDownloadedEvent(data);
                break;
            case BusinessEventDataType.BarCode2DDownloadedEvent:
                DoBarCode2DDownloadedEvent(data);
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }

    }

    /// <summary>
    /// 登录二维码下载完成事件处理
    /// </summary>
    /// <param name="data"></param>
    private void DoBarCode2DDownloadedEvent(BusinessEvent<Object> data) {
        try {
            FileEventData fileBean = (FileEventData) data.data;
            this.SetLoginBarCode2D(fileBean.fileStorageId);
            //LocalCacheHelper.SetLoginBarcode(fileBean.fileStorageId.ToStr());
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }
    }

    /// <summary>
    /// 设置二维码
    /// </summary>
    /// <param name="fileStorageId"></param>
    private void SetLoginBarCode2D(string fileStorageId) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                String loginBarcodePath =
                    ImageHelper.load2DBarcode(fileStorageId, ImageBarcode);
            }));

        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
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
            FileEventData fileBean = (FileEventData) data.data;
            if (fileBean.fileStorageId == this.loingUserAvatarId) {
                this.SetLoginUserAvatar(fileBean.fileStorageId);
            }
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }
    }

    /// <summary>
    /// 显示登陆人头像
    /// </summary>
    /// <param name="fileStorageId"></param>
    private void SetLoginUserAvatar(string fileStorageId) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                ImageHelper.loadLoginUserAvatar(fileStorageId, ImageAvatar);
            }));
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }
    }

    /// <summary>
    /// 数据加载完成事件处理
    /// </summary>
    /// <param name="data"></param>
    private void DoLoadingOk(BusinessEvent<Object> data) {
        try {
            if (App.GroupsLoadOk) {
                Thread = new Thread(new ThreadStart(Download));
                Thread.IsBackground = true;
                Thread.Start();
            }
            if (LoginServices.getInstance().IsLogin()) {

                this.Dispatcher.Invoke(new Action(() => {

                    if (!jdg) {
                        jdg = true;
                        if (!App.mainWindowLoaded) {

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
                            PcStart pcStart = PcStart.getInstance();
                            pcStart.Show();
                            this.loginStatus = LoginStatus.login_complete;

                            //查询未读数量
                            MessageService.getInstance().countOfUnreadMessages();
                            if (stateTimer != null) {
                                stateTimer.Dispose();
                            }
                            App.mainWindowLoaded = true;
                            this.Close();
                        }
                    }


                }));
            }
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
        }
    }


    /// <summary>
    /// 登录按钮点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnLogin_Click(object sender, RoutedEventArgs e) {
        try {
            if (this.loginStatus == LoginStatus.waitting_login) {
                this.loginStatus = LoginStatus.waitting_mobile_login;
                Thread t = new Thread(new ThreadStart(() => {
                    this.PageControlByLoginStatus();
                    LoginByBarcodeService.getInstance().sendLoginRequestMessage();
                }));
                t.IsBackground = true;
                t.Start();
            } else {
                if (this.ValidateForm()) {
                    this.ItemsCtrl(true);
                    // 隐藏按钮
                    this.PageControlBtnChangeLoginType(Visibility.Hidden);
                    String loginid = TxtUser.Text;
                    String pass = TxtPwd.Password;

                    Thread t = new Thread(new ThreadStart(() => {
                        this.DoLogin(loginid, pass, null, null);
                    }));
                    t.IsBackground = true;
                    t.Start();
                }
            }

        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 校验输入
    /// </summary>
    /// <returns></returns>
    public Boolean ValidateForm() {
        try {
            this.SetErrorMessage("");
            if ("".Equals(TxtUser.Text)) {
                this.SetErrorMessage("请输入用户名！");
                TxtUser.Focus();
                return false;
            }
            if ("".Equals(TxtPwd.Password)) {
                this.SetErrorMessage("请输入密码！");
                TxtPwd.Focus();
                return false;
            }
            return true;
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
        return false;
    }

    /// <summary>
    /// 设置登录文字效果
    /// </summary>
    /// <param name="stateInfo"></param>
    public void SetLoadingText(Object stateInfo) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                // 如果是初次登录，则拉取数据的时候，显示数据同步中
                if (App.IsFirstLogin) {
                    TxtMessage.Text = "数据同步中";
                    App.IsFirstLogin = false;
                }
                if (TxtMessage.Text.Equals("登录中...")) {
                    TxtMessage.Text = "登录中";
                } else if (TxtMessage.Text.Equals("数据同步中...")) {
                    TxtMessage.Text = "数据同步中";
                }

                TxtMessage.Text = TxtMessage.Text + ".";
            }));
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }

    }

    /// <summary>
    /// 设置提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void SetErrorMessage(String msg) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                TxtMessage.Text = msg;
            }));

        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 画面控件控制
    /// </summary>
    /// <param name="loginStatus">true：登录中，false：未登录或登录失败</param>
    public void ItemsCtrl(Boolean loginStatus) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                if (loginStatus) {
                    // 重置标识
                    App.IsFirstLogin = false;
                    // 等待条
                    //GridLoading.Visibility = Visibility.Visible;
                    TxtUser.IsEnabled = false;
                    TxtPwd.IsEnabled = false;
                    BtnLogin.IsEnabled = false;
                    this.SetErrorMessage("登录中.");

                    var autoEvent = new AutoResetEvent(false);
                    stateTimer = new Timer(this.SetLoadingText, autoEvent, 1000, interval);


                } else {
                    // 等待条
                    //GridLoading.Visibility = Visibility.Hidden;
                    TxtUser.IsEnabled = true;
                    TxtPwd.IsEnabled = true;
                    BtnLogin.IsEnabled = true;
                    TxtMessage.Text = "";
                    if (stateTimer != null) {
                        stateTimer.Dispose();
                    }
                }
            }));

        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    private void updateDatabase() {
        DbManager dbManager = new DbManager();
        dbManager.updateDatabase();

    }

    public Boolean IsConectToImFinished = false;

    /// <summary>
    /// 登录处理
    /// </summary>
    private void DoLogin(string loginId, string password, String mobile, String captcha) {
        try {
            // 记录登录前的状态，并设置为登录中
            LoginStatus oldLoginStatus = this.loginStatus;
            this.loginStatus = LoginStatus.login_ing;

            EventData<Object> eventData = null;
            eventData = LoginServices.getInstance()
                        .LoginPost(loginId, password, mobile, captcha);
            // 请求成功
            if (eventData != null && eventData.eventType == EventType.HttpRequest) {

                // 开启校验进程
                Updater.getInstance().CheckUpdateStatus();

                ThreadPool.QueueUserWorkItem(conectToIm);

                // 初始化数据库相关
                DbManager manager = new DbManager();
                manager.createDatabaseFile(App.AccountsModel.no);

                // 初始化应用设置
                //设置图片地址
                string path = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" +
                              App.AccountsModel.no + "/" + "Images";
                //全局变量 图片存储的位置
                App.ImagePath = path;
                //全局变量 个人缓存的根目录地址
                App.CacheRootPath = System.IO.Path.GetFullPath(Environment.CurrentDirectory) +
                                    @"/DataSqlite/private/" +
                                    App.AccountsModel.no + "/";

                App.AppRootPath = System.IO.Path.GetFullPath(Environment.CurrentDirectory);


                if (App.AccountsModel.tenants.Count >1) {
                    Thread winShowt = new Thread(new ThreadStart(() => {
                        this.Dispatcher.Invoke(new Action(() => {
                            TenantsWindow win =  TenantsWindow.getInstance();
                            win.Show();
                        }));
                    }));
                    winShowt.IsBackground = true;
                    winShowt.Start();
                }


                string tenantList = SqlHelper.GetInClausePart(App.TenantNoList);
                LoginServices.getInstance().DeleteByTenantNo(tenantList);

                // 判断数据库是否存在，如果存在则直接进入系统，异步同步必要数据
                if (!SQLiteHelper.getInstance().TableExists("master")) {
                    // 标识是初次登录
                    App.IsFirstLogin = true;

                    this.updateDatabase();

                    this.getInitData(null);
                } else {
                    // 为避免getMyDetail接口拉取失败，导致缺少App.AccountsModel.clientuserId的值，非初次登录时做下处理，如果是初次登录失败的话，暂时没招了……
                    AccountsTable accountsTable = AccountsServices.getInstance().findByNo(App.AccountsModel.no);
                    if (accountsTable != null) {
                        App.AccountsModel.clientuserId = accountsTable.clientuserId;
                    }

                    this.updateDatabase();

                    // （非第一次登陆系统）刚登陆系统时，把所有的消息状态是发送中的改为发送失败
                    Thread t = new Thread(new ThreadStart(() => {
                        MessageService.getInstance().setAllSentFlagError();
                        this.getInitData(null);
                    }));
                    t.IsBackground = true;
                    t.Start();
                }

                //ProgramSettingHelper.LoginUser = RestRequestHelper.ConvertCookiesToModel(strPageUri);
                ////根据权限修改xml
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/settings/Program.xml");//加载xml文件，文件
                //XmlElement xe = xmlDoc.DocumentElement; // DocumentElement 获取xml文档对象的根XmlElement.
                //string strPath = string.Format("/Root/ProcessSettings/ProcessButton[@Text=\"{0}\"]", "通讯录");
                //XmlElement selectXe = (XmlElement)xe.SelectSingleNode(strPath);  //selectSingleNode 根据XPath表达式,获得符合条件的第一个节点.

                //selectXe.GetElementsByTagName("ISVisible").Item(0).InnerText = "F";

                //xmlDoc.Save(System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/settings/Program.xml");
            } else {
                this.ItemsCtrl(false);

                if (oldLoginStatus == LoginStatus.waitting_mobile_login) {
                    this.loginStatus = LoginStatus.first_time_login;
                    this.loginType = LoginType.barCode;
                    this.PageControlByLoginType();
                } else {
                    this.loginStatus = oldLoginStatus;
                }

                // 提示错误信息
                String errors = "";
                if (eventData != null) {
                    foreach (RestRequestError error in eventData.errors) {
                        errors += error.errmsg + "\n";
                    }
                } else {
                    errors = TostMessage.MSG_E9999;
                }
                this.SetErrorMessage(errors);
                this.PageControlBtnChangeLoginType(Visibility.Visible);
            }
        } catch (Exception e) {
            Log.Error(typeof (LoginWindow), e);
            this.PageControlBtnChangeLoginType(Visibility.Visible);
        }
    }

    /// <summary>
    /// 同步必要数据
    /// </summary>
    /// <param name="obj"></param>
    private void getInitData(object obj) {


        //重构没有拼音的数据
        ApplicationService.getInstance().ReBuildDataWithoutPinyin();

        //获取我的信息
        AccountsServices.getInstance().GetMyDetail();
        //下载好友列表
        ContactsServices.getInstance().Contacts();
        //下载群列表
        MucServices.getInstance().RequestGroups();


        if (App.TenantNoList.Count > 0) {
            foreach (string tenantNo in App.TenantNoList) {
                // 同步组织数据
                OrganizationServices.getInstance().GetOrganization(tenantNo);
            }
        } else {
            // 同步组织数据
            OrganizationServices.getInstance().GetOrganization(string.Empty);
        }


        //拉取公众号
        if (App.TenantNoList.Count > 0) {
            foreach (string tenantNo in App.TenantNoList) {
                Dictionary<String, Object> extras = new Dictionary<String, Object>();
                extras.Add("tenantNo", tenantNo);
                PublicAccountsService.getInstance().request(extras);
            }
        } else {
            PublicAccountsService.getInstance().request(null);
        }
        ThirdAppGroupAndClassService.getInstance().RequestForBaseAppGroups(App.CurrentTenantNo);
        // 拉取应用类别的分组
        if (App.TenantNoList.Count > 0) {
            foreach (string tenantNo in App.TenantNoList) {
                ThirdAppGroupAndClassService.getInstance().RequestForThirdAppGroups(tenantNo);
            }
        } else {
            ThirdAppGroupAndClassService.getInstance().RequestForThirdAppGroups(string.Empty);
        }

        // 拉取应用列表
        if (App.TenantNoList.Count > 0) {
            foreach (string tenantNo in App.TenantNoList) {
                PublicWebService.getInstance().RequestForApps(tenantNo);
            }
        } else {
            PublicWebService.getInstance().RequestForApps(string.Empty);
        }
        //待办事项拉取
        if (App.TenantNoList.Count > 0) {
            foreach (string tenantNo in App.TenantNoList) {
                TodoTaskService.getInstance().requestForTodoTask(tenantNo);
            }
        } else {
            TodoTaskService.getInstance().requestForTodoTask(string.Empty);
        }


        //这个需要调整，需要变更为异步加载
        CacheHelper.getInstance().loadAllToCache();

        // 预先加载一次会话
        ApplicationService.getInstance().LoadChatSession();

    }



    /// <summary>
    /// 密码框keydown事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TxtPwd_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            BtnLogin_Click(null, null);
        }
    }

    /// <summary>
    /// 窗体左键点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        this.DragMove();
    }

    private void PageControlByLoginStatus() {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                switch (this.loginStatus) {
                case LoginStatus.first_time_login:
                    BtnLogin.Visibility = Visibility.Visible;
                    TxtUser.Visibility = Visibility.Visible;
                    TxtPwd.Visibility = Visibility.Visible;
                    ImageLogo.Visibility = Visibility.Visible;
                    ImageBarcode.Visibility = Visibility.Hidden;
                    ImageAvatar.Visibility = Visibility.Hidden;
                    LblScanNotice.Visibility = Visibility.Hidden;
                    LblLoginUserName.Visibility = Visibility.Hidden;
                    LblWaitingMobileConfirm.Visibility = Visibility.Hidden;
                    this.loginType = LoginType.id_pass;
                    BtnChangeLoginType.Content = "切换到扫码登录";
                    break;
                case LoginStatus.waitting_login:
                    BtnLogin.Visibility = Visibility.Visible;
                    TxtUser.Visibility = Visibility.Hidden;
                    TxtPwd.Visibility = Visibility.Hidden;
                    ImageLogo.Visibility = Visibility.Hidden;
                    ImageBarcode.Visibility = Visibility.Hidden;
                    ImageAvatar.Visibility = Visibility.Visible;
                    LblScanNotice.Visibility = Visibility.Hidden;
                    LblLoginUserName.Visibility = Visibility.Visible;
                    LblWaitingMobileConfirm.Visibility = Visibility.Hidden;
                    this.loginType = LoginType.barCode;
                    BtnChangeLoginType.Content = "切换到帐号密码登录";
                    break;
                case LoginStatus.waitting_mobile_login:
                    BtnLogin.Visibility = Visibility.Hidden;
                    TxtUser.Visibility = Visibility.Hidden;
                    TxtPwd.Visibility = Visibility.Hidden;
                    ImageLogo.Visibility = Visibility.Hidden;
                    ImageBarcode.Visibility = Visibility.Hidden;
                    ImageAvatar.Visibility = Visibility.Visible;
                    LblScanNotice.Visibility = Visibility.Hidden;
                    LblLoginUserName.Visibility = Visibility.Visible;
                    LblWaitingMobileConfirm.Visibility = Visibility.Visible;
                    this.loginType = LoginType.id_pass; // TODO :感觉通过这个控制有点怪怪的。
                    BtnChangeLoginType.Content = "返回到扫码登录";
                    break;
                }
            }));

        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 根据登录类型控制画面显示状态
    /// </summary>
    private void PageControlByLoginType() {
        try {
            // 只要点击过下面的切换登录状态按钮，都需要修改这个登录状态
            this.loginStatus = LoginStatus.first_time_login;

            this.Dispatcher.Invoke(new Action(() => {
                switch (this.loginType) {
                case LoginType.id_pass:
                    BtnLogin.Visibility = Visibility.Visible;
                    TxtUser.Visibility = Visibility.Visible;
                    TxtPwd.Visibility = Visibility.Visible;
                    ImageLogo.Visibility = Visibility.Visible;
                    ImageBarcode.Visibility = Visibility.Hidden;
                    ImageAvatar.Visibility = Visibility.Hidden;
                    LblScanNotice.Visibility = Visibility.Hidden;
                    ;
                    LblLoginUserName.Visibility = Visibility.Hidden;
                    LblWaitingMobileConfirm.Visibility = Visibility.Hidden;
                    BtnChangeLoginType.Content = "切换到扫码登录";
                    break;
                case LoginType.barCode:
                    BtnLogin.Visibility = Visibility.Hidden;
                    TxtUser.Visibility = Visibility.Hidden;
                    TxtPwd.Visibility = Visibility.Hidden;
                    ImageLogo.Visibility = Visibility.Hidden;
                    ImageBarcode.Visibility = Visibility.Visible;
                    ImageAvatar.Visibility = Visibility.Hidden;
                    LblScanNotice.Visibility = Visibility.Visible;
                    LblLoginUserName.Visibility = Visibility.Hidden;
                    LblWaitingMobileConfirm.Visibility = Visibility.Hidden;
                    BtnChangeLoginType.Content = "切换到帐号密码登录";
                    break;
                }
            }));

        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 控制登录方式切换按钮的显示
    /// </summary>
    /// <param name="visibility"></param>
    private void PageControlBtnChangeLoginType(Visibility visibility) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                this.BtnChangeLoginType.Visibility = visibility;
            }));
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// 切换登录类型
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnChangeLoginType_OnClick(object sender, RoutedEventArgs e) {
        try {
            if (this.loginType == LoginType.id_pass) {
                this.loginType = LoginType.barCode;
            } else {
                this.loginType = LoginType.id_pass;
            }
            this.PageControlByLoginType();
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    /// <summary>
    /// TODO:模拟登录用于测试二维码登录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ImageBarcode_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        ThreadPool.QueueUserWorkItem(conectToImQrcode);
        //if(ImageBarcode.Source==null) {
        // LoginByBarcodeService.getInstance().GetQrcode();
        //}

        //SendMessage sendMessage = new SendMessage();
        //sendMessage.setFromClientId("CACB488A6566AB4CDB70DF3FA4A3E");
        //sendMessage.setToClientId("C1N7CDLJE65C5");
        //sendMessage.setMessageId(ImClientService.getInstance().generateMessageId());
        //sendMessage.setMessage("{\r\n  \"text\": \"asdfsdfsf\"\r\n}");
        //sendMessage.setMessageType(1);
        //sendMessage.setTime(DateTimeHelper.getTimeStamp());
        //ImClientService.getInstance().sendMessage(sendMessage);

        //Thread t = new Thread(new ThreadStart(() => {
        //    this.Dispatcher.Invoke(new Action(() => {
        //        this.TxtUser.Text = "quwei";
        //        this.TxtPwd.Password = "123456";
        //        EventData<Object> eventData = LoginServices.getInstance().LoginPost(TxtUser.Text, TxtPwd.Password);
        //        // 请求成功
        //        if (eventData != null && eventData.eventType == EventType.HttpRequest) {
        //            // 测试获取登录二维码
        //            LoginByBarcodeService.getInstance().GetQrcode();
        //            // 测试进行IM连接（扫码登录用）
        //            //LoginByBarcodeService.getInstance().conectToIm();
        //        } else {

        //        }
        //    }));
        //}));
        //t.Start();
    }

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnClose_Click(object sender, RoutedEventArgs e) {

        try {
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof (LoginWindow), ex);
        }
    }

    private void LoginWindow_OnClosing(object sender, CancelEventArgs e) {
        if (!App.mainWindowLoaded) {
            App.ExitApp(false);
        }

    }

    //TODO: 测试https
    //private void BtnTest_OnClick(object sender, RoutedEventArgs e) {
    //    string url =
    //        @"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2017-07-20&leftTicketDTO.from_station=DFT&leftTicketDTO.to_station=BJP&purpose_codes=ADULT";
    //    var client = RestRequestHelper.GetHttpClient(url);
    //    RestRequest request = new RestRequest("", Method.GET);
    //    client.ExecuteAsync(request, restResponse => {
    //        if (restResponse.ErrorException != null) {
    //            //throw new ApplicationException(message, restResponse.ErrorException);
    //            Log.Error(typeof (RestRequestHelper), url, restResponse.ErrorException);
    //        }

    //        if (restResponse.StatusCode.Equals(HttpStatusCode.OK)) {
    //            //异步处理，返回
    //            JObject obj = JObject.Parse(restResponse.Content);
    //            JToken jtoken = obj.GetValue("status");
    //            String status = jtoken.ToString();

    //        }
    //    });
    //}
}
}
