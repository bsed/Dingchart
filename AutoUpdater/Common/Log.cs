
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace cn.lds.chatcore.AutoUpdater.Common {

public class Log {
    //在网站根目录下创建日志目录
    public static string path = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";


    /// <summary>
    /// 输出日志到Log4Net
    /// </summary>
    /// <param name="t"></param>
    /// <param name="ex"></param>
    public static void Error(Type t, Exception ex) {
        //Console.WriteLine(ex.StackTrace);
        //log4net.ILog log = log4net.LogManager.GetLogger(t);
        //log.Error("Error", ex);
    }



    /// <summary>
    /// 输出日志到Log4Net
    /// </summary>
    /// <param name="t"></param>
    /// <param name="msg"></param>
    public static void Error(Type t, string msg) {
        //log4net.ILog log = log4net.LogManager.GetLogger(t);
        //log.Error(msg);
    }

}
}
