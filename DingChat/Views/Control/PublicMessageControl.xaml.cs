using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Beans.Convertors;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Windows;
using ikvm.extensions;
using Newtonsoft.Json;
using Button = System.Windows.Controls.Button;
using ContextMenu = System.Windows.Controls.ContextMenu;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MenuItem = System.Windows.Controls.MenuItem;
using UserControl = System.Windows.Controls.UserControl;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// PublicMessageControl.xaml 的交互逻辑
/// </summary>
public partial class PublicMessageControl : UserControl {
    public PublicMessageControl() {
        InitializeComponent();
    }
    public ContextMenu menuClassify = new ContextMenu();
    private string appId = string.Empty;
    private Grid grid = null;
    private List<PublicMenus> list;
    public event Action<bool> ClickBtnUnfold;
    public string AppId {
        get {
            return appId;
        } set {
            appId = value;
            Init();
        }
    }

    private string tenantNo = string.Empty;
    private void Init() {
        try {
            panel.Children.Remove(grid);
            grid = new Grid();
            grid.Margin = new Thickness(10, 0, 0, 0);

            panel.Children.Add(grid);
            PublicAccountsTable dt = PublicAccountsService.getInstance().findByAppId(AppId);

            if (dt == null) return;
            tenantNo = dt.tenantNo;
            string menu = dt.menu;
            if (menu=="null" || menu.ToStr() == string.Empty) return;
            list = JsonConvert.DeserializeObject<List<PublicMenus>>(menu, new convertor<PublicMenus>());
            for (int i = 0; i < list.Count; i++) {
                PublicMenus publicMenus = list[i];
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                Button btn = new Button();
                btn.Click += Btn_Click;
                Style btn_style = (Style)this.FindResource("TextButton");
                btn.Content = publicMenus.name;
                btn.Tag = publicMenus;
                btn.BorderThickness = new Thickness(0);
                btn.Background = new SolidColorBrush(Colors.White);



                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.Style = btn_style;
                //btn.Width = this.ActualWidth/list.Count;
                btn.SetValue(Grid.ColumnProperty, i );
                btn.SetValue(Grid.RowProperty, 0);
                btn.Content = publicMenus.name;
                grid.Children.Add(btn);


                GridSplitter gs = new GridSplitter();
                gs.ShowsPreview = true;
                gs.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
                gs.HorizontalAlignment = HorizontalAlignment.Left;
                gs.VerticalAlignment = VerticalAlignment.Stretch;
                gs.Width = 1;
                gs.SetValue(Grid.ColumnProperty, i);
                gs.SetValue(Grid.RowProperty, 0);
                grid.Children.Add(gs);

                if(this.ActualWidth!=0) {
                    CreatViewSize();
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }
    }

    public void clickMenuLeft(String appid, PublicMenus listmenu) {
        try {

            PublicAccountsTable dt = PublicAccountsService.getInstance().findByAppId(appid);

            if (dt == null || dt.status == "0") {
                NotificationHelper.ShowInfoMessage("该公众号不可用");
                return;
            }
            if ("click".Equals(listmenu.type)) {
                ContactsApi.clickSubscriptionMenu(appid, listmenu.code, tenantNo);
            } else if ("view".Equals(listmenu.type)) {
                // 构建URL
                String url = ServiceCode.clickSubscriptionMenu;
                url = url.replace("{appId}", appid);
                url = url.replace("{menuCode}", listmenu.code);

                WebPageWindow win = new WebPageWindow();
                win.TenantNo = tenantNo;
                win.Url = url;
                win.Show();

            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }
    }

    /// <summary>
    /// 点击菜单
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Btn_Click(object sender, RoutedEventArgs e) {
        try {
            Button btn = sender as Button;
            PublicMenus publicMenus = btn.Tag as PublicMenus;
            List<PublicMenus> publicMenusList = publicMenus.children;
            if(publicMenusList!=null) {
                CreatContextMenu(publicMenusList);
                menuClassify.PlacementTarget = btn;
                menuClassify.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                menuClassify.Width = btn.ActualWidth - 26;
                menuClassify.VerticalOffset = -5;
                menuClassify.HorizontalOffset = 13;
                menuClassify.HorizontalAlignment = HorizontalAlignment.Center;
                menuClassify.IsOpen = true;
            } else {
                clickMenuLeft(AppId, publicMenus);
            }

        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }

    }
    /// <summary>
    /// 加载分组右键菜单
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    public virtual void CreatContextMenu(List<PublicMenus> publicMenusList) {
        try {
            Style styleClassify = this.FindResource("PublicContextMenu") as Style;
            menuClassify.Style = styleClassify;
            menuClassify.Items.Clear();

            for(int i=0; i< publicMenusList.Count; i++) {
                if(i!=0) {
                    Separator separator = new Separator();
                    separator.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
                    separator.HorizontalAlignment = HorizontalAlignment.Stretch;
                    separator.Margin = new Thickness(-9, 0, 9, 0);
                    menuClassify.Items.Add(separator);
                }
                MenuItem menuItem = new MenuItem();

                menuItem.Header = publicMenusList[i].name;
                Style styleMenu = this.FindResource("PublicMenuItem") as Style;
                menuItem.Style = styleMenu;
                menuItem.Click += MenuItem_Click; ;
                menuItem.Tag = publicMenusList[i];
                menuClassify.Items.Add(menuItem);
            }


        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e) {
        try {
            MenuItem btn = sender as MenuItem;
            PublicMenus publicMenus = btn.Tag as PublicMenus;
            clickMenuLeft(AppId, publicMenus);

        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }
    }

    public void CreatViewSize() {
        try {
            if (this.ActualWidth == 0 || this.Visibility != Visibility.Visible) return;
            for (int i = 0; i < grid.ColumnDefinitions.Count; i++) {
                grid.ColumnDefinitions[i].Width = new GridLength((this.ActualWidth - 30) / list.Count, GridUnitType.Pixel);
            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        CreatViewSize();
    }

    private void BtnCollapsed_Click(object sender, RoutedEventArgs e) {
        try {
            if (ClickBtnUnfold != null) {
                ClickBtnUnfold.Invoke(true);
            }
        } catch (Exception ex) {
            Log.Error(typeof(PublicMessageControl), ex);
        }
    }
}
}
