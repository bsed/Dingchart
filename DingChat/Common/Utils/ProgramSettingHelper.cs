using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Views;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class ProgramSettingHelper {
    // 固定文字列
    public static Dictionary<string, string> StaticStringMap = new Dictionary<string, string>();

    public static List<ButtonImageCollection> MainProcessButtonList = new List<ButtonImageCollection>();

    public static List<ButtonImageCollection> MainSubProcessButtonList = new List<ButtonImageCollection>();

    public static List<SubMenuCollection> SubMenuProcessButtonList = new List<SubMenuCollection>();


    public static string LogoImage;


    public static string LogoName;




    // 主菜单默认选择项目
    private static String m_mainDefaultSelected = String.Empty;

    public static String MainDefaultSelected {
        get {
            return ProgramSettingHelper.m_mainDefaultSelected;
        } set {
            ProgramSettingHelper.m_mainDefaultSelected = value;
        }
    }



    public static String Host {
        set;
        get;
    }

    private static Int64 m_port = 0;
    public static Int64 Port {
        get {
            return (m_port == 0 ? 80 : m_port);
        } set {
            m_port = value;
        }
    }





    public static bool Version30 = false;

    public static String VideoKey {
        get;
        set;
    }



    /// <summary>
    /// 取所有key
    /// </summary>
    /// <returns></returns>
    public static void initProgramSettingXmlAllContents() {

        String fileName = Constants.PROGRAM_SETTING_FILE_NAME;

        FileInfo fileInfo = new FileInfo(fileName);
        if (!fileInfo.Exists) {
            return;
        }

        DataSet msgDS = null;
        try {
            msgDS = DataSetHelper.CXmlToDataSet(fileName);
        } catch (Exception ex) {
            Log.Error(typeof(ProgramSettingHelper), ex);
            throw new LdException("程序出现错误请联系管理员");
        }



        LogoImage = (String)(msgDS.Tables["Application"].Rows[0]["LogoImage"]);

        LogoName = (String)(msgDS.Tables["Application"].Rows[0]["LogoName"]);
        if (MainProcessButtonList != null && MainProcessButtonList.Count > 0) {
            MainProcessButtonList.Clear();
        }

        foreach (DataRow dr in msgDS.Tables["ProcessButton"].Rows) {
            Boolean isDfault = false;
            Boolean isVisible = true;
            if ("T".Equals(dr["IsDefault"].ToString())) {
                isDfault = true;
            }
            if ("F".Equals(dr["ISVisible"].ToString())) {
                isVisible = false;
            }
            MainProcessButtonList.Add(new ButtonImageCollection() {
                Text = dr["Text"].ToString(),
                Url = dr["Url"].ToString(),
                IsDefualt = isDfault,
                ISVisible=isVisible,
                Name = dr["Name"].ToString(),
                LogoPath= new BitmapImage(new Uri(@"pack://application:,,,/ResourceDictionary;Component" + dr["LogoPath"].ToString(), UriKind.RelativeOrAbsolute)),
                UnRead= dr["UnRead"].ToString().ToBool(),
                CheckLogoPath = new BitmapImage(new Uri(@"pack://application:,,,/ResourceDictionary;Component" + dr["CheckLogoPath"].ToString(), UriKind.RelativeOrAbsolute))
            });

            if ("T".Equals(dr["IsDefault"].ToString())) {
                MainDefaultSelected = dr["Text"].ToString();
            }
        }

        Version30 = msgDS.Tables["Application"].Rows[0]["Version30"].ToStr().ToBool();
        VideoKey = msgDS.Tables["Application"].Rows[0]["VideoKey"].ToStr();
        Host = (String)(msgDS.Tables["Application"].Rows[0]["Host"]);
        Int64.TryParse(msgDS.Tables["Application"].Rows[0]["Port"] as String, out m_port);


    }
}


}
