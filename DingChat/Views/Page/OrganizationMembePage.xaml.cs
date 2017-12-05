using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Views.Control;
using cn.lds.chatcore.pcw.Views.Windows;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common;
using GalaSoft.MvvmLight.Command;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// OrganizationMembePage.xaml 的交互逻辑
/// </summary>
public partial class OrganizationMembePage : System.Windows.Controls.Page {
    public OrganizationMembePage() {
        InitializeComponent();
        this.DataContext = this;

        //try {
        //    _getqlWorker.DoWork += GetqlWorkerDoWork;
        //    _getqlWorker.RunWorkerCompleted += GetqlWorkerRunWorkerCompleted;
        //} catch (Exception ex) {
        //    Log.Error(typeof(OrganizationMembePage), ex);
        //}

    }

    // 变量定义
    //点击组织人员 跳转到名片
    public event Action<int,string,string> ClickOrgMember;

    private ObservableCollection<OrganizationMemberTable> myListViewItems = new ObservableCollection<OrganizationMemberTable>();
    public ObservableCollection<OrganizationMemberTable> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }

    private List<OrganizationMemberTable> dt = new List<OrganizationMemberTable>();
    private int _organizationId;
    private string tenantNo;
    readonly BackgroundWorker _getqlWorker = new BackgroundWorker();
    public int OrganizationId { // the Name property
        get {

            return _organizationId;
        } set {
            _organizationId = value;
            try {
                OrganizationTable dtTen = OrganizationServices.getInstance().FindOrganizationByOrgId(OrganizationId);
                tenantNo = dtTen.tenantNo;
                myListViewItems.Clear();
                dt = OrganizationMemberService.getInstance().FindOrganizationMemberByOrgId(_organizationId);
                for (int i = 0; i < dt.Count; i++) {
                    dt[i].avatarPath = ImageHelper.getAvatarPath(dt[i].avatarId);
                    string postName = MasterServices.getInstance().getTextByTypeAndKey(MasterType.post, dt[i].post.ToStr(), tenantNo);
                    dt[i].postName = postName;
                    myListViewItems.Add(dt[i]);
                }
                ListViewLxr.ItemsSource = MyListViewItems;
            } catch (Exception ex) {
                Log.Error(typeof(OrganizationMembePage), ex);
            }
        }
    }



    /// <summary>
    /// 加载群聊
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    void GetqlWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
        try {
            ListViewLxr.Dispatcher.Invoke(new Action(() => {
                dt = OrganizationMemberService.getInstance().FindOrganizationMemberByOrgId(_organizationId);
                myListViewItems = new ObservableCollection<OrganizationMemberTable>();
                lock (this) {
                    for (int i = 0; i < dt.Count; i++) {
                        dt[i].avatarPath = ImageHelper.getAvatarPath(dt[i].avatarId);
                        string post = MasterServices.getInstance().getTextByTypeAndKey(MasterType.post, dt[i].post.ToStr(), tenantNo);
                        dt[i].post = post;
                        myListViewItems.Add(dt[i]);
                    }
                    ListViewLxr.ItemsSource = MyListViewItems;
                }

            }));
        } catch (Exception ex) {
            Log.Error(typeof(OrganizationMembePage), ex);
        }

    }
    private RelayCommand listViewLxrMouseLeftButtonUpCommand = null;
    public RelayCommand ListViewLxrMouseLeftButtonUpCommand {
        get {
            if (listViewLxrMouseLeftButtonUpCommand == null) {
                listViewLxrMouseLeftButtonUpCommand = new RelayCommand(() => {
                    if (ListViewLxr.SelectedIndex == -1) return;
                    if (ClickOrgMember != null ) {
                        OrganizationMemberTable o = ListViewLxr.SelectedItem as OrganizationMemberTable;
                        if (o == null)
                            return;

                        ClickOrgMember.Invoke(o.userId.ToStr().ToInt(), o.memberId,tenantNo);
                    }
                });
            }
            return listViewLxrMouseLeftButtonUpCommand;
        }
    }

    /// <summary>
    /// 获取组织成员
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    //void GetqlWorkerDoWork(object sender, DoWorkEventArgs e) {

    //    try {

    //    } catch (Exception ex) {
    //        Log.Error(typeof(OrganizationMembePage), ex);
    //    }
    //}





}
}
