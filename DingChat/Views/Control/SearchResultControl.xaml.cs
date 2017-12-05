using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// SearchResultControl.xaml 的交互逻辑
/// </summary>
public partial class SearchResultControl : UserControl {
    public SearchResultControl() {
        InitializeComponent();

        this.DataContext = this;

    }

    // 跳转后的回调，用于清除PcOA的检索框中的文本
    public delegate void RedirectCallBack();

    private static RedirectCallBack redirectCallBack;

    private ObservableCollection<SearchResultItem> searchResultItems = new ObservableCollection<SearchResultItem>();

    public ObservableCollection<SearchResultItem> SearchResultItems {
        get {
            return searchResultItems;
        } set {
            searchResultItems = value;
        }
    }

    private List<ContactsTable> allContacts = new List<ContactsTable>();
    private List<MucTable> allMucMember = new List<MucTable>();
    private List<PublicAccountsTable> allPublicAccount = new List<PublicAccountsTable>();

    /// <summary>
    /// 加载公众号
    /// </summary>
    private void InitPublicAccount() {
        try {
            allPublicAccount = PublicAccountsService.getInstance().findAllPublicAccounts();
        } catch (Exception ex) {
            Log.Error(typeof (SearchResultControl), ex);
        }
    }

    /// <summary>
    /// 加载群组
    /// </summary>
    private void InitMucMember() {
        try {
            allMucMember = MucServices.getInstance().FindAllGroup();
        } catch (Exception ex) {
            Log.Error(typeof (SearchResultControl), ex);
        }
    }

    /// <summary>
    /// 加载联系人
    /// </summary>
    private void InitContacts() {
        try {
            allContacts = ContactsServices.getInstance().FindAllFriend(null, null);
        } catch (Exception ex) {
            Log.Error(typeof (SearchResultControl), ex);
        }
    }

    private string searchKey = "";

    public int Search(string key) {
        searchKey = key;
        searchResultItems.Clear();
        if (String.IsNullOrEmpty(searchKey)) {
            return searchResultItems.Count;
        }

        SearchChat(key);
        SearchMucMember(key);
        SearchPublicAccount(key);
        if (searchResultItems.Count > 0) {
            SearchResultListView.SelectedIndex = 0;
        }

        return searchResultItems.Count;
    }

    /// <summary>
    /// 过滤联系人
    /// </summary>
    /// <param Name="key"></param>
    /// <returns></returns>
    private int SearchChat(string key) {

        InitContacts();

        List<SearchResultItem> listChatSession = (from bean in allContacts
                where bean.nickname.ToLower().Contains(key.ToLower())
        select new SearchResultItem {
            Type = ChatSessionType.CHAT,
            GroupName = "联系人",
            ItemNo = bean.clientuserId,
            Avatar = ImageHelper.getAvatarPath(bean.avatarStorageRecordId),
            MatchedDesc = "昵称：" + bean.nickname,
            Title = bean.nickname
        }).ToList();
        listChatSession.ForEach(item => searchResultItems.Add(item));

        return listChatSession.Count;
    }

    /// <summary>
    /// 过滤公众号
    /// </summary>
    /// <param Name="key"></param>
    /// <returns></returns>
    private int SearchPublicAccount(string key) {

        InitPublicAccount();

        List<SearchResultItem> listPublicAccount = (from bean in allPublicAccount
                where bean.name.ToLower().Contains(key.ToLower())
        select new SearchResultItem {
            Type = ChatSessionType.PUBLIC,
            GroupName = "公众号",
            ItemNo = bean.appid,
            Avatar = ImageHelper.getAvatarPath(bean.logoId),
            MatchedDesc = String.Empty,
            Title = bean.name
        }).ToList();
        listPublicAccount.ForEach(item => searchResultItems.Add(item));

        return listPublicAccount.Count;
    }

