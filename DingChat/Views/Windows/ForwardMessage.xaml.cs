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
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// ForwardMessage.xaml 的交互逻辑
/// </summary>
public partial class ForwardMessage : Window {
    public ForwardMessage() {
        InitializeComponent();

    }

    public MessageItem Message=null;
    private List<string> selectNo = new List<string>();
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
            Log.Error(typeof(ForwardMessage), ex);
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
            Log.Error(typeof(ForwardMessage), ex);
        }
    }

    private void LoadQl() {
        List<MucTable> groupContactsDt = MucServices.getInstance().FindAllGroup();
        //this.Dispatcher.BeginInvoke((Action)delegate () {
        ListViewQl.Items.Clear();
        for (int i = 0; i < groupContactsDt.Count; i++) {
            MucTable model = groupContactsDt[i];
            if (model == null) continue;
            CheckPersonControl p = new CheckPersonControl();
            p.HeadPortraitId = ImageHelper.loadAvatarPath(model.avatarStorageRecordId);
            p.Name = model.name;
            p.No = model.no;
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
    /// 切换tab页 需要把已选择的人选择状态
    /// </summary>
    private void ReSelect() {
        if(Tab.SelectedIndex==0) {
            for (int i = 0; i < ListViewLxr.Items.Count; i++) {
                CheckPersonControl control = ListViewLxr.Items[i] as CheckPersonControl;
                if (control == null) continue;
                if (selectNo.Contains(control.No)) {
                    control.ChkButton.IsChecked = true;
                } else {
                    control.ChkButton.IsChecked = false;
                }
            }
        } else {
            for (int i = 0; i < ListViewQl.Items.Count; i++) {
                CheckPersonControl control = ListViewQl.Items[i] as CheckPersonControl;
                if (control == null) continue;
                if (selectNo.Contains(control.No)) {
                    control.ChkButton.IsChecked = true;
                } else {
                    control.ChkButton.IsChecked = false;
                }
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
            if (control.Del == false) {
                if (isChecked) {
                    if(Tab.SelectedIndex==0) {
                        ListViewLxr.SelectedItems.Add(control);
                    } else {
                        ListViewQl.SelectedItems.Add(control);
                    }

                    CheckPersonControl del = new CheckPersonControl();
                    del.AX -= ListSelect;
                    del.AX += ListSelect;
                    del.Del = true;
                    del.HeadPortraitId = control.HeadPortraitId;
                    del.Name = control.Name;
                    del.No = control.No;
                    del.Width = 130;
                    ListBoxDel.Items.Add(del);
                    //list 加人
                    if (!selectNo.Contains(control.No)) {
                        selectNo.Add(control.No);
                    }
                } else {
                    //删除左侧选择状态
                    if (Tab.SelectedIndex == 0) {
                        ListViewLxr.SelectedItems.Remove(control);
                    } else {
                        ListViewQl.SelectedItems.Remove(control);
                    }

                    //删除右侧相应的数据
                    for (int i = ListBoxDel.Items.Count - 1; i > -1; i--) {
                        CheckPersonControl p = ListBoxDel.Items[i] as CheckPersonControl;
                        if (p.No == control.No) {
                            ListBoxDel.Items.Remove(p);
                            if (selectNo.Contains(control.No)) {
                                selectNo.Remove(control.No);
                            }
                        }
                    }

                }


            } else {
                //右侧列表
                //右侧删除项
                ListBoxDel.Items.Remove(control);
                //左侧的项相应的操作
                if (selectNo.Contains(control.No)) {
                    selectNo.Remove(control.No);
                }

                ReSelect();

            }
        } catch (Exception ex) {
            Log.Error(typeof(ForwardMessage), ex);
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
            Log.Error(typeof(ForwardMessage), ex);
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e) {

        if (Message == null) return;
        if (selectNo.Count >0) {

            for(int i=0; i< selectNo.Count; i++) {
                MessageService.getInstance().ForwardMessage(Message.messageId, selectNo[i]);
            }

            this.Close();
        }

    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.Source is TabControl) {
            ReSelect();
        }

    }
}

}

