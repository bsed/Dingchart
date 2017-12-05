using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Business {
/// <summary>
/// 业务级别的工具类
/// </summary>
public class ToolsHelper {


    /// <summary>
    /// 根据NO判断NO所属对象的类型
    /// </summary>
    /// <param Name="strNo"></param>
    /// <returns></returns>
    public static ChatSessionType getChatSessionTypeByNo(String strNo) {
        if (strNo.StartsWith(Constants.CHAT_FLAG)) {
            return ChatSessionType.CHAT;
        } else if (strNo.StartsWith(Constants.GROUP_FLAG)) {
            return ChatSessionType.MUC;
        } else if (strNo.StartsWith(Constants.PUBLIC_ACCOUNT_FLAG)) {
            return ChatSessionType.PUBLIC;
        } else if (strNo.StartsWith(Constants.SYSTEM_NOTICE_FLAG)) {
            return ChatSessionType.NOTICE;
        } else if (strNo.StartsWith(Constants.BUSINESS_NOTICE_FLAG)) {
            return ChatSessionType.BUSINESS_NOTICE;
        } else if (strNo.StartsWith(Constants.TODO_TASK_FLAG)) {
            return ChatSessionType.TODO_TASK;
        } else if (strNo.StartsWith(Constants.APPMSG_FLAG)) {
            return ChatSessionType.APPMSG;
        } else {
            return ChatSessionType.CHAT;
        }
    }

    public static String getFileSuffix(String filePath) {
        String downloadFileSuffix = "";
        int pointIndex = filePath.LastIndexOf(".");
        if (pointIndex >= 0) {
            downloadFileSuffix = filePath.Substring(pointIndex);
        }
        return downloadFileSuffix;
    }

    public static String getFileNameFromFilePath(String filePath) {
        String fileName = "";
        //filePath = Path.GetFullPath(filePath);
        //int pointIndex = filePath.LastIndexOf("/");
        //if (pointIndex >= 0) {
        //    fileName = filePath.Substring(pointIndex+1);
        //}
        fileName = Path.GetFileName(filePath);
        return fileName;
    }

    /// <summary>
    /// 根据后缀获取文件类型
    /// </summary>
    /// <param Name="suffix"></param>
    /// <returns></returns>
    public static FileType getFileTypeBySuffix(String suffix) {
        FileType type;
        switch (suffix.ToLower()) {
        case ".jpg":
        case ".jpeg":
        case ".png":
        case ".bmp":
        case ".gif":
        case ".svg":
            type = FileType.IMAGES;
            break;
        case ".mp4":
            type =  FileType.VEDIO;
            break;
        case ".mp3":
        case ".amr":
            type =  FileType.VOICE;
            break;
        case ".apk":
            type =  FileType.APK;
            break;
        default:
            type =  FileType.FILE;
            break;
        }
        return type;
    }


    /// <summary>
    /// 根据后缀获取文件类型
    /// </summary>
    /// <param Name="suffix"></param>
    /// <returns></returns>
    public static String getForderNameByDownloadType(DownloadType downloadType) {

        switch (downloadType) {
        case DownloadType.SYSTEM_IMAGE:
        case DownloadType.SYSTEM_BARCODE:
            return "SYSTEM";
            break;
        case DownloadType.MSG_IMAGE:
        case DownloadType.MSG_VIDEO:
        case DownloadType.MSG_VOICE:
        case DownloadType.MSG_FILE:
            return "TEMP";
            break;
        default:
            return "TEMP";
        }
    }
}
}
