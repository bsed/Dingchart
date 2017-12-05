using System;

namespace cn.lds.chatcore.pcw.Common.Utils
// ReSharper restore CheckNamespace
{
/// <summary>
/// </summary>
public static class BooleanExtensions {
    /// <summary> ת��Ϊ0��1������
    /// </summary>
    /// <param Name="val">Ҫת����ֵ</param>
    /// <returns>ת���õ�������</returns>
    public static int ToInt(this bool val) {
        return val ? 1 : 0;
    }

    /// <summary> ת��Ϊ"Y"��"N"���ַ�
    /// </summary>
    /// <param Name="val">Ҫת����ֵ</param>
    /// <returns>ת���õ����ַ���</returns>
    public static string ToYesNo(this bool val) {
        return val ? "Y" : "N";
    }
}
}