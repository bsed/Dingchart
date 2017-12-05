using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Views.Windows;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// PersonGroupControl.xaml 的交互逻辑
/// </summary>
public partial class PersonGroupControl : UserControl {


    public PersonGroupControl() {
        InitializeComponent();
        this.DataContext = this;
    }

    // 变量定义
    //PersonControl person = new PersonControl();

    public bool FindAllMember = false;
    private PersonDetailed personWindow = PersonDetailed.getInstance();

    private ObservableCollection<ChatSessionBean> myListViewItems = new ObservableCollection<ChatSessionBean>();
    public ObservableCollection<ChatSessionBean> MyListViewItems {
        get {
            return myListViewItems;
        } set {
            myListViewItems = value;
        }
    }
    private string userNo;
    public string UserNo {
        get {
            return userNo;
        } set {
            userNo = value;

            try {
                VcardsTable dt = VcardService.getInstance().findByNo(userNo);
                if (dt == null) return;
                myListViewItems.Clear();

                ChatSessionBean bean = new ChatSessionBean();
                bean.Account = dt.clientuserId;
                bean.Name = ContactsServices.getInstance().getContractNameByNo(dt.no);
                bean.AvatarStorageRecordId = dt.avatarStorageRecordId;
                bean.AvatarPath = ImageHelper.loadAvatarPath(dt.avatarStorageRecordId);

                myListViewItems.Add(bean);



                if (FindAllMember == false) {
                    ChatSessionBean beanAdd = new ChatSessionBean();
                    beanAdd.Account = "-1";
                    beanAdd.Name = string.Empty;
                    beanAdd.AvatarPath = ImageHelper.loadSysImageBrush("Add_big.png");
                    myListViewItems.Add(beanAdd);
                }
            } catch (Exception ex) {
                Log.Error(typeof(PersonGroupControl), ex);
            }
        }
    }


    private string mucNo;

    public string MucNo {
        get {
            return mucNo;
        } set {
            mucNo = value;
            InitMuc();
        }
    }
    private void   InitMuc() {
        try {
            bool addPerson = false;

            List<MucMembersTable> dt = MucServices.getInstance().findByMucNo(MucNo);

            this.Dispatcher.BeginInvoke((Action)delegate () {

                myListViewItems.Clear();
                if (dt.Count == 0) return;
                int i = 0;
                foreach (MucMembersTable model in dt) {
                    if (FindAllMember == false && i == 13) {
                        ChatSessionBean beanAdd = new ChatSessionBean();
                        beanAdd.Account = "-1";
                        beanAdd.Name = string.Empty;
                        beanAdd.AvatarPath = ImageHelper.loadSysImageBrush("Add_big.png");
                        myListViewItems.Add(beanAdd);
                        return;
                    }

                    ChatSessionBean bean = new ChatSessionBean();
                    bean.Account = model.clientuserId;
                    string name = string.Empty;
                    if (model.no == App.AccountsModel.no) {
                        name = model.nickname;
                    } else {
                        name = ContactsServices.getInstance().getContractNameByNo(model.no);
                    }
                    bean.Name = name;
                    bean.AvatarStorageRecordId = model.avatarStorageRecordId;
                    bean.AvatarPath = ImageHelper.loadAvatarPath(model.avatarStorageRecordId);

                    myListViewItems.Add(bean);
                    i++;
                    //this.DoEvents();
                }
                if (addPerson == false && FindAllMember == false) {
                    ChatSessionBean beanAdd = new ChatSessionBean();
                    beanAdd.Account = "-1";
                    beanAdd.Name = string.Empty;
                    beanAdd.AvatarPath = ImageHelper.loadSysImageBrush("Add_big.png");
                    myListViewItems.Add(beanAdd);
                    addPerson = true;
                }
            });
        } catch (Exception ex) {
            Log.Error(typeof(PersonGroupControl), ex);
        }
    }
    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {


        } catch (Exception ex) {
            Log.Error(typeof(PersonGroupControl), ex);
        }
    }

    /// <summary>
    /// 人员点击
    /// </summary>
    /// <param Name="userId"></param>
    void person_click(string userId) {
        try {
            if (personWindow.IsVisible == true) {
                personWindow.Hide();
            }
            //弹出加人窗口
            if (userId == "-1") {


                CreateGroup group = new CreateGroup();
                group.MucNo = mucNo;
                if (!string.IsNullOrEmpty(userNo)) {
                    VcardsTable dt = VcardService.getInstance().findByNo(userNo);
                    if (dt == null) return;
                    group.UserId = dt.clientuserId;
                }

                group.ShowDialog();
                return;
            }
            personWindow = PersonDetailed.getInstance();
            //PersonDetailed WinObj = PersonDetailed.getInstance();
            //WinObj =  PersonDetailed.getInstance();
            personWindow.Topmost = true;
            personWindow.Id = userId.ToInt();
            personWindow.ShowDialog();
            personWindow.Activate();
        } catch (Exception ex) {
            Log.Error(typeof(PersonGroupControl), ex);
        }
    }


    public void DoEvents() {
        try {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(delegate(object f) {
                ((DispatcherFrame)f).Continue = false;

                return null;
            }
                                                   ), frame);
            Dispatcher.PushFrame(frame);
        } catch (Exception ex) {
            Log.Error(typeof(PersonGroupControl), ex);
        }
    }

    /// <summary>
    /// 切换群成员
    /// </summary>
    /// <param Name="model"></param>
    public void Change(MucMembersTable model) {
        try {
            //InitMuc();
            for (int i = 0; i < myListViewItems.Count; i++) {
                ChatSessionBean c = myListViewItems[i] as ChatSessionBean;
                if (c.Account == model.clientuserId) {
                    string name = model.nickname;

                    c.Name = name;

                    //DoEvents();
                    return;
                }
            }

        } catch (Exception ex) {
            Log.Error(typeof(PersonGroupControl), ex);
        }

    }

    private void ListBoxMembers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        object o = ListBoxMembers.SelectedItem;
        if (o == null)
            return;

        ChatSessionBean p = o as ChatSessionBean;
        person_click(p.Account);
    }
}

}

