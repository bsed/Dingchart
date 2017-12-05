using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using System.Diagnostics;
using cn.lds.chatcore.pcw.Common.Enums;

namespace cn.lds.chatcore.pcw.Views.Windows {
/// <summary>
/// CreateGroup.xaml 的交互逻辑
/// </summary>
public partial class CreateGroup : Window {
    public CreateGroup() {
        InitializeComponent();

    }
    public  List<string> listOldMember = new List<string>();
    List<string> listAddPerson = new List<string>();
    List<string> listRemovePerson = new List<string>();

    //单聊的人no
    public string UserId = string.Empty;

    //群加人减人
    public string MucNo = string.Empty;
    private string MucId = string.Empty;
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
            Log.Error(typeof(CreateGroup), ex);
        }
    }

    private MucTable mucTable = null;
    /// <summary>
    /// 画面加载事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        try {
            App.listCreatGroupMember = new List<string>();
            if(!string.IsNullOrEmpty(MucNo)) {
                mucTable = MucServices.getInstance().FindGroupByNo(MucNo);
                if (mucTable == null) return;
                LbMember.SetValue(Grid.RowProperty, 0);
                BorderMember.SetValue(Grid.RowProperty, 1);
                BorderMember.SetValue(Grid.RowSpanProperty, 3);
                TxtMucName.Visibility = Visibility.Hidden;
                LbName.Visibility = Visibility.Hidden;

            } else {
                TxtMucName.Visibility = Visibility.Visible;
                LbName.Visibility = Visibility.Visible;
                LbMember.SetValue(Grid.RowProperty, 2);
                BorderMember.SetValue(Grid.RowProperty, 3);
                //BorderMember.SetValue(Grid.RowSpanProperty, 3);
            }


            List<ContactsTable> contactsDt = ContactsServices.getInstance().FindAllFriend(null, null);
            //this.Dispatcher.BeginInvoke((Action)delegate () {
            ListViewLxr.Items.Clear();
            for (int i = 0; i < contactsDt.Count; i++) {
                ContactsTable model = contactsDt[i];
                if (model == null) continue;
                if(model.clientuserId== UserId) {
                    continue;
                }
                if (model.clientuserId == App.AccountsModel.clientuserId) {
                    continue;
                }
                CheckPersonControl p = new CheckPersonControl();
                p.HeadPortraitId = ImageHelper.loadAvatarPath(model.avatarStorageRecordId);
                p.Name = model.nickname;
                p.Id = model.clientuserId;
                p.No = model.no;
                p.AX -= ListSelect;
                p.AX += ListSelect;
                p.Width = ListViewLxr.ActualWidth;

                ListViewLxr.Items.Add(p);

                DoEvents();
            }
            ListViewLxr.UpdateLayout();
            //});

            Thread orgThread = new Thread(() => {
                OrganizationList.Init(this, UserId,App.CurrentTenantNo);
            });
            orgThread.IsBackground = true;
            orgThread.Start();

            //Stopwatch watch = new Stopwatch();
            //return;
            //群加人的情况
            Thread addThread = new Thread(() => {
                if (MucNo != string.Empty) {
                    //watch.Start();
                    List<MucMembersTable> dt = MucServices.getInstance().findByMucNo(MucNo);
                    //watch.Stop();
                    //Console.WriteLine("sssss"+watch.ElapsedMilliseconds);

                    if (dt.Count == 0) return;
                    //watch.Restart();


                    MucId = mucTable.mucId;


                    foreach (MucMembersTable mucMember in dt) {
                        if (mucMember.no == App.AccountsModel.no) {
                            continue;
                        }
                        this.Dispatcher.BeginInvoke((Action) delegate() {
                            this.Title = "";
                            CheckPersonControl del = new CheckPersonControl();
                            del.AX -= ListSelect;
                            del.AX += ListSelect;
                            del.Del = true;
                            del.HeadPortraitId = ImageHelper.loadAvatarPath(mucMember.avatarStorageRecordId);
                            del.Name = mucMember.nickname;
                            del.Id = mucMember.clientuserId;
                            del.No = mucMember.no;
                            del.Width = 130;
                            if(mucTable!=null && mucTable.isOwner) {
                                ListBoxDel.Items.Add(del);
                            }

                            ListBoxDel.UpdateLayout();
                        });

                        //list 加人
                        if (!App.listCreatGroupMember.Contains(mucMember.no)) {
                            App.listCreatGroupMember.Add(mucMember.no);
                        }

                        if (!listOldMember.Contains(mucMember.clientuserId)) {
                            listOldMember.Add(mucMember.clientuserId);
                        }

                    }
                    //watch.Stop();
                    //Console.WriteLine("wwww"+watch.ElapsedMilliseconds);
                    this.Dispatcher.BeginInvoke((Action)delegate () {
                        TxtMucName.Text = mucTable.name;
                        ReSelect();
                        OrganizationList.ReSelect(mucTable);
                    });
                }
            });
            addThread.IsBackground = true;
            addThread.Start();


        } catch (Exception ex) {
            Log.Error(typeof(CreateGroup), ex);
        }
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
                if (mucTable != null && !mucTable.isOwner) {
                    control.IsEnabled = false;
                } else {
                    control.IsEnabled = true;
                }
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
            if (control.Del == false) {
                if (isChecked) {
                    ListViewLxr.SelectedItems.Add(control);
                    CheckPersonControl del = new CheckPersonControl();
                    del.AX -= ListSelect;
                    del.AX += ListSelect;
                    del.Del = true;
                    del.HeadPortraitId = control.HeadPortraitId;
                    del.Name = control.Name;
                    del.Id = control.Id;
                    del.No = control.No;
                    del.Width = 130;
                    ListBoxDel.Items.Add(del);
                    //list 加人
                    if (!App.listCreatGroupMember.Contains(control.No)) {
                        App.listCreatGroupMember.Add(control.No);
                    }

                    if (!listOldMember.Contains(control.Id)) {
                        if(!listAddPerson.Contains(control.Id)) {
                            listAddPerson.Add(control.Id);
                        }
                        if(listRemovePerson.Contains(control.Id)) {
                            listRemovePerson.Remove(control.Id);
                        }
                    }
                    if (listOldMember.Contains(control.Id)) {
                        if (listAddPerson.Contains(control.Id)) {
                            listAddPerson.Remove(control.Id);
                        }
                        if (listRemovePerson.Contains(control.Id)) {
                            listRemovePerson.Remove(control.Id);
                        }
                    }

                } else {
                    //删除左侧选择状态
                    ListViewLxr.SelectedItems.Remove(control);
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
                    if (listOldMember.Contains(control.Id)) {
                        if (listAddPerson.Contains(control.Id)) {
                            listAddPerson.Remove(control.Id);
                        }
                        if (!listRemovePerson.Contains(control.Id)) {
                            listRemovePerson.Add(control.Id);
                        }
                    }
                    if (!listOldMember.Contains(control.Id)) {
                        if (listAddPerson.Contains(control.Id)) {
                            listAddPerson.Remove(control.Id);
                        }
                    }
                }


            } else {
                //右侧列表
                //右侧删除项
                ListBoxDel.Items.Remove(control);
                //左侧的项相应的操作
                if (App.listCreatGroupMember.Contains(control.No)) {
                    App.listCreatGroupMember.Remove(control.No);
                }
                if (listOldMember.Contains(control.Id)) {
                    if (listAddPerson.Contains(control.Id)) {
                        listAddPerson.Remove(control.Id);
                    }
                    if (!listRemovePerson.Contains(control.Id)) {
                        listRemovePerson.Add(control.Id);
                    }
                }
                if (!listOldMember.Contains(control.Id)) {
                    if (listAddPerson.Contains(control.Id)) {
                        listAddPerson.Remove(control.Id);
                    }
                    //if (!listRemovePerson.Contains(control.Id))
                    //{
                    //    listRemovePerson.Add(control.Id);
                    //}
                }
                ReSelect();
                OrganizationList.ReSelect(mucTable);
            }
        } catch (Exception ex) {
            Log.Error(typeof(CreateGroup), ex);
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
            Log.Error(typeof(CreateGroup), ex);
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e) {

        if (ListBoxDel.Items.Count == 0) return;

        List<string> listAdd = new List<string>();
        if(UserId!=string.Empty) {
            listAdd.Add(UserId);
        }

        for (int i = 0; i < ListBoxDel.Items.Count; i++) {
            CheckPersonControl control = ListBoxDel.Items[i] as CheckPersonControl;
            if (control == null) continue;
            if (!listAdd.Contains(control.Id)) {
                listAdd.Add(control.Id);
            }
        }
        //群加人 减人
        if (MucId!=string.Empty) {
            if(listAddPerson.Count>0) {
                ContactsApi.addGroupMember(MucId, listAddPerson, SourceType.addressList.ToStr());
            }

            if (listRemovePerson.Count > 0) {
                ContactsApi.deleteGroupMember(MucId, listRemovePerson);
            }
            this.Close();
        } else {
            if (listAdd.Count == 1) {
                BusinessEvent<object> businessdata = new BusinessEvent<object>();
                businessdata.data = listAdd[0];
                businessdata.eventDataType = BusinessEventDataType.RedirectChatSessionEvent;
                EventBusHelper.getInstance().fireEvent(businessdata);
                this.Close();
            } else if (listAdd.Count > 1) {
                if (TxtMucName.Text.Trim() == string.Empty) {
                    this.Topmost = true;
                    NotificationHelper.ShowWarningMessage("群名称不能为空！");
                    return;
                }
                MucServices.getInstance().createGroup(TxtMucName.Text, listAdd);
                this.Close();
            }
        }



    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.Source is TabControl) {
            if (Tab.SelectedIndex == 0) {
                ReSelect();
            } else {
                OrganizationList.ReSelect(mucTable);
            }
        }

    }
}

}

