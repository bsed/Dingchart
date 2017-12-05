using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;
using  cn.lds.chatcore.AutoUpdater.Common;

namespace cn.lds.chatcore.AutoUpdater {
class Program {
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main(string[] args) {
        Log.Error(typeof(Program),"dddddddddddddddddd---1");
        //try {
        //    //MessageBox.Show("外部更新");
        //    string callExeName = "RGluZ2NoYXQ=";
        //    string updateFileDir = "RDpcZGV2ZWxvcFxWUzIwMTJcZGluZ3hpbl9QQ1xhcHBsZVxiaW5cVXBkYXRlXHF1d2Vp";
        //    string appDir = "RDpcZGV2ZWxvcFxWUzIwMTJcZGluZ3hpbl9QQ1xhcHBsZVxiaW5cRGVidWc=";
        //    string appName = "RGluZ2NoYXQ=";
        //    string appVersion = "MS4wLjAuMQ==";
        //    string desc = "MS7mm7Lnjq7mnIDluIXkuoYNCjIu5pa95L+K5rKh5puy546u5biFDQozLuS4m+aWh+aJjeaYr+ecn+eahOW4hQ==";
        //    string downloadFileUrl = "aHR0cDovLzE5Mi4xNjguMC4xMDo4Nzg3L2Rvd25sb2FkLzE2OTE=";
        //    string md5 = "MS4wLjAuMQ==";
        //    //Check If Have New Vision
        //    cn.lds.chatcore.AutoUpdater.App app = new cn.lds.chatcore.AutoUpdater.App();
        //    UI.DownFileProcess downUI = new UI.DownFileProcess(callExeName, updateFileDir, appDir, appName, appVersion, desc, downloadFileUrl, md5) {
        //        WindowStartupLocation = WindowStartupLocation.CenterScreen
        //    };
        //    app.Run(downUI);
        //} catch (Exception ex) {
        //    MessageBox.Show(ex.Message);
        //}

        //return;

        if (args.Length == 0) {
            Log.Error(typeof(Program), "dddddddddddddddddd---2");
            //cn.lds.chatcore.AutoUpdater.App app = new cn.lds.chatcore.AutoUpdater.App();
            //UI.DownFileProcess downUI = new UI.DownFileProcess("", "", "","","","");
            //app.Run(downUI);


            //MessageBox.Show("无参数");
            //UI.DownFileProcess downUI = new UI.DownFileProcess();

            //cn.lds.chatcore.AutoUpdater.App app = new cn.lds.chatcore.AutoUpdater.App();
            //app.Run(downUI);
            return;
        } else if (args[0] == "update") {
            Log.Error(typeof(Program), "dddddddddddddddddd---3");
            try {
                //MessageBox.Show("外部更新");
                string callExeName = args[1];
                string updateFileDir = args[2];
                string appDir = args[3];
                string appName = args[4];
                string appVersion = args[5];
                string desc = args[6];
                string downloadFileUrl = args[7];
                string serverMd5 = args[8];

                //先关闭原先的程序，只显示更新程序
                String processName = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(callExeName));
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0) {
                    foreach (var process in processes) {
                        process.Kill();
                    }
                }

                //Check If Have New Vision
                cn.lds.chatcore.AutoUpdater.App app = new cn.lds.chatcore.AutoUpdater.App();
                UI.DownFileProcess downUI = new UI.DownFileProcess(callExeName, updateFileDir, appDir, appName, appVersion, desc, downloadFileUrl, serverMd5) {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                app.Run(downUI);
            } catch (Exception ex) {
                Log.Error(typeof(Program), ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
}
