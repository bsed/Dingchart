using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace PictureViewer {
/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }


    private double windowWidth;
    private double windowHeight;
    // 显示的图片列表
    private List<ImageBean> listImageBeans;
    private ImageBean currentImageBean;
    private int currentIndex;

    private bool mouseDown;
    private Point mouseXY;
    private double min = 0.1, max = 3.0;//最小/最大放大倍数
    private int RotateTime = 1;

    /// <summary>
    /// 鼠标移动
    /// </summary>
    /// <param name="img"></param>
    /// <param name="e"></param>
    private void Domousemove(ContentControl img, MouseEventArgs e) {
        try {
            if (e.LeftButton != MouseButtonState.Pressed) {
                return;
            }


            var group = IMG.FindResource("TfGroup") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            //只有放大的才能拖拽
            if (transform.X == 0) return;
            var point = new System.Windows.Point();
            point.X = this.GetWindowCenterX();
            point.Y = this.GetWindowCenterY();

            var position = e.GetPosition(img);
            //if (mouseXY.X - position.X < 10) return;
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    /// <summary>
    /// 放大&缩小（滚轮）
    /// </summary>
    /// <param name="group"></param>
    /// <param name="point"></param>
    /// <param name="delta"></param>
    private void DowheelZoom(TransformGroup group, Point point, double delta) {

        //ScaleTransform scaleTransform = new ScaleTransform();

        //scaleTransform.ScaleX = 0.1;              //宽度放大一倍
        //scaleTransform.ScaleY = 0.1;              //高度放大一倍
        //this.IMG.RenderTransform = scaleTransform;

        try {
            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;

            if (transform.ScaleX + delta < min) return;
            if (transform.ScaleX + delta > max) return;
            transform.ScaleX += delta;
            transform.ScaleY += delta;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }

    }

    /// <summary>
    /// 松开鼠标左键
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        try {
            Type senderType = sender.GetType();
            var img = sender as ContentControl;
            if (img == null) {
                return;
            }
            img.CaptureMouse();
            mouseDown = true;
            mouseXY = e.GetPosition(img);
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }

    }

    private void ContentControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        try {
            var img = sender as ContentControl;
            if (img == null) {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    private void ContentControl_MouseMove(object sender, MouseEventArgs e) {
        try {
            var img = sender as ContentControl;
            if (img == null) {
                return;
            }
            if (mouseDown) {
                Domousemove(img, e);
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    private void ContentControl_MouseWheel(object sender, MouseWheelEventArgs e) {
        try {
            //var img = sender as ContentControl;
            //if (img == null) {
            //    return;
            //}
            var point = new System.Windows.Point();
            point.X = this.GetWindowCenterX();
            point.Y = this.GetWindowCenterY();
            var group = IMG.FindResource("TfGroup") as TransformGroup;
            var delta = e.Delta * 0.001;
            DowheelZoom(group, point, delta);
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    private void OpenImg_Click(object sender, RoutedEventArgs e) {
        try {
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Files (*.png)|*.png|Files(*.jpg)|*.jpg";
            if (dialog.ShowDialog() == true) {
                //MessageBox.Show(dialog.FileName);
                //this.IMG.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }
    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSaveImg_Click(object sender, RoutedEventArgs e) {
        try {
            //创建一个保存文件式的对话框
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            //设置这个对话框的起始保存路径
            sfd.InitialDirectory = @"D:\";
            try {
                // 默认名字设置有问题，不能影响到保存功能
                sfd.FileName = this.currentImageBean.fileName.Substring(this.currentImageBean.fileName.LastIndexOf("\\") + 1);
            } catch (Exception ex1) {
                Console.WriteLine(ex1.StackTrace);
            }

            //设置保存的文件的类型，注意过滤器的语法
            sfd.Filter = "PNG图片|*.png|JPG图片|*.jpg|JPEG图片|*.jpeg|BMP图片|*.bmp|GIF图片|*.gif";
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            if (sfd.ShowDialog() == true) {
                String sourcePath = this.currentImageBean.localPath;
                String targetPath = sfd.FileName;

                Console.WriteLine("sourcePath=" + sourcePath);
                Console.WriteLine("targetPath=" + targetPath);

                //String targetForder = targetPath.Substring(0, targetPath.LastIndexOf("\\") + 1);
                //if (!System.IO.Directory.Exists(@targetPath)) {
                //    // 目录不存在，建立目录
                //    System.IO.Directory.CreateDirectory(@targetPath);
                //}
                //Console.WriteLine("targetForder=" + targetForder);

                System.IO.File.Copy(sourcePath, targetPath, true);
                //MessageBox.Show("保存成功");
            } else {
                //MessageBox.Show("取消保存");
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }
    /// <summary>
    /// 放大按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnZoomIn_Click(object sender, RoutedEventArgs e) {
        try {
            try {
                var img = sender as ContentControl;
                if (img == null) {
                    return;
                }
                var point = new System.Windows.Point();
                point.X = this.GetWindowCenterX();
                point.Y = this.GetWindowCenterY();
                var group = IMG.FindResource("TfGroup") as TransformGroup;
                var delta = 120 * 0.001;
                DowheelZoom(group, point, delta);

            } catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    private double GetWindowCenterY() {
        return this.ContentControl.ActualHeight/2;
    }
    private double GetWindowCenterX() {
        return this.ContentControl.ActualWidth / 2;
    }
    /// <summary>
    /// 缩小按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnZoomOut_Click(object sender, RoutedEventArgs e) {
        try {
            var img = sender as ContentControl;
            if (img == null) {
                return;
            }
            var point = new System.Windows.Point();
            point.X = this.GetWindowCenterX();
            point.Y = this.GetWindowCenterY();
            var group = IMG.FindResource("TfGroup") as TransformGroup;
            var delta = -120 * 0.001;
            DowheelZoom(group, point, delta);

        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }
    /// <summary>
    /// 旋转按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnRotate_Click(object sender, RoutedEventArgs e) {
        try {
            var group = IMG.FindResource("TfGroup") as TransformGroup;
            var rotateTransform = group.Children[2] as RotateTransform;

            rotateTransform.CenterX = this.GetWindowCenterX();
            rotateTransform.CenterY = this.GetWindowCenterY();
            rotateTransform.Angle = this.RotateTime*90;

            this.RotateTime++;
            if (this.RotateTime == 4) {
                this.RotateTime = 0;
            }


        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    /// <summary>
    /// 前一张
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnPre_Click(object sender, RoutedEventArgs e) {
        try {


            // 查看到最后一张，不处理
            if (this.currentIndex == this.listImageBeans.Count - 1) {

                return;
            }
            this.currentIndex++;
            this.currentImageBean = this.listImageBeans[this.currentIndex];
            this.ShowImage(this.currentImageBean);
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    /// <summary>
    /// 后一张
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnNext_Click(object sender, RoutedEventArgs e) {
        try {

            // 查看到第一张，不处理
            if (this.currentIndex == 0) {
                return;
            }
            this.currentIndex--;
            this.currentImageBean = this.listImageBeans[this.currentIndex];
            this.ShowImage(this.currentImageBean);

        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }

    }



    private void Window_Loaded(object sender, RoutedEventArgs e) {



        try {

            for (int i=0; i<this.listImageBeans.Count; i++) {
                ImageBean imageBean = this.listImageBeans[i];
                if (imageBean.imageStorageId == null) continue;
                if (imageBean.imageStorageId.Equals(this.currentImageBean.imageStorageId)) {
                    this.currentIndex = i;
                    break;
                }
            }
            this.ShowImage(this.currentImageBean);

            double imageW = IMG.Source.Width;
            double imageH = IMG.Source.Height;
            if (imageW < 388 && imageH < 335) {
                windowWidth = 400;
                windowHeight = 400;

            }

            else if (imageW > imageH) {
                windowWidth = 470;
                windowHeight = 400;
            } else if (imageH > imageW) {
                windowWidth = 400;
                windowHeight = 525;
            }
            this.Width = windowWidth;
            this.Height = windowHeight;

            setViewSize();
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }
    private void setViewSize() {
        //try {
        //    mainScrollv.Width = this.ActualWidth;
        //    mainScrollv.Height = this.ActualHeight - 50;
        //} catch (Exception ex) {
        //    Console.WriteLine(ex.StackTrace);
        //}
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
        try {
            setViewSize();
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
    }

    private void txtMinSize_TextChanged(object sender, TextChangedEventArgs e) {
        //this.min = double.Parse(txtMinSize.Text);
    }

    private void txtMaxSize_TextChanged(object sender, TextChangedEventArgs e) {
        //this.max = double.Parse(txtMaxSize.Text);
    }

    private void Window_DragEnter(object sender, DragEventArgs e) {
        //try {
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //        e.Effects = DragDropEffects.Link;//WinForm中为e.Effect = DragDropEffects.Link
        //    else e.Effects = DragDropEffects.None;//WinFrom中为e.Effect = DragDropEffects.None
        //} catch (Exception ex) {
        //    Console.WriteLine(ex.StackTrace);
        //}

    }

    private void Window_Drop(object sender, DragEventArgs e) {
        //try {
        //    string filename = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        //    this.IMG.Source = new BitmapImage(new Uri(filename));
        //} catch (Exception ex) {
        //    Console.WriteLine(ex.StackTrace);
        //}
    }

    /// <summary>
    /// 显示图片
    /// </summary>
    /// <param name="imageBean"></param>
    private void ShowImage(ImageBean imageBean) {
        try {
            bool a = false;
            if (File.Exists(this.currentImageBean.localPath)) {
                FileInfo fileInfo = new FileInfo(currentImageBean.localPath);
                if (imageBean.size == fileInfo.Length) {
                    a = true;
                }
            }


            // 判断文件是否存在，如果有则加载文件
            if (File.Exists(this.currentImageBean.localPath) && a==true) {
                this.IMG.Source = new BitmapImage(new Uri(this.currentImageBean.localPath));
            }
            // 否则加载缩略图
            else {
                byte[] imageBytes = Convert.FromBase64String(imageBean.thumbnail);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    this.IMG.Source= this.BitmapToBitmapImage(new Bitmap(image));
                }
            }

            double imageW = IMG.Source.Width;
            double imageH = IMG.Source.Height;
            if (imageW < 388 && imageH < 335) {
                IMG.Width = IMG.Source.Width;
                IMG.Height = IMG.Source.Height;
            }

            var group = IMG.FindResource("TfGroup") as TransformGroup;
            var rotateTransform = group.Children[2] as RotateTransform;

            rotateTransform.CenterX = this.GetWindowCenterX();
            rotateTransform.CenterY = this.GetWindowCenterY();
            rotateTransform.Angle = 0;



            var transform = group.Children[1] as TranslateTransform;

            transform.X = 0;
            transform.Y = 0;

            var scaleTransform = group.Children[0] as ScaleTransform;

            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;


            this.RotateTime = 1;

            // 查看到最后一张，不处理
            if (this.currentIndex == this.listImageBeans.Count - 1) {
                BtnPre.Visibility = Visibility.Hidden;
            }
            if (this.currentIndex == 0) {
                BtnNext.Visibility = Visibility.Hidden;
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }

    }

    private BitmapImage BitmapToBitmapImage(Bitmap bitmap) {
        try {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bit3 = new BitmapImage();
            bit3.BeginInit();
            bit3.StreamSource = ms;
            bit3.EndInit();
            return bit3;

        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }
        return null;

    }

    public MainWindow Init(List<ImageBean> listImageBeans, ImageBean currentImageBean) {
        try {
            this.listImageBeans = listImageBeans;
            this.currentImageBean = currentImageBean;

        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }

        return this;
    }

    /// <summary>
    /// 左
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Border_MouseEnter(object sender, MouseEventArgs e) {
        if (listImageBeans!=null && this.currentIndex == this.listImageBeans.Count - 1) {
            return;
        }
        BtnPre.Visibility = Visibility.Visible;
    }
    private void Border_MouseLeave(object sender, MouseEventArgs e) {
        BtnPre.Visibility = Visibility.Hidden;
    }

    private void BorderR_MouseLeave(object sender, MouseEventArgs e) {
        BtnNext.Visibility = Visibility.Hidden;
    }

    private void BorderR_MouseEnter(object sender, MouseEventArgs e) {
        if (this.currentIndex == 0) {
            return;
        }
        BtnNext.Visibility = Visibility.Visible;
    }


    #region 窗口变化

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        Type senderType = e.Source.GetType();
        if ("PictureViewer.MainWindow".Equals(senderType.FullName)) {
            this.DragMove();
        }

    }



    private void x_Click(object sender, RoutedEventArgs e) {

        this.Close();
    }

    private void ___Click(object sender, RoutedEventArgs e) {
        this.WindowState = WindowState.Minimized;
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


    private double left = 0;
    private double top = 0;
    private void tomax_Click(object sender, RoutedEventArgs e) {
        left = this.Left;
        top = this.Top;
        //this.WindowState = WindowState.Maximized;
        this.WindowState = System.Windows.WindowState.Normal;
        Rect rc = SystemParameters.WorkArea;//获取工作区大小
        Size curTaskbarSize = getCurTaskbarSize();
        Point p = getLocation(curTaskbarSize);
        System.Drawing.Rectangle taskbarRect = Screen.PrimaryScreen.WorkingArea;
        this.Left = p.X;//设置位置
        this.Top = p.Y;
        this.Width = rc.Width;
        this.Height = rc.Height;
        this.tomax.Visibility = Visibility.Collapsed;
        this.frommax.Visibility = Visibility.Visible;

    }








    private void frommax_Click(object sender, RoutedEventArgs e) {
        this.WindowState = System.Windows.WindowState.Normal;
        this.Width = windowWidth;
        this.Height = windowHeight;
        this.Left = left;//设置位置
        this.Top = top;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.tomax.Visibility = Visibility.Visible;
        this.frommax.Visibility = Visibility.Collapsed;
    }



    #endregion
}
}
