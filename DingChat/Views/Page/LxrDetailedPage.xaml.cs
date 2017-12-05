using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Windows;
using EventBus;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event.Publisher;
using Newtonsoft.Json;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// GroupChatDetailed.xaml 的交互逻辑
/// </summary>
public partial class LxrDetailedPage : System.Windows.Controls.Page {

    public LxrDetailedPage() {
        InitializeComponent();
    }

    private static LxrDetailedPage instance = null;
    public static LxrDetailedPage getInstance() {
        if (instance == null) {
            instance = new LxrDetailedPage();
        }
        return instance;
    }

    // 变量定义
    public LxrDetailedType lxrDetailedType;

    public event Action<object> BnBack;
    private int id;
    private String no;
    public int Id { // the Name property
        get {

            return id;
        } set {
            id = value;
            if (App.TenantNoDic.Count > 1) {
                tenantPanel.Children.Clear();
                List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserId(id);
                Dictionary<string, LoginBeanTenants> tenantNoDic = new Dictionary<string, LoginBeanTenants>();

                foreach (OrganizationMemberTable dt in dtOrg) {
                    if(App.TenantNoDic.ContainsKey(dt.tenantNo) && !tenantNoDic.ContainsKey(dt.tenantNo)) {
                        tenantNoDic.Add(dt.tenantNo,App.TenantNoDic[dt.tenantNo]);
                    }
                }

                int count = 0;
                foreach (var item in tenantNoDic) {
                    LoginBeanTenants bean = item.Value;
                    RadioButton but = new RadioButton();
                    Style btn_style = (Style)this.FindResource("RadioButton");
                    but.Style = btn_style;
                    but.GroupName = "tenant";
                    but.Name = item.Key;
                    but.Margin= new Thickness(10, 0, 0, 0);
                    but.Click += But_Click; ;
                    but.Content = bean.name;
                    tenantPanel.Children.Add(but);
                    if(count==0) {
                        but.IsChecked = true;
                        currentTenantNo = item.Key;
                    }
                    count++;
                }
                //判断是否显示 租户选择
                if (tenantNoDic.Count <= 1) {
                    tenantPanel.Visibility = Visibility.Collapsed;
                } else {
                    tenantPanel.Visibility = Visibility.Visible;
                }
            }
            Refesh();
        }
    }

    public String memberId;

    public String tenantNo;

