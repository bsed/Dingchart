using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Enums {


public static class GetDescription {
    /// <summary>
    /// 获取描述信息
    /// </summary>
    /// <param name="en"></param>
    /// <returns></returns>
    public static string description(this Enum en) {
        Type type = en.GetType();
        MemberInfo[] memInfo = type.GetMember(en.ToString());
        if (memInfo != null && memInfo.Length > 0) {
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return ((DescriptionAttribute)attrs[0]).Description;
        }
        return en.ToString();
    }
}

public enum Sex {
    [Description("男")]
    male = 10031001,
    [Description("女")]
    female = 10031002,
    [Description("未知")]
    unknown = 10031003
}





}