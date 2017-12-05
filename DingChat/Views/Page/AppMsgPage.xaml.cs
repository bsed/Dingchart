using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Services.core;
using cn.lds.chatcore.pcw.Views.Control.Message;
using EventBus;

namespace cn.lds.chatcore.pcw.Views.Page {
/// <summary>
/// TodoTaskPage.xaml 的交互逻辑
/// </summary>
public partial class AppMsgPage : System.Windows.Controls.Page {
    private AppMsgPage() {
        InitializeComponent();

        Thread newMsgThread = new Thread(() => {
            List<MessageItem> x = new List<MessageItem>();

            while (true) {
                if (queuemMessageItems.Count > 0) {
                    x.Clear();
                    while (x.Count < 8 && queuemMessageItems.Count > 0) {
                        x.Add(queuemMessageItems.Dequeue());
                    }

                    LoadMessage(x);
                    Thread.Sleep(50);
                } else {
                    Thread.Sleep(1000);
                }
            }
        });
        newMsgThread.IsBackground = true;
        newMsgThread.Start();
    }

    private Queue<MessageItem> queuemMessageItems = new Queue<MessageItem>();
    private bool load = false;
    private List<MessageItem> _messagesList = new List<MessageItem>();
    private static AppMsgPage instance = null;
    public static AppMsgPage getInstance() {
        if (instance == null) {
            instance = new AppMsgPage();
        }
        return instance;
    }
    private long lastTimestamp = 0;
    private void Page_Loaded(object sender, RoutedEventArgs e) {

        LoadPanel();
        load = true;
    }


    private void LoadPanel() {
        StackMain.Children.Clear();
        Image imageNull = new Image();
        string HeadPortrait = @"Todo/AppMessage_nocontent.png";


        //加载所有
        _messagesList = MessageService.getInstance().getMessagesByPage(Constants.APPMSG_FLAG, DateTimeHelper.getTimeStamp());
        if (_messagesList.Count == 0) {
            ImageHelper.loadSysImage(HeadPortrait, imageNull);
            imageNull.Stretch = Stretch.None;
            imageNull.Margin = new Thickness(0, 160, 0, 0);
            imageNull.HorizontalAlignment = HorizontalAlignment.Center;
            imageNull.VerticalAlignment = VerticalAlignment.Center;
            StackMain.Children.Add(imageNull);
        } else {
            for (int i =0; i < _messagesList.Count; i++) {
                queuemMessageItems.Enqueue(_messagesList[i]);
            }
            if (_messagesList[_messagesList.Count - 1].timestamp != null) {
                lastTimestamp = long.Parse(_messagesList[_messagesList.Count - 1].timestamp);
            }
            //LoadMessage(_messagesList);
        }


    }

    private void LoadMessage(List<MessageItem> messagesList) {
        try {
            for (int i = 0; i < messagesList.Count; i++) {
                MessageItem item = messagesList[i];
                this.Dispatcher.BeginInvoke(new Action(() => {
                    AppMsgMessageControl control = new AppMsgMessageControl();
                    control.Item = item;
                    control.HorizontalAlignment = HorizontalAlignment.Stretch;
                    control.Margin = new Thickness(10, 0, 0, 20);
                    StackMain.Children.Insert(0, control);
                }));
                ////取最上面一条的时间
                //if (i == messagesList.Count - 1) {
                //    lastTimestamp = DateTimeHelper.getLong(item.timeDate);
                //}
            }
            ScrollViewer.ScrollToBottom();
        } catch (Exception ex) {
            Log.Error(typeof(AppMsgPage), ex);
        }
    }

    /// <summary>
    /// 加载控件
    /// </summary>
    /// <param Name="dt"></param>
    private void AddControl(MessageItem dt) {
        try {
            AppMsgMessageControl control = new AppMsgMessageControl();
            control.Item = dt;
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.Margin = new Thickness(10, 0, 0, 20);
            StackMain.Children.Add(control);
        } catch (Exception ex) {
            Log.Error(typeof(AppMsgPage), ex);
        }

    }

    [EventSubscriber]
    public void OnBusinessEvent(BusinessEvent<Object> data) {
        try {
            switch (data.eventDataType) {
            //来消息
            case BusinessEventDataType.MessageChangedEvent:
                DoMessageChangedEvent(data);
                break;

            }
        } catch (Exception ex) {
            Log.Error(typeof(AppMsgPage), ex);
        }

    }

    private void DoMessageChangedEvent(BusinessEvent<object> data) {
        try {
            if (data.data.ToStr() == string.Empty) return;

            MessageItem item = data.data as MessageItem;
            if (item == null) return;
            if (App.SelectChartSessionNo != Constants.APPMSG_FLAG) return;
            this.Dispatcher.BeginInvoke((Action)delegate() {

                if (item.type == MsgType.App.ToStr()) {
                    queuemMessageItems.Enqueue(item);
                    //设置已读
                    MessageService.getInstance().setMessageRead(item.user);
                }





                ScrollViewer.ScrollToBottom();
            });

            //更新chartsession
            ChatSessionTable chart = new ChatSessionTable();
            chart.user = Constants.APPMSG_FLAG;
            //更新chartsession
            BusinessEvent<object> businessEvent = new BusinessEvent<object>();
            businessEvent.data = chart;
            businessEvent.eventDataType = BusinessEventDataType.ChartSessionChangeEvent;
            EventBusHelper.getInstance().fireEvent(businessEvent);
        } catch (Exception ex) {
            Log.Error(typeof(AppMsgPage), ex);
        }
    }

    public event Action<ChatSessionType> BtnSetClick;
    //点击设置
    private void BtnSet_Click(object sender, RoutedEventArgs e) {
        if (BtnSetClick != null && e != null) {
            BtnSetClick.Invoke(ChatSessionType.TODO_TASK);
        }
    }

    private void scrollViewer_MouseWheel(object sender, MouseWheelEventArgs e) {
        //向下就return
        if (e.Delta < 0) return;
        if (load == false) return;

        //滚到最顶
        if (ScrollViewer.ScrollableHeight > 0 && ScrollViewer.VerticalOffset == 0) {

            _messagesList = MessageService.getInstance().getMessagesByPage(Constants.APPMSG_FLAG, lastTimestamp);
            LoadMessage(_messagesList);
            if (_messagesList.Count == 0) return;
            if (_messagesList[_messagesList.Count - 1].timestamp != null) {
                lastTimestamp = long.Parse(_messagesList[_messagesList.Count - 1].timestamp);
            }
            //scrollViewer1.ScrollToHome();
        }
    }
}
}
