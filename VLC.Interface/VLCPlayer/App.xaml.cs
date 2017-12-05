using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using VLCPlayer;

namespace VLCPlayer {
/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application {
    private void Application_Startup(object sender, StartupEventArgs e) {
        VLCPlayerWin win = null;
        if (e.Args.Length > 0) {
            string filePath = e.Args[0];
            if (System.IO.File.Exists(filePath)) {
                win = new VLCPlayerWin(filePath);

            } else {
                win = new VLCPlayerWin();
            }
        } else {
            win = new VLCPlayerWin();
        }
        win.Show();
    }
}
}
