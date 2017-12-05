using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using cn.lds.chatcore.capture;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;

using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Emoji;
using cn.lds.chatcore.pcw.Emoji.Entity;
using cn.lds.chatcore.pcw.imtp.message;

namespace cn.lds.chatcore.pcw.Views.Control {

public enum PictureTag {
    Image,
    Emoji
}
/// <summary>
/// SendMessageControl.xaml 的交互逻辑
/// </summary>
public partial class RichTextBoxControl : UserControl, ICaptureCallback {

    private static int maxImgHeight = 150;
    private static int maxImgWidth = 150;
    private static int maxEmojiHeight = 24;
    private static int maxEmojiWidth = 150;
    public RichTextBoxControl() {
        InitializeComponent();

        DicSelectedFiles = new Dictionary<string, string>();
        ImagesExtension = new string[] {".png", ".jpg", ".jpeg", ".bmp", ".gif"};
    }

    public delegate void SendMsgEventHandler(object sender, RoutedEventArgs e);//定义委托
    public event SendMsgEventHandler SendMsgEvent; //定义事件


    //变量定义
    public string[] ImagesExtension {
        get;
    }

    public Dictionary<string, string> DicSelectedFiles {
        get;
    }

    /// <summary>
    /// 画面加载
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void RichTextBoxUserControl_Loaded(object sender, RoutedEventArgs e) {

        Clear();

        groupMemberListControl.SetItemClickCallBack(SelMemberCallBack);

        PcStart.getInstance().captureCallback = this;
    }

    public void Clear() {
        //this.Dispatcher.BeginInvoke(new Action(() => {
        TxtMessage.Document.Blocks.Clear();
        DicSelectedFiles.Clear();
        Paragraph txtMessageParagraph = new Paragraph(new Run(""));
        TxtMessage.Document.Blocks.Add(txtMessageParagraph);
        //TxtMessage.ScrollToEnd();
        TxtMessage.Focus();
        //}));
    }

    #region 事件

    /// <summary>
    /// 文本输入时触发的事件
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void TxtMessage_KeyDown(object sender, KeyEventArgs e) {
        try {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.KeyStates == Keyboard.GetKeyStates(Key.Enter)) {
                TxtMessage.Focus();
                TxtMessage.CaretPosition = TxtMessage.CaretPosition.InsertParagraphBreak();
                e.Handled = true;
            } else if (e.Key == Key.Enter) {
                if (groupMemberListPopup.IsOpen) {
                    // 群聊中，选择@对象时
                    groupMemberListControl.ItemClickCommandFun();
                    e.Handled = true;
                } else if (null != SendMsgEvent) {
                    SendMsgEvent(null, null);
                    e.Handled = true;
                }
            } else if (Keyboard.Modifiers == ModifierKeys.Alt && e.KeyStates == Keyboard.GetKeyStates(Key.S)) {
                if (null != SendMsgEvent) {
                    SendMsgEvent(null, null);
                    e.Handled = true;
                }
            }

        } catch (Exception ex) {
            Log.Error(typeof(RichTextBoxControl), ex);
        }
    }

