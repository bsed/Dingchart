using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// SendCardWindow.xaml 的交互逻辑
/// </summary>
public partial class SendCardWindow : Window {
    public SendCardWindow() {
        InitializeComponent();

    }
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
            Log.Error(typeof(SendCardWindow), ex);
        }
    }

    public string ToUserNo = string.Empty;



    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {
            App.listCreatGroupMember = new List<string>();
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
            });

            LoadGzh();
            OrganizationList.Init(this,App.CurrentTenantNo);
        } catch (Exception ex) {
            Log.Error(typeof(SendCardWindow), ex);
        }
    }
    private void LoadGzh() {
        List<PublicAccountsTable> gzhDt = PublicAccountsService.getInstance().findAllPublicAccounts();

        //this.Dispatcher.BeginInvoke((Action)delegate () {
        ListViewGzh.Items.Clear();
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
            ListViewGzh.Items.Add(p);
            //DoEvents();
        }
        ListViewGzh.UpdateLayout();
        //});
    }
    /// <summary>
    /// 切换tab页 需要把已选择的人选择状态
    /// </summary>
    private void ReSelect() {
        for (int i = 0; i < ListViewLxr.Items.Count; i++) {
            CheckPersonControl control = ListViewLxr.Items[i] as CheckPersonControl;
            if (control == null) continue;
            if (App.listCreatGroupMember.Contains(control.No)) {
                control.ChkButton.IsChecked = true;
            } else {
                control.ChkButton.IsChecked = false;
            }
        }
    }


    private void ReSelectGzh() {
        for (int i = 0; i < ListViewGzh.Items.Count; i++) {
            CheckPersonControl control = ListViewGzh.Items[i] as CheckPersonControl;
            if (control == null) continue;
            if (App.listCreatGroupMember.Contains(control.No)) {
                control.ChkButton.IsChecked = true;
            } else {
                control.ChkButton.IsChecked = false;
            }
        }
    }

    /// <summary>
    /// 人员列表选项click事件
    /// </summary>
    /// <param Name="control"></param>
    /// <param Name="isChecked"></param>
    public   void ListSelect(CheckPersonControl control, bool isChecked) {
        try {
            //左侧列表
            if (control.Del == false ) {
                if (isChecked) {
                    //ListViewLxr.SelectedItems.Add(control);
                    CheckPersonControl del = new CheckPersonControl();
                    del.AX -= ListSelect;
                    del.AX += ListSelect;
                    del.Del = true;
                    del.HeadPortraitId = control.HeadPortraitId;
                    del.Name = control.Name;
                    del.Id = control.No;
                    del.No = control.No;
                    del.Width = 130;
                    ListBoxDel.Items.Add(del);
                    //list 加人
                    if (!App.listCreatGroupMember.Contains(control.No)) {
                        App.listCreatGroupMember.Add(control.No);
                    }
                } else {

                    //删除右侧相应的数据
                    for (int i = ListBoxDel.Items.Count - 1; i > -1; i--) {
                        CheckPersonControl p = ListBoxDel.Items[i] as CheckPersonControl;
                        if (p.No == control.No) {
                            ListBoxDel.Items.Remove(p);
                            if (App.listCreatGroupMember.Contains(control.No)) {
                                App.listCreatGroupMember.Remove(control.No);
                            }
                        }
                    }
                }
            } else  if (control.Del == true) {
                //右侧列表
                //右侧删除项
                ListBoxDel.Items.Remove(control);
                App.listCreatGroupMember.Remove(control.No);
                //左侧的项相应的操作
                if (App.listCreatGroupMember.Contains(control.No)) {
                    App.listCreatGroupMember.Remove(control.No);
                }

                ReSelect();
                ReSelectGzh();
                OrganizationList.ReSelect(null);
            }
        } catch (Exception ex) {
            Log.Error(typeof(SendCardWindow), ex);
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
            Log.Error(typeof(SendCardWindow), ex);
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e) {

        if (App.listCreatGroupMember.Count > 0) {
            for (int i = 0; i < App.listCreatGroupMember.Count; i++) {
                string no = App.listCreatGroupMember[i];

                if (ToolsHelper.getChatSessionTypeByNo(no) == ChatSessionType.PUBLIC) {
                    MessageItem item = MessageService.getInstance()
                                       .sendPublicCardMessage(no, ToUserNo);
                    if (item != null) {
                        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                        businessEvent.data = item;
                        businessEvent.eventDataType = BusinessEventDataType.SendPublicCard;
                        EventBusHelper.getInstance().fireEvent(businessEvent);
                    }
                } else {
                    MessageItem item = MessageService.getInstance()
                                       .sendVCardMessage(ToUserNo, no);
                    if (item != null) {
                        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
                        businessEvent.data = item;
                        businessEvent.eventDataType = BusinessEventDataType.SendVcCard;
                        EventBusHelper.getInstance().fireEvent(businessEvent);
                    }
                }

            }
            this.Close();
        }



    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.Source is TabControl) {
            if (Tab.SelectedIndex == 0) {
                ReSelect();
            } else  if (Tab.SelectedIndex ==1) {
                ReSelectGzh();
            } else {
                OrganizationList.ReSelect(null);
            }
        }

    }
}

}

