using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cn.lds.chatcore.pcw.Common.Utils {
public   class CommonMethod {
    /// <summary>
    /// 获取单个子元素
    /// </summary>
    /// <typeparam Name="T">子元素类型</typeparam>
    /// <param Name="obj">父元素</param>
    /// <param Name="name">子元素名称</param>
    /// <returns></returns>
    public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement {
        DependencyObject child = null;
        T grandChild = null;

        for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++) {
            child = VisualTreeHelper.GetChild(obj, i);


            if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name))) {
                return (T)child;
            } else {
                grandChild = GetChildObject<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据父控件对象查找指定类型子控件
    /// </summary>
    /// <typeparam Name="T"></typeparam>
    /// <param Name="parent"></param>
    /// <returns></returns>
    public static T GetVisualChild<T>(DependencyObject parent) where T : Visual {
        T child = default(T);
        int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < numVisuals; i++) {
            Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
            child = v as T;
            if (child == null) {
                child = GetVisualChild<T>(v);
            }
            if (child != null) {
                break;
            }
        }
        return child;
    }

    public static string GetValidCode(int codeNum, CommonValidCode.CodeType type, Image image) {
        CommonValidCode validCode = new CommonValidCode();
        validCode.ValidCode(codeNum, type);
        image.Source = BitmapFrame.Create(validCode.CreateCheckCodeImage());
        return validCode.CheckCode;
    }

    ///
    public static void SetBookshelfList(int codeNum, CommonValidCode.CodeType type, Image image) {

    }





}
}
