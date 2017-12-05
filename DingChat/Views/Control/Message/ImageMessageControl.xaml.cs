using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using WpfAnimatedGif;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;

namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class ImageMessageControl : MessageBase {
    public ImageMessageControl() {
        InitializeComponent();

    }





    //public override MessageItem Item {
    //    get;
    //    set;
    //}


    public string ContentStr = string.Empty;

    //聊天的时间
    public string DateTime = System.DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");

    public string ImagePath = ImageHelper.getSysImagePath("Default_avatar.jpg");


    private PictureMessage messageBean = null;
    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {

        LayoutRoot.Children.Clear();

        lable = new Label();
        lable.MouseLeftButtonDown += Lable_MouseLeftButtonDown;
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        //lable.MouseRightButtonDown += Lable_MouseRightButtonDown;
        lable.ContextMenu = menuClassify;
        lable.Content = Item.text; ;



        messageBean = new PictureMessage().toModel(Item.content);
        BitmapImage bi = ImageHelper.Base64ToImage(messageBean.thumbnail);
        lable.Loaded += Lable_Loaded;
        if (Left) {
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);



            //图片消息
            Style btn_style = (Style)this.FindResource("LeftImageMessageLableNew");
            lable.HorizontalAlignment = HorizontalAlignment.Left;
            lable.Style = btn_style;
            lable.SetValue(Grid.ColumnProperty, 2);
            lable.SetValue(Grid.RowProperty, 1);
            lable.Margin = new Thickness(10, 10, 0, 0);
            if (bi.Width > bi.Height) {
                if (bi.Width > 230) {
                    lable.Width = 230;
                }
            }

            if (bi.Height > bi.Width) {
                if (bi.Height > 230) {
                    lable.Height = 230;
                }
            }
            lable.Tag = bi;
            LayoutRoot.Children.Add(lable);

            //人名

            if (ChatSessionType == cn.lds.chatcore.pcw.Common.Enums.ChatSessionType.MUC) {
                LayoutRoot.Children.Add(NameLb);
            }




        } else { //显示在右侧的自己的消息
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Auto);





            Style btn_style = (Style)this.FindResource("RightImageMessageLableNew");
            lable.HorizontalAlignment = HorizontalAlignment.Right;
            lable.Style = btn_style;
            lable.Margin = new Thickness(0, 10, 10, 0);
            lable.SetValue(Grid.ColumnProperty, 3);
            lable.SetValue(Grid.RowProperty, 1);
            if (bi.Width > bi.Height) {
                if (bi.Width > 230) {
                    lable.Width = 230;
                }
            }

            if(bi.Height>bi.Width) {
                if (bi.Height > 230) {
                    lable.Height = 230;
                }
            }


            lable.Tag = bi;
            LayoutRoot.Children.Add(lable);





            LayoutRoot.Children.Add(imageStatus);
        }


        LayoutRoot.Children.Add(HeadPortraitEllipse);

        if (Item.showTimestamp) {
            LayoutRoot.RowDefinitions[0].Height = new GridLength(35, GridUnitType.Pixel);
            LayoutRoot.Children.Add(lableD);
        } else {
            LayoutRoot.RowDefinitions[0].Height = new GridLength(20, GridUnitType.Pixel);
        }

    }

    private void Lable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        ImageHelper.ShowPictuerViewer(Item);
    }

    private void Lable_Loaded(object sender, RoutedEventArgs e) {
        Image image = CommonMethod.GetChildObject<Image>(lable, "image");
        if (image == null) return;

        //image.MaxWidth = 230;
        //image.MaxWidth = 230;
        //image.Width = messageBean.getThumbnailWidth();
        //image.Height = messageBean.getThumbnailHeight();
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
