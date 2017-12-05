using cn.lds.chatcore.pcw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.DbHelper;
using cn.lds.chatcore.pcw.Views;

using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Views.Page;
using cn.lds.chatcore.pcw.Views.Windows;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control.Message;

namespace cn.lds.chatcore.pcw {
public class StartLogic {

    public static void init(String[] args) {

        try {
            if (args != null && args.Length>0) {
                App.AppStartType = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
                App.AppStartMessage = Encoding.UTF8.GetString(Convert.FromBase64String(args[1]));
            }
        } catch (Exception ex) {

        }
        // UI线程的未处理异常
        Application.Current.DispatcherUnhandledException -= dispatcherUnhandledExceptionHandle;
        Application.Current.DispatcherUnhandledException += dispatcherUnhandledExceptionHandle;

        // 非UI线程抛出的未处理异常
        AppDomain.CurrentDomain.UnhandledException -= domain_UnhandledExceptionHandle;
        AppDomain.CurrentDomain.UnhandledException += domain_UnhandledExceptionHandle;

        // 当主窗口关闭或调用Application.Shutdown时关闭
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

        // 取得全部  拿到登陆成功再加载
        ProgramSettingHelper.initProgramSettingXmlAllContents();

        // 取得窗体控件列表


        // 初期化网卡监控事件
        //RestRequestHelper.initSniff();

        // 设定启动窗口
        Application.Current.MainWindow =  PcStart.getInstance();

        //Application.Current.MainWindow = new Win_Start();

    }
    /// <summary>
    /// UI线程的未处理异常事件处理
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    static void dispatcherUnhandledExceptionHandle(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        try {
            //MessageBox.Show("当前应用程序遇到一些问题", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true; //该异常不再作为UnhandledException抛出
            Log.Error(sender.GetType(), "dispatcherUnhandledExceptionHandle***: " + sender.GetType().FullName, e.Exception);
            //throw new LdException("程序出现错误请联系管理员");
            Console.Out.WriteLine("===========Error=================[" + e.Exception.Message + "]=====================");
        } catch (Exception ex) {
            Log.Error(typeof(StartLogic), ex);
        }
    }

    /// <summary>
    /// 非UI线程抛出的未处理异常事件处理
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    static void domain_UnhandledExceptionHandle(object sender, UnhandledExceptionEventArgs e) {
        try {
            Exception ex = (Exception)e.ExceptionObject;
            //MessageBox.Show("当前应用程序遇到一些问题，操作已经终止.", "内部错误", MessageBoxButton.OK, MessageBoxImage.Error);

            Log.Error(sender.GetType(), "domain_UnhandledExceptionHandle***: " + sender.GetType().FullName, ex);
            //throw new LdException("程序出现错误请联系管理员");
            Console.Out.WriteLine("===========Error=================[" + e.ToString() + "]=====================");
        } catch (Exception ex) {
            Log.Error(typeof(StartLogic), ex);
        }
    }


}

}
