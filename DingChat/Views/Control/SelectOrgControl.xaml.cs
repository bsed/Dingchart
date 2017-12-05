using cn.lds.chatcore.pcw.Models.Tables;
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
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Services;
using GalaSoft.MvvmLight.Command;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Views.Windows;
using javax.naming;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// SelectOrgControl.xaml 的交互逻辑
/// </summary>
public partial class SelectOrgControl : UserControl {
    public SelectOrgControl() {
        InitializeComponent();
        this.DataContext = this;

        CollectionView cv = new CollectionView(App.TenantListViewItems);
        this.cmb.ItemsSource = cv;

        cmb.SelectedValue = App.CurrentTenantNo;
    }
    public event Action<CheckPersonControl, bool> SecletPerson;
    public CreateGroup parentWindow;
    //当前选择的租户
    private string selectTentantNo=string.Empty;
    public SendCardWindow parentCardWindow;

    private ObservableCollection<OrganizationTable> myListViewItems = new ObservableCollection<OrganizationTable>();
    public ObservableCollection<OrganizationTable> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }
    //标题栏
    private ObservableCollection<OrganizationTable> myListBoxItems = new ObservableCollection<OrganizationTable>();
    public ObservableCollection<OrganizationTable> MyListBoxItems {
        get {
            return myListBoxItems;
        } set {
            myListBoxItems = value;
        }
    }

    private void LoadOrg(int id) {
        myListViewItems = new ObservableCollection<OrganizationTable>();
        List<OrganizationTable> list = OrganizationServices.getInstance().FindChildOrganization(id);
        if (list!=null) {
            for (int i = 0; i < list.Count; i++) {
                list[i].logoPath = ImageHelper.getOrgAvatarPath(list[i].logoStorageRecordId);
                myListViewItems.Add(list[i]);
            }
        }
        this.Dispatcher.BeginInvoke((Action) delegate {
            ListViewOrg.ItemsSource = MyListViewItems;
        });

    }


    private void LoadLxr(int id,string userId) {

        List<OrganizationMemberTable> contactsDt=OrganizationMemberService.getInstance().FindOrganizationMemberByOrgId(id);

        this.Dispatcher.BeginInvoke((Action)delegate () {
            ListViewLxr.Items.Clear();
            for (int i = 0; i < contactsDt.Count; i++) {
                OrganizationMemberTable model = contactsDt[i];
                if (model == null) continue;
                if(model.userId== userId) continue;
                if (model.userId == App.AccountsModel.clientuserId) continue;
                CheckPersonControl p = new CheckPersonControl();
                p.HeadPortraitId = ImageHelper.loadAvatarPath(model.avatarId);
                p.Name = model.nickname;
                p.Id = model.userId;
                p.No = model.no;
                if(parentWindow!=null) {
                    p.AX -= parentWindow.ListSelect;
                    p.AX += parentWindow.ListSelect;
                } else if (parentCardWindow != null) {
                    p.AX -= parentCardWindow.ListSelect;
                    p.AX += parentCardWindow.ListSelect;
                }

                p.HorizontalAlignment = HorizontalAlignment.Stretch;
                if(App.listCreatGroupMember.Contains(p.No)) {
                    p.ChkButton.IsChecked = true;
                    if (mucTableLocal != null && !mucTableLocal.isOwner) {
                        p.ChkButton.IsEnabled = false;
                    } else {
                        p.ChkButton.IsEnabled = true;
                    }
                }
                ListViewLxr.Items.Add(p);
            }
            ListViewLxr.UpdateLayout();
        });
    }


    private MucTable mucTableLocal = null;
    /// <summary>
    /// 切换tab页 需要把已选择的人选择状态
    /// </summary>
    public void ReSelect(MucTable mucTable) {
        if(mucTable!=null) {
            mucTableLocal = mucTable;
        }

        for (int i = 0; i < ListViewLxr.Items.Count; i++) {
            CheckPersonControl control = ListViewLxr.Items[i] as CheckPersonControl;
            if (control == null) continue;
            if (App.listCreatGroupMember.Contains(control.No)) {
                control.ChkButton.IsChecked = true;
                if (mucTable!=null&&!mucTable.isOwner) {
                    control.IsEnabled = false;
                } else {
                    control.IsEnabled = true;
                }
            } else {
                control.ChkButton.IsChecked = false;
            }
        }
    }


    //如果单聊的时候建群 把和谁聊天id传过来
    private string _userId = string.Empty;

    /// <summary>
    /// 建群的时候初始化
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="userId">如果单聊的时候建群 把和谁聊天id传过来</param>
    public void Init(CreateGroup parent,string userId,string tenant) {
        OrganizationTable listOrg=null;
        listOrg = OrganizationServices.getInstance().FindAllOrganizationFromDBByTentant(tenant, "-1");
        if (listOrg == null) return;
        parentWindow = parent;
        _userId = userId;
        LoadOrg(listOrg.organizationId.ToInt());

        LoadLxr(listOrg.organizationId.ToInt(), userId);

        myListBoxItems.Clear();
        OrganizationTable model= OrganizationServices.getInstance().FindOrganizationByOrgId(listOrg.organizationId.ToInt());
        if(model.parentId!="-1"&& !myListBoxItems.Contains(model)) {
            myListBoxItems.Add(model);
        }
        this.Dispatcher.BeginInvoke((Action) delegate {
            ListBar.ItemsSource = MyListBoxItems;
        });

    }

    public void Init(SendCardWindow parent, string tenant) {


        OrganizationTable listOrg = null;
        listOrg = OrganizationServices.getInstance().FindAllOrganizationFromDBByTentant(tenant, "-1");
        if (listOrg == null) return;
        parentCardWindow = parent;
        LoadOrg(listOrg.organizationId.ToInt());

        LoadLxr(listOrg.organizationId.ToInt(),"");

        myListBoxItems.Clear();
        OrganizationTable model = OrganizationServices.getInstance().FindOrganizationByOrgId(listOrg.organizationId.ToInt());
        if (model.parentId != "-1" && !myListBoxItems.Contains(model)) {
            myListBoxItems.Add(model);
        }

        ListBar.ItemsSource = MyListBoxItems;
    }
    private RelayCommand listOrgMouseLeftButtonUpCommand = null;
    public RelayCommand ListOrgMouseLeftButtonUpCommand {
        get {
            if (listOrgMouseLeftButtonUpCommand == null) {
                listOrgMouseLeftButtonUpCommand = new RelayCommand(() => {
                    OrganizationTable table = ListViewOrg.SelectedItem as OrganizationTable;

                    myListBoxItems.Add(table);
                    ListBar.SelectedIndex = ListBar.Items.Count - 1;
                    ListBar.ScrollIntoView(ListBar.SelectedItem);


                    LoadOrg(table.organizationId.ToInt());

                    LoadLxr(table.organizationId.ToInt(),"");
                });
            }
            return listOrgMouseLeftButtonUpCommand;
        }
    }


    private RelayCommand listBarMouseLeftButtonUpCommand = null;
    public RelayCommand ListBarMouseLeftButtonUpCommand {
        get {
            if (listBarMouseLeftButtonUpCommand == null) {
                listBarMouseLeftButtonUpCommand = new RelayCommand(() => {
                    OrganizationTable table = ListBar.SelectedItem as OrganizationTable;

                    for(int i= myListBoxItems.Count-1; i>-1; i--) {
                        if (myListBoxItems[i] != table) {
                            myListBoxItems.Remove(myListBoxItems[i]);
                        } else {
                            break;
                        }
                    }
                    LoadOrg(table.organizationId.ToInt());

                    LoadLxr(table.organizationId.ToInt(),"");
                });
            }
            return listBarMouseLeftButtonUpCommand;
        }
    }

    private bool loadOk = false;
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        //  cmb.SelectedItem = App.CurrentTenantNo;


        //cmb.SelectedValue = App.CurrentTenantNo;
        loadOk = true;
    }



    private void cmb_DropDownClosed(object sender, EventArgs e) {
        if (loadOk == false) return;
        selectTentantNo = cmb.SelectedValue.ToStr();

        if (parentWindow != null) {
            this.Init(parentWindow, _userId, selectTentantNo);
        } else if (parentCardWindow != null) {
            this.Init(parentCardWindow, selectTentantNo);
        }
    }

    private RelayCommand textrMouseLeftButtonUpCommand = null;
    public RelayCommand TextrMouseLeftButtonUpCommand {
        get {
            if (textrMouseLeftButtonUpCommand == null) {
                textrMouseLeftButtonUpCommand = new RelayCommand(() => {
                    cmb_DropDownClosed(null, null);
                });
            }
            return textrMouseLeftButtonUpCommand;
        }
    }
}



}
