using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Page;
using CSCore.SoundOut;
using EventBus;
using WpfAnimatedGif;


namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class VoiceMessageControl : MessageBase {
    public VoiceMessageControl() {
        InitializeComponent();

    }

    private VoiceMessage messageBean = null;


    Image UnreadImage = null;


    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {

        LayoutRoot.Children.Clear();

        this.HorizontalAlignment = HorizontalAlignment.Stretch;


        //聊天lable
        lable = new Label();
        lable.MinWidth = 60;
        lable.MaxWidth = 250;
        lable.Content = Item.text;
        lable.ContextMenu = menuClassify;
        lable.MouseLeftButtonDown -= LableVoice_MouseLeftButtonDown;
        lable.MouseLeftButtonDown+= LableVoice_MouseLeftButtonDown;


        //未读消息标志
        UnreadImage = new Image();
        ImageHelper.loadSysImage("Message_count1.png",UnreadImage);
        UnreadImage.VerticalAlignment = VerticalAlignment.Top;
        UnreadImage.Height = 8;

        //语音时长
        Label LengthLb = new Label();
        LengthLb.Content = Item.name;
        LengthLb.FontSize = 12;
        LengthLb.Foreground = (SolidColorBrush)this.FindResource("foreground99");
        LengthLb.VerticalAlignment = VerticalAlignment.Center;
        LengthLb.Width = Double.NaN;
        if (Left) {
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);

            Style btn_style = (Style)this.FindResource("VoiceMessageLeft");
            lable.HorizontalAlignment = HorizontalAlignment.Left;
            lable.Style = btn_style;
            lable.SetValue(Grid.ColumnProperty, 2);
            lable.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(lable);

            //人名
            if (ChatSessionType == cn.lds.chatcore.pcw.Common.Enums.ChatSessionType.MUC) {
                LayoutRoot.Children.Add(NameLb);
            }


            //未读消息
            UnreadImage.HorizontalAlignment = HorizontalAlignment.Left;
            UnreadImage.SetValue(Grid.ColumnProperty, 3);
            UnreadImage.SetValue(Grid.RowProperty, 1);
            UnreadImage.Margin = new Thickness(7, 5, 0, 0);

            //语音时长
            LengthLb.SetValue(Grid.ColumnProperty, 3);
            LengthLb.SetValue(Grid.RowProperty, 1);
            LengthLb.HorizontalAlignment = HorizontalAlignment.Left;
            LengthLb.Margin = new Thickness(2, 4, 0, 0);
        } else {
            //显示在右侧的自己的消息
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Auto);


            Style btn_style = (Style)this.FindResource("VoiceMessageRight");
            lable.HorizontalAlignment = HorizontalAlignment.Right;
            lable.Style = btn_style;
            lable.SetValue(Grid.ColumnProperty, 3);
            lable.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(lable);



            //未读消息
            UnreadImage.HorizontalAlignment = HorizontalAlignment.Right;
            UnreadImage.SetValue(Grid.ColumnProperty, 2);
            UnreadImage.SetValue(Grid.RowProperty, 1);
            UnreadImage.Margin = new Thickness(0, 5, 7, 0);
            Item.flag = true;

            //语音时长
            LengthLb.SetValue(Grid.ColumnProperty, 2);
            LengthLb.SetValue(Grid.RowProperty, 1);
            LengthLb.HorizontalAlignment = HorizontalAlignment.Right;
            LengthLb.Margin = new Thickness(0, 4, 2, 0);
        }

        //未读消息标志
        LayoutRoot.Children.Add(UnreadImage);
        if (Item.flag) {
            UnreadImage.Visibility = Visibility.Hidden;
        } else {
            UnreadImage.Visibility = Visibility.Visible;
        }

        //语音时长
        LayoutRoot.Children.Add(LengthLb);



        LayoutRoot.Children.Add(HeadPortraitEllipse);
        //日期
        if (Item.showTimestamp) {
            LayoutRoot.RowDefinitions[0].Height = new GridLength(35, GridUnitType.Pixel);
            LayoutRoot.Children.Add(lableD);
        } else {
            LayoutRoot.RowDefinitions[0].Height = new GridLength(20, GridUnitType.Pixel);
        }

        messageBean = new VoiceMessage().toModel(Item.content);
        lable.Width = messageBean.duration / 240;

        double len = Math.Ceiling((double) messageBean.duration/1000);
        if (len > 60) {
            len = 60;
        }
        LengthLb.Content = len + "'";

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

    public void PlayStopped() {
        Image     imageControl =
            CommonMethod.GetVisualChild<Image>(lable);
        var controller = ImageBehavior.GetAnimationController(imageControl);
        controller.Pause();
        //转到最后一帧
        controller.GotoFrame(0);
    }

    public void PlayStart() {
        if(PlaybackState.Playing == VoiceHelper.GetPlayState()) {
            VoiceHelper.StopPlayASound(true);
            if (App.soundPlayingId == Item.messageId) {
                return;
            }
        }


        Image imageControl =
            CommonMethod.GetVisualChild<Image>(lable);

        // 如果本地存在文件，则不执行下载
        if (!FilesService.getInstance().existFile(messageBean.voiceStorageId)) {
            Dictionary<String, Object> extras = new Dictionary<String, Object>();
            extras.Add("voiceMessage", Item.content);
            extras.Add("messageId", Item.messageId);
            DownloadServices.getInstance().DownloadMethod(messageBean.voiceStorageId, DownloadType.MSG_VOICE, extras);
        }


        var controller = ImageBehavior.GetAnimationController(imageControl);
        controller.Play();

        VoiceHelper.StartPlayASound(messageBean.voiceStorageId, Item.messageId, null);
        App.soundPlayingId = Item.messageId;
        MessageService.getInstance().updateFlagStatus(Item.messageId,true);
        UnreadImage.Visibility = Visibility.Hidden;
    }
    private void LableVoice_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

        PlayStart();
    }

    private void VoiceMessageControl_OnUnloaded(object sender, RoutedEventArgs e) {
        VoiceHelper.StopPlayASound(false);
    }
}
}

