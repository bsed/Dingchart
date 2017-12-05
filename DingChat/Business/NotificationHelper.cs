using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.MediaHelper;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Services.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using cn.lds.chatcore.pcw.Views;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Messages.Core;
using ToastNotifications.Position;

namespace cn.lds.chatcore.pcw.Business {
class NotificationHelper {

    private static DateTime lastNewMsgTime;
    private static int fontSize = 15;

    static Notifier  notifier = new Notifier(cfg => {
        cfg.PositionProvider = new WindowPositionProvider(
            parentWindow: Application.Current.MainWindow,
            corner: Corner.BottomRight,
            offsetX: 10,
            offsetY: 10);

        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
            notificationLifetime: TimeSpan.FromSeconds(2),
            maximumNotificationCount: MaximumNotificationCount.FromCount(5));
        cfg.DisplayOptions.TopMost = true; // set the option to show notifications over other windows
        //cfg.DisplayOptions.Width = 250; // set the notifications width
        cfg.Dispatcher = Application.Current.Dispatcher;
    });


    /// <summary>
    /// 收到新消息
    /// </summary>
    public static void NewMessage(String user) {
        // 声音处理
        NewMessageSound(user);
        // 状态栏处理
        NewMessageStatusBar(user);
        // 右下角图标处理
        NewMessageIcon(user);
    }

    /// <summary>
    /// 新消息的声音提示处理
    /// </summary>
    /// <param Name="user"></param>
    private static void NewMessageSound(String user) {
        try {

            //判断会话是否开启了免打扰
            if (SettingService.getInstance().isQuiet(user)) {
                //Console.WriteLine("会话开启了免打扰");
                return;
            }

            // 获取当前时间
            DateTime nowTime = DateTime.Now;

            // 如果开启了整体的免打扰
            if (App.AccountsModel.enableNoDisturb) {
                // 获取免打扰的开始结束时间
                String startTimeOfNoDisturb = App.AccountsModel.startTimeOfNoDisturb;
                String endTimeOfNoDisturb = App.AccountsModel.endTimeOfNoDisturb;
                // 获取当前系统时间
                String currentTime = DateTimeHelper.getFormateDateString(nowTime, DateTimeType.HHmm_1);
                // 如果开始开始时间>结束数时间则免打扰时间段为：开始时间->半夜12点，0点-结束时间。
                if (startTimeOfNoDisturb.CompareTo(endTimeOfNoDisturb) > 0) {
                    if (currentTime.CompareTo(startTimeOfNoDisturb) >= 0 && currentTime.CompareTo("2359") <= 0) {

                        //Console.WriteLine("消息面声音打扰：当前时间在 开始时间->半夜12点 这个时间段");
                        return;
                    }
                    if (currentTime.CompareTo("0000") >= 0 && currentTime.CompareTo(endTimeOfNoDisturb) <= 0) {
                        //Console.WriteLine("消息面声音打扰：当前时间在 0点-结束时间 这个时间段");
                        return;
                    }
                }
                // 否则 免打扰时间段为：开始时间->结束时间
                else {
                    if (currentTime.CompareTo(startTimeOfNoDisturb) >= 0 && currentTime.CompareTo(endTimeOfNoDisturb) <= 0) {
                        //Console.WriteLine("消息面声音打扰：当前时间在 开始时间->结束时间 这个时间段");
                        return;
                    }
                }
            }


            Boolean bolIsPlay = false;
            if (user.Equals(App.SelectChartSessionNo)) {
            } else {
                if (lastNewMsgTime == null) {
                    //初次收到消息，直接提醒
                    bolIsPlay = true;
                } else {
                    TimeSpan ts = DateTimeHelper.DateDiff(nowTime, lastNewMsgTime);
                    // 消息间隔大于5秒才提示
                    if (ts.Seconds > 5) {
                        bolIsPlay = true;
                    }
                }

            }
            if (bolIsPlay) {
                String soundPath = App.AppRootPath + @"/SysConfig/Sound/" + "NewMessage.mp3";
                VoiceHelper.StartPlayASysSound(soundPath);
            }
            lastNewMsgTime = nowTime;
        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }

    /// <summary>
    /// 新消息的状态栏闪烁
    /// </summary>
    /// <param Name="user"></param>
    private static void NewMessageStatusBar(String user) {
        try {
            PcStart.getInstance().StartTaskBarFlash();
        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }

    /// <summary>
    /// 新消息的右下角图标处理
    /// </summary>
    /// <param Name="user"></param>
    private static void NewMessageIcon(String user) {
        try {

            PcStart.getInstance().StartNotifyIconFlash();

        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }

    /// <summary>
    /// toast提示
    /// </summary>
    /// <param Name="message"></param>
    public static void ShowInfoMessage(String message) {
        try {
            var options = new MessageOptions {
                FontSize = fontSize, // set notification font size
                ShowCloseButton = false, // set the option to show or hide notification close button
                NotificationClickAction = n => // set the callback for notification click event
                {
                    n.Close(); // call Close method to remove notification

                }
            };
            notifier.ShowInformation(message,options);
        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }

    public static void ShowSuccessMessage(String message) {
        try {
            var options = new MessageOptions {
                FontSize = fontSize, // set notification font size
                ShowCloseButton = false, // set the option to show or hide notification close button
                NotificationClickAction = n => // set the callback for notification click event
                {
                    n.Close(); // call Close method to remove notification

                }
            };
            notifier.ShowSuccess(message, options);
        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }

    public static void ShowErrorMessage(String message) {
        try {
            var options = new MessageOptions {
                FontSize = fontSize, // set notification font size
                ShowCloseButton = false, // set the option to show or hide notification close button
                NotificationClickAction = n => // set the callback for notification click event
                {
                    n.Close(); // call Close method to remove notification

                }
            };
            notifier.ShowError(message,options);
        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }

    public static void ShowWarningMessage(String message) {
        try {
            var options = new MessageOptions {
                FontSize = fontSize, // set notification font size
                ShowCloseButton = false, // set the option to show or hide notification close button
                NotificationClickAction = n => // set the callback for notification click event
                {
                    n.Close(); // call Close method to remove notification

                }
            };
            notifier.ShowWarning(message,options);
        } catch (Exception e) {
            Log.Error(typeof(NotificationHelper), e);
        }
    }
}
}
