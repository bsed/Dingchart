using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// SendCard.xaml 的交互逻辑
/// </summary>
public partial class SendCard : Window {
    public SendCard() {
        InitializeComponent();

    }

    public string ToUserNo = string.Empty;
    private List<string> selectNo = new List<string>();

    private List<string> selectAppId = new List<string>();
    public void DoEvents() {
        try {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(delegate (object f) {
                ((DispatcherFrame)f).Continue = false;

                return null;
            }
                                                   ), frame);
            Dispatcher.PushFrame(frame);
        } catch (Exception ex) {
            Log.Error(typeof(SendCard), ex);
        }
    }
    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {


            List<ContactsTable> contactsDt = ContactsServices.getInstance().FindAllFriend(null, null);
            this.Dispatcher.BeginInvoke((Action)delegate () {
                ListViewLxr.Items.Clear();
                for (int i = 0; i < contactsDt.Count; i++) {
                    ContactsTable model = contactsDt[i];
                    if (model == null) continue;
                    CheckPersonControl p = new CheckPersonControl();
                    p.HeadPortraitId = ImageHelper.loadAvatarPath(model.avatarStorageRecordId);
                    p.Name = model.nickname;
                    p.No = model.no;
                    p.AX -= ListSelect;
                    p.AX += ListSelect;
                    p.Width = ListViewLxr.ActualWidth;
                    ListViewLxr.Items.Add(p);
                    DoEvents();
                }
                ListViewLxr.UpdateLayout();


                LoadQl();
            });



        } catch (Exception ex) {
            Log.Error(typeof(SendCard), ex);
        }
    }

    private void LoadQl() {
        List<PublicAccountsTable> gzhDt = PublicAccountsService.getInstance().findAllPublicAccounts();

        //this.Dispatcher.BeginInvoke((Action)delegate () {
        ListViewQl.Items.Clear();
        for (int i = 0; i < gzhDt.Count; i++) {
            PublicAccountsTable model = gzhDt[i];
            if (model == null) continue;
            CheckPersonControl p = new CheckPersonControl();
            p.HeadPortraitId = ImageHelper.loadAvatarPath(model.logoId);
            p.Name = model.name;
            p.No = model.appid;
            p.AX -= ListSelect;
            p.AX += ListSelect;
            //p.Width = ListViewQl.ActualWidth;
            ListViewQl.Items.Add(p);
            //DoEvents();
        }
        ListViewQl.UpdateLayout();
        //});
    }


    /// <summary>
    /// 人员列表选项click事件
    /// </summary>
    /// <param Name="control"></param>
    /// <param Name="isChecked"></param>
    public   void ListSelect(CheckPersonControl control, bool isChecked) {
        try {


            if(Tab.SelectedIndex==0) {
                if (isChecked) {
                    if (!selectNo.Contains(control.No)) {
                        selectNo.Add(control.No);
                    }
                } else {
                    selectNo.Remove(control.No);
                }

            } else {
                if (isChecked) {
                    if (!selectAppId.Contains(control.No)) {
                        selectAppId.Add(control.No);
                    }
                } else {
                    selectAppId.Remove(control.No);
                }
            }

            //list 加人





        } catch (Exception ex) {
            Log.Error(typeof(SendCard), ex);
        }
    }

    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnCancel_Click(object sender, RoutedEventArgs e) {
        try {
            this.Close();
        } catch (Exception ex) {
            Log.Error(typeof(SendCard), ex);
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e) {


        if (selectNo.Count >0) {
            for(int i=0; i< selectNo.Count; i++) {

                MessageItem item = MessageService.getInstance()
                                   .sendVCardMessage(ToUserNo, selectNo[i]);
                if (item != null) {
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = item;
                    businessEvent.eventDataType = BusinessEventDataType.SendVcCard;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }
            }
            this.Close();
        }

        if (selectAppId.Count > 0) {
            for (int i = 0; i < selectAppId.Count; i++) {
                MessageItem item = MessageService.getInstance()
                                   .sendPublicCardMessage(selectAppId[i], ToUserNo);
                if (item != null) {
                    BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                    businessEvent.data = item;
                    businessEvent.eventDataType = BusinessEventDataType.SendPublicCard;
                    EventBusHelper.getInstance().fireEvent(businessEvent);
                }
            }
            this.Close();
        }


    }


}

}

