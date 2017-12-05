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
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Views.Windows;
using EventBus;
using GalaSoft.MvvmLight.Command;

namespace cn.lds.chatcore.pcw.Views.Page.PublicAccounts {
/// <summary>
/// PublicAccountsFindPage.xaml 的交互逻辑
/// </summary>
public partial class PublicAccountsFindPage : System.Windows.Controls.Page {
    public PublicAccountsFindPage() {
        InitializeComponent();
        this.DataContext = this;
    }
    private ObservableCollection<PublicAccountsBean> myListViewItems = new ObservableCollection<PublicAccountsBean>();
    public ObservableCollection<PublicAccountsBean> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }
    private static PublicAccountsFindPage instance = null;
    public static PublicAccountsFindPage getInstance() {
        if (instance == null) {
            instance = new PublicAccountsFindPage();
        }
        return instance;
    }
    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {

        try {
            switch (data.eventDataType) {

            case BusinessEventDataType.PublicAccountFindEvent:
                DoPublicAccountFindEvent(data);
                break;
            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicAccountsFindPage), ex);
        }

    }

    private void SearchText_OnPreviewKeyDown(object sender, KeyEventArgs e) {
        switch (e.Key) {
        case Key.Enter:
            ContactsApi.searchServiceAccounts(SearchText.Text, 0, 1000);
            e.Handled = true;
            break;
        }
    }

    private void DoPublicAccountFindEvent(BusinessEvent<object> data) {
        if (data.data == null) return;
        List<PublicAccountsBean> item = data.data as List<PublicAccountsBean>;
        if (item == null) return;

        this.Dispatcher.BeginInvoke((Action)delegate () {

            myListViewItems.Clear();
            for (int i = 0; i < item.Count; i++) {
                item[i].logoPath = ImageHelper.getAvatarPath(item[i].logoId);
                myListViewItems.Add(item[i]);
            }
        });
    }

    private RelayCommand listMouseLeftButtonUpCommand = null;
    public RelayCommand ListMouseLeftButtonUpCommand {
        get {
            if (listMouseLeftButtonUpCommand == null) {
                listMouseLeftButtonUpCommand = new RelayCommand(() => {
                    ListBoxLeft_SelectionChanged();
                });
            }
            return listMouseLeftButtonUpCommand;
        }
    }

    private void ListBoxLeft_SelectionChanged() {

        try {
            object o = ListBoxLeft.SelectedItem;
            if (o == null)
                return;

            PublicAccountsBean p = o as PublicAccountsBean;
            if (p == null)
                return;
            PublicAccountsDetailedWindow win = new PublicAccountsDetailedWindow();
            win.tenantNo = p.tenantNo;
            win.AppId = p.appId;
            win.ShowDialog();
        } catch (Exception ex) {
            Log.Error(typeof (PublicAccountsFindPage), ex);
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e) {
        AddressBookPage.getInstance().loadFindPublic = false;
    }
}
}
