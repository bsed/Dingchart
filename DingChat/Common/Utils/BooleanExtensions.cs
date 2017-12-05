using System;

namespace cn.lds.chatcore.pcw.Common.Utils
// ReSharper restore CheckNamespace
{
/// <summary>
/// </summary>
public static class BooleanExtensions {
    /// <summary> 转化为0和1的整数
    /// </summary>
    /// <param Name="val">要转换的值</param>
    /// <returns>转换得到的整数</returns>
    public static int ToInt(this bool val) {
        return val ? 1 : 0;
    }

    /// <summary> 转化为"Y"和"N"的字符
    /// </summary>
    /// <param Name="val">要转换的值</param>
    /// <returns>转换得到的字符串</returns>
    public static string ToYesNo(this bool val) {
        return val ? "Y" : "N";
    }
}
}