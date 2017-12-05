using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using EventBus;
using Newtonsoft.Json;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// GetPerson.xaml 的交互逻辑
/// </summary>
public partial class OrgMemberDetailed : System.Windows.Controls.Page {

    private OrgMemberDetailed() {
        InitializeComponent();
    }
    private static OrgMemberDetailed instance = null;
    public static OrgMemberDetailed getInstance() {
        if (instance == null) {
            instance = new OrgMemberDetailed();
        }
        return instance;
    }

    // 变量定义
    //public event Action<object, ContactsTpye> BnSendClick;
    public event Action<object> BnBack;

    private int id;
    public int Id { // the Name property
        get {

            return id;
        } set {
            id = value;
            Refesh();
        }
    }
    public String memberId;

    public String tenantNo;
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

            // 判断是否查看当前登录人
            if (App.AccountsModel.clientuserId.Equals(id.ToStr())) {
                BtnSend.Visibility = Visibility.Collapsed;
                BtnAdd.Visibility = Visibility.Collapsed;
                AccountsTable acTable= AccountsServices.getInstance().findByNo(App.AccountsModel.no);

                string imagePath = acTable.avatarStorageRecordId;

                ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                LbName.Content = acTable.nickname;
                LbTelphone.Content = acTable.mobile;
                LbMail.Content = acTable.email.ToStr();
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), acTable.gender.ToStr()));
            } else {
                // 判断是否是好友
                if (ContactsServices.getInstance().isFriend(id)) {
                    BtnSend.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Collapsed;
                    // 拉取好友的详情
                    ContactsServices.getInstance().GetContactsDetaile(id);
                } else {
                    BtnSend.Visibility = Visibility.Collapsed;
                    BtnAdd.Visibility = Visibility.Visible;
                    // 拉取陌生人的详情
                    ContactsServices.getInstance().GetStranger(id.ToString());
                }
            }

            // 获取联系人详情
            VcardsTable drVs = VcardService.getInstance().findByClientuserId(id);
            if (drVs != null) {
                LbTelphone.Content = drVs.mobileNumber.ToStr();
                LbMail.Content = drVs.email.ToStr();
                LbSex.Content = GetDescription.description((Sex)Enum.Parse(typeof(Sex), drVs.gender.ToStr()));
            }

            // 获取组织信息
            OrganizationMemberTable organizationMemberTable = null;
            if (string.IsNullOrEmpty(memberId)) {
                List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserId(id);
                organizationMemberTable = dtOrg[0];
            } else {
                organizationMemberTable = OrganizationMemberService.getInstance().FindOrganizationMemberByMemberId(memberId);
            }

            if (organizationMemberTable!=null) {

                LbZw.Content = MasterServices.getInstance().getTextByTypeAndKey(MasterType.post, organizationMemberTable.post.ToStr(), tenantNo);
                LbDw.Content = organizationMemberTable.office.ToStr();
                int orgId = organizationMemberTable.organizationId.ToStr().ToInt();
                OrganizationTable dt = OrganizationServices.getInstance().FindOrganizationByOrgId(orgId);
                if (dt != null) {
                    LbBm.Content = dt.name.ToStr();
                } else {
                    LbBm.Content = string.Empty;
                }
                // LbDw.Content = organizationMemberTable.nickname.ToStr();
                ImageHelper.loadAvatarImageBrush(organizationMemberTable.avatarId, Ico);
            }


            /*
            // 获取组织信息
            List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserId(id);
            if (dtOrg.Count > 0) {
                OrganizationMemberTable organizationMemberTable = dtOrg[0];
                LbZw.Content = MasterServices.getInstance().getTextByTypeAndKey(MasterType.post, organizationMemberTable.post.ToStr());
                LbDw.Content = organizationMemberTable.office.ToStr();
                int orgId = organizationMemberTable.organizationId.ToStr().ToInt();
                OrganizationTable dt = OrganizationServices.getInstance().FindOrganizationByOrgId(orgId);
                if (dt != null) {
                    LbBm.Content = dt.Name.ToStr();
                } else {
                    LbBm.Content = string.Empty;
                }
                ImageHelper.loadAvatarImageBrush(organizationMemberTable.avatarId, Ico);
            }
            */
            /*
            //获取联系人信息
            ContactsTable model = ContactsServices.getInstance().FindContactsById(id);
            if (model != null) {
                // 如果查看的联系人是自己，则什么都不显示
                if (!App.AccountsModel.clientuserId.Equals(id.ToStr())) {
                    BtnSend.Visibility = Visibility.Visible;
                    BtnAdd.Visibility = Visibility.Collapsed;
                    // 拉取好友的详情
                    ContactsServices.getInstance().GetContactsDetaile(id);
                } else {
                    BtnSend.Visibility = Visibility.Collapsed;
                    BtnAdd.Visibility = Visibility.Collapsed;
                }

                string imagePath = model.AvatarStorageRecordId.ToStr();

                ImageHelper.loadAvatarImageBrush(imagePath, Ico);

                VcardsTable drVs = VcardService.getInstance().findByClientuserId(id);
                if (drVs != null) {
                    LbName.Content = drVs.nickname.ToStr();
                    LbTelphone.Content = drVs.mobileNumber.ToStr();
                    LbMail.Content = drVs.email.ToStr();
                }

            } else {
                // 如果查看的联系人是自己，则什么都不显示
                if (!App.AccountsModel.clientuserId.Equals(id.ToStr())) {
                    BtnSend.Visibility = Visibility.Collapsed;
                    BtnAdd.Visibility = Visibility.Visible;
                    // 拉取陌生人的详情
                    ContactsServices.getInstance().GetStranger(id.ToString());
                } else {
                    BtnSend.Visibility = Visibility.Collapsed;
                    BtnAdd.Visibility = Visibility.Collapsed;
                }

                List<OrganizationMemberTable> dtList = OrganizationMemberService.getInstance().FindOrganizationMemberByUserId(Id);
                if (dtList.Count > 0) {
                    OrganizationMemberTable dt = dtList[0];
                    string imagePath = dt.avatarId.ToStr();

                    ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                    LbName.Content = dt.nickname.ToStr();
                    LbTelphone.Content = dt.officeTel.ToStr();
                    //LbMail.Content = dt.ma.ToStr();
                }
            }
            */


        } catch (Exception ex) {
            Log.Error(typeof(OrgMemberDetailed), ex);
        }
    }



    /// <summary>
    /// 界面
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {

        try {
            switch (data.eventDataType) {
            // 添加好友之后
            case BusinessEventDataType.ContactsChangedEvent:
                DoContactsChangedEvent(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof (OrgMemberDetailed), ex);
        }

    }

    /// <summary>
    /// 添加好友之后
    /// </summary>
    /// <param Name="data"></param>
    public void DoContactsChangedEvent(BusinessEvent<Object> data) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                BtnSend.Visibility = Visibility.Visible;
                BtnAdd.Visibility = Visibility.Collapsed;

            }));

        } catch(Exception ex) {
            Log.Error(typeof(OrgMemberDetailed), ex);
        }
    }

    /// <summary>
    /// API请求处理
    /// C002: 获取好友详细 getFriend
    /// C002.2: 获陌生人详细
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnHttpRequestEvent(EventData<Object> data) {
        try {
            switch (data.eventDataType) {
            //  添加新朋友 addFriend
            case EventDataType.addFriend:
                // API请求成功
                if (data.eventType == EventType.HttpRequest) {
                    this.Dispatcher.Invoke(new Action(() => {
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
                if (data.eventType == EventType.HttpRequest) {
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
                if (data.eventType == EventType.HttpRequest) {
                    C002(data);
                } else {
                }
                break;
            // C002.2: 获陌生人详细
            case EventDataType.getStranger:
                if (data.eventType == EventType.HttpRequest) {
                    C002_2(data);
                } else {
                }
                break;

            }
        } catch (Exception ex) {
            Log.Error(typeof(OrgMemberDetailed), ex);
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
            Log.Error(typeof(OrgMemberDetailed), ex);
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
            Log.Error(typeof(OrgMemberDetailed), ex);
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
            Log.Error(typeof(OrgMemberDetailed), ex);
        }
    }

    /// <summary>
    /// 点击发送按钮
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
            //    //this.Hide();
            //}
        } catch (Exception ex) {
            Log.Error(typeof(OrgMemberDetailed), ex);
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
            Log.Error(typeof(OrgMemberDetailed), ex);
        }

    }
}
}