    //当前选择的
    private string currentTenantNo = string.Empty;
    /// <summary>
    /// 刷新方法
    /// </summary>
    private void Refesh() {
        try {

            //重置数据
            LbName.Content = string.Empty;
            LbTelphone.Content = string.Empty;
            LbMail.Content = string.Empty;
            LbZw.Content = string.Empty;
            LbDw.Content = string.Empty;
            LbBm.Content = string.Empty;
            LbSex.Content = string.Empty;
            LbEmpno.Content = string.Empty;
            LbLocation.Content = string.Empty;



            // 判断是否查看当前登录人
            if (App.AccountsModel.clientuserId.Equals(id.ToStr())) {
                BtnSend.Visibility = Visibility.Collapsed;
                BtnAdd.Visibility = Visibility.Collapsed;
                AccountsTable acTable = AccountsServices.getInstance().findByNo(App.AccountsModel.no);

                string imagePath = acTable.avatarStorageRecordId;

                ImageHelper.loadAvatar(imagePath, Ico);
                LbName.Content = acTable.nickname;
                LbTelphone.Content = acTable.mobile;
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), acTable.gender.ToStr()));
                LbQm.Content = acTable.desc.ToStr();   //签名

            } else {
                // 判断是否是好友
                if (ContactsServices.getInstance().isFriend(id)) {
                    BtnSend.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Collapsed;

                    //获取联系人数据
                    VcardsTable vsModel = VcardService.getInstance().findByClientuserId(id);
                    if (vsModel != null) {
                        no = vsModel.no;
                        LbTelphone.Content = vsModel.mobileNumber.ToStr();
                        if(vsModel.gender.ToStr()!=string.Empty) {
                            LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), vsModel.gender.ToStr()));
                        }
                        LbQm.Content = vsModel.desc.ToStr();   //签名
                    }
                    //获取联系人数据
                    ContactsTable contactsModel = ContactsServices.getInstance().FindContactsById(id);
                    if (contactsModel != null) {
                        Btnbz.Tag = contactsModel.alias;
                        LbName.Content = ContactsServices.getInstance().getContractOriginalNameByNo(contactsModel.no);
                        string imagePath = contactsModel.avatarStorageRecordId.ToStr();
                        ImageHelper.loadAvatar(imagePath, Ico);
                    }
                    // 拉取好友的详情
                    // ContactsServices.getInstance().GetContactsDetaile(id);
                } else {
                    BtnSend.Visibility = Visibility.Collapsed;
                    BtnAdd.Visibility = Visibility.Visible;

                    // 拉取陌生人的详情
                    ContactsServices.getInstance().GetStranger(id.ToString());
                }
            }


            if (currentTenantNo == string.Empty) {
                currentTenantNo = App.CurrentTenantNo;
            }
            //查看好友的时候多租户的人要显示多个职务

            // 获取组织信息
            List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserIdAndTent(id, currentTenantNo);

            if (dtOrg.Count > 0) {

                foreach (OrganizationMemberTable dt in dtOrg) {
                    string zw = MasterServices.getInstance()
                                .getTextByTypeAndKey(MasterType.post, dt.post.ToStr(), dt.tenantNo);
                    if (zw != string.Empty) {
                        LbZw.Content += zw + ";";
                    }

                    string dw = dt.office.ToStr();
                    if (dw != string.Empty) {
                        LbDw.Content += dw + ";";
                    }
                    int orgId = dt.organizationId.ToStr().ToInt();
                    OrganizationTable orgDt = OrganizationServices.getInstance().FindOrganizationByOrgId(orgId);
                    if (orgDt != null) {
                        string bm = orgDt.name.ToStr();

                        if (bm != string.Empty) {
                            LbBm.Content += bm + ";";
                        }
                    }
                }

                LbMail.Content = dtOrg[0].email;
                LbEmpno.Content = dtOrg[0].empno;
                LbLocation.Content = MasterServices.getInstance()
                                     .getTextByTypeAndKey(MasterType.location, dtOrg[0].location, dtOrg[0].tenantNo);

                if(LbZw.Content.ToStr()!=string.Empty) {
                    LbZw.Content = LbZw.Content.ToStr().Substring(0, LbZw.Content.ToStr().Length - 1);
                }
                if (LbDw.Content.ToStr() != string.Empty) {
                    LbDw.Content = LbDw.Content.ToStr().Substring(0, LbDw.Content.ToStr().Length - 1);
                }
                if (LbBm.Content.ToStr() != string.Empty) {
                    LbBm.Content = LbBm.Content.ToStr().Substring(0, LbBm.Content.ToStr().Length - 1);
                }

            }

        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }

    /// <summary>
    /// C003: 设置好友备注名
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (eventData.eventDataType) {
            //  添加新朋友 addFriend
            case EventDataType.addFriend:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    this.Dispatcher.Invoke(new Action(() => {
                        BtnSend.Visibility = Visibility.Visible;
                        BtnAdd.Visibility = Visibility.Collapsed;
                        NotificationHelper.ShowSuccessMessage("添加成功！");
                    }));
                }
                // API请求失败
                else {
                    this.Dispatcher.Invoke(new Action(() => {
                        NotificationHelper.ShowErrorMessage("添加失败！");
                    }));

                }
                break;
            //  C007: 删除朋友 deleteFriend
            case EventDataType.deleteFriend:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    this.Dispatcher.Invoke(new Action(() => {
                        NotificationHelper.ShowSuccessMessage("删除成功！");
                    }));
                }
                // API请求失败
                else {
                    this.Dispatcher.Invoke(new Action(() => {
                        NotificationHelper.ShowErrorMessage("删除失败！");
                    }));
                }
                break;
            // C002: 获取好友详细 getFriend
            case EventDataType.getFriend:
                if (eventData.eventType == EventType.HttpRequest) {
                    C002(eventData);
                } else {
                }
                break;
            // C002.2: 获陌生人详细
            case EventDataType.getStranger:
                if (eventData.eventType == EventType.HttpRequest) {
                    C002_2(eventData);
                } else {
                }
                break;
            //  C003: 设置好友备注名
            case EventDataType.changeAlias:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    this.Dispatcher.Invoke(new Action(() => {
                        NotificationHelper.ShowSuccessMessage("设置成功！");
                    }));
                }
                // API请求失败
                else {
                    this.Dispatcher.Invoke(new Action(() => {
                        NotificationHelper.ShowErrorMessage("设置失败！");
                    }));

                }
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(ContactsServices), e);
        }
    }

    /// <summary>
    /// C002: 获取好友详细 getFriend
    /// </summary>
    /// <param Name="data"></param>
    public void C002(EventData<Object> data) {
        try {
            Object contacts = data.data;

            VcardsTableBean vsCard = JsonConvert.DeserializeObject<VcardsTableBean>(contacts.ToStr());

            this.Dispatcher.Invoke(new Action(() => {
                string imagePath = vsCard.avatarStorageRecordId;

                ImageHelper.loadAvatar(imagePath, Ico);
                LbName.Content = vsCard.nickname;
                LbTelphone.Content = vsCard.mobileNumber;
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), vsCard.gender.ToStr()));
            }));
            //DoEvents();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }

    /// <summary>
    /// C002.2: 获陌生人详细
    /// </summary>
    /// <param Name="data"></param>
    public void C002_2(EventData<Object> data) {
        try {
            Object contacts = data.data;

            VcardsTableBean vsCard = JsonConvert.DeserializeObject<VcardsTableBean>(contacts.ToStr());

            this.Dispatcher.Invoke(new Action(() => {
                string imagePath = vsCard.avatarStorageRecordId;

                ImageHelper.loadAvatar(imagePath, Ico);
                LbName.Content = vsCard.nickname;
                LbTelphone.Content = vsCard.mobile;
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), vsCard.gender.ToStr()));
            }));
            //DoEvents();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }
    /// <summary>
    /// 业务事件处理
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            // 修改个人信息后
            case BusinessEventDataType.ContactsDetailsChangeEvent:
                DoContactsDetailsChangeEvent(data);
                break;
            // 修改个人信息后
            case BusinessEventDataType.MyDetailsChangeEvent:
                DoContactsDetailsChangeEvent(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }

    /// <summary>
    /// 修改个人信息后
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsDetailsChangeEvent(BusinessEvent<Object> data) {

        try {
            this.Dispatcher.Invoke(new Action(() => {
                Refesh();
            }));
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }


    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Page_Loaded(object sender, RoutedEventArgs e) {
        try {
            this.ButtonCtrl();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }

    /// <summary>
    /// 按钮控制
    /// </summary>
    public void ButtonCtrl() {

        try {
            if (lxrDetailedType == LxrDetailedType.Org) {
                //GridM.RowDefinitions[0].Height = new GridLength(44);
                GridM.RowDefinitions[5].Height = new GridLength(0);
                BtnBack.Visibility = Visibility.Visible;
                BtnDel.Visibility = Visibility.Collapsed;
                //BtnMail.Visibility = Visibility.Collapsed;
                BtnSex.Visibility = Visibility.Collapsed;
                BtnQm.Visibility = Visibility.Collapsed;
            } else if (lxrDetailedType == LxrDetailedType.Lxr) {
                //GridM.RowDefinitions[0].Height = new GridLength(44);
                GridM.RowDefinitions[5].Height = new GridLength(64);
                BtnBack.Visibility = Visibility.Collapsed;
                BtnDel.Visibility = Visibility.Visible;
                //BtnMail.Visibility = Visibility.Collapsed;
                BtnSex.Visibility = Visibility.Collapsed;
                BtnQm.Visibility = Visibility.Collapsed;
            } else if (lxrDetailedType == LxrDetailedType.Account) {
                //GridM.RowDefinitions[0].Height = new GridLength(0);
                GridM.RowDefinitions[5].Height = new GridLength(0);
                BtnBack.Visibility = Visibility.Collapsed;
                BtnDel.Visibility = Visibility.Collapsed;
                //BtnMail.Visibility = Visibility.Visible;
                BtnSex.Visibility = Visibility.Visible;
                BtnQm.Visibility = Visibility.Visible;
            }
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }

    private void But_Click(object sender, RoutedEventArgs e) {
        RadioButton btn = sender as RadioButton;
        if (btn == null)
            return;
        currentTenantNo = btn.Name.ToStr();
        Refesh();
    }



    /// <summary>
    /// 备注点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Btnbz_Click(object sender, RoutedEventArgs e) {

        try {
            EditBZ editGroupNotice = new EditBZ();
            editGroupNotice.ClientuserId = id;
            editGroupNotice.Show();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }

    /// <summary>
    /// 删除联系人按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnDel_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认删除常用联系人  " + LbName.Content + "   ?", CommonMessageBox.MsgBtn.OKCancel) ==
                    CommonMessageBox.MsgResult.OK) {
                ContactsServices.getInstance().deleteFriend(id,no);
            }
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }
    /// <summary>
    /// 点击添加按钮
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnAdd_Click(object sender, RoutedEventArgs e) {
        try {
            ContactsServices.getInstance().addFriend(id, null);
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }
    /// <summary>
    /// 点击发送
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnSend_Click(object sender, RoutedEventArgs e) {
        try {
            BusinessEvent<object> businessdata = new BusinessEvent<object>();
            businessdata.data = Id;
            businessdata.eventDataType = BusinessEventDataType.RedirectChatSessionEvent;
            EventBusHelper.getInstance().fireEvent(businessdata);


            //    if (BnSendClick != null && e != null) {
            //    BnSendClick.Invoke(Id, ContactsTpye.LXR);
            //}
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }

    /// <summary>
    /// 返回按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        try {
            if (BnBack != null && e != null) {
                BnBack.Invoke(Id);
            }
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }

    /// <summary>
    /// 编辑邮箱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnMail_Click(object sender, RoutedEventArgs e) {


        try {
            EditEmail edit = new EditEmail();
            edit.ClientuserId = App.AccountsModel.clientuserId.ToInt();
            edit.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }

    /// <summary>
    /// 编辑性别
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSex_Click(object sender, RoutedEventArgs e) {
        try {
            EditGender edit = new EditGender();
            edit.ClientuserId = App.AccountsModel.clientuserId.ToInt();
            edit.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }
    }

    /// <summary>
    /// 签名编辑
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnQm_Click(object sender, RoutedEventArgs e) {

        try {
            EditDesc edit = new EditDesc();
            edit.ClientuserId = App.AccountsModel.clientuserId.ToInt();
            edit.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(LxrDetailedPage), ex);
        }

    }
}
}
