using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Windows;
using WpfAnimatedGif;
using Clipboard = System.Windows.Clipboard;
using ContextMenu = System.Windows.Controls.ContextMenu;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace cn.lds.chatcore.pcw.Views.Control.Message {
public class MessageBase : UserControl {
    //聊天人名
    public TextBlock NameLb = new TextBlock();

    //聊天时间
    public Label lableD = new Label();
    //头像
    public   System.Windows.Shapes.Ellipse HeadPortraitEllipse = new System.Windows.Shapes.Ellipse();
    //聊天lable
    public  System.Windows.Controls.Label lable = new System.Windows.Controls.Label();

    public ChatSessionType ChatSessionType;

    //添加事件代理 多选
    public event EventHandler DxClick;
    public string HeadPortrait = ImageHelper.getSysImagePath("Default_avatar.jpg");


    //左侧或者右侧 区分是自己说的话 还是对方
    public bool Left = true;

    private MessageItem item;
    public  MessageItem Item {
        get {
            return item;
        } set {
            item = value;
            Init();
        }
    }
    public   ContextMenu menuClassify = new ContextMenu();


    public Image imageStatus = new Image();

    private void Init() {
        UserAndPostion user = GetUserAndPostion.Get(Item);
        if (user.Postion == MessagePostionType.Left) {
            Left = true;
        } else if (user.Postion == MessagePostionType.Right) {
            Left = false;
        }


        ImageBrush imageBrush = new ImageBrush();
        HeadPortraitEllipse.Cursor = System.Windows.Input.Cursors.Hand;
        HeadPortraitEllipse.Height = 40;
        HeadPortraitEllipse.Width = 40;
        HeadPortraitEllipse.Margin = new Thickness(0, 0, 0, 0);
        HeadPortraitEllipse.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
        imageBrush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(ImageHelper.getSysImagePath("Default_avatar.jpg"), UriKind.RelativeOrAbsolute));
        HeadPortraitEllipse.Fill = imageBrush;


        HeadPortraitEllipse.VerticalAlignment = VerticalAlignment.Top;
        if(Left) {
            if (ChatSessionType == cn.lds.chatcore.pcw.Common.Enums.ChatSessionType.PUBLIC) {
                PublicAccountsTable dt = PublicAccountsService.getInstance().findByAppId(Item.user);
                HeadPortrait = dt.logoId;
            } else {
                VcardsTable dt = VcardService.getInstance().findByNo(Item.user);
                HeadPortrait = Item.avatar;
                if (HeadPortrait == string.Empty  && dt!=null) {
                    HeadPortrait = dt.avatarStorageRecordId;
                }
            }
            if (Item.user == Constants.SYSTEM_NOTICE_FLAG) {
                HeadPortrait = @"Notice.png";
                ImageHelper.loadSysImageBrush(HeadPortrait, imageBrush);
            } else if (Item.user == App.AccountsModel.no) {
                //HeadPortrait = @"FileSend.png";
                //ImageHelper.loadSysImageBrush(HeadPortrait, imageBrush);
                HeadPortrait = App.AccountsModel.avatarStorageRecordId.ToStr();
                ImageHelper.loadAvatarImageBrush(HeadPortrait, imageBrush);
            } else {
                ImageHelper.loadAvatarImageBrush(HeadPortrait, imageBrush);
            }
            HeadPortraitEllipse.HorizontalAlignment = HorizontalAlignment.Right;
            HeadPortraitEllipse.SetValue(Grid.ColumnProperty, 1);
            HeadPortraitEllipse.SetValue(Grid.RowProperty, 1);
        } else {
            //查询我的头像
            HeadPortrait = App.AccountsModel.avatarStorageRecordId.ToStr();
            ImageHelper.loadAvatarImageBrush(HeadPortrait, imageBrush);
            HeadPortraitEllipse.HorizontalAlignment = HorizontalAlignment.Left;
            HeadPortraitEllipse.SetValue(Grid.ColumnProperty, 4);
            HeadPortraitEllipse.SetValue(Grid.RowProperty, 1);
        }



        lableD.FontSize = 12;
        SolidColorBrush color = (SolidColorBrush)this.FindResource("foreground99");
        lableD.Foreground = color;
        //lableD.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
        lableD.SetValue(Grid.ColumnSpanProperty, 6);
        lableD.SetValue(Grid.RowProperty, 0);
        lableD.SetValue(Grid.ColumnProperty, 0);
        lableD.Content = Item.delayTimeDate.ToString("yyyy-MM-dd  HH:mm:ss");
        lableD.VerticalAlignment = VerticalAlignment.Top;
        lableD.HorizontalContentAlignment = HorizontalAlignment.Center;
        lableD.VerticalContentAlignment = VerticalAlignment.Top;

        //聊天人名
        if (Item.name.ToStr() == string.Empty) {
            Item.name = ContactsServices.getInstance().getContractNameByNo(Item.user);
        }
        NameLb.Text = Item.name;
        NameLb.Height = Double.NaN;
        NameLb.FontFamily = new FontFamily("Microsoft YaHei");
        NameLb.FontSize = 10;
        //NameLb.SetValue(Grid.RowSpanProperty, 2);
        NameLb.Foreground = (SolidColorBrush)this.FindResource("foreground99");
        NameLb.Margin = new Thickness(10, 0, 0, 0);
        //NameLb.Background = Brushes.Red;
        NameLb.SetValue(Grid.ColumnProperty, 2);
        NameLb.SetValue(Grid.RowProperty, 0);
        NameLb.VerticalAlignment = VerticalAlignment.Bottom;
        NameLb.HorizontalAlignment = HorizontalAlignment.Left;

        //状态按钮
        //imageStatus = new Image();
        imageStatus.Margin = new Thickness(0, 0, 2, 0);
        imageStatus.VerticalAlignment = VerticalAlignment.Center;
        imageStatus.HorizontalAlignment = HorizontalAlignment.Right;
        imageStatus.SetValue(Grid.ColumnProperty, 2);
        imageStatus.SetValue(Grid.RowProperty, 1);


        //发送状态
        imageStatus.Stretch = Stretch.None;
        imageStatus.Cursor = System.Windows.Input.Cursors.Hand;
        imageStatus.MouseLeftButtonDown += imageStatus_MouseLeftButtonDown;
        imageStatus.Width = 15;
        imageStatus.Height = 15;
        imageStatus.Stretch = Stretch.Fill;
        imageStatus.Margin = new Thickness(0, 5, 5, 0);
        if (Item.sent == "0" || Item.sent == string.Empty) {
            imageStatus.Visibility = Visibility.Visible;
            //imageStatus.Source = new BitmapImage(new Uri(ImageHelper.getSysImagePath("Message/Sending.png"),UriKind.RelativeOrAbsolute));

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(ImageHelper.getSysImagePath("Message/Sending.gif"));
            image.EndInit();
            ImageBehavior.SetAnimatedSource(imageStatus, image);

        } else if (Item.sent == "1") {
            imageStatus.Visibility = Visibility.Collapsed;
        } else if (Item.sent == "-1") {
            imageStatus.Visibility = Visibility.Visible;
            imageStatus.Source = new BitmapImage(new Uri(ImageHelper.getSysImagePath("Message/Send_error.png"), UriKind.RelativeOrAbsolute));

        }
        CreatContextMenu();


    }




    //重新发送
    public void imageStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

        if (Item.sent != "-1") return;
        MessageService.getInstance().ReSendMessage(Item);

        imageStatus.Visibility = Visibility.Visible;
        var image = new BitmapImage();
        image.BeginInit();
        image.UriSource = new Uri(ImageHelper.getSysImagePath("Message/Sending.gif"));
        image.EndInit();
        ImageBehavior.SetAnimatedSource(imageStatus, image);

    }

    /// <summary>
    /// 加载分组右键菜单
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    public virtual void CreatContextMenu() {
        Style styleMenu = this.FindResource("MenuItem") as Style;
        MenuItem menuItemZF = new MenuItem();
        MenuItem menuItemSC = new MenuItem();
        MenuItem menuItemDel = new MenuItem();
        MenuItem menuItemBack = new MenuItem();
        MenuItem menuItemDX = new MenuItem();
        MenuItem menuItemFZ = new MenuItem();
        MenuItem menuItemSave = new MenuItem();
        menuItemZF.Style = styleMenu;
        menuItemSC.Style = styleMenu;
        menuItemDel.Style = styleMenu;
        menuItemBack.Style = styleMenu;
        menuItemDX.Style = styleMenu;
        menuItemFZ.Style = styleMenu;
        menuItemSave.Style = styleMenu;

        menuClassify.Items.Clear();
        //Style styleMenu = this.FindResource("ContextMenu") as Style;
        Style style = this.FindResource("PublicContextMenu") as Style;
        menuClassify.Style = style;

        menuItemFZ.Header = "复制";
        menuItemFZ.Name = "copy";
        menuItemFZ.Click += menuItemFZClick;
        menuItemFZ.Tag = "";

        menuItemZF.Header = "转发";
        menuItemZF.Name = "redirt";
        menuItemZF.Click += menuItemZFClick;
        menuItemZF.Tag = "";


        menuItemSC.Header = "收藏";
        menuItemSC.Name = "SC";
        menuItemSC.Click += menuItemDel_Click;
        menuItemSC.Tag = "";
        menuItemSC.Visibility = Visibility.Collapsed;


        menuItemDel.Header = "删除";
        menuItemDel.Name = "del";
        menuItemDel.Click += menuItemDel_Click;
        menuItemDel.Tag = "";



        menuItemBack.Header = "撤回";
        menuItemBack.Name = "undo";
        menuItemBack.Click += menuItemBack_Click;
        menuItemBack.Tag = "";
        menuItemBack.Visibility = Visibility.Collapsed;

        menuItemDX.Header = "多选";
        menuItemDX.Name = "mulsel";
        menuItemDX.Click += menuItemDXClick;
        menuItemDX.Tag = "";
        menuItemDX.Visibility = Visibility.Collapsed;

        menuItemSave.Header = "另存为";
        menuItemSave.Name = "saveas";
        menuItemSave.Click += MenuItemSave_Click; ;
        menuItemSave.Tag = "";

        menuClassify.Items.Add(menuItemSC);
        menuClassify.Items.Add(menuItemDX);

        menuClassify.Items.Add(menuItemFZ);
        menuClassify.Items.Add(menuItemZF);
        menuClassify.Items.Add(menuItemSave);
        menuClassify.Items.Add(menuItemDel);
        menuClassify.Items.Add(menuItemBack);

        if (Item.type == MsgType.Text.ToStr()|| Item.type == MsgType.At.ToStr()) {
            menuItemSave.Visibility = Visibility.Collapsed;
        } else if (Item.type == MsgType.Image.ToStr() || Item.type == MsgType.File.ToStr()
                   || Item.type == MsgType.Video.ToStr()) {
            menuItemZF.Visibility = Visibility.Collapsed;
            menuItemFZ.Visibility = Visibility.Collapsed;
        } else if (Item.type == MsgType.VCard.ToStr() ||
                   Item.type == MsgType.PublicCard.ToStr()) {
            menuItemSave.Visibility = Visibility.Collapsed;
            menuItemFZ.Visibility = Visibility.Collapsed;
        } else if (Item.type == MsgType.Location.ToStr() ||
                   Item.type == MsgType.PublicCard.ToStr()) {
            menuItemSave.Visibility = Visibility.Collapsed;
            menuItemFZ.Visibility = Visibility.Collapsed;
        }

        //撤销
        bool timeDiff = false;
        TimeSpan ts = DateTimeHelper.DateDiff(System.DateTime.Now, Item.timeDate);
        // 消息间隔大于2分钟
        if (ts.Minutes < 2) {
            timeDiff = true;
        }


        if (Left == false && timeDiff) {
            menuItemBack.Visibility = Visibility.Visible;
        }

        //menuClassify.PlacementTarget = lable;
        //if(Left==false) {
        //    menuClassify.Placement = System.Windows.Controls.Primitives.PlacementMode.Left;
        //    menuClassify.HorizontalOffset = -5;
        //} else {
        //    menuClassify.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
        //    menuClassify.HorizontalOffset = 5;
        //}

        //menuClassify.HorizontalOffset = 5;
        //menuClassify.Width = 74;
        //menuClassify.VerticalOffset = -5;
        //menuClassify.HorizontalOffset = 13;
        //menuClassify.HorizontalAlignment = HorizontalAlignment.Center;
        //menuClassify.IsOpen = true;
    }

    private void MenuItemSave_Click(object sender, RoutedEventArgs e) {
        string filePath = string.Empty;
        SaveFileDialog SaveFile = new SaveFileDialog();
        FilesTable dt = null;
        if(item.type==MsgType.File.ToStr()) {
            FileMessage messageBean = null;
            messageBean = new FileMessage().toModel(Item.content);
            if (messageBean.fileStorageId != string.Empty) {
                dt = FilesService.getInstance().getFile(messageBean.fileStorageId);
            } else {
                dt = FilesService.getInstance().getFileByOwner(Item.messageId);
            }

            if (dt == null) return;
            filePath = dt.localpath;
            String downloadFileSuffix = ToolsHelper.getFileSuffix(filePath);

            SaveFile.FileName = messageBean.fileName;
            SaveFile.Filter = "文件|*" + downloadFileSuffix;
        } else if (item.type == MsgType.Video.ToStr()) {
            VideoMessage messageBean = null;
            messageBean = new VideoMessage().toModel(Item.content);
            if (!string.IsNullOrEmpty(messageBean.videoStorageId )) {
                dt = FilesService.getInstance().getFile(messageBean.videoStorageId);
            } else {
                dt = FilesService.getInstance().getFileByOwner(Item.messageId);
            }

            if (dt == null) return;
            filePath = dt.localpath;
            String downloadFileSuffix = ToolsHelper.getFileSuffix(filePath);
            SaveFile.FileName = messageBean.fileName;
            SaveFile.Filter = "视频|*" + downloadFileSuffix;
        } else if (item.type == MsgType.Image.ToStr()) {
            PictureMessage messageBean = null;
            messageBean = new PictureMessage().toModel(Item.content);
            if (messageBean.imageStorageId != string.Empty) {
                dt = FilesService.getInstance().getFile(messageBean.imageStorageId);
            } else {
                dt = FilesService.getInstance().getFileByOwner(Item.messageId);
            }

            if (dt == null) return;
            filePath = dt.localpath;
            String downloadFileSuffix = ToolsHelper.getFileSuffix(filePath);
            SaveFile.FileName = DateTimeHelper.getTimeStamp().ToStr();
            SaveFile.Filter = "图片|*" + downloadFileSuffix;
        }
        SaveFile.RestoreDirectory = true;
        if (SaveFile.ShowDialog() == DialogResult.OK) {
            System.IO.File.Copy(filePath, SaveFile.FileName, true);//文件另存为
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void menuItemDel_Click(object sender, RoutedEventArgs e) {

        MessageService.getInstance().deleteByMessageId(Item.messageId);

        //发出通知
        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
        businessEvent.data = Item;
        businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_DELETE;
        EventBusHelper.getInstance().fireEvent(businessEvent);
    }

    /// <summary>
    /// 撤回
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void menuItemBack_Click(object sender, RoutedEventArgs e) {

        MessageItem backItem = MessageService.getInstance().sendCancelMessage(Item);

        Item.text = backItem.text;
        Item.type = backItem.type;

        MessageService.getInstance().DoSendMessage(Item);

        //发出通知
        BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
        businessEvent.data = Item;
        businessEvent.eventDataType = BusinessEventDataType.MessageChangedEvent_TYPE_CANCLE;
        EventBusHelper.getInstance().fireEvent(businessEvent);

    }


    //多选
    private void menuItemDXClick(object sender, RoutedEventArgs e) {
        if (DxClick != null) {
            DxClick(this, e);
        }


    }
    //复制
    private void menuItemFZClick(object sender, RoutedEventArgs e) {
        Clipboard.SetDataObject(Item.text);
    }

    //转发
    public virtual void menuItemZFClick(object sender, RoutedEventArgs e) {
        ForwardMessage win = new ForwardMessage();
        win.Message = Item;
        win.ShowDialog();
    }
}
}
