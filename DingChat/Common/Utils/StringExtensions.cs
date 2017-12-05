using System;
using System.Collections.Generic;
using System.Globalization;
using cn.lds.chatcore.pcw.Common.Utils;



namespace cn.lds.chatcore.pcw.Common.Utils {

/// <summary>
/// 功　　能：字符串扩展方法类
/// 版　　本：V1.00
/// 日　　期：2010-06-03
///
/// </summary>
public static class StringExtensions {

    #region 截取操作

    /// <summary> 截取右子串
    /// </summary>
    /// <param Name="val">字符串</param>
    /// <param Name="len">截取长度</param>
    /// <returns>截取后字符串</returns>
    public static string Right(this string val, int len) {
        if (val.Length < len)
            len = val.Length;
        return val == "" ? "" : val.Substring(val.Length - len, len);
    }

    /// <summary> 截取左子串
    /// </summary>
    /// <param Name="val">字符串</param>
    /// <param Name="len">截取长度</param>
    /// <returns>截取后字符串</returns>
    public static string Left(this string val, int len) {
        if (val.Length < len) {
            len = val.Length;
        }

        return val == String.Empty ? String.Empty : val.Substring(0, len);
    }

    /// <summary> 取字符串中某个字符的左侧部分
    /// </summary>
    /// <param Name="val">源字符串</param>
    /// <param Name="c">字符</param>
    /// <returns>字符串中某个字符的左侧部分</returns>
    public static string LeftOf(this string val,char c) {
        return val.Split(c)[0];
    }

    #endregion

    /// <summary> 判断两个字符串是否相同（不区分大小写）
    /// </summary>
    /// <param Name="str1">第一个字符串</param>
    /// <param Name="str2">第二个字符串</param>
    /// <returns>bool值</returns>
    public static bool SameAs(this string str1, string str2) {
        return String.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
    }


    /// <summary> 将字符串翻转
    /// </summary>
    /// <param Name="srcStr"></param>
    /// <returns></returns>
    public static string Reverse(this string srcStr) {
        char[] cs = srcStr.ToCharArray();

        Array.Reverse(cs);

        return new string(cs);
    }

    /// <summary> 替换字串，不区分大小写
    /// </summary>
    /// <param Name="strOrg">原字符串</param>
    /// <param Name="valOld">被替换的旧字符串</param>
    /// <param Name="valNew">替换成的新字符串</param>
    /// <returns></returns>
    public static string ReplaceEx(this string strOrg, string valOld, string valNew) {
        if (valOld.Length == 0 || valOld == valNew) {
            return strOrg;
        }

        int start = 0;

        do {
            start = strOrg.IndexOf(valOld, start, StringComparison.CurrentCultureIgnoreCase);
            if (start >= 0) {
                strOrg = strOrg.Remove(start, valOld.Length);
                strOrg = strOrg.Insert(start, valNew);
                start += valNew.Length;
            }
        } while (start < strOrg.Length && start >= 0);

        return strOrg;
    }

    #region 字符串转换整型

    /// <summary> 将整数的字符串形式转换为Int值
    /// </summary>
    /// <param Name="strInt">整数的字符串形式</param>
    /// <returns>Int值</returns>
    public static int ToInt(this string strInt) {
        return ToInt(strInt, default(int));
    }

    /// <summary> 将整数的字符串形式转换为Int值
    /// </summary>
    /// <param Name="strInt">整数的字符串形式</param>
    /// <param Name="defaultValue">缺省值</param>
    /// <returns>Int值</returns>
    public static int ToInt(this string strInt, int defaultValue) {
        int r;

        if (!Int32.TryParse(strInt, out r)) {
            r = defaultValue;
        }
        return r;
    }

    /// <summary> 将整数的字符串形式转换为Int64值
    /// </summary>
    /// <param Name="strInt">整数的字符串形式</param>
    /// <returns>Int64值</returns>
    public static Int64 ToInt64(this string strInt) {
        return ToInt64(strInt, default(Int64));
    }

    /// <summary> 将整数的字符串形式转换为Int64值
    /// </summary>
    /// <param Name="strInt">整数的字符串形式</param>
    /// <param Name="defaultValue">缺省值</param>
    /// <returns>Int64值</returns>
    public static Int64 ToInt64(this string strInt, Int64 defaultValue) {
        Int64 r;

        if (!Int64.TryParse(strInt, out r)) {
            r = defaultValue;
        }
        return r;
    }


    #endregion

    #region 字符串转换布尔型

    /// <summary> 将布尔型字符串形式转换为bool值
    /// </summary>
    /// <param Name="strBool">布尔型字符串</param>
    /// <returns>bool值</returns>
    public static bool ToBool(this string strBool) {
        return ToBool(strBool, false);
    }

