using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Common {
public class Constants {


    /// <summary>
    /// 消息定义文件
    /// </summary>
    public static String MESSAGE_DEFINE_FILE_NAME = "./Settings/MessageDefine.xml";

    /// <summary>
    /// 程序设定文件
    /// </summary>
    public static String PROGRAM_SETTING_FILE_NAME = "./Settings/Program.xml";

    /// <summary>
    /// DB实体类存放NameSpaces
    /// </summary>
    public static String ENTITY_CLASS_NAMESPACES = "cn.lds.chatcore.pcw.Models.Tables";

    /// <summary>
    /// 本地DB密码
    /// </summary>
    public static String LOCAL_DB_PASSWORD = "ldsmchat";

    /// <summary>
    /// 本地DB用户
    /// </summary>
    public static String LOCAL_DB_USER = "sa";

    /// <summary>
    /// 语音消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_VE = "[语音消息]";

    /// <summary>
    /// 视频消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_VO = "[视频消息]";

    /// <summary>
    /// 位置消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_LN = "[位置消息]";

    /// <summary>
    /// 单图文消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_HL = "[图文消息]";

    /// <summary>
    /// 多图文消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_ML = "[多图文消息]";

    /// <summary>
    /// 文件消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_FE = "[文件]";

    /// <summary>
    /// 图片消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_PE = "[图片]";

    /// <summary>
    /// 名片消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_VD = "[名片]";

    /// <summary>
    /// 公众号名片消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_PD = "[公众号名片]";

    /// <summary>
    /// 代金卷消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_TICKET = "[代金卷]";

    /// <summary>
    /// 业务消息消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_BUSINESS = "[业务]";

    /// <summary>
    /// 语音消息消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_AUDIO = "[语音聊天]";
    /// <summary>
    /// 视频消息消息格式化提示
    /// </summary>
    public static String MESSAGE_FORMAT_TEXT_VIDEO = "[视频聊天]";

    /// <summary>
    /// 群视频消息消息格式化提示
    /// </summary>
    public static String GROUPMESSAGE_FORMAT_TEXT_VIDEO = "发起语音聊天";

    /// <summary>
    /// 区别是否是单聊
    /// </summary>
    public static String CHAT_FLAG = "C";

    /// <summary>
    /// 区别是否是聊天室
    /// </summary>
    public static String GROUP_FLAG = "G";

    /// <summary>
    /// 区别是否是公众号
    /// </summary>
    public static String PUBLIC_ACCOUNT_FLAG = "SA";

    /// <summary>
    /// 区别是否是系统通知
    /// </summary>
    public static String SYSTEM_NOTICE_FLAG = "SYSTEM_NOTICE";

    /// <summary>
    /// 区别是否是业务通知
    /// </summary>
    public static String BUSINESS_NOTICE_FLAG = "BUSINESS_NOTICE";

    /// <summary>
    /// 区别是否是待办
    /// </summary>
    public static String TODO_TASK_FLAG = "TODO";

    /// <summary>
    /// 应用消息
    /// </summary>
    public static String APPMSG_FLAG = "APPMSG";



    /// <summary>
    /// 选择图片回调
    /// </summary>
    public static String SELECT_PHOTO_SEND = "sendDatas";

    /// <summary>
    ///  @消息提示
    /// </summary>
    public static String MESSAGE_AT = "[有人@我]";
    public static String MESSAGE_AT_IN_MUC = "@了你";
    public static String MESSAGE_DRAFT = "[草稿]";

    /// <summary>
    /// 会话历史>公众号是否强制置顶
    /// </summary>
    public static Boolean SYS_CONFIG_IS_PUBLIC_TOP = false;
    /// <summary>
    /// 消息>待办
    /// </summary>
    public static Boolean SYS_CONFIG_SHOW_TODO = true;
    /// <summary>
    /// 消息>应用消息
    /// </summary>
    public static Boolean SYS_CONFIG_SHOW_APPMSG = true;

    /// <summary>
    /// 聊天中每次刷新的消息数
    /// </summary>
    public static int SYS_CONFIG_MESSAGE_NUM_PERPAGE = 20;

    /// <summary>
    /// 缓存文件根路径
    /// </summary>
    public static String STS_CACHE_PATH = System.IO.Path.GetFullPath(Environment.CurrentDirectory) + @"/DataSqlite/private/" +
                                          App.AccountsModel.no + "/";

    public static int SYS_ScHight = 300;

    public static int AmTimeOut = 60000;
}
}
