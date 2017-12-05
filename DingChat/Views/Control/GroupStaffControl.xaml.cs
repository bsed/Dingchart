using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
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
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// GroupStaffControl.xaml 的交互逻辑
/// </summary>
public partial class GroupStaffControl : UserControl {


    public GroupStaffControl() {
        InitializeComponent();

    }

    // 变量定义
    private string mucNo;
    ObservableCollection<LVData> LVDatas = new ObservableCollection<LVData>();

    public class LVData {
        public string Name {
            get;
            set;
        }
        public string Pic {
            get;
            set;
        }
    }

    public string MucNo {
        get {
            return mucNo;
        } set {
            mucNo = value;
            Init();
        }
    }

    /// <summary>
    /// TODO：这个咋说
    /// </summary>
    public void DoEvents() {
        try {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(delegate(object f) {
                ((DispatcherFrame)f).Continue = false;

                return null;
            }), frame);
            Dispatcher.PushFrame(frame);
        } catch (Exception ex) {
            Log.Error(typeof(GroupStaffControl), ex);
        }
    }

    /// <summary>
    /// 画面初始化
    /// </summary>
    private void Init() {
        try {
            lock(this) {
                LVDatas.Clear();
                ListBoxMucMembers.ItemsSource = null;

                //GroupCacheEntity group = CacheHelper.getInstance().getGroup(MucNo);
                //IList members = group.getMembers();
                //foreach (object obj in members) {
                //    GroupMemberCacheEntity member = (GroupMemberCacheEntity)obj;
                //    LVData LVData = new LVData();
                //    LVData.Name = member.vcard.nickName;
                //    LVData.Pic = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/images/Avatar_man.png";
                //    LVDatas.Add(LVData);
                //    this.DoEvents();
                //}

                List<MucMembersTable> dt = MucServices.getInstance().findByMucNo(MucNo);
                for (int i = 0; i < dt.Count; i++) {
                    MucMembersTable model = dt[i];
                    LVData LVData = new LVData();
                    string name = string.Empty;
                    if(model.no==App.AccountsModel.no) {
                        name = model.nickname;
                    } else {
                        name = ContactsServices.getInstance().getContractNameByNo(model.no);
                    }

                    LVData.Name = " " + name;
                    VcardsTable vsDt = VcardService.getInstance().findByClientuserId(long.Parse(model.clientuserId));
                    string gender = vsDt.gender;
                    LVData.Pic = ImageHelper.getAvatarPath(vsDt.avatarStorageRecordId);
                    //if (Sex.unknown.ToStr().Equals(gender)) {
                    //    LVData.Pic = ImageHelper.getSysImagePath("Avatar_man.png");
                    //} else if (Sex.female.ToStr().Equals(gender)) {
                    //    LVData.Pic = ImageHelper.getSysImagePath("Avatar_woman.png");
                    //} else {
                    //    LVData.Pic = ImageHelper.getSysImagePath("Avatar_man.png");
                    //}

                    if(!LVDatas.Contains(LVData)) {
                        LVDatas.Add(LVData);
                    }

                    this.DoEvents();
                }

                ListBoxMucMembers.ItemsSource = LVDatas;



                LbCount.Content = "群成员（" + LVDatas.Count + "）";
            }

        } catch (Exception ex) {
            Log.Error(typeof(GroupStaffControl), ex);
        }
    }

    /// <summary>
    /// 窗体加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {


        } catch (Exception ex) {
            Log.Error(typeof(GroupStaffControl), ex);
        }
    }

    /// <summary>
    /// 鼠标进入
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    public void OnMouseEnter(object sender, MouseEventArgs e) {
        try {
            ScrollViewer scroll = CommonMethod.GetChildObject<ScrollViewer>(this.ListBoxMucMembers, "ScrollViewer");
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        } catch (Exception ex) {
            Log.Error(typeof(GroupStaffControl), ex);
        }
    }

    /// <summary>
    /// 鼠标离开
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    public void OnMouseLeave(object sender, MouseEventArgs e) {
        try {
            ScrollViewer scroll = CommonMethod.GetChildObject<ScrollViewer>(this.ListBoxMucMembers, "ScrollViewer");
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //scrollViewer1.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        } catch (Exception ex) {
            Log.Error(typeof(GroupStaffControl), ex);
        }
    }
}
}
