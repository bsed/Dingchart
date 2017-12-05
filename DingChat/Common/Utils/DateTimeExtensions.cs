
using System;
using cn.lds.chatcore.pcw.Common.Utils;


namespace cn.lds.chatcore.pcw.Common.Utils {
/// <summary>
/// </summary>
public static class DateTimeExtensions {
    /// <summary> 转换为数据库的字符串
    /// </summary>
    /// <param Name="val">待转换的DateTime值</param>
    /// <returns></returns>
    public static string ToDBString(this DateTime? val) {
        if (val == null) {
            return "null";
        }

        return string.Format("'{0}'", val.ToStr());
    }

    /// <summary>转换为数据库的字符串
    /// </summary>
    /// <param Name="val">待转换的DateTime值</param>
    /// <returns></returns>
    public static string ToDBString(this DateTime val) {
        return string.Format("'{0}'", val.ToStr());
    }
    /// <summary> 本应用系统运行期间约定的最小有效日期 [可以在系统加载时设置上]
    /// </summary>
    public static DateTime ValidDateTimeMin = new DateTime(1900, 1, 1);

    /// <summary>  本应用系统运行期间约定的最大有效日期 [可以在系统加载时设置上]
    /// </summary>
    public static DateTime ValidDateTimeMax = new DateTime(2900, 1, 1);

    /// <summary>  是否有效的日期
    /// </summary>
    /// <param Name="datetime"></param>
    /// <returns></returns>
    public static bool IsValidDateTime(this DateTime datetime) {
        if (datetime < ValidDateTimeMin) {
            return false;
        }

        if (datetime > ValidDateTimeMax) {
            return false;
        }

        return true;
    }
}
}