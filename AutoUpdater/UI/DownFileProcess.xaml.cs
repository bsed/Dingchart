using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using cn.lds.chatcore.AutoUpdater.Common;

namespace cn.lds.chatcore.AutoUpdater.UI {
public partial class DownFileProcess : WindowBase {
    private string updateFileDir;//更新文件存放的文件夹
    private string callExeName;
    private string appDir;
    private string appName;
    private string appVersion;
    private string desc;
    private string downloadFileUrl;
    private String serverMd5;
    private System.Timers.Timer timer = new System.Timers.Timer();

    public DownFileProcess(string callExeName, string updateFileDir, string appDir, string appName, string appVersion, string desc, string downloadFileUrl, string serverMd5) {
        InitializeComponent();
        this.Loaded += (sl, el) => {
            YesButton.Content = "      开始更新      ";
            NoButton.Content = "      关   闭      ";

            this.YesButton.Click += (sender, e) => {
                Process[] processes = Process.GetProcessesByName(this.callExeName);

                if (processes.Length > 0) {
                    foreach (var p in processes) {
                        p.Kill();
                    }
                }

                DownloadUpdateFile();
            };

            this.NoButton.Click += (sender, e) => {
                this.Close();
            };

            this.newVersion.Text = "发现新版本 " + this.appVersion  ;
            txtDes.Text = this.desc;
        };
        this.callExeName = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(callExeName));
        this.updateFileDir = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(updateFileDir));
        this.appDir = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(appDir));
        this.appName = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(appName));
        this.appVersion = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(appVersion));
        this.downloadFileUrl = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(downloadFileUrl));
        this.serverMd5 = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(serverMd5));

        string sDesc = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(desc));
        if (sDesc.ToLower().Equals("null")) {
            this.desc = "";
        } else {
            this.desc = sDesc;
        }
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        timer.Stop();
        this.Dispatcher.BeginInvoke((Action)delegate () {

            YesButton.Content = "      更新失败      ";
            NoButton.IsEnabled = true;
            NoButton.Visibility = Visibility.Visible;
        });

    }
    public void DownloadUpdateFile() {

        //设置timer可用
        timer.Enabled = true;
        //设置timer
        timer.Interval = 60000;

        //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
        timer.AutoReset = false;

        timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

        timer.Start();
        bProcess.Visibility = System.Windows.Visibility.Visible;
        YesButton.IsEnabled = false;
        NoButton.IsEnabled = false;

        YesButton.Content = "      下载中...      ";

        string url = this.downloadFileUrl;
        var client = new System.Net.WebClient();

        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        client.DownloadProgressChanged += (sender, e) => {
            timer.Stop();
            timer.Start();
            UpdateProcess(e.BytesReceived, e.TotalBytesToReceive);
        };
        client.DownloadDataCompleted += (sender, e) => {
            timer.Stop();
            YesButton.Content = "      下载完成      ";
            string zipFilePath = System.IO.Path.Combine(updateFileDir, "update.zip");
            try {
                byte[] data = e.Result;
                BinaryWriter writer = new BinaryWriter(new FileStream(zipFilePath, FileMode.OpenOrCreate));
                writer.Write(data);
                writer.Flush();
                writer.Close();
                Log.Error(typeof(DownFileProcess), "正在校验更新包..");
                YesButton.Content = "      正在校验更新包...      ";

                //String fileMd5 = MD5Util.GetMD5HashFromFile(zipFilePath);
                //if (fileMd5 == null) {
                //    //异常
                //    YesButton.Content = "      更新包无效      ";
                //    Log.Error(typeof(DownFileProcess), "更新包无效...");
                //    return;
                //}
                //if (!fileMd5.ToUpper().Equals(serverMd5.ToUpper())) {
                //    //异常
                //    YesButton.Content = "      更新包无效      ";
                //    Log.Error(typeof(DownFileProcess), "更新包无效...");
                //    return;
                //}

            } catch (Exception ex) {
                timer.Stop();
                YesButton.Content = "      更新包无效      ";
                NoButton.IsEnabled = true;
                NoButton.Visibility = Visibility.Visible;
                Log.Error(typeof(DownFileProcess), ex);
                return;
            }

            //获取文件的md5和server的md5进行对比

            System.Threading.ThreadPool.QueueUserWorkItem((s) => {
                Action f = () => {
                    txtProcess.Text = "正在更新，请稍候...";
                    YesButton.Content = "      正在更新应用      ";
                };
                this.Dispatcher.Invoke(f);

                string tempDir = System.IO.Path.Combine(updateFileDir, "temp");
                if (!Directory.Exists(tempDir)) {
                    Directory.CreateDirectory(tempDir);
                }
                UnZipFile(zipFilePath, tempDir);

                //移动文件
                //App
                if(Directory.Exists(System.IO.Path.Combine(tempDir,"App"))) {
                    CopyDirectory(System.IO.Path.Combine(tempDir,"App"),appDir);
                }

                f = () => {
                    txtProcess.Text = "更新完成!";

                    try {
                        //清空缓存文件夹
                        string rootUpdateDir = updateFileDir.Substring(0, updateFileDir.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                        foreach (string p in System.IO.Directory.EnumerateDirectories(rootUpdateDir)) {
                            if (!p.ToLower().Equals(updateFileDir.ToLower())) {
                                System.IO.Directory.Delete(p, true);
                            }
                        }
                    } catch (Exception ex) {
                        Log.Error(typeof(DownFileProcess), ex);
                    }

                };
                this.Dispatcher.Invoke(f);

                try {
                    f = () => {
                        //AlertWin alert = new AlertWin("更新完成,是否现在启动软件?") {
                        //    WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this
                        //};
                        //alert.Title = "更新完成";
                        //alert.Loaded += (ss, ee) => {
                        //    alert.YesButton.Width = 40;
                        //    alert.NoButton.Width = 40;
                        //};
                        //alert.Width=300;
                        //alert.Height = 200;
                        //alert.ShowDialog();
                        //if (alert.YesBtnSelected) {
                        //    //启动软件
                        //    string exePath = System.IO.Path.Combine(appDir, callExeName + ".exe");
                        //    var info = new System.Diagnostics.ProcessStartInfo(exePath);
                        //    info.UseShellExecute = true;
                        //    info.WorkingDirectory = appDir;// exePath.Substring(0, exePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                        //    System.Diagnostics.Process.Start(info);
                        //} else {

                        //}
                        try {
                            //启动软件
                            YesButton.Content = "      打开应用      ";

                            string exePath = System.IO.Path.Combine(appDir, callExeName + ".exe");
                            var info = new System.Diagnostics.ProcessStartInfo(exePath);
                            info.UseShellExecute = true;
                            info.WorkingDirectory = appDir;// exePath.Substring(0, exePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                            System.Diagnostics.Process.Start(info);

                            this.Close();
                        } catch (Exception ex) {
                            Log.Error(typeof(DownFileProcess), ex);
                        }

                    };
                    this.Dispatcher.Invoke(f);
                } catch (Exception ex) {
                    timer.Stop();
                    YesButton.Content = "      无法打开应用      ";
                    Log.Error(typeof(DownFileProcess), ex);
                }
            });

        };
        client.DownloadDataAsync(new Uri(url), "update.zip");
    }

    private static void UnZipFile(string zipFilePath, string targetDir) {
        ICCEmbedded.SharpZipLib.Zip.FastZipEvents evt = new ICCEmbedded.SharpZipLib.Zip.FastZipEvents();
        ICCEmbedded.SharpZipLib.Zip.FastZip fz = new ICCEmbedded.SharpZipLib.Zip.FastZip(evt);
        fz.ExtractZip(zipFilePath, targetDir, "");
    }

    public void UpdateProcess(long current, long total) {
        string status = (int)((float)current * 100 / (float)total) + "%";
        this.txtProcess.Text = status;
        rectProcess.Width = ((float)current / (float)total) * bProcess.ActualWidth;
    }

    public void CopyDirectory(string sourceDirName, string destDirName) {
        try {
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }
            if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                destDirName = destDirName + Path.DirectorySeparatorChar;
            string[] files = Directory.GetFiles(sourceDirName);
            foreach (string file in files) {
                try {
                    File.Copy(file, destDirName + Path.GetFileName(file), true);
                    File.SetAttributes(destDirName + Path.GetFileName(file), FileAttributes.Normal);
                } catch (Exception ex) {
                    Log.Error(typeof(DownFileProcess), ex);
                    //todo 日志输出
                }

            }
            string[] dirs = Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs) {
                CopyDirectory(dir, destDirName + Path.GetFileName(dir));
            }
        } catch (Exception ex) {
            throw new Exception("复制文件错误");
            Log.Error(typeof(DownFileProcess), ex);
        }
    }
}
}
