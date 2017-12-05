using System;
using System.Windows;
using log4net;


namespace DingChatChecker {
/// <summary>
///     App.xaml 的交互逻辑
/// </summary>
public partial class App : Application {
    private void App_OnStartup(object sender, StartupEventArgs e) {

        // 运行环境检测
        //if (!EnvironmentCheck.CheckVc2012()) {
        //    EnvironmentCheck.InstallVc2012();
        //    if (EnvironmentCheck.CheckVc2012()) {
        //        LogManager.GetLogger("App").Error("安装12成功。");
        //    } else {
        //        LogManager.GetLogger("App").Error("安装12失败。");
        //        //MessageBox.Show("安装组件失败，请重新运行该程序。");
        //        Environment.Exit(-1);
        //    }
        //}

        if (!EnvironmentCheck.CheckVc2015()) {
            EnvironmentCheck.InstallVc2015();
            if (EnvironmentCheck.CheckVc2015()) {
                LogManager.GetLogger("App").Error("安装15成功。");
            } else {
                LogManager.GetLogger("App").Error("安装15失败。");
                //MessageBox.Show("安装组件失败，请重新运行该程序。");
                Environment.Exit(-1);
            }
        }

        EnvironmentCheck.StartDingChat();

        Environment.Exit(0);
    }
}
}