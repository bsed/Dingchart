using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Windows;
using EventBus;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.imtp.message;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// GroupChatDetailed.xaml 的交互逻辑
/// </summary>
public partial class GroupChatDetailedPage : System.Windows.Controls.Page {


    public GroupChatDetailedPage() {
        InitializeComponent();
    }
    public event Action<string, string> ClickAllGroup;
    private static GroupChatDetailedPage instance = null;
    public static GroupChatDetailedPage getInstance() {
        if (instance == null) {
            instance = new GroupChatDetailedPage();
        }
        return instance;
    }

    // 变量定义
    //群名称   正式的应该传过来群的id 查询出结果
    public string MucNo = string.Empty;
    public string MucId = string.Empty;

    private MucTable table = new MucTable();

    /// <summary>
    /// C023: 设置我在群中的昵称
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void onHttpRequestEvent(EventData<Object> eventData) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (eventData.eventDataType) {

            //  C023: 设置我在群中的昵称
            case EventDataType.updateNicknameInGroup:
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
    /// 业务事件监听
    /// </summary>
    /// <param Name="data"></param>
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<object> data) {
        if (App.mainWindowLoaded == false) return;
        try {
            switch (data.eventDataType) {
            // 群成员变化
            case BusinessEventDataType.MucEditTextChangedEvent_TYPE_ADD:
                DoMucEditTextChangedEvent_TYPE_ADD(data);
                break;
            // 群成员变化
            case BusinessEventDataType.MucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE:
                DoMucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE(data);
                break;
            // 群成员昵称变更
            case BusinessEventDataType.GroupMemberNicknameChangedEvent:
                DoGroupMemberNicknameChangedEvent(data);
                break;
            // 修改群名
            case BusinessEventDataType.MucChangeEvent_TYPE_API_UPDATE_GROUP_NAME:
                DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(data);
                break;
            // 修改昵称
            case BusinessEventDataType.GroupMemberNicknameChangedEvent_TYPE_UPDATE_ME:
                DoGroupMemberNicknameChangedEvent_TYPE_UPDATE_ME(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }

    /// <summary>
    /// 群成员变化
    /// </summary>
    /// <param Name="data"></param>
    public void DoMucEditTextChangedEvent_TYPE_ADD(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            string dtMucNo = data.data.ToStr();
            if (dtMucNo == string.Empty) return;
            if (string.IsNullOrEmpty(MucNo)) return;
            if (dtMucNo != MucNo) return;
            Thread.Sleep(500);
            this.Dispatcher.BeginInvoke((Action)delegate() {
                Refesh();
            });
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 群成员变化
    /// </summary>
    /// <param Name="data"></param>
    public void DoMucChangeEvent_TYPE_MESSAGE_GROUP_MEMBER_CHANGE(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            MucTable dt = data.data as MucTable;

            if (string.IsNullOrEmpty(MucNo)) return;
            if (dt.no != MucNo) return;
            Thread.Sleep(500);
            this.Dispatcher.BeginInvoke((Action)delegate() {
                Refesh();
            });
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 群成员昵称变更
    /// </summary>
    /// <param Name="data"></param>
    public void DoGroupMemberNicknameChangedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;
            GroupMemberNicknameChangedEventData dt = data.data as GroupMemberNicknameChangedEventData;
            if (dt == null) return;

            MucMembersTable model = MucServices.getInstance().findByMucNoAndClientuserId(dt.mucNo, dt.memberId);

            this.Dispatcher.Invoke(new Action(() => {
                Refesh();
            }));
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 修改群名
    /// </summary>
    /// <param Name="data"></param>
    public void DoMucChangeEvent_TYPE_API_UPDATE_GROUP_NAME(BusinessEvent<object> data) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                MucTable mucTable = (MucTable)data.data;
                BtnGroupName.Tag = mucTable.name;
                BtnBack.Tag = mucTable.name;
            }));
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 修改昵称
    /// </summary>
    /// <param Name="data"></param>
    public void DoGroupMemberNicknameChangedEvent_TYPE_UPDATE_ME(BusinessEvent<object> data) {
        try {
            this.Dispatcher.Invoke(new Action(() => {
                MucMembersTable model = data.data as MucMembersTable;
                if (model == null) return;
                BtnGroupNickname.Tag = model.nickname;

                personGroupControl.Change(model);

                //personGroupControl.MucNo = MucNo;
            }));
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 刷新画面
    /// </summary>
    private void Refesh() {
        Stopwatch wat = new Stopwatch();
        try {
            wat.Start();
            if (MucNo == string.Empty) return;
            Thread thread = new Thread(() => {
                personGroupControl.MucNo = MucNo;
            });
            thread.IsBackground = true;
            thread.Start();
            wat.Stop();
            Console.WriteLine(wat.ElapsedMilliseconds);
            wat.Restart();
            table = MucServices.getInstance().FindGroupByNo(MucNo);
            if (table == null) return;
            wat.Stop();
            Console.WriteLine(wat.ElapsedMilliseconds);

            BtnBack.Tag = table.name;

            BtnGroupName.Tag = table.name;
            LbCount.Content = "全部群成员(" + table.count + ")";
            ChkAddressList.IsChecked = table.savedAsContact;
            ChkChatTop.IsChecked = table.isTopmost;
            ChkNoTrouble.IsChecked = table.enableNoDisturb;
            wat.Restart();
            MucMembersTable mucMembersTable = MucServices.getInstance().findByMucIdAndClientuserId(table.mucId, App.AccountsModel.clientuserId);
            if (mucMembersTable != null) {
                BtnGroupNickname.Tag = mucMembersTable.nickname;
            }
            wat.Stop();
            Console.WriteLine(wat.ElapsedMilliseconds);
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }

    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Page_Loaded_1(object sender, RoutedEventArgs e) {
        try {
            Refesh();
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 修改群名称
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnGroupName_Click(object sender, RoutedEventArgs e) {
        try {
            EditGroupName editGroupName = new EditGroupName();
            editGroupName.MucNo = MucNo;
            editGroupName.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }

    /// <summary>
    /// 修改群昵称
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnGroupNickname_Click(object sender, RoutedEventArgs e) {
        try {
            EditNickName editNickName = new EditNickName();
            editNickName.MucNo = MucNo;
            editNickName.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }
    /// <summary>
    /// 设置群公告
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnGroupNotice_Click(object sender, RoutedEventArgs e) {
        return;
        try {
            EditGroupNotice editGroupNotice = new EditGroupNotice();
            editGroupNotice.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// TODO:这个是啥
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnGanapati_Click(object sender, RoutedEventArgs e) {
        try {

        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    /// <summary>
    /// 修改群主
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnChangeOwner_Click(object sender, RoutedEventArgs e) {
        return;
        try {
            CommonMessageBox.Msg.Show("确认选择xx为群主？", CommonMessageBox.MsgBtn.OKCancel);
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }

    /// <summary>
    /// 加入通讯
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkAddressList_Checked(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkAddressList.IsChecked.ToStr().ToBool();
            ContactsApi.addGroupToAddressList(table.mucId, check);
            MucServices.getInstance().SendGroupSavedAsContactMessage(check, table.no);
            if (table.activeFlag == false) {
                // 激活群
                MucServices.getInstance().RequestChangeMucStatus(table.mucId);
            }
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }

    /// <summary>
    /// 置顶
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkChatTop_Checked(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkChatTop.IsChecked.ToStr().ToBool();
            ContactsApi.setGroupTopmost(table.mucId, check);
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }


    }
    /// <summary>
    /// 消息免打扰
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void ChkNoTrouble_Checked(object sender, RoutedEventArgs e) {
        try {
            bool check = ChkNoTrouble.IsChecked.ToStr().ToBool();
            ContactsApi.enableGroupNoDisturb(table.mucId, check);
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }

    /// <summary>
    /// 清空聊天记录
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnCleanUp_Click(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确认清空聊天记录 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                MessageService.getInstance().clearMessages(MucNo);
                NotificationHelper.ShowSuccessMessage("清空聊天记录完成！");
            }
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }

    }
    public event Action<string> BtnBackOnClick;
    private void BtnBack_Click(object sender, RoutedEventArgs e) {
        if (BtnBackOnClick != null && e != null) {

            BtnBackOnClick.Invoke(MucNo);
        }
    }

    private void BtnAllGroup_Click_1(object sender, RoutedEventArgs e) {
        if (ClickAllGroup != null && e != null) {

            ClickAllGroup.Invoke(MucNo, LbCount.Content.ToStr());
        }

    }

    /// <summary>
    /// 退出群
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void BtnExitGroup_OnClick(object sender, RoutedEventArgs e) {
        try {
            if (CommonMessageBox.Msg.Show("确定要退出群组并删除聊天记录 ?", CommonMessageBox.MsgBtn.YesNO) ==
                    CommonMessageBox.MsgResult.Yes) {
                MessageService.getInstance().clearMessages(MucNo);
                MucTable mucTable =  MucServices.getInstance().FindGroupByNo(MucNo);
                if (mucTable!=null) {
                    MucId = mucTable.mucId;
                }
                ContactsApi.deleteGroup(MucId, MucNo);
            }
        } catch (Exception ex) {
            Log.Error(typeof(GroupChatDetailedPage), ex);
        }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e) {
        Thread thread = new Thread(() => {
            personGroupControl.MucNo = "-1";
        });
        thread.IsBackground = true;
        thread.Start();

    }
}
}
