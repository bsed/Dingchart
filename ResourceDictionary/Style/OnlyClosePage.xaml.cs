using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace ldcoacommon.Views.Style {
public partial class OnlyClosePage {
    // 拖动
    private void CustomPage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseEventArgs e) {
        Window win = (Window)((FrameworkElement)sender).TemplatedParent;
        if (e.LeftButton == MouseButtonState.Pressed) {
            win.DragMove();
        }
    }


    // 关闭
    private void CustomPageBtnClose_Click(object sender, RoutedEventArgs e) {
        Window win = (Window)((FrameworkElement)sender).TemplatedParent;
        win.Close();
    }

    // 最小化
    private void CustomPageBtnMinimized_Click(object sender, RoutedEventArgs e) {
        Window win = (Window)((FrameworkElement)sender).TemplatedParent;
        win.WindowState = WindowState.Minimized;
    }
    /// <summary>
    /// 获得窗口任务栏尺寸
    /// </summary>
    /// <returns></returns>
    public Size getCurTaskbarSize() {
        int width = 0, height = 0;

        if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0)) {
            //taskbar bottom
            width = Screen.PrimaryScreen.WorkingArea.Width;
            height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X == 0)) {
            //taskbar right
            width = Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.WorkingArea.Width;
            height = Screen.PrimaryScreen.WorkingArea.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                   (Screen.PrimaryScreen.WorkingArea.Y > 0)) {
            //taskbar up
            width = Screen.PrimaryScreen.WorkingArea.Width;
            //height = Screen.PrimaryScreen.WorkingArea.Y;
            height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X > 0)) {
            //taskbar left
            width = Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.WorkingArea.Width;
            height = Screen.PrimaryScreen.WorkingArea.Height;
        }

        return new Size(width, height);
    }

    /// <summary>
    /// 获得最大化时候窗口坐标
    /// </summary>
    /// <param name="windowSize"></param>
    /// <returns></returns>
    public Point getLocation(Size windowSize) {
        double xPos = 0, yPos = 0;

        if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0)) {
            //taskbar bottom
            xPos = 0;
            yPos = 0;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X == 0)) {
            //taskbar right
            xPos = 0;
            yPos = 0;
        } else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                   (Screen.PrimaryScreen.WorkingArea.Y > 0)) {
            //taskbar up
            xPos = 0;
            yPos = windowSize.Height;
        } else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                   (Screen.PrimaryScreen.WorkingArea.X > 0)) {
            //taskbar left
            xPos = windowSize.Width;
            yPos = 0;
        }

        return new Point(xPos, yPos);
    }

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
    private System.Windows.Controls.Button tomax = null;
    private System.Windows.Controls.Button frommax = null;

    private double winWidth = 0;
    private double winHeight = 0;
    private double left = 0;
    private double top = 0;
    private void tomax_Click(object sender, RoutedEventArgs e) {
        Window win = (Window)((FrameworkElement)sender).TemplatedParent;
        frommax = GetChildObject<System.Windows.Controls.Button>(win, "frommax");
        tomax = GetChildObject<System.Windows.Controls.Button>(win, "tomax");
        winWidth = win.Width;
        winHeight = win.Height;
        left = win.Left;
        top = win.Top;
        win.WindowState = System.Windows.WindowState.Normal;
        Rect rc = SystemParameters.WorkArea;//获取工作区大小
        Size curTaskbarSize = getCurTaskbarSize();
        Point p = getLocation(curTaskbarSize);
        System.Drawing.Rectangle taskbarRect = Screen.PrimaryScreen.WorkingArea;
        win.Left = p.X;//设置位置
        win.Top = p.Y;
        win.Width = rc.Width;
        win.Height = rc.Height;
        tomax.Visibility = Visibility.Collapsed;
        frommax.Visibility = Visibility.Visible;

    }
    private void frommax_Click(object sender, RoutedEventArgs e) {
        Window win = (Window)((FrameworkElement)sender).TemplatedParent;
        frommax = GetChildObject<System.Windows.Controls.Button>(win, "frommax");
        tomax = GetChildObject<System.Windows.Controls.Button>(win, "tomax");
        win.WindowState = System.Windows.WindowState.Normal;
        win.Width = winWidth;
        win.Height = winHeight;
        win.Left = left;//设置位置
        win.Top = top;
        win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        tomax.Visibility = Visibility.Visible;
        frommax.Visibility = Visibility.Collapsed;
    }
}
}
