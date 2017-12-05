using System;
using System.Reflection;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class ModuleHelper {
    /// <summary>
    /// 把源对象里的各个字段的内容直接赋值给目标对象（只是字段复制，两个对象的字段名和类型都必须一致）
    /// </summary>
    /// <param Name="dest">目标对象</param>
    /// <param Name="src">源对象</param>
    public static void CopyValues(object dest, object src) {
        if (null == src || null == dest) {
            return;
        }

        Type srcType = src.GetType();
        Type destType = dest.GetType();

        //FieldInfo[] srcInfo = srcType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        //FieldInfo[] destInfo = destType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        //for (int i = 0; i < srcInfo.Length; i++)
        //{
        //    string Name = srcInfo[i].Name;
        //    object val = srcInfo[i].GetValue(src);

        //    for (int j = 0; j < destInfo.Length; j++)
        //    {
        //        string targetName = destInfo[j].Name;

        //        if (Name.Equals(targetName))
        //        {
        //            destInfo[j].SetValue(dest, val);
        //            break;
        //        }
        //    }
        //}
        PropertyInfo[] srcPropertyInfo = srcType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        PropertyInfo[] destPropertyInfo = destType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        for (int i = 0; i < srcPropertyInfo.Length; i++) {
            string name = srcPropertyInfo[i].Name;
            object val = srcPropertyInfo[i].GetValue(src);

            for (int j = 0; j < destPropertyInfo.Length; j++) {
                string targetName = destPropertyInfo[j].Name;

                if (name.Equals(targetName)) {
                    destPropertyInfo[j].SetValue(dest, val);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 把源对象里的各个字段的内容直接赋值给目标对象（只复制[ColumnArrtibute]中ColumnName相同的属性值）
    /// </summary>
    /// <param Name="dest">目标对象</param>
    /// <param Name="src">源对象</param>
    public static void CopyValuesByColumnAttribute(object dest, object src) {
        if (null == src || null == dest) {
            return;
        }

        Type srcType = src.GetType();
        Type destType = dest.GetType();

        PropertyInfo[] srcInfo = srcType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        PropertyInfo[] destInfo = destType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        for (int i = 0; i < srcInfo.Length; i++) {
            string name = srcInfo[i].Name;
            object val = srcInfo[i].GetValue(src);

            ColumnAttribute srcColumnInfo = srcInfo[i].GetCustomAttribute<ColumnAttribute>();
            if (srcColumnInfo == null ) {
                continue;
            }

            for (int j = 0; j < destInfo.Length; j++) {
                string targetName = destInfo[j].Name;

                ColumnAttribute destColumnInfo = destInfo[j].GetCustomAttribute<ColumnAttribute>();
                if (destColumnInfo == null) {
                    continue;
                }

                if (srcColumnInfo.ColumnName.Equals(destColumnInfo.ColumnName)) {
                    destInfo[j].SetValue(dest, val);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 把源对象里的各个字段的内容直接赋值给目标对象（只复制[ColumnArrtibute]中ColumnName相同的属性值）
    /// </summary>
    /// <param Name="dest">目标对象</param>
    /// <param Name="src">源对象</param>
    /// <param Name="exceptFields">复制对象外的ColumnName名</param>
    public static void CopyValuesByColumnAttribute(object dest, object src, String[] exceptPropertys) {
        if (null == src || null == dest) {
            return;
        }

        Type srcType = src.GetType();
        Type destType = dest.GetType();

        PropertyInfo[] srcInfo = srcType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        PropertyInfo[] destInfo = destType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        for (int i = 0; i < srcInfo.Length; i++) {
            string name = srcInfo[i].Name;
            object val = srcInfo[i].GetValue(src);

            ColumnAttribute srcColumnInfo = srcInfo[i].GetCustomAttribute<ColumnAttribute>();
            if (srcColumnInfo == null) {
                continue;
            }

            Boolean exceptFlag = false;
            if (exceptPropertys != null && exceptPropertys.Length > 0) {
                foreach (String exceptFieldName in exceptPropertys) {
                    if (srcColumnInfo.ColumnName.Equals(exceptFieldName)) {
                        exceptFlag = true;
                        break;
                    }
                }
            }
            if (exceptFlag) {
                continue;
            }

            for (int j = 0; j < destInfo.Length; j++) {
                string targetName = destInfo[j].Name;

                ColumnAttribute destColumnInfo = destInfo[j].GetCustomAttribute<ColumnAttribute>();
                if (destColumnInfo == null) {
                    continue;
                }

                if (srcColumnInfo.ColumnName.Equals(destColumnInfo.ColumnName)) {
                    destInfo[j].SetValue(dest, val);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 从一个对象里复制属性到另一个对象的同名属性
    /// </summary>
    /// <param Name="dest">目标对象</param>
    /// <param Name="src">源对象</param>
    /// <param Name="fieldName">要复制的属性名字</param>
    public static void CopyProperty(ref object dest, object src, string fieldName) {
        if (null == src || null == dest || null == fieldName) {
            return;
        }
        Type srcType = src.GetType();
        Type destType = dest.GetType();
        PropertyInfo srcInfo = srcType.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        PropertyInfo destInfo = destType.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        object val = srcInfo.GetValue(src);
        destInfo.SetValue(dest, val);

    }


    /// <summary>
    /// 用于设置对象的属性值
    /// </summary>
    /// <param Name="dest">目标对象</param>
    /// <param Name="fieldName">属性名字</param>
    /// <param Name="value">属性里要设置的值</param>
    public static void setProperty(ref object dest, string fieldName, object value) {
        if (null == dest || null == fieldName || null == value) {
            return;
        }
        Type t = dest.GetType();
        FieldInfo f = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        f.SetValue(dest, value);
    }


}
}
