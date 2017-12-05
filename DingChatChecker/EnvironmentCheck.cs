using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using log4net;
using Microsoft.Win32;

namespace DingChatChecker {

public enum INSTALLSTATE {
    INSTALLSTATE_NOTUSED = -7,  // component disabled
    INSTALLSTATE_BADCONFIG = -6,  // configuration data corrupt
    INSTALLSTATE_INCOMPLETE = -5,  // installation suspended or in progress
    INSTALLSTATE_SOURCEABSENT = -4,  // run from source, source is unavailable
    INSTALLSTATE_MOREDATA = -3,  // return buffer overflow
    INSTALLSTATE_INVALIDARG = -2,  // invalid function argument
    INSTALLSTATE_UNKNOWN = -1,  // unrecognized product or feature
    INSTALLSTATE_BROKEN = 0,  // broken
    INSTALLSTATE_ADVERTISED = 1,  // advertised feature
    INSTALLSTATE_REMOVED = 1,  // component being removed (action state, not settable)
    INSTALLSTATE_ABSENT = 2,  // uninstalled (or action state absent but clients remain)
    INSTALLSTATE_LOCAL = 3,  // installed on local drive
    INSTALLSTATE_SOURCE = 4,  // run from source, CD or net
    INSTALLSTATE_DEFAULT = 5,  // use default, local or source
}

public class EnvironmentCheck {
    private  static  ILog log = LogManager.GetLogger("EnvironmentCheck");
    private static string dependenciesKey = @"Installer\Dependencies\";

    [DllImport("msi.dll")]
    public static extern INSTALLSTATE MsiQueryProductState(string product);


    /// <summary>
    /// Microsoft Visual C++ 2012 x86
    /// </summary>
    /// <returns></returns>
    public static bool CheckVc2012() {

        //if (MsiQueryProductState("{46f42e59-8bc8-4144-9c41-869ad2fdb65d}") != INSTALLSTATE.INSTALLSTATE_DEFAULT
        //        && MsiQueryProductState("{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}") != INSTALLSTATE.INSTALLSTATE_DEFAULT) {
        //    return false;
        //}
        bool retValue = false;
        RegistryKey key = Registry.ClassesRoot;
        RegistryKey dependencies = key.OpenSubKey(dependenciesKey);
        if (dependencies != null) {
            string[] subKeyNames = dependencies.GetSubKeyNames();
            List<string> vcList = subKeyNames.Where(q => {
                return q.StartsWith("Microsoft.VS.VC_RuntimeAdditional") &&
                       q.EndsWith("x86,v11");
            }).ToList();

            //HKEY_CLASSES_ROOT\Installer\Dependencies\Microsoft.VS.VC_RuntimeAdditional_x86,v11
            retValue = vcList.Count > 0;
        } else {
            log.Error("OpenSubKey(" + dependenciesKey + ") 失败！");
        }

        return retValue;
    }

    /// <summary>
    /// Microsoft Visual C++ 2015 x86
    /// </summary>
    /// <returns></returns>
    public static bool CheckVc2015() {
        //if (MsiQueryProductState("{74d0e5db-b326-4dae-a6b2-445b9de1836e}") != INSTALLSTATE.INSTALLSTATE_DEFAULT
        //        && MsiQueryProductState("{3CB4E2E8-04EB-371A-9433-4CA0D934B260}") != INSTALLSTATE.INSTALLSTATE_DEFAULT) {
        //    return false;
        //}
        bool retValue = false;
        RegistryKey key = Registry.ClassesRoot;
        RegistryKey dependencies = key.OpenSubKey(dependenciesKey);
        if (dependencies != null) {
            string[] subKeyNames = dependencies.GetSubKeyNames();
            List<string> vcList = subKeyNames.Where(q => {
                return q.StartsWith("Microsoft.VS.VC_RuntimeAdditional") &&
                       q.EndsWith("x86,v14");
            }).ToList();

            //HKEY_CLASSES_ROOT\Installer\Dependencies\Microsoft.VS.VC_RuntimeAdditionalVSU_x86,v14
            retValue = vcList.Count > 0;
        } else {
            log.Error("OpenSubKey(" + dependenciesKey  + ") 失败！");
        }

        return retValue;
    }

    public static int InstallVc2012() {
        try {
            string exeFile = @"aa_vc_2012_redist_x86.exe";
            //if (Environment.Is64BitOperatingSystem) {
            //    exeFile = @"aa_vc_2012_redist_x64.exe"; // 64支持
            //}
            string strExePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + exeFile;
            ProcessStartInfo info = new ProcessStartInfo(strExePath);
            // ProcessStartInfo info = new ProcessStartInfo(strExePath,传给EXE 的参数);
            info.UseShellExecute = false;
            //隐藏exe窗口状态
            info.WindowStyle = ProcessWindowStyle.Normal;
            //运行exe
            Process proBach = Process.Start(info);
            proBach.WaitForExit();
            // 取得EXE运行后的返回值，返回值只能是整型
            int returnValue = proBach.ExitCode;
            log.Info("returnValue : " + returnValue);
            return returnValue;
        } catch (Exception ex) {
            log.Error("安装Vc2012环境时发生异常！" + ex.Message);
        }
        return 0;
    }

    public static int InstallVc2015() {
        try {
            string exeFile = @"aa_vc_2015_redist_x86.exe";
            //if (Environment.Is64BitOperatingSystem) {
            //    exeFile = @"aa_vc_2015_redist.x64.exe"; // 64支持
            //}
            string strExePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + exeFile;
            ProcessStartInfo info = new ProcessStartInfo(strExePath);
            // ProcessStartInfo info = new ProcessStartInfo(strExePath,传给EXE 的参数);
            info.UseShellExecute = false;
            //隐藏exe窗口状态
            info.WindowStyle = ProcessWindowStyle.Normal;
            //运行exe
            Process proBach = Process.Start(info);
            proBach.WaitForExit();
            // 取得EXE运行后的返回值，返回值只能是整型
            int returnValue = proBach.ExitCode;
            log.Info("returnValue : " + returnValue);
            return returnValue;
        } catch (Exception ex) {
            log.Error("安装Vc2015环境时发生异常！" + ex.Message);
        }
        return 0;
    }

    public static void StartDingChat() {
        try {
            string exeFile = @"DingChatExt.exe";

            if (File.Exists(exeFile)) {
                //运行exe
                Process.Start(exeFile);
            }
            log.Info("StartUp DingChat Success!!");
            return;
        } catch (Exception ex) {
            log.Error("StartUp DingChat 时发生异常！" + ex.Message);
        }
    }

}

}
