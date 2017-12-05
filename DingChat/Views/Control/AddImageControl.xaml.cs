using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// AddImageControl.xaml 的交互逻辑
/// </summary>
public partial class AddImageControl : UserControl {
    public AddImageControl() {
        InitializeComponent();
    }

    public string ImagePath = string.Empty;
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        BtnDel.Visibility = Visibility.Collapsed;
    }

    public void clear() {
        ImagePath = string.Empty;
        ImageBlock.Inlines.Clear();
        BtnAddImage.Visibility = Visibility.Visible;
        BtnDel.Visibility = Visibility.Collapsed;
    }
    private void BtnAddImage_Click(object sender, RoutedEventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        //openFileDialog.Filter = "图片 |*.png;*.jpg;*.jpeg;*.bmp|全部文件 (*.*)|*.*";
        openFileDialog.Filter = "图片 |*.png;*.jpg;*.jpeg;*.bmp;*.gif";
        openFileDialog.RestoreDirectory = false;
        openFileDialog.Multiselect = false;
        if (openFileDialog.ShowDialog() == true) {

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(openFileDialog.FileNames[0].ToString(), UriKind.RelativeOrAbsolute));
            ImagePath = openFileDialog.FileNames[0].ToString();
            ImageBlock.Inlines.Add(image);
            BtnAddImage.Visibility = Visibility.Collapsed;
            BtnDel.Visibility = Visibility.Visible;

        }
    }

    private void BtnDel_Click(object sender, RoutedEventArgs e) {
        clear();
    }
}
}
