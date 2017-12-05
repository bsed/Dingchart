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
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Windows;
using ikvm.extensions;

namespace cn.lds.chatcore.pcw.Views.Control.Message {


/// <summary>
/// ImageTextControl.xaml 的交互逻辑
/// </summary>
public partial class ImageTextControl : MessageBase {
    public ImageTextControl() {
        InitializeComponent();

        //searcListView.ItemsSource = MyListViewItems;
    }
    //public override MessageItem Item {
    //    get;
    //    set;
    //}

    private NewsMessage messageBean = null;
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {


        try {
            messageBean = new NewsMessage().toModel(Item.content);
            if (messageBean.count != 1) {
                GridMain.RowDefinitions[0].Height =new GridLength(0);
                GridMain.RowDefinitions[3].Height = new GridLength(0);
                summary.Visibility = Visibility.Collapsed;
                TitleLb.Visibility = Visibility.Visible;
                TitleGrid.Visibility = Visibility.Visible;
                TitleLb.Visibility = Visibility.Visible;
            } else {
                GridMain.RowDefinitions[0].Height = new GridLength(50);
                GridMain.RowDefinitions[3].Height = new GridLength(30);
                summary.Visibility = Visibility.Visible;
                TitleLb.Visibility = Visibility.Hidden;
                TitleGrid.Visibility = Visibility.Hidden;
                TitleLb.Visibility = Visibility.Hidden;
            }
            List<MultimediaEntry> articles = new List<MultimediaEntry>();
            articles= messageBean.articles;

            for(int i=1; i< articles.Count; i++) {
                articles[i].thumbnail = ImageHelper.getAvatarPath(articles[i].thumbnail);
                myListViewItems.Add(articles[i]);
            }
            TitleDate.Content= DateTimeHelper.getDate(articles[0].timestamp).ToString("yyyy-MM-dd  HH:mm:ss");
            TitleLb.Content = articles[0].title;
            Title.Content = articles[0].title;
            panel.Tag = articles[0].no;
            summary.Content = articles[0].summary;
            ImageHelper.loadAvatar(articles[0].thumbnail, firstImage);
            //日期

            LableD.Content = Item.delayTimeDate.ToString("yyyy-MM-dd  HH:mm:ss");
            searcListView.ItemsSource = MyListViewItems;
        } catch (Exception ex) {
            Log.Error(typeof(ImageTextControl), ex);
        }
    }
    private  ObservableCollection<MultimediaEntry> myListViewItems = new ObservableCollection<MultimediaEntry>();
    public  ObservableCollection<MultimediaEntry> MyListViewItems {
        get {
            return myListViewItems;
        }

        set {
            myListViewItems = value;
        }
    }

    private void Btnxq_Click(object sender, RoutedEventArgs e) {
        clickMenuLeft(Item.user, panel.Tag.ToStr());
    }

    private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        clickMenuLeft(Item.user, panel.Tag.ToStr());
    }


    public void clickMenuLeft(String publicAccountNo, String articleNo) {
        try {
            PublicAccountsTable dt = PublicAccountsService.getInstance().findByAppId(publicAccountNo);

            if (dt == null || dt.status == "0") {
                NotificationHelper.ShowInfoMessage("该公众号不可用");
                return;
            }
            string   tenantNo = dt.tenantNo;
            // 构建URL
            String url = ServiceCode.getPublicAccountArticleUrl(publicAccountNo, articleNo);
            WebPageWindow win = new WebPageWindow();
            win.TenantNo = tenantNo;
            win.Url = url;
            win.Topmost = true;
            win.Show();
        } catch (Exception ex) {
            Log.Error(typeof(ImageTextControl), ex);
        }
    }

    private void searcListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        try {
            if (searcListView.SelectedIndex == -1) return;
            object o = searcListView.SelectedItem;
            if (o == null)
                return;
            MultimediaEntry list = o as MultimediaEntry;
            clickMenuLeft(Item.user, list.no.ToStr());
        } catch (Exception ex) {
            Log.Error(typeof(ImageTextControl), ex);
        }
    }
}
}