    /// <summary> 将布尔型字符串形式转换为bool值
    /// </summary>
    /// <param Name="strBool">布尔型字符串</param>
    /// <param Name="defaultValue">缺省值</param>
    /// <returns>bool值</returns>
    public static bool ToBool(this string strBool, bool defaultValue) {

        if (strBool == "0") {
            return false;
        }

        if (strBool == "1") {
            return true;
        }

        if (strBool.SameAs("true")) {
            return true;
        }

        bool r;

        if (!Boolean.TryParse(strBool, out r)) {
            int t;

            if (!Int32.TryParse(strBool, out t)) {
                r = defaultValue;
            } else {
                r = (t != 0);
            }
        }

        return r;
    }

    /// <summary> 将Y/N串形式转换为bool值
    /// </summary>
    /// <param Name="strYesNo">Y/N字符串</param>
    /// <param Name="defaultValue">缺省值</param>
    /// <returns>bool值</returns>
    public static bool YesNo2Bool(this string strYesNo, bool defaultValue) {
        return strYesNo.SameAs("Y");
    }

    #endregion


    /// <summary> 转化为日期类型[返回可空] 可以自动识别当前语言设置下的所有日期字串形式
    /// </summary>
    /// <param Name="val">字符串</param>
    /// <returns></returns>
    public static DateTime? ToDateTimeN(this string val) {
        if (String.IsNullOrEmpty(val)) {
            return null;
        }

        try {
            return Convert.ToDateTime(val);
        } catch(Exception e) {
            Log.Error(typeof(StringExtensions), e);
            return DateTimeExtensions.ValidDateTimeMin;
        }
    }

    /// <summary> 转化为日期类型( 可以自动识别当前语言设置下的所有日期字串形式 )
    /// </summary>
    /// <param Name="val">字符串</param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string val) {
        if (String.IsNullOrEmpty(val)) {
            return DateTimeExtensions.ValidDateTimeMin;
        }

        try {
            return Convert.ToDateTime(val);
        } catch(Exception e) {
            Log.Error(typeof(StringExtensions), e);
            return DateTimeExtensions.ValidDateTimeMin;
        }
    }

    /// <summary> 转化为Decimal数值
    /// </summary>
    /// <param Name="num">待转换的字符串</param>
    /// <returns></returns>
    public static decimal ToDec(this string num) {
        return ToDec(num, 0);
    }

    /// <summary> 转化为Decimal数值
    /// </summary>
    /// <param Name="num">待转换的字符串</param>
    /// <param Name="defaultValue">默认值</param>
    /// <returns></returns>
    public static decimal ToDec(this string num, decimal defaultValue) {
        decimal d;

        if (!Decimal.TryParse(num, out d)) {
            d = defaultValue;
        }

        return d;
    }

    /// <summary> 转化为float数值
    /// </summary>
    /// <param Name="num">待转换的字符串</param>
    /// <returns></returns>
    public static float ToFloat(this string num) {
        return ToFloat(num, 0);
    }

    /// <summary> 转化为float数值
    /// </summary>
    /// <param Name="num">待转换的字符串</param>
    /// <param Name="defaultValue">默认值</param>
    /// <returns></returns>
    public static float ToFloat(this string num, float defaultValue) {
        float f;

        if (!float.TryParse(num, out f)) {
            f = defaultValue;
        }

        return f;
    }


    /// <summary> 将Double的字符串形式转换为Double值
    /// </summary>
    /// <param Name="strDouble">Double的字符串形式</param>
    /// <returns>Double值</returns>
    public static double ToDouble(this string strDouble) {
        return ToDouble(strDouble, default(double));
    }

    /// <summary> 将Double的字符串形式转换为Double值
    /// </summary>
    /// <param Name="strDouble">Double的字符串形式</param>
    /// <param Name="defaultValue">缺省值</param>
    /// <returns>Double值</returns>
    public static double ToDouble(this string strDouble, double defaultValue) {
        double r;

        if (!double.TryParse(strDouble, out r)) {
            r = defaultValue;
        }
        return r;
    }




    /// <summary>将一个string[]转化为 List&lt;string&gt;</summary>
    /// <param Name="ss"></param>
    /// <returns></returns>
    public static List<string> StrArrayToList(this string[] ss) {
        List<string> li = new List<string>();
        foreach (var s in ss) {
            li.Add(s);
        }

        return li;
    }

    /// <summary> 将一个Unicode字符串分割成特定字节数的字串列表
    /// </summary>
    /// <param Name="str">要分割的字符串</param>
    /// <param Name="byteSplitCount">要分割的个子字符串单位的字节数</param>
    /// <returns></returns>
    public static List<string> ToSubStrList(this string str, int byteSplitCount) {
        List<string> li = new List<string>();

        var bs = System.Text.Encoding.Default.GetBytes(str);

        if (bs.LongLength <= byteSplitCount) {
            li.Add(str);

            return li;
        }

        int index = 0;


        while (index < bs.LongLength) {
            var leftLen = bs.LongLength - index;

            int len = byteSplitCount;

            if (byteSplitCount > leftLen) {
                len = (int)leftLen;
            }

            var s = System.Text.Encoding.Default.GetString(bs, index, len);

            var s1 = s.Right(1);

            if (s1 == "?") {
                len = len - 1;
                s = System.Text.Encoding.Default.GetString(bs, index, len);
            }


            index += len;

            li.Add(s);


        }

        return li;
    }
}
}
