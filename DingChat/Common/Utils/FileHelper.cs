using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class FileHelper {

    // <summary>
    /// 返回指示文件是否已被其它程序使用的布尔值
    /// </summary>
    /// <param Name="fileFullName">文件的完全限定名，例如：“C:\MyFile.txt”。</param>
    /// <returns>如果文件已被其它程序使用，则为 true；否则为 false。</returns>
    public static Boolean FileIsInUsed(String fileFullName) {
        Boolean result = false;

        //如果文件存在，则继续判断文件是否已被其它程序使用
        //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
        System.IO.FileStream fileStream = null;
        try {
            fileStream = System.IO.File.Open(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            result = false;
        } catch (System.IO.IOException ioEx) {
            result = true;
        } catch (System.Exception ex) {
            result = true;
        } finally {
            if (fileStream != null) {
                fileStream.Close();
            }
        }

        return result;
    }

}
}