    /// <summary>
    /// 监听系统粘贴事件
    /// </summary>
    /// <param Name="sender">RichTextBox</param>
    /// <param Name="e"></param>
    private void CommandBinding_PasteExecuted(object sender, ExecutedRoutedEventArgs e) {
        //取出剪切板内数据
        IDataObject iData = Clipboard.GetDataObject();
        string[] dataFormat = iData.GetFormats();
        if (iData != null) {
            if (iData.GetDataPresent(DataFormats.Bitmap)) {
                if (CountSelectedFiles() >= 5) {
                    NotificationHelper.ShowWarningMessage("您最多只能选择5个文件！");

                } else {
                    BitmapSource bitmapSource = Clipboard.GetImage();
                    string path = ProcessImage(bitmapSource);
                    CreatePictureControl(path, PictureTag.Image.ToString());
                }
            } else if (iData.GetDataPresent(DataFormats.Text)) {
                string text = Clipboard.GetText();
                List<WordItem> wordItems = EmojiHelper.ConvertTextImgToEmoji(text);
                foreach (var wordItem in wordItems) {
                    switch (wordItem.Type) {
                    case WordType.Text:
                        TxtMessage.CaretPosition.InsertTextInRun(wordItem.Content);
                        MoveCaretPosition();
                        break;
                    case WordType.Wrap:
                        TxtMessage.CaretPosition.InsertLineBreak();
                        MoveCaretPosition();
                        break;
                    case WordType.Emoji:
                        InsertEmoji(wordItem.Content);
                        break;
                    case WordType.Image:
                        InsertPicture(wordItem.Content);
                        break;
                    case WordType.File:
                        InsertFile(wordItem.Content);
                        break;
                    case WordType.AtChar:
                        TextBlock txtBlock = new TextBlock();
                        txtBlock.Text = "@" + wordItem.Content;
                        txtBlock.Tag = WordType.AtChar.ToString();
                        InlineUIContainer atcharContainer = new InlineUIContainer(txtBlock, TxtMessage.CaretPosition);
                        TxtMessage.CaretPosition = atcharContainer.ElementEnd;
                        TxtMessage.Focus();
                        break;

                    }
                }
            } else if (iData.GetDataPresent(DataFormats.Rtf)) {
                //内存内实例化一个RichTextBox
                RichTextBox rtb = new RichTextBox();
                //rtf格式
                var rtf = iData.GetData(DataFormats.Rtf);
                //将rtf载入内存内的RichTextBox
                TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                using (MemoryStream rtfMemoryStream = new MemoryStream()) {
                    using (StreamWriter rtfStreamWriter = new StreamWriter(rtfMemoryStream)) {
                        rtfStreamWriter.Write(rtf);
                        rtfStreamWriter.Flush();
                        rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                        //Load the MemoryStream into TextRange ranging from start to end of RichTextBox.
                        textRange.Load(rtfMemoryStream, DataFormats.Rtf);
                    }
                }

                foreach (Block block in rtb.Document.Blocks) {
                    //Paragraph newParagrph = new Paragraph();
                    if (block is Paragraph) {
                        Paragraph rcbParagrph = block as Paragraph;
                        if (rcbParagrph == null) continue;
                        foreach (Inline il in rcbParagrph.Inlines) {
                            if (il is Run) {
                                //获取text内容
                                TxtMessage.CaretPosition.InsertTextInRun((il as Run).Text);
                                MoveCaretPosition();
                            } else if (il is InlineUIContainer) {
                                InlineUIContainer inlineUiContainer = il as InlineUIContainer;
                                if (inlineUiContainer.Child is System.Windows.Controls.Image) {
                                    if (CountSelectedFiles() >= 5) {
                                        // 只能选择5个文件
                                        NotificationHelper.ShowWarningMessage("您最多只能选择5个文件！");
                                    } else {
                                        System.Windows.Controls.Image image =
                                            inlineUiContainer.Child as System.Windows.Controls.Image;
                                        string path = ProcessImage(image.Source as BitmapSource);
                                        InsertPicture(path);
                                    }
                                }
                            } else if (il is Span) {
                                foreach (Inline spanInline in (il as Span).Inlines) {
                                    if (spanInline is Run) {
                                        //获取text内容
                                        TxtMessage.CaretPosition.InsertTextInRun((spanInline as Run).Text);
                                        MoveCaretPosition();
                                    }
                                }
                            }
                        }
                    }
                }

            } else {
                TxtMessage.Paste();
            }
        }
        e.Handled = true;
    }

