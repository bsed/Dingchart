using System;
using System.Windows.Forms;


namespace cn.lds.chatcore.pcw.Common.Utils
// ReSharper restore CheckNamespace
{
/// <summary> 通用扩展方法类
/// </summary>
public static class Ext {
    #region 扩展方法

    /// <summary> 获取某个对象的HashCode值。[如果对象实例为null，则HashCode为常量-1]
    /// </summary>
    /// <param Name="obj"></param>
    /// <returns></returns>
    public static int GetHash(this object obj) {
        if (obj == null) {
            return -1;
        }

        return obj.GetHashCode();
    }

    /// <summary> 将某个对象转换为String。[支持对null的转换]
    /// </summary>
    /// <param Name="obj"></param>
    /// <returns></returns>
    public static string ToStr(this object obj) {
        return ToStr(obj, String.Empty);
    }

    /// <summary> 将某个对象转换为String。[支持对null的转换]
    /// </summary>
    /// <param Name="obj"></param>
    /// <param Name="defaultStrIfNull">如果obj为null，则返回的默认字串</param>
    /// <returns></returns>
    public static string ToStr(this object obj, string defaultStrIfNull) {
        if (obj == null) {
            return defaultStrIfNull;
        }

        return obj.ToString();
    }

    /// <summary> 将某个DateTime转换为String。[支持对null的转换]
    /// </summary>
    /// <param Name="dateTime"></param>
    /// <param Name="format">格式串 </param>
    /// <returns></returns>
    public static string ToStr(this DateTime? dateTime, string format) {
        if (dateTime == null) {
            return DateTimeExtensions.ValidDateTimeMin.ToString(format);
        }

        return dateTime.Value.ToString(format);
    }

    /// <summary> 将某个DateTime转换为String。[支持对null的转换]
    /// </summary>
    /// <param Name="dateTime"></param>
    /// <returns></returns>
    public static string ToStr(this DateTime? dateTime) {
        return ToStr(dateTime, "yyyy-MM-dd HH:mm:ss");
    }

    #endregion

    /// <summary> 判断字符是数字或字母
    /// </summary>
    /// <param Name="chr"></param>
    /// <returns></returns>
    public static bool IsNumOrLetter(this char chr) {
        return chr >= 'A' && chr <= 'Z' || chr >= '0' && chr <= '9' || chr >= 'a' && chr <= 'z';
    }

    /// <summary> 判断字符是数字或字母
    /// </summary>
    /// <param Name="keyE"></param>
    /// <returns></returns>
    public static bool IsNumInput(this KeyEventArgs keyE) {
        int keyvalue = keyE.KeyValue;

        return keyvalue >= 96 && keyvalue <= 105 || keyvalue >= 48 && keyvalue <= 57;
    }

    /// <summary> 判断字符是数字或字母
    /// </summary>
    /// <param Name="keyE"></param>
    /// <param Name="canBeSign">可以输入有符号 </param>
    /// <returns></returns>
    public static bool IsNumInput(this KeyEventArgs keyE, bool canBeSign) {
        int keyvalue = keyE.KeyValue;

        return keyvalue >= 96 && keyvalue <= 105 || keyvalue >= 48 && keyvalue <= 57 || canBeSign && keyvalue == '-';
    }

    /// <summary> 判断是否修改键，比如箭头光标、删除、回退等
    /// </summary>
    /// <param Name="keyE"></param>
    /// <returns></returns>
    public static bool IsModKeyInput(this KeyEventArgs keyE) {
        if (
            keyE.KeyCode == Keys.Back || keyE.KeyCode == Keys.Delete ||
            keyE.KeyCode == Keys.Left || keyE.KeyCode == Keys.Right ||
            keyE.KeyCode == Keys.Up || keyE.KeyCode == Keys.Down
            || keyE.KeyCode == Keys.Enter) {
            return true;
        }

        return false;
    }


    /// <summary> 判断字符是数字或字母
    /// </summary>
    /// <param Name="keyE"></param>
    /// <returns></returns>
    public static bool IsNumOrLetterInput(this KeyEventArgs keyE) {
        int keyvalue = keyE.KeyValue;

        return keyvalue >= 96 && keyvalue <= 105 //小键盘数字
               || keyvalue >= 48 && keyvalue <= 57 //主键盘数字
               || keyvalue >= 'A' && keyvalue <= 'Z'; //主键盘字母
    }

    /// <summary>
    /// 将输入小键盘数字码值转化为正常Char
    /// </summary>
    /// <param Name="keyE"></param>
    /// <returns></returns>
    public static char GetInputChar(this KeyEventArgs keyE) {
        int keyvalue = keyE.KeyValue;

        if (keyvalue >= 96 && keyvalue <= 105) { //小键盘数字
            keyvalue = keyvalue - (96 - 48);
        }

        return (char)keyvalue;
    }

    /// <summary> 检测字符是否是空，是空串，是空白串。
    /// </summary>
    /// <param Name="str">待检测的字符串</param>
    /// <returns>TRUE则为空,FALSE为非空</returns>
    public static bool IsNullOrBlank(this string str) {
        if (String.IsNullOrEmpty(str)) {
            return true;
        }

        return str.Trim().Length == 0;
    }

    /// <summary> 检测字符是否是空或是空串
    /// </summary>
    /// <param Name="str">待检测的字符串</param>
    /// <returns>TRUE则为空,FALSE为非空</returns>
    public static bool IsNullOrEmpty(this string str) {
        return String.IsNullOrEmpty(str);
    }

    ///// <summary>// 以字符串自身实例为格式串，参数化一个新字串。
    ///// </summary>
    ///// <param Name="str"></param>
    ///// <param Name="args"></param>
    ///// <returns></returns>
    //public static string FormatSelf(this string str, params object[] args)
    //{
    //    return string.Format(str, args);
    //}

    /// <summary>将DataTime类型转换成string类型</summary>
    /// <param Name="date"></param>
    /// <returns></returns>
    public static string ToStr(this DateTime date) {
        return date.ToString();
    }
}
}