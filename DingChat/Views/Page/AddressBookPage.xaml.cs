using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models.Tables;
using EventBus;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Adorners;
using cn.lds.chatcore.pcw.Views.Page.PublicAccounts;
using GalaSoft.MvvmLight.Command;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// Window2.xaml 的交互逻辑
/// </summary>
public partial class AddressBookPage : System.Windows.Controls.Page {

    private static AddressBookPage instance = null;
    public static AddressBookPage getInstance() {
        if (instance == null) {
            instance = new AddressBookPage();
        }
        return instance;
    }

    public AddressBookPage() {
        InitializeComponent();
        this.DataContext = this;
        try {

            TreeOrg.ClickTree += Tree_ClickTree;
            p.ClickOrgMember += p_ClickOrgMember;
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
        CheckBean beanZz = new CheckBean();
        beanZz.LogoPath = ImageHelper.getSysImagePath("Org.png");
        beanZz.Name = "组织机构";
        ChkZzjg.DataContext = beanZz;

        CheckBean beanQl = new CheckBean();
        beanQl.LogoPath = ImageHelper.getSysImagePath("Groupchat.png");
        beanQl.Name = "群聊";
        ChkQl.DataContext = beanQl;

        CheckBean beanGzh = new CheckBean();
        beanGzh.LogoPath = ImageHelper.getSysImagePath("Subscription.png");
        beanGzh.Name = "公众号";
        ChkGzh.DataContext = beanGzh;

        CheckBean beanLxr = new CheckBean();
        beanLxr.LogoPath = ImageHelper.getSysImagePath("Contact.png");
        beanLxr.Name = "常用联系人";
        ChkLxr.DataContext = beanLxr;
    }
    private ObservableCollection<PublicAccountsTable> myGzhItems = new ObservableCollection<PublicAccountsTable>();
    public ObservableCollection<PublicAccountsTable> MyGzhItems {
        get {
            return myGzhItems;
        } set {
            myGzhItems = value;
        }
    }

    private ObservableCollection<ContactsTable> myLxrItems = new ObservableCollection<ContactsTable>();
    public ObservableCollection<ContactsTable> MyLxrItems {
        get {
            return myLxrItems;
        } set {
            myLxrItems = value;
        }
    }

    private ObservableCollection<MucTable> myQlItems = new ObservableCollection<MucTable>();
    public ObservableCollection<MucTable> MyQlItems {
        get {
            return myQlItems;
        } set {
            myQlItems = value;
        }
    }
    // 变量定义

    //选中树 取出选中的orgid
    int  _selectedOrganizationId = 0;


    public PublicAccountsDetailedPage publicAccountsPage = new PublicAccountsDetailedPage();
    public LxrDetailedPage lxrDetailedPage = LxrDetailedPage.getInstance();

    public QlDetailedPage qlDetailedPage = new QlDetailedPage();

    OrganizationMembePage p = new OrganizationMembePage();




    public bool loadFindPublic = false;
    private RelayCommand clickPublicCommand = null;

    public RelayCommand ClickPublicCommand {
        get {
            if (clickPublicCommand == null) {
                clickPublicCommand = new RelayCommand(() => {

                    PublicAccountsFindPage findPage = PublicAccountsFindPage.getInstance();

                    FrameChat.Navigate(findPage);
                    //if (findPage != null) {
                    //    loadFindPublic = true;
                    //}
                });
                //findPage = null;
            }
            return clickPublicCommand;
        }
    }

    /// <summary>
    /// 点击组织人 跳转到名片
    /// </summary>
    /// <param Name="obj"></param>
    void p_ClickOrgMember(int obj,string memberId,string tenantNo) {
        try {
            LxrDetailedPage lxrDetailedPage = LxrDetailedPage.getInstance();
            lxrDetailedPage.lxrDetailedType = LxrDetailedType.Org;
            lxrDetailedPage.tenantNo = tenantNo;
            lxrDetailedPage.memberId = memberId;
            lxrDetailedPage.Id = obj;

            lxrDetailedPage.BnBack -= lxrDetailedPage_BnBack;
            lxrDetailedPage.BnBack += lxrDetailedPage_BnBack;
            FrameChat.Navigate(lxrDetailedPage);
        } catch (Exception ex) {
            Log.Error(typeof (AddressBookPage), ex);
        }

    }

    /// <summary>
    /// 名片界面点击返回
    /// </summary>
    /// <param Name="obj"></param>
    void lxrDetailedPage_BnBack(object obj) {
        try {
            //p.OrganizationId = _selectedOrganizationId;
            //FrameChat.Navigate(p);
            if (FrameChat.NavigationService.CanGoBack) {
                FrameChat.NavigationService.GoBack();
            }
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 业务事件监听
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {

        try {
            switch (data.eventDataType) {

            // 群保存通讯录
            case BusinessEventDataType.MucSavedAsContactChangeEvent:
                DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(data);
                break;
            // 群名 头像 换了
            case BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME:
                DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(data);
                break;
            // 好友头像换了
            case BusinessEventDataType.ContactsDetailsChangeEvent:
                DoContactsDetailsChangeEvent(data);
                break;
            // 好友变化
            case BusinessEventDataType.ContactsChangedEvent:
                DoContactsChangedEvent(data);
                break;
            // 组织机构变化
            case BusinessEventDataType.OrgChangedEvent:
                DoOrgChangedEvent(data);
                break;
            // 公众号变化
            case BusinessEventDataType.PublicAccountChangedEvent:
                DoPublicAccountChangedEvent(data);
                break;

            // 好友变化 删好友
            case BusinessEventDataType.ContactsChangedEvent_TYPE_API_DELETE_Contacts:
                DoContactsChangedEvent_TYPE_API_DELETE_Contacts(data);
                break;
            // 删除群
            case BusinessEventDataType.MucChangeEvent_TYPE_API_DELETE_GROUP:
                DoMucChangeEvent_TYPE_API_DELETE_GROUP(data);
                break;
            // 删除公众号
            case BusinessEventDataType.PublicAccountRemovedEvent:
                DoPublicAccountRemovedEvent(data);
                break;
            // 加载完成
            case BusinessEventDataType.LoadingOk:
                DoLoadingOk(data);
                break;
            // 创建群完事
            case BusinessEventDataType.MucChangeEvent_TYPE_API_CREATE_GROUP:
                DoMucChangeEvent_TYPE_API_CREATE_GROUP(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }

    }

    private void DoPublicAccountChangedEvent(BusinessEvent<object> data) {
        try {

            this.Dispatcher.BeginInvoke((Action)delegate () {
                LoadPublicAccounts(null);
            });

        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private void DoPublicAccountRemovedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            //删除人
            this.Dispatcher.BeginInvoke((Action)delegate () {

                NotificationHelper.ShowSuccessMessage("取消关注成功！");

                LoadPublicAccounts(null);
                ClearSelect();

            });
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private void DoMucChangeEvent_TYPE_API_CREATE_GROUP(BusinessEvent<object> data) {
        this.Dispatcher.BeginInvoke((Action)delegate () {
            LoadQl(null);
            if (App.createGroup != null) App.createGroup.Close();
        });

        if (data.data!=null) {
            MucTable dt = data.data as MucTable;
            BusinessEvent<object> businessdata = new BusinessEvent<object>();

            businessdata.data = dt.no;
            businessdata.eventDataType = BusinessEventDataType.RedirectMucChatSessionEvent;
            EventBusHelper.getInstance().fireEvent(businessdata);
        }
    }

    /// <summary>
    /// 群名 头像 换了
    /// </summary>
    /// <param Name="data"></param>
    public void DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                MucTable dt = data.data as MucTable;
                if (dt == null) return;
                LoadQl(null);
            });
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 好友头像换了
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsDetailsChangeEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;

            this.Dispatcher.BeginInvoke((Action)delegate() {
                ContactsTable dt = data.data as ContactsTable;
                LoadLxr(null);
            });
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    public void DoOrgChangedEvent(BusinessEvent<object> data) {
        try {

            this.Dispatcher.BeginInvoke((Action)delegate () {
                TreeOrg.LoadTree();
            });

        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 好友变化
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsChangedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() != string.Empty) {
                //删除人
                this.Dispatcher.BeginInvoke((Action)delegate() {
                    LoadLxr(null);
                    ClearSelect();

                });
            } else {
                //加人
                LoadLxr(null);
            }
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 好友变化 删好友
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsChangedEvent_TYPE_API_DELETE_Contacts(BusinessEvent<object> data) {
        try {
            //删除人
            if (data.data.ToStr() == string.Empty) return;
            this.Dispatcher.BeginInvoke((Action)delegate() {
                LoadLxr(null);

                Thread.Sleep(500);

                ClearSelect();

            });
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 删除群
    /// </summary>
    /// <param Name="data"></param>
    public void DoMucChangeEvent_TYPE_API_DELETE_GROUP(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            //删除群
            this.Dispatcher.BeginInvoke((Action)delegate() {
                string no = data.data.ToStr();
                LoadQl(null);

                ClearSelect();

            });
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 数据加载完成
    /// </summary>
    /// <param Name="data"></param>
    public void DoLoadingOk(BusinessEvent<object> data) {
        try {
            //this.Dispatcher.BeginInvoke((Action)delegate() {
            //    this.LoadOrg(null);
            //    this.LoadLxr(null);
            //});
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 树的点击事件
    /// </summary>
    /// <param Name="OrganizationId"></param>
    void Tree_ClickTree(int OrganizationId) {
        try {
            if (OrganizationId != 0) {
                _selectedOrganizationId = OrganizationId;
                //lxrDetailedPage = new LxrDetailedPage();

                p.OrganizationId = OrganizationId;
                FrameChat.Navigate(p);
            }
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }



    public void LoadQl(object o) {
        if (App.GroupsLoadOk == true) {

            List<MucTable> groupContactsDt = MucServices.getInstance().FindAllGroup();

            this.Dispatcher.BeginInvoke((Action)delegate () {
                MyQlItems.Clear();
                for (int i = 0; i < groupContactsDt.Count; i++) {

                    //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                    bool flag = MyQlItems.Where(q => q.mucId.Equals(groupContactsDt[i].mucId)).Count() > 0;
                    if (flag == false) {
                        groupContactsDt[i].logoPath = ImageHelper.loadAvatarPath(groupContactsDt[i].avatarStorageRecordId.ToStr());
                        //groupContactsDt[i].name = groupContactsDt[i].name;
                        MyQlItems.Add(groupContactsDt[i]);
                    }
                }
            });
        }
    }


    /// <summary>
    /// 加载组织
    /// </summary>
    public void LoadOrg(object o) {
        try {
            // 组织和组织成员数据都加载完成了
            if (OrganizationServices.getInstance().DataLoadComplete && OrganizationMemberService.getInstance().DataLoadComplete) {
                this.Dispatcher.BeginInvoke((Action)delegate() {
                    TreeControl tree = new TreeControl();
                    tree.LoadTree();
                });
            }

        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 加载联系人
    /// </summary>
    public void LoadLxr(object o) {
        if (App.ContactsLoadOk == true) {
            List<ContactsTable> contactsDt = ContactsServices.getInstance().FindAllFriend(null, null);
            this.Dispatcher.BeginInvoke((Action)delegate() {
                MyLxrItems.Clear();
                for (int i = 0; i < contactsDt.Count; i++) {

                    //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                    bool flag = MyLxrItems.Where(q => q.clientuserId.Equals(contactsDt[i].clientuserId)).Count() > 0;
                    if (flag == false) {
                        contactsDt[i].logoPath = ImageHelper.loadAvatarPath(contactsDt[i].avatarStorageRecordId.ToStr());
                        contactsDt[i].name = ContactsServices.getInstance().getContractNameByNo(contactsDt[i].no);
                        MyLxrItems.Add(contactsDt[i]);
                    }

                }

            });
        }

    }

    /// <summary>
    /// 加载公众号
    /// </summary>
    public void LoadPublicAccounts(object o) {
        if (App.PublicAccountsLoadOk == true) {
            List<PublicAccountsTable> gzhDt = PublicAccountsService.getInstance().findAllPublicAccounts();
            this.Dispatcher.BeginInvoke((Action)delegate () {
                MyGzhItems.Clear();
                for (int i = 0; i < gzhDt.Count; i++) {
                    //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                    bool flag = MyGzhItems.Where(q => q.appid.Equals(gzhDt[i].appid)).Count() > 0;
                    if (flag == false) {

                        gzhDt[i].logoPath = ImageHelper.loadAvatarPath(gzhDt[i].logoId.ToStr());
                        LoginBeanTenants bean = App.TenantNoDic[gzhDt[i].tenantNo];
                        if(bean!=null) {
                            gzhDt[i].tenantName = bean.name;
                        }

                        MyGzhItems.Add(gzhDt[i]);
                    }

                }
            });
        }

    }

    private AdornerLayer searchAdornerLayer = null;
    private SearchAdorner searchAdorner = null;
    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {



            ClearSelect();

            ChkZzjg.IsChecked = false;

            //加载组织树
            ThreadPool.QueueUserWorkItem(LoadOrg, null);

            //加载组织树
            ThreadPool.QueueUserWorkItem(LoadQl, null);
            //加载公众号

            ThreadPool.QueueUserWorkItem(LoadPublicAccounts, null);

            //加载标签
            {
                //Label btn = new Label();
                //Style btn_style = (Style)this.FindResource("LabelStyle");
                //btn.Content = "OA项目研发群（8）";
                //btn.Style = btn_style;
                //ListViewBq.Items.Add(btn);

                //Label btn1 = new Label();
                //btn1.Content = "产品小黑屋（8）";
                //btn1.Style = btn_style;
                //ListViewBq.Items.Add(btn1);
            }


            //加载常用联系人
            ThreadPool.QueueUserWorkItem(LoadLxr, null);


            // 初始化搜索窗体
            searchAdornerLayer = AdornerLayer.GetAdornerLayer(DockPanelBook);
            if (searchAdornerLayer != null) {
                searchAdornerLayer.Opacity = 1;
                searchAdorner = new SearchAdorner(DockPanelBook);
                searchAdorner.ResultControl.SetRedirectCallBack(() => {
                    SearchText.Text = "";
                });
                searchAdornerLayer.Add(searchAdorner);
                searchAdornerLayer.Visibility = Visibility.Hidden;
            }
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }



    /// <summary>
    /// 清空选择状态
    /// </summary>
    private void ClearSelect() {
        try {

            ListViewLxr.SelectedIndex = -1;
            ListViewBq.SelectedIndex = -1;
            ListViewGzh.SelectedIndex = -1;
            ListViewQl.SelectedIndex = -1;
            ContactsImagePage contactsImagePage = new Page.ContactsImagePage();
            contactsImagePage.Contacts = true;
            FrameChat.Navigate(contactsImagePage);
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 组织机构勾选
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkZzjg_Checked(object sender, RoutedEventArgs e) {
        try {
            if (ChkZzjg.IsChecked == true) {
                TreeOrg.Visibility = Visibility.Visible;
                TreeOrg.MaxHeight = MaxHeight();
                ChkGzh.IsChecked = false;
                ChkGzh_Click(null, null);

                ChkQl.IsChecked = false;
                ChkQl_Click(null, null);

                ChkLxr.IsChecked = false;
                ChkLxr_Click(null, null);

                ChkBq.IsChecked = false;
                ChkBq_Click(null, null);
                TreeOrg.Expand();
            } else {
                TreeOrg.Visibility = Visibility.Collapsed;

            }
            ClearSelect();
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private double MaxHeight() {
        return DockPanelBook.ActualHeight - ChkZzjg.ActualHeight - ChkQl.ActualHeight - ChkGzh.ActualHeight - ChkBq.ActualHeight - ChkLxr.ActualHeight;
    }
    /// <summary>
    /// 群聊点击
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkQl_Click(object sender, RoutedEventArgs e) {
        try {
            if (ChkQl.IsChecked == true) {
                ListViewQl.Visibility = Visibility.Visible;
                ListViewQl.MaxHeight = MaxHeight();
                ChkGzh.IsChecked = false;
                ChkGzh_Click(null, null);

                ChkZzjg.IsChecked = false;
                ChkZzjg_Checked(null, null);

                ChkLxr.IsChecked = false;
                ChkLxr_Click(null, null);

                ChkBq.IsChecked = false;
                ChkBq_Click(null, null);
            } else {
                ListViewQl.Visibility = Visibility.Collapsed;

            }
            ClearSelect();
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// TODO:这个咋描述
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkGzh_Click(object sender, RoutedEventArgs e) {
        try {

            if (ChkGzh.IsChecked == true && ListViewGzh.Items.Count > 0) {
                ListViewGzh.Visibility = Visibility.Visible;
                ListViewGzh.MaxHeight = MaxHeight();

                ChkQl.IsChecked = false;
                ChkQl_Click(null, null);

                ChkZzjg.IsChecked = false;
                ChkZzjg_Checked(null, null);

                ChkLxr.IsChecked = false;
                ChkLxr_Click(null, null);

                ChkBq.IsChecked = false;
                ChkBq_Click(null, null);
            } else {
                ListViewGzh.Visibility = Visibility.Collapsed;

            }
            ClearSelect();
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// TODO：这个咋描述
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkBq_Click(object sender, RoutedEventArgs e) {
        try {
            if (ChkBq.IsChecked == true && ListViewBq.Items.Count > 0) {

                ListViewBq.Visibility = Visibility.Visible;
                ListViewBq.MaxHeight = MaxHeight();
                ChkQl.IsChecked = false;
                ChkQl_Click(null, null);

                ChkZzjg.IsChecked = false;
                ChkZzjg_Checked(null, null);

                ChkLxr.IsChecked = false;
                ChkLxr_Click(null, null);

                ChkGzh.IsChecked = false;
                ChkGzh_Click(null, null);
            } else {
                ListViewBq.Visibility = Visibility.Collapsed;

            }
            ClearSelect();
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// TODO:如何描述
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkLxr_Click(object sender, RoutedEventArgs e) {
        try {
            if (ChkLxr.IsChecked == true && ListViewLxr.Items.Count > 0) {
                ListViewLxr.Visibility = Visibility.Visible;
                ListViewLxr.MaxHeight = MaxHeight();
                //ListViewLxr_SelectionChanged(null, null);
                ChkQl.IsChecked = false;
                ChkQl_Click(null, null);

                ChkZzjg.IsChecked = false;
                ChkZzjg_Checked(null, null);

                ChkBq.IsChecked = false;
                ChkBq_Click(null, null);

                ChkGzh.IsChecked = false;
                ChkGzh_Click(null, null);
            } else {
                ListViewLxr.Visibility = Visibility.Collapsed;

            }
            ClearSelect();
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }


    /// <summary>
    /// 群聊选中事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ListViewQl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            if (ListViewQl.SelectedIndex == -1) return;

            object o = ListViewQl.SelectedItem;
            if (o == null)
                return;

            ListViewLxr.SelectedIndex = -1;
            ListViewBq.SelectedIndex = -1;
            ListViewGzh.SelectedIndex = -1;


            MucTable p = o as MucTable;

            qlDetailedPage.No = p.no;
            FrameChat.Navigate(qlDetailedPage);
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    /// <summary>
    /// 公众号选项选中事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ListViewGzh_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            if (ListViewGzh.SelectedIndex == -1) return;
            object o = ListViewGzh.SelectedItem;
            if (o == null)
                return;

            PublicAccountsTable p = o as PublicAccountsTable;
            if (p == null)
                return;
            ListViewBq.SelectedIndex = -1;
            ListViewLxr.SelectedIndex = -1;
            ListViewQl.SelectedIndex = -1;


            publicAccountsPage.AppId = p.appid;
            FrameChat.Navigate(publicAccountsPage);
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private void ListViewBq_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            if (ListViewBq.SelectedIndex == -1) return;
            ListViewLxr.SelectedIndex = -1;
            ListViewGzh.SelectedIndex = -1;
            ListViewQl.SelectedIndex = -1;
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }

    }

    /// <summary>
    /// 常用联系人选择
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ListViewLxr_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            if (ListViewLxr.SelectedIndex == -1) return;
            object o = ListViewLxr.SelectedItem;
            if (o == null)
                return;

            ContactsTable p = o as ContactsTable;
            if (p == null)
                return;
            ListViewBq.SelectedIndex = -1;
            ListViewGzh.SelectedIndex = -1;
            ListViewQl.SelectedIndex = -1;


            lxrDetailedPage.lxrDetailedType = LxrDetailedType.Lxr;
            lxrDetailedPage.tenantNo = string.Empty;
            lxrDetailedPage.memberId = string.Empty;
            lxrDetailedPage.Id = p.clientuserId.ToInt();
            FrameChat.Navigate(lxrDetailedPage);
        } catch (Exception ex) {
            Log.Error(typeof(AddressBookPage), ex);
        }
    }

    private string searchTextBak = string.Empty;
    private void SearchText_TextChanged(object sender, TextChangedEventArgs e) {
        searchTextBak = SearchText.Text;
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
    private void SearchText_OnKeyUp(object sender, KeyEventArgs e) {
        switch (e.Key) {
        case Key.Up:
            searchAdorner.ResultControl.ItemUpKeyUpCommand();
            break;
        case Key.Down:
            searchAdorner.ResultControl.ItemDownKeyUpCommand();
            break;
        case Key.Enter:
            if (!searchTextBak.Equals(SearchText.Text)) {
                searchAdorner.ResultControl.ItemClickCommandFun();
            }
            break;
        }
    }

    private void SearchText_OnPreviewKeyDown(object sender, KeyEventArgs e) {
        switch (e.Key) {
        case Key.Enter:
            if (!searchTextBak.Equals(SearchText.Text)) {
                searchAdorner.ResultControl.ItemClickCommandFun();
            }
            break;
        }
    }
}
}
