using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// 联系人控件
/// </summary>
public partial class AppControl : UserControl {

    public AppControl() {
        InitializeComponent();
    }

    // 变量定义
    private bool isChecked;

    public bool IsChecked {
        get {
            return isChecked;
        } set {
            isChecked = value;
            try {
                if (isChecked) {
                    SolidColorBrush color = (SolidColorBrush)this.FindResource("rowClickBackground");
                    this.Background = color;
                } else {
                    SolidColorBrush color = (SolidColorBrush)this.FindResource("leftBackground");
                    this.Background = color;
                }
            } catch (Exception ex) {
                Log.Error(typeof(AppControl), ex);
            }
        }
    }



    public string HeadPortrait =App.ImagePathDefault;

    public string Title = string.Empty;

    private string _appId;

    public string AppId {
        get {
            return _appId;
        } set {
            _appId = value;
            Init();
        }
    }


    public string Url;
    /// <summary>
    /// 画面初始化
    /// </summary>
    private void Init() {
        try {
            if (_appId != string.Empty) {

                PublicWebTable model = PublicWebService.getInstance().FindByAppId(_appId);
                if (model != null) {
                    HeadPortrait = model.logoId;
                    Title = model.name.ToString();
                    Url = model.url;
                }
            }

            ImageHelper.loadAvatar(HeadPortrait, Ico);

            LbTitle.Content = Title;

        } catch (Exception ex) {
            Log.Error(typeof(AppControl), ex);
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
            Log.Error(typeof(AppControl), ex);
        }
    }

    private void GridContact_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) {
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = Url;
        Businessdata.eventDataType = BusinessEventDataType.SelectApp;
        EventBusHelper.getInstance().fireEvent(Businessdata);
    }



}
}
