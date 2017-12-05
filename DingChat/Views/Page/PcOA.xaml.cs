using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Windows;
using EventBus;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Views.Adorners;
using cn.lds.chatcore.pcw.Views.Page.PublicAccounts;
using GalaSoft.MvvmLight.Command;
using System.Threading;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// Window2.xaml 的交互逻辑
/// </summary>
public partial class PcOA : System.Windows.Controls.Page {
    private PcOA() {
        InitializeComponent();
        this.DataContext = this;


    }

    private ObservableCollection<ChatSessionBean> myListViewItems = new ObservableCollection<ChatSessionBean>();
    public ObservableCollection<ChatSessionBean> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }
    private static PcOA instance = null;
    public static PcOA getInstance() {
        if (instance == null) {
            instance = new PcOA();
        }
        return instance;
    }

    //左侧聊天记录列表选择的行数
    private int _listBoxSelectIndex = -1;
    //通讯录点击发消息传过来的Id
    public int AddChartUserId = 0;
    //群聊点击发消息传过来的Id
    public string AddChartGroupNo = string.Empty;
    //公众号点击发消息传过来的
    public string AddPublicAccounts = string.Empty;





    /// <summary>
    /// 界面
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<object> data) {
        try {
            switch (data.eventDataType) {
            // 头像换了
            case BusinessEventDataType.ContactsDetailsChangeEvent:
                DoContactsDetailsChangeEvent(data);
                break;
            // 修改chartsession
            case BusinessEventDataType.ChartSessionChangeEvent:
                DoChartSessionChangeEvent(data);
                break;
            // 聊天详情修改
            case BusinessEventDataType.ChatDetailedChangeEvent:
                DoChatDetailedChangeEvent(data);
                break;
            // 群名  头像 换了
            case BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME:
                DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(data);
                break;
            // 点击公众号
            case BusinessEventDataType.ClickPublic:
                DoClickPublic(data);
                break;
            // 点击公众号设置
            case BusinessEventDataType.ClickPublicSetting:
                DoClickPublicSetting(data);
                break;
            case BusinessEventDataType.RequestCancelGoBack:
                DoRequestCancelGoBack(data);
                break;
            case BusinessEventDataType.MucChangeEvent_TYPE_API_DELETE_GROUP:
                DoMucChangeEvent_TYPE_API_DELETE_GROUP(data);
                break;
            // 建群
            case BusinessEventDataType.MucChangeEvent_TYPE_API_CREATE_GROUP:
                DoMucChangeEvent_TYPE_API_CREATE_GROUP(data);
                break;
            //置顶
            case BusinessEventDataType.ChatTopEvent:
                DoChatTopEvent(data);
                break;

            }
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    private void DoChatTopEvent(BusinessEvent<object> data) {
        Refesh(string.Empty, false);
        //Init(false);
    }

    private void DoMucChangeEvent_TYPE_API_CREATE_GROUP(BusinessEvent<object> data) {
        try {
            Thread.Sleep(1000);
            if (data.data.ToStr() == string.Empty) return;
            MucTable table = data.data as MucTable;
            if (table == null) return;
            this.Dispatcher.BeginInvoke((Action)delegate () {

                bool flag = myListViewItems.Count(q => q.Contact.Equals(table.no)) > 0;
                if (flag == false) return;

                ChatSessionBean oldBean = myListViewItems.First(q => q.Contact.Equals(table.no));
                ListBoxLeft.SelectedItem = oldBean;
                ListBoxLeft.ScrollIntoView(ListBoxLeft.SelectedItem);
                //ListBoxLeft_SelectionChanged();

            });
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    private void DoMucChangeEvent_TYPE_API_DELETE_GROUP(BusinessEvent<object> data) {
        try {

            if (data.data.ToStr() == string.Empty) return;
            this.Dispatcher.BeginInvoke((Action)delegate () {
                int index = ListBoxLeft.SelectedIndex;
                if (index <=0) return;
                myListViewItems.Remove(ListBoxLeft.SelectedItem as ChatSessionBean);
                ListBoxLeft.SelectedIndex = 0;
                ListBoxLeft_SelectionChanged();

            });
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    private void DoClickPublicSetting(BusinessEvent<object> data) {
        try {

            if (data.data.ToStr() == string.Empty) return;
            this.Dispatcher.BeginInvoke((Action)delegate () {

                PublicAccountsSettingPage chatP =new PublicAccountsSettingPage();
                chatP.appId = data.data.ToStr();
                chatP.BtnBackOnClick -= BtnBackOnClick;
                chatP.BtnBackOnClick += BtnBackOnClick;
                ChatFrame.Navigate(chatP);


            });
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    private void DoClickPublic(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;

            this.Dispatcher.BeginInvoke((Action)delegate () {
                string appId = data.data.ToStr();
                if (appId == string.Empty) return;
                ChatPage chatP = ChatPage.getInstance();
                chatP.ChatSessionType = ChatSessionType.PUBLIC;
                chatP.BtnBackOnClick -= BtnBackOnClick; ;
                chatP.BtnBackOnClick += BtnBackOnClick;


                chatP.ClientuserId = string.Empty;
                chatP.MucNo = string.Empty;
                chatP.PublicAppId = appId;
                //有时候加载页面的时候 菜单栏没有加载完事，在这里提前加载下
                // chatP.publicMessage.AppId = appId;
                App.SelectChartSessionNo = appId;

                ChatFrame.Navigate(chatP);

                //设置已读
                MessageService.getInstance().setMessageRead(appId);
            });
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }


    /// <summary>
    /// 头像更换处理
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsDetailsChangeEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;

            //this.Dispatcher.BeginInvoke((Action)delegate() {
            ContactsTable dt = data.data as ContactsTable;
            if (dt == null) return;
            Refesh(dt.no,false);
            //});
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }

    }

    /// <summary>
    /// 修改chartsession
    /// </summary>
    /// <param Name="data"></param>
    public void DoChartSessionChangeEvent(BusinessEvent<object> data) {
        try {
            ChatSessionTable item = data.data as ChatSessionTable;
            if (item == null) return;
            //this.Dispatcher.BeginInvoke(new Action(() => {
            Refesh(item.user,true);
            //}));
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }

    }

    /// <summary>
    /// 聊天详情修改
    /// </summary>
    /// <param Name="data"></param>
    public void DoChatDetailedChangeEvent(BusinessEvent<object> data) {
        try {
            if (this.IsVisible == false) return;
            string user = data.data.ToStr();
            if (user == string.Empty) return;

            //this.Dispatcher.BeginInvoke(new Action(() => {
            if (string.IsNullOrEmpty(App.SelectChartSessionNo)) return;
            Refesh(user,false);

            //}));
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    /// <summary>
    /// 群名  头像 换了
    /// </summary>
    /// <param Name="data"></param>
    public void DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(BusinessEvent<object> data) {

        try {
            if (data.data.ToStr() == string.Empty) return;
            Thread.Sleep(500);
            //this.Dispatcher.BeginInvoke((Action)delegate() {
            MucTable dt = data.data as MucTable;
            if (dt == null) return;
            Refesh(dt.no,false);
            //});
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }



    public void Refesh(string contact, bool gotoItem) {

        try {
            lock (this) {
                if (string.IsNullOrEmpty(contact)) {
                    Init(gotoItem);
                    return;
                }

                Thread.Sleep(500);

                // 会话类型
                ChatSessionType type = ToolsHelper.getChatSessionTypeByNo(contact);
                if (type == ChatSessionType.APPMSG) {
                    contact = Constants.APPMSG_FLAG;
                } else if (type == ChatSessionType.TODO_TASK) {
                    contact = Constants.TODO_TASK_FLAG;
                } else if (type == ChatSessionType.PUBLIC) {
                    contact = Constants.PUBLIC_ACCOUNT_FLAG;
                }
                bool flag = myListViewItems.Where(q => q.Contact.Equals(contact)).Count() > 0;
                //有则修改 无责添加
                if (flag) {
                    ChatSessionBean oldBean = myListViewItems.First(q => q.Contact.Equals(contact));


                    List<ChatSessionBean> dt = ChatSessionService.getInstance().findAllChatSession();

                    bool newBeanFlag = dt.Where(q => q.Contact.Equals(contact)).Count() > 0;
                    if (newBeanFlag == false) return;
                    ChatSessionBean newBean = dt.First(q => q.Contact.Equals(contact));
                    //this.Dispatcher.BeginInvoke(new Action(() => {

                    oldBean.LastMessage = newBean.LastMessage;
                    if (newBean.AvatarPath != null) {
                        oldBean.AvatarPath = newBean.AvatarPath;
                    }
                    oldBean.AvatarStorageRecordId = newBean.AvatarStorageRecordId;
                    oldBean.Name = newBean.Name;
                    if (contact == App.SelectChartSessionNo &&
                            ToolsHelper.getChatSessionTypeByNo(contact) != ChatSessionType.TODO_TASK) {
                        oldBean.NewMsgCount = 0;
                    } else {
                        oldBean.NewMsgCount = newBean.NewMsgCount;
                    }

                    oldBean.Quiet = newBean.Quiet;
                    oldBean.ShowYuan = newBean.ShowYuan;
                    oldBean.Timestamp = newBean.Timestamp;
                    oldBean.Top = newBean.Top;
                    oldBean.DateStr = newBean.DateStr;
                    oldBean.ChatTime = newBean.ChatTime;
                    oldBean.Chatdraft = newBean.Chatdraft;
                    oldBean.Atme = newBean.Atme;


                    if (gotoItem && contact != Constants.APPMSG_FLAG
                            && contact != Constants.TODO_TASK_FLAG) {
                        this.Dispatcher.BeginInvoke(new Action(() => {

                            bool select = ListBoxLeft.SelectedItem == oldBean;

                            if (myListViewItems.Contains(oldBean)) {
                                myListViewItems.Remove(oldBean);

                            }
                            myListViewItems.Insert(App.ChartSessionCount,
                                                   oldBean);

                            if(select) {
                                ListBoxLeft.SelectedItem = oldBean;
                                ListBoxLeft.ScrollIntoView(ListBoxLeft.SelectedItem);
                            }
                        }));

                    }

                } else {
                    List<ChatSessionBean> dt = ChatSessionService.getInstance().findAllChatSession();
                    bool newBeanFlag = dt.Where(q => q.Contact.Equals(contact)).Count() > 0;
                    if (newBeanFlag == false) {
                        return;
                    }


                    ChatSessionBean newBean = dt.First(q => q.Contact.Equals(contact));
                    if (contact == App.SelectChartSessionNo) {
                        newBean.NewMsgCount = 0;
                    }
                    if (myListViewItems.Count < App.ChartSessionCount) return;
                    this.Dispatcher.BeginInvoke(new Action(() => {

                        if (!myListViewItems.Contains(newBean)) {
                            myListViewItems.Insert(App.ChartSessionCount,
                                                   newBean);

                            // Console.WriteLine("insert"+ newBean.Name);
                            //ListBoxLeft.SelectedIndex = App.ChartSessionCount;
                        }
                    }));
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    /// <summary>
    /// 初始化方法  gotoItem=false 的话就刷新列表不用点击事件 否则的话置顶会跳出置顶界面
    /// </summary>
    private void Init(bool gotoItem) {
        try {

            ListBoxLeft.Dispatcher.BeginInvoke((Action)delegate () {
                List<ChatSessionBean> dt = ChatSessionService.getInstance().findAllChatSession();


                myListViewItems.Clear();
                for (int i = 0; i < dt.Count; i++) {
                    //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                    bool flag = myListViewItems.Where(q => q.Contact.Equals(dt[i].Contact)).Count() > 0;
                    if(flag==false) {

                        myListViewItems.Add(dt[i]);
                    }

                }


                if (AddChartUserId != 0  ) {
                    VcardsTable model = VcardService.getInstance().findByClientuserId(AddChartUserId);
                    if (model == null) return;
                    App.SelectChartSessionNo = model.no;
                    if(dt.Count(q => q.Contact.Equals(AddChartUserId.ToStr())) == 0) {
                        LoadLxrChart(AddChartUserId);
                    }
                }

                if (AddChartGroupNo != string.Empty  ) {
                    App.SelectChartSessionNo = AddChartGroupNo;
                    if (dt.Count(q => q.Contact.Equals(AddChartGroupNo.ToStr())) == 0) {
                        LoadQlChart(AddChartGroupNo);
                    }

                }
                if (AddPublicAccounts != string.Empty ) {
                    App.SelectChartSessionNo = Constants.PUBLIC_ACCOUNT_FLAG;
                    if (dt.Count(q => q.Contact.Equals(AddPublicAccounts.ToStr())) == 0) {
                        LoadPublicAccounts(AddPublicAccounts);
                    }
                }

                for (int i = 0; i < ListBoxLeft.Items.Count; i++) {
                    ChatSessionBean c = ListBoxLeft.Items[i] as ChatSessionBean;
                    if (c == null) continue;
                    if (c.Contact == App.SelectChartSessionNo) {
                        ListBoxLeft.SelectedItem = c;
                        ListBoxLeft.SelectedIndex = i;
                        ListBoxLeft.ScrollIntoView(ListBoxLeft.SelectedItem);
                        if(gotoItem==true) {
                            ListBoxLeft_SelectionChanged();
                        }

                        return;
                    }
                }
            });



        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    private AdornerLayer searchAdornerLayer = null;
    private SearchAdorner searchAdorner = null;

    /// <summary>
    /// 画面初始化
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {

        var task1 = new Task(() => {
            Refesh(string.Empty,true);
        });

        task1.Start();
        //this.Dispatcher.BeginInvoke((Action)delegate () {
        ContactsImagePage imagePage = new Page.ContactsImagePage();
        imagePage.Message = true;
        ChatFrame.Navigate(imagePage);
        //});
        //公众号搜索
        //PublicAccountsFindPage a =  PublicAccountsFindPage.getInstance();
        //ChatFrame.Navigate(a);

        // 初始化搜索窗体
        searchAdornerLayer = AdornerLayer.GetAdornerLayer(ListBoxLeft);
        if (searchAdornerLayer != null) {
            searchAdornerLayer.Opacity = 1;
            searchAdorner = new SearchAdorner(ListBoxLeft);
            searchAdorner.ResultControl.SetRedirectCallBack(() => {
                SearchText.Text = "";
            });


            searchAdornerLayer.Add(searchAdorner);
            searchAdornerLayer.Visibility = Visibility.Hidden;
        }
    }


/// <summary>
///加载联系人单击发送后，跳转到聊天界面
/// </summary>
/// <param Name="deptid"></param>
    public void LoadLxrChart(int id) {
        try {


            VcardsTable model = VcardService.getInstance().findByClientuserId(AddChartUserId);
            if (model == null) return;
            AddChartUserId = 0;
            ChatSessionTable chatSessionModel = ChatSessionService.getInstance().findByNo(model.no);
            if (chatSessionModel == null) {
                ChatSessionBean info = new ChatSessionBean();
                VcardsTable table = VcardService.getInstance().findByClientuserId(id);
                if (table!=null) {
                    info.Name = ContactsServices.getInstance().getContractNameByNo(table.no);
                    info.AvatarStorageRecordId = table.avatarStorageRecordId;
                    info.AvatarPath = ImageHelper.loadAvatarPath(info.AvatarStorageRecordId);
                    info.Contact = table.no;
                }

                info.ChatSessionType = ChatSessionType.CHAT;
                info.LastMessage =string.Empty;
                int count = MessageService.getInstance().countOfUnreadMessages(table.no);
                info.NewMsgCount = count;
                info.Top = SettingService.getInstance().isTop(table.no);
                info.Quiet = SettingService.getInstance().isQuiet(table.no);


                if (!myListViewItems.Contains(info)) {
                    myListViewItems.Insert(App.ChartSessionCount, info);
                }

            }

        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }


/// <summary>
///加载群聊人单击发送后，跳转到聊天界面
/// </summary>
/// <param Name="deptid"></param>
    public void LoadQlChart(string no) {


        try {
            AddChartGroupNo = string.Empty;
            MucTable table = MucServices.getInstance().FindGroupByNo(no);

            ChatSessionTable chatSessionModel = ChatSessionService.getInstance().findByNo(no);
            if (chatSessionModel == null) {
                ChatSessionBean info = new ChatSessionBean();

                if (table != null) {
                    info.Name = table.name;
                    info.AvatarStorageRecordId = table.avatarStorageRecordId;
                    info.AvatarPath = ImageHelper.loadAvatarPath(info.AvatarStorageRecordId);
                    info.Contact = table.no;
                }

                info.ChatSessionType = ChatSessionType.MUC;
                info.LastMessage = string.Empty;
                int count = MessageService.getInstance().countOfUnreadMessages(table.no);
                info.NewMsgCount = count;
                info.Top = SettingService.getInstance().isTop(table.no);
                info.Quiet = SettingService.getInstance().isQuiet(table.no);

                if (!myListViewItems.Contains(info)) {
                    myListViewItems.Insert(App.ChartSessionCount, info);
                }


            }
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

/// <summary>
///加载公众号
/// </summary>
/// <param Name="deptid"></param>
    public void LoadPublicAccounts(string no) {
        try {
            int count = 0;
            AddPublicAccounts = string.Empty;
            // 如果已经有该人员的chartsession 就直接定位到这个人
            for (int i = 0; i < ListBoxLeft.Items.Count; i++) {
                ChatSessionBean c = ListBoxLeft.Items[i] as ChatSessionBean;
                if (c.Contact == Constants.PUBLIC_ACCOUNT_FLAG) {
                    ListBoxLeft.SelectedItem = c;
                    ListBoxLeft.ScrollIntoView(ListBoxLeft.SelectedItem);
                    ListBoxLeft_SelectionChanged();
                    count++;
                    break;
                }
            }
            if(count==0) {
                //没有加载公众号chart 添加
                ChatSessionBean bean = ChatSessionService.getInstance().getChatSessionPublic();
                if (!myListViewItems.Contains(bean)) {
                    myListViewItems.Insert(App.ChartSessionCount, bean);
                }


                ListBoxLeft.SelectedItem = bean;
                ListBoxLeft_SelectionChanged();
            }


            //公众号 直接跳到公众号聊天界面
            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = no;
            Businessdata.eventDataType = BusinessEventDataType.ClickPublic;
            EventBusHelper.getInstance().fireEvent(Businessdata);

        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }



    private RelayCommand listMouseLeftButtonUpCommand = null;
    public RelayCommand ListMouseLeftButtonUpCommand {
        get {
            if (listMouseLeftButtonUpCommand == null) {
                listMouseLeftButtonUpCommand = new RelayCommand(() => {
                    ListBoxLeft_SelectionChanged();
                });
            }
            return listMouseLeftButtonUpCommand;
        }
    }

    /// <summary>
    /// 右键事件
    /// </summary>
    private RelayCommand listMouseRightButtonDownCommand = null;
    public RelayCommand ListMouseRightButtonDownCommand {
        get {
            if (listMouseRightButtonDownCommand == null) {
                listMouseRightButtonDownCommand = new RelayCommand(() => {
                    ListBoxLeft_RightButtonDown();
                });
            }
            return listMouseRightButtonDownCommand;
        }
    }
    public ContextMenu menuClassify = new ContextMenu();
    /// <summary>
    /// 加载分组右键菜单
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    public virtual void CreatContextMenu(ChatSessionBean bean) {
        menuClassify.Tag = bean;
        Style styleMenu = this.FindResource("MenuItem") as Style;
        MenuItem menuItemTop = new MenuItem();
        MenuItem menuItemUnTop = new MenuItem();
        MenuItem menuItemQuite = new MenuItem();
        MenuItem menuItemUnQuite = new MenuItem();
        MenuItem menuItemDel = new MenuItem();

        menuItemTop.Style = styleMenu;
        menuItemUnTop.Style = styleMenu;
        menuItemDel.Style = styleMenu;
        menuItemQuite.Style = styleMenu;
        menuItemUnQuite.Style = styleMenu;

        menuClassify.Items.Clear();
        Style style = this.FindResource("PublicContextMenu") as Style;
        menuClassify.Style = style;

        menuItemTop.Header = "置顶";
        menuItemTop.Name = "top";
        menuItemTop.Click += MenuItemTop_Click; ;
        menuItemTop.Tag = "";

        menuItemUnTop.Header = "取消置顶";
        menuItemUnTop.Name = "unTop";
        menuItemUnTop.Click += MenuItemUnTop_Click; ;
        menuItemUnTop.Tag = "";

        menuItemQuite.Header = "消息免打扰";
        menuItemQuite.Name = "quite";
        menuItemQuite.Click += MenuItemQuite_Click; ;
        menuItemQuite.Tag = "";

        menuItemDel.Header = "删除聊天";
        menuItemDel.Name = "del";
        menuItemDel.Click += MenuItemDel_Click; ;
        menuItemDel.Tag = "";

        menuItemUnQuite.Header = "开启消息提醒";
        menuItemUnQuite.Name = "unQuite";
        menuItemUnQuite.Click += MenuItemUnQuite_Click; ;
        menuItemUnQuite.Tag = "";


        menuClassify.Items.Add(menuItemTop);
        menuClassify.Items.Add(menuItemUnTop);

        menuClassify.Items.Add(menuItemQuite);
        menuClassify.Items.Add(menuItemUnQuite);
        menuClassify.Items.Add(menuItemDel);

        if(bean.Quiet==true) {
            menuItemQuite.Visibility = Visibility.Collapsed;
            menuItemUnQuite.Visibility = Visibility.Visible;
        } else {
            menuItemQuite.Visibility = Visibility.Visible;
            menuItemUnQuite.Visibility = Visibility.Collapsed;
        }

        if (bean.Top == true) {
            menuItemTop.Visibility = Visibility.Collapsed;
            menuItemUnTop.Visibility = Visibility.Visible;
        } else {
            menuItemTop.Visibility = Visibility.Visible;
            menuItemUnTop.Visibility = Visibility.Collapsed;
        }

        //menuClassify.HorizontalOffset = 5;
        menuClassify.Width = 110;
        //menuClassify.VerticalOffset = -5;
        //menuClassify.HorizontalOffset = 13;
        //menuClassify.HorizontalAlignment = HorizontalAlignment.Center;
        menuClassify.IsOpen = true;
    }

    private void MenuItemDel_Click(object sender, RoutedEventArgs e) {
        ChatSessionBean bean = menuClassify.Tag as ChatSessionBean;

        ChatSessionService.getInstance().deleteChatSessionByNo(bean.Contact);

        //this.Dispatcher.BeginInvoke((Action)delegate () {
        int index = ListBoxLeft.SelectedIndex;
        if (index <= 0) return;
        myListViewItems.Remove(ListBoxLeft.SelectedItem as ChatSessionBean);
        ListBoxLeft.SelectedIndex = 0;
        ListBoxLeft_SelectionChanged();

        //});
    }

    private void MenuItemUnQuite_Click(object sender, RoutedEventArgs e) {
        ChatSessionBean bean = menuClassify.Tag as ChatSessionBean;
        if (bean.ChatSessionType == ChatSessionType.CHAT) {
            VcardsTable dt = VcardService.getInstance().findByNo(bean.Contact);
            SettingService.getInstance().EnableNoDisturbFriend(dt.clientuserId, false);
        } else if (bean.ChatSessionType == ChatSessionType.MUC) {
            MucTable dt = MucServices.getInstance().FindGroupByNo(bean.Contact);
            ContactsApi.enableGroupNoDisturb(dt.mucId, false);
        }
        ListBoxLeft.SelectedItem = bean;
        ListBoxLeft_SelectionChanged();
    }

    private void MenuItemQuite_Click(object sender, RoutedEventArgs e) {
        ChatSessionBean bean = menuClassify.Tag as ChatSessionBean;
        if (bean.ChatSessionType == ChatSessionType.CHAT) {
            VcardsTable dt = VcardService.getInstance().findByNo(bean.Contact);
            SettingService.getInstance().EnableNoDisturbFriend(dt.clientuserId, true);
        } else if (bean.ChatSessionType == ChatSessionType.MUC) {
            MucTable dt = MucServices.getInstance().FindGroupByNo(bean.Contact);
            ContactsApi.enableGroupNoDisturb(dt.mucId, true);
        }
        ListBoxLeft.SelectedItem = bean;
        ListBoxLeft_SelectionChanged();
    }

    private void MenuItemUnTop_Click(object sender, RoutedEventArgs e) {
        ChatSessionBean bean = menuClassify.Tag as ChatSessionBean;
        if(bean.ChatSessionType==ChatSessionType.CHAT) {
            VcardsTable dt= VcardService.getInstance().findByNo(bean.Contact);
            SettingService.getInstance().SetTopmost(dt.clientuserId, false);
        } else if (bean.ChatSessionType == ChatSessionType.MUC) {
            MucTable dt = MucServices.getInstance().FindGroupByNo(bean.Contact);
            ContactsApi.setGroupTopmost(dt.mucId, false);
        }
        ListBoxLeft.SelectedItem = bean;
        ListBoxLeft_SelectionChanged();
    }

    private void MenuItemTop_Click(object sender, RoutedEventArgs e) {
        ChatSessionBean bean = menuClassify.Tag as ChatSessionBean;
        if (bean.ChatSessionType == ChatSessionType.CHAT) {
            VcardsTable dt = VcardService.getInstance().findByNo(bean.Contact);
            SettingService.getInstance().SetTopmost(dt.clientuserId, true);
        } else if (bean.ChatSessionType == ChatSessionType.MUC) {
            MucTable dt = MucServices.getInstance().FindGroupByNo(bean.Contact);
            ContactsApi.setGroupTopmost(dt.mucId, true);
        }
        ListBoxLeft.SelectedItem = bean;
        ListBoxLeft_SelectionChanged();
    }

    private void ListBoxLeft_RightButtonDown() {

        try {
            object o = ListBoxLeft.SelectedItem;
            if (o == null)
                return;

            ChatSessionBean p = o as ChatSessionBean;
            if (p == null)
                return;

            if ((p.ChatSessionType == ChatSessionType.CHAT || p.ChatSessionType == ChatSessionType.MUC)
                    && p.Contact != App.AccountsModel.no) {
                CreatContextMenu(p);
            }


        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }
    /// <summary>
    /// 选择聊天窗口
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ListBoxLeft_SelectionChanged() {

        try {
            object o = ListBoxLeft.SelectedItem;
            if (o == null)
                return;

            ChatSessionBean p = o as ChatSessionBean;
            ListBoxLeft.ScrollIntoView(p);
            _listBoxSelectIndex = ListBoxLeft.SelectedIndex;
            //待办
            if (p.ChatSessionType == ChatSessionType.TODO_TASK) {
                TodoTaskPage page =  TodoTaskPage.getInstance();
                page.BtnSetClick -= page_BtnSetClick;
                page.BtnSetClick += page_BtnSetClick;
                ChatFrame.Navigate(page);
                App.SelectChartSessionNo = Constants.TODO_TASK_FLAG;
                //return;
                //应用消息
            } else if (p.ChatSessionType == ChatSessionType.APPMSG) {
                AppMsgPage page =  AppMsgPage.getInstance();
                page.BtnSetClick -= page_BtnSetClick;
                page.BtnSetClick += page_BtnSetClick;
                ChatFrame.Navigate(page);
                App.SelectChartSessionNo = Constants.APPMSG_FLAG;


                //设置已读
                Thread t = new Thread(()=> {
                    MessageService.getInstance().setMessageRead(p.Contact);
                    if(p.NewMsgCount>0) {
                        MessageService.getInstance().sendReadMessage(Constants.APPMSG_FLAG);
                    }

                });
                t.IsBackground = true;
                t.Start();

                //p.UserNo = p.UserNo;
                //p.Refesh();
            } else if (p.ChatSessionType == ChatSessionType.PUBLIC) {
                PublicAccountsChartSessionPage page = PublicAccountsChartSessionPage.getInstance();
                //page.BtnSetClick -= page_BtnSetClick;
                //page.BtnSetClick += page_BtnSetClick;
                //page.ClickPublicAccountsChartSession -= Page_ClickPublicAccountsChartSession;
                //page.ClickPublicAccountsChartSession += Page_ClickPublicAccountsChartSession;



                ChatFrame.Navigate(page);
                App.SelectChartSessionNo = Constants.PUBLIC_ACCOUNT_FLAG;
            } else { //人或者群的聊天
                ChatPage chatP = ChatPage.getInstance();
                chatP.ChatSessionType = p.ChatSessionType;
                chatP.BtnAddPersonClick -= chatP_BtnAddPersonClick;
                chatP.BtnAddPersonClick += chatP_BtnAddPersonClick;
                chatP.BtnGroupChatDetailedClick -= chatP_BtnGroupChatDetailedClick;
                chatP.BtnGroupChatDetailedClick += chatP_BtnGroupChatDetailedClick;
                chatP.BtnSingleChatDetailedClick -= chatP_BtnSingleChatDetailedClick;
                chatP.BtnSingleChatDetailedClick += chatP_BtnSingleChatDetailedClick;
                if (p.ChatSessionType == ChatSessionType.CHAT) {
                    VcardsTable model = VcardService.getInstance().findByNo(p.Contact);

                    chatP.ClientuserId = model.clientuserId;
                    chatP.MucNo = string.Empty;

                } else if(p.ChatSessionType == ChatSessionType.MUC) {

                    chatP.ClientuserId = string.Empty;
                    chatP.MucNo = p.Contact.ToStr();
                } else if (p.ChatSessionType == ChatSessionType.NOTICE) {
                    chatP.ClientuserId = Constants.SYSTEM_NOTICE_FLAG;
                    chatP.MucNo = string.Empty;
                } else {

                    chatP.ClientuserId = string.Empty;
                    chatP.MucNo = string.Empty;
                }
                App.SelectChartSessionNo = p.Contact;
                ChatFrame.Navigate(chatP);

                Thread t = new Thread(() => {
                    if (p.NewMsgCount > 0) {
                        MessageService.getInstance().sendReadMessage(p.Contact);
                    }
                });
                t.IsBackground = true;
                t.Start();
                //更新atme
                ChatSessionService.getInstance().updateAtme(p.Contact, false);
                //设置已读
                MessageService.getInstance().setMessageRead(p.Contact);

                chatP.sendMessage.TxtMessage.Focus();
            }
            Thread tRefesh = new Thread(() => {
                Refesh(App.SelectChartSessionNo,false);
            });
            tRefesh.IsBackground = true;
            tRefesh.Start();


        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }





/// <summary>
/// 待办事项点击设置
/// </summary>
/// <param Name="obj"></param>
    void page_BtnSetClick(ChatSessionType obj) {
        TodoSetingPage page = TodoSetingPage.getInstance();
        page.ChatSessionType=obj;
        page.BtnBackOnClick -= BtnBackOnClick;
        page.BtnBackOnClick += BtnBackOnClick;
        ChatFrame.Navigate(page);
    }

/// <summary>
/// 单聊详情按钮
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void chatP_BtnSingleChatDetailedClick(string obj) {
        try {
            SingleChatDetailedPage singleChatDetailedPage = new SingleChatDetailedPage();
            singleChatDetailedPage.userNo = App.SelectChartSessionNo;
            singleChatDetailedPage.BtnBackOnClick -= BtnBackOnClick;
            singleChatDetailedPage.BtnBackOnClick += BtnBackOnClick;
            ChatFrame.Navigate(singleChatDetailedPage);



        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

/// <summary>
/// 群聊详情按钮
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void chatP_BtnGroupChatDetailedClick(string obj) {
        try {
            GroupChatDetailedPage groupChatDetailedPage = GroupChatDetailedPage.getInstance();

            groupChatDetailedPage.MucNo = App.SelectChartSessionNo;
            groupChatDetailedPage.BtnBackOnClick -= BtnBackOnClick;
            groupChatDetailedPage.BtnBackOnClick+=BtnBackOnClick;

            groupChatDetailedPage.ClickAllGroup -= groupChatDetailedPage_ClickAllGroup;
            groupChatDetailedPage.ClickAllGroup+=groupChatDetailedPage_ClickAllGroup;

            ChatFrame.Navigate(groupChatDetailedPage);
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }
    private void DoRequestCancelGoBack(BusinessEvent<object> data) {

        try {
            this.Dispatcher.BeginInvoke(new Action(() => {
                PublicAccountsChartSessionPage page = PublicAccountsChartSessionPage.getInstance();
                ChatFrame.Navigate(page);
                App.SelectChartSessionNo = Constants.PUBLIC_ACCOUNT_FLAG;


            }));

        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }
    private void BtnBackOnClick(string obj) {
        try {
            if (ChatFrame.NavigationService.CanGoBack) {
                ChatFrame.NavigationService.GoBack();
                //ChatFrame.Refresh();
                // object a = ChatFrame.Content;
                //不显示返回按钮
                //BtnBack.Visibility = Visibility.Collapsed;
                //ListBoxLeft.SelectedIndex = -1;
                //ListBoxLeft.SelectedIndex = _listBoxSelectIndex;
            }
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }

    private void chatP_BtnAddPersonClick(string obj) {

    }



    private void groupChatDetailedPage_ClickAllGroup(string mucNo, string titel) {

        AllMucMemberPage page = new AllMucMemberPage();
        page.BtnBackOnClick -= BtnBackOnClick;
        page.BtnBackOnClick += BtnBackOnClick;
        page.MucNo = App.SelectChartSessionNo;
        page.Count = titel;


        ChatFrame.Navigate(page);
    }


/// <summary>
/// 添加按钮点击事件
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void BtnAddPerson_OnClick(object sender, RoutedEventArgs e) {
        try {


        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }



/// <summary>
/// 返回按钮
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        try {
            if (ChatFrame.NavigationService.CanGoBack) {
                ChatFrame.NavigationService.GoBack();
                object a=     ChatFrame.Content;
                //不显示返回按钮
                //BtnBack.Visibility = Visibility.Collapsed;
                ListBoxLeft.SelectedIndex = -1;
                ListBoxLeft.SelectedIndex = _listBoxSelectIndex;
            }
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }

    }

/// <summary>
/// 鼠标点击的时候需要把一些小的窗口关闭
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
        try {
            if (App.PersonDetailedWindow != null && App.PersonDetailedWindow.IsVisible == true) {
                App.PersonDetailedWindow.Close();
            }
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }
    }



/// <summary>
/// 建立群
/// </summary>
/// <param Name="sender"></param>
/// <param Name="e"></param>
    private void BtnAdd_Click(object sender, RoutedEventArgs e) {
        try {
            CreateGroup group = new CreateGroup();
            //group.Topmost = true;
            App.createGroup = group;
            group.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(PcOA), ex);
        }

    }





    private void SearchText_TextChanged(object sender, TextChangedEventArgs e) {
        if (searchAdornerLayer != null) {
            if (String.IsNullOrEmpty(SearchText.Text.Trim())) {
                searchAdornerLayer.Visibility = Visibility.Hidden;
            } else {
                if (searchAdorner.ResultControl.Search(SearchText.Text.Trim()) > 0) {
                    searchAdornerLayer.Visibility = Visibility.Visible;
                } else {
                    searchAdornerLayer.Visibility = Visibility.Hidden;
                }
            }
        }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e) {
        // 清除检索关键字及窗口
        SearchText.Text = "";
    }

    private void SearchText_OnKeyUp(object sender, KeyEventArgs e) {
        switch (e.Key) {
        case Key.Up:
            searchAdorner.ResultControl.ItemUpKeyUpCommand();
            e.Handled = true;
            break;
        case Key.Down:
            searchAdorner.ResultControl.ItemDownKeyUpCommand();
            e.Handled = true;
            break;
        }
    }

    private void SearchText_OnPreviewKeyDown(object sender, KeyEventArgs e) {
        switch (e.Key) {
        case Key.Enter:
            searchAdorner.ResultControl.ItemClickCommandFun();
            e.Handled = true;
            break;
        }
    }

}
}