    private void MoveCaretPosition() {
        if (TxtMessage.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward) != null) {
            TxtMessage.CaretPosition =
                TxtMessage.CaretPosition.GetNextContextPosition(LogicalDirection.Forward);
        }
        TxtMessage.Focus();
    }
    public void InsertPicture(string path) {
        try {
            if (!DicSelectedFiles.ContainsKey(path)) {
                // 不重复的图片才创建做略图
                string previewImagePath = CreatePreviewImage(path);
                if (!String.IsNullOrEmpty(previewImagePath)) {
                    DicSelectedFiles.Add(path, previewImagePath);
                } else {
                    Log.Error(typeof(SendMessageControl), "创建文件缩略图失败：" + path);
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(RichTextBoxControl), ex);
        }
        CreatePictureControl(path, PictureTag.Image.ToString());
    }

    public void InsertFile(string path) {
        try {
            FileInfo info = new FileInfo(path);
            if (info.Exists) {
                if (FileHelper.FileIsInUsed(path)) {
                    CommonMessageBox.Msg.Show("文件被占用中无法发送");
                    return;
                }
                if (info.Length > 1024*1000*100) {
                    CommonMessageBox.Msg.Show("发送的文件大小不能大于100M");
                    return;
                }
                FileIconControl fileIconControl = new FileIconControl();
                fileIconControl.FileIcon = FileIcon.GetFileIcon(path);
                fileIconControl.FileFullName = path;
                fileIconControl.FileName = Path.GetFileName(path);
                InlineUIContainer fileContainer = new InlineUIContainer(fileIconControl, TxtMessage.Selection.End);
                TxtMessage.CaretPosition = fileContainer.ElementEnd;
                TxtMessage.Focus();
            }
        } catch (Exception ex) {
            Log.Error(typeof(RichTextBoxControl), ex);
        }

    }

    public void InsertEmoji(string path) {
        CreatePictureControl(path, PictureTag.Emoji.ToString());
    }

/// <summary>
///  截图回调
/// </summary>
/// <param Name="bitmap"></param>
    public void CaptureCallback(Bitmap bitmap) {
        if (CountSelectedFiles() >= 5) {
            NotificationHelper.ShowWarningMessage("您最多只能选择5个文件！");
            return;
        }
        BitmapSource bitmapSource = Clipboard.GetImage();
        string path = ProcessImage(bitmapSource);
        CreatePictureControl(path, PictureTag.Image.ToString());
    }

    #endregion


    #region 私有方法开始

    public int CountSelectedFiles() {
        List<MessageItem> itemList = ProcessSendMsg();
        return itemList.Where(item => !item.type.Equals(MsgType.Text.ToStr())).ToList().Count;
    }

/// <summary>
/// 分割消息内容
/// </summary>
/// <returns></returns>
    public List<MessageItem> ProcessSendMsg() {

        BlockCollection blocks = TxtMessage.Document.Blocks;
        List<MessageItem> msgList = new List<MessageItem>();
        if (blocks.Count > 0) {
            foreach (Block block in blocks) {
                if (msgList.Count > 0 && msgList[msgList.Count - 1].type.Equals(MsgType.Text.ToStr())) {
                    msgList[msgList.Count - 1].content += Environment.NewLine;
                }
                Paragraph txtMessageParagraph = block as Paragraph;
                if (txtMessageParagraph != null) {
                    foreach (Inline il in txtMessageParagraph.Inlines) {
                        if (il is Run) {
                            //获取text内容
                            if((il as Run).Text.Trim()!=string.Empty) {
                                if (msgList.Count > 0 && msgList[msgList.Count - 1].type.Equals(MsgType.Text.ToStr())) {
                                    msgList[msgList.Count - 1].content += (il as Run).Text;
                                } else {
                                    msgList.Add(new MessageItem {type = MsgType.Text.ToStr(),content = (il as Run).Text});
                                }
                            }

                        } else if (il is InlineUIContainer) {
                            InlineUIContainer inlineUiContainer = il as InlineUIContainer;
                            if (inlineUiContainer.Child is System.Windows.Controls.Image) {
                                System.Windows.Controls.Image image =
                                    inlineUiContainer.Child as System.Windows.Controls.Image;
                                string localPath = ((BitmapImage)(image.Source)).UriSource.OriginalString;
                                if (image.Tag.Equals(PictureTag.Emoji.ToString())) {
                                    // 表情符号的处理
                                    string emojiCode = EmojiHelper.ConvertEmojiToString(localPath);
                                    if (msgList.Count > 0 &&
                                            msgList[msgList.Count - 1].type.Equals(MsgType.Text.ToStr())) {
                                        msgList[msgList.Count - 1].content += emojiCode;
                                    } else {
                                        msgList.Add(new MessageItem { type = MsgType.Text.ToStr(), content = emojiCode });
                                    }
                                } else {
                                    // 普通图片的处理
                                    msgList.Add(new MessageItem {type = MsgType.Image.ToStr(), content = localPath});
                                }
                            } else if (inlineUiContainer.Child is FileIconControl) {
                                msgList.Add(new MessageItem { type = MsgType.File.ToStr(), content = (inlineUiContainer.Child as FileIconControl).FileFullName});
                            } else if (inlineUiContainer.Child is TextBlock) {
                                WordType charType = WordType.AtChar;
                                string no = string.Empty;
                                if ((inlineUiContainer.Child as TextBlock).Tag != null) {
                                    if (Enum.TryParse((inlineUiContainer.Child as TextBlock).Tag.ToString(), out charType)) {
                                        no = "," + inlineUiContainer.Tag.ToStr();
                                    }
                                }
                                //获取text内容
                                if (msgList.Count > 0 && msgList[msgList.Count - 1].type.Equals(MsgType.Text.ToStr())) {
                                    msgList[msgList.Count - 1].content += (inlineUiContainer.Child as TextBlock).Text;
                                    msgList[msgList.Count - 1].user += no;
                                } else {
                                    msgList.Add(new MessageItem { type = MsgType.Text.ToStr(),
                                                                  content = (inlineUiContainer.Child as TextBlock).Text,
                                                                  user = no
                                                                });
                                }
                            }
                        } else if (il is Span) {
                            foreach (Inline spanInline in (il as Span).Inlines) {
                                if (spanInline is Run) {
                                    //获取text内容
                                    if (msgList.Count > 0 &&
                                            msgList[msgList.Count - 1].type.Equals(MsgType.Text.ToStr())) {
                                        msgList[msgList.Count - 1].content += (il as Run).Text;
                                    } else {
                                        msgList.Add(new MessageItem {type = MsgType.Text.ToStr(),content = (spanInline as Run).Text});
                                    }
                                }
                            }
                        } else if (il is LineBreak) {
                            if (msgList.Count > 0 && msgList[msgList.Count - 1].type.Equals(MsgType.Text.ToStr())) {
                                msgList[msgList.Count - 1].content += Environment.NewLine;
                            }
                        }
                    }
                }
            }
        }

        return msgList;
    }

/// <summary>
/// 通过图片路径创建图片控件
/// </summary>
/// <param Name="path"></param>
/// <returns></returns>
    private System.Windows.Controls.Image CreateImageControl(string path,int maxWidth, int maxHeight) {
        System.Windows.Controls.Image img = new System.Windows.Controls.Image();
        BitmapImage bImg = new BitmapImage();
        try {
            img.IsEnabled = true;
            bImg.BeginInit();
            bImg.CacheOption = BitmapCacheOption.OnLoad;
            bImg.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bImg.EndInit();
            bImg.Freeze();
            img.Source = bImg;
        } catch (Exception ex) {
            CommonMessageBox.Msg.Show("图片格式不正确，或者文件已损坏。");
            return null;
        }
        // 调整图片大小
        if (bImg.Height > maxHeight || bImg.Width > maxWidth) {
            if ((bImg.Width * maxHeight) > (bImg.Height * maxWidth)) {
                img.Height = (bImg.Height * maxWidth) / bImg.Width;
                img.Width = maxWidth;
            } else {
                img.Width = (bImg.Width * maxHeight) / bImg.Height;
                img.Height = maxHeight;
            }
        } else {
            img.Width = bImg.Width;
            img.Height = bImg.Height;
        }


        // 图片缩放模式
        img.Stretch = Stretch.Uniform;

        return img;
    }

/// <summary>
/// 剪贴板内的图片保存到本地
/// </summary>
/// <param Name="bitmap"></param>
/// <returns></returns>
    private string SaveClipboardImg(Bitmap bitmap) {
        string path = System.Environment.GetEnvironmentVariable("TEMP")
                      + Guid.NewGuid().ToString() + ".bmp";
        bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        bitmap.Dispose();

        return path;
    }

    private string ProcessImage(BitmapSource bitmapSource) {
        Bitmap bitmap = BitmapSourceToBitmap(bitmapSource);
        string path = SaveClipboardImg(bitmap);
        string previewImagePath = CreatePreviewImage(path);
        if (!String.IsNullOrEmpty(previewImagePath)) {
            DicSelectedFiles.Add(path, previewImagePath);
        }

        return path;
    }

/// <summary>
/// 生成缩略图
/// </summary>
/// <param Name="imgFilePath"></param>
/// <returns></returns>
    private string CreatePreviewImage(String imgFilePath) {
        if (File.Exists(imgFilePath)) {
            System.Drawing.Image img = System.Drawing.Image.FromFile(imgFilePath);
            if (img.Width < 230 && img.Height < 230) {
                return imgFilePath;
            }
            int width = img.Width;
            int height = img.Height;

            if (height > 230 || width > 230) {
                if ((width * 230) > (height * 230)) {
                    width = 230;
                    height = (height * 230) / img.Width;

                } else {
                    height = 230;
                    width = (width * 230) / img.Height;
                }
            }

            string previewImagePath = System.Environment.GetEnvironmentVariable("TEMP")
                                      + Guid.NewGuid().ToString() + Path.GetExtension(imgFilePath);
            ImageHelper. CreateThumbnail(img, width, height, previewImagePath);

            return previewImagePath;
        } else {

            return "";
        }
    }

/// <summary>
/// BitmapSourceToBitmap
/// </summary>
/// <param Name="image"></param>
/// <returns></returns>
    private Bitmap BitmapSourceToBitmap(BitmapSource image) {
        MemoryStream ms = new MemoryStream();
        BitmapEncoder enc = new BmpBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(image));
        enc.Save(ms);
        Bitmap bitmap = new Bitmap(ms);
        ms.Flush();
        ms = null;

        return bitmap;
    }

