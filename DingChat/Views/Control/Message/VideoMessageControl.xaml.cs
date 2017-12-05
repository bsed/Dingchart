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
using cn.lds.chatcore.pcw.Common.MediaHelper;
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
using CircularProgressBar;
using EventBus;
using WpfAnimatedGif;


namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class VideoMessageControl : MessageBase {
    public VideoMessageControl() {
        InitializeComponent();
        dataModel = new MainViewModel();
        this.DataContext = dataModel;
    }


    //播放按钮
    public  Image imagePlay = new Image();
    private VideoMessage messageBean = null;
    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {

        LayoutRoot.Children.Clear();


        messageBean = new VideoMessage().toModel(Item.content);
        this.HorizontalAlignment = HorizontalAlignment.Stretch;

        BitmapImage bi = ImageHelper.Base64ToImage(messageBean.firstFrame);
        messageBean.firstFrameImage = bi;

        double len = Math.Ceiling((double)messageBean.duration / 1000);
        if (len > 60) {
            len = 60;
        }
        messageBean.videoLength = len + "'";
        //聊天lable
        lable = new Label();
        lable.DataContext = messageBean;
        lable.MouseLeftButtonDown -= lable_MouseLeftButtonDown;
        lable.MouseLeftButtonDown += lable_MouseLeftButtonDown;

        lable.ContextMenu = menuClassify;
        lable.Loaded -= lable_Loaded;
        lable.Loaded += lable_Loaded;

        //播放按钮
        imagePlay.MouseLeftButtonDown += lable_MouseLeftButtonDown;
        imagePlay.ContextMenu = menuClassify;
        imagePlay.Cursor = System.Windows.Input.Cursors.Hand;
        imagePlay.HorizontalAlignment = HorizontalAlignment.Center;
        imagePlay.VerticalAlignment = VerticalAlignment.Center;
        ImageHelper.loadSysImage("Play.png", imagePlay);
        if (Left) {
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);


            Style btn_style = (Style)this.FindResource("VideoMessageLable");
            lable.HorizontalAlignment = HorizontalAlignment.Left;
            Thickness margin = (Thickness)this.FindResource("LeftMessageMargin");
            lable.Margin = margin;
            lable.Style = btn_style;
            lable.Tag = "left";
            lable.SetValue(Grid.ColumnProperty, 2);
            lable.SetValue(Grid.RowProperty, 1);

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
            LayoutRoot.Children.Add(lable);

            //人名
            if (ChatSessionType == cn.lds.chatcore.pcw.Common.Enums.ChatSessionType.MUC) {
                LayoutRoot.Children.Add(NameLb);
            }

            imagePlay.Margin = margin;
            imagePlay.SetValue(Grid.ColumnProperty, 2);
            imagePlay.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(imagePlay);

        } else { //显示在右侧的自己的消息
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Auto);


            Style btn_style = (Style)this.FindResource("VideoMessageLable");
            lable.HorizontalAlignment = HorizontalAlignment.Right;
            Thickness margin = (Thickness)this.FindResource("RightMessageMargin");
            lable.Margin = margin;
            lable.Tag = "right";
            lable.Style = btn_style;
            lable.SetValue(Grid.ColumnProperty, 3);
            lable.SetValue(Grid.RowProperty, 1);

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


            LayoutRoot.Children.Add(lable);


            LayoutRoot.Children.Add(imageStatus);

            imagePlay.Margin = margin;
            imagePlay.SetValue(Grid.ColumnProperty, 3);
            imagePlay.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(imagePlay);
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

        //TextBox txtBlock = CommonMethod.GetChildObject<TextBox>(sender as Label, "textBox");

        //if (txtBlock != null) {
        //    txtBlock.ContextMenu = menuClassify;
        //}

    }

    private void lable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        FilesTable dt = null;
        if (string.IsNullOrEmpty(messageBean.videoStorageId)==false) {
            dt = FilesService.getInstance().getFile(messageBean.videoStorageId);
        } else {
            dt = FilesService.getInstance().getFileByOwner(Item.messageId);
        }
        if (dt != null) {
            VideoHelper.StartPlayAVideo(dt);
        }

    }
    private MainViewModel dataModel = null;
    private long lastpersent = 0;
    public void UpdateProcess(int persent) {
        if (lastpersent > persent) return;
        if (Left) {
            imagePlay.Visibility = Visibility.Collapsed;
            bProcess.Visibility = Visibility.Visible;
            this.dataModel.SetProcessValue(persent);
        } else {
            imagePlay.Visibility = Visibility.Collapsed;
            bProcessR.Visibility = Visibility.Visible;
            this.dataModel.SetProcessValue(persent);
        }
        lastpersent = persent;
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
