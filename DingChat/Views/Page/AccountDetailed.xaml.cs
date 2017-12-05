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
using cn.lds.chatcore.pcw.Views.Windows;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// GetPerson.xaml 的交互逻辑
/// </summary>
public partial class AccountDetailed : System.Windows.Controls.Page {

    private AccountDetailed() {
        InitializeComponent();
    }
    private static AccountDetailed instance = null;
    public static AccountDetailed getInstance() {
        if (instance == null) {
            instance = new AccountDetailed();
        }
        return instance;
    }



    private int id;
    public int Id { // the Name property
        get {

            return id;
        } set {
            id = value;
            Refesh();
        }
    }
    /// <summary>
    /// 业务事件处理
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (data.eventDataType) {
            // 修改个人信息后
            case BusinessEventDataType.MyDetailsChangeEvent:
                this.Dispatcher.BeginInvoke((Action) delegate {
                    Refesh();
                });


                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(AccountDetailed), ex);
        }
    }
    /// <summary>
    /// 刷新方法
    /// </summary>
    private void Refesh() {
        try {

            //重置数据
            LbName.Content = string.Empty;
            LbTelphone.Content = string.Empty;
            BtnMail.Tag = string.Empty;
            LbZw.Content = string.Empty;
            LbDw.Content = string.Empty;
            LbBm.Content = string.Empty;
            BtnSex.Tag = string.Empty;
            //获取联系人信息
            AccountsTable model = AccountsServices.getInstance().findByNo(App.AccountsModel.no);
            if (model != null) {

                string imagePath = model.avatarStorageRecordId.ToStr();
                ImageHelper.loadAvatarImageBrush(imagePath, Ico);
                LbName.Content = model.nickname.ToStr();
                LbTelphone.Content = model.mobile.ToStr();
                BtnMail.Tag = model.email.ToStr();
                BtnSex.Tag = GetDescription.description((Sex)Enum.Parse(typeof(Sex), model.gender.ToStr()));
                BtnQm.Tag = model.desc;
            }
            // 获取组织信息
            List<OrganizationMemberTable> dtOrg = OrganizationMemberService.getInstance().FindOrganizationMemberByUserId(id);
            if (dtOrg.Count > 0) {
                //todo 需要修改 双租户的时候
                foreach (OrganizationMemberTable dt in dtOrg) {
                    string zw= MasterServices.getInstance().getTextByTypeAndKey(MasterType.post, dt.post.ToStr(), App.CurrentTenantNo);
                    if(zw!=string.Empty) {
                        LbZw.Content += zw + ";";
                    }

                    string dw= dt.office.ToStr();
                    if (dw != string.Empty) {
                        LbDw.Content += dw + ";";
                    }
                    int orgId = dt.organizationId.ToStr().ToInt();
                    OrganizationTable orgDt = OrganizationServices.getInstance().FindOrganizationByOrgId(orgId);
                    if (orgDt != null) {
                        string bm= orgDt.name.ToStr() ;

                        if (bm != string.Empty) {
                            LbBm.Content += bm + ";";
                        }
                    }

                }
                //LbZw.Content = MasterServices.getInstance().getTextByTypeAndKey(MasterType.post, dtOrg[0].post.ToStr(),App.CurrentTenantNo);
                //LbDw.Content = dtOrg[0].office.ToStr();


            }
        } catch (Exception ex) {
            Log.Error(typeof(AccountDetailed), ex);
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
            Log.Error(typeof(AccountDetailed), ex);
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
            Log.Error(typeof(AccountDetailed), ex);
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
            Log.Error(typeof(AccountDetailed), ex);
        }

    }
}
}
