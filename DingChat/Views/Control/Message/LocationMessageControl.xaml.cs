using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Emoji;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Views.Windows;
using EventBus;
using WpfAnimatedGif;


namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class LocationMessageControl : MessageBase {
    public LocationMessageControl() {
        InitializeComponent();

    }



    private LocationMessage messageBean = null;
    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {

        LayoutRoot.Children.Clear();

        messageBean = new LocationMessage().toModel(Item.content);

        BitmapImage bi = ImageHelper.Base64ToImage(messageBean.picture);
        messageBean.BitmapPicture = bi;
        this.HorizontalAlignment = HorizontalAlignment.Stretch;


        //聊天lable
        lable = new Label();
        lable.DataContext = messageBean;
        lable.MouseLeftButtonDown -= lable_MouseLeftButtonDown;
        lable.MouseLeftButtonDown += lable_MouseLeftButtonDown;
        lable.ContextMenu = menuClassify;
        //lable.MouseRightButtonDown += Lable_MouseRightButtonDown;
        lable.Loaded -= lable_Loaded;
        lable.Loaded += lable_Loaded;


        if (Left) {
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);


            Style btn_style = (Style)this.FindResource("LocationMessage");
            lable.HorizontalAlignment = HorizontalAlignment.Left;
            Thickness margin = (Thickness)this.FindResource("LeftMessageMargin");
            lable.Margin = margin;
            lable.Style = btn_style;
            lable.Tag = "left";
            lable.SetValue(Grid.ColumnProperty, 2);
            lable.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(lable);

            //人名
            if (ChatSessionType == cn.lds.chatcore.pcw.Common.Enums.ChatSessionType.MUC) {
                LayoutRoot.Children.Add(NameLb);
            }


        } else { //显示在右侧的自己的消息
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Auto);


            Style btn_style = (Style)this.FindResource("LocationMessage");
            lable.HorizontalAlignment = HorizontalAlignment.Right;
            Thickness margin = (Thickness)this.FindResource("RightMessageMargin");
            lable.Margin = margin;
            lable.Tag = "right";
            lable.Style = btn_style;
            lable.SetValue(Grid.ColumnProperty, 3);
            lable.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(lable);


            LayoutRoot.Children.Add(imageStatus);
        }

        LayoutRoot.Children.Add(HeadPortraitEllipse);

        //日期
        if (Item.showTimestamp) {
            LayoutRoot.RowDefinitions[0].Height = new GridLength(35, GridUnitType.Pixel);
            LayoutRoot.Children.Add(lableD);
        } else {
            LayoutRoot.RowDefinitions[0].Height = new GridLength(20, GridUnitType.Pixel);
        }


    }



    private void lable_Loaded(object sender, RoutedEventArgs e) {



        TextBlock txtBlockLocation = CommonMethod.GetChildObject<TextBlock>(sender as Label, "location");

        if (txtBlockLocation != null) {
            txtBlockLocation.ContextMenu = menuClassify;
        }
    }



    private void lable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

        LocationEventData locationEventData = new LocationEventData();
        locationEventData.address = ((LocationMessage)messageBean).address;
        locationEventData.latitude = ((LocationMessage)messageBean).latitude;
        locationEventData.longitude = ((LocationMessage)messageBean).longitude;
        locationEventData.scale = ((LocationMessage)messageBean).scale;

        //发送位置通知
        BusinessEvent<object> Businessdata = new BusinessEvent<object>();
        Businessdata.data = locationEventData;
        Businessdata.eventDataType = BusinessEventDataType.ClickLocation;
        EventBusHelper.getInstance().fireEvent(Businessdata);

    }





    /// <summary>
    /// 滑轮滚动（解决子控件滑轮不滚动问题）
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void LayoutRoot_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e) {
        var scroll =
            CommonMethod.GetVisualChild<ScrollViewer>(((this.Parent as ListView).Parent as ScrollViewer));
        if (scroll != null) {
            //指定父级ScrollViewer滚动偏移量
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta);
        }
    }






}
}
