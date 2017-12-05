using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using cn.lds.chatcore.pcw.Emoji.Emoji.Defaults;
using cn.lds.chatcore.pcw.Emoji.Entity;

namespace ResourceDictionary.Converters {
public class ReplaceEmojiToIcoConvertor : IMultiValueConverter {
    #region IValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values[0] == null) return null;
        TextBlock txtBlock = values[1] as TextBlock;

        txtBlock.Inlines.Clear();
        string text = values[0].ToString();
        int skip;
        int textLengthToProcess = text.Length;
        StringBuilder textBuilder = new StringBuilder(); // 文字消息
        for (int i = 0; i < text.Length; i += skip) {
            skip = 0;
            string icon = string.Empty;
            int unicode = char.ConvertToUtf32(text, i);
            skip = char.IsSurrogatePair(text, i) ? 2 : 1;
            if (unicode > 0xff) {
                icon = "0x" + unicode.ToString("x2");
                if (!DefaultsEmojis.Instance.EmojiToIcoDictionary.ContainsKey(icon)) {
                    icon = string.Empty;
                }
            }
            if (icon.Equals(string.Empty) && i + skip < textLengthToProcess) {
                int followUnicode = char.ConvertToUtf32(text, i + skip);
                if (followUnicode == 0x20e3) {
                    int followSkip = char.IsSurrogatePair(text, i + skip) ? 2 : 1;
                    if ((unicode >= 0x0030 && unicode <= 0x0039) || 0x0023 == unicode) {
                        icon = "0x" + unicode.ToString("x2") + "_0x20e3";
                    } else {
                        followSkip = 0;
                    }
                    skip += followSkip;
                } else {
                    int followSkip = char.IsSurrogatePair(text, i + skip) ? 2 : 1;
                    switch (unicode) {
                    case 0x1f1ef:
                        icon = (followUnicode == 0x1f1f5) ? "0x1f1ef_0x1f1f5"
                               : string.Empty;
                        break;
                    case 0x1f1fa:
                        icon = (followUnicode == 0x1f1f8) ? "0x1f1fa_0x1f1f8"
                               : string.Empty;
                        break;
                    case 0x1f1eb:
                        icon = (followUnicode == 0x1f1f7) ? "0x1f1eb_0x1f1f7"
                               : string.Empty;
                        break;
                    case 0x1f1e9:
                        icon = (followUnicode == 0x1f1ea) ? "0x1f1e9_0x1f1ea"
                               : string.Empty;
                        break;
                    case 0x1f1ee:
                        icon = (followUnicode == 0x1f1f9) ? "0x1f1ee_0x1f1f9"
                               : string.Empty;
                        break;
                    case 0x1f1ec:
                        icon = (followUnicode == 0x1f1e7) ? "0x1f1ec_0x1f1e7"
                               : string.Empty;
                        break;
                    case 0x1f1ea:
                        icon = (followUnicode == 0x1f1f8) ? "0x1f1ea_0x1f1f8"
                               : string.Empty;
                        break;
                    case 0x1f1f7:
                        icon = (followUnicode == 0x1f1fa) ? "0x1f1f7_0x1f1fa"
                               : string.Empty;
                        break;
                    case 0x1f1e8:
                        icon = (followUnicode == 0x1f1f3) ? "0x1f1e8_0x1f1f3"
                               : string.Empty;
                        break;
                    case 0x1f1f0:
                        icon = (followUnicode == 0x1f1f7) ? "0x1f1f0_0x1f1f7"
                               : string.Empty;
                        break;
                    default:
                        followSkip = 0;
                        break;
                    }
                    skip += followSkip;
                }
            }
            //}

            if (!icon.Equals(string.Empty)) {
                // 表情符号的场合
                EmojiItem emojiItem = null;
                if (DefaultsEmojis.Instance.EmojiToIcoDictionary.TryGetValue(icon, out emojiItem)) {
                    string iconPath = emojiItem.ImgPath;
                    if (textBuilder.Length > 0) {
                        // 把表情符号前面的文字创建出来
                        Run textrRun = new Run();
                        textrRun.Text = textBuilder.ToString();
                        txtBlock.Inlines.Add(textrRun);
                        textBuilder.Clear();
                    }
                    // 找到表情图标，创建表情控件
                    Image imgMessage = new Image();
                    imgMessage.Width = 15;
                    imgMessage.Height = 15;
                    BitmapImage bImg = new BitmapImage();
                    imgMessage.IsEnabled = true;
                    bImg.BeginInit();
                    bImg.UriSource = new Uri(iconPath, UriKind.RelativeOrAbsolute);
                    bImg.EndInit();
                    imgMessage.Source = bImg;
                    InlineUIContainer iuc = new InlineUIContainer(imgMessage);
                    txtBlock.Inlines.Add(iuc);
                } else {
                    // 没有找到表情图标,直接当作普通字符输出
                    textBuilder.Append(text.Substring(i, skip));
                }
            } else {
                // 普通文字的场合
                string currChar = text.Substring(i, 1);
                if (currChar.Equals(Environment.NewLine)) {
                    // 换行符的场合
                    if (textBuilder.Length > 0) {
                        // 生成前面的文字
                        Run textrRun = new Run();
                        textrRun.Text = textBuilder.ToString();
                        txtBlock.Inlines.Add(textrRun);
                        textBuilder.Clear();
                    }
                    // 生成换行
                    LineBreak lineBreak = new LineBreak();
                    txtBlock.Inlines.Add(lineBreak);
                } else {
                    // 除换行符意外的普通文字
                    textBuilder.Append(text.Substring(i, 1));
                }
            }
        }
        if (textBuilder.Length > 0) {
            // 生成前面的文字
            Run textrRun = new Run();
            textrRun.Text = textBuilder.ToString();
            txtBlock.Inlines.Add(textrRun);
            textBuilder.Clear();
        }
        return values[0];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
    #endregion

}
}
