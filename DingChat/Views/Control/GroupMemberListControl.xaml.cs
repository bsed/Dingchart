using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using System.Windows.Controls;

using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using GalaSoft.MvvmLight.Command;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// GroupMemberListControl.xaml 的交互逻辑
/// </summary>
public partial class GroupMemberListControl : UserControl {
    public GroupMemberListControl() {
        InitializeComponent();

        this.DataContext = this;
    }

    // 选择某项后的回调
    public delegate void ItemClickCallBack(string title, string no);

    private static ItemClickCallBack itemClickCallBack;

    public void SetItemClickCallBack(ItemClickCallBack callback) {
        itemClickCallBack = callback;
    }

    private ObservableCollection<SearchResultItem> groupMemberItems = new ObservableCollection<SearchResultItem>();

    public ObservableCollection<SearchResultItem> GroupMemberItems {
        get {
            return groupMemberItems;
        } set {
            groupMemberItems = value;
        }
    }

    private List<MucMembersTable> allMembers = new List<MucMembersTable>();


    /// <summary>
    /// 加载群内联系人
    /// </summary>
    private void InitMembers() {
        try {
            string mucNo = ChatPage.getInstance().MucNo;
            allMembers = MucServices.getInstance().findByMucNo(mucNo);
        } catch (Exception ex) {
            Log.Error(typeof (SearchResultControl), ex);
        }
    }

    private string searchKey = "";

    public int Search(string key) {
        searchKey = key;
        groupMemberItems.Clear();

        SearchMember(key);

        if (groupMemberItems.Count > 0) {
            GroupMemberListView.SelectedIndex = 0;
        }

        return groupMemberItems.Count;
    }

    /// <summary>
    /// 过滤联系人
    /// </summary>
    /// <param Name="key"></param>
    /// <returns></returns>
    private int SearchMember(string key) {

        InitMembers();
        string currClientNo = App.AccountsModel.clientuserId;
        List<SearchResultItem> listChatSession = new List<SearchResultItem>();
        if (String.IsNullOrEmpty(key)) {
            listChatSession =(from bean in allMembers
                              where !bean.clientuserId.Equals(currClientNo)
            select new SearchResultItem {
                Type = ChatSessionType.CHAT,
                GroupName = "联系人",
                ItemNo = bean.no,
                Avatar = ImageHelper.getAvatarPath(bean.avatarStorageRecordId),
                MatchedDesc = "昵称：" + bean.nickname,
                Title = bean.nickname
            }).ToList();
        } else {
            listChatSession = (from bean in allMembers
                               where bean.nickname.ToLower().Contains(key.ToLower())
                               && !bean.clientuserId.Equals(currClientNo)
            select new SearchResultItem {
                Type = ChatSessionType.CHAT,
                GroupName = "联系人",
                ItemNo = bean.no,
                Avatar = ImageHelper.getAvatarPath(bean.avatarStorageRecordId),
                MatchedDesc = "昵称：" + bean.nickname,
                Title = bean.nickname
            }).ToList();

        }
        listChatSession.ForEach(item => groupMemberItems.Add(item));

        return listChatSession.Count;
    }

    private RelayCommand itemClickCommand = null;
    public RelayCommand ItemClickCommand {
        get {
            if (itemClickCommand == null) {
                itemClickCommand = new RelayCommand(ItemClickCommandFun);
            }
            return itemClickCommand;
        }
    }

    public void ItemDownKeyUpCommand() {
        int selIndex = GroupMemberListView.SelectedIndex;
        if (selIndex!= -1) {
            if (selIndex + 1 < GroupMemberItems.Count) {
                GroupMemberListView.SelectedIndex = selIndex + 1;
            } else {
                GroupMemberListView.SelectedIndex = 0;
            }
        }
        GroupMemberListView.ScrollIntoView(GroupMemberListView.SelectedItem);
    }

    public void ItemUpKeyUpCommand() {
        int selIndex = GroupMemberListView.SelectedIndex;
        if (selIndex != -1) {
            if (GroupMemberListView.SelectedIndex > 0) {
                GroupMemberListView.SelectedIndex = selIndex - 1;
            } else {
                GroupMemberListView.SelectedIndex = GroupMemberItems.Count -1;
            }
        }
        GroupMemberListView.ScrollIntoView(GroupMemberListView.SelectedItem);
    }

/// <summary>
/// 双击检索结果
/// </summary>
    public void ItemClickCommandFun() {
        if (GroupMemberListView.SelectedItem != null) {
            if (itemClickCallBack != null) {
                itemClickCallBack((GroupMemberListView.SelectedItem as SearchResultItem).Title,
                                  (GroupMemberListView.SelectedItem as SearchResultItem).ItemNo);
            }
        }
    }
}
}
