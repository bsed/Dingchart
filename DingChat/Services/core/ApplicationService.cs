using cn.lds.chatcore.AutoUpdater;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.imtp.message;
using EventBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Views;
using cn.lds.chatcore.pcw.Views.Windows;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.Publisher;

namespace cn.lds.chatcore.pcw.Services.core {
class ApplicationService {
    private static ApplicationService instance = null;
    public static ApplicationService getInstance() {
        if (instance == null) {
            instance = new ApplicationService();
        }
        return instance;
    }

    /// <summary>
    /// 处理IM消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onMessageArrivedEvent(MessageArrivedEvent messageArrivedEvent) {

        try {
            // 获取消息类型
            Message message = messageArrivedEvent.message;
            MsgType msgType = message.getType();
            switch (msgType) {
            // 用户被禁用
            case MsgType.UserDisabled:

                this.ReStartApplication(MsgType.UserDisabled.ToStr(), "当前帐户已禁用");
                break;
            // 用户被手机端强制退出
            case MsgType.LoginQuit:
                this.ReStartApplication(MsgType.LoginQuit.ToStr(), "你已退出系统");
                break;
            }
        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }
    }

    /// <summary>
    /// 处理IM消息
    /// </summary>
    /// <returns></returns>
    [EventSubscriber]
    public void onFrameEvent(FrameEvent<Object> eventData) {
        try {
            // 获取事件类型
            switch (eventData.frameEventDataType) {
            case FrameEventDataType.IM_CONNECTED:
                Console.WriteLine("IM 链接成功 哈哈哈哈哈 哈哈哈");
                LoginWindow.getInstance().IsConectToImFinished = true;
                break;
            // 登陆TOKEN过期
            case FrameEventDataType.LOGOUT_TOKEN_INVALID:
                this.ReStartApplication(FrameEventDataType.LOGOUT_TOKEN_INVALID.ToStr(),"登录认证过期！");
                break;
            // 用户被踢
            case FrameEventDataType.LOGOUT_USER_KICKED:
                this.ReStartApplication(FrameEventDataType.LOGOUT_USER_KICKED.ToStr(), "你的帐号于" + DateTimeHelper.getFormateDateString(DateTime.Now, DateTimeType.HHmm) + "在其他设备上登录！如果不是你的操作，你的密码已经泄露，请尽快登录修改密码！");
                break;
            }

        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }

    }

    /// <summary>
    /// 重启 应用程序
    /// </summary>
    public void ReStartApplication(String type,String message) {

        try {

            BusinessEvent<Object> businessEvent = new BusinessEvent<Object>();
            businessEvent.eventDataType = BusinessEventDataType.DisposeIcon;
            EventBusHelper.getInstance().fireEvent(businessEvent);


            String appDir = App.AppRootPath;
            string callExeName = Updater.Instance.CallExeName;

            string exePath = System.IO.Path.Combine(appDir, callExeName+".exe");
            var info = new ProcessStartInfo(exePath);
            info.UseShellExecute = true;
            info.WorkingDirectory = appDir;

            info.Arguments = Convert.ToBase64String(Encoding.UTF8.GetBytes(type)) + " "
                             + Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            try {
                if (!PcStart.getInstance().Topmost) {
                    PcStart.getInstance().Topmost = true;
                }
            } catch (Exception ex) {
                Log.Error(typeof(ApplicationService), ex);
            }


            //var task1 = new Task(() => {
            //    PcStart.getInstance().DisposeIcon();
            //});

            //task1.Start();

            Process.Start(info);

            PcStart.getInstance().DisposeIcon();
            App.ExitApp();
        } catch (Exception e) {
            PcStart.getInstance().DisposeIcon();
            App.ExitApp();
            Log.Error(typeof(ApplicationService), e);
        }
    }

    /// <summary>
    /// 判断应用是否已经启动
    /// </summary>
    public Boolean IsApplicationExist() {
        try {
            string callExeName = Updater.Instance.CallExeName;
            Process[] processes = Process.GetProcessesByName(callExeName);
            if (processes.Length > 1) {
                return true;
            }
        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }
        return false;
    }

    /// <summary>
    /// 判断应用是否已经启动
    /// </summary>
    public void KillAllOtherProcess() {
        try {
            string callExeName = Updater.Instance.CallExeName;
            // 当前进程
            Process cur = Process.GetCurrentProcess();
            // 全部进程
            Process[] processes = Process.GetProcessesByName(callExeName);
            if (processes!=null) {
                foreach (var process in processes) {
                    try {
                        if (process.Id != cur.Id) {
                            process.Kill();
                        }
                    } catch (Exception e) {
                        Log.Error(typeof(ApplicationService), e);
                    }
                }
            }

        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }
    }
    /// <summary>
    /// 拉取系统用数据
    /// </summary>
    public void RequestSystemData() {
        try {

            // 执行缓存头像的下载
            DownloadServices.getInstance().DownloadCatchExcute();
            // 同步我的二维码
            AccountsServices.getInstance().getMyQRcode();
            // 同步码表

            if (App.TenantNoList.Count > 0) {
                foreach (string tenantNo in App.TenantNoList) {
                    MasterServices.getInstance().RequestMaster(tenantNo);
                }
            } else {
                MasterServices.getInstance().RequestMaster(string.Empty);
            }
            // 开启关键数据请求完整性校验
            DataPullService.getInstance().startDataPull();
        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }

    }

    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {
        try {
            ContactsDao.getInstance().ReBuildDataWithoutPinyin();
            MucDao.getInstance().ReBuildDataWithoutPinyin();
            MucMembersDao.getInstance().ReBuildDataWithoutPinyin();
            OrganizationDao.getInstance().ReBuildDataWithoutPinyin();
            OrganizationMemberDao.getInstance().ReBuildDataWithoutPinyin();
            PublicAccountsDao.getInstance().ReBuildDataWithoutPinyin();
            PublicWebDao.getInstance().ReBuildDataWithoutPinyin();
            ThirdAppClassDao.getInstance().ReBuildDataWithoutPinyin();
            ThirdAppGroupDao.getInstance().ReBuildDataWithoutPinyin();

        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }

    }

    /// <summary>
    /// 预先调用加载会话
    /// </summary>
    public void LoadChatSession() {
        try {
            ChatSessionService.getInstance().findAllChatSession();
            App.ChatSessionLoadOk = true;
            Console.WriteLine("会话数据加载：完毕");
            BusinessEvent<Object> Businessdata = new BusinessEvent<Object>();
            Businessdata.eventDataType = BusinessEventDataType.LoadingOk;
            EventBusHelper.getInstance().fireEvent(Businessdata);
        } catch (Exception e) {
            Log.Error(typeof(ApplicationService), e);
        }
    }
}
}
