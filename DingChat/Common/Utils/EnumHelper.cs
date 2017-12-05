using System;
using System.ComponentModel;
using System.Reflection;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class EnumHelper {
    public static String getDescription(Type enumType, Object enumValue) {
        FieldInfo EnumInfo = enumType.GetField(enumValue.ToString());

        DescriptionAttribute[] EnumAttributes =
            (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (EnumAttributes.Length > 0) {
            return EnumAttributes[0].Description;
        }
        return String.Empty;
    }

    public static Object getTypeByDescription(Type enumType, String description) {
        foreach (FieldInfo fieldInfo in enumType.GetFields()) {
            DescriptionAttribute descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null && descriptionAttribute.Description.Equals(description)) {
                Object enumObj = Activator.CreateInstance(enumType);
                return fieldInfo.GetValue(enumObj);
            }
        }
        return null;
    }

    public static Object getDefault(Type enumType, Object obj) {
        foreach (FieldInfo fieldInfo in enumType.GetFields()) {
            DefaultAttribute defaultAttribute = fieldInfo.GetCustomAttribute<DefaultAttribute>();
            if (defaultAttribute != null) {
                Object enumObj = Activator.CreateInstance(enumType);
                return fieldInfo.GetValue(enumObj);
            }
        }
        return null;
    }

    public static Object getDefault(Type enumType) {
        foreach (FieldInfo fieldInfo in enumType.GetFields()) {
            DefaultAttribute defaultAttribute = fieldInfo.GetCustomAttribute<DefaultAttribute>();
            if (defaultAttribute != null) {
                Object enumObj = Activator.CreateInstance(enumType);
                return fieldInfo.GetValue(enumObj);
            }
        }
        return null;
    }
}
}
