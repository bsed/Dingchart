using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ResourceDictionary.Converters {
class FileImageConvertor : IValueConverter {

    //Dictionary<string, string> FILE_ICON_Table = null;

    //private void CreatFileIco() {
    //    FILE_ICON_Table = new Dictionary<string, string>();
    //    FILE_ICON_Table.Add("default", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");

    //    FILE_ICON_Table.Add(".7z", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".gtar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".gz", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".jar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".rar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".tar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".tgz", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");
    //    FILE_ICON_Table.Add(".zip", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png");

    //    FILE_ICON_Table.Add(".m3u", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".m4a", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".m4b", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".mp2", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".mp3", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".mpga", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".ogg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".wav", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".wma", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".ra", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".mid", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".ape", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".flac", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");
    //    FILE_ICON_Table.Add(".amr", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png");

    //    FILE_ICON_Table.Add(".doc", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_word.png");
    //    FILE_ICON_Table.Add(".docx", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_word.png");

    //    FILE_ICON_Table.Add(".bmp", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png");
    //    FILE_ICON_Table.Add(".gif", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png");
    //    FILE_ICON_Table.Add(".jpeg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png");
    //    FILE_ICON_Table.Add(".jpg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png");
    //    FILE_ICON_Table.Add(".png", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png");

    //    FILE_ICON_Table.Add(".pdf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_pdf.png");

    //    FILE_ICON_Table.Add(".pps", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png");
    //    FILE_ICON_Table.Add(".ppt", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png");
    //    FILE_ICON_Table.Add(".pptx", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png");

    //    FILE_ICON_Table.Add(".pst", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png");


    //    FILE_ICON_Table.Add(".bin", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".conf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".class", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".exe", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".prop", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add("properties", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".mpc", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".sh", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");
    //    FILE_ICON_Table.Add(".rc", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png");

    //    FILE_ICON_Table.Add(".c", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".cpp", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".h", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".java", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".log", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".msg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".rtf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".txt", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".js", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");
    //    FILE_ICON_Table.Add(".xml", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png");

    //    FILE_ICON_Table.Add(".3gp", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".asf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".avi", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".m4u", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".m4v", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".mov", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".mp4", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".mkv", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".mpe", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".mpeg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".mpg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".rmvb", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".rm", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".flv", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".wmv", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");
    //    FILE_ICON_Table.Add(".vob", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png");


    //    FILE_ICON_Table.Add(".htm", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_html.png");
    //    FILE_ICON_Table.Add(".html", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_html.png");

    //    FILE_ICON_Table.Add(".xls", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_excel.png");
    //    FILE_ICON_Table.Add(".xlsx", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_excel.png");

    //    FILE_ICON_Table.Add(".apk", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_apk.png");
    //}


    public static Dictionary<string, string> FILE_ICON_Table = new Dictionary<string, string>() {
        { "default", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},

        {".7z", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".gtar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".gz", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".jar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".rar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".tar", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".tgz", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},
        {".zip", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_cutdown.png"},

        {".m3u", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".m4a", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".m4b", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".mp2", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".mp3", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".mpga", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".ogg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".wav", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".wma", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".ra", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".mid", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".ape", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".flac", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},
        {".amr", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_voice.png"},

        {".doc", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_word.png"},
        {".docx", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_word.png"},

        {".bmp", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png"},
        {".gif", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png"},
        {".jpeg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png"},
        {".jpg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png"},
        {".png", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_image.png"},

        {".pdf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_pdf.png"},

        {".pps", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png"},
        {".ppt", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png"},
        {".pptx", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png"},

        {".pst", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_ppt.png"},


        {".bin", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".conf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".class", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".exe", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".prop", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {"properties", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".mpc", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".sh", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},
        {".rc", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_unknow.png"},

        {".c", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".cpp", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".h", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".java", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".log", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".msg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".rtf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".txt", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".js", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},
        {".xml", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_txt.png"},

        {".3gp", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".asf", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".avi", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".m4u", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".m4v", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".mov", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".mp4", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".mkv", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".mpe", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".mpeg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".mpg", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".rmvb", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".rm", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".flv", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".wmv", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},
        {".vob", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_video.png"},

        {".htm", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_html.png"},
        {".html", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_html.png"},

        {".xls", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_excel.png"},
        {".xlsx", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_excel.png"},

        {".apk", @"pack://application:,,,/ResourceDictionary;Component/images/File/file_apk.png"}
    };

    public static String getFileSuffix(String filePath) {
        String downloadFileSuffix = "";
        int pointIndex = filePath.LastIndexOf(".");
        if (pointIndex >= 0) {
            downloadFileSuffix = filePath.Substring(pointIndex+1);
        }
        return downloadFileSuffix;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

        try {
            String downloadFileSuffix = getFileSuffix(value.ToString());

            return string.Format(@"pack://application:,,,/ResourceDictionary;Component/images/File/{0}.png",
                                 downloadFileSuffix);

        } catch (Exception ex) {
            return FILE_ICON_Table["default"];
        }


    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
}
}
