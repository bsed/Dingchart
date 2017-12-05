using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VLCPlayer {
/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class VLCPlayerWin : Window {
    public VLCPlayerWin() {
        InitializeComponent();
    }

    private void Window_Closed(object sender, EventArgs e) {
        player.DestroyPlayer();
    }

    private void player_MouseDown(object sender, MouseButtonEventArgs e) {
        if (e.ClickCount == 2) {
            player.ToggleFullScreen();
        }
    }
}
}
