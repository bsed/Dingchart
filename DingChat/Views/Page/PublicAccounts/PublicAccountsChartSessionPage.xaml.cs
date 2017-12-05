using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control;

namespace cn.lds.chatcore.pcw.Views.Page.PublicAccounts {
/// <summary>
/// PublicAccountsChartPage.xaml 的交互逻辑
/// </summary>
public partial class PublicAccountsChartSessionPage : System.Windows.Controls.Page {
    public PublicAccountsChartSessionPage() {
        InitializeComponent();
    }
    private static PublicAccountsChartSessionPage instance = null;
    public static PublicAccountsChartSessionPage getInstance() {
        if (instance == null) {
            instance = new PublicAccountsChartSessionPage();
        }
        return instance;
    }

    private ObservableCollection<ChatSessionBean> myListViewItems = new ObservableCollection<ChatSessionBean>();
    public ObservableCollection<ChatSessionBean> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }

    private ObservableCollection<ChatSessionBean> topListViewItems = new ObservableCollection<ChatSessionBean>();
    public ObservableCollection<ChatSessionBean> TopListViewItems {
        get {
            return topListViewItems;
        } set {
            topListViewItems = value;
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e) {
        try {
            List<ChatSessionBean> dtTop = ChatSessionService.getInstance().findPublic(true);
            topListViewItems.Clear();
            for (int i = 0; i < dtTop.Count; i++) {
                //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                bool flagTop = topListViewItems.Where(q => q.Contact.Equals(dtTop[i].Contact)).Count() > 0;
                if (flagTop == false) {

                    topListViewItems.Add(dtTop[i]);
                }
            }



            List<ChatSessionBean> dt = ChatSessionService.getInstance().findPublic(false);
            myListViewItems.Clear();
            for (int i = 0; i < dt.Count; i++) {
                //查看是否存在 不存在增加  避免和来消息更新chartsession 的时候发生冲突
                bool flag = myListViewItems.Where(q => q.Contact.Equals(dt[i].Contact)).Count() > 0;
                if (flag == false) {

                    myListViewItems.Add(dt[i]);
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsChartSessionPage), ex);
        }
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        try {
            ListView list = sender as ListView;
            object o = list.SelectedItem;
            if (o == null)
                return;

            ChatSessionBean p = o as ChatSessionBean;
            list.ScrollIntoView(p);
            //p.IsChecked = true;

            BusinessEvent<object> Businessdata = new BusinessEvent<object>();
            Businessdata.data = p.Contact;
            Businessdata.eventDataType = BusinessEventDataType.ClickPublic;
            EventBusHelper.getInstance().fireEvent(Businessdata);

        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsChartSessionPage), ex);
        }
    }
}
}
