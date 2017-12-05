using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using EventBus;
using Newtonsoft.Json;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Views.Windows {

/// <summary>
/// 用户明细
/// </summary>
public partial class PersonDetailed : Window {
    //public event Action<object, ContactsTpye> BnSendClick;
    private static PersonDetailed _instance = null;
    public static PersonDetailed getInstance() {
        if (_instance == null) {
            _instance = new PersonDetailed();
        }
        return _instance;
    }

    /// <summary>
    /// 画面构造方法
    /// </summary>
    private PersonDetailed() {
        InitializeComponent();
    }


    private int _id;
    public int Id {
        get {
            return _id;
        } set {
            _id = value;
            if (App.TenantNoDic.Count > 1) {
                tenantPanel.Children.Clear();
                List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserId(_id);
                Dictionary<string, LoginBeanTenants> tenantNoDic = new Dictionary<string, LoginBeanTenants>();

                foreach (OrganizationMemberTable dt in dtOrg) {
                    if (App.TenantNoDic.ContainsKey(dt.tenantNo) && !tenantNoDic.ContainsKey(dt.tenantNo)) {
                        tenantNoDic.Add(dt.tenantNo, App.TenantNoDic[dt.tenantNo]);
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
                    but.Margin = new Thickness(10, 0, 0, 0);
                    but.Click += But_Click; ;
                    but.Content = bean.name;
                    tenantPanel.Children.Add(but);
                    if (count == 0) {
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
    public String memberId=string.Empty;
    public String tenantNo = string.Empty;
    //当前选择的
    private string currentTenantNo = string.Empty;
    /// <summary>
    /// 刷新画面
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
            if (App.AccountsModel.clientuserId.Equals(_id.ToStr())) {
                BtnSend.Visibility = Visibility.Collapsed;
                BtnAdd.Visibility = Visibility.Collapsed;
                AccountsTable acTable = AccountsServices.getInstance().findByNo(App.AccountsModel.no);

                string imagePath = acTable.avatarStorageRecordId;

                ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                LbName.Content = acTable.nickname;
                LbTelphone.Content = acTable.mobile;
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), acTable.gender.ToStr()));
                LbQm.Content = acTable.desc.ToStr();   //签名

            } else {
                // 判断是否是好友
                if (ContactsServices.getInstance().isFriend(_id)) {
                    BtnSend.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Collapsed;

                    //获取联系人数据
                    VcardsTable vsModel = VcardService.getInstance().findByClientuserId(_id);
                    if (vsModel != null) {

                        LbTelphone.Content = vsModel.mobileNumber.ToStr();
                        if (vsModel.gender.ToStr() != string.Empty) {
                            LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), vsModel.gender.ToStr()));
                        }
                        LbQm.Content = vsModel.desc.ToStr();   //签名
                    }
                    //获取联系人数据
                    ContactsTable contactsModel = ContactsServices.getInstance().FindContactsById(_id);
                    if (contactsModel != null) {

                        LbName.Content = ContactsServices.getInstance().getContractOriginalNameByNo(contactsModel.no);
                        string imagePath = contactsModel.avatarStorageRecordId.ToStr();
                        ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                    }
                    // 拉取好友的详情
                    // ContactsServices.getInstance().GetContactsDetaile(id);
                } else {
                    BtnSend.Visibility = Visibility.Collapsed;
                    BtnAdd.Visibility = Visibility.Visible;

                    // 拉取陌生人的详情
                    ContactsServices.getInstance().GetStranger(_id.ToString());
                }
            }


            if (currentTenantNo == string.Empty) {
                currentTenantNo = App.CurrentTenantNo;
            }
            //查看好友的时候多租户的人要显示多个职务

            // 获取组织信息
            List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserIdAndTent(_id, currentTenantNo);

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
                                     .getTextByTypeAndKey(MasterType.location, dtOrg[0].location, dtOrg[0].tenantNo); ;
            }

        } catch (Exception ex) {
            Log.Error(typeof(PersonDetailed), ex);
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
    ///点击关闭按钮
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnClose_Click(object sender, RoutedEventArgs e) {
        this.Close();
    }

    /// <summary>
    /// 业务事件处理
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEventEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            // 添加好友之后
            case BusinessEventDataType.ContactsChangedEvent:
                this.Dispatcher.Invoke(new Action(() => {
                    BtnSend.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Collapsed;
                }));
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(PersonDetailed), e);
        }
    }

    /// <summary>
    /// API请求处理
    /// C002: 获取好友详细 getFriend
    /// C002.2: 获陌生人详细
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (eventData.eventDataType) {
            // C002: 获取好友详细 getFriend
            case EventDataType.getFriend:
                if (eventData.eventType == EventType.HttpRequest) {
                    C002(eventData);
                } else {
                }
                break;
            //  C002.2: 获陌生人详细
            case EventDataType.getStranger:
                // API请求成功
                if (eventData.eventType == EventType.HttpRequest) {
                    C002_2(eventData);
                }
                // API请求失败
                else {

                }
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(PersonDetailed), e);
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

                ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                LbName.Content = vsCard.nickname;
                LbTelphone.Content = vsCard.mobileNumber;
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), vsCard.gender.ToStr()));
            }));
            //DoEvents();
        } catch (Exception ex) {
            Log.Error(typeof(PersonDetailed), ex);
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

                ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                LbName.Content = vsCard.nickname;
                LbTelphone.Content = vsCard.mobile;
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), vsCard.gender.ToStr()));
            }));
            //DoEvents();
        } catch (Exception ex) {
            Log.Error(typeof(PersonDetailed), ex);
        }

    }

    /// <summary>
    /// 重写Close,窗口关闭时设置为隐藏。
    /// </summary>
    /// <param Name="e"></param>
    protected override void OnClosing(CancelEventArgs e) {
        try {
            //Ico.Source = null;

            LbName.Content = string.Empty;
            LbTelphone.Content = string.Empty;
            LbMail.Content = string.Empty;
            Hide();
            e.Cancel = true;
        } catch (Exception ex) {
            Log.Error(typeof(PersonDetailed), ex);
        }
    }


    /// <summary>
    /// 点击添加通讯录按钮
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnAdd_Click(object sender, RoutedEventArgs e) {
        try {
            ContactsServices.getInstance().addFriend(_id, null);
        } catch (Exception ex) {
            Log.Error(typeof(PersonDetailed), ex);
        }
    }

    /// <summary>
    /// 点击发消息按钮
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnSend_Click(object sender, RoutedEventArgs e) {

        try {
            BusinessEvent<object> businessdata = new BusinessEvent<object>();
            businessdata.data = Id;
            businessdata.eventDataType = BusinessEventDataType.RedirectChatSessionEvent;
            EventBusHelper.getInstance().fireEvent(businessdata);

            this.Hide();


        } catch (Exception ex) {
            Log.Error(typeof(PersonDetailed), ex);
        }
    }
}
}