/// <summary>
/// 文本框内插入图片
/// </summary>
/// <param Name="path"></param>
/// <param Name="tag">区分图片还是表情</param>
/// <returns></returns>
    private bool CreatePictureControl(string path, string tag) {
        System.Windows.Controls.Image img = CreateImageControl(path,
                                            tag.Equals(PictureTag.Emoji.ToString()) ? maxEmojiWidth: maxImgWidth,
                                            tag.Equals(PictureTag.Emoji.ToString()) ? maxEmojiHeight:maxImgHeight);
        if (img == null) return false;

        img.Tag = tag;

        //TxtMessage.Focus();
        // 添加到控件中
        InlineUIContainer inlineUIContainer = new InlineUIContainer(img, TxtMessage.Selection.End);
        TxtMessage.CaretPosition = inlineUIContainer.ElementEnd;
        TxtMessage.Focus();
        return true;
    }

    #endregion

    #region richtextbox 拖放
    private void richTextBox_DragEnter(object sender, DragEventArgs e) {
        e.Effects = DragDropEffects.All;
        e.Handled = true;
    }
    private void richTextBox_PreviewDrop(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            Array files = (Array) e.Data.GetData(DataFormats.FileDrop);
            if (files != null) {
                foreach (String path in files) {
                    if (CountSelectedFiles() >= 5) {
                        NotificationHelper.ShowWarningMessage("您最多只能选择5个文件！");
                        break;
                    }
                    try {
                        String fileExtension = Path.GetExtension(path);
                        if (ImagesExtension.Contains(fileExtension.ToLower())) {
                            // 图片
                            if (!DicSelectedFiles.ContainsKey(path)) {
                                string previewImagePath = CreatePreviewImage(path);
                                if (!String.IsNullOrEmpty(previewImagePath)) {
                                    DicSelectedFiles.Add(path, previewImagePath);
                                } else {
                                    Log.Error(typeof (SendMessageControl), "创建缩略图失败：" + path);
                                }
                            }
                            CreatePictureControl(path, PictureTag.Image.ToString());
                        } else {
                            InsertFile(path);
                        }
                    } catch (Exception ex) {
                        Log.Error(typeof(SendMessageControl), ex);
                    }
                }
            }
        }
    }
    #endregion

    private void CommandBinding_CopyExecuted(object sender, ExecutedRoutedEventArgs e) {
        TextPointer startPosition = TxtMessage.Selection.Start;
        TextPointer endPosition = TxtMessage.Selection.End;
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
                            item.Content = ((BitmapImage) (image.Source)).UriSource.OriginalString;
                            selectWordItems.Add(item);
                        }
                    } else if (inlineUiContainer.Child is FileIconControl) {
                        WordItem item = new WordItem();
                        item.Type = WordType.File;
                        item.Content = (inlineUiContainer.Child as FileIconControl).FileFullName;
                        selectWordItems.Add(item);
                    } else if (inlineUiContainer.Child is TextBlock) {
                        WordType charType = WordType.AtChar;
                        if ((inlineUiContainer.Child as TextBlock).Tag != null) {
                            if (Enum.TryParse((inlineUiContainer.Child as TextBlock).Tag.ToString(), out charType)) {
                                WordItem item = new WordItem();
                                item.Type = charType;
                                item.Content = (inlineUiContainer.Child as TextBlock).Text;
                                selectWordItems.Add(item);
                            }
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
            case  WordType.Wrap:
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
            case WordType.File:
                stringBuilder.Append("[file]");
                stringBuilder.Append(item.Content);
                stringBuilder.Append("[/file]");
                break;
            }
        }
        int loop = 5;
        while (loop > 0) {
            // 循环5次防止复制粘贴板失败
            try {
                Clipboard.Clear();
                Clipboard.SetDataObject(string.Empty);
                Clipboard.SetDataObject(stringBuilder.ToString());
                loop = 0;
            } catch (Exception ex) {
                loop--;
                Log.Error(typeof(RichTextBoxControl), "Copy or Cut Failed : " + ex.Message);
            }
        }
        if (e.Command.Equals(ApplicationCommands.Cut)) TxtMessage.Selection.Text = "";
    }

    private int atCharOffSet = 0;
    private bool isAtChar = false;
    private void TxtMessage_OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
        if (ChatSessionType.MUC == ChatPage.getInstance().ChatSessionType) {
            if (e.Text.Equals("@")) {
                isAtChar = true;
                atCharOffSet = TxtMessage.Document.ContentStart.GetOffsetToPosition(TxtMessage.Selection.End);
                Console.WriteLine(string.Format("@ CurrPos:{0}", atCharOffSet));

                //if (groupMemberListControl.Search("") > 0) {
                //    groupMemberListPopup.IsOpen = true;
                //}
            }
        }
    }

    private void TxtMessage_OnPreviewKeyDown(object sender, KeyEventArgs e) {

    }

    private void TxtMessage_OnKeyUp(object sender, KeyEventArgs e) {
        if (groupMemberListPopup.IsOpen) {
            switch (e.Key) {
            case Key.Up:
                groupMemberListControl.ItemUpKeyUpCommand();
                e.Handled = true;
                break;
            case Key.Down:
                groupMemberListControl.ItemDownKeyUpCommand();
                e.Handled = true;
                break;
            }
        }
    }

    private void SelMemberCallBack(string name, string no) {
        groupMemberListPopup.IsOpen = false;

        TxtMessage.Focus();
        TextRange textrange = new TextRange(TxtMessage.Document.ContentStart.GetPositionAtOffset(atCharOffSet - 2),
                                            TxtMessage.Selection.End);
        textrange.Text = "";
        TextBlock txtBlock = new TextBlock();
        txtBlock.Text = "@" + name + " ";
        txtBlock.Tag = WordType.AtChar.ToString();
        InlineUIContainer inlineUIContainer =  new InlineUIContainer(txtBlock, TxtMessage.Selection.End);
        inlineUIContainer.Tag = no;
        TxtMessage.CaretPosition = inlineUIContainer.ElementEnd;
        TxtMessage.Focus();
        atCharOffSet = 0;
    }

    private void TxtMessage_OnTextChanged(object sender, TextChangedEventArgs e) {
        if (groupMemberListPopup != null) {
            int currOffset = TxtMessage.Document.ContentStart.GetOffsetToPosition(TxtMessage.Selection.End);
            if (isAtChar) {
                isAtChar = false;
                atCharOffSet = currOffset;
            }
            if (atCharOffSet > 0 && currOffset >= atCharOffSet) {
                TextRange textrange = new TextRange(TxtMessage.Document.ContentStart.GetPositionAtOffset(atCharOffSet),
                                                    TxtMessage.Selection.End);
                Console.WriteLine(string.Format("TxtMessage_OnTextChanged : textrange:{0}", textrange.Text));
                if (groupMemberListControl.Search(textrange.Text) > 0) {
                    Rect rect = TxtMessage.CaretPosition.GetCharacterRect(LogicalDirection.Forward);
                    groupMemberListPopup.PlacementRectangle = rect;
                    groupMemberListPopup.IsOpen = true;
                } else {
                    groupMemberListPopup.IsOpen = false;
                }
            } else {
                atCharOffSet = 0;
                groupMemberListPopup.IsOpen = false;
            }
        }
    }
}
}
