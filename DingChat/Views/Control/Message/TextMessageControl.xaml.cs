using System;
using System.Collections.Generic;
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
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Emoji;
using cn.lds.chatcore.pcw.Emoji.Entity;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Page;
using EventBus;
using WpfAnimatedGif;


namespace cn.lds.chatcore.pcw.Views.Control.Message {
/// <summary>
/// TextMessageControl.xaml 的交互逻辑
/// </summary>
public partial class TextMessageControl : MessageBase {
    public TextMessageControl() {
        InitializeComponent();

    }
    public void Refresh() {
        lable_Loaded(null, null);
    }


    private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        LayoutRoot.Children.Clear();

        //聊天lable
        lable = new Label();
        //lable.Content = Item.text;
        //lable.MouseRightButtonDown += Lable_MouseRightButtonDown;
        lable.ContextMenu = menuClassify;
        lable.Loaded -= lable_Loaded;
        lable.Loaded += lable_Loaded;


        if (Left) {
            //显示内容的lable  大小可变
            LayoutRoot.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);


            Style btn_style = (Style)this.FindResource("LeftMessageLableNew");
            lable.HorizontalAlignment = HorizontalAlignment.Left;
            lable.VerticalAlignment = VerticalAlignment.Top;
            lable.Style = btn_style;
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


            Style btn_style = (Style)this.FindResource("RightMessageLableNew");
            lable.HorizontalAlignment = HorizontalAlignment.Right;
            lable.VerticalAlignment = VerticalAlignment.Top;
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
        RichTextBox txtBlock = CommonMethod.GetChildObject<RichTextBox>(sender as Label, "textBox");
        TextBlock txtBlockHidden = CommonMethod.GetChildObject<TextBlock>(sender as Label, "textBlock");
        if (txtBlock != null) {
            txtBlock.ContextMenu = menuClassify;
            //txtBlock.MouseRightButtonDown+= Lable_MouseRightButtonDown;
            txtBlock.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CommandBinding_CopyExecuted));
            EmojiHelper.ReplaceEmojiToIco(Item.text, txtBlockHidden);
            EmojiHelper.ReplaceEmojiToIco(Item.text, txtBlock);
        }
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

    private void CommandBinding_CopyExecuted(object sender, ExecutedRoutedEventArgs e) {
        TextPointer startPosition = (sender as RichTextBox).Selection.Start;
        TextPointer endPosition = (sender as RichTextBox).Selection.End;
        TextPointer currentPosition = startPosition;
        List<WordItem> selectWordItems = new List<WordItem>();
        StringBuilder sb = new StringBuilder();
        StringBuilder stringBuilder = new StringBuilder();
        do {
            switch (currentPosition.GetPointerContext(LogicalDirection.Forward)) {
            case TextPointerContext.None:
                sb.AppendLine("None:" + currentPosition.Parent.GetType());
                break;
            case TextPointerContext.Text:
                sb.AppendLine("Text:" + currentPosition.Parent.GetType());
                string textRun = currentPosition.GetTextInRun(LogicalDirection.Forward);
                if (!String.IsNullOrEmpty(textRun)) {
                    stringBuilder.Append(textRun);
                }
                break;
            case TextPointerContext.ElementStart:
                sb.AppendLine("ElementStart:" + currentPosition.Parent.GetType());
                break;
            case TextPointerContext.ElementEnd:
                sb.AppendLine("ElementEnd:" + currentPosition.Parent.GetType());
                if (currentPosition.Parent is LineBreak) {
                    if (stringBuilder.Length > 0) {
                        WordItem txtItem = new WordItem();
                        txtItem.Type = WordType.Text;
                        txtItem.Content = stringBuilder.ToString();
                        selectWordItems.Add(txtItem);
                        stringBuilder.Clear();
                    }
                    WordItem item = new WordItem();
                    item.Type = WordType.Wrap;
                    item.Content = String.Empty;
                    selectWordItems.Add(item);
                }
                break;
            case TextPointerContext.EmbeddedElement:
                sb.AppendLine("EmbeddedElement:" + currentPosition.Parent.GetType());
                if (currentPosition.Parent is InlineUIContainer) {
                    if (stringBuilder.Length > 0) {
                        WordItem item = new WordItem();
                        item.Type = WordType.Text;
                        item.Content = stringBuilder.ToString();
                        selectWordItems.Add(item);
                        stringBuilder.Clear();
                    }
                    InlineUIContainer inlineUiContainer = currentPosition.Parent as InlineUIContainer;
                    if (inlineUiContainer.Child is System.Windows.Controls.Image) {
                        System.Windows.Controls.Image image =
                            inlineUiContainer.Child as System.Windows.Controls.Image;
                        WordType imgType = WordType.Image;
                        if (Enum.TryParse(image.Tag.ToString(), out imgType)) {
                            WordItem item = new WordItem();
                            item.Type = imgType;
                            item.Content = ((BitmapImage)(image.Source)).UriSource.OriginalString;
                            selectWordItems.Add(item);
                        }
                    }
                }
                break;
            }
            currentPosition = currentPosition.GetNextContextPosition(LogicalDirection.Forward);

        } while (currentPosition.CompareTo(endPosition) < 0);

        if (stringBuilder.Length > 0) {
            WordItem item = new WordItem();
            item.Type = WordType.Text;
            item.Content = stringBuilder.ToString();
            selectWordItems.Add(item);
            stringBuilder.Clear();
        }
        foreach (var item in selectWordItems) {
            switch (item.Type) {
            case WordType.Text:
                stringBuilder.Append(item.Content);
                break;
            case WordType.Wrap:
                stringBuilder.Append(Environment.NewLine);
                break;
            case WordType.Emoji:
                string emojiCode = EmojiHelper.ConvertEmojiToString(item.Content);
                stringBuilder.Append(emojiCode);
                break;
            case WordType.Image:
                stringBuilder.Append("[img]");
                stringBuilder.Append(item.Content);
                stringBuilder.Append("[/img]");
                break;
            }
        }
        Clipboard.Clear();
        Clipboard.SetDataObject(stringBuilder.ToString());
        Console.Out.Write(sb.ToString());

    }




}
}
