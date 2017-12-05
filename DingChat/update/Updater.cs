using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common.DbHelper;
namespace cn.lds.chatcore.pcw.update {
public class Updater {
    private static Updater _instance;
    public static Updater getInstance() {
        if (_instance == null) {
            _instance = new Updater();
        }
        return _instance;

    }

    public void CheckUpdateStatus() {
        System.Threading.ThreadPool.QueueUserWorkItem((s) => {
            String data = RestRequestHelper.getSync(ServiceCode.getLastestVersion,null,null);
            if (data == null || "".Equals(data)) {
                return;
            }
            UpdateInfo updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(data);

            Updater.getInstance().StartUpdate(updateInfo);
        });

    }

    public void StartUpdate(UpdateInfo updateInfo) {
        Version serverVersion = new Version(updateInfo.versionName);
        if (Updater.getInstance().CurrentVersion >= serverVersion) {
            //当前版本是最新的，不更新
            return;
        }



        //更新程序复制到缓存文件夹
        string appDir = System.IO.Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0, System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar)));
        string updateFileDir = System.IO.Path.Combine(System.IO.Path.Combine(appDir.Substring(0, appDir.LastIndexOf(System.IO.Path.DirectorySeparatorChar))), "Update");
        if (!Directory.Exists(updateFileDir)) {
            Directory.CreateDirectory(updateFileDir);
        }
        if (updateInfo.md5 == null || "".Equals(updateInfo.md5)) {
            return;
        }
        updateFileDir = System.IO.Path.Combine(updateFileDir, updateInfo.md5.ToString());
        if (!Directory.Exists(updateFileDir)) {
            Directory.CreateDirectory(updateFileDir);
        }

        string exePath = System.IO.Path.Combine(updateFileDir, "AutoUpdater.exe");
        string dllPath = System.IO.Path.Combine(updateFileDir, "ResourceDictionary.dll");
        File.Copy(System.IO.Path.Combine(appDir, "AutoUpdater.exe"), exePath, true);
        File.Copy(System.IO.Path.Combine(appDir, "ResourceDictionary.dll"), dllPath, true);
        var info = new System.Diagnostics.ProcessStartInfo(exePath);
        info.UseShellExecute = false;
        info.WorkingDirectory = exePath.Substring(0, exePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
        //info.Desc = updateInfo.Desc;
        String spliter = " ";
        info.Arguments = "update "
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(CallExeName)) + spliter
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateFileDir)) + spliter
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(appDir)) + spliter
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(CallExeName)) + spliter
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.versionName.ToString())) + spliter
                         + (string.IsNullOrEmpty(updateInfo.description) ? "" : Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.description))) + spliter
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ServiceCode.getDownloadUrl(updateInfo.fileId.ToString()))) + spliter
                         + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.md5.ToString()));



        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        //process.WaitForExit();
        App.ExitApp(false);
    }

    public bool UpdateFinished = false;

    private string _callExeName;
    public string CallExeName {
        get {
            if (string.IsNullOrEmpty(_callExeName)) {
                _callExeName = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1).Replace(".exe", "");
            }
            return _callExeName;
        }
    }

    /// <summary>
    /// 获得当前应用软件的版本
    /// </summary>
    public virtual Version CurrentVersion {
        get {
            return new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).ProductVersion);
        }
    }

    /// <summary>
    /// 获得当前应用程序的根目录
    /// </summary>
    public virtual string CurrentApplicationDirectory {
        get {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
}
