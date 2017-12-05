
using System;
using cn.lds.chatcore.pcw.Common.Utils;


namespace cn.lds.chatcore.pcw.Common.Utils {
/// <summary>
/// </summary>
public static class DateTimeExtensions {
    /// <summary> ת��Ϊ���ݿ���ַ���
    /// </summary>
    /// <param Name="val">��ת����DateTimeֵ</param>
    /// <returns></returns>
    public static string ToDBString(this DateTime? val) {
        if (val == null) {
            return "null";
        }

        return string.Format("'{0}'", val.ToStr());
    }

    /// <summary>ת��Ϊ���ݿ���ַ���
    /// </summary>
    /// <param Name="val">��ת����DateTimeֵ</param>
    /// <returns></returns>
    public static string ToDBString(this DateTime val) {
        return string.Format("'{0}'", val.ToStr());
    }
    /// <summary> ��Ӧ��ϵͳ�����ڼ�Լ������С��Ч���� [������ϵͳ����ʱ������]
    /// </summary>
    public static DateTime ValidDateTimeMin = new DateTime(1900, 1, 1);

    /// <summary>  ��Ӧ��ϵͳ�����ڼ�Լ���������Ч���� [������ϵͳ����ʱ������]
    /// </summary>
    public static DateTime ValidDateTimeMax = new DateTime(2900, 1, 1);

    /// <summary>  �Ƿ���Ч������
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