    /// <summary>
    /// 过滤群聊
    /// </summary>
    /// <param Name="key"></param>
    /// <returns></returns>
    private int SearchMucMember(string key) {

        InitMucMember();

        List<SearchResultItem> listMucMember = (from bean in allMucMember
                                                where bean.name.ToLower().Contains(key.ToLower())
        select new SearchResultItem {
            Type = ChatSessionType.MUC,
            GroupName = "群聊",
            ItemNo = bean.no,
            Avatar = ImageHelper.getAvatarPath(bean.avatarStorageRecordId),
            MatchedDesc = string.Empty,
            Title = bean.name
        }).ToList();
        listMucMember.ForEach(item => searchResultItems.Add(item));

        return listMucMember.Count;
    }

    public void SetRedirectCallBack(RedirectCallBack callback) {
        redirectCallBack = callback;
    }

    private RelayCommand<TextBlock> textBlockLoadedCommand = null;

    public RelayCommand<TextBlock> TextBlockLoadedCommand {
        get {
            if (textBlockLoadedCommand == null) {
                textBlockLoadedCommand = new RelayCommand<TextBlock>((TextBlock txtBlock) => {
                    string text = txtBlock.Text;
                    if (string.IsNullOrEmpty(text)) {
                        return;
                    }
                    int searchKeyLen = searchKey.Length;
                    int startIndex = text.ToLower().IndexOf(searchKey.ToLower());
                    string matchKey = text.Substring(startIndex, searchKeyLen);
                    txtBlock.Text = "";
                    txtBlock.Inlines.Add(new Run(text.Substring(0, startIndex)));
                    Run highlight = new Run(matchKey);
                    highlight.Foreground = new SolidColorBrush(Color.FromRgb(0x40, 0xBB, 0x3A));
                    txtBlock.Inlines.Add(highlight);
                    txtBlock.Inlines.Add(new Run(text.Substring(startIndex + searchKeyLen)));
                });
            }
            return textBlockLoadedCommand;
        }
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
        int selIndex = SearchResultListView.SelectedIndex;
        if (selIndex!= -1) {
            if (selIndex + 1 < SearchResultItems.Count) {
                SearchResultListView.SelectedIndex = selIndex + 1;
            } else {
                SearchResultListView.SelectedIndex = 0;
            }
        }
        SearchResultListView.ScrollIntoView(SearchResultListView.SelectedItem);
    }

    public void ItemUpKeyUpCommand() {
        int selIndex = SearchResultListView.SelectedIndex;
        if (selIndex != -1) {
            if (SearchResultListView.SelectedIndex > 0) {
                SearchResultListView.SelectedIndex = selIndex - 1;
            } else {
                SearchResultListView.SelectedIndex = SearchResultItems.Count -1;
            }
        }
        SearchResultListView.ScrollIntoView(SearchResultListView.SelectedItem);
    }

/// <summary>
/// 点击检索结果
/// </summary>
    public void ItemClickCommandFun() {
        if (SearchResultListView.SelectedItem != null) {
            // 跳转
            BusinessEvent<object> businessdata = new BusinessEvent<object>();
            SearchResultItem selectItem = SearchResultListView.SelectedItem as SearchResultItem;
            switch (selectItem.Type) {
            case ChatSessionType.CHAT:
                VcardsTable vsModel = VcardService.getInstance().findByNo(selectItem.ItemNo);
                businessdata.data = vsModel.clientuserId;
                businessdata.eventDataType = BusinessEventDataType.RedirectChatSessionEvent;
                EventBusHelper.getInstance().fireEvent(businessdata);
                break;
            case ChatSessionType.MUC:
                businessdata.data = selectItem.ItemNo;
                businessdata.eventDataType = BusinessEventDataType.RedirectMucChatSessionEvent;
                EventBusHelper.getInstance().fireEvent(businessdata);
                break;
            case ChatSessionType.PUBLIC:
                businessdata.data = selectItem.ItemNo;
                businessdata.eventDataType = BusinessEventDataType.RedirectPublicChatSessionEvent;
                EventBusHelper.getInstance().fireEvent(businessdata);
                break;
            default:
                Log.Info(typeof(PcOA), "未识别的消息类型（检索）");
                break;
            }

            redirectCallBack();
        }
    }
}
}
