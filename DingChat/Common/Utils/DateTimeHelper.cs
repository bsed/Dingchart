using System;
using cn.lds.chatcore.pcw.Common.Enums;
using java.util;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class DateTimeHelper {

    public static DateTime getDate(string timeStamp) {
        //return new Date(timeLong);
        if (timeStamp == "0") {
            return DateTime.Now;
        }
        DateTime dateTimeStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp.Substring(0, 10) + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dateTimeStart.Add(toNow);


    }


    /// <summary>
    /// 获取日期
    /// </summary>
    /// <param Name="Timestamp"></param>
    /// <returns></returns>
    public static string ChartGetDate(string timestamp) {
        try {
            if (timestamp == string.Empty || timestamp == "0") return string.Empty;
            DateTime time = getDate(timestamp);
            if (time.Date == DateTime.Now.Date) {
                return time.ToString("HH:mm");
            } else {
                return time.ToString("MM-dd ");
            }
        } catch (Exception ex) {
            Log.Error(typeof(DateTimeHelper), ex);
        }
        return "";
    }

    public static Int64 getLong(DateTime time) {
        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (long)(time - startTime).TotalMilliseconds;
    }





    /// <summary>
    /// 获取客户端当前时间的时间戳
    /// </summary>
    /// <returns></returns>
    public static long getTimeStamp() {
        //// todo时间戳可能有问题
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }

    /// <summary>
    /// 获取客户端指定时间的时间戳
    /// </summary>
    /// <returns></returns>
    public static long getTimeStamp(DateTime dateTime) {
        //// todo时间戳可能有问题
        TimeSpan ts = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }

    /// <summary>
    /// 计算时间差
    /// </summary>
    /// <param Name="DateTime1"></param>
    /// <param Name="DateTime2"></param>
    /// <returns></returns>
    public static TimeSpan DateDiff(DateTime DateTime1, DateTime DateTime2) {
        TimeSpan ts = DateTime1 - DateTime2;
        //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        //TimeSpan ts2 = new
        //TimeSpan(DateTime2.Ticks);
        //TimeSpan ts = ts1.Subtract(ts2).Duration();
        //dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
        return ts;
    }

    /// <summary>
    /// 计算时间差
    /// </summary>
    /// <param Name="DateTime1"></param>
    /// <param Name="DateTime2"></param>
    /// <returns></returns>
    public static TimeSpan DateDiff(long DateTimeStamp1, long DateTimeStamp2) {
        DateTime DateTime1 = getDate(DateTimeStamp1.ToStr());
        DateTime DateTime2 = getDate(DateTimeStamp2.ToStr());
        return DateDiff(DateTime1, DateTime2);
    }

    /// <summary>
    /// 格式化时间显示
    /// </summary>
    /// <param Name="dateTime"></param>
    /// <param Name="type"></param>
    /// <returns></returns>
    public static String getFormateDateString(DateTime dateTime,DateTimeType type) {
        return dateTime.ToString(EnumHelper.getDescription(typeof(DateTimeType), type));
    }

    /// <summary>
    /// 格式化时间显示（当前系统时间）
    /// </summary>
    /// <param Name="type"></param>
    /// <returns></returns>
    public static String getFormateDateString(DateTimeType type) {
        return getFormateDateString(DateTime.Now,type);
    }

/// <summary>
/// 检查时间间隔
/// </summary>
/// <param name="timestamp"></param>
/// <param name="minutes"></param>
/// <returns></returns>
    public static Boolean CheckTimestampDiffByMinutes(long timestamp,int minutes) {
        try {
            TimeSpan ts = DateTime.UtcNow - DateTimeHelper.getDate(timestamp.ToStr());
            if (ts.Minutes > minutes) {
                return false;
            }
            return true;
        } catch (Exception e) {
            Log.Error(typeof(DateTimeHelper), e);
        }
        return false;
    }
}
}